using ChildhoodAdventure.RetroSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine.Core;
using TileEngine.Rendering;

namespace ChildhoodAdventure.Scenes;

/// <summary>
/// Per-ROM status & override manager. Lists every spec from
/// <see cref="GameLibrary.RequiredRoms"/>, shows how each one resolved
/// against the configured ROM folder (hash / name / override / missing),
/// and lets the player browse for a specific file when the auto-match
/// is wrong or absent. Selections persist through
/// <see cref="EmulatorConfig.RomOverrides"/>.
///
/// Re-resolves after every change so the badges stay in sync with what
/// the cartridge-select screen will load on return.
/// </summary>
public sealed class RomManagerScene :	Scene
{
	private readonly Func<Scene>	_returnTo;
	private EmulatorConfig			_config;
	private IReadOnlyList<RomMatch>	_matches =	Array.Empty<RomMatch>();

	private int				_selection;
	private int				_scrollOffset;
	private KeyboardState	_prevKeys;
	private string?			_statusMessage;
	private double			_statusFadeAt;
	private double			_now;

	public RomManagerScene(EmulatorConfig config, Func<Scene> returnTo)
	{
		_config =	config;
		_returnTo =	returnTo;
		Name =	"RomManager";
	}

	protected override void OnLoad()
	{
		Refresh();
		// Swallow whatever key is currently held (the R that brought us
		// here, or Enter in a sub-menu) so it doesn't immediately register
		// as a fresh edge in this scene's Update.
		_prevKeys =	Keyboard.GetState();
	}

	protected override void OnUnload() {}

	// Re-run the resolver and re-clamp the selection in case the new
	// match-set is shorter (shouldn't happen — spec list is static —
	// but defensive against future schema changes).
	private void Refresh()
	{
		_matches =	RomResolver.Resolve(
			GameLibrary.RequiredRoms,
			_config.EffectiveRomRoot,
			_config.RomOverrides);

		if (_selection >= _matches.Count)	_selection =	Math.Max(0, _matches.Count - 1);
	}

	public override void Update(GameTime gameTime)
	{
		_now =	gameTime.TotalGameTime.TotalSeconds;

		var keys =	Keyboard.GetState();
		bool down =	IsPressed(keys, Keys.Down) || IsPressed(keys, Keys.S);
		bool up =	IsPressed(keys, Keys.Up)   || IsPressed(keys, Keys.W);
		bool pgDown =	IsPressed(keys, Keys.PageDown);
		bool pgUp =		IsPressed(keys, Keys.PageUp);
		bool home =		IsPressed(keys, Keys.Home);
		bool end =		IsPressed(keys, Keys.End);
		bool select =	IsPressed(keys, Keys.Enter) || IsPressed(keys, Keys.E);
		bool clear =	IsPressed(keys, Keys.C) || IsPressed(keys, Keys.Delete) || IsPressed(keys, Keys.Back);
		bool cancel =	IsPressed(keys, Keys.Escape);
		bool refresh =	IsPressed(keys, Keys.F5) || IsPressed(keys, Keys.R);

		_prevKeys =	keys;

		if (cancel)
		{
			Engine.LoadScene(_returnTo());
			return;
		}

		if (_matches.Count > 0)
		{
			int page =	Math.Max(1, VisibleRows() - 1);
			if (down)	_selection =	Math.Min(_matches.Count - 1, _selection + 1);
			if (up)		_selection =	Math.Max(0, _selection - 1);
			if (pgDown)	_selection =	Math.Min(_matches.Count - 1, _selection + page);
			if (pgUp)	_selection =	Math.Max(0, _selection - page);
			if (home)	_selection =	0;
			if (end)	_selection =	_matches.Count - 1;
		}

		if (select)	BrowseSelected();
		if (clear)	ClearSelected();

		// Manual rescan: needed when files change on disk while this scene
		// is open (the resolver only runs on OnLoad and after Browse/Clear,
		// so external file additions / deletions don't show up until the
		// user asks for a refresh). Cheap — same code path as OnLoad.
		if (refresh)
		{
			Refresh();
			ShowStatus("Rescanned ROM folder.");
		}

		if (_statusMessage != null && _now >= _statusFadeAt)	_statusMessage =	null;
	}

