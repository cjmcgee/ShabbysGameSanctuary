using ChildhoodAdventure.RetroSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine.Collision;
using TileEngine.Components;
using TileEngine.Core;
using TileEngine.ECS;
using TileEngine.Gameplay;
using TileEngine.Rendering;

namespace ChildhoodAdventure.Scenes
{
    /// <summary>
    /// The player's home — where the game begins.
    /// Open-plan layout: living room (left) and kitchen/dining (right).
    /// NPCs: Mom (Karen) in the kitchen, Dad (Tom) in the living room,
    /// Jamie (younger sibling) on the living room floor.
    /// Exit: walk to the front door at the bottom-centre of the map.
    /// </summary>
    public class HomeInteriorScene : Scene
    {
        // ── Tile GIDs (1-based) ──────────────────────────────────────────────
        private const int T_WOOD     = 1;   // warm wood floor
        private const int T_CARPET   = 2;   // red-orange carpet (living room)
        private const int T_KITCHEN  = 3;   // pale cream kitchen tile
        private const int T_WALL     = 4;   // plaster wall
        private const int T_DOOR     = 5;   // dark-wood front door (walkable)
        private const int T_FURN     = 6;   // dark furniture (sofa, table, bookshelf)
        private const int T_COUNTER  = 7;   // light kitchen counter
        private const int T_WINDOW   = 8;   // window (light blue)

        // Map dimensions (tiles)
        private const int MapW = 24, MapH = 18;

        private Entity? _player;
        private readonly List<(Entity Npc, Action TalkFn)> _npcTalks = new();
        private Tilemap? _tilemap;
        private KeyboardState _prevKeys;
        private bool _transitioning;

        // ── Scene load ───────────────────────────────────────────────────────

        protected override void OnLoad()
        {
            Name = "Home";
            GameState.ActiveScene = GameState.SceneType.Home;
            var gd  = Engine.GraphicsDevice;
            var sys = RetroSystemRegistry.Current;

            // Build tileset from the active retro system's pixel art.
            // GIDs 1-8 map to the TileTypes below in order.
            var tileset = sys.BuildTileset(gd, "home", new[]
            {
                TileType.WoodFloor,   // 1
                TileType.Carpet,      // 2
                TileType.KitchenTile, // 3
                TileType.Wall,        // 4
                TileType.Door,        // 5
                TileType.Furniture,   // 6
                TileType.Counter,     // 7
                TileType.Window,      // 8
            });

            _tilemap = new Tilemap("home", MapW, MapH, 16, 16)
            {
                BackgroundColor = Color.Black
            };
            _tilemap.AddTileset(tileset);
            BuildHomeMap(_tilemap);

            Engine.CollisionSystem.SetTilemap(_tilemap);
            Engine.RenderSystem.TilemapRenderer.SetTilemap(_tilemap);
            Engine.RenderSystem.Camera.Bounds = new Rectangle(0, 0, _tilemap.PixelWidth, _tilemap.PixelHeight);
            Engine.RenderSystem.Camera.Zoom   = sys.DisplayScale;
            Engine.RenderSystem.LightingSystem.Enabled = false;

            // Player
            _player = SpawnPlayer(gd, GameState.PlayerSpawnPosition);

            // NPCs
            SpawnDad(gd);
            SpawnMom(gd);
            SpawnJamie(gd);
        }

        // ── Map builder ──────────────────────────────────────────────────────

