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
    /// Generic neighbour home interior, parameterised by GameState.TargetInterior.
    /// Each house has a unique colour scheme, furniture layout, and set of NPCs.
    ///
    /// Shared map dimensions: 22×16 tiles (352×256 px).
    /// All interiors share the same tile GIDs; only colours differ per house.
    ///
    /// Families:
    ///   Chen      — Mr. Chen (chess, wisdom) + Mrs. Chen (cookies, warmth)
    ///   Devon     — Devon, a stressed-but-thoughtful college student
    ///   JakeAndEmma — newlyweds Jake + Emma (board games, laughter)
    ///   Thompson  — Vic Thompson, widower (gruff exterior, soft heart)
    ///   Santos    — Maria Santos (single mum, hardworking) + baby Carlos
    ///   Petrov    — Mr. &amp; Mrs. Petrov (recent immigrants, limited English, huge warmth)
    ///   Sam       — Sam is outside; Sam's mum Linda is home
    ///   Johnson   — DeShonda + Destiny (guitar prodigy) + Tyler (moody teen)
    /// </summary>
    public class NeighborInteriorScene : Scene
    {
        // ── Shared tile GIDs ─────────────────────────────────────────────────
        private const int T_FLOOR   = 1;
        private const int T_WALL    = 2;
        private const int T_FURN    = 3;  // dark furniture
        private const int T_COUNTER = 4;  // light counter / shelf
        private const int T_DOOR    = 5;
        private const int T_ACCENT  = 6;  // carpet / rug accent
        private const int T_WINDOW  = 7;
        private const int T_BOOK    = 8;  // bookshelf
        private const int T_KITCHEN = 9;  // kitchen tile floor
        private const int T_PLANT   = 10; // houseplant

        private const int MapW = 22, MapH = 16;

        private Entity? _player;
        private readonly List<(Entity Npc, Action TalkFn)> _npcTalks = new();
        private Tilemap? _tilemap;
        private KeyboardState _prevKeys;
        private bool _transitioning;

        // ── Scene load ───────────────────────────────────────────────────────

        protected override void OnLoad()
        {
            Name = $"{GameState.TargetInterior} Home";
            GameState.ActiveScene = GameState.SceneType.NeighborInterior;
            var gd  = Engine.GraphicsDevice;
            var id  = GameState.TargetInterior ?? HouseId.Chen;
            var sys = RetroSystemRegistry.Current;

            // Each house uses the same tile GIDs but a unique accent (carpet/rug) color
            // that matches the house exterior. The active RetroSystem styles all tiles
            // in its own pixel-art palette; only the accent slot gets the house color.
            Color accentColor = id switch
            {
                HouseId.Chen        => new Color(  0, 220, 220),  // cyan
                HouseId.Devon       => new Color(140,   0, 220),  // violet
                HouseId.JakeAndEmma => new Color( 80, 220,   0),  // lime
                HouseId.Thompson    => new Color(160, 160, 160),  // gray
                HouseId.Santos      => new Color(220,   0, 140),  // magenta
                HouseId.Petrov      => new Color(  0,  80, 220),  // blue
                HouseId.Sam         => new Color(220, 220,   0),  // yellow
                HouseId.Johnson     => new Color(220, 100,   0),  // orange
                _                   => new Color(220, 220, 220),  // white
            };

            var tileset = sys.BuildTileset(gd, "interior", new[]
            {
                TileType.WoodFloor,   //  1
                TileType.Wall,        //  2
                TileType.Furniture,   //  3
                TileType.Counter,     //  4
                TileType.Door,        //  5
                TileType.Accent,      //  6 (house-specific carpet)
                TileType.Window,      //  7
                TileType.Bookshelf,   //  8
                TileType.KitchenTile, //  9
                TileType.Plant,       // 10
            }, accentColor: accentColor, firstGid: 1);

            _tilemap = new Tilemap("interior", MapW, MapH, 16, 16)
            {
                BackgroundColor = Color.Black
            };
            _tilemap.AddTileset(tileset);
            BuildInteriorMap(_tilemap, id);

            Engine.CollisionSystem.SetTilemap(_tilemap);
            Engine.RenderSystem.TilemapRenderer.SetTilemap(_tilemap);
            Engine.RenderSystem.Camera.Bounds         = new Rectangle(0, 0, _tilemap.PixelWidth, _tilemap.PixelHeight);
            Engine.RenderSystem.Camera.MaxWorldVisible = sys.MaxZoomOutArea;
            Engine.RenderSystem.Camera.Zoom            = sys.DisplayScale;
            Engine.RenderSystem.LightingSystem.Enabled = false;

            // Load Yarn dialogue from the embedded bundle (no-op after first call)
            Engine.DialogueSystem.EnsureYarnLoaded(GetType().Assembly, AdventureGame.DialogueBundleResource);
            Engine.DialogueSystem.RegisterCommandHandler("flag",
                args => { if (args.Length > 0) GameState.SetFlag(args[0]); });

            _player = SpawnPlayer(gd, GameState.PlayerSpawnPosition);
            SpawnNpcs(gd, id);
        }

        // ── Map builder ──────────────────────────────────────────────────────

        private static void BuildInteriorMap(Tilemap map, HouseId id)
        {
            var bg  = map.AddLayer("bg",  LayerType.Background);
            var mid = map.AddLayer("mid", LayerType.Midground);
            var col = map.AddLayer("col", LayerType.Collision);

            // Wood floor everywhere inside
            FillRect(bg, 1, 1, MapW - 2, MapH - 2, T_FLOOR);

            // Kitchen tile in back-right corner
            FillRect(bg, 13, 1, 8, 6, T_KITCHEN);

            // Rug/accent in living area
            FillRect(bg, 2, 3, 8, 5, T_ACCENT);

            // Border walls
            FillRowMid(mid, col, 0);
            FillRowMid(mid, col, MapH - 1);
            FillColMid(mid, col, 0);
            FillColMid(mid, col, MapW - 1);

            // Windows on the top wall — visual only, wall collision stays solid
            mid.SetTile(4,  0, T_WINDOW);
            mid.SetTile(9,  0, T_WINDOW);
            mid.SetTile(16, 0, T_WINDOW);

            // Front door: two walkable tiles in bottom wall
            mid.SetTile(10, MapH - 1, T_DOOR); col.SetTile(10, MapH - 1, 0);
            mid.SetTile(11, MapH - 1, T_DOOR); col.SetTile(11, MapH - 1, 0);

            // House-specific furniture
            switch (id)
            {
                case HouseId.Chen:        BuildChenLayout(mid, col);        break;
                case HouseId.Devon:       BuildDevonLayout(mid, col);       break;
                case HouseId.JakeAndEmma: BuildJakeEmmaLayout(mid, col);    break;
                case HouseId.Thompson:    BuildThompsonLayout(mid, col);    break;
                case HouseId.Santos:      BuildSantosLayout(mid, col);      break;
                case HouseId.Petrov:      BuildPetrovLayout(mid, col);      break;
                case HouseId.Sam:         BuildSamLayout(mid, col);         break;
                case HouseId.Johnson:     BuildJohnsonLayout(mid, col);     break;
            }
        }

        // Chen: bookshelf-heavy, chess table, very tidy
        private static void BuildChenLayout(TileLayer mid, TileLayer col)
        {
            PlaceFurn(mid, col, 1, 1, 4, 1); // bookshelf row
            PlaceFurn(mid, col, 1, 2, 1, 3); // left bookshelf column
            Furn(mid, col, 5, 4);            // chess table
            Furn(mid, col, 5, 5);
            PlaceFurn(mid, col, 13, 1, 7, 1); // kitchen counter
            Furn(mid, col, 20, 1); Furn(mid, col, 20, 2); // fridge corner
            Furn(mid, col, 15, 5); Furn(mid, col, 16, 5); // dining table
            Furn(mid, col, 9, 8); mid.SetTile(9, 8, T_PLANT); // plant
        }

        // Devon: sparse, stacks of books, study desk
        private static void BuildDevonLayout(TileLayer mid, TileLayer col)
        {
            Furn(mid, col, 2, 1); Furn(mid, col, 3, 1); // desk
            PlaceFurn(mid, col, 1, 2, 2, 1); // bookstack
            PlaceFurn(mid, col, 6, 1, 3, 1); // more books (scattered)
            Furn(mid, col, 2, 4); // chair at desk
            PlaceFurn(mid, col, 13, 1, 7, 1); // kitchen counter
            Furn(mid, col, 20, 1); // fridge
            Furn(mid, col, 1, 7); Furn(mid, col, 1, 8); // bookshelf side
            mid.SetTile(10, 8, T_PLANT); // sad little plant
        }

        // Jake & Emma: colourful, game boxes, comfortable couch
        private static void BuildJakeEmmaLayout(TileLayer mid, TileLayer col)
        {
            PlaceFurn(mid, col, 1, 1, 2, 1); // game shelf A
            PlaceFurn(mid, col, 4, 1, 3, 1); // game shelf B
            PlaceFurn(mid, col, 2, 3, 4, 1); // sofa
            Furn(mid, col, 4, 5);             // coffee table
            PlaceFurn(mid, col, 13, 1, 7, 1); // kitchen counter
            Furn(mid, col, 20, 1); Furn(mid, col, 20, 2); // fridge
            Furn(mid, col, 15, 5); Furn(mid, col, 16, 5); // table
            mid.SetTile(8, 2, T_PLANT); mid.SetTile(8, 3, T_PLANT); // twin plants
        }

        // Thompson: sparse, worn armchair, memorial shelf
        private static void BuildThompsonLayout(TileLayer mid, TileLayer col)
        {
            Furn(mid, col, 3, 3); // armchair
            Furn(mid, col, 4, 3);
            Furn(mid, col, 3, 5); // footstool / side table
            PlaceFurn(mid, col, 1, 1, 3, 1); // memorial shelf / mantle
            PlaceFurn(mid, col, 13, 1, 7, 1); // kitchen counter
            Furn(mid, col, 20, 1); // fridge
            Furn(mid, col, 15, 4); // small table
            mid.SetTile(9, 1, T_PLANT); // one lonely plant, watered
        }

        // Santos: warm kitchen, child's drawings on walls (decorative books), lively
        private static void BuildSantosLayout(TileLayer mid, TileLayer col)
        {
            PlaceFurn(mid, col, 2, 3, 3, 1); // sofa
            Furn(mid, col, 3, 5);             // coffee table
            PlaceFurn(mid, col, 13, 1, 7, 1); // kitchen counter
            Furn(mid, col, 20, 1); Furn(mid, col, 20, 2);
            PlaceFurn(mid, col, 14, 5, 3, 2); // dining table + chairs
            mid.SetTile(1, 2, T_PLANT);        // plant by the door
            mid.SetTile(10, 2, T_BOOK);        // drawings on wall (bookshelf stand-in)
            mid.SetTile(11, 2, T_BOOK);        col.SetTile(10,2,1); col.SetTile(11,2,1);
        }

        // Petrov: minimal (just moved), a few boxes, tea set on table
        private static void BuildPetrovLayout(TileLayer mid, TileLayer col)
        {
            Furn(mid, col, 2, 3); Furn(mid, col, 3, 3); // small couch
            Furn(mid, col, 3, 5); // tea table
            // "Boxes" represented as furniture clusters
            Furn(mid, col, 7, 2); Furn(mid, col, 8, 2);
            Furn(mid, col, 7, 3);
            PlaceFurn(mid, col, 13, 1, 7, 1); // kitchen counter
            Furn(mid, col, 20, 1); Furn(mid, col, 20, 2);
            Furn(mid, col, 15, 5); // small kitchen table
            mid.SetTile(1, 1, T_PLANT); // a plant from the old home
        }

        // Sam: tidy, kids' stuff everywhere, cosy
        private static void BuildSamLayout(TileLayer mid, TileLayer col)
        {
            PlaceFurn(mid, col, 2, 3, 3, 1); // sofa
            Furn(mid, col, 3, 5);
            PlaceFurn(mid, col, 1, 1, 3, 1); // toy shelf
            PlaceFurn(mid, col, 5, 1, 2, 1); // more shelves
            PlaceFurn(mid, col, 13, 1, 7, 1); // kitchen counter
            Furn(mid, col, 20, 1); Furn(mid, col, 20, 2);
            Furn(mid, col, 15, 5); Furn(mid, col, 16, 5);
            mid.SetTile(9, 2, T_PLANT);
        }

        // Johnson: larger family feel, guitar stand, comfortable sectional
        private static void BuildJohnsonLayout(TileLayer mid, TileLayer col)
        {
            PlaceFurn(mid, col, 1, 3, 4, 1); // large sofa
            PlaceFurn(mid, col, 1, 4, 1, 2); // sofa arm
            Furn(mid, col, 4, 5);             // coffee table
            Furn(mid, col, 7, 2);             // guitar stand (mid layer, not blocked)
            col.SetTile(7, 2, 0);             // walkable (can walk past guitar)
            PlaceFurn(mid, col, 1, 1, 2, 1); // TV unit
            PlaceFurn(mid, col, 13, 1, 7, 1);
            Furn(mid, col, 20, 1); Furn(mid, col, 20, 2);
            PlaceFurn(mid, col, 14, 5, 4, 2);
            mid.SetTile(10, 1, T_PLANT); mid.SetTile(10, 2, T_PLANT);
        }

        // ── Furniture helpers ─────────────────────────────────────────────────

        private static void PlaceFurn(TileLayer mid, TileLayer col, int x, int y, int w, int h)
        {
            for (int dy = 0; dy < h; dy++)
                for (int dx = 0; dx < w; dx++)
                    Furn(mid, col, x + dx, y + dy);
        }

        private static void Furn(TileLayer mid, TileLayer col, int x, int y)
        {
            mid.SetTile(x, y, T_FURN);
            col.SetTile(x, y, 1);
        }

        private static void FillRect(TileLayer layer, int x, int y, int w, int h, int gid)
        {
            for (int dy = 0; dy < h; dy++)
                for (int dx = 0; dx < w; dx++)
                    layer.SetTile(x + dx, y + dy, gid);
        }

        private static void FillRowMid(TileLayer mid, TileLayer col, int y)
        {
            for (int x = 0; x < MapW; x++) { mid.SetTile(x, y, T_WALL); col.SetTile(x, y, 1); }
        }

        private static void FillColMid(TileLayer mid, TileLayer col, int x)
        {
            for (int y = 0; y < MapH; y++) { mid.SetTile(x, y, T_WALL); col.SetTile(x, y, 1); }
        }

        // ── Entity spawners ──────────────────────────────────────────────────

        private Entity SpawnPlayer(GraphicsDevice gd, Vector2 pos)
        {
            var e = Engine.EntityWorld.CreateEntity("Player");
            Engine.EntityWorld.RegisterTag("player", e);
            e.AddComponent(new TransformComponent(pos) { MaxSpeed = 90f });
            e.AddComponent(new CollisionComponent(10, 6, new Vector2(-5, -3)));
            var playerSprite = SpriteFactory.BuildCharacter(gd, NpcAppearances.Player);
            playerSprite.Scale = 0.5f;
            e.AddComponent(new SpriteComponent { Sprite = playerSprite });
            Engine.RenderSystem.Camera.FollowTarget = pos;
            Engine.RenderSystem.Camera.FollowSpeed  = 8f;
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

        private void SpawnNpcs(GraphicsDevice gd, HouseId id)
        {
            switch (id)
            {
                case HouseId.Chen:        SpawnChenNpcs(gd);        break;
                case HouseId.Devon:       SpawnDevonNpcs(gd);       break;
                case HouseId.JakeAndEmma: SpawnJakeEmmaNpcs(gd);    break;
                case HouseId.Thompson:    SpawnThompsonNpcs(gd);    break;
                case HouseId.Santos:      SpawnSantosNpcs(gd);      break;
                case HouseId.Petrov:      SpawnPetrovNpcs(gd);      break;
                case HouseId.Sam:         SpawnSamNpcs(gd);         break;
                case HouseId.Johnson:     SpawnJohnsonNpcs(gd);     break;
            }
        }

        // ── Per-house NPC spawning ───────────────────────────────────────────

        private void SpawnChenNpcs(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Mr. Chen",  new Vector2( 5 * 16 + 8,  5 * 16 + 8), NpcAppearances.MrChen,
                () => Engine.DialogueSystem.StartYarnNode("MrChen"));
            SpawnNpc(gd, "Mrs. Chen", new Vector2(16 * 16 + 8,  4 * 16 + 8), NpcAppearances.MrsChen,
                () => Engine.DialogueSystem.StartYarnNode("MrsChen"));
        }

        private void SpawnDevonNpcs(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Devon", new Vector2(3 * 16 + 8, 4 * 16 + 8), NpcAppearances.Devon,
                () => Engine.DialogueSystem.StartYarnNode("Devon"));
        }

        private void SpawnJakeEmmaNpcs(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Emma", new Vector2(5 * 16 + 8, 4 * 16 + 8), NpcAppearances.Emma,
                () => Engine.DialogueSystem.StartYarnNode("Emma"));
            SpawnNpc(gd, "Jake", new Vector2(7 * 16 + 8, 4 * 16 + 8), NpcAppearances.Jake,
                () => Engine.DialogueSystem.StartYarnNode("Jake"));
        }

        private void SpawnThompsonNpcs(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Mr. Thompson", new Vector2(3 * 16 + 8, 4 * 16 + 8), NpcAppearances.MrThompson,
                () => Engine.DialogueSystem.StartYarnNode("Thompson"));
        }

        private void SpawnSantosNpcs(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Maria", new Vector2(16 * 16 + 8, 5 * 16 + 8), NpcAppearances.Maria,
                () => Engine.DialogueSystem.StartYarnNode("Maria"));
        }

        private void SpawnPetrovNpcs(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Mr. Petrov",  new Vector2( 3 * 16 + 8,  5 * 16 + 8), NpcAppearances.MrPetrov,
                () => Engine.DialogueSystem.StartYarnNode("MrPetrov"));
            SpawnNpc(gd, "Mrs. Petrov", new Vector2(16 * 16 + 8,  4 * 16 + 8), NpcAppearances.MrsPetrov,
                () => Engine.DialogueSystem.StartYarnNode("MrsPetrov"));
        }

        private void SpawnSamNpcs(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Linda", new Vector2(5 * 16 + 8, 4 * 16 + 8), NpcAppearances.Linda,
                () => Engine.DialogueSystem.StartYarnNode("Linda"));
        }

        private void SpawnJohnsonNpcs(GraphicsDevice gd)
        {
            SpawnNpc(gd, "DeShonda", new Vector2( 4 * 16 + 8,  6 * 16 + 8), NpcAppearances.DeShonda,
                () => Engine.DialogueSystem.StartYarnNode("DeShonda"));
            SpawnNpc(gd, "Destiny",  new Vector2( 8 * 16 + 8,  4 * 16 + 8), NpcAppearances.Destiny,
                () => Engine.DialogueSystem.StartYarnNode("Destiny"), scale: 0.5f);
            SpawnNpc(gd, "Tyler",    new Vector2(18 * 16 + 8,  4 * 16 + 8), NpcAppearances.Tyler,
                () => Engine.DialogueSystem.StartYarnNode("Tyler"),   scale: 0.5f);
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

            // Exit when player is at the front door tiles (bottom-centre of map).
            if (pos.Y >= (MapH - 2) * 16 - 2 && pos.X > 9 * 16 && pos.X < 12 * 16 + 16)
            {
                _transitioning = true;
                Engine.LoadScene(new NeighborhoodScene());
            }
        }

        // ── Draw ──────────────────────────────────────────────────────────────

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Engine.RenderSystem.DrawScene(Engine.EntityWorld, gameTime, Color.Black);
            DrawUI();
        }

        private Texture2D? _pixel;

        private void DrawUI()
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

            // House-tinted border
            Color borderColor = (GameState.TargetInterior ?? HouseId.Chen) switch
            {
                HouseId.Chen        => new Color( 72, 172, 172),
                HouseId.Devon       => new Color(132,  72, 192),
                HouseId.JakeAndEmma => new Color( 92, 172,  72),
                HouseId.Thompson    => new Color(142, 142, 142),
                HouseId.Santos      => new Color(192,  92, 152),
                HouseId.Petrov      => new Color( 72, 112, 192),
                HouseId.Sam         => new Color(212, 192,  72),
                HouseId.Johnson     => new Color(202, 142,  62),
                _                  => Color.White,
            };

            sb.Draw(_pixel, new Rectangle(8, boxY, vp.Width - 16, boxH), new Color(0, 0, 0, 215));
            sb.Draw(_pixel, new Rectangle(8, boxY, vp.Width - 16, 2), borderColor);
            sb.Draw(_pixel, new Rectangle(8, boxY + boxH - 2, vp.Width - 16, 2), borderColor);
            sb.Draw(_pixel, new Rectangle(8, boxY, 2, boxH), borderColor);
            sb.Draw(_pixel, new Rectangle(vp.Width - 10, boxY, 2, boxH), borderColor);

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
