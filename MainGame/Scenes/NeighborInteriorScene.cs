using TileEngine.Rendering;

namespace ChildhoodAdventure.Scenes
{
	/// <summary>
	/// Generic neighbour home interior, parameterised by GameState.TargetInterior.
	/// Each house has a unique colour scheme, furniture layout, and set of NPCs.
	///
	/// Shared map dimensions: 22×16 tiles.
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
	internal sealed class NeighborInteriorScene :	AdventureScene
	{
		// ── Shared tile GIDs ─────────────────────────────────────────────────
		private const int T_FLOOR	= 1;
		private const int T_WALL	= 2;
		private const int T_FURN	= 3;	// dark furniture
		private const int T_DOOR	= 5;
		private const int T_ACCENT	= 6;	// carpet / rug accent
		private const int T_WINDOW	= 7;
		private const int T_BOOK	= 8;	// bookshelf
		private const int T_KITCHEN = 9;	// kitchen tile floor
		private const int T_PLANT	= 10;	// houseplant

		private const int MapW		= 22;
		private const int MapH 		= 16;

		protected override Color DialogueBorderColor =>
			HouseToneFor( GameState.TargetInterior ?? HouseId.Chen, RetroSystemRegistry.Current.ScenePalette );

		// Map each neighbour's house to its semantic ScenePalette tone. The
		// active retro system decides what colour each tone resolves to.
		private static Color HouseToneFor( HouseId id, ScenePalette sp ) => id switch
		{
			HouseId.Chen		=> sp.HouseTeal,
			HouseId.Devon		=> sp.HousePurple,
			HouseId.JakeAndEmma => sp.HouseLime,
			HouseId.Thompson	=> sp.HouseGray,
			HouseId.Santos		=> sp.HousePink,
			HouseId.Petrov		=> sp.HouseBlue,
			HouseId.Sam			=> sp.HouseYellow,
			HouseId.Johnson		=> sp.HouseOrange,
			_					=> sp.HouseBeige,
		};

		// ── Scene load ───────────────────────────────────────────────────────

		protected override void OnSceneLoad()
		{
			Name					= $"{GameState.TargetInterior} Home";
			GameState.ActiveScene	= GameState.SceneType.NeighborInterior;
			var gd					= Engine.GraphicsDevice;
			var id					= GameState.TargetInterior ?? HouseId.Chen;
			var sys					= RetroSystemRegistry.Current;
			Color accentColor		= HouseToneFor( id, sys.ScenePalette );

			var tileset = sys.BuildTileset( gd, "interior", new[]
			{
				TileType.WoodFloor,		//  1
				TileType.Wall,			//  2
				TileType.Furniture,		//  3
				TileType.Counter,		//  4
				TileType.Door,			//  5
				TileType.Accent,		//  6 (house-specific carpet)
				TileType.Window,		//  7
				TileType.Bookshelf,		//  8
				TileType.KitchenTile,	//  9
				TileType.Plant,			// 10
			},	accentColor: accentColor, firstGid: 1 );

			var tilemap = new Tilemap( "interior", MapW, MapH )
			{
				BackgroundColor =	Color.Black
			};
			tilemap.AddTileset( tileset );
			BuildInteriorMap( tilemap, id );

			Engine.CollisionSystem.SetTilemap( tilemap );
			Engine.RenderSystem.TilemapRenderer.SetTilemap( tilemap );
			Engine.RenderSystem.Camera.Bounds			= new RectangleF( 0, 0, tilemap.Width, tilemap.Height );
			Engine.RenderSystem.Camera.MaxTilesTall 	= sys.MaxTilesTall;
			Engine.RenderSystem.Camera.TilesTall		= sys.DefaultTilesTall;
			Engine.RenderSystem.LightingSystem.Enabled	= false;

			SpawnPlayer( gd, GameState.PlayerSpawnPosition );
			SpawnNpcs( gd, id );
		}

		// ── Map builder ──────────────────────────────────────────────────────

