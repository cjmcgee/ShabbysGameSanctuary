using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.NES;

public class NESPalette : Palette
{
	public NESPalette() : base(
		[
			new Color( 36,  24,   0),   //  0 dark background
			new Color(188, 120,  40),   //  1 warm amber (wood light)
			new Color(148,  80,  16),   //  2 medium brown (wood mid)
			new Color( 96,  40,   0),   //  3 dark brown (wood shadow/grain)
			new Color(220,  20,  20),   //  4 vivid red (carpet)
			new Color(148,   0,   0),   //  5 dark red (carpet shadow)
			new Color(240, 240, 140),   //  6 pale yellow (kitchen tile light)
			new Color(196, 196,  60),   //  7 yellow-green (kitchen tile dark)
			new Color(236, 236, 220),   //  8 near-white (wall)
			new Color(168, 168, 160),   //  9 mid-gray (wall shadow band)
			new Color(212, 212, 200),   // 10 light wall
			new Color( 44,  28,   0),   // 11 near-black (door frame)
			new Color(120,  64,   8),   // 12 warm door wood
			new Color( 40,  80, 196),   // 13 NES blue (furniture)
			new Color(  8,  40, 132),   // 14 dark NES blue (furniture shadow)
			new Color( 96, 180, 196),   // 15 aqua (window glass)
			new Color( 56, 128,  56),   // 16 NES green (grass)
			new Color( 24,  72,  24),   // 17 dark green (grass shadow)
			new Color( 24,  24,  24),   // 18 near-black (road)
			new Color( 64,  64,  64),   // 19 dark gray (road surface)
			new Color(128, 128, 128),   // 20 mid gray (sidewalk)
			new Color(176, 176, 176),   // 21 light gray (sidewalk highlight)
			new Color(180,  80,   8),   // 22 rust orange (bookshelf wood)
			new Color(220, 180, 100),   // 23 warm tan (bookshelf shelf)
			new Color( 40, 160,  48),   // 24 leaf green (plant)
			new Color( 16,  80,  24),   // 25 dark leaf (plant shadow)
			new Color(176, 128,  72),   // 26 warm tan (counter surface)
			new Color(120,  72,  32),   // 27 dark tan (counter edge)
		])
	{
	}
}
