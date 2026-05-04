using ChildhoodAdventure.RetroSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine.Rendering;

namespace ChildhoodAdventure.Scenes
{
	/// <summary>
	/// The outdoor neighbourhood: one tree-lined street with nine homes.
	/// Each house is built from real tiles — a dark roof, accent-coloured
	/// siding, windows and a centred front door — instead of being a single
	/// flat slab. Per-house identity comes from the active system's
	/// <see cref="ScenePalette"/> mapping a house tone onto its siding.
	///
	/// Street layout (tile rows):
	///   y  0- 5  : north grass
	///   y  6- 9  : house facades (roof / wall / window / door rows)
	///   y 10     : north sidewalk
	///   y 11-12  : road
	///   y 13     : south sidewalk
	///   y 14-21  : south grass / open space
	///
	/// Houses left-to-right (all 7 tiles wide):
	///   Chen | Devon | Jake&amp;Emma | Thompson | Player | Sam | Santos | Petrov | Johnson
	/// </summary>
	public class NeighborhoodScene : AdventureScene
	{
		// ── Base tileset GIDs (firstGid: 1) ──────────────────────────────────
		private const int T_GRASS		= 1;
		private const int T_ROAD		= 2;
		private const int T_SIDEWALK 	= 3;
		private const int T_BUSH		= 4;
		private const int T_GRASS2		= 5;

		// Per-house tilesets follow base. Each house allocates 4 GIDs:
		// roof, siding, window, door (in that order).
		private const int FirstHouseGid	= 6;
		private const int TilesPerHouse = 4;

		private const int MapW			= 82;
		private const int MapH			= 22;
		private const int HouseW 		= 7;
		private const int HouseStartY	= 6;
		private const int HouseDoorY	= 9;		// bottom row of the house = the door row
		private const int SidewalkN_Y	= 10;

		protected override float PlayerMaxSpeed		=> 5.94f;
		protected override float CameraFollowSpeed	=> 7f;
		protected override float InteractionRadius	=> 2f;

		// Outdoor green border; speaker name uses the same tone for cohesion.
		protected override Color DialogueBorderColor =>
			RetroSystemRegistry.Current.ScenePalette.HouseLime;
		protected override Color DialogueSpeakerColor =>
			RetroSystemRegistry.Current.ScenePalette.HouseLime;

		// ── House catalogue ──────────────────────────────────────────────────
		// Index 0 reserved for the player's own home; the eight neighbour
		// homes follow. Order is significant: it controls the per-house GID
		// allocation in the tileset.
		private record HouseEntry( HouseId? Id, int X, Color SidingColor );

		private static HouseEntry[]	BuildHouseList()
		{
			var sp = RetroSystemRegistry.Current.ScenePalette;
			return
			[
				new( null,                    37, sp.HouseBeige ),	// player home (centre)
				new( HouseId.Chen,             1, sp.HouseTeal ),
				new( HouseId.Devon,           10, sp.HousePurple ),
				new( HouseId.JakeAndEmma,     19, sp.HouseLime ),
				new( HouseId.Thompson,        28, sp.HouseGray ),
				new( HouseId.Sam,             46, sp.HouseYellow ),
				new( HouseId.Santos,          55, sp.HousePink ),
				new( HouseId.Petrov,          64, sp.HouseBlue ),
				new( HouseId.Johnson,         73, sp.HouseOrange ),
			];
		}

		// ── Scene load ───────────────────────────────────────────────────────