		private static void BuildInteriorMap( Tilemap map, HouseId id )
		{
			var bg	=	map.AddLayer( "bg",  LayerType.Background );
			var mid =	map.AddLayer( "mid", LayerType.Midground );
			var col =	map.AddLayer( "col", LayerType.Collision );

			FillRect( bg, 1, 1, MapW - 2, MapH - 2, T_FLOOR );
			FillRect( bg, 13, 1, 8, 6, T_KITCHEN );
			FillRect( bg, 2, 3, 8, 5, T_ACCENT );

			FillRowMid( mid, col, 0 );
			FillRowMid( mid, col, MapH - 1 );
			FillColMid( mid, col, 0 );
			FillColMid( mid, col, MapW - 1 );

			mid.SetTile( 4,  0, T_WINDOW );
			mid.SetTile( 9,  0, T_WINDOW );
			mid.SetTile( 16, 0, T_WINDOW );

			mid.SetTile( 10, MapH - 1, T_DOOR );	
			col.SetTile( 10, MapH - 1, 0 );
			mid.SetTile( 11, MapH - 1, T_DOOR );	
			col.SetTile( 11, MapH - 1, 0 );

			switch( id )
			{
				case HouseId.Chen:			BuildChenLayout( mid, col );		break;
				case HouseId.Devon:			BuildDevonLayout( mid, col );		break;
				case HouseId.JakeAndEmma:	BuildJakeEmmaLayout( mid, col );	break;
				case HouseId.Thompson:		BuildThompsonLayout( mid, col );	break;
				case HouseId.Santos:		BuildSantosLayout( mid, col );		break;
				case HouseId.Petrov:		BuildPetrovLayout( mid, col );		break;
				case HouseId.Sam:			BuildSamLayout( mid, col );			break;
				case HouseId.Johnson:		BuildJohnsonLayout( mid, col );		break;
			}
		}

		private static void BuildChenLayout( TileLayer mid, TileLayer col )
		{
			PlaceFurn( mid, col, 1, 1, 4, 1 );
			PlaceFurn( mid, col, 1, 2, 1, 3 );
			Furn( mid, col, 5, 4 );	
			Furn( mid, col, 5, 5 );
			PlaceFurn( mid, col, 13, 1, 7, 1 );
			Furn( mid, col, 20, 1 );	
			Furn( mid, col, 20, 2 );
			Furn( mid, col, 15, 5 );	
			Furn( mid, col, 16, 5 );
			Furn( mid, col, 9, 8 );
			mid.SetTile( 9, 8, T_PLANT );
		}

		private static void BuildDevonLayout( TileLayer mid, TileLayer col )
		{
			Furn( mid, col, 2, 1 );	
			Furn( mid, col, 3, 1 );
			PlaceFurn( mid, col, 1, 2, 2, 1 );
			PlaceFurn( mid, col, 6, 1, 3, 1 );
			Furn( mid, col, 2, 4 );
			PlaceFurn( mid, col, 13, 1, 7, 1 );
			Furn( mid, col, 20, 1 );
			Furn( mid, col, 1, 7 );	
			Furn( mid, col, 1, 8 );
			mid.SetTile( 10, 8, T_PLANT );
		}

		private static void BuildJakeEmmaLayout( TileLayer mid, TileLayer col )
		{
			PlaceFurn( mid, col, 1, 1, 2, 1 );
			PlaceFurn( mid, col, 4, 1, 3, 1 );
			PlaceFurn( mid, col, 2, 3, 4, 1 );
			Furn( mid, col, 4, 5 );
			PlaceFurn( mid, col, 13, 1, 7, 1 );
			Furn( mid, col, 20, 1 );	
			Furn( mid, col, 20, 2 );
			Furn( mid, col, 15, 5 );	
			Furn( mid, col, 16, 5 );
			mid.SetTile( 8, 2, T_PLANT );	
			mid.SetTile( 8, 3, T_PLANT );
		}

		private static void BuildThompsonLayout( TileLayer mid, TileLayer col )
		{
			Furn( mid, col, 3, 3 );	
			Furn( mid, col, 4, 3 );
			Furn( mid, col, 3, 5 );
			PlaceFurn( mid, col, 1, 1, 3, 1 );
			PlaceFurn( mid, col, 13, 1, 7, 1 );
			Furn( mid, col, 20, 1 );
			Furn( mid, col, 15, 4 );
			mid.SetTile( 9, 1, T_PLANT );
		}

