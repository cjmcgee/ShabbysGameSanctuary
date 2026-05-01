using ChildhoodAdventure.RetroSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine.Collision;
using TileEngine.Components;
using TileEngine.Core;
using TileEngine.ECS;
using TileEngine.Rendering;

namespace ChildhoodAdventure.Scenes
{
    /// <summary>
    /// The outdoor neighbourhood: one tree-lined street with nine homes.
    /// The player can walk between houses, chat with neighbours outside,
    /// and step on any front door to enter that home.
    ///
    /// Street layout (tiles, 16 px each):
    ///   y  0- 3  : north grass
    ///   y  4-18  : house facades (roof overhead y=4-6, walls y=7-17, door y=18)
    ///   y 19     : north sidewalk
    ///   y 20-21  : road
    ///   y 22     : south sidewalk
    ///   y 23-37  : south grass / open space
    ///
    /// Houses left-to-right (all 7 tiles wide):
    ///   Chen | Devon | Jake&Emma | Thompson | Player | Sam | Santos | Petrov | Johnson
    /// </summary>
    public class NeighborhoodScene : Scene
    {
        // ── Tile GIDs ────────────────────────────────────────────────────────
        private const int T_GRASS    =  1;
        private const int T_ROAD     =  2;
        private const int T_SIDEWALK =  3;
        private const int T_BEIGE    =  4;  // Player's home
        private const int T_YELLOW   =  5;  // Sam
        private const int T_PINK     =  6;  // Santos
        private const int T_TEAL     =  7;  // Chen
        private const int T_GRAY     =  8;  // Thompson
        private const int T_BLUE     =  9;  // Petrov
        private const int T_GREEN    = 10;  // Jake & Emma
        private const int T_PURPLE   = 11;  // Devon
        private const int T_DOOR     = 12;
        private const int T_WINDOW   = 13;
        private const int T_BUSH     = 14;
        private const int T_ORANGE   = 15;  // Johnson

        // Map size
        private const int MapW = 82, MapH = 38;

        // House data: (houseId, startX, wallGid, doorTileX) — all at y=4, height=15, width=7
        private record HouseData(HouseId Id, int X, int WallGid, int DoorX);

        private static readonly HouseData[] _neighborHouses =
        {
            new(HouseId.Chen,        1,  T_TEAL,   4),
            new(HouseId.Devon,      10,  T_PURPLE, 13),
            new(HouseId.JakeAndEmma,19,  T_GREEN,  22),
            new(HouseId.Thompson,   28,  T_GRAY,   31),
            // x=37-43 is the player's home (beige) — handled separately, door at x=40
            new(HouseId.Sam,        46,  T_YELLOW, 49),
            new(HouseId.Santos,     55,  T_PINK,   58),
            new(HouseId.Petrov,     64,  T_BLUE,   67),
            new(HouseId.Johnson,    73,  T_ORANGE, 76),
        };

        private const int HouseStartY = 4, HouseH = 15, HouseDoorY = 18;

        private Entity? _player;
        private readonly List<(Entity Npc, Action TalkFn)> _npcTalks = new();
        private Tilemap? _tilemap;
        private KeyboardState _prevKeys;
        private bool _transitioning;

        // ── Scene load ───────────────────────────────────────────────────────

