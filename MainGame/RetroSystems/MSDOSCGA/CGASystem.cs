namespace ChildhoodAdventure.RetroSystems.MSDOSCGA;

/// <summary>
/// MS-DOS CGA visual style — mode 4, palette 1 (high intensity).
///
/// Authentic characteristics:
///   • 16-px tile art (NativeTilePixels=16); the IBM CGA hardware text-mode tile is 8×8,
///     redrawn here at 16×16 to give space for the 4-color palette
///   • 4-color fixed palette: black, bright cyan, bright magenta, white
///     (IBM CGA mode 4 palette 1 — the most iconic CGA look)
///   • Character sprites: 16×16 texture (CharWidth=16, HeadRows=4, BodyRows=6, LegsRows=6)
///   • Native screen 320×200 → DefaultTilesTall = 12.5
///   • MaxTilesTall = 25 (~2× zoom out)
/// </summary>
internal sealed class CGASystem :	RetroSystem
{
	public override string Name					=> "MS-DOS CGA";
	public override string Description			=> "16-px tiles · 16×16 sprites · 12.5 tiles tall · Mode 4 palette 1 (black/cyan/magenta/white)";
	public override int NativeTilePixels		=> 16;
	public override float DefaultTilesTall		=> 12.5f;	// native 320×200 → 12.5 tiles vertically
	public override float MaxTilesTall			=> 25f;		// ~2× zoom out

	protected override Palette TilePalette { get; } = new CGAPalette();

	// CGA mode-4 palette 1 has only 4 colours (black/cyan/magenta/white).
	// The console becomes magenta wood, white highlights, cyan switches —
	// the unmistakable DOS-era look.
	protected override ConsolePalette GetConsolePalette()	=>	new(
		Wood:			CGAPalette.BrightMagenta,
		WoodLight:		CGAPalette.White,
		WoodShadow:		CGAPalette.Black,
		Body:			CGAPalette.Black,
		BodyShadow:		CGAPalette.Black,
		Switch:			CGAPalette.BrightCyan);

	// CGA mode-4 palette 1 has only 4 colours; many house tones collapse onto
	// the same palette entry, mirroring authentic CGA limitations.
	public override ScenePalette ScenePalette { get; } = new(
		HouseBeige:		CGAPalette.White,
		HouseYellow:	CGAPalette.BrightCyan,
		HousePink:		CGAPalette.BrightMagenta,
		HouseTeal:		CGAPalette.BrightCyan,
		HouseGray:		CGAPalette.White,
		HouseBlue:		CGAPalette.BrightCyan,
		HouseLime:		CGAPalette.BrightCyan,
		HousePurple:	CGAPalette.BrightMagenta,
		HouseOrange:	CGAPalette.BrightMagenta,
		Door:			CGAPalette.Black,
		UiBackground:	CGAPalette.Black,
		UiText:			CGAPalette.White,
		UiAccent:		CGAPalette.BrightCyan,
		UiChoice:		CGAPalette.White,
		UiDim:			CGAPalette.BrightMagenta);

	// ── Tile pixel art (16×16, indices 0–3 only) ─────────────────────────────

	protected override byte[][]	GetTilePixels(TileType type, Color accentColor)	=> type switch
	{
		TileType.WoodFloor		=>	CGATiles.WoodFloor,
		TileType.Carpet			=>	CGATiles.Carpet,
		TileType.KitchenTile	=>	CGATiles.KitchenTile,
		TileType.Wall			=>	CGATiles.Wall,
		TileType.Door			=>	CGATiles.Door,
		TileType.Window			=>	CGATiles.Window,
		TileType.Furniture		=>	CGATiles.Furniture,
		TileType.Counter		=>	CGATiles.Counter,
		TileType.Bookshelf		=>	CGATiles.Bookshelf,
		TileType.Plant			=>	CGATiles.Plant,
		TileType.Grass			=>	CGATiles.Grass,
		TileType.Grass2			=>	CGATiles.Grass,
		TileType.Road			=>	CGATiles.Road,
		TileType.Sidewalk		=>	CGATiles.Sidewalk,
		TileType.HouseExterior	=>	CGATiles.HouseExterior,
		TileType.HouseRoof		=>	CGATiles.HouseRoof,
		TileType.Bush			=>	CGATiles.Bush,
		TileType.Accent			=>	CGATiles.Accent,
		_ =>	CGATiles.Wall
	};

	public override int CharWidth  => CGASprites.CharWidth;
	public override int HeadRows   => CGASprites.HeadRows;
	public override int BodyRows   => CGASprites.BodyRows;
	public override int LegsRows   => CGASprites.LegsRows;

	public override HeadPalette[] HeadPalettes	=> CGASprites.HeadPalettes;
	public override BodyPalette[] BodyPalettes	=> CGASprites.BodyPalettes;
	public override LegsPalette[] LegsPalettes	=> CGASprites.LegPalettes;

	public override byte[][][][] HeadParts { get; }		= [ CGASprites.Head0, CGASprites.Head1, CGASprites.Head2 ];
	public override byte[][][][] BodyParts { get; }		= [ CGASprites.Body0, CGASprites.Body1, CGASprites.Body2 ];
	public override byte[][][][] LegsParts { get; }		= [ CGASprites.Legs0, CGASprites.Legs1, CGASprites.Legs2 ];
	public override byte[][][][] HeadPartsBack { get; }	= [ CGASprites.Head0Back, CGASprites.Head1Back, CGASprites.Head2Back ];
	public override byte[][][][] HeadPartsSide { get; }	= [ CGASprites.Head0Side, CGASprites.Head1Side, CGASprites.Head2Side ];
	public override byte[][][][] BodyPartsBack { get; }	= [ CGASprites.Body0Back, CGASprites.Body1Back, CGASprites.Body2Back ];
	public override byte[][][][] BodyPartsSide { get; }	= [ CGASprites.Body0Side, CGASprites.Body1Side, CGASprites.Body2Side ];
	public override byte[][][][] LegsPartsSide { get; }	= [ CGASprites.Legs0Side, CGASprites.Legs1Side, CGASprites.Legs2Side ];

}