        private static void BuildHomeMap(Tilemap map)
        {
            var bg  = map.AddLayer("bg",  LayerType.Background);
            var mid = map.AddLayer("mid", LayerType.Midground);
            var col = map.AddLayer("col", LayerType.Collision);

            // Background floors
            FillRect(bg, 1, 1, 22, 16, T_WOOD);       // all interior wood
            FillRect(bg, 1, 1,  9, 13, T_CARPET);     // living room carpet
            FillRect(bg, 12,1, 11, 13, T_KITCHEN);    // kitchen tile

            // Border walls
            FillRow(mid, col, 0,       T_WALL);
            FillRow(mid, col, MapH-1,  T_WALL);
            FillCol(mid, col, 0,       T_WALL);
            FillCol(mid, col, MapW-1,  T_WALL);

            // Solid walls — no window cutouts (Adventure rectangles are unbroken)

            // Partial vertical divider between living room and kitchen (y=1-7)
            for (int y = 1; y <= 7; y++) { mid.SetTile(11, y, T_WALL); col.SetTile(11, y, 1); }
            // Opening at y=8-10 (archway)

            // Front door: two walkable tiles in the bottom wall (exit to neighbourhood)
            mid.SetTile(11, MapH-1, T_DOOR); col.SetTile(11, MapH-1, 0);
            mid.SetTile(12, MapH-1, T_DOOR); col.SetTile(12, MapH-1, 0);

            // ── Furniture (midground, blocked in collision) ──────────────────

            // TV / bookshelf unit top-left
            PlaceFurniture(mid, col, 1, 1, 3, 1);
            // Sofa (3 tiles wide, facing down)
            PlaceFurniture(mid, col, 2, 3, 3, 1);
            // Coffee table
            PlaceFurniture(mid, col, 3, 5, 1, 1);
            // Bookshelf on left wall
            PlaceFurniture(mid, col, 1, 7, 1, 2);
            // Plant in corner
            mid.SetTile(9, 9, T_FURN); col.SetTile(9, 9, 1);

            // Kitchen counters (top row, east side)
            PlaceFurniture(mid, col, 13, 1, 8, 1); // counter top row
            PlaceFurniture(mid, col, 22, 1, 1, 3); // fridge/appliance on right wall
            // Stove break at x=19-20 col=0 (the counter has gaps for NPC)
            col.SetTile(19, 1, 0); col.SetTile(20, 1, 0);

            // Dining table + chairs
            PlaceFurniture(mid, col, 14, 6, 3, 2);
            // Chairs around table (decorative, not blocked)
            mid.SetTile(13, 6, T_FURN); mid.SetTile(13, 7, T_FURN);
            mid.SetTile(18, 6, T_FURN); mid.SetTile(18, 7, T_FURN);
        }

        private static void FillRect(TileLayer layer, int x, int y, int w, int h, int gid)
        {
            for (int dy = 0; dy < h; dy++)
                for (int dx = 0; dx < w; dx++)
                    layer.SetTile(x + dx, y + dy, gid);
        }

        private static void FillRow(TileLayer mid, TileLayer col, int y, int gid)
        {
            for (int x = 0; x < MapW; x++) { mid.SetTile(x, y, gid); col.SetTile(x, y, 1); }
        }

        private static void FillCol(TileLayer mid, TileLayer col, int x, int gid)
        {
            for (int y = 0; y < MapH; y++) { mid.SetTile(x, y, gid); col.SetTile(x, y, 1); }
        }

        private static void PlaceFurniture(TileLayer mid, TileLayer col, int x, int y, int w, int h)
        {
            for (int dy = 0; dy < h; dy++)
                for (int dx = 0; dx < w; dx++)
                {
                    mid.SetTile(x + dx, y + dy, T_FURN);
                    col.SetTile(x + dx, y + dy, 1);
                }
        }

        // ── Entity spawners ──────────────────────────────────────────────────

        private Entity SpawnPlayer(GraphicsDevice gd, Vector2 pos)
        {
            var e = Engine.EntityWorld.CreateEntity("Player");
            Engine.EntityWorld.RegisterTag("player", e);
            e.AddComponent(new TransformComponent(pos) { MaxSpeed = 90f });
            e.AddComponent(new CollisionComponent(10, 6, new Vector2(-5, -3)));
            var sc = e.AddComponent(new SpriteComponent
            {
                Sprite = SpriteFactory.BuildCharacter(gd, NpcAppearances.Player)
            });
            Engine.RenderSystem.Camera.FollowTarget = pos;
            Engine.RenderSystem.Camera.FollowSpeed  = 8f;
            return e;
        }