        protected override void OnLoad()
        {
            Name = "Neighbourhood";
            GameState.ActiveScene = GameState.SceneType.Neighborhood;
            var gd  = Engine.GraphicsDevice;
            var sys = RetroSystemRegistry.Current;

            // Each house has its own GID (4-15) so each can be a distinct system color.
            // We build one tileset entry per GID, mapping house GIDs to HouseExterior
            // tile art; the actual color comes from the house-specific palette passed
            // via accentColor (only the first three GIDs really differ in tile art).
            // For simplicity we pass Color.Transparent as accent — house tiles use
            // the system's built-in HouseExterior tile pattern, which is always solid.
            // The distinct house colors are supplied by passing them as TileType.Accent
            // tiles so the system can style them per its palette.

            // GID order mirrors the original hardcoded constants.
            // GIDs 4-15 are house exterior tiles; we use the system's HouseExterior art
            // tinted to each house's characteristic color (via Accent with that color).
            var houseColors = new Color[]
            {
                new Color(220, 220, 220),   //  4 white        (player home)
                new Color(220, 220,   0),   //  5 yellow       (Sam)
                new Color(220,   0, 140),   //  6 magenta      (Santos)
                new Color(  0, 220, 220),   //  7 cyan         (Chen)
                new Color(160, 160, 160),   //  8 light gray   (Thompson)
                new Color(  0,  80, 220),   //  9 blue         (Petrov)
                new Color( 80, 220,   0),   // 10 lime         (Jake&Emma)
                new Color(140,   0, 220),   // 11 violet       (Devon)
                new Color( 30,  12,   0),   // 12 near-black   (door)
                new Color(220, 220, 220),   // 13 (alignment)
                new Color(  0,  80,   0),   // 14 (unused)
                new Color(220, 100,   0),   // 15 orange       (Johnson)
            };

            // Build first slab: GIDs 1-3 (grass, road, sidewalk)
            var tilesetBase = sys.BuildTileset(gd, "hood_base", new[]
            {
                TileType.Grass,    // 1
                TileType.Road,     // 2
                TileType.Sidewalk, // 3
            }, firstGid: 1);

            // Build house GIDs 4-15 as Accent tiles with per-house colors.
            // We generate one texture strip with one 16px slot per house color.
            var houseTexture = BuildHouseTextureSlab(gd, sys, houseColors);
            var tilesetHouses = new Tileset("hood_houses", houseTexture, 16, 16, firstGid: 4);

            _tilemap = new Tilemap("hood", MapW, MapH, 16, 16)
            {
                BackgroundColor = Color.Black
            };
            _tilemap.AddTileset(tilesetBase);
            _tilemap.AddTileset(tilesetHouses);
            BuildNeighborhoodMap(_tilemap);

            Engine.CollisionSystem.SetTilemap(_tilemap);
            Engine.RenderSystem.TilemapRenderer.SetTilemap(_tilemap);
            Engine.RenderSystem.Camera.Bounds         = new Rectangle(0, 0, _tilemap.PixelWidth, _tilemap.PixelHeight);
            Engine.RenderSystem.Camera.MaxWorldVisible = sys.MaxZoomOutArea;
            Engine.RenderSystem.Camera.Zoom            = sys.DisplayScale;
            Engine.RenderSystem.LightingSystem.Enabled = false;

            // Load Yarn dialogue (no-op if already loaded from a prior scene)
            var yarnDir = Path.Combine(AppContext.BaseDirectory, "Dialogue");
            Engine.DialogueSystem.EnsureYarnLoaded(yarnDir);
            Engine.DialogueSystem.RegisterCommandHandler("flag",
                args => { if (args.Length > 0) GameState.SetFlag(args[0]); });

            // Player
            _player = SpawnPlayer(gd, GameState.NeighborhoodReturnPosition);

            // Outdoor NPCs
            SpawnOutdoorNpcs(gd);
        }

        // ── Texture helpers ───────────────────────────────────────────────────

        /// <summary>
        /// Builds a texture strip for house exterior tiles using the active system's
        /// HouseExterior pixel art, tinted per-house by providing each house color as
        /// the "accent" color to <see cref="RetroSystem.BuildTileset"/>.
        /// Returns one 16px-wide slot per color entry.
        /// </summary>
        private static Texture2D BuildHouseTextureSlab(
            GraphicsDevice gd, RetroSystem sys, Color[] houseColors)
        {
            const int tileSize = 16;
            int count   = houseColors.Length;
            var texture = new Texture2D(gd, tileSize * count, tileSize);
            var data    = new Color[tileSize * count * tileSize];

            for (int i = 0; i < count; i++)
            {
                // BuildTileset with a single Accent tile using this house's color
                var slab = sys.BuildTileset(gd, $"h{i}", new[] { TileType.Accent },
                    accentColor: houseColors[i]);

                // Copy the 16×16 tile from slab.Texture into our combined texture at offset i
                var slabData = new Color[tileSize * tileSize];
                slab.Texture.GetData(slabData);
                for (int ty = 0; ty < tileSize; ty++)
                    for (int tx = 0; tx < tileSize; tx++)
                        data[ty * (tileSize * count) + i * tileSize + tx] = slabData[ty * tileSize + tx];

                slab.Texture.Dispose();
            }

            texture.SetData(data);
            return texture;
        }

