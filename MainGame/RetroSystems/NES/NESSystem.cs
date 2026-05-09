using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.NES;

/// <summary>
/// Nintendo Entertainment System visual style.
///
/// Authentic characteristics:
///   • 16-px tile art (NativeTilePixels=16); each engine tile maps to 2×2 NES hardware
///     8×8 tiles in spirit, so the art is hand-drawn at the 16×16 super-tile size
///   • NES PPU palette: 54 available colors; each background palette has 3 colors + shared bg
///   • Rich multi-color pixel art — the most detailed system in this set
///   • Character sprites: 8×16 (HeadRows=4, BodyRows=6, LegsRows=6)
///   • Native screen: 256×224 → DefaultTilesTall = 14
///   • MaxTilesTall = 28 (~2× zoom out)
/// </summary>
public sealed class NESSystem :	RetroSystem
{
	public override string	Name				=> "NES";
	public override string	Description			=> "16-px tiles · 8×16 sprites · 14 tiles tall · NES PPU palette";
	public override int		NativeTilePixels	=> 16;
	public override float	DefaultTilesTall	=> 14f;		// native NES: 256×224 → 14 tiles vertically
	public override float	MaxTilesTall		=> 28f;		// ~2× zoom out

	protected override Palette TilePalette { get; } = new NESPalette();

	public override ScenePalette ScenePalette { get; } = new(
		HouseBeige:		NESPalette.NearWhite,
		HouseYellow:	NESPalette.PaleYellow,
		HousePink:		NESPalette.VividRed,				// no pink → vivid red
		HouseTeal:		NESPalette.Aqua,
		HouseGray:		NESPalette.SidewalkLight,
		HouseBlue:		NESPalette.NesBlue,
		HouseLime:		NESPalette.LeafGreen,
		HousePurple:	NESPalette.MidMagenta,
		HouseOrange:	NESPalette.RustOrange,
		Door:			NESPalette.DoorBlack,
		UiBackground:	NESPalette.DarkBackground,
		UiText:			NESPalette.NearWhite,
		UiAccent:		NESPalette.PaleYellow,
		UiChoice:		NESPalette.SidewalkLight,
		UiDim:			NESPalette.SidewalkGray);

	protected override byte[][]	GetTilePixels(TileType type, Color accentColor)	=> type switch
	{
		TileType.WoodFloor		=> NESTiles.WoodFloor,
		TileType.Carpet			=> NESTiles.Carpet,
		TileType.KitchenTile	=> NESTiles.KitchenTile,
		TileType.Wall			=> NESTiles.Wall,
		TileType.Door			=> NESTiles.Door,
		TileType.Window			=> NESTiles.Window,
		TileType.Furniture		=> NESTiles.Furniture,
		TileType.Counter		=> NESTiles.Counter,
		TileType.Bookshelf		=> NESTiles.Bookshelf,
		TileType.Plant			=> NESTiles.Plant,
		TileType.Grass			=> NESTiles.Grass,
		TileType.Grass2			=> NESTiles.Grass2,
		TileType.Road			=> NESTiles.Road,
		TileType.Sidewalk		=> NESTiles.Sidewalk,
		TileType.HouseExterior	=> NESTiles.HouseExterior,
		TileType.HouseRoof		=> NESTiles.HouseRoof,
		TileType.Bush			=> NESTiles.Bush,
		TileType.Accent			=> NESTiles.Accent,
		_ =>	NESTiles.Wall
	};

	public override int CharWidth  => NESSprites.CharWidth;
	public override int HeadRows   => NESSprites.HeadRows;
	public override int BodyRows   => NESSprites.BodyRows;
	public override int LegsRows   => NESSprites.LegsRows;

	public override HeadPalette[] HeadPalettes	=> NESSprites.HeadPalettes;
	public override BodyPalette[] BodyPalettes	=> NESSprites.BodyPalettes;
	public override LegsPalette[] LegsPalettes	=> NESSprites.LegPalettes;

	public override byte[][][][] HeadParts { get; }		= [ NESSprites.Head0, NESSprites.Head1, NESSprites.Head2 ];
	public override byte[][][][] BodyParts { get; }		= [ NESSprites.Body0, NESSprites.Body1, NESSprites.Body2 ];
	public override byte[][][][] LegsParts { get; }		= [ NESSprites.Legs0, NESSprites.Legs1, NESSprites.Legs2 ];
	public override byte[][][][] HeadPartsBack { get; }	= [ NESSprites.Head0Back, NESSprites.Head1Back, NESSprites.Head2Back ];
	public override byte[][][][] HeadPartsSide { get; }	= [ NESSprites.Head0Side, NESSprites.Head1Side, NESSprites.Head2Side ];
	public override byte[][][][] BodyPartsBack { get; }	= [ NESSprites.Body0Back, NESSprites.Body1Back, NESSprites.Body2Back ];
	public override byte[][][][] BodyPartsSide { get; }	= [ NESSprites.Body0Side, NESSprites.Body1Side, NESSprites.Body2Side ];
	public override byte[][][][] LegsPartsSide { get; }	= [ NESSprites.Legs0Side, NESSprites.Legs1Side, NESSprites.Legs2Side ];
}