		protected override void OnSceneLoad()
		{
			Name					= "Neighbourhood";
			GameState.ActiveScene	= GameState.SceneType.Neighborhood;
			var gd					= Engine.GraphicsDevice;
			var sys					= RetroSystemRegistry.Current;
			var houses				= BuildHouseList();

			// Base tileset: ground + bush.
			var tilesetBase = sys.BuildTileset( gd, "hood_base",
				[
					TileType.Grass,		// 1
					TileType.Road,		// 2
					TileType.Sidewalk,	// 3
					TileType.Bush,		// 4
					TileType.Grass2,	// 5
				],	firstGid: 1 );

			var tilemap = new Tilemap( "hood", MapW, MapH )
			{
				BackgroundColor = Color.Black
			};
			tilemap.AddTileset( tilesetBase );

			// One small tileset per house, tinted with that house's siding colour.
			// Each tileset contributes 4 GIDs (roof / siding / window / door).
			for( int i = 0; i < houses.Length; i++ )
			{
				int firstGid = FirstHouseGid + i * TilesPerHouse;
				var houseTileset = sys.BuildTileset( gd, $"house_{i}",
					[
						TileType.HouseRoof,			// +0
						TileType.HouseExterior,		// +1
						TileType.Window,			// +2
						TileType.Door,				// +3
					],	accentColor: houses[i].SidingColor,	firstGid: firstGid );
				tilemap.AddTileset( houseTileset );
			}

			BuildNeighborhoodMap( tilemap, houses );

			Engine.CollisionSystem.SetTilemap( tilemap );
			Engine.RenderSystem.TilemapRenderer.SetTilemap( tilemap );
			Engine.RenderSystem.Camera.Bounds			= new RectangleF( 0, 0, tilemap.Width, tilemap.Height );
			Engine.RenderSystem.Camera.MaxTilesTall		= sys.MaxTilesTall;
			Engine.RenderSystem.Camera.TilesTall		= sys.DefaultTilesTall;
			Engine.RenderSystem.LightingSystem.Enabled	= false;

			// Cache the houses for transition checking.
			_houses = houses;

			SpawnPlayer( gd, GameState.NeighborhoodReturnPosition );
			SpawnOutdoorNpcs( gd );
		}

		// ── Map builder ──────────────────────────────────────────────────────

		private static void BuildNeighborhoodMap( Tilemap map, HouseEntry[] houses )
		{
			var bg	= map.AddLayer( "bg",  LayerType.Background );
			var mid = map.AddLayer( "mid", LayerType.Midground );
			var col = map.AddLayer( "col", LayerType.Collision );

			// Use a constant seed to keep the map always the same
			const int mapSeed =	12181827;
			var rnd =	new Random( mapSeed );

			// Ground bands
			int[]	grassTiles =	[T_GRASS, T_GRASS2];
			FillRect( bg, 0, 0,            MapW, MapH,   grassTiles, rnd );
			FillRect( bg, 0, SidewalkN_Y,  MapW, 1,      T_SIDEWALK );
			FillRect( bg, 0, SidewalkN_Y+1,MapW, 2,      T_ROAD );
			FillRect( bg, 0, SidewalkN_Y+3,MapW, 1,      T_SIDEWALK );

			// Invisible perimeter collision so the player can't walk off the
			// edges of the map. The collision-layer tiles never render.
			for( int x = 0; x < MapW; x++ )
			{ 
				col.SetTile( x, 0, 1 ); 
				col.SetTile( x, MapH - 1, 1 );
			}
			for( int y = 0; y < MapH; y++ )
			{
				col.SetTile( 0, y, 1 );
				col.SetTile( MapW - 1, y, 1 );
			}

			// Houses
			for( int i = 0; i < houses.Length; i++ )
			{
				PlaceHouse( mid, col, houses[i].X, HouseStartY, FirstHouseGid + i * TilesPerHouse );
			}
			
			// Decorative bushes — one between each adjacent pair of houses on
			// the top grass strip, plus a sparse scatter on the south grass.
			for( int i = 0; i < houses.Length - 1; i++ )
			{
				int gapMid = (houses[i].X + HouseW + houses[i + 1].X) / 2;
				mid.SetTile( gapMid, 4, T_BUSH );

				int grassIndex = grassTiles[ rnd.Next() % grassTiles.Length ];
				col.SetTile( gapMid, 4, grassIndex );
			}

			// South grass scatter
			int[] southBushXs = [ 6, 16, 26, 35, 45, 55, 65, 75 ];
			int[] southBushYs = [16, 18, 17, 19, 16, 18, 17, 19];
			for( int i = 0; i < southBushXs.Length; i++ )
			{
				mid.SetTile( southBushXs[i], southBushYs[i], T_BUSH );

				int grassIndex = grassTiles[ rnd.Next() % grassTiles.Length ];
				col.SetTile( southBushXs[i], southBushYs[i], grassIndex );
			}
		}