        private Entity SpawnNpc(GraphicsDevice gd, string name, Vector2 pos, CharacterAppearance appearance, Action talkFn)
        {
            var e = Engine.EntityWorld.CreateEntity(name);
            e.AddComponent(new TransformComponent(pos));
            e.AddComponent(new CollisionComponent(10, 8, new Vector2(-5, -4)) { IsSolid = true });
            e.AddComponent(new SpriteComponent { Sprite = SpriteFactory.BuildCharacter(gd, appearance) });
            _npcTalks.Add((e, talkFn));
            return e;
        }

        private void SpawnDad(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Dad", new Vector2(5 * 16 + 8, 6 * 16 + 8), NpcAppearances.Dad, TalkToDad);
        }

        private void SpawnMom(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Mom", new Vector2(17 * 16 + 8, 3 * 16 + 8), NpcAppearances.Mom, TalkToMom);
        }

        private void SpawnJamie(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Jamie", new Vector2(3 * 16 + 8, 11 * 16 + 8), NpcAppearances.Jamie, TalkToJamie);
        }

        // ── Dialogue ─────────────────────────────────────────────────────────

        private void TalkToDad()
        {
            if (!GameState.HasFlag("talked_dad"))
            {
                GameState.SetFlag("talked_dad");
                Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                {
                    new("Dad", "Hey champ! Ready for a big day? The whole neighbourhood is out there waiting."),
                    new("Dad", "Remember to be kind to the neighbours, okay? Especially Mr. Thompson — he's been having a rough time lately.",
                        choices: new[]
                        {
                            new DialogueChoice("What happened to Mr. Thompson?", onSelected: () =>
                                Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                {
                                    new("Dad", "His wife Margaret passed away last winter. He's still getting used to being on his own."),
                                    new("Dad", "Sometimes people just need to know someone's thinking of them. A quick hello goes a long way."),
                                })),
                            new DialogueChoice("I will, Dad.", onSelected: () =>
                                Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                {
                                    new("Dad", "That's my kid. Now go have fun — and be home before dinner!"),
                                })),
                        }),
                });
            }
            else
            {
                Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                {
                    new("Dad", "Remember — home before dinner!"),
                });
            }
        }

        private void TalkToMom()
        {
            if (!GameState.HasFlag("talked_mom"))
            {
                GameState.SetFlag("talked_mom");
                Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                {
                    new("Mom", "Good morning, sweetheart! I just put some cookies in the oven — they'll be ready when you get back."),
                    new("Mom", "Oh! The Santos family moved in down the street — little Lucia would love a playmate. Maybe go say hello?",
                        choices: new[]
                        {
                            new DialogueChoice("I'll go say hi to Lucia.", onSelected: () =>
                                Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                {
                                    new("Mom", "Wonderful! Maria works so hard for that girl. Be kind to them both."),
                                })),
                            new DialogueChoice("Who else should I visit?", onSelected: () =>
                                Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                {
                                    new("Mom", "Well — Mrs. Chen always loves visitors. And the new family at the end of the block, the Petrovs, just arrived from very far away."),
                                    new("Mom", "International moves are so hard. A friendly face would mean the world to them."),
                                })),
                        }),
                });
            }
            else
            {
                Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                {
                    new("Mom", "The cookies will be ready soon. Don't be too long!"),
                });
            }
        }

        private void TalkToJamie()
        {
            if (!GameState.HasFlag("talked_jamie"))
            {
                GameState.SetFlag("talked_jamie");
                Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                {
                    new("Jamie", "Go away! I'm building something VERY important.",
                        choices: new[]
                        {
                            new DialogueChoice("What are you building?", onSelected: () =>
                                Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                {
                                    new("Jamie", "A FORTRESS. For my stuffed animals. They need protection from... things."),
                                    new("Jamie", "You can look but DON'T TOUCH anything."),
                                })),
                            new DialogueChoice("Okay, bye.", onSelected: () =>
                                Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                {
                                    new("Jamie", "BYE."),
                                })),
                        }),
                });
            }
            else
            {
                Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                {
                    new("Jamie", "Hey... do you know the new kid? Nadia? I saw her from my window."),
                    new("Jamie", "She looks kind of sad. You should go say hi to her."),
                });
            }
        }

        // ── Update ────────────────────────────────────────────────────────────

        public override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState();

            // Snapshot dialogue state BEFORE processing — prevents the E press that
            // closes a dialogue from immediately re-triggering an interaction in the
            // same frame.
            bool wasDialogueActive = Engine.DialogueSystem.IsActive;

            HandleDialogueInput(keys);

            if (!Engine.DialogueSystem.IsActive && !_transitioning)
            {
                HandlePlayerMovement(keys, gameTime);
                if (!wasDialogueActive)
                    HandleInteraction(keys);
                CheckExitTrigger();
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
            var t = _player.GetComponent<TransformComponent>();
            var sc = _player.GetComponent<SpriteComponent>();
            if (t == null) return;

            var move = Vector2.Zero;
            if (keys.IsKeyDown(Keys.W) || keys.IsKeyDown(Keys.Up))    move.Y -= 1;
            if (keys.IsKeyDown(Keys.S) || keys.IsKeyDown(Keys.Down))  move.Y += 1;
            if (keys.IsKeyDown(Keys.A) || keys.IsKeyDown(Keys.Left))  move.X -= 1;
            if (keys.IsKeyDown(Keys.D) || keys.IsKeyDown(Keys.Right)) move.X += 1;

            if (move != Vector2.Zero)
            {
                t.Velocity = Vector2.Normalize(move) * t.MaxSpeed;
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
                if (Vector2.Distance(pt, nt) < 30f)
                {
                    talkFn();
                    return;
                }
            }
        }

        private void CheckExitTrigger()
        {
            if (_player == null) return;
            var pos = _player.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;

            // Exit when player reaches the front door tiles (bottom-centre of map).
            if (pos.Y >= (MapH - 2) * 16 - 2 && pos.X > 10 * 16 && pos.X < 13 * 16 + 16)
            {
                _transitioning = true;
                GameState.NeighborhoodReturnPosition = new Vector2(40 * 16 + 8, 19 * 16 + 8);
                Engine.LoadScene(new NeighborhoodScene());
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
            sb.Draw(_pixel, new Rectangle(8, boxY, vp.Width - 16, 2), new Color(200, 180, 100));
            sb.Draw(_pixel, new Rectangle(8, boxY + boxH - 2, vp.Width - 16, 2), new Color(200, 180, 100));
            sb.Draw(_pixel, new Rectangle(8, boxY, 2, boxH), new Color(200, 180, 100));
            sb.Draw(_pixel, new Rectangle(vp.Width - 10, boxY, 2, boxH), new Color(200, 180, 100));

            var line = Engine.DialogueSystem.CurrentLine;
            if (line == null) return;

            float cy = boxY + 8;
            if (!string.IsNullOrEmpty(line.Speaker))
            {
                font.DrawText(sb, line.Speaker, new Vector2(textX, cy), Color.Yellow, scale);
                cy += lineH + 2;
            }

            float bodyH = font.DrawWrappedText(sb, Engine.DialogueSystem.DisplayedText,
                new Vector2(textX, cy), Color.White, textMaxW, scale);
            float afterBody = cy + bodyH + 4;

            if (Engine.DialogueSystem.WaitingForChoice && line.Choices != null)
            {
                for (int i = 0; i < line.Choices.Length; i++)
                {
                    bool sel = i == Engine.DialogueSystem.SelectedChoiceIndex;
                    font.DrawText(sb, (sel ? "> " : "  ") + line.Choices[i].Text,
                        new Vector2(textX, afterBody + i * lineH),
                        sel ? Color.Yellow : Color.LightGray, scale);
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
