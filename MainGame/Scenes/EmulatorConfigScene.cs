using TileEngine.Rendering;

namespace ChildhoodAdventure.Scenes;

/// <summary>
/// In-game configuration screen for the emulator paths. Lets the player
/// point ROM/Core/System roots at folders on disk via the native OS
/// folder-picker dialog, without ever touching the install directory or
/// editing JSON by hand.
///
/// Each row autosaves on change: pressing Enter opens the native picker
/// and a successful pick is written straight to the per-user config file
/// (see <see cref="EmulatorConfig.UserConfigPath"/>). C clears the row.
/// Esc returns to the caller.
/// </summary>
internal sealed class EmulatorConfigScene :	Scene
{
	private readonly Func<Scene> _returnTo;
	private EmulatorConfig _config;
	private int _selection;
	private KeyboardState _prevKeys;
	private string? _statusMessage;
	private double _statusFadeAt;
	private double _now;
	// Cached tally line for the ROM Manager row. Recomputed on scene load
	// and after any path-row change (which may have moved RomRoot under
	// the resolver's feet). Without this cache, the previous "Get = c =>
	// BuildRomTally(c)" line was re-resolving on every Draw frame —
	// hashing the same files at ~60Hz.
	private string _romTally =	"";

	// Row descriptors. Ordered so RomRoot is on top — that's the one users
	// almost always need to set. Core/System are advanced and usually fine
	// at their defaults.
	private readonly Row[] _rows;

	private sealed class Row
	{
		public string Label =	"";
		public string Hint =	"";
		public Func<EmulatorConfig, string> Get =	_ =>	"";
		public Action<EmulatorConfig, string> Set =	(_, _) =>	{};

		// Non-null on rows that aren't folder pickers — e.g. the "ROM
		// Manager" row navigates to another scene instead of popping the
		// native folder dialog. When set, BrowseSelected calls this
		// instead of running the folder-picker flow; Clear becomes a no-op.
		public Action<EmulatorConfigScene>? OnSelect;
	}

	public EmulatorConfigScene(EmulatorConfig config, Func<Scene> returnTo)
	{
		_config =	config;
		_returnTo =	returnTo;
		Name =	"EmulatorConfig";

		_rows =	new[]
		{
			new Row
			{
				Label =	"ROM folder",
				Hint =	"Where your .bin / .a26 ROM files live.",
				Get =	c =>	c.RomRoot,
				Set =	(c, v) =>	c.RomRoot =	v,
			},
			new Row
			{
				Label =	"Core folder",
				Hint =	"libretro cores (stella_libretro.*). Leave empty to use the install dir.",
				Get =	c =>	c.CoreRoot,
				Set =	(c, v) =>	c.CoreRoot =	v,
			},
			new Row
			{
				Label =	"System folder",
				Hint =	"BIOS files for cores that need them. Stella does not.",
				Get =	c =>	c.SystemRoot,
				Set =	(c, v) =>	c.SystemRoot =	v,
			},
			new Row
			{
				Label =	"ROM Manager",
				Hint =	"Verify per-ROM matches and pick files for any the auto-resolver missed.",
				// Return the cached tally — recomputing here would re-walk
				// the ROM folder and re-hash candidates on every Draw frame.
				Get =	_ =>	_romTally,
				// No file-system value to clear on this row.
				Set =	(_, _) =>	{},
				OnSelect =	self =>	self.OpenRomManager(),
			},
		};
	}