		private static void PlaceHouse( TileLayer mid, TileLayer col, int hx,	int hy,	int firstGid )
		{
			int roofGid		= firstGid + 0;
			int sidingGid	= firstGid + 1;
			int windowGid	= firstGid + 2;
			int doorGid		= firstGid + 3;

			int yRoof		= hy + 0;
			int yTop		= hy + 1;
			int yWindow 	= hy + 2;
			int yDoor		= hy + 3;

			// Row 0: roof across full width
			for( int dx = 0; dx < HouseW; dx++ )
			{
				PlaceBlocking( mid, col, hx + dx, yRoof, roofGid );
			}

			// Row 1: solid top wall
			for( int dx = 0; dx < HouseW; dx++ )
			{
				PlaceBlocking( mid, col, hx + dx, yTop, sidingGid );
			}

			// Row 2: walls with windows at cols 1 and 5
			for( int dx = 0; dx < HouseW; dx++ )
			{
				int gid = (dx == 1 || dx == 5) ? windowGid : sidingGid;
				PlaceBlocking( mid, col, hx + dx, yWindow, gid );
			}

			// Row 3: walls with door at centre (col 3, walkable)
			for( int dx = 0; dx < HouseW; dx++ )
			{
				if( dx == 3 )
				{
					mid.SetTile( hx + dx, yDoor, doorGid );
					// door is walkable — leave collision empty
				}
				else
				{
					PlaceBlocking( mid, col, hx + dx, yDoor, sidingGid );
				}
			}
		}

		private static void PlaceBlocking( TileLayer mid, TileLayer col, int x, int y, int gid )
		{
			mid.SetTile( x, y, gid );
			col.SetTile( x, y, 1 );
		}

		private static void FillRect( TileLayer layer, int x, int y, int w, int h, int tileIndex )
		{
			for( int dy = 0; dy < h; dy++ )
			{
				for( int dx = 0; dx < w; dx++ )
				{
					layer.SetTile( x + dx, y + dy, tileIndex );
				}
			}
		}

		private static void FillRect( TileLayer layer, int x, int y, int w, int h, int[] tileIndexes, Random rnd )
		{
			for( int dy = 0; dy < h; dy++ )
			{
				for( int dx = 0; dx < w; dx++ )
				{
					int tileIndex =	tileIndexes[ rnd.Next() % tileIndexes.Length ];
					layer.SetTile( x + dx, y + dy, tileIndex );
				}
			}
		}

		// ── NPC spawners ──────────────────────────────────────────────────────

		private void SpawnOutdoorNpcs( GraphicsDevice gd )
		{
			// Place NPCs on the south grass just past the sidewalk.
			SpawnNpc( gd, "Sam",   new Vector2( 49.5f, 14.5f ), NpcAppearances.Sam,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Sam" ),	scale:	0.5f );
			SpawnNpc( gd, "Lucia", new Vector2( 58.5f, 14.5f ), NpcAppearances.Lucia,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Lucia" ),	scale:	0.5f );
			SpawnNpc( gd, "Nadia", new Vector2( 67.5f, 14.5f ), NpcAppearances.Nadia,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Nadia" ),	scale:	0.5f );
		}

		// ── Scene transitions ────────────────────────────────────────────────

		private HouseEntry[]? _houses;

		protected override void CheckSceneTransitions()
		{
			if( _houses == null ) { return; }

			var pos			= PlayerPosition;
			float pTileX	= pos.X;
			float pTileY	= pos.Y;

			if( pTileY < HouseDoorY - 0.5f || pTileY > HouseDoorY + 0.5f ) { return; }

			// Each house's door is at hx + 3.
			foreach( var h in _houses )
			{
				int doorX =	h.X + 3;
				if( MathF.Abs( pTileX - doorX ) < 0.6f )
				{
					Transitioning =	true;
					if( h.Id == null )
					{
						// Player home
						GameState.PlayerSpawnPosition =	new Vector2( 12.5f, 15.5f );
						Engine.LoadScene( new HomeInteriorScene() );
					}
					else
					{
						GameState.TargetInterior				= h.Id;
						GameState.PlayerSpawnPosition			= new Vector2( 11.5f, 13.5f );
						GameState.NeighborhoodReturnPosition	= new Vector2( doorX + 0.5f, SidewalkN_Y + 0.5f );
						Engine.LoadScene( new NeighborInteriorScene() );
					}
					return;
				}
			}
		}
	}
}
