using ChildhoodAdventure.RetroSystems;
using ChildhoodAdventure.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine.Core;

namespace ChildhoodAdventure
{
    public class AdventureGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private GameEngine _engine = null!;

        public AdventureGame()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth  = 800,
                PreferredBackBufferHeight = 600,
                IsFullScreen = false
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            UpdateWindowTitle();
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;
        }

        private void OnClientSizeChanged(object? sender, EventArgs e)
        {
            var bounds = Window.ClientBounds;
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Do NOT call _graphics.ApplyChanges() here — that invokes SDL_SetWindowSize,
            // which re-fires ClientSizeChanged and creates an oscillating feedback loop.
            // On OpenGL DesktopGL the default framebuffer already tracks the OS window;
            // we just need to sync MonoGame's bookkeeping so render-target switches
            // reset the viewport to the correct dimensions.
            var pp = GraphicsDevice.PresentationParameters;
            pp.BackBufferWidth  = bounds.Width;
            pp.BackBufferHeight = bounds.Height;
            GraphicsDevice.Viewport = new Viewport(0, 0, bounds.Width, bounds.Height);

            _engine?.OnViewportChanged();
        }

        protected override void LoadContent()
        {
            _engine = GameEngine.Initialize(GraphicsDevice, Content);
            _engine.LoadScene(new HomeInteriorScene());
        }

        private bool _updateErr, _drawErr;
        private KeyboardState _prevKeys;

        protected override void Update(GameTime gameTime)
        {
            try
            {
                var keys = Keyboard.GetState();

                if (keys.IsKeyDown(Keys.Escape)) Exit();

                // F1-F5: switch retro system and reload current scene
                HandleSystemSwitch(keys);

                _engine.Update(gameTime);
                base.Update(gameTime);
                _prevKeys = keys;
            }
            catch (Exception ex)
            {
                if (!_updateErr) { Console.WriteLine($"[Update] {ex}"); _updateErr = true; }
            }
        }

        private void HandleSystemSwitch(KeyboardState keys)
        {
            int? newIdx = null;
            if (keys.IsKeyDown(Keys.F1) && !_prevKeys.IsKeyDown(Keys.F1)) newIdx = 0;
            if (keys.IsKeyDown(Keys.F2) && !_prevKeys.IsKeyDown(Keys.F2)) newIdx = 1;
            if (keys.IsKeyDown(Keys.F3) && !_prevKeys.IsKeyDown(Keys.F3)) newIdx = 2;
            if (keys.IsKeyDown(Keys.F4) && !_prevKeys.IsKeyDown(Keys.F4)) newIdx = 3;
            if (keys.IsKeyDown(Keys.F5) && !_prevKeys.IsKeyDown(Keys.F5)) newIdx = 4;

            if (newIdx.HasValue && newIdx.Value != RetroSystemRegistry.CurrentIndex)
            {
                RetroSystemRegistry.SetSystem(newIdx.Value);
                UpdateWindowTitle();
                ReloadCurrentScene();
            }
        }

        private void ReloadCurrentScene()
        {
            _engine.LoadScene(GameState.ActiveScene switch
            {
                GameState.SceneType.Home             => (TileEngine.Core.Scene)new HomeInteriorScene(),
                GameState.SceneType.Neighborhood     => new NeighborhoodScene(),
                GameState.SceneType.NeighborInterior => new NeighborInteriorScene(),
                _                                    => new HomeInteriorScene(),
            });
        }

        private void UpdateWindowTitle()
        {
            var sys = RetroSystemRegistry.Current;
            Window.Title =
                $"Childhood Adventure  [{sys.Name}  {sys.Description}]  " +
                $"  WASD: Move  |  E: Talk  |  F1-F5: Switch System  |  Esc: Quit";
        }

        protected override void Draw(GameTime gameTime)
        {
            try
            {
                _engine.Draw(gameTime);
                base.Draw(gameTime);
            }
            catch (Exception ex)
            {
                if (!_drawErr) { Console.WriteLine($"[Draw] {ex}"); _drawErr = true; }
            }
        }

        protected override void UnloadContent()
        {
            _engine.Dispose();
            base.UnloadContent();
        }
    }
}
