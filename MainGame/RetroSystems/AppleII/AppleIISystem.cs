using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.AppleII;

/// <summary>
/// Apple II visual style — hi-res graphics mode.
///
/// Tile art is 14×14 — exactly two 7-pixel hi-res palette segments wide — and
/// follows the actual Apple II hi-res color rules rather than a simple double-
/// wide-pixel approximation:
///   • Each row splits into two 7-pixel segments (cols 0-6 and 7-13). Each
///     segment selects ONE palette — palette 0 (black/green/violet/white) or
///     palette 1 (black/orange/blue/white). Black and white work in either.
///   • Green / orange (palette odd-column color): only at odd x, with the
///     previous and following pixels both 0.
///   • Violet / blue  (palette even-column color): only at even x, with the
///     previous and following pixels both 0.
///   • White = run of ≥2 adjacent non-zero pixels. Black = run of ≥2 zeros.
///   • Cols 0 and 13 (left/right tile borders) are forced to 0 so that
///     horizontally adjacent tiles always meet at predictable black, instead
///     of bleeding their boundary pixels into each other.
///   • Tiles are designed "mostly black with simple motifs" so the above
///     constraints stay satisfiable without dense color masses.
///
/// Other characteristics:
///   • 6-color hi-res palette: black, green, violet, white, orange, blue
///     (NTSC artifact colors from the 1-MHz pixel clock — the defining Apple II look)
///   • Character sprites: 14×14 (HeadRows=4, BodyRows=6, LegsRows=4). Sprite
///     art is hand-authored to obey the same hi-res palette rules as the tiles.
///   • Native screen: 280×192 → DefaultTilesTall = 12
///   • MaxTilesTall = 24 (~2× zoom out)
/// </summary>
public sealed class AppleIISystem :	RetroSystem
{
	public override string	Name =>				"Apple II";
	public override string	Description =>		"14-px tiles · 14×14 sprites · 12 tiles tall · Hi-res 6-color w/ NTSC palette rules";
	public override int		NativeTilePixels =>	14;
	public override float	DefaultTilesTall =>	12f;	// native 280×192 → 12 tiles vertically
	public override float	MaxTilesTall =>		24f;	// ~2× zoom out

	// TODO: need to simulate color fringing for any text!
	protected override Palette TilePalette { get; } = new AppleIIPalette();

	// Apple II hi-res has only 6 colours. The console becomes orange-wood with
	// a black plastic body and white switches — a stylised, very-Apple-II look.
	protected override ConsolePalette GetConsolePalette()	=>	new(
		Wood:			AppleIIPalette.Orange,
		WoodLight:		AppleIIPalette.White,
		WoodShadow:		AppleIIPalette.Black,
		Body:			AppleIIPalette.Black,
		BodyShadow:		AppleIIPalette.Black,
		Switch:			AppleIIPalette.White);

	// Apple II hi-res has only 6 colours; warm/yellow/orange roles all share
	// Orange, blue/teal share Blue, etc.
	public override ScenePalette ScenePalette { get; } = new(
		HouseBeige:		AppleIIPalette.White,
		HouseYellow:	AppleIIPalette.Orange,
		HousePink:		AppleIIPalette.Violet,
		HouseTeal:		AppleIIPalette.Blue,
		HouseGray:		AppleIIPalette.White,
		HouseBlue:		AppleIIPalette.Blue,
		HouseLime:		AppleIIPalette.Green,
		HousePurple:	AppleIIPalette.Violet,
		HouseOrange:	AppleIIPalette.Orange,
		Door:			AppleIIPalette.Black,
		UiBackground:	AppleIIPalette.Black,
		UiText:			AppleIIPalette.White,
		UiAccent:		AppleIIPalette.Orange,
		UiChoice:		AppleIIPalette.Green,
		UiDim:			AppleIIPalette.Violet);