		private static void BuildSantosLayout( TileLayer mid, TileLayer col )
		{
			PlaceFurn( mid, col, 2, 3, 3, 1 );
			Furn( mid, col, 3, 5 );
			PlaceFurn( mid, col, 13, 1, 7, 1 );
			Furn( mid, col, 20, 1 );	
			Furn( mid, col, 20, 2 );
			PlaceFurn( mid, col, 14, 5, 3, 2 );
			mid.SetTile( 1, 2, T_PLANT );
			mid.SetTile( 10, 2, T_BOOK );	
			mid.SetTile( 11, 2, T_BOOK );
			col.SetTile( 10, 2, 1 );	
			col.SetTile( 11, 2, 1 );
		}

		private static void BuildPetrovLayout( TileLayer mid, TileLayer col )
		{
			Furn( mid, col, 2, 3 );	
			Furn( mid, col, 3, 3 );
			Furn( mid, col, 3, 5 );
			Furn( mid, col, 7, 2 );	
			Furn( mid, col, 8, 2 );	
			Furn( mid, col, 7, 3 );
			PlaceFurn( mid, col, 13, 1, 7, 1 );
			Furn( mid, col, 20, 1 );	
			Furn( mid, col, 20, 2 );
			Furn( mid, col, 15, 5 );
			mid.SetTile( 1, 1, T_PLANT );
		}

		private static void BuildSamLayout( TileLayer mid, TileLayer col )
		{
			PlaceFurn(mid, col, 2, 3, 3, 1);
			Furn(mid, col, 3, 5);
			PlaceFurn(mid, col, 1, 1, 3, 1);
			PlaceFurn(mid, col, 5, 1, 2, 1);
			PlaceFurn(mid, col, 13, 1, 7, 1);
			Furn(mid, col, 20, 1);	Furn(mid, col, 20, 2);
			Furn(mid, col, 15, 5);	Furn(mid, col, 16, 5);
			mid.SetTile(9, 2, T_PLANT);
		}

		private static void BuildJohnsonLayout( TileLayer mid, TileLayer col )
		{
			PlaceFurn( mid, col, 1, 3, 4, 1 );
			PlaceFurn( mid, col, 1, 4, 1, 2 );
			Furn( mid, col, 4, 5 );
			Furn( mid, col, 7, 2 );	
			col.SetTile( 7, 2, 0 );
			PlaceFurn( mid, col, 1, 1, 2, 1 );
			PlaceFurn( mid, col, 13, 1, 7, 1 );
			Furn( mid, col, 20, 1 );	
			Furn( mid, col, 20, 2 );
			PlaceFurn( mid, col, 14, 5, 4, 2 );
			mid.SetTile( 10, 1, T_PLANT );	
			mid.SetTile( 10, 2, T_PLANT );
		}

		// ── Furniture helpers ─────────────────────────────────────────────────

		private static void PlaceFurn( TileLayer mid, TileLayer col, int x, int y, int w, int h )
		{
			for( int dy = 0; dy < h; dy++ )
			{
				for( int dx = 0; dx < w; dx++ )
				{
					Furn( mid, col, x + dx, y + dy );
				}
			}
		}

		private static void Furn( TileLayer mid, TileLayer col, int x, int y )
		{
			mid.SetTile( x, y, T_FURN );
			col.SetTile( x, y, 1 );
		}

		private static void FillRect( TileLayer layer, int x, int y, int w, int h, int gid )
		{
			for( int dy = 0; dy < h; dy++ )
			{
				for( int dx = 0; dx < w; dx++ )
				{
					layer.SetTile( x + dx, y + dy, gid );
				}
			}
		}

		private static void FillRowMid( TileLayer mid, TileLayer col, int y )
		{
			for( int x = 0; x < MapW; x++ )	
			{ 
				mid.SetTile( x, y, T_WALL ); 
				col.SetTile( x, y, 1 ); 
			}
		}

		private static void FillColMid( TileLayer mid, TileLayer col, int x )
		{
			for( int y = 0; y < MapH; y++ )	
			{ 
				mid.SetTile( x, y, T_WALL ); 
				col.SetTile( x, y, 1 ); 
			}
		}

