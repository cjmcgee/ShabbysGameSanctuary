namespace ChildhoodAdventure.RetroSystems.Commodore64;

/// <summary>
/// Commodore 64 visual style.
///
/// Authentic characteristics:
///   • 24-px tile art (NativeTilePixels=24); the C64's hardware text/PETSCII unit is 8×8,
///     redrawn here at 24×24 to allow multicolor detail
///   • VIC-II 16-color palette with distinctive color relationships
///   • Multicolor mode: 1 global color + up to 3 local colors per tile (index 7 = grey shared register)
///   • Multicolor sprites: up to 4 semantic colors per part frame (transparent + 2 globals + 1 unique)
///   • Character sprites: 12×21 texture (HeadRows=6, BodyRows=8, LegsRows=7)
///   • Native screen: 160×200 (double-wide pixels → 80 unique columns × 200 rows)
///   • DefaultTilesTall = 12.5, MaxTilesTall = 25 (~2× zoom out)
///   • Double-wide pixels: each logical pixel occupies two adjacent horizontal pixels;
///     all odd columns equal the preceding even column.
/// </summary>
internal sealed class C64System :	RetroSystem
{
	public override string Name					=> "Commodore 64";
	public override string Description			=> "24-px tiles dbl-wide · 12×21 sprites · 12.5 tiles tall · VIC-II multicolor · 1 global + 3 local / 4 sprite colors";
	public override int NativeTilePixels		=> 24;
	public override float DefaultTilesTall		=> 12.5f;	// native 160×200 → 12.5 tiles vertically
	public override float MaxTilesTall			=> 25f;	// ~2× zoom out
	protected override bool DoubleWidePixels	=> true;

	// VIC-II multicolor: palette index 7 (C64 grey) acts as the shared global color register.
	// Each tile may use it freely plus up to 3 additional local indices.
	// Sprites are limited to 4 distinct non-transparent semantic indices per part frame.
	protected override int GlobalTileColorIndex		=> 7;
	protected override int MaxLocalTileColors		=> 3;
	protected override int MaxSpriteSemanticColors	=> 4;

	protected override Palette TilePalette { get; } = new C64Palette();

	// C64: Brown wood, light-grey switches, near-black body. DoubleWidePixels
	// (true on C64) gives the icon the system's chunky multicolor look.
	protected override ConsolePalette GetConsolePalette()	=>	new(
		Wood:			C64Palette.Brown,
		WoodLight:		C64Palette.MidBrown,
		WoodShadow:		C64Palette.DarkBrown,
		Body:			C64Palette.NearBlack,
		BodyShadow:		C64Palette.Black,
		Switch:			C64Palette.LightGrey);

	public override ScenePalette ScenePalette { get; } = new(
		HouseBeige:		C64Palette.White,
		HouseYellow:	C64Palette.Yellow,
		HousePink:		C64Palette.MidMagenta,			// sprite-only ext
		HouseTeal:		C64Palette.Cyan,
		HouseGray:		C64Palette.LightGrey,
		HouseBlue:		C64Palette.Blue,
		HouseLime:		C64Palette.LightGreen,			// sprite-only ext
		HousePurple:	C64Palette.MidMagenta,			// no violet → reuse magenta
		HouseOrange:	C64Palette.BurntOrange,			// sprite-only ext
		Door:			C64Palette.DoorBlack,
		UiBackground:	C64Palette.Black,
		UiText:			C64Palette.White,
		UiAccent:		C64Palette.Yellow,
		UiChoice:		C64Palette.LightGrey,
		UiDim:			C64Palette.Grey);

	// ── Tile pixel art (24×24) ───────────────────────────────────────────────
	// All rows obey the double-wide rule: col[2k+1] == col[2k] (12 logical cols).
	// Color constraints: index 0 (bg) + index 7 (global grey) + up to 3 locals per tile.

	protected override byte[][]	GetTilePixels(TileType type, Color accentColor)	=>	type switch
	{
		TileType.WoodFloor		=> C64Tiles.WoodFloor,
		TileType.Carpet			=> C64Tiles.Carpet,
		TileType.KitchenTile 	=> C64Tiles.KitchenTile,
		TileType.Wall			=> C64Tiles.Wall,
		TileType.Door			=> C64Tiles.Door,
		TileType.Window			=> C64Tiles.Window,
		TileType.Furniture		=> C64Tiles.Furniture,
		TileType.Counter		=> C64Tiles.Counter,
		TileType.Bookshelf		=> C64Tiles.Bookshelf,
		TileType.Plant			=> C64Tiles.Plant,
		TileType.Grass			=> C64Tiles.Grass,
		TileType.Grass2			=> C64Tiles.Grass,
		TileType.Road			=> C64Tiles.Road,
		TileType.Sidewalk		=> C64Tiles.Sidewalk,
		TileType.HouseExterior	=> C64Tiles.HouseExterior,
		TileType.HouseRoof		=> C64Tiles.HouseRoof,
		TileType.Bush			=> C64Tiles.Bush,
		TileType.Accent			=> C64Tiles.Accent,
		_ =>	C64Tiles.Wall
	};


	// ── Sprite dimensions (12×21 total) ──────────────────────────────────────
	// 6 logical pixels per row (2 physical pixels each).

	public override int CharWidth	=> C64Sprites.CharWidth;
	public override int HeadRows	=> C64Sprites.HeadRows;
	public override int BodyRows	=> C64Sprites.BodyRows;
	public override int LegsRows	=> C64Sprites.LegsRows;

	public override HeadPalette[] HeadPalettes => C64Sprites.HeadPalettes;
	public override BodyPalette[] BodyPalettes => C64Sprites.BodyPalettes;
	public override LegsPalette[] LegsPalettes => C64Sprites.LegPalettes;

	public override byte[][][][] HeadParts { get; }		= [ C64Sprites.Head0, C64Sprites.Head1, C64Sprites.Head2 ];
	public override byte[][][][] BodyParts { get; }		= [ C64Sprites.Body0, C64Sprites.Body1, C64Sprites.Body2 ];
	public override byte[][][][] LegsParts { get; }		= [ C64Sprites.Legs0, C64Sprites.Legs1, C64Sprites.Legs2 ];
	public override byte[][][][] HeadPartsBack { get; }	= [ C64Sprites.Head0Back, C64Sprites.Head1Back, C64Sprites.Head2Back ];
	public override byte[][][][] HeadPartsSide { get; }	= [ C64Sprites.Head0Side, C64Sprites.Head1Side, C64Sprites.Head2Side ];
	public override byte[][][][] BodyPartsBack { get; }	= [ C64Sprites.Body0Back, C64Sprites.Body1Back, C64Sprites.Body2Back ];
	public override byte[][][][] BodyPartsSide { get; }	= [ C64Sprites.Body0Side, C64Sprites.Body1Side, C64Sprites.Body2Side ];
	public override byte[][][][] LegsPartsSide { get; }	= [ C64Sprites.Legs0Side, C64Sprites.Legs1Side, C64Sprites.Legs2Side ];
}
