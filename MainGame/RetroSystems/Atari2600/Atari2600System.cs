using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.Atari2600;

/// <summary>
/// Atari 2600 visual style.
///
/// Authentic characteristics:
///   • 16-px tile art (NativeTilePixels=16); hardware Atari 2600 tiles are 8×8 in spirit
///   • 1-bit tile color: each tile uses only black (bg) and one foreground color
///   • Sprites limited to one color per horizontal scanline (TIA player register style)
///   • 128-color NTSC palette; sprites are hard-edged silhouettes
///   • Character sprites: 16×16 texture (CharWidth=16, HeadRows+BodyRows+LegsRows=16)
///   • Native screen: 160×192 (double-wide pixels → 80 unique columns × 192 rows)
///   • DefaultTilesTall = 12, MaxTilesTall = 24 (~2× zoom out)
///   • Double-wide pixels: each logical pixel occupies two adjacent horizontal pixels;
///     all odd columns equal the preceding even column.
/// </summary>
public sealed class Atari2600System :	RetroSystem
{
	public override string	Name				=>	"Atari 2600";
	public override string	Description			=>	"16-px tiles dbl-wide · 16×16 sprites · 12 tiles tall · 1-bit · 1-color/scanline sprites";
	public override int		NativeTilePixels	=>	16;
	public override float	DefaultTilesTall	=>	12f;		// native 160×192 → 12 tiles vertically
	public override float	MaxTilesTall		=>	24f;		// ~2× zoom out

	protected override bool DoubleWidePixels			=>	true;
	protected override bool OneBitTiles					=>	true;
	protected override bool SpriteOneColorPerScanline	=>	true;

	protected override Palette TilePalette { get; } =	new Atari2600Palette();

	public override ScenePalette ScenePalette { get; } =	new(
		HouseBeige:		Atari2600Palette.NearWhite,
		HouseYellow:	Atari2600Palette.BrightYellow,
		HousePink:		Atari2600Palette.VividRed,		// no pink → warm red
		HouseTeal:		Atari2600Palette.BrightCyan,
		HouseGray:		Atari2600Palette.MediumGray,
		HouseBlue:		Atari2600Palette.BoldBlue,
		HouseLime:		Atari2600Palette.VividGreen,
		HousePurple:	Atari2600Palette.DarkRed,			// no violet → distinct dark warm
		HouseOrange:	Atari2600Palette.WarmAmber,
		Door:			Atari2600Palette.NearBlack,
		UiBackground:	Atari2600Palette.NearBlack,
		UiText:			Atari2600Palette.NearWhite,
		UiAccent:		Atari2600Palette.BrightYellow,
		UiChoice:		Atari2600Palette.LightGray,
		UiDim:			Atari2600Palette.MediumGray);

	// ── Tile pixel art (16×16, palette indices) ──────────────────────────────
	// Rules: (1) only index 0 and ONE non-zero foreground index per tile.
	//        (2) double-wide: col[2k+1] == col[2k] for all k (8 logical cols).
	protected override byte[][]	GetTilePixels(TileType type, Color accentColor)	=>	type switch
	{
		TileType.WoodFloor		=>	Atari2600Tiles.WoodFloor,
		TileType.Carpet			=>	Atari2600Tiles.Carpet,
		TileType.KitchenTile   =>	Atari2600Tiles.KitchenTile,
		TileType.Wall			=>	Atari2600Tiles.Wall,
		TileType.Door			=>	Atari2600Tiles.Door,
		TileType.Window			=>	Atari2600Tiles.Window,
		TileType.Furniture		=>	Atari2600Tiles.Furniture,
		TileType.Counter		=>	Atari2600Tiles.Counter,
		TileType.Bookshelf		=>	Atari2600Tiles.Bookshelf,
		TileType.Plant			=>	Atari2600Tiles.Plant,
		TileType.Grass			=>	Atari2600Tiles.Grass,
		TileType.Grass2			=>	Atari2600Tiles.Grass,
		TileType.Road			=>	Atari2600Tiles.Road,
		TileType.Sidewalk		=>	Atari2600Tiles.Sidewalk,
		TileType.HouseExterior =>	Atari2600Tiles.HouseExterior,
		TileType.HouseRoof		=>	Atari2600Tiles.HouseRoof,
		TileType.Bush			=>	Atari2600Tiles.Bush,
		TileType.Accent			=>	Atari2600Tiles.Accent,
		_ => Atari2600Tiles.Wall
	};

	// ── Sprite dimensions ─────────────────────────────────────────────────────
	// HeadRows=5, BodyRows=7, LegsRows=4  (total 16)
	// 8 logical pixels per row (2 physical pixels each, double-wide).
	public override int CharWidth  =>	Atari2600Sprites.CharWidth;
	public override int HeadRows   =>	Atari2600Sprites.HeadRows;
	public override int BodyRows   =>	Atari2600Sprites.BodyRows;
	public override int LegsRows   =>	Atari2600Sprites.LegsRows;

	public override LegsPalette[]	LegsPalettes =>	Atari2600Sprites.LegPalettes;
	public override BodyPalette[]	BodyPalettes =>	Atari2600Sprites.BodyPalettes;
	public override HeadPalette[]	HeadPalettes =>	Atari2600Sprites.HeadPalettes;

	public override byte[][][][]	HeadParts { get; } =		[ Atari2600Sprites.Head0, Atari2600Sprites.Head1, Atari2600Sprites.Head2 ];
	public override byte[][][][]	HeadPartsBack { get; } =	[ Atari2600Sprites.Head0Back, Atari2600Sprites.Head1Back, Atari2600Sprites.Head2Back ];
	public override byte[][][][]	BodyParts { get; } =		[ Atari2600Sprites.Body0, Atari2600Sprites.Body1, Atari2600Sprites.Body2 ];
	public override byte[][][][]	BodyPartsBack { get; } =	[ Atari2600Sprites.Body0Back, Atari2600Sprites.Body1Back, Atari2600Sprites.Body2Back ];
	public override byte[][][][]	LegsParts { get; } =		[ Atari2600Sprites.Legs0, Atari2600Sprites.Legs1, Atari2600Sprites.Legs2 ];
	public override byte[][][][]	LegsPartsSide { get; } =	[ Atari2600Sprites.Legs0Side, Atari2600Sprites.Legs1Side, Atari2600Sprites.Legs2Side ];
}
