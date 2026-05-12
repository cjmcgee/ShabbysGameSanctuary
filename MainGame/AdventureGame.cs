using System.IO;
using System.Reflection;
using ChildhoodAdventure.RetroSystems;
using ChildhoodAdventure.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine.Core;
using TileEngine.MiniGames;

namespace ChildhoodAdventure
{
	public class AdventureGame : Game
	{
		internal const string DialogueBundleResource = "ChildhoodAdventure.dialogue.bundle.gz";

		private readonly GraphicsDeviceManager _graphics;
		private GameEngine _engine = null!;

		public AdventureGame()
		{
			// Bring up the logger before anything else so even early-init
			// failures (graphics device, content root, etc.) land in the
			// per-user log file. Same per-user dir convention as
			// EmulatorConfig: %APPDATA%\ChildhoodAdventure\ on Windows,
			// ~/.config/ChildhoodAdventure/ on Linux.
			var logFile =	Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"ChildhoodAdventure", "Logs", "game.log");
			Log.Configure(logFile);
			Log.WriteSessionHeader(
				appName:	"ChildhoodAdventure",
				version:	Assembly.GetExecutingAssembly().GetName().Version?.ToString());

			_graphics =	new GraphicsDeviceManager( this )
			{
				PreferredBackBufferWidth  =	800,
				PreferredBackBufferHeight =	600,
				IsFullScreen =	false
			};
			Content.RootDirectory =	"Content";
			IsMouseVisible = true;

			// Variable timestep with vsync. With the default fixed timestep,
			// if any single Update tick exceeds the 16.67ms target (e.g.
			// libretro core init + first emulator frame can take 100s of ms),
			// MonoGame enters a catch-up loop and stops calling Draw while it
			// tries to catch up — leaving the screen frozen on whatever was
			// last rendered. Variable timestep guarantees Update→Draw→Update
			// alternation regardless of per-frame cost.
			IsFixedTimeStep = false;
			_graphics.SynchronizeWithVerticalRetrace = true;
		}

		protected override void Initialize()
		{
			base.Initialize();
			UpdateWindowTitle();
			Window.AllowUserResizing =	true;
			Window.ClientSizeChanged +=	OnClientSizeChanged;
		}

		private void OnClientSizeChanged( object? sender, EventArgs e )
		{
			var bounds = Window.ClientBounds;
			if( bounds.Width <= 0 || bounds.Height <= 0 ) { return; }

			// Do NOT call _graphics.ApplyChanges() here — that invokes SDL_SetWindowSize,
			// which re-fires ClientSizeChanged and creates an oscillating feedback loop.
			// On OpenGL DesktopGL the default framebuffer already tracks the OS window;
			// we just need to sync MonoGame's bookkeeping so render-target switches
			// reset the viewport to the correct dimensions.
			var pp 					= GraphicsDevice.PresentationParameters;
			pp.BackBufferWidth		= bounds.Width;
			pp.BackBufferHeight 	= bounds.Height;
			GraphicsDevice.Viewport = new Viewport( 0, 0, bounds.Width, bounds.Height );

			_engine?.OnViewportChanged();
		}

		protected override void LoadContent()
		{
			_engine = GameEngine.Initialize( GraphicsDevice, Content );
			_engine.LoadScene( new HomeInteriorScene() );
		}

		private bool _updateErr;
		private bool _drawErr;
		private KeyboardState _prevKeys;
		private int _prevScrollWheel;
		private bool _miniGameTitleActive;

