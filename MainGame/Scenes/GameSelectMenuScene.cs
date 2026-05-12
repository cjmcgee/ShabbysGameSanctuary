using ChildhoodAdventure.RetroSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine.Core;
using TileEngine.MiniGames;
using TileEngine.Rendering;

namespace ChildhoodAdventure.Scenes;

/// <summary>
/// Cartridge-select menu shown when the player interacts with the in-world
/// Atari console. Lists every <see cref="GameEntry"/> in the
/// <see cref="GameLibrary"/>; arrow keys (or W/S) navigate, E launches,
/// Escape returns to the home interior.
///
/// Unavailable entries (e.g. emulated games whose core / ROM the
/// configuration can't find on disk) render dimmed with their reason printed
/// inline, so misconfiguration is visible without launching anything.
/// </summary>
public sealed class GameSelectMenuScene :	Scene
{
	private readonly GameLibrary _library;
	private readonly Func<Scene>	_returnTo;
	private int _selection;
	private KeyboardState _prevKeys;
	private Texture2D?	_pixel;

	public GameSelectMenuScene(GameLibrary library, Func<Scene> returnTo)
	{
		_library =	library;
		_returnTo =	returnTo;
		Name =	"GameSelectMenu";
	}

	protected override void OnLoad()
	{
		_pixel =	new Texture2D(Engine.GraphicsDevice, 1, 1);
		_pixel.SetData(new[] { Color.White });
		_selection =	Math.Max(0, FirstAvailable());

		// Seed the previous-keys state with whatever's currently held so the
		// E press that opened this menu doesn't immediately register as a
		// fresh edge here and auto-select the first item.
		_prevKeys =	Keyboard.GetState();
	}

	protected override void OnUnload()
	{
		_pixel?.Dispose();
		_pixel =	null;
	}

	public override void Update(GameTime gameTime)
	{
		var keys =	Keyboard.GetState();
		bool down =	IsPressed(keys, Keys.Down) || IsPressed(keys, Keys.S);
		bool up =	IsPressed(keys, Keys.Up)   || IsPressed(keys, Keys.W);
		bool select =	IsPressed(keys, Keys.E)    || IsPressed(keys, Keys.Enter);
		bool cancel =	IsPressed(keys, Keys.Escape);
		bool configure =	IsPressed(keys, Keys.C);

		if (down)	Move(+1);
		if (up)		Move(-1);

		if (cancel)
		{
			Engine.LoadScene(_returnTo());
			return;
		}

		if (configure && HasUnavailableEntry())
		{
			// Reload-on-return: build a fresh library from disk so the menu
			// reflects whatever the user just configured (e.g. Combat flips
			// from greyed-out to playable as soon as RomRoot is valid).
			var returnHere =	_returnTo;
			Engine.LoadScene(new EmulatorConfigScene(
				EmulatorConfig.LoadOrDefault(),
				returnTo: () =>	new GameSelectMenuScene(
					new GameLibrary(EmulatorConfig.LoadOrDefault()),
					returnHere)));
			return;
		}

		if (select)
		{
			var entry =	_library.Games[_selection];
			if (entry.IsAvailable)
				Engine.LoadScene(new MiniGameScene(entry.Create(), _returnTo));
			// If unavailable, the on-screen reason already explains why; do
			// nothing — the user can press Escape or pick another title.
		}

		_prevKeys =	keys;
	}

	private bool HasUnavailableEntry()
	{
		for (int i = 0; i < _library.Games.Count; i++)
			if (!_library.Games[i].IsAvailable)	return true;
		return false;
	}

	private bool IsPressed(KeyboardState keys, Keys k)	=>
		keys.IsKeyDown(k) && !_prevKeys.IsKeyDown(k);

	private void Move(int delta)
	{
		int n =	_library.Games.Count;
		_selection =	((_selection + delta)	% n + n)	% n;
	}

	private int FirstAvailable()
	{
		for (int i = 0; i < _library.Games.Count; i++)
			if (_library.Games[i].IsAvailable)	return i;
		return 0;
	}

	public override void Draw(SpriteBatch sb, GameTime gameTime)
	{
		var sp =	RetroSystemRegistry.Current.ScenePalette;
		Engine.GraphicsDevice.Clear(sp.UiBackground);

		Engine.RenderSystem.BeginUI();
		var spriteBatch =	Engine.RenderSystem.SpriteBatch;
		var font =	Engine.RenderSystem.Font;
		var vp =	Engine.GraphicsDevice.Viewport;

		const float scale =	2.5f;
		const float reasonScale =	1.6f;
		float mainTextH =	PixelFont.CharH * scale;
		const float postReasonGap =	8f;
		const float postEntryGap =	14f;

		// Bright red so misconfiguration is loud, not lost under the dim
		// "disabled choice" colour. Stays the same across all retro systems
		// because it's a config-error message, not in-world UI flavor.
		var errorColor =	new Color(232, 80, 80);

		// Title.
		string title =	"ATARI 2600 — SELECT GAME";
		float titleW =	font.MeasureWidth(title)	* scale;
		font.DrawText(spriteBatch, title,
			new Vector2((vp.Width - titleW)	/ 2f, 60),
			sp.UiAccent, scale);

		// Menu items — y is accumulated so unavailable entries take the
		// extra vertical space their reason line needs.
		float menuLeft =	(vp.Width / 2f) - 200;
		float y =	120;
		for (int i = 0; i < _library.Games.Count; i++)
		{
			var entry =	_library.Games[i];
			bool selected =	(i == _selection);
			bool avail =	entry.IsAvailable;

			Color c =	!avail ? sp.UiDim
				:	selected ? sp.UiAccent :	sp.UiText;

			string prefix =	selected ? "▶  " : "    ";
			string suffix =	entry.IsEmulated ? "  (emulated)" :	"  (native)";
			string text =	$"{prefix}{entry.Name}{suffix}";
			font.DrawText(spriteBatch, text, new Vector2(menuLeft, y), c, scale);
			y +=	mainTextH;

			if (!avail && entry.UnavailableReason != null)
			{
				y +=	4;	// small gap between name and reason
				// Left-justified and wrapped to the viewport so long config
				// paths don't run off the right edge. Indent slightly from the
				// screen edge for readability; the menu items above are
				// centered, this line breaks that alignment intentionally.
				const float reasonLeft =	24f;
				float reasonMaxWidth =	vp.Width - reasonLeft * 2;
				float used =	font.DrawWrappedText(spriteBatch,
					"⚠ " + entry.UnavailableReason,
					new Vector2(reasonLeft, y), errorColor, reasonMaxWidth, reasonScale);
				y +=	used + postReasonGap;
			}

			y +=	postEntryGap;
		}

		// Footer hint. Mention C only when there is something to fix — no
		// point teaching the keystroke when every cartridge already works.
		string hint =	HasUnavailableEntry()
			? "↑/↓: select   E/Enter: load   C: configure ROM path   Esc: back"
			: "↑/↓: select   E/Enter: load   Esc: back";
		float hintW =	font.MeasureWidth(hint)	* scale * 0.8f;
		font.DrawText(spriteBatch, hint,
			new Vector2((vp.Width - hintW)	/ 2f, vp.Height - 60),
			sp.UiDim, scale * 0.8f);

		Engine.RenderSystem.EndUI();
	}
}
