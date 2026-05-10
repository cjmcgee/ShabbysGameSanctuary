using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine.MiniGames;
using TileEngine.Rendering;

namespace Battleshoot;

/// <summary>
/// MonoGame host that runs <see cref="BattleshootGame"/> in its own window.
/// Used when Battleshoot.exe is launched directly (rather than embedded in
/// ChildhoodAdventure as a <see cref="MiniGameScene"/>).
///
/// The host's only job is to own the GraphicsDeviceManager / Window / input,
/// pump the embeddable interface each frame, and exit when the game says
/// it's done.
/// </summary>
public sealed class StandaloneHost :	Game
{
	private readonly GraphicsDeviceManager _gdm;
	private readonly IEmbeddedMiniGame _game;
	private readonly KeyboardMiniGameInput _input =	new();
	private SpriteBatch _sb =	null!;

	public StandaloneHost()
	{
		_game =	new BattleshootGame();
		_gdm =	new GraphicsDeviceManager(this)
		{
			// 320×192 native display × 4 = 1280×768. Atari 2600 pixels are
			// double-wide on a real screen, so the display width is
			// DisplayWidth (320), not FieldWidth (160).
			PreferredBackBufferWidth =	BattleshootGame.DisplayWidth * 4,
			PreferredBackBufferHeight =	BattleshootGame.FieldHeight * 4,
			SynchronizeWithVerticalRetrace =	true,
		};
		IsMouseVisible =	false;
		Window.Title =	_game.Title;
		Content.RootDirectory =	"Content";
	}

	protected override void Initialize()
	{
		base.Initialize();
	}

	protected override void LoadContent()
	{
		_sb =	new SpriteBatch(GraphicsDevice);
		_game.Initialize(GraphicsDevice, Content);
	}

	protected override void Update(GameTime gameTime)
	{
		_input.Update();

		// ESC closes the standalone window directly — no parent to return to.
		if (_input.IsKeyPressed(Keys.Escape))	_input.ExitRequested =	true;

		_game.Update(gameTime, _input);
		if (_game.IsFinished)	Exit();

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.Black);
		var vp =	GraphicsDevice.Viewport;
		_sb.Begin(samplerState:	SamplerState.PointClamp);
		_game.Draw(_sb, new RectangleF(0, 0, vp.Width, vp.Height));
		_sb.End();
		base.Draw(gameTime);
	}

	protected override void OnExiting(object? sender, ExitingEventArgs args)
	{
		_game.Shutdown();
		base.OnExiting(sender, args);
	}
}
