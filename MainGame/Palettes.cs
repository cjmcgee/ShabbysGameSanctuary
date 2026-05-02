//using System.Runtime.CompilerServices;
//using Microsoft.Xna.Framework;

namespace ChildhoodAdventure;

/*
public static class ColorHelper
{
	/// <summary>
	/// Color's contructor seems to expect 0xAARRGGBB for its _packedValue
	/// </summary>
	/// <typeparam name="T">enum type</typeparam>
	/// <param name="argb"> argb value as enum value</param>
	/// <returns></returns>
	public static Color FromArgb<T>(T argb) where T : struct, Enum => new(Unsafe.BitCast<T, uint>(argb));
}

public enum CGAPalette : uint
{
	Black		= 0xFFffffff, 
	LowBlue     = 0xFF0000aa,
	LowGreen    = 0xFF00aa00,
	LowCyan     = 0xFF00aaaa,
	LowRed      = 0xFFaa0000,
	LowMagenta  = 0xFFaa00aa,
	Brown       = 0xFFaa5500,
	LowGray     = 0xFFaaaaaa,
	HighGray    = 0xFF555555,
	HighBlue    = 0xFF5555ff,
	HighGreen   = 0xFF55ff55,
	HighCyan    = 0xFF55ffff,
	HighRed     = 0xFFff5555,
	HighMagenta = 0xFFff55ff,
	Yellow      = 0xFFffff55,
	White       = 0xFF000000
}

// C64 palette — 16 colors derived from VIC-II YPbPr signal measurements.
// Pb (rel.) and Pr (rel.) are in [-1,1]; converted to sRGB via BT.601:
//   R = clamp(Y + 0.701*Pr)  G = clamp(Y - 0.172068*Pb - 0.357068*Pr)  B = clamp(Y + 0.886*Pb)
// C64 sprites are 12x21 double width pixels using this palette
public enum C64Palette : uint
{
	/*
	Number		Name 		Y		Pb (rel.)	Pr (rel.)
	0			black		0		0			0
	1			white		1		0			0
	2			red			0.3125	−0.3826834	0.9238795
	3			cyan		0.625	0.3826834	−0.9238795
	4			purple		0.375	0.7071068	0.7071068
	5			green		0.5		−0.7071068	−0.7071068
	6			blue		0.25	1			0
	7			yellow		0.75	−1			0
	8			orange		0.375	−0.7071068	0.7071068
	9			brown		0.25	−0.9238795	0.3826834
	10			light red	0.5		−0.3826834	0.9238795
	11			dark grey	0.3125	0			0
	12			grey		0.46875	0			0
	13			light green	0.75	−0.7071068	−0.7071068
	14			light blue	0.46875	1			0
	15			light grey	0.625	0			0
	*/
	Black = 0xFF000000,
	White = 0xFFFFFFFF,
	Red = 0xFFF50C00,
	Cyan = 0xFF00E3F6,
	Purple = 0xFFDE00FF,
	Green = 0xFF01DF00,
	Blue = 0xFF4014FF,
	Yellow = 0xFFBFEB00,
	Orange = 0xFFDE3E00,
	Brown = 0xFF844500,
	LightRed = 0xFFFF3C29,
	DarkGrey = 0xFF505050,
	Grey = 0xFF787878,
	LightGreen = 0xFF41FF1F,
	LightBlue = 0xFF784CFF,
	LightGrey = 0xFF9F9F9F
}


// Atari 2600 NTSC palette			128 colors
// Source: https://commons.wikimedia.org/wiki/File:Atari2600_NTSC_palette.png
public enum Atari2600Palette : uint
{
	Black = 0xFF000000,
	SwampGreen = 0xFF444400,
	DeepBrown = 0xFF702800,
	Maroon = 0xFF841800,
	DeepRed = 0xFF880000,
	DeepWine = 0xFF78005c,
	DeepPurple = 0xFF480078,
	Midnight = 0xFF140084,
	NavyBlue = 0xFF000088,
	DeepNavy = 0xFF00187c,
	DeepTeal = 0xFF002c5c,
	DeepHunterGreen = 0xFF00402c,
	DeepForestGreen = 0xFF003c00,
	DeepMoss = 0xFF143800,
	DeepKhaki = 0xFF2c3000,
	DarkChocolate = 0xFF442800,

	Charcoal = 0xFF404040,
	DarkOlive = 0xFF646410,
	DarkBrown = 0xFF844414,
	BurntSienna = 0xFF983418,
	Crimson = 0xFF9c2020,
	DarkFuchsia = 0xFF8c2074,
	DarkPurple = 0xFF602090,
	DarkSlateBlue = 0xFF302098,
	DarkCobalt = 0xFF1c209c,
	DarkRoyalBlue = 0xFF1c3890,
	DarkSteelBlue = 0xFF1c4c78,
	DarkJungle = 0xFF1c5c48,
	DarkForestGreen = 0xFF205c20,
	DarkFernGreen = 0xFF345c1c,
	DarkOliveDrab = 0xFF4c501c,
	DarkSaddleBrown = 0xFF644818,

