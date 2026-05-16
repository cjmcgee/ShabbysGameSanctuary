namespace ChildhoodAdventure.RetroSystems.NES;

internal sealed class NESPalette :	Palette
{
	// ── Tile palette ───────────────────────────────────────────────────────
	public static readonly Color DarkBackground	= new( 36,  24,   0);	//  0  dark background
	public static readonly Color WarmAmber		= new(188, 120,  40);	//  1  warm amber (wood light)
	public static readonly Color MediumBrown	= new(148,  80,  16);	//  2  medium brown (wood mid)
	public static readonly Color DarkBrown		= new( 96,  40,   0);	//  3  dark brown (wood shadow/grain)
	public static readonly Color VividRed		= new(220,  20,  20);	//  4  vivid red (carpet)
	public static readonly Color DarkRed		= new(148,   0,   0);	//  5  dark red (carpet shadow)
	public static readonly Color PaleYellow		= new(240, 240, 140);	//  6  pale yellow (kitchen tile light)
	public static readonly Color YellowGreen 	= new(196, 196,  60);	//  7  yellow-green (kitchen tile dark)
	public static readonly Color NearWhite		= new(236, 236, 220);	//  8  near-white (wall)
	public static readonly Color WallGray		= new(168, 168, 160);	//  9  mid-gray (wall shadow band)
	public static readonly Color LightWall		= new(212, 212, 200);	// 10  light wall
	public static readonly Color DoorBlack		= new( 44,  28,   0);	// 11  near-black (door frame)
	public static readonly Color WarmDoorWood 	= new(120,  64,   8);	// 12  warm door wood
	public static readonly Color NesBlue		= new( 40,  80, 196);	// 13  NES blue (furniture)
	public static readonly Color DarkNesBlue	= new(  8,  40, 132);	// 14  dark NES blue (furniture shadow)
	public static readonly Color Aqua			= new( 96, 180, 196);	// 15  aqua (window glass)
	public static readonly Color NesGreen		= new( 56, 128,  56);	// 16  NES green (grass)
	public static readonly Color DarkGreen		= new( 24,  72,  24);	// 17  dark green (grass shadow)
	public static readonly Color RoadBlack		= new( 24,  24,  24);	// 18  near-black (road)
	public static readonly Color RoadGray		= new( 64,  64,  64);	// 19  dark gray (road surface)
	public static readonly Color SidewalkGray	= new(128, 128, 128);	// 20  mid gray (sidewalk)
	public static readonly Color SidewalkLight	= new(176, 176, 176);	// 21  light gray (sidewalk highlight)
	public static readonly Color RustOrange		= new(180,  80,   8);	// 22  rust orange (bookshelf wood)
	public static readonly Color ShelfTan		= new(220, 180, 100);	// 23  warm tan (bookshelf shelf)
	public static readonly Color LeafGreen		= new( 40, 160,  48);	// 24  leaf green (plant)
	public static readonly Color DarkLeaf		= new( 16,  80,  24);	// 25  dark leaf (plant shadow)
	public static readonly Color CounterTan		= new(176, 128,  72);	// 26  warm tan (counter surface)
	public static readonly Color CounterDark	= new(120,  72,  32);	// 27  dark tan (counter edge)

	// ── Sprite-only extensions ─────────────────────────────────────────────
	// Used by Head/Body/Legs palettes; not used by any tile art.
	public static readonly Color DarkestBrown	= new( 60,  30,   0);	// 28  sprite-only
	public static readonly Color SkinDark		= new(120,  80,  36);	// 29  sprite-only
	public static readonly Color SkinMedium		= new(148,  96,  48);	// 30  sprite-only
	public static readonly Color SkinTan		= new(188, 136,  72);	// 31  sprite-only
	public static readonly Color PaleCyan		= new(196, 232, 228);	// 32  sprite-only
	public static readonly Color MidMagenta		= new(204,  68, 204);	// 33  sprite-only
	public static readonly Color SkinLight		= new(216, 168, 100);	// 34  sprite-only
	public static readonly Color SkinFair		= new(228, 192, 148);	// 35  sprite-only
	public static readonly Color SkinPale		= new(244, 212, 172);	// 36  sprite-only
	public static readonly Color BrightYellow	= new(248, 216,  96);	// 37  sprite-only

	public NESPalette()	:	base(
		[
			DarkBackground,
			WarmAmber,
			MediumBrown,
			DarkBrown,
			VividRed,
			DarkRed,
			PaleYellow,
			YellowGreen,
			NearWhite,
			WallGray,
			LightWall,
			DoorBlack,
			WarmDoorWood,
			NesBlue,
			DarkNesBlue,
			Aqua,
			NesGreen,
			DarkGreen,
			RoadBlack,
			RoadGray,
			SidewalkGray,
			SidewalkLight,
			RustOrange,
			ShelfTan,
			LeafGreen,
			DarkLeaf,
			CounterTan,
			CounterDark,
			DarkestBrown,
			SkinDark,
			SkinMedium,
			SkinTan,
			PaleCyan,
			MidMagenta,
			SkinLight,
			SkinFair,
			SkinPale,
			BrightYellow,
		])
	{
	}
}
