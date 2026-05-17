using ChildhoodAdventure.RetroSystems.Assets;

namespace ChildhoodAdventure.RetroSystems.WinXP;

/// <summary>
/// Modern direct-color retro system — no palette, no hardware-style
/// constraints. Tile art is 32×32 RGBA cells; character sprites are 16×32
/// total (8/12/12 rows of head/body/legs). All assets come from on-disk
/// JSON + PNG files loaded through <see cref="AssetLoader"/>; if those files
/// aren't present the system invokes <see cref="WinXPAssetSeed"/> to write a
/// procedural baseline once.
///
/// The graphics-device dependent state (tile arrays, sprite parts) is loaded
/// lazily on first use through <see cref="EnsureLoaded"/> because the retro
/// systems are constructed at static-init time, long before MonoGame creates
/// a <see cref="GraphicsDevice"/>.
/// </summary>
internal sealed class WinXPSystem : RetroSystem
{
	public override string	Name				=> "Modern";
	public override string	Description			=> "32px tiles · 16×32 sprites · 16 tiles tall · 24-bit color";
	public override int		NativeTilePixels	=> 32;
	public override float	DefaultTilesTall	=> 16f;	// 1920×1080 ⇒ 60×33.75 tiles, 1080/16 ≈ 67 screen-px per tile
	public override float	MaxTilesTall		=> 32f;

	// Lazily-loaded asset bundles. Both depend on a live GraphicsDevice, which
	// the registry doesn't have at construction time — see EnsureLoaded.
	private Dictionary<string, Color[][]>? _tiles;
	private WinXPSprites? _sprites;

	protected override void OnGraphicsReady(GraphicsDevice gd)
	{
		if (_tiles is not null && _sprites is not null) return;
		var dir = ResolveAssetDir();
		WinXPAssetSeed.EnsureSeeded(dir);
		_tiles ??= AssetLoader.LoadTileSheet(gd, Path.Combine(dir, "WinXPTiles.json"));
		_sprites ??= new WinXPSprites(gd, dir);
	}

	/// <summary>
	/// Asset directory next to the running executable. Files are seeded once,
	/// after which they live as ordinary PNG/JSON the user can hand-edit.
	/// </summary>
	private static string ResolveAssetDir() =>
		Path.Combine(AppContext.BaseDirectory, "RetroSystems", "WinXP", "Assets");

	// ── Tile art ────────────────────────────────────────────────────────
	// Uses the direct-color path: GetTileColors returns final RGBA so the
	// base class skips palette resolution entirely.

	protected override Color[][]? GetTileColors(TileType type, Color accentColor)
	{
		// Defensive null check: the base class only invokes GetTileColors
		// from BuildTileset (which triggers OnGraphicsReady first), so the
		// dictionary is normally populated by the time we get here.
		if (_tiles is null) return null;
		if (_tiles.TryGetValue(type.ToString(), out var pixels))         return pixels;
		// Manifest is missing this tile (e.g. an older hand-edited asset
		// pack that predates a new TileType). Fall back through a couple of
		// reasonable substitutes before giving up.
		if (type == TileType.Grass2 && _tiles.TryGetValue("Grass", out pixels)) return pixels;
		return _tiles.TryGetValue("Wall", out pixels) ? pixels : null;
	}

	// ── Scene palette (per-house facade + dialog UI) ─────────────────────
	public override ScenePalette ScenePalette { get; } = new(
		HouseBeige:		new(238, 230, 210),
		HouseYellow:	new(245, 215,  95),
		HousePink:		new(235, 145, 170),
		HouseTeal:		new( 75, 175, 175),
		HouseGray:		new(160, 165, 175),
		HouseBlue:		new( 95, 140, 195),
		HouseLime:		new(165, 200,  95),
		HousePurple:	new(155, 105, 180),
		HouseOrange:	new(235, 145,  75),
		Door:			new( 50,  35,  25),
		UiBackground:	new( 30,  35,  50),
		UiText:			new(240, 240, 240),
		UiAccent:		new(245, 215,  95),
		UiChoice:		new(190, 200, 220),
		UiDim:			new(120, 130, 150));

	protected override ConsolePalette GetConsolePalette() => new(
		Wood:		new(170, 115,  60),
		WoodLight:	new(210, 165, 105),
		WoodShadow:	new(110,  65,  30),
		Body:		new( 25,  25,  30),
		BodyShadow:	new( 45,  45,  55),
		Switch:		new(230, 230, 235));

	// ── Character sprite slicing dims (resolved from manifests on first use) ─

	public override int CharWidth	=> _sprites?.CharWidth	?? 16;
	public override int HeadRows	=> _sprites?.HeadRows	?? 8;
	public override int BodyRows	=> _sprites?.BodyRows	?? 12;
	public override int LegsRows	=> _sprites?.LegsRows	?? 12;

	public override HeadPalette[] HeadPalettes	=> WinXPSprites.HeadPalettes;
	public override BodyPalette[] BodyPalettes	=> WinXPSprites.BodyPalettes;
	public override LegsPalette[] LegsPalettes	=> WinXPSprites.LegPalettes;

	private static readonly byte[][][][] EmptyPartArr = [];

	public override byte[][][][] HeadParts		=> _sprites?.HeadParts ?? EmptyPartArr;
	public override byte[][][][] BodyParts		=> _sprites?.BodyParts ?? EmptyPartArr;
	public override byte[][][][] LegsParts		=> _sprites?.LegsParts ?? EmptyPartArr;
	public override byte[][][][] HeadPartsBack	=> _sprites?.HeadPartsBack ?? HeadParts;
	public override byte[][][][] HeadPartsSide	=> _sprites?.HeadPartsSide ?? HeadParts;
	public override byte[][][][] BodyPartsBack	=> _sprites?.BodyPartsBack ?? BodyParts;
	public override byte[][][][] BodyPartsSide	=> _sprites?.BodyPartsSide ?? BodyParts;
	public override byte[][][][] LegsPartsSide	=> _sprites?.LegsPartsSide ?? LegsParts;
}
