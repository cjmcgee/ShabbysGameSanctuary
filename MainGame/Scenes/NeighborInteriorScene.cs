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

            // Windows on the top wall
            mid.SetTile(4,  0, T_WINDOW); col.SetTile(4,  0, 0);
            mid.SetTile(9,  0, T_WINDOW); col.SetTile(9,  0, 0);
            mid.SetTile(16, 0, T_WINDOW); col.SetTile(16, 0, 0);

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
            e.AddComponent(new SpriteComponent
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

        // ── Per-house NPC spawning & dialogue ────────────────────────────────

        private void SpawnChenNpcs(GraphicsDevice gd)
        {
            // Mr. Chen — chess table area
            SpawnNpc(gd, "Mr. Chen", new Vector2(5 * 16 + 8, 5 * 16 + 8),
                NpcAppearances.MrChen, () =>
                {
                    if (!GameState.HasFlag("talked_mrchen"))
                    {
                        GameState.SetFlag("talked_mrchen");
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Mr. Chen", "Ah, a visitor! Come in, come in — my wife just made tea."),
                            new("Mr. Chen", "Tell me — do you know how to play chess? I have been looking for a worthy opponent. My grandchildren never visit...",
                                choices: new[]
                                {
                                    new DialogueChoice("I'd love to learn!", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Mr. Chen", "Wonderful! A student! Come back when you have time and I will teach you."),
                                            new("Mr. Chen", "The game teaches patience — and patience, my young friend, teaches everything else."),
                                        })),
                                    new DialogueChoice("I'm not very good at chess.", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Mr. Chen", "Nobody starts out good! I was terrible when I was your age. My grandfather nearly threw the board at me!"),
                                            new("Mr. Chen", "Come back sometime. We will learn together. Or argue. Either way is enjoyable."),
                                        })),
                                }),
                        });
                    }
                    else
                    {
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Mr. Chen", "The door is always open, young friend. And the chess board is always waiting."),
                        });
                    }
                });

            // Mrs. Chen — kitchen
            SpawnNpc(gd, "Mrs. Chen", new Vector2(16 * 16 + 8, 4 * 16 + 8),
                NpcAppearances.MrsChen, () =>
                {
                    if (!GameState.HasFlag("talked_mrschenC"))
                    {
                        GameState.SetFlag("talked_mrschenC");
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Mrs. Chen", "Oh! Hello dear! Have you eaten today? I always make far too much."),
                            new("Mrs. Chen", "Here — take these sesame cookies home to your family. My mother's recipe. Ninety years old and still perfect."),
                            new("Mrs. Chen", "That family at the end of the block — the Petrovs — I brought some to them too when they arrived. They seemed so tired."),
                            new("Mrs. Chen", "Moving is hard. Even harder when everything is unfamiliar."),
                        });
                        GameState.SetFlag("received_chen_cookies");
                    }
                    else
                    {
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Mrs. Chen", "Come back anytime, dear! There are always cookies."),
                        });
                    }
                });
        }

        private void SpawnDevonNpcs(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Devon", new Vector2(3 * 16 + 8, 4 * 16 + 8),
                NpcAppearances.Devon, () =>
                {
                    if (!GameState.HasFlag("talked_devon"))
                    {
                        GameState.SetFlag("talked_devon");
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Devon", "Oh — hey. Sorry, I'm kind of deep in a study trance. Finals week is... a lot."),
                            new("Devon", "What are you studying? Oh — I mean, what am I studying. Philosophy.",
                                choices: new[]
                                {
                                    new DialogueChoice("What's philosophy?", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Devon", "It's the study of why people do what they do. Why anything exists. Whether choices matter."),
                                            new("Devon", "Do you ever think about whether anything you choose actually matters? In a cosmic sense?"),
                                            new("Devon", "...Sorry. That's a lot to dump on a kid. My advisor says I spend too much time in my head."),
                                            new("Devon", "The short version: I study Big Questions. And right now I'm failing to answer the smaller ones, like this exam."),
                                        })),
                                    new DialogueChoice("Sounds hard.", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Devon", "It is! But also... kind of beautiful? The harder the question, the more alive you feel asking it."),
                                            new("Devon", "Anyway. Hi. I'm Devon. What's your name?"),
                                        })),
                                }),
                        });
                    }
                    else
                    {
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Devon", "Still studying. Or trying to. Come back after finals — I'll actually be human again."),
                        });
                    }
                });
        }

        private void SpawnJakeEmmaNpcs(GraphicsDevice gd)
        {
            // Emma greets first
            SpawnNpc(gd, "Emma", new Vector2(5 * 16 + 8, 4 * 16 + 8),
                NpcAppearances.Emma, () =>
                {
                    if (!GameState.HasFlag("talked_jakeemma"))
                    {
                        GameState.SetFlag("talked_jakeemma");
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Emma", "Oh hi! Jake — JAKE — come meet the neighbour kid!"),
                            new("Jake", "Hey! Sorry about her. She gets excited."),
                            new("Emma", "We're still setting up but — do you want to see our game collection? We have SO MANY."),
                            new("Jake", "She means we have a problem. A fun problem, but still a problem.",
                                choices: new[]
                                {
                                    new DialogueChoice("What kind of games?", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Jake", "Board games, card games, video games — if it exists and has rules, we own it."),
                                            new("Emma", "We're a little competitive. Just a tiny bit."),
                                            new("Jake", "She means she has absolutely destroyed me at everything we have ever tried."),
                                            new("Emma", "It is not my fault I am naturally gifted at things."),
                                        })),
                                    new DialogueChoice("Nice to meet you both.", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Jake", "You too! Seriously — if you ever need anything, just knock. Borrow sugar, whatever."),
                                            new("Emma", "Our door is open. Literally, we keep forgetting to close it. It's a whole thing."),
                                        })),
                                }),
                        });
                    }
                    else
                    {
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Emma", "Come play games with us sometime! We need a tiebreaker judge."),
                        });
                    }
                });

            // Jake is nearby
            SpawnNpc(gd, "Jake", new Vector2(7 * 16 + 8, 4 * 16 + 8),
                NpcAppearances.Jake, () =>
                {
                    Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                    {
                        new("Jake", "Hey kid! Come by anytime. And tell your parents we said hello."),
                    });
                });
        }

        private void SpawnThompsonNpcs(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Mr. Thompson", new Vector2(3 * 16 + 8, 4 * 16 + 8),
                NpcAppearances.MrThompson, () =>
                {
                    if (!GameState.HasFlag("talked_thompson"))
                    {
                        GameState.SetFlag("talked_thompson");
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Mr. Thompson", "...What do you want?"),
                            new("Mr. Thompson", "I'm not being rude. I just... I don't get many visitors. Takes me a moment.",
                                choices: new[]
                                {
                                    new DialogueChoice("Just saying hi.", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Mr. Thompson", "Hmph."),
                                            new("Mr. Thompson", "...You're the first person who's stopped by all week."),
                                            new("Mr. Thompson", "Tell your parents I said hello. And thank your mother for the casserole she left last month. I never properly thanked her."),
                                        })),
                                    new DialogueChoice("Do you need help with anything?", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Mr. Thompson", "..."),
                                            new("Mr. Thompson", "The garden has gotten a bit wild. Margaret always kept it beautiful. I haven't had the... motivation."),
                                            new("Mr. Thompson", "Not that I'm asking for help. I'm just — stating a fact."),
                                            new("Mr. Thompson", "...You could come back and look at it, I suppose. If you had nothing better to do."),
                                        })),
                                }),
                        });
                        GameState.SetFlag("met_thompson");
                    }
                    else
                    {
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Mr. Thompson", "You came back. Hmph. Well. The kettle's on, if you want tea."),
                        });
                    }
                });
        }

        private void SpawnSantosNpcs(GraphicsDevice gd)
        {
            // Maria in the kitchen
            SpawnNpc(gd, "Maria", new Vector2(16 * 16 + 8, 5 * 16 + 8),
                NpcAppearances.Maria, () =>
                {
                    if (!GameState.HasFlag("talked_maria"))
                    {
                        GameState.SetFlag("talked_maria");
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Maria", "Oh! Hello! Come in, come in. I hope Lucia hasn't been causing trouble — she's been SO excited to meet everyone."),
                            new("Maria", "It's just the two of us here. Well, three counting little Carlos.",
                                choices: new[]
                                {
                                    new DialogueChoice("She seems really nice.", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Maria", "She is. She had to say goodbye to all her friends when we moved. Not one complaint."),
                                            new("Maria", "...I know it's harder for her than she lets on. She's protecting me, I think. At six years old, already protecting me."),
                                        })),
                                    new DialogueChoice("What do you do for work?", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Maria", "I'm a nurse! Night shifts mostly — which means Lucia handles a lot on her own during the day."),
                                            new("Maria", "She never complains. But sometimes I worry... I want more for her, you know? More than just getting by."),
                                        })),
                                }),
                        });
                    }
                    else
                    {
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Maria", "Mi casa es su casa, mijo. Come by whenever you like. The more friends Lucia has, the better."),
                        });
                    }
                });
        }

        private void SpawnPetrovNpcs(GraphicsDevice gd)
        {
            // Mr. Petrov — living area
            SpawnNpc(gd, "Mr. Petrov", new Vector2(3 * 16 + 8, 5 * 16 + 8),
                NpcAppearances.MrPetrov, () =>
                {
                    if (!GameState.HasFlag("talked_mrpetrov"))
                    {
                        GameState.SetFlag("talked_mrpetrov");
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Mr. Petrov", "Good day! You are... neighbour child? Yes? Please, come."),
                            new("Mr. Petrov", "We are still learning. How to say. Everything is new.",
                                choices: new[]
                                {
                                    new DialogueChoice("How do you like it here?", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Mr. Petrov", "Is... quiet. Good quiet. In city we come from, not so quiet."),
                                            new("Mr. Petrov", "We are... hopeful. Yes. That is right word. Hopeful."),
                                        })),
                                    new DialogueChoice("Your daughter Nadia is very nice.", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Mrs. Petrov", "You meet Nadia? She is shy at first — good heart, big heart, but shy."),
                                            new("Mr. Petrov", "She misses her grandmother very much. Is hard. But she is brave. Like her mother."),
                                        })),
                                }),
                        });
                    }
                    else
                    {
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Mr. Petrov", "Thank you for visiting. It means very much."),
                        });
                    }
                });

            // Mrs. Petrov — kitchen with tea
            SpawnNpc(gd, "Mrs. Petrov", new Vector2(16 * 16 + 8, 4 * 16 + 8),
                NpcAppearances.MrsPetrov, () =>
                {
                    Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                    {
                        new("Mrs. Petrov", "Oh! Guest! I make tea. Please sit, please sit."),
                        new("Mrs. Petrov", "You are kind child. Nadia needs kind friends."),
                    });
                });
        }

        private void SpawnSamNpcs(GraphicsDevice gd)
        {
            // Sam's mum Linda
            SpawnNpc(gd, "Linda", new Vector2(5 * 16 + 8, 4 * 16 + 8),
                NpcAppearances.Linda, () =>
                {
                    if (!GameState.HasFlag("talked_linda"))
                    {
                        GameState.SetFlag("talked_linda");
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Linda", "Oh hello! You're Sam's friend, right? They're outside somewhere — probably causing mayhem."),
                            new("Linda", "I'm glad Sam has a friend like you. They talk about you all the time, you know."),
                            new("Linda", "Hey, if you two get hungry later — I've got snacks. The door is always open.",
                                choices: new[]
                                {
                                    new DialogueChoice("Thank you, Mrs...?", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Linda", "Linda! Just Linda. 'Mrs.' makes me feel ancient."),
                                            new("Linda", "Now go find Sam before they rope someone into another 'expedition'."),
                                        })),
                                    new DialogueChoice("We'll be careful!", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Linda", "Ha! You're already more responsible than Sam. That's... both reassuring and slightly concerning."),
                                            new("Linda", "Go on, have fun."),
                                        })),
                                }),
                        });
                    }
                    else
                    {
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Linda", "Sam's still outside. Go rescue them from whatever scheme they're hatching!"),
                        });
                    }
                });
        }

        private void SpawnJohnsonNpcs(GraphicsDevice gd)
        {
            // DeShonda — living room
            SpawnNpc(gd, "DeShonda", new Vector2(4 * 16 + 8, 6 * 16 + 8),
                NpcAppearances.DeShonda, () =>
                {
                    if (!GameState.HasFlag("talked_deshonda"))
                    {
                        GameState.SetFlag("talked_deshonda");
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("DeShonda", "Well hello! You must be from the neighbourhood. I'm DeShonda. Always happy to have visitors."),
                            new("DeShonda", "Have you met my daughter Destiny? She's right over there. And Tyler's around somewhere — probably being moody."),
                        });
                    }
                    else
                    {
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("DeShonda", "Come back anytime, sweetheart. This house is always full of people and noise — one more is never a problem."),
                        });
                    }
                });

            // Destiny — guitar area
            SpawnNpc(gd, "Destiny", new Vector2(8 * 16 + 8, 4 * 16 + 8),
                NpcAppearances.Destiny, () =>
                {
                    if (!GameState.HasFlag("talked_destiny"))
                    {
                        GameState.SetFlag("talked_destiny");
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Destiny", "Oh my GOD, finally someone who isn't my mom."),
                            new("Destiny", "I'm Destiny. I play guitar. Want to hear something?",
                                choices: new[]
                                {
                                    new DialogueChoice("Yes please!", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Destiny", "*plays a surprisingly good melody*"),
                                            new("Destiny", "Not bad for six months of lessons, right? Mom says I need to practice more. I'm practically already a prodigy."),
                                        })),
                                    new DialogueChoice("Maybe later?", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Destiny", "Okay, okay. No pressure. Not everyone appreciates art right away."),
                                            new("Destiny", "Come back when you're ready. I'll still be here being brilliant."),
                                        })),
                                }),
                        });
                    }
                    else
                    {
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Destiny", "Practice makes perfect, they say. I say talent makes perfect. But I practice anyway."),
                        });
                    }
                });

            // Tyler — brooding in the corner
            SpawnNpc(gd, "Tyler", new Vector2(18 * 16 + 8, 4 * 16 + 8),
                NpcAppearances.Tyler, () =>
                {
                    if (!GameState.HasFlag("talked_tyler"))
                    {
                        GameState.SetFlag("talked_tyler");
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Tyler", "Ugh. Another neighbourhood kid."),
                            new("Tyler", "Look, I'm busy. Not that it's any of your business, but I'm trying to figure out how to apologize to my best friend.",
                                choices: new[]
                                {
                                    new DialogueChoice("What happened?", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Tyler", "I said something dumb. The kind of dumb where you know it's wrong the second it comes out of your mouth, but it's too late."),
                                            new("Tyler", "It's... look, it doesn't matter. Just — if you ever say something dumb to someone you care about?"),
                                            new("Tyler", "Apologize fast. Don't do the thing where you wait and hope they forget. They don't forget."),
                                        })),
                                    new DialogueChoice("Sorry to bother you.", onSelected: () =>
                                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                                        {
                                            new("Tyler", "You're not bothering me. I'm just... in a mood. Happens."),
                                            new("Tyler", "Ask Destiny if you need anything. She's the friendly one."),
                                        })),
                                }),
                        });
                    }
                    else
                    {
                        Engine.DialogueSystem.StartDialogue(new DialogueLine[]
                        {
                            new("Tyler", "Still figuring things out. I'll get there. ...Probably."),
                        });
                    }
                });
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