	// Re-resolve every spec to render the live tally inline with the row.
	// Cheap (single dir walk, hashes already cached during the row's draw
	// frame… well, no — resolver is called fresh here. With 22 ROMs and
	// hashing only size-matched files this is well under a frame even on
	// slow disks, so it's fine to call from the draw path. If it ever
	// becomes a hot spot, cache per-scene-load).
	private static string BuildRomTally(EmulatorConfig c)
	{
		var matches =	RomResolver.Resolve(GameLibrary.RequiredRoms, c.EffectiveRomRoot, c.RomOverrides);
		int byHash =	0, byName =	0, ovr =	0, missing =	0;
		foreach (var m in matches)
		{
			switch (m.Resolution)
			{
				case RomResolution.FoundByHash:	byHash++; break;
				case RomResolution.FoundByName:	byName++; break;
				case RomResolution.Override:		ovr++; break;
				default:							missing++; break;
			}
		}
		return $"{matches.Count} ROMs:  {byHash} by hash  •  {byName} by name  •  {ovr} overridden  •  {missing} missing";
	}

	private void OpenRomManager()
	{
		// Capture our own return path so the manager comes back here, and
		// from here Esc returns to wherever EmulatorConfigScene was launched
		// from (game-select / home interior).
		var ourReturn =	_returnTo;
		Engine.LoadScene(new RomManagerScene(
			_config,
			returnTo: () =>	new EmulatorConfigScene(EmulatorConfig.LoadOrDefault(), ourReturn)));
	}

	protected override void OnLoad()
	{
		_selection =	0;
		RefreshRomTally();
		// Swallow whatever key currently holds (the C that brought us here)
		// so the keypress doesn't immediately register as a fresh edge.
		_prevKeys =	Keyboard.GetState();
	}

	// Single source-of-truth for the cached tally string. Called from
	// OnLoad and after any action that could have changed the resolver's
	// inputs (a RomRoot edit, a RomOverrides change).
	private void RefreshRomTally()
	{
		_romTally =	BuildRomTally(_config);
	}

	protected override void OnUnload() {}

	public override void Update(GameTime gameTime)
	{
		_now =	gameTime.TotalGameTime.TotalSeconds;
		var keys =	Keyboard.GetState();
		bool down =	IsPressed(keys, Keys.Down) || IsPressed(keys, Keys.S);
		bool up =	IsPressed(keys, Keys.Up)   || IsPressed(keys, Keys.W);
		bool select =	IsPressed(keys, Keys.Enter) || IsPressed(keys, Keys.E);
		bool clear =	IsPressed(keys, Keys.C) || IsPressed(keys, Keys.Delete) || IsPressed(keys, Keys.Back);
		bool cancel =	IsPressed(keys, Keys.Escape);

		_prevKeys =	keys;

		if (cancel)
		{
			Engine.LoadScene(_returnTo());
			return;
		}

		if (down)	_selection =	(_selection + 1)	% _rows.Length;
		if (up)		_selection =	(_selection - 1 + _rows.Length)	% _rows.Length;

		if (select)	BrowseSelected();
		if (clear)	ClearSelected();

		// Fade the status banner after ~3s so it doesn't linger forever.
		if (_statusMessage != null && _now >= _statusFadeAt)
			_statusMessage =	null;
	}

	private void BrowseSelected()
	{
		var row =	_rows[_selection];

		// Non-folder rows route Enter to their own action (e.g. opening a
		// sub-scene). The folder-picker flow below is for path rows only.
		if (row.OnSelect != null)
		{
			row.OnSelect(this);
			return;
		}

		string current =	row.Get(_config);
		string? start =	!string.IsNullOrEmpty(current) ? current :	null;

		string? picked =	NativeFilePicker.PickFolder(
			$"Select {row.Label.ToLowerInvariant()}", start);
		if (picked == null)	return;	// user cancelled or picker unavailable

		row.Set(_config, picked);
		bool saved =	_config.Save();
		// Any path change might have moved the resolver's RomRoot under it.
		RefreshRomTally();
		ShowStatus(saved
			? $"Saved. {row.Label}: {picked}"
			: $"Could not write {EmulatorConfig.UserConfigPath}");
	}

	private void ClearSelected()
	{
		var row =	_rows[_selection];

		// Action rows have no path value to wipe — Clear is a no-op there.
		if (row.OnSelect != null)	return;

		if (string.IsNullOrEmpty(row.Get(_config)))	return;	// already empty

		row.Set(_config, "");
		bool saved =	_config.Save();
		RefreshRomTally();
		ShowStatus(saved
			? $"Cleared {row.Label}."
			: $"Could not write {EmulatorConfig.UserConfigPath}");
	}