	// ── Tile pixel art (14×14, Apple II hi-res rules) ────────────────────────
	//   col 0 / col 13 always 0           (predictable black at tile borders)
	//   palette 0 (green/violet) OR palette 1 (orange/blue) per 7-pixel segment
	//   1 (green)  / 4 (orange):  odd  x, with 0 neighbors on both sides
	//   2 (violet) / 5 (blue):    even x, with 0 neighbors on both sides
	//   3 (white):  any x, in a run of ≥2 set pixels
	//   0 (black):  any x, in a run of ≥2 zero pixels

	protected override byte[][]	GetTilePixels(TileType type, Color accentColor)	=> type switch
	{
		TileType.WoodFloor		=>	AppleIITiles.WoodFloor,
		TileType.Carpet			=>	AppleIITiles.Carpet,
		TileType.KitchenTile	=>	AppleIITiles.KitchenTile,
		TileType.Wall			=>	AppleIITiles.Wall,
		TileType.Door			=>	AppleIITiles.Door,
		TileType.Window			=>	AppleIITiles.Window,
		TileType.Furniture		=>	AppleIITiles.Furniture,
		TileType.Counter		=>	AppleIITiles.Counter,
		TileType.Bookshelf		=>	AppleIITiles.Bookshelf,
		TileType.Plant			=>	AppleIITiles.Plant,
		TileType.Grass			=>	AppleIITiles.Grass,
		TileType.Grass2			=>	AppleIITiles.Grass,
		TileType.Road			=>	AppleIITiles.Road,
		TileType.Sidewalk		=>	AppleIITiles.Sidewalk,
		TileType.HouseExterior	=>	AppleIITiles.HouseExterior,
		TileType.HouseRoof		=>	AppleIITiles.HouseRoof,
		TileType.Bush			=>	AppleIITiles.Bush,
		TileType.Accent			=>	AppleIITiles.Accent,
		_ =>	AppleIITiles.Wall
	};


	// ── Sprite dimensions (14×14 total) ───────────────────────────────────────
	// Sprite art follows the same Apple II discipline as tile art (loose form):
	//   • 14 wide × 14 tall (head 4 + body 6 + legs 4)
	//   • Features rendered as runs of ≥2 set pixels (the "white-rule" form)
	//     so single isolated colored pixels — which would impose strict even/
	//     odd column rules per palette — never appear.
	//   • Cols 0 and 13 are typically 0 to keep the silhouette inside the
	//     visible 12-col band, mirroring the tile-edge convention.

	public override int CharWidth =>	AppleIISprites.CharWidth;
	public override int HeadRows =>		AppleIISprites.HeadRows;
	public override int BodyRows =>		AppleIISprites.BodyRows;
	public override int LegsRows =>		AppleIISprites.LegsRows;

	public override HeadPalette[] HeadPalettes =>	AppleIISprites.HeadPalettes;
	public override BodyPalette[] BodyPalettes =>	AppleIISprites.BodyPalettes;
	public override LegsPalette[] LegsPalettes =>	AppleIISprites.LegPalettes;

	public override byte[][][][]	HeadParts { get; }		= [ AppleIISprites.Head0, AppleIISprites.Head1, AppleIISprites.Head2 ];
	public override byte[][][][]	BodyParts { get; }		= [ AppleIISprites.Body0, AppleIISprites.Body1, AppleIISprites.Body2 ];
	public override byte[][][][]	LegsParts { get; }		= [ AppleIISprites.Legs0, AppleIISprites.Legs1, AppleIISprites.Legs2 ];
	public override byte[][][][]	HeadPartsBack { get; }	= [ AppleIISprites.Head0Back, AppleIISprites.Head1Back, AppleIISprites.Head2Back ];
	public override byte[][][][]	BodyPartsBack { get; }	= [ AppleIISprites.Body0Back, AppleIISprites.Body1Back, AppleIISprites.Body2Back ];
	public override byte[][][][]	LegsPartsSide { get; }	= [ AppleIISprites.Legs0Side, AppleIISprites.Legs1Side, AppleIISprites.Legs2Side ];
}
