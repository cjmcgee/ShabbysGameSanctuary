namespace ChildhoodAdventure.RetroSystems.Atari2600;

internal sealed class Atari2600Palette :	Palette
{
	// ── Tile palette ────────────────────────────────────────────────────────
	// Each tile in the Atari2600 art uses index 0 (background) plus ONE
	// non-zero foreground index. Entries 2/4/7 are reserved for sprites.
	public static readonly Color Black			= new(  0,   0,   0);		//  0  background
	public static readonly Color WarmAmber		= new(188, 140,  56);		//  1  WoodFloor
	public static readonly Color DarkBrown		= new(124,  72,   8);		//  2  sprite
	public static readonly Color VividRed		= new(200,  20,  20);		//  3  Carpet, Bookshelf
	public static readonly Color DarkRed		= new(132,   4,   4);		//  4  sprite
	public static readonly Color BrightYellow	= new(220, 220,   0);		//  5  KitchenTile
	public static readonly Color NearWhite		= new(220, 220, 200);		//  6  Wall
	public static readonly Color LightGray		= new(148, 148, 132);		//  7  sprite
	public static readonly Color NearBlack		= new( 20,  12,   4);		//  8  Door
	public static readonly Color BoldBlue		= new( 36,  80, 200);		//  9  Furniture
	public static readonly Color MidGray		= new(144, 144, 144);		// 10  Counter
	public static readonly Color BrightCyan		= new(  0, 200, 220);		// 11  Window
	public static readonly Color VividGreen		= new(  0, 188,   0);		// 12  Grass, Plant
	public static readonly Color DarkSlate		= new( 28,  28,  40);		// 13  Road
	public static readonly Color MediumGray		= new(104, 104, 104);		// 14  Sidewalk

	// ── Sprite-only extensions ──────────────────────────────────────────────
	// Used by Body palette accessories; never appear in tile art.
	public static readonly Color DarkGreen		= new(  0, 100,   0);		// 15  body accent
	public static readonly Color DarkTeal		= new(  0, 100, 120);		// 16  body accent

	public Atari2600Palette()	:	base(
 		[
			Black,
			WarmAmber,
			DarkBrown,
			VividRed,
			DarkRed,
			BrightYellow,
			NearWhite,
			LightGray,
			NearBlack,
			BoldBlue,
			MidGray,
			BrightCyan,
			VividGreen,
			DarkSlate,
			MediumGray,
			DarkGreen,
			DarkTeal,
 		])
	{
	}
}
