namespace ChildhoodAdventure.RetroSystems.AppleII;

public static partial class AppleIISprites
{
	public const int LegsRows =	4;

	public static LegsPalette[]	LegPalettes { get; } =
		[
			new("blue pants",
				Skin:			AppleIIPalette.Orange,	// orange
				Pants:			AppleIIPalette.Blue,	// blue
				PantsHighlight:	AppleIIPalette.White,	// white
				Belt:			AppleIIPalette.Black,	// black
				BeltHighlight:	AppleIIPalette.White,	// white
				Shoes:			AppleIIPalette.Black,	// black
				ShoeHighlight:	AppleIIPalette.Orange),	// orange

			new("violet pants",
				Skin:			AppleIIPalette.Orange,	// orange
				Pants:			AppleIIPalette.Violet,	// violet
				PantsHighlight:	AppleIIPalette.White,	// white
				Belt:			AppleIIPalette.Black,	// black
				BeltHighlight:	AppleIIPalette.Orange,	// orange
				Shoes:			AppleIIPalette.Black,	// black
				ShoeHighlight:	AppleIIPalette.White),	// white

			new("black pants",
				Skin:			AppleIIPalette.Orange,	// orange
				Pants:			AppleIIPalette.Black,	// black
				PantsHighlight:	AppleIIPalette.Blue,	// blue
				Belt:			AppleIIPalette.Orange,	// orange
				BeltHighlight:	AppleIIPalette.White,	// white
				Shoes:			AppleIIPalette.Black,	// black
				ShoeHighlight:	AppleIIPalette.White),	// white

			new("green pants",
				Skin:			AppleIIPalette.Orange,	// orange
				Pants:			AppleIIPalette.Green,	// green
				PantsHighlight:	AppleIIPalette.White,	// white
				Belt:			AppleIIPalette.Black,	// black
				BeltHighlight:	AppleIIPalette.Orange,	// orange
				Shoes:			AppleIIPalette.Black,	// black
				ShoeHighlight:	AppleIIPalette.White),	// white
		];

	public static readonly byte[][][]	Legs0 =	// pants + belt
	[
		[   // idle
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],	// belt
			[ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],	// pants + buckle
			[ 0, 2, 2, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 0 ],	// legs split
			[ 0, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6, 0 ],	// shoes
		],
		[   // left foot forward
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
			[ 0, 2, 2, 2, 2, 2, 2, 2, 0, 0, 2, 2, 2, 0 ],	// left ahead
			[ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6, 6, 0 ],
		],
		[   // crossing / passing
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],	// legs together
			[ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
		],
		[   // right foot forward
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
			[ 0, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 2, 2, 0 ],	// right ahead
			[ 0, 6, 6, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0 ],
		],
	];

	public static readonly byte[][][]	Legs1 =	// formal trousers (crease)
	[
		[   // idle
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],	// belt
			[ 0, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 0 ],	// crease band
			[ 0, 2, 2, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 0 ],	// legs split
			[ 0, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6, 0 ],	// shoes
		],
		[   // left foot forward
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 0 ],
			[ 0, 2, 2, 2, 2, 2, 2, 2, 0, 0, 2, 2, 2, 0 ],
			[ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6, 6, 0 ],
		],
		[   // crossing
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
		],
		[   // right foot forward
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 0 ],
			[ 0, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 2, 2, 0 ],
			[ 0, 6, 6, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0 ],
		],
	];

	public static readonly byte[][][]	Legs2 =	// shorts + bare skin
	[
		[   // idle
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],	// belt
			[ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],	// shorts + buckle
			[ 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0 ],	// bare legs
			[ 0, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6, 0 ],	// shoes
		],
		[   // left foot forward
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
			[ 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0 ],
			[ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6, 6, 0 ],
		],
		[   // crossing
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
			[ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
			[ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
		],
		[   // right foot forward
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
			[ 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0 ],
			[ 0, 6, 6, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0 ],
		],
	];

	public static readonly byte[][][]	Legs0Side =
	[
		[   // idle — both legs centered, toe extends right
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],	// belt
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],	// pants
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],	// shoe extends right (toe)
		],
		[   // walk A — front foot forward
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],
			[ 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2 ],	// legs split fwd/back
			[ 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6 ],	// back heel + front toe
		],
		[   // mid — legs together, lifted
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],	// feet lifted
		],
		[   // walk B — back foot forward (heel kick)
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 0, 0 ],	// both feet planted
		],
	];

	public static readonly byte[][][]	Legs1Side =
	[
		[   // idle
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
		],
		[   // walk A
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],
			[ 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2 ],
			[ 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6 ],
		],
		[   // mid
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
		],
		[   // walk B
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 0, 0 ],
		],
	];

	public static readonly byte[][][]	Legs2Side =
	[
		[   // idle (shorts + bare legs)
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
			[ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
		],
		[   // walk A
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],
			[ 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1 ],
			[ 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6 ],
		],
		[   // mid
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
			[ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
		],
		[   // walk B
			[ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0 ],
			[ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 0, 0 ],
		],
	];
}
