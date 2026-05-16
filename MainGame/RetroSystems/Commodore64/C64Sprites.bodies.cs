namespace ChildhoodAdventure.RetroSystems.Commodore64;

internal static partial class C64Sprites
{
	public const int CharWidth =	12;
	public const int BodyRows =	8;

	public static BodyPalette[]	BodyPalettes { get; } =
		[
			new("green",
				Skin:			C64Palette.SkinLight,
				Shirt:			C64Palette.Green,	// VIC-II green
				ShirtHighlight:	C64Palette.LightGreen,
				Buttons:		C64Palette.White,
				Accessory:		C64Palette.Cyan),
			new("blue",
				Skin:			C64Palette.SkinLight,
				Shirt:			C64Palette.Blue,	// VIC-II blue
				ShirtHighlight:	C64Palette.MidBlue,
				Buttons:		C64Palette.LightGrey,
				Accessory:		C64Palette.BrightYellow),
			new("red",
				Skin:			C64Palette.SkinLight,
				Shirt:			C64Palette.DeepRed,		// VIC-II red
				ShirtHighlight:	C64Palette.Salmon,
				Buttons:		C64Palette.White,
				Accessory:		C64Palette.BrightYellow),
			new("white/light",
				Skin:			C64Palette.SkinLight,
				Shirt:			C64Palette.LightGrey,
				ShirtHighlight:	C64Palette.White,
				Buttons:		C64Palette.Blue,
				Accessory:		C64Palette.Green),
			new("teal",
				Skin:			C64Palette.SkinLight,
				Shirt:			C64Palette.MidBlue,
				ShirtHighlight:	C64Palette.Cyan,
				Buttons:		C64Palette.White,
				Accessory:		C64Palette.MidMagenta),
		];

	public static readonly byte[][][]	Body0 =	// casual shirt
	[
		[
			[ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
			[ 0, 0, 1, 1, 2, 2, 2, 2, 1, 1, 0, 0 ],		// shoulder skin + shirt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// chest
			[ 0, 0, 2, 2, 3, 3, 2, 2, 3, 3, 0, 0 ],		// highlights at lp2, lp4
			[ 0, 0, 2, 2, 4, 4, 4, 4, 2, 2, 0, 0 ],		// buttons centre
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// mid torso
			[ 0, 0, 3, 3, 2, 2, 2, 2, 3, 3, 0, 0 ],		// side highlights
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// bottom
		],
	];

	public static readonly byte[][][]	Body1 =	// collared / formal
	[
		[
			[ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
			[ 0, 0, 1, 1, 5, 5, 5, 5, 1, 1, 0, 0 ],		// lapels start
			[ 0, 0, 2, 2, 5, 5, 5, 5, 2, 2, 0, 0 ],		// lapels chest
			[ 0, 0, 2, 2, 4, 4, 4, 4, 2, 2, 0, 0 ],		// upper buttons
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// torso
			[ 0, 0, 2, 2, 4, 4, 4, 4, 2, 2, 0, 0 ],		// lower buttons
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// torso (highlight dropped → {1,2,4,5}=4)
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// bottom
		],
	];

	public static readonly byte[][][]	Body2 =	// jacket / hoodie
	[
		[
			[ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
			[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket (full)
			[ 0, 0, 5, 5, 2, 2, 2, 2, 5, 5, 0, 0 ],		// jacket open, shirt visible
			[ 0, 0, 5, 5, 2, 2, 2, 2, 5, 5, 0, 0 ],		// shirt (highlight dropped → {1,2,4,5}=4)
			[ 0, 0, 5, 5, 4, 4, 4, 4, 5, 5, 0, 0 ],		// buttons
			[ 0, 0, 5, 5, 2, 2, 2, 2, 5, 5, 0, 0 ],		// shirt
			[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket lower
			[ 0, 0, 5, 5, 2, 2, 2, 2, 5, 5, 0, 0 ],		// shirt at hem
		],
	];

	public static readonly byte[][][]	Body0Back =	// casual shirt — back view
	[
		[
			[ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
			[ 0, 0, 1, 1, 2, 2, 2, 2, 1, 1, 0, 0 ],		// shoulders + shirt back
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// upper back
			[ 0, 0, 2, 2, 3, 3, 3, 3, 2, 2, 0, 0 ],		// back crease/highlight centre
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// mid back
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// lower back
			[ 0, 0, 3, 3, 2, 2, 2, 2, 3, 3, 0, 0 ],		// side highlights
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// bottom
		],
	];

	public static readonly byte[][][]	Body1Back =	// formal — back view (no lapels/buttons)
	[
		[
			[ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
			[ 0, 0, 1, 1, 2, 2, 2, 2, 1, 1, 0, 0 ],		// shoulders
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// upper back
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// back
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// torso
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// lower back
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// lower back
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// bottom
		],
	];

	public static readonly byte[][][]	Body2Back =	// jacket — back view
	[
		[
			[ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
			[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket back (full)
			[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket
			[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket
			[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket
			[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket
			[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket lower
			[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket bottom
		],
	];

	public static readonly byte[][][]	Body0Side =	// casual shirt — side view
	[
		[
			[ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
			[ 0, 0, 1, 1, 2, 2, 2, 2, 0, 0, 0, 0 ],		// near shoulder + torso
			[ 0, 0, 1, 1, 2, 2, 2, 2, 0, 0, 0, 0 ],		// arm + chest
			[ 0, 0, 1, 1, 2, 2, 2, 2, 0, 0, 0, 0 ],		// arm + mid torso
			[ 0, 0, 1, 1, 2, 2, 2, 2, 0, 0, 0, 0 ],		// arm + torso
			[ 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0 ],		// torso only
			[ 0, 0, 0, 0, 2, 2, 3, 3, 0, 0, 0, 0 ],		// shirt highlight
			[ 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0 ],		// bottom
		],
	];

	public static readonly byte[][][]	Body1Side =	// formal — side view
	[
		[
			[ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
			[ 0, 0, 1, 1, 5, 5, 2, 2, 0, 0, 0, 0 ],		// lapel (near side) + shirt
			[ 0, 0, 2, 2, 5, 5, 2, 2, 0, 0, 0, 0 ],		// lapel + torso
			[ 0, 0, 2, 2, 4, 4, 2, 2, 0, 0, 0, 0 ],		// button visible from side
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// torso
			[ 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0 ],		// lower torso
			[ 0, 0, 0, 0, 4, 4, 2, 2, 0, 0, 0, 0 ],		// lower button
			[ 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0 ],		// bottom
		],
	];

	public static readonly byte[][][]	Body2Side =	// jacket/hoodie — side view
	[
		[
			[ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
			[ 0, 0, 5, 5, 5, 5, 2, 2, 0, 0, 0, 0 ],		// jacket open + shirt
			[ 0, 0, 5, 5, 5, 5, 2, 2, 0, 0, 0, 0 ],		// jacket
			[ 0, 0, 5, 5, 2, 2, 2, 2, 0, 0, 0, 0 ],		// jacket + shirt
			[ 0, 0, 5, 5, 4, 4, 2, 2, 0, 0, 0, 0 ],		// jacket + button
			[ 0, 0, 5, 5, 2, 2, 2, 2, 0, 0, 0, 0 ],		// jacket + shirt
			[ 0, 0, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],		// jacket lower
			[ 0, 0, 5, 5, 2, 2, 2, 2, 0, 0, 0, 0 ],		// shirt at hem
		],
	];
}
