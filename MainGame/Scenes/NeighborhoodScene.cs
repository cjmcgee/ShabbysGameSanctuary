using ChildhoodAdventure.RetroSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    ///   Chen | Devon | Jake&amp;Emma | Thompson | Player | Sam | Santos | Petrov | Johnson
    /// </summary>
    public class NeighborhoodScene : AdventureScene
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

        private const int MapW = 82, MapH = 38;

        private record HouseData(HouseId Id, int X, int WallGid, int DoorX);

        private static readonly HouseData[] _neighborHouses =
        {
            new(HouseId.Chen,        1,  T_TEAL,   4),
            new(HouseId.Devon,      10,  T_PURPLE, 13),
            new(HouseId.JakeAndEmma,19,  T_GREEN,  22),
            new(HouseId.Thompson,   28,  T_GRAY,   31),
            new(HouseId.Sam,        46,  T_YELLOW, 49),
            new(HouseId.Santos,     55,  T_PINK,   58),
            new(HouseId.Petrov,     64,  T_BLUE,   67),
            new(HouseId.Johnson,    73,  T_ORANGE, 76),
        };

        private const int HouseStartY = 4, HouseH = 15, HouseDoorY = 18;

        protected override float PlayerMaxSpeed    => 95f;
        protected override float CameraFollowSpeed => 7f;
        protected override float InteractionRadius => 32f;

        protected override Color DialogueBorderColor  => new(120, 200, 120);
        protected override Color DialogueSpeakerColor => Color.LightGreen;

        // ── Scene load ───────────────────────────────────────────────────────

        protected override void OnSceneLoad()
        {
            Name = "Neighbourhood";
            GameState.ActiveScene = GameState.SceneType.Neighborhood;
            var gd  = Engine.GraphicsDevice;
            var sys = RetroSystemRegistry.Current;

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

            var tilesetBase = sys.BuildTileset(gd, "hood_base", new[]
            {
                TileType.Grass,    // 1
                TileType.Road,     // 2
                TileType.Sidewalk, // 3
            }, firstGid: 1);

            var houseTexture  = BuildHouseTextureSlab(gd, sys, houseColors);
            var tilesetHouses = new Tileset("hood_houses", houseTexture, 16, 16, firstGid: 4);

            var tilemap = new Tilemap("hood", MapW, MapH, 16, 16)
            {
                BackgroundColor = Color.Black
            };
            tilemap.AddTileset(tilesetBase);
            tilemap.AddTileset(tilesetHouses);
            BuildNeighborhoodMap(tilemap);

            Engine.CollisionSystem.SetTilemap(tilemap);
            Engine.RenderSystem.TilemapRenderer.SetTilemap(tilemap);
            Engine.RenderSystem.Camera.Bounds         = new Rectangle(0, 0, tilemap.PixelWidth, tilemap.PixelHeight);
            Engine.RenderSystem.Camera.MaxWorldVisible = sys.MaxZoomOutArea;
            Engine.RenderSystem.Camera.Zoom            = sys.DisplayScale;
            Engine.RenderSystem.LightingSystem.Enabled = false;

            SpawnPlayer(gd, GameState.NeighborhoodReturnPosition);
            SpawnOutdoorNpcs(gd);
        }

        // ── Texture helpers ───────────────────────────────────────────────────

        private static Texture2D BuildHouseTextureSlab(
            GraphicsDevice gd, RetroSystem sys, Color[] houseColors)
        {
            const int tileSize = 16;
            int count   = houseColors.Length;
            var texture = new Texture2D(gd, tileSize * count, tileSize);
            var data    = new Color[tileSize * count * tileSize];

            for (int i = 0; i < count; i++)
            {
                var slab = sys.BuildTileset(gd, $"h{i}", new[] { TileType.Accent },
                    accentColor: houseColors[i]);

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

            FillRect(bg, 0, 0, MapW, MapH, T_GRASS);
            FillRect(bg, 0, 19, MapW, 1, T_SIDEWALK);
            FillRect(bg, 0, 20, MapW, 2, T_ROAD);
            FillRect(bg, 0, 22, MapW, 1, T_SIDEWALK);

            PlaceHouse(mid, col, 37, HouseStartY, 7, HouseH, T_BEIGE, 40);
            foreach (var h in _neighborHouses)
                PlaceHouse(mid, col, h.X, HouseStartY, 7, HouseH, h.WallGid, h.DoorX);
        }

        private static void PlaceHouse(TileLayer mid, TileLayer col,
            int hx, int hy, int w, int h, int wallGid, int doorX)
        {
            for (int y = hy; y < hy + h; y++)
                for (int x = hx; x < hx + w; x++)
                {
                    mid.SetTile(x, y, wallGid);
                    col.SetTile(x, y, 1);
                }

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

        // ── NPC spawners ──────────────────────────────────────────────────────

        private void SpawnOutdoorNpcs(GraphicsDevice gd)
        {
            SpawnNpc(gd, "Sam",   new Vector2(49 * 16 + 8, 21 * 16 + 8), NpcAppearances.Sam,
                () => Engine.DialogueSystem.StartYarnNode("Sam"),   scale: 0.5f);
            SpawnNpc(gd, "Lucia", new Vector2(58 * 16 + 8, 21 * 16 + 8), NpcAppearances.Lucia,
                () => Engine.DialogueSystem.StartYarnNode("Lucia"), scale: 0.5f);
            SpawnNpc(gd, "Nadia", new Vector2(67 * 16 + 8, 21 * 16 + 8), NpcAppearances.Nadia,
                () => Engine.DialogueSystem.StartYarnNode("Nadia"), scale: 0.5f);
        }

        // ── Scene transitions ────────────────────────────────────────────────

        protected override void CheckSceneTransitions()
        {
            var pos = PlayerPosition;
            float pTileX = pos.X / 16f;
            float pTileY = pos.Y / 16f;

            if (pTileY < HouseDoorY - 0.5f || pTileY > HouseDoorY + 0.5f) return;

            if (MathF.Abs(pTileX - 40f) < 0.6f)
            {
                Transitioning = true;
                GameState.PlayerSpawnPosition = new Vector2(12 * 16 + 8, 15 * 16 + 8);
                Engine.LoadScene(new HomeInteriorScene());
                return;
            }

            foreach (var h in _neighborHouses)
            {
                if (MathF.Abs(pTileX - h.DoorX) < 0.6f)
                {
                    Transitioning = true;
                    GameState.TargetInterior  = h.Id;
                    GameState.PlayerSpawnPosition        = new Vector2(11 * 16 + 8, 13 * 16 + 8);
                    GameState.NeighborhoodReturnPosition = new Vector2(h.DoorX * 16 + 8, 19 * 16 + 8);
                    Engine.LoadScene(new NeighborInteriorScene());
                    return;
                }
            }
        }
    }
}