        // ── Map builder ──────────────────────────────────────────────────────

        private static void BuildNeighborhoodMap(Tilemap map)
        {
            var bg  = map.AddLayer("bg",  LayerType.Background);
            var mid = map.AddLayer("mid", LayerType.Midground);
            var col = map.AddLayer("col", LayerType.Collision);

            // Pure green grass everywhere — Adventure-style solid background
            FillRect(bg, 0, 0, MapW, MapH, T_GRASS);

            // Road strip (near-black) + sidewalk borders
            FillRect(bg, 0, 19, MapW, 1, T_SIDEWALK);
            FillRect(bg, 0, 20, MapW, 2, T_ROAD);
            FillRect(bg, 0, 22, MapW, 1, T_SIDEWALK);

            // Player's own house — solid white block, door at x=40
            PlaceHouse(mid, col, 37, HouseStartY, 7, HouseH, T_BEIGE, 40);

            // Neighbour houses — each a pure bold-colour rectangle
            foreach (var h in _neighborHouses)
                PlaceHouse(mid, col, h.X, HouseStartY, 7, HouseH, h.WallGid, h.DoorX);
        }

        // Adventure-style house: solid colour rectangle with one walkable door gap.
        // No overhead tricks, no windows — just a flat coloured wall like the Atari castles.
        private static void PlaceHouse(TileLayer mid, TileLayer col,
            int hx, int hy, int w, int h, int wallGid, int doorX)
        {
            for (int y = hy; y < hy + h; y++)
                for (int x = hx; x < hx + w; x++)
                {
                    mid.SetTile(x, y, wallGid);
                    col.SetTile(x, y, 1);
                }

            // Door: single walkable tile at bottom centre
            int doorY = hy + h - 1;
            mid.SetTile(doorX, doorY, T_DOOR);
            col.SetTile(doorX, doorY, 0);
        }

        private static void FillRect(TileLayer layer, int x, int y, int w, int h, int gid)
        {
            for (int dy = 0; dy < h; dy++)
                for (int dx = 0; dx < w; dx++)
                    layer.SetTile(x + dx, y + dy, gid);
        }

        // ── Entity spawners ──────────────────────────────────────────────────

        private Entity SpawnPlayer(GraphicsDevice gd, Vector2 pos)
        {
            var e = Engine.EntityWorld.CreateEntity("Player");
            Engine.EntityWorld.RegisterTag("player", e);
            e.AddComponent(new TransformComponent(pos) { MaxSpeed = 95f });
            e.AddComponent(new CollisionComponent(10, 6, new Vector2(-5, -3)));
            var playerSprite = SpriteFactory.BuildCharacter(gd, NpcAppearances.Player);
            playerSprite.Scale = 0.5f;
            e.AddComponent(new SpriteComponent { Sprite = playerSprite });
            Engine.RenderSystem.Camera.FollowTarget = pos;
            Engine.RenderSystem.Camera.FollowSpeed  = 7f;
            Engine.RenderSystem.Camera.CenterOn(pos);
            return e;
        }

        private Entity SpawnNpc(GraphicsDevice gd, string name, Vector2 pos, CharacterAppearance appearance, Action talkFn, float scale = 1f)
        {
            var e = Engine.EntityWorld.CreateEntity(name);
            e.AddComponent(new TransformComponent(pos));
            e.AddComponent(new CollisionComponent(10, 8, new Vector2(-5, -4)) { IsSolid = true });
            var sprite = SpriteFactory.BuildCharacter(gd, appearance);
            sprite.Scale = scale;
            e.AddComponent(new SpriteComponent { Sprite = sprite });
            _npcTalks.Add((e, talkFn));
            return e;
        }