	private void BrowseSelected()
	{
		if (_matches.Count == 0)	return;
		var match =	_matches[_selection];

		// Open the picker at the currently-resolved file's directory if we
		// have one, otherwise at RomRoot, otherwise wherever the OS picks.
		string? start =	null;
		if (!string.IsNullOrEmpty(match.ResolvedPath))
			start =	Path.GetDirectoryName(match.ResolvedPath);
		if (string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(_config.EffectiveRomRoot))
			start =	_config.EffectiveRomRoot;

		var filters =	new[]
		{
			new FileFilter("Atari 2600 ROMs", "*.bin;*.a26;*.rom"),
			new FileFilter("All files",       "*.*"),
		};

		string? picked =	NativeFilePicker.PickFile(
			$"Select ROM for {match.Spec.Name}", start, filters);
		if (picked == null)	return;

		_config.RomOverrides[match.Spec.Name] =	picked;
		bool saved =	_config.Save();
		Refresh();

		ShowStatus(saved
			? $"Override saved: {match.Spec.Name} → {Path.GetFileName(picked)}"
			: $"Could not write {EmulatorConfig.UserConfigPath}");
	}

	private void ClearSelected()
	{
		if (_matches.Count == 0)	return;
		var match =	_matches[_selection];

		if (!_config.RomOverrides.ContainsKey(match.Spec.Name))
		{
			ShowStatus($"No override to clear on {match.Spec.Name}.");
			return;
		}

		_config.RomOverrides.Remove(match.Spec.Name);
		bool saved =	_config.Save();
		Refresh();

		ShowStatus(saved
			? $"Cleared override on {match.Spec.Name}."
			: $"Could not write {EmulatorConfig.UserConfigPath}");
	}

	private void ShowStatus(string msg)
	{
		_statusMessage =	msg;
		_statusFadeAt =	_now + 3.0;
	}

	private bool IsPressed(KeyboardState keys, Keys k) =>
		keys.IsKeyDown(k) && !_prevKeys.IsKeyDown(k);

	// ── Rendering ─────────────────────────────────────────────────────────

	// Vertical space in pixels reserved for one row, including the gap.
	private const float RowHeight =	28f;

	private int VisibleRows()
	{
		var vp =	Engine.GraphicsDevice.Viewport;
		float available =	vp.Height - HeaderHeight - FooterHeight;
		return Math.Max(1, (int)(available / RowHeight));
	}

	private const float HeaderHeight =	130f;
	private const float FooterHeight =	170f;

