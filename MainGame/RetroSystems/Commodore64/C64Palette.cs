namespace ChildhoodAdventure.RetroSystems.Commodore64;

internal sealed class C64Palette :	Palette
{
	// ── Tile palette ───────────────────────────────────────────────────────
	public static readonly Color Black			= new(  0,   0,   0);	//  0  black       (bg)
	public static readonly Color Brown			= new(132,  96,  60);	//  1  brown        — wood floor
	public static readonly Color DarkBrown		= new( 72,  40,   0);	//  2  dark brown   — wood grain
	public static readonly Color Red			= new(245,  12,   0);	//  3  C64 red      — carpet
	public static readonly Color DarkRed		= new(140,   0,   0);	//  4  dark red     — carpet border
	public static readonly Color Yellow			= new(191, 235,   0);	//  5  C64 yellow   — kitchen tile
	public static readonly Color White			= new(255, 255, 255);	//  6  white        — wall light
	public static readonly Color Grey			= new(120, 120, 120);	//  7  C64 grey     — wall mortar (global multicolor register)
	public static readonly Color Blue			= new( 64,  20, 255);	//  8  C64 blue     — door frame
	public static readonly Color DoorBlack		= new(  0,   0,   0);	//  9  black        — door panel dark
	public static readonly Color LightGrey		= new(172, 172, 172);	// 10  C64 lt grey  — counter
	public static readonly Color Cyan			= new(  0, 227, 246);	// 11  C64 cyan     — window / plant accent
	public static readonly Color Green			= new(  1, 223,   0);	// 12  C64 green    — grass
	public static readonly Color NearBlack		= new( 40,  40,  40);	// 13  near-black   — road
	public static readonly Color DarkGrey		= new( 80,  80,  80);	// 14  C64 dark grey — sidewalk

	// ── Sprite-only extensions ─────────────────────────────────────────────
	// Used by Head/Body/Legs palettes; not used by any tile art.
	public static readonly Color MidBlue		= new(  0, 136, 255);	// 15  sprite-only
	public static readonly Color DarkGray		= new( 51,  51,  51);	// 16  sprite-only
	public static readonly Color DarkOlive		= new(102,  68,   0);	// 17  sprite-only
	public static readonly Color MidBrown		= new(119,  85,  34);	// 18  sprite-only
	public static readonly Color DeepRed		= new(136,   0,   0);	// 19  sprite-only
	public static readonly Color SkinMedium		= new(145, 100,  50);	// 20  sprite-only
	public static readonly Color SkinMedium2	= new(150, 105,  40);	// 21  sprite-only
	public static readonly Color LightGreen		= new(170, 255, 102);	// 22  sprite-only
	public static readonly Color BurntOrange	= new(180,  90,   0);	// 23  sprite-only
	public static readonly Color SkinTan		= new(180, 130,  80);	// 24  sprite-only
	public static readonly Color SkinTan2		= new(200, 155, 100);	// 25  sprite-only
	public static readonly Color MidMagenta		= new(204,  68, 204);	// 26  sprite-only
	public static readonly Color SkinTan3		= new(221, 136,  85);	// 27  sprite-only
	public static readonly Color SkinLight		= new(221, 179, 130);	// 28  sprite-only
	public static readonly Color SkinPale		= new(235, 205, 155);	// 29  sprite-only
	public static readonly Color BrightYellow	= new(238, 238, 119);	// 30  sprite-only
	public static readonly Color Salmon			= new(255, 119, 119);	// 31  sprite-only

	public C64Palette()	:	base(
 		[
			Black,
			Brown,
			DarkBrown,
			Red,
			DarkRed,
			Yellow,
			White,
			Grey,
			Blue,
			DoorBlack,
			LightGrey,
			Cyan,
			Green,
			NearBlack,
			DarkGrey,
			MidBlue,
			DarkGray,
			DarkOlive,
			MidBrown,
			DeepRed,
			SkinMedium,
			SkinMedium2,
			LightGreen,
			BurntOrange,
			SkinTan,
			SkinTan2,
			MidMagenta,
			SkinTan3,
			SkinLight,
			SkinPale,
			BrightYellow,
			Salmon,
		])
	{
	}
}