        private void SpawnOutdoorNpcs(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Sam",   new Vector2(49 * 16 + 8, 21 * 16 + 8), NpcAppearances.Sam,
                () => Engine.DialogueSystem.StartYarnNode("Sam"),   scale: 0.5f);
            SpawnNpc(gd, "Lucia", new Vector2(58 * 16 + 8, 21 * 16 + 8), NpcAppearances.Lucia,
                () => Engine.DialogueSystem.StartYarnNode("Lucia"), scale: 0.5f);
            SpawnNpc(gd, "Nadia", new Vector2(67 * 16 + 8, 21 * 16 + 8), NpcAppearances.Nadia,
                () => Engine.DialogueSystem.StartYarnNode("Nadia"), scale: 0.5f);
        }

        // ── Update ────────────────────────────────────────────────────────────

        public override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState();

            bool wasDialogueActive = Engine.DialogueSystem.IsActive;

            HandleDialogueInput(keys);

            if (!Engine.DialogueSystem.IsActive && !_transitioning)
            {
                HandlePlayerMovement(keys, gameTime);
                if (!wasDialogueActive)
                    HandleInteraction(keys);
                CheckDoorTriggers();
            }

            _prevKeys = keys;
        }

        private void HandleDialogueInput(KeyboardState keys)
        {
            if (!Engine.DialogueSystem.IsActive) return;

            if (Engine.DialogueSystem.WaitingForChoice)
            {
                if (keys.IsKeyDown(Keys.Up)   && !_prevKeys.IsKeyDown(Keys.Up))
                    Engine.DialogueSystem.MoveChoiceSelection(-1);
                if (keys.IsKeyDown(Keys.Down) && !_prevKeys.IsKeyDown(Keys.Down))
                    Engine.DialogueSystem.MoveChoiceSelection(1);
                if (keys.IsKeyDown(Keys.E) && !_prevKeys.IsKeyDown(Keys.E))
                    Engine.DialogueSystem.SelectChoice(Engine.DialogueSystem.SelectedChoiceIndex);
            }
            else if (keys.IsKeyDown(Keys.E) && !_prevKeys.IsKeyDown(Keys.E))
            {
                Engine.DialogueSystem.Advance();
            }
        }

        private void HandlePlayerMovement(KeyboardState keys, GameTime gameTime)
        {
            if (_player == null) return;
            var t  = _player.GetComponent<TransformComponent>();
            var sc = _player.GetComponent<SpriteComponent>();
            if (t == null) return;

            var move = Vector2.Zero;
            if (keys.IsKeyDown(Keys.W) || keys.IsKeyDown(Keys.Up))    move.Y -= 1;
            if (keys.IsKeyDown(Keys.S) || keys.IsKeyDown(Keys.Down))  move.Y += 1;
            if (keys.IsKeyDown(Keys.A) || keys.IsKeyDown(Keys.Left))  move.X -= 1;
            if (keys.IsKeyDown(Keys.D) || keys.IsKeyDown(Keys.Right)) move.X += 1;

            if (move != Vector2.Zero)
            {
                var dir = Vector2.Normalize(move);
                t.Velocity = dir * t.MaxSpeed;
                t.Facing   = dir;
                sc?.Play("walk");
            }
            else
            {
                t.Velocity = Vector2.Zero;
                sc?.Play("idle");
            }

            Engine.CollisionSystem.MoveAndSlide(_player, (float)gameTime.ElapsedGameTime.TotalSeconds);
            t.Velocity = Vector2.Zero;
            Engine.RenderSystem.Camera.FollowTarget = t.Position;
        }

        private void HandleInteraction(KeyboardState keys)
        {
            if (!keys.IsKeyDown(Keys.E) || _prevKeys.IsKeyDown(Keys.E)) return;
            if (_player == null) return;

            var pt = _player.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;

            foreach (var (npc, talkFn) in _npcTalks)
            {
                var nt = npc.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;
                if (Vector2.Distance(pt, nt) < 32f)
                {
                    talkFn();
                    return;
                }
            }
        }

        private void CheckDoorTriggers()
        {
            if (_player == null) return;
            var pos = _player.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;

            float pTileX = pos.X / 16f;
            float pTileY = pos.Y / 16f;

            // Must be at approximately door row to trigger entry
            if (pTileY < HouseDoorY - 0.5f || pTileY > HouseDoorY + 0.5f) return;

            // Player's own home
            if (MathF.Abs(pTileX - 40f) < 0.6f)
            {
                _transitioning = true;
                GameState.PlayerSpawnPosition = new Vector2(12 * 16 + 8, 15 * 16 + 8);
                Engine.LoadScene(new HomeInteriorScene());
                return;
            }

            // Neighbour houses
            foreach (var h in _neighborHouses)
            {
                if (MathF.Abs(pTileX - h.DoorX) < 0.6f)
                {
                    _transitioning = true;
                    GameState.TargetInterior = h.Id;
                    GameState.PlayerSpawnPosition = new Vector2(11 * 16 + 8, 13 * 16 + 8);
                    GameState.NeighborhoodReturnPosition = new Vector2(h.DoorX * 16 + 8, 19 * 16 + 8);
                    Engine.LoadScene(new NeighborInteriorScene());
                    return;
                }
            }
        }

        // ── Draw ──────────────────────────────────────────────────────────────

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Engine.RenderSystem.DrawScene(Engine.EntityWorld, gameTime, Color.Black);
            DrawUI(spriteBatch);
        }

        private Texture2D? _pixel;

        private void DrawUI(SpriteBatch spriteBatch)
        {
            Engine.RenderSystem.BeginUI();
            var sb = Engine.RenderSystem.SpriteBatch;

            if (Engine.DialogueSystem.IsActive)
                DrawDialogueBox(sb);

            Engine.RenderSystem.EndUI();
        }

        private void DrawDialogueBox(SpriteBatch sb)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(Engine.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }

            var vp    = Engine.GraphicsDevice.Viewport;
            var font  = Engine.RenderSystem.Font;
            const float scale = 2f;
            float lineH = PixelFont.CharH * scale;
            int boxH = 110, boxY = vp.Height - boxH - 8;
            int textX = 20, textMaxW = vp.Width - 40;

            sb.Draw(_pixel, new Rectangle(8, boxY, vp.Width - 16, boxH), new Color(0, 0, 0, 215));
            sb.Draw(_pixel, new Rectangle(8, boxY, vp.Width - 16, 2), new Color(120, 200, 120));
            sb.Draw(_pixel, new Rectangle(8, boxY + boxH - 2, vp.Width - 16, 2), new Color(120, 200, 120));
            sb.Draw(_pixel, new Rectangle(8, boxY, 2, boxH), new Color(120, 200, 120));
            sb.Draw(_pixel, new Rectangle(vp.Width - 10, boxY, 2, boxH), new Color(120, 200, 120));

            var line = Engine.DialogueSystem.CurrentLine;
            if (line == null) return;

            float cy = boxY + 8;
            if (!string.IsNullOrEmpty(line.Speaker))
            {
                font.DrawText(sb, line.Speaker, new Vector2(textX, cy), Color.LightGreen, scale);
                cy += lineH + 2;
            }

            float bodyH = font.DrawWrappedText(sb, Engine.DialogueSystem.DisplayedText,
                new Vector2(textX, cy), Color.White, textMaxW, scale);
            float afterBody = cy + bodyH + 4;

            if (Engine.DialogueSystem.WaitingForChoice)
            {
                var choices = Engine.DialogueSystem.VisibleChoices;
                for (int i = 0; i < choices.Length; i++)
                {
                    bool sel     = i == Engine.DialogueSystem.SelectedChoiceIndex;
                    bool enabled = choices[i].EnabledCondition?.Invoke() ?? true;
                    Color color  = !enabled ? Color.DarkGray
                                 : sel      ? Color.Yellow
                                            : Color.LightGray;
                    font.DrawText(sb, (sel && enabled ? "> " : "  ") + choices[i].Text,
                        new Vector2(textX, afterBody + i * lineH), color, scale);
                }
            }
            else if (Engine.DialogueSystem.IsTextComplete)
            {
                if ((int)(Engine.PlayTime.TotalSeconds * 2) % 2 == 0)
                    font.DrawText(sb, "[ E ]",
                        new Vector2(vp.Width - 68, boxY + boxH - lineH - 6), Color.Gray, scale);
            }
        }

        protected override void OnUnload()
        {
            Engine.RenderSystem.LightingSystem.ClearLights();
        }
    }
}
