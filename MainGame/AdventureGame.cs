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
            Window.Title = "Childhood Adventure  —  WASD: Move  |  E: Talk  |  Esc: Quit";
            Window.AllowUserResizing = false;
        }

        protected override void LoadContent()
        {
            _engine = GameEngine.Initialize(GraphicsDevice, Content);
            _engine.LoadScene(new HomeInteriorScene());
        }

        private bool _updateErr, _drawErr;

        protected override void Update(GameTime gameTime)
        {
            try
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
                _engine.Update(gameTime);
                base.Update(gameTime);
            }
            catch (Exception ex)
            {
                if (!_updateErr) { Console.WriteLine($"[Update] {ex}"); _updateErr = true; }
            }
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