		protected override void Update( GameTime gameTime )
		{
			try
			{
				var keys   = Keyboard.GetState();
				var mouse  = Mouse.GetState();

				// Escape quits the program — but NOT while a mini-game is hosted,
				// and NOT inside the in-game menus (game select, emulator config).
				// MiniGameScene routes Escape to the embedded game; the menu scenes
				// route it to "back" via their own handlers. If we Exit() here on
				// the same frame those scenes try to navigate, the quit wins.
				//
				// Edge-triggered (press, not hold): a single Escape press while
				// in the mini-game routes through MiniGameScene; the scene swap
				// back to Home completes within a few frames, often before the
				// user has released Escape. Without edge-triggering, the still-
				// held Escape would re-enter this branch with CurrentScene now
				// being Home and quit the program immediately.
				bool escapePressed = keys.IsKeyDown( Keys.Escape ) && !_prevKeys.IsKeyDown( Keys.Escape );
				var scene =	TileEngine.Core.GameEngine.Instance.CurrentScene;
				if( escapePressed
					&& scene is not MiniGameScene
					&& scene is not GameSelectMenuScene
					&& scene is not EmulatorConfigScene ) { Exit(); }

				// F1-F5: switch retro system and reload current scene
				HandleSystemSwitch( keys );

				// Scroll wheel: zoom camera in/out
				HandleScrollZoom( mouse );

				_engine.Update( gameTime );
				base.Update( gameTime );
				_prevKeys =	keys;
				_prevScrollWheel = mouse.ScrollWheelValue;

				// Title-bar key hint follows the active scene: world controls
				// in the overworld, console-switch controls inside a hosted
				// libretro game. Only refresh on transition so we're not
				// thrashing SDL_SetWindowTitle every frame.
				bool inMiniGame = TileEngine.Core.GameEngine.Instance.CurrentScene is MiniGameScene;
				if( inMiniGame != _miniGameTitleActive ) { UpdateWindowTitle(); }
			}
			catch( Exception ex )
			{
				if( !_updateErr )
				{
					Log.Error( "Update", ex.ToString() );
					_updateErr = true;
				}
			}
		}

		private void HandleScrollZoom( MouseState mouse )
		{
			int delta =	mouse.ScrollWheelValue - _prevScrollWheel;
			if( delta == 0 ) { return; }

			var camera = _engine.RenderSystem.Camera;
			// Each scroll notch( ±120 ) changes the visible-tile count by ~10%.
			// Scrolling up zooms IN( fewer tiles visible ).
			float factor = delta >	0 ? 1f / 1.1f :	1.1f;
			int notches = Math.Abs( delta )	/ 120;
			for( int i = 0; i < Math.Max( notches, 1 ); i++ )
			{
				camera.TilesTall *=	factor;
			}
		}

		private void HandleSystemSwitch( KeyboardState keys )
		{
			int? newIdx = null;
			if( keys.IsKeyDown( Keys.F1 ) && !_prevKeys.IsKeyDown( Keys.F1 )) newIdx = 0;
			if( keys.IsKeyDown( Keys.F2 ) && !_prevKeys.IsKeyDown( Keys.F2 )) newIdx = 1;
			if( keys.IsKeyDown( Keys.F3 ) && !_prevKeys.IsKeyDown( Keys.F3 )) newIdx = 2;
			if( keys.IsKeyDown( Keys.F4 ) && !_prevKeys.IsKeyDown( Keys.F4 )) newIdx = 3;
			if( keys.IsKeyDown( Keys.F5 ) && !_prevKeys.IsKeyDown( Keys.F5 )) newIdx = 4;

			if( newIdx.HasValue && newIdx.Value != RetroSystemRegistry.CurrentIndex )
			{
				RetroSystemRegistry.SetSystem( newIdx.Value );
				UpdateWindowTitle();
				ReloadCurrentScene();
			}
		}

		private void ReloadCurrentScene()
		{
			_engine.LoadScene( GameState.ActiveScene switch
			{
				GameState.SceneType.Home				=> new HomeInteriorScene(),
				GameState.SceneType.Neighborhood		=> new NeighborhoodScene(),
				GameState.SceneType.NeighborInterior	=> new NeighborInteriorScene(),
				_										=> new HomeInteriorScene(),
			} );
		}

		private void UpdateWindowTitle()
		{
			var sys = RetroSystemRegistry.Current;
			bool inMiniGame = TileEngine.Core.GameEngine.Instance?.CurrentScene is MiniGameScene;
			_miniGameTitleActive = inMiniGame;

			string controls = inMiniGame
				? "WASD/Arrows: Move  |  Space: Fire  |  Tab: Game Select  |  Enter: Game Reset  |  Esc: Exit Game"
				: "WASD: Move  |  E: Talk  |  Scroll: Zoom  |  F1-F5: Switch System  |  Esc: Quit";

			Window.Title =
				$"Childhood Adventure  [{sys.Name}  {sys.Description}]    {controls}";
		}

		protected override void Draw( GameTime gameTime )
		{
			try
			{
				_engine.Draw( gameTime );
				base.Draw( gameTime );
			}
			catch( Exception ex )
			{
				if( !_drawErr )
				{
					Log.Error( "Draw", ex.ToString() );
					_drawErr = true;
				}
			}
		}

		protected override void UnloadContent()
		{
			_engine.Dispose();
			base.UnloadContent();
		}
	}
}