		// ── NPC spawners ──────────────────────────────────────────────────────

		private void SpawnNpcs( GraphicsDevice gd, HouseId id )
		{
			switch( id )
			{
				case HouseId.Chen:			SpawnChenNpcs( gd );		break;
				case HouseId.Devon:			SpawnDevonNpcs( gd );		break;
				case HouseId.JakeAndEmma:	SpawnJakeEmmaNpcs( gd );	break;
				case HouseId.Thompson:		SpawnThompsonNpcs( gd );	break;
				case HouseId.Santos:		SpawnSantosNpcs( gd );		break;
				case HouseId.Petrov:		SpawnPetrovNpcs( gd );		break;
				case HouseId.Sam:			SpawnSamNpcs( gd );			break;
				case HouseId.Johnson:		SpawnJohnsonNpcs( gd );		break;
			}
		}

		private void SpawnChenNpcs( GraphicsDevice gd )
		{
			SpawnNpc( gd, "Mr. Chen",  new Vector2( 5.5f, 5.5f ), NpcAppearances.MrChen,
				()	=>	Engine.DialogueSystem.StartYarnNode( "MrChen" ));
			SpawnNpc( gd, "Mrs. Chen", new Vector2( 16.5f, 4.5f ), NpcAppearances.MrsChen,
				()	=>	Engine.DialogueSystem.StartYarnNode( "MrsChen" ));
		}

		private void SpawnDevonNpcs( GraphicsDevice gd )	=>
			SpawnNpc( gd, "Devon", new Vector2( 3.5f, 4.5f ), NpcAppearances.Devon,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Devon" ));

		private void SpawnJakeEmmaNpcs( GraphicsDevice gd )
		{
			SpawnNpc( gd, "Emma", new Vector2( 5.5f, 4.5f ), NpcAppearances.Emma,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Emma" ));
			SpawnNpc( gd, "Jake", new Vector2( 7.5f, 4.5f ), NpcAppearances.Jake,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Jake" ));
		}

		private void SpawnThompsonNpcs( GraphicsDevice gd )	=>
			SpawnNpc( gd, "Mr. Thompson", new Vector2( 3.5f, 4.5f ), NpcAppearances.MrThompson,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Thompson" ));

		private void SpawnSantosNpcs( GraphicsDevice gd )	=>
			SpawnNpc( gd, "Maria", new Vector2( 16.5f, 5.5f ), NpcAppearances.Maria,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Maria" ));

		private void SpawnPetrovNpcs( GraphicsDevice gd )
		{
			SpawnNpc( gd, "Mr. Petrov",  new Vector2( 3.5f, 5.5f ), NpcAppearances.MrPetrov,
				()	=>	Engine.DialogueSystem.StartYarnNode( "MrPetrov" ));
			SpawnNpc( gd, "Mrs. Petrov", new Vector2( 16.5f, 4.5f ), NpcAppearances.MrsPetrov,
				()	=>	Engine.DialogueSystem.StartYarnNode( "MrsPetrov" ));
		}

		private void SpawnSamNpcs( GraphicsDevice gd )	=>
			SpawnNpc( gd, "Linda", new Vector2( 5.5f, 4.5f ), NpcAppearances.Linda,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Linda" ));

		private void SpawnJohnsonNpcs( GraphicsDevice gd )
		{
			SpawnNpc( gd, "DeShonda", new Vector2( 4.5f, 6.5f ), NpcAppearances.DeShonda,
				()	=>	Engine.DialogueSystem.StartYarnNode( "DeShonda" ));
			SpawnNpc( gd, "Destiny",  new Vector2( 8.5f, 4.5f ), NpcAppearances.Destiny,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Destiny" ) );
			SpawnNpc( gd, "Tyler",    new Vector2( 18.5f, 4.5f ), NpcAppearances.Tyler,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Tyler" ) );
		}

		// ── Scene transitions ────────────────────────────────────────────────

		protected override void CheckSceneTransitions()
		{
			var pos =	PlayerPosition;
			if( pos.Y >=( MapH - 2 ) - 0.125f && pos.X > 9f && pos.X < 13f )
			{
				Transitioning =	true;
				Engine.LoadScene( new NeighborhoodScene() );
			}
		}
	}
}
