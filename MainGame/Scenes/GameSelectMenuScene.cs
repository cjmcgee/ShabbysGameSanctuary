using ChildhoodAdventure.RetroSystems;
using ChildhoodAdventure.Scoring;
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
	private int _scrollOffset;
	private KeyboardState _prevKeys;
	private Texture2D?	_pixel;

	// Per-row vertical budget. With scale=2.5 the name itself is ~20px;
	// the extra is the gap between entries.
	private const float RowHeight =	34f;

	// Fixed strip above the key-hint footer where the selected-row's
	// UnavailableReason is rendered. The list shrinks to make room.
	private const float ReasonFooterHeight =	90f;

	// Distance from the top of the viewport to the first list row.
	private const float ListTop =	120f;

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
		bool pgDown =	IsPressed(keys, Keys.PageDown);
		bool pgUp =		IsPressed(keys, Keys.PageUp);
		bool home =		IsPressed(keys, Keys.Home);
		bool end =		IsPressed(keys, Keys.End);
		bool select =	IsPressed(keys, Keys.E)    || IsPressed(keys, Keys.Enter);
		bool cancel =	IsPressed(keys, Keys.Escape);
		bool configure =	IsPressed(keys, Keys.C);
		bool romManager =	IsPressed(keys, Keys.R);

		int n =	_library.Games.Count;
		if (n > 0)
		{
			// Bounded navigation (no wraparound — wrapping past 23 entries
			// in a scrollable list is more disorienting than useful).
			int page =	Math.Max(1, VisibleRows() - 1);
			if (down)	_selection =	Math.Min(n - 1, _selection + 1);
			if (up)		_selection =	Math.Max(0, _selection - 1);
			if (pgDown)	_selection =	Math.Min(n - 1, _selection + page);
			if (pgUp)	_selection =	Math.Max(0, _selection - page);
			if (home)	_selection =	0;
			if (end)	_selection =	n - 1;
		}

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

		// R opens the ROM Manager. Available whenever there's a RomRoot
		// configured to scan against — pressing R with no RomRoot would
		// just show 22 [MISSING] rows. Send the user to the path config
		// first in that case so they have something to manage.
		if (romManager)
		{
			var cfg =	EmulatorConfig.LoadOrDefault();
			var returnHere =	_returnTo;

			Scene returnScene() =>	new GameSelectMenuScene(
				new GameLibrary(EmulatorConfig.LoadOrDefault()),
				returnHere);

			if (string.IsNullOrEmpty(cfg.EffectiveRomRoot))
			{
				// Fall through to the path-config scene; from there the
				// user can pick a folder and then walk into the manager.
				Engine.LoadScene(new EmulatorConfigScene(cfg, returnScene));
			}
			else
			{
				Engine.LoadScene(new RomManagerScene(cfg, returnScene));
			}
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

	private int FirstAvailable()
	{
		for (int i = 0; i < _library.Games.Count; i++)
			if (_library.Games[i].IsAvailable)	return i;
		return 0;
	}

	// How many uniform rows fit between the title and the reason-footer.
	// Depends on viewport height; recomputed each frame so a resized
	// window auto-adjusts.
	private int VisibleRows()
	{
		var vp =	Engine.GraphicsDevice.Viewport;
		// 60px allowance for the key-hint line at the very bottom.
		float available =	vp.Height - ListTop - ReasonFooterHeight - 60f;
		return Math.Max(1, (int)(available / RowHeight));
	}

	// Slide the viewport so the selection stays visible. Called from Draw
	// (after navigation has updated _selection) and again uses the live
	// VisibleRows() value so a window resize doesn't strand the cursor.
	private void ClampScroll(int visible)
	{
		int n =	_library.Games.Count;
		if (_selection < _scrollOffset)
			_scrollOffset =	_selection;
		else if (_selection >= _scrollOffset + visible)
			_scrollOffset =	_selection - visible + 1;
		_scrollOffset =	Math.Max(0, Math.Min(_scrollOffset, Math.Max(0, n - visible)));
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
		const float scrollIndScale =	1.3f;

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

		// ── List: uniform single-line rows, scrolled to keep _selection visible
		int visible =	VisibleRows();
		ClampScroll(visible);

		float menuLeft =	(vp.Width / 2f) - 200;
		int endIdx =	Math.Min(_library.Games.Count, _scrollOffset + visible);

		// Soft gold for the high-score number. Picks the row's main colour
		// when the row is selected (so the cursor highlight stays
		// coherent) or when the row is unavailable (then everything is red).
		var scoreAccent =	new Color(220, 200, 80);

		for (int i = _scrollOffset; i < endIdx; i++)
		{
			var entry =	_library.Games[i];
			bool selected =	(i == _selection);
			bool avail =	entry.IsAvailable;
			float y =	ListTop + (i - _scrollOffset) * RowHeight;

			// Colour rules:
			//   selected + available   → accent  (active, ready)
			//   selected + missing     → bright red (active, broken)
			//   unselected + available → normal text
			//   unselected + missing   → red    (loud so 22 unresolved ROMs are obvious at a glance)
			Color c =	!avail
				?	errorColor
				:	(selected ? sp.UiAccent : sp.UiText);

			string prefix =	selected ? "▶  " : "    ";
			string typeTag =	entry.IsEmulated ? "  (emulated)" :	"  (native)";
			// The user-requested "(missing)" suffix takes precedence over
			// the type tag so the row reads as "Combat (missing)" rather
			// than "Combat (emulated) (missing)". The type info is less
			// useful when the entry is unplayable anyway.
			string suffix =	avail ? typeTag :	"  (missing)";

			// Look up the persisted best for this game. Zero means
			// "never recorded a score" — either we haven't tracked one
			// yet (game not played, or it has no formula in the
			// AtariScores.json catalog) or the user's first session
			// hasn't produced a non-zero score yet.
			long best =	Highscores.Instance.GetBest(entry.Name);

			// Layered draw: name → score → suffix. Three DrawText calls
			// so the score can render in its own colour while name and
			// suffix follow the row's main colour. Advance x by the
			// previous segment's measured width to keep the trio on a
			// single visual line at the pixel font's natural pitch.
			string namePart =	$"{prefix}{entry.Name}";
			font.DrawText(spriteBatch, namePart, new Vector2(menuLeft, y), c, scale);
			float x =	menuLeft + font.MeasureWidth(namePart) * scale;

			if (best > 0)
			{
				string scoreText =	$"   [{best:N0}]";
				// Selected rows keep the accent so the cursor reads as
				// one continuous highlight; otherwise the score gets its
				// own gold colour. Unavailable rows force red on
				// everything (a recorded score on a now-missing ROM is
				// still useful info, but the redness telegraphs why
				// you can't currently play to beat it).
				Color scoreCol =	!avail
					?	errorColor
					:	(selected ? sp.UiAccent : scoreAccent);
				font.DrawText(spriteBatch, scoreText, new Vector2(x, y), scoreCol, scale);
				x +=	font.MeasureWidth(scoreText) * scale;
			}

			font.DrawText(spriteBatch, suffix, new Vector2(x, y), c, scale);
		}

		// Scroll indicators — only when there's actually content above/below.
		if (_scrollOffset > 0)
		{
			font.DrawText(spriteBatch, "▲ more above",
				new Vector2(menuLeft, ListTop - PixelFont.CharH * scrollIndScale - 4),
				sp.UiDim, scrollIndScale);
		}
		if (endIdx < _library.Games.Count)
		{
			float belowY =	ListTop + visible * RowHeight + 2;
			font.DrawText(spriteBatch, "▼ more below",
				new Vector2(menuLeft, belowY),
				sp.UiDim, scrollIndScale);
		}

		// ── Reason footer: only the selected row's reason, wrapped.
		// Avoids the visual wall that came from rendering 22 identical
		// "ROM 'X' not found" lines inline in the list.
		var selectedEntry =	_library.Games.Count > 0 ?	_library.Games[_selection] :	null;
		if (selectedEntry != null && !selectedEntry.IsAvailable && selectedEntry.UnavailableReason != null)
		{
			const float reasonLeft =	24f;
			float reasonMaxWidth =	vp.Width - reasonLeft * 2;
			float reasonTop =	vp.Height - ReasonFooterHeight - 30f;
			font.DrawWrappedText(spriteBatch,
				"⚠ " + selectedEntry.UnavailableReason,
				new Vector2(reasonLeft, reasonTop),
				errorColor, reasonMaxWidth, reasonScale);
		}

		// Footer hint. C and R are advanced/diagnostic; only surface them
		// when there's actually something for them to fix or inspect.
		string hint =	HasUnavailableEntry()
			? "↑/↓: select   PgUp/PgDn: jump   E/Enter: load   C: configure paths   R: ROM manager   Esc: back"
			: "↑/↓: select   PgUp/PgDn: jump   E/Enter: load   R: ROM manager   Esc: back";
		float hintW =	font.MeasureWidth(hint)	* scale * 0.7f;
		font.DrawText(spriteBatch, hint,
			new Vector2((vp.Width - hintW)	/ 2f, vp.Height - 30),
			sp.UiDim, scale * 0.7f);

		Engine.RenderSystem.EndUI();
	}
}
