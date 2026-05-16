using TileEngine.Components;
using TileEngine.ECS;
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
	internal sealed class HomeInteriorScene : AdventureScene
	{
		// ── Tile GIDs (1-based) ──────────────────────────────────────────────
		private const int T_WOOD		= 1;	// warm wood floor
		private const int T_CARPET		= 2;	// red-orange carpet (living room)
		private const int T_KITCHEN		= 3;	// pale cream kitchen tile
		private const int T_WALL		= 4;	// plaster wall
		private const int T_DOOR		= 5;	// dark-wood front door (walkable)
		private const int T_FURN		= 6;	// dark furniture (sofa, table, bookshelf)

		private const int MapW			= 24;
		private const int MapH			= 18;

		// Warm tone for the player's home — pulls from the active system's palette.
		protected override Color DialogueBorderColor => RetroSystemRegistry.Current.ScenePalette.HouseYellow;

		// ── Scene load ───────────────────────────────────────────────────────

		protected override void OnSceneLoad()
		{
			Name 					= "Home";
			GameState.ActiveScene	= GameState.SceneType.Home;
			var gd					= Engine.GraphicsDevice;
			var sys					= RetroSystemRegistry.Current;

			var tileset = sys.BuildTileset( gd, "home", new[]
			{
				TileType.WoodFloor,		// 1
				TileType.Carpet,		// 2
				TileType.KitchenTile,	// 3
				TileType.Wall,			// 4
				TileType.Door,			// 5
				TileType.Furniture,		// 6
				TileType.Counter,		// 7
				TileType.Window,		// 8
			} );

			var tilemap = new Tilemap( "home", MapW, MapH )
			{
				BackgroundColor =	Color.Black
			};
			tilemap.AddTileset( tileset );
			BuildHomeMap( tilemap );

			Engine.CollisionSystem.SetTilemap( tilemap );
			Engine.RenderSystem.TilemapRenderer.SetTilemap( tilemap );
			Engine.RenderSystem.Camera.Bounds			= new RectangleF(0, 0, tilemap.Width, tilemap.Height);
			Engine.RenderSystem.Camera.MaxTilesTall 	= sys.MaxTilesTall;
			Engine.RenderSystem.Camera.TilesTall		= sys.DefaultTilesTall;
			Engine.RenderSystem.LightingSystem.Enabled	= false;

			SpawnPlayer( gd, GameState.PlayerSpawnPosition );
			SpawnDad( gd );
			SpawnMom( gd );
			SpawnJamie( gd );
			SpawnAtariConsole( gd );
		}

		// ── Map builder ──────────────────────────────────────────────────────

		private static void BuildHomeMap(Tilemap map)
		{
			var bg	= map.AddLayer( "bg",  LayerType.Background );
			var mid = map.AddLayer( "mid", LayerType.Midground );
			var col = map.AddLayer( "col", LayerType.Collision );

			FillRect( bg, 1,  1, 22, 16, T_WOOD );
			FillRect( bg, 1,  1,  9, 13, T_CARPET );
			FillRect( bg, 12, 1, 11, 13, T_KITCHEN );

			FillRow( mid, col, 0,       T_WALL );
			FillRow( mid, col, MapH-1,  T_WALL );
			FillCol( mid, col, 0,       T_WALL );
			FillCol( mid, col, MapW-1,  T_WALL );

			for( int y = 1; y <= 7; y++ )
			{ 
				mid.SetTile(11, y, T_WALL); 
				col.SetTile(11, y, 1); 
			}

			mid.SetTile( 11, MapH-1, T_DOOR );
			col.SetTile( 11, MapH-1, 0 );
			mid.SetTile( 12, MapH-1, T_DOOR );	
			col.SetTile( 12, MapH-1, 0 );

			PlaceFurniture( mid, col, 1, 1, 3, 1 );
			PlaceFurniture( mid, col, 2, 3, 3, 1 );
			PlaceFurniture( mid, col, 3, 5, 1, 1 );
			PlaceFurniture( mid, col, 1, 7, 1, 2 );
			mid.SetTile( 9, 9, T_FURN );
			col.SetTile( 9, 9, 1 );

			PlaceFurniture( mid, col, 13, 1, 8, 1 );
			PlaceFurniture( mid, col, 22, 1, 1, 3 );
			col.SetTile( 19, 1, 0 );
			col.SetTile( 20, 1, 0 );

			PlaceFurniture( mid, col, 14, 6, 3, 2 );
			mid.SetTile( 13, 6, T_FURN );
			mid.SetTile( 13, 7, T_FURN );
			mid.SetTile( 18, 6, T_FURN );
			mid.SetTile( 18, 7, T_FURN );
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

		private static void FillRow( TileLayer mid, TileLayer col, int y, int gid )
		{
			for( int x = 0; x < MapW; x++ )
			{ 
				mid.SetTile( x, y, gid );
				col.SetTile( x, y, 1 ); 
			}
		}

		private static void FillCol( TileLayer mid, TileLayer col, int x, int gid )
		{
			for( int y = 0; y < MapH; y++ )
			{ 
				mid.SetTile( x, y, gid ); 
				col.SetTile( x, y, 1 ); 
			}
		}

		private static void PlaceFurniture( TileLayer mid, TileLayer col, int x, int y, int w, int h )
		{
			for( int dy = 0; dy < h; dy++ )
			{
				for( int dx = 0; dx < w; dx++ )
				{
					mid.SetTile( x + dx, y + dy, T_FURN );
					col.SetTile( x + dx, y + dy, 1 );
				}
			}
		}

		// ── NPC spawners ──────────────────────────────────────────────────────

		private void SpawnDad( GraphicsDevice gd ) =>
			SpawnNpc( gd, "Dad", new Vector2( 5.5f, 6.5f ), NpcAppearances.Dad,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Dad" ));

		private void SpawnMom( GraphicsDevice gd ) =>
			SpawnNpc( gd, "Mom", new Vector2( 17.5f, 3.5f ), NpcAppearances.Mom,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Mom" ) );

		private void SpawnJamie( GraphicsDevice gd )	=>
			SpawnNpc( gd, "Jamie", new Vector2( 3.5f, 11.5f ), NpcAppearances.Jamie,
				()	=>	Engine.DialogueSystem.StartYarnNode( "Jamie" ) );

		// ── Atari 2600 console ────────────────────────────────────────────────

		private Entity SpawnAtariConsole( GraphicsDevice gd )
		{
			// Sits on the carpet near the sofa — a kid's natural play spot.
			var pos = new Vector2( 5.5f, 9.5f );
			// Per-system styled icon: each retro system supplies its own palette
			// so the console fits Atari/C64/Apple II/CGA/NES aesthetics.
			var sprite = RetroSystemRegistry.Current.BuildAtariConsoleSprite( gd );

			var e =	Engine.EntityWorld.CreateEntity( "AtariConsole" );
			e.AddComponent( new TransformComponent( pos ) );
			var sc = e.AddComponent( new SpriteComponent { Sprite = sprite } );
			// SpriteComponent dispatches an "idle_<facing>" animation each
			// frame; without Play(...) the base name is null and AnimatedSprite
			// has no CurrentFrame to render.
			sc.Play( "idle" );
			// Small, solid hitbox so the player can't walk over the console.
			e.AddComponent( new CollisionComponent( 0.9f, 0.4f, new Vector2( -0.45f, -0.2f ) ) { IsSolid = true } );
			e.AddComponent( new InteractableComponent(
				onInteract:	_ =>	LaunchAtariGame(),
				range:		1.4f,
				prompt:		"Play Atari" ) );

			Engine.EntityWorld.RegisterTag( "atari_console", e );
			return e;
		}

		/// <summary>
		/// Launch the Atari game-select menu. The menu lists every entry in
		/// the configured <see cref="GameLibrary"/> (currently Battleshoot
		/// native + Combat emulated via libretro) and lets the player pick
		/// one; selecting an entry hands off to <see cref="MiniGameScene"/>.
		/// </summary>
		private void LaunchAtariGame()
		{
			// Snapshot the player's current spot so they return next to the
			// console rather than at the front-door spawn.
			GameState.PlayerSpawnPosition =	PlayerPosition;

			var config =	EmulatorConfig.LoadOrDefault();
			var library =	new GameLibrary( config );
			Engine.LoadScene( new GameSelectMenuScene( library, returnTo: () => new HomeInteriorScene() ) );
		}

		// ── Scene transitions ────────────────────────────────────────────────

		protected override void CheckSceneTransitions()
		{
			var pos = PlayerPosition;
			if( pos.Y >= (MapH - 2) - 0.125f && pos.X > 10f && pos.X < 14f )
			{
				Transitioning =	true;
				// Land on the sidewalk (row 10) just south of the player's house door.
				GameState.NeighborhoodReturnPosition =	new Vector2( 40.5f, 10.5f );
				Engine.LoadScene( new NeighborhoodScene() );
			}
		}
	}
}