	public override void Draw(SpriteBatch sb, GameTime gameTime)
	{
		var sp =	RetroSystemRegistry.Current.ScenePalette;
		Engine.GraphicsDevice.Clear(sp.UiBackground);

		Engine.RenderSystem.BeginUI();
		var spriteBatch =	Engine.RenderSystem.SpriteBatch;
		var font =	Engine.RenderSystem.Font;
		var vp =	Engine.GraphicsDevice.Viewport;

		const float titleScale =	2.5f;
		const float rowScale =		1.6f;
		const float hintScale =		1.2f;

		float titleH =	PixelFont.CharH * titleScale;
		float rowH =	PixelFont.CharH * rowScale;

		// Title
		string title =	"ROM MANAGER";
		float titleW =	font.MeasureWidth(title) * titleScale;
		font.DrawText(spriteBatch, title,
			new Vector2((vp.Width - titleW) / 2f, 35),
			sp.UiAccent, titleScale);

		// Subtitle: tallied counts by resolution category.
		string subtitle =	BuildSubtitle();
		float subW =	font.MeasureWidth(subtitle) * hintScale * 1.3f;
		font.DrawText(spriteBatch, subtitle,
			new Vector2((vp.Width - subW) / 2f, 35 + titleH + 12),
			sp.UiDim, hintScale * 1.3f);

		// ── List ───────────────────────────────────────────────────────
		int visible =	VisibleRows();
		ClampScroll(visible);

		float listLeft =	50f;
		float listTop =	HeaderHeight;
		float badgeW =	font.MeasureWidth("[OVERR]") * rowScale + 10f;
		float nameW =	font.MeasureWidth(new string('M', 26)) * rowScale;

		int end =	Math.Min(_matches.Count, _scrollOffset + visible);
		for (int i = _scrollOffset; i < end; i++)
		{
			var match =	_matches[i];
			bool selected =	(i == _selection);
			float y =	listTop + (i - _scrollOffset) * RowHeight;

			// Selection indicator
			string marker =	selected ? "▶ " : "  ";
			font.DrawText(spriteBatch, marker,
				new Vector2(listLeft - 24, y),
				selected ? sp.UiAccent :	sp.UiText, rowScale);

			// Status badge (coloured)
			var (badge, badgeColor) =	BadgeFor(match.Resolution);
			font.DrawText(spriteBatch, badge,
				new Vector2(listLeft, y), badgeColor, rowScale);

			// Title
			font.DrawText(spriteBatch, match.Spec.Name,
				new Vector2(listLeft + badgeW, y),
				selected ? sp.UiAccent :	sp.UiText, rowScale);

			// Resolved filename (or — for not-found)
			string filename =	match.ResolvedPath != null
				?	Path.GetFileName(match.ResolvedPath) ?? ""
				:	"—";
			font.DrawText(spriteBatch, filename,
				new Vector2(listLeft + badgeW + nameW, y),
				match.Resolution == RomResolution.NotFound ? sp.UiDim :	sp.UiText,
				rowScale * 0.9f);
		}

		// Scroll-indicator: little "more above ↑" / "more below ↓" caps.
		if (_scrollOffset > 0)
		{
			font.DrawText(spriteBatch, "▲ more above",
				new Vector2(listLeft, listTop - rowH * 0.9f),
				sp.UiDim, hintScale);
		}
		if (end < _matches.Count)
		{
			font.DrawText(spriteBatch, "▼ more below",
				new Vector2(listLeft, listTop + visible * RowHeight + 4),
				sp.UiDim, hintScale);
		}

		// ── Footer: selected-row detail + status banner + keys ────────
		float footerTop =	vp.Height - FooterHeight;
		if (_matches.Count > 0)
		{
			var match =	_matches[_selection];

			string detailLine1 =	$"{match.Spec.Name}  —  expected {match.Spec.ExpectedSize:N0} bytes / SHA256 {Short(match.Spec.ExpectedSha256Hex)}";
			font.DrawText(spriteBatch, detailLine1,
				new Vector2(50, footerTop), sp.UiText, hintScale * 1.2f);

			string detailLine2 =	match.ResolvedPath != null
				?	"Path: " + match.ResolvedPath
				:	"Press Enter to browse to this ROM.";
			font.DrawWrappedText(spriteBatch, detailLine2,
				new Vector2(50, footerTop + rowH * 0.9f),
				match.Resolution == RomResolution.NotFound ? new Color(232, 80, 80) :	sp.UiDim,
				vp.Width - 100, hintScale * 1.1f);
		}

		if (_statusMessage != null)
		{
			float msgW =	font.MeasureWidth(_statusMessage) * hintScale;
			font.DrawText(spriteBatch, _statusMessage,
				new Vector2((vp.Width - msgW) / 2f, vp.Height - 65),
				sp.UiAccent, hintScale);
		}

		string footerKeys =	"↑/↓: select   PgUp/PgDn: jump   Enter: browse   C: clear override   F5/R: rescan   Esc: back";
		float keysW =	font.MeasureWidth(footerKeys) * hintScale;
		font.DrawText(spriteBatch, footerKeys,
			new Vector2((vp.Width - keysW) / 2f, vp.Height - 35),
			sp.UiDim, hintScale);

		Engine.RenderSystem.EndUI();
	}

	private void ClampScroll(int visible)
	{
		if (_selection < _scrollOffset)
			_scrollOffset =	_selection;
		else if (_selection >= _scrollOffset + visible)
			_scrollOffset =	_selection - visible + 1;
		_scrollOffset =	Math.Max(0, Math.Min(_scrollOffset, Math.Max(0, _matches.Count - visible)));
	}

	private string BuildSubtitle()
	{
		int byHash =	0, byName =	0, ovr =	0, missing =	0;
		foreach (var m in _matches)
		{
			switch (m.Resolution)
			{
				case RomResolution.FoundByHash:	byHash++; break;
				case RomResolution.FoundByName:	byName++; break;
				case RomResolution.Override:		ovr++; break;
				default:							missing++; break;
			}
		}
		return $"{_matches.Count} ROMs:  {byHash} by hash  •  {byName} by name  •  {ovr} overridden  •  {missing} missing";
	}

	private static (string text, Color color) BadgeFor(RomResolution r) => r switch
	{
		RomResolution.FoundByHash =>	("[HASH ]",	new Color( 80, 200,  80)),	// green
		RomResolution.FoundByName =>	("[NAME ]",	new Color(220, 200,  80)),	// yellow
		RomResolution.Override    =>	("[OVERR]",	new Color( 80, 180, 220)),	// cyan
		_                         =>	("[MISS ]",	new Color(232,  80,  80)),	// red
	};

	// 8 hex chars is plenty to eyeball-distinguish dumps when comparing
	// against a known-good source — saves real estate vs the full 64.
	private static string Short(string hex) =>
		string.IsNullOrEmpty(hex) ? "—" :	hex.Substring(0, Math.Min(8, hex.Length)) + "…";
}
