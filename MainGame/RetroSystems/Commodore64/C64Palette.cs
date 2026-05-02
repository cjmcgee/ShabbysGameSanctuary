using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.Commodore64;

public class C64Palette : Palette
{
	public C64Palette() : base(
		[
			new Color(  0,   0,   0),   //  0 black       (bg)
			new Color(132,  96,  60),   //  1 brown        — wood floor
			new Color( 72,  40,   0),   //  2 dark brown   — wood grain
			new Color(245,  12,   0),   //  3 C64 red      — carpet
			new Color(140,   0,   0),   //  4 dark red     — carpet border
			new Color(191, 235,   0),   //  5 C64 yellow   — kitchen tile
			new Color(255, 255, 255),   //  6 white        — wall light
			new Color(120, 120, 120),   //  7 C64 grey     — wall mortar (global multicolor register)
			new Color( 64,  20, 255),   //  8 C64 blue     — door frame
			new Color(  0,   0,   0),   //  9 black        — door panel dark
			new Color(172, 172, 172),   // 10 C64 lt grey  — counter
			new Color(  0, 227, 246),   // 11 C64 cyan     — window / plant accent
			new Color(  1, 223,   0),   // 12 C64 green    — grass
			new Color( 40,  40,  40),   // 13 near-black   — road
			new Color( 80,  80,  80),   // 14 C64 dark grey — sidewalk
		])
	{
	}
}