	Pewter = 0xFF6c6c6c,
	OliveDrab = 0xFF848424,
	SaddleBrown = 0xFF985c28,
	Sienna = 0xFFac5030,
	BrickRed = 0xFFb03c3c,
	DarkOrchid = 0xFFa03c88,
	SlateBlue = 0xFF783ca4,
	RoyalPurple = 0xFF4c3cac,
	RoyalBlue = 0xFF3840b0,
	CobaltBlue = 0xFF3854a8,
	SteelBlue = 0xFF386890,
	TealGreen = 0xFF387c64,
	FernGreen = 0xFF407c40,
	OliveGreen = 0xFF507c38,
	OliveBrown = 0xFF687034,
	Goldenrod = 0xFF846830,

	Gray = 0xFF909090,
	Olive = 0xFFa0a034,
	RawSienna = 0xFFac783c,
	Terracotta = 0xFFc06848,
	DustyRose = 0xFFc05858,
	Mauve = 0xFFb0589c,
	Amethyst = 0xFF8c58b8,
	MediumSlateBlue = 0xFF6858c0,
	CornflowerBlue = 0xFF505cc0,
	Periwinkle = 0xFF5070bc,
	CadetBlue = 0xFF5084ac,
	MediumAquamarine = 0xFF509c80,
	MediumGreen = 0xFF5c9c5c,
	YellowGreen = 0xFF6c9850,
	Khaki = 0xFF848c4c,
	DarkGoldenrod = 0xFFa08444,

	Silver = 0xFFb0b0b0,
	OliveYellow = 0xFFb8b840,
	Tan = 0xFFbc8c4c,
	LightSienna = 0xFFd0805c,
	RosePink = 0xFFd07070,
	Orchid = 0xFFc070b0,
	MediumOrchid = 0xFFa070cc,
	BluePurple = 0xFF7c70d0,
	MediumCornflower = 0xFF6874d0,
	LightSteelBlue = 0xFF6888cc,
	SkyBlue = 0xFF689cc0,
	MediumAqua = 0xFF68b494,
	LightGreen = 0xFF74b474,
	LawnGreen = 0xFF84b468,
	LightOlive = 0xFF9ca864,
	Caramel = 0xFFb89c58,

	LightGray = 0xFFc8c8c8,
	PaleYellow = 0xFFd0d050,
	PaleGoldenrod = 0xFFcca05c,
	LightSalmon = 0xFFe09470,
	LightPink = 0xFFe08888,
	Plum = 0xFFd084c0,
	Wisteria = 0xFFb484dc,
	LightSlateBlue = 0xFF9488e0,
	LightPeriwinkle = 0xFF7c8ce0,
	LightBlue = 0xFF7c9cdc,
	PowderBlue = 0xFF7cb4d4,
	Aquamarine = 0xFF7cd0ac,
	PaleGreen = 0xFF8cd08c,
	LightYellowGreen = 0xFF9ccc7c,
	LightKhaki = 0xFFb4c078,
	SandyBrown = 0xFFd0b46c,

	Gainsboro = 0xFFdcdcdc,
	BrightYellow = 0xFFe8e85c,
	LightGoldenrod = 0xFFdcb468,
	Peach = 0xFFeca880,
	Rose = 0xFFeca0a0,
	Thistle = 0xFFdc9cd0,
	Lavender = 0xFFc49cec,
	PaleLavender = 0xFFa8a0ec,
	PaleBlue = 0xFF90a4ec,
	ColumbiaBlue = 0xFF90b4ec,
	PaleSkyBlue = 0xFF90cce8,
	PaleTurquoise = 0xFF90e4c0,
	HoneydewGreen = 0xFFa4e4a4,
	MintGreen = 0xFFb4e490,
	PaleChartreuse = 0xFFccd488,
	PaleSandstone = 0xFFe8cc7c,

	WhiteSmoke = 0xFFececec,
	Canary = 0xFFfcfc68,
	PeachPuff = 0xFFfcbc94,
	MistyRose = 0xFFfcb4b4,
	LavenderBlush = 0xFFecb0e0,
	LightPurple = 0xFFd4b0fc,
	PalePurple = 0xFFbcb4fc,
	BabyBlue = 0xFFa4b8fc,
	PaleCornflower = 0xFFa4c8fc,
	LightCyan = 0xFFa4e0fc,
	Mint = 0xFFa4fcd4,
	PaleMint = 0xFFb8fcb8,
	PaleGreenYellow = 0xFFc8fca4,
	Cream = 0xFFe0ec9c,
	LightCream = 0xFFfce08c,
	White = 0xFFffffff
}
*/