	private void ShowStatus(string msg)
	{
		_statusMessage =	msg;
		_statusFadeAt =	_now + 3.0;
	}

	private bool IsPressed(KeyboardState keys, Keys k) =>
		keys.IsKeyDown(k) && !_prevKeys.IsKeyDown(k);

	public override void Draw(SpriteBatch sb, GameTime gameTime)
	{
		var sp =	RetroSystemRegistry.Current.ScenePalette;
		Engine.GraphicsDevice.Clear(sp.UiBackground);

		Engine.RenderSystem.BeginUI();
		var spriteBatch =	Engine.RenderSystem.SpriteBatch;
		var font =	Engine.RenderSystem.Font;
		var vp =	Engine.GraphicsDevice.Viewport;

		const float titleScale =	2.5f;
		const float labelScale =	2.0f;
		const float valueScale =	1.4f;
		const float hintScale =	1.2f;

		float titleH =	PixelFont.CharH * titleScale;
		float labelH =	PixelFont.CharH * labelScale;
		float valueH =	PixelFont.CharH * valueScale;
		float hintH =	PixelFont.CharH * hintScale;

		// Title
		string title =	"CONFIGURE EMULATOR PATHS";
		float titleW =	PixelFont.MeasureWidth(title) * titleScale;
		font.DrawText(spriteBatch, title,
			new Vector2((vp.Width - titleW) / 2f, 50),
			sp.UiAccent, titleScale);

		const float leftPad =	60f;
		float rowMaxWidth =	vp.Width - leftPad * 2;
		float y =	50 + titleH + 30;

		for (int i = 0; i < _rows.Length; i++)
		{
			var row =	_rows[i];
			bool selected =	(i == _selection);
			string value =	row.Get(_config);
			bool unset =	string.IsNullOrEmpty(value);

			Color labelColor =	selected ? sp.UiAccent :	sp.UiText;
			Color valueColor =	unset ? sp.UiDim :	sp.UiText;

			string marker =	selected ? "▶ " :	"  ";
			font.DrawText(spriteBatch, marker + row.Label,
				new Vector2(leftPad, y), labelColor, labelScale);
			y +=	labelH + 4;

			string shown =	unset ? "(not set)" :	value;
			font.DrawWrappedText(spriteBatch, "   " + shown,
				new Vector2(leftPad, y), valueColor, rowMaxWidth, valueScale);
			y +=	valueH + 4;

			font.DrawWrappedText(spriteBatch, "   " + row.Hint,
				new Vector2(leftPad, y), sp.UiDim, rowMaxWidth, hintScale);
			y +=	hintH + 20;
		}

		// Status banner (last save/clear result), centred above the footer.
		if (_statusMessage != null)
		{
			float msgW =	PixelFont.MeasureWidth(_statusMessage) * hintScale;
			font.DrawText(spriteBatch, _statusMessage,
				new Vector2((vp.Width - msgW) / 2f, vp.Height - 110),
				sp.UiAccent, hintScale);
		}

		// Storage location — so users can find the file manually if they
		// want to back it up or sync it.
		string storage =	"Saved to: " + EmulatorConfig.UserConfigPath;
		font.DrawWrappedText(spriteBatch, storage,
			new Vector2(leftPad, vp.Height - 80),
			sp.UiDim, vp.Width - leftPad * 2, hintScale);

		// Footer hint.
		string footer =	"↑/↓: select   Enter: browse   C: clear   Esc: back";
		float footerW =	PixelFont.MeasureWidth(footer) * hintScale * 1.2f;
		font.DrawText(spriteBatch, footer,
			new Vector2((vp.Width - footerW) / 2f, vp.Height - 40),
			sp.UiDim, hintScale * 1.2f);

		Engine.RenderSystem.EndUI();
	}
}

