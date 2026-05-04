namespace ChildhoodAdventure.RetroSystems.Commodore64;

public static partial class C64Sprites
{
	public const int LegsRows =	7;

	public static LegsPalette[]	LegPalettes { get; } =
		[
			new("blue jeans/brown shoes",
				Skin:			C64Palette.SkinLight,
				Pants:			C64Palette.Blue,	// VIC-II blue
				PantsHighlight:	C64Palette.MidBlue,
				Belt:			C64Palette.DarkOlive,
				BeltHighlight:	C64Palette.SkinTan3,
				Shoes:			C64Palette.DarkBrown,
				ShoeHighlight:	C64Palette.Brown),
			new("black/black",
				Skin:			C64Palette.SkinLight,
				Pants:			C64Palette.DoorBlack,
				PantsHighlight:	C64Palette.DarkGray,
				Belt:			C64Palette.DarkGray,
				BeltHighlight:	C64Palette.Grey,
				Shoes:			C64Palette.DoorBlack,
				ShoeHighlight:	C64Palette.DarkGray),
			new("khaki/tan",
				Skin:			C64Palette.SkinLight,
				Pants:			C64Palette.SkinTan3,
				PantsHighlight:	C64Palette.BrightYellow,
				Belt:			C64Palette.DarkOlive,
				BeltHighlight:	C64Palette.BurntOrange,
				Shoes:			C64Palette.DarkOlive,
				ShoeHighlight:	C64Palette.SkinMedium2),
			new("gray/dark",
				Skin:			C64Palette.SkinLight,
				Pants:			C64Palette.Grey,
				PantsHighlight:	C64Palette.LightGrey,
				Belt:			C64Palette.DarkGray,
				BeltHighlight:	C64Palette.Grey,
				Shoes:			C64Palette.DarkGray,
				ShoeHighlight:	C64Palette.Grey),
		];

	public static readonly byte[][][]	Legs0 =	// pants + belt
	[
		// Frame 0 — idle
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// upper pants (buckle highlight dropped)
			[ 0, 0, 2, 2, 3, 3, 3, 3, 2, 2, 0, 0 ],		// upper pants crease
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// pants merged
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],		// shoes
			[ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],		// shoes (highlight dropped → {2,3,4,6}=4)
		],
		// Frame 1 — left foot forward
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// upper pants (buckle highlight dropped)
			[ 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],		// left leg strides out
			[ 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2 ],		// legs spreading
			[ 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2 ],		// wide stride
			[ 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6 ],		// shoes spread
			[ 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6 ],		// shoes (highlight dropped)
		],
		// Frame 2 — crossing / mid stride
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// upper pants (buckle highlight dropped)
			[ 0, 0, 2, 2, 3, 3, 3, 3, 2, 2, 0, 0 ],		// legs crossing
			[ 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0 ],		// coming together
			[ 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoes together
			[ 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoes (highlight dropped)
		],
		// Frame 3 — right foot forward
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// upper pants (buckle highlight dropped)
			[ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2 ],		// right leg strides out
			[ 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2 ],
			[ 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2 ],
			[ 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6 ],
			[ 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6 ],		// shoes (highlight dropped)
		],
	];

	public static readonly byte[][][]	Legs1 =	// formal trousers — crease highlight
	[
		// Frame 0 — idle
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// upper pants (buckle highlight dropped)
			[ 0, 0, 2, 2, 3, 3, 3, 3, 2, 2, 0, 0 ],		// crease
			[ 0, 0, 2, 2, 3, 3, 3, 3, 2, 2, 0, 0 ],		// crease continues
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
			[ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],		// shoes (highlight dropped → {2,3,4,6}=4)
		],
		// Frame 1 — left foot forward
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// upper pants (buckle highlight dropped)
			[ 2, 2, 2, 2, 3, 3, 0, 0, 0, 0, 0, 0 ],		// left leg out
			[ 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2 ],
			[ 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2 ],
			[ 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6 ],
			[ 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6 ],		// shoes (highlight dropped)
		],
		// Frame 2 — crossing
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// upper pants (buckle highlight dropped)
			[ 0, 0, 2, 2, 3, 3, 3, 3, 2, 2, 0, 0 ],
			[ 0, 0, 0, 0, 3, 3, 3, 3, 0, 0, 0, 0 ],		// crease visible mid-cross
			[ 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoes (highlight dropped)
		],
		// Frame 3 — right foot forward
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// upper pants (buckle highlight dropped)
			[ 0, 0, 0, 0, 0, 0, 3, 3, 2, 2, 2, 2 ],		// right leg out
			[ 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2 ],
			[ 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2 ],
			[ 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6 ],
			[ 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6 ],		// shoes (highlight dropped)
		],
	];

	public static readonly byte[][][]	Legs2 =	// shorts + bare skin below knee
	[
		// Frame 0 — idle
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shorts (buckle highlight dropped)
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shorts (crease dropped → {1,2,4,6}=4)
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shorts end
			[ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],		// bare legs
			[ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],		// shoes
			[ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],		// shoes (highlight dropped)
		],
		// Frame 1 — left foot forward
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shorts (buckle highlight dropped)
			[ 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],		// left shorts out
			[ 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1 ],		// bare legs spreading
			[ 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 ],		// wide stride
			[ 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6 ],
			[ 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6 ],		// shoes (highlight dropped)
		],
		// Frame 2 — crossing
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shorts (buckle highlight dropped)
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shorts crossing
			[ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],		// bare legs
			[ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],		// narrowing
			[ 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoes (highlight dropped)
		],
		// Frame 3 — right foot forward
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shorts (buckle highlight dropped)
			[ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2 ],		// right shorts out
			[ 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1 ],
			[ 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 ],
			[ 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6 ],
			[ 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6 ],		// shoes (highlight dropped)
		],
	];

	public static readonly byte[][][]	Legs0Side =	// pants + belt — side walk
	[
		// Frame 0 — idle / standing
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// upper pants
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// pants
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// pants
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// lower pants
			[ 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoe
			[ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],		// shoe toe extends right (forward)
		],
		// Frame 1 — front foot forward (right)
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// hips
			[ 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],		// back leg angles left
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0, 0 ],		// front leg angles right
			[ 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 0, 0 ],		// legs spread
			[ 6, 6, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],		// back shoe left / front shoe right
			[ 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],		// front shoe toe
		],
		// Frame 2 — crossing / mid stride
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// hips
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// legs crossing
			[ 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0 ],		// narrowing
			[ 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0 ],		//
			[ 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoe centre
			[ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 0, 0 ],		// shoe toe right
		],
		// Frame 3 — back foot forward (right), near foot trailing
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// hips
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0, 0 ],		// far leg forward right
			[ 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],		// near leg back left
			[ 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 0, 0 ],		// legs spread
			[ 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6 ],		// near shoe back / far shoe right
			[ 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],		// far shoe toe
		],
	];

	public static readonly byte[][][]	Legs1Side =	// formal trousers — side walk
	[
		// Frame 0 — idle
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// upper pants
			[ 0, 0, 2, 2, 3, 3, 2, 2, 0, 0, 0, 0 ],		// crease visible from side
			[ 0, 0, 2, 2, 3, 3, 2, 2, 0, 0, 0, 0 ],		// crease continues
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// lower pants
			[ 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoe
			[ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],		// shoe toe
		],
		// Frame 1 — front foot forward
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// hips
			[ 2, 2, 2, 2, 3, 3, 0, 0, 0, 0, 0, 0 ],		// back leg with crease, angles left
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0, 0 ],		// front leg angles right
			[ 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 0, 0 ],		// legs spread
			[ 6, 6, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],		// shoes
			[ 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],		// front shoe toe
		],
		// Frame 2 — crossing
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// hips
			[ 0, 0, 2, 2, 3, 3, 2, 2, 0, 0, 0, 0 ],		// crossing (crease visible)
			[ 0, 0, 0, 0, 3, 3, 2, 2, 0, 0, 0, 0 ],		// narrowing
			[ 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0 ],		//
			[ 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoe
			[ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 0, 0 ],		// shoe toe
		],
		// Frame 3 — back foot forward
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// hips
			[ 0, 0, 0, 0, 2, 2, 3, 3, 2, 2, 0, 0 ],		// far leg forward right (crease)
			[ 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],		// near leg back left
			[ 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 0, 0 ],		// spread
			[ 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6 ],		// shoes
			[ 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],		// far shoe toe
		],
	];

	public static readonly byte[][][]	Legs2Side =	// shorts + bare skin — side walk
	[
		// Frame 0 — idle
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// shorts
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// shorts
			[ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// bare legs
			[ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// bare legs
			[ 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoe
			[ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],		// shoe toe
		],
		// Frame 1 — front foot forward
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// shorts
			[ 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],		// back shorts angles left
			[ 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0 ],		// bare legs spread
			[ 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0 ],		// spread
			[ 6, 6, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],		// shoes
			[ 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],		// front shoe toe
		],
		// Frame 2 — crossing
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// shorts
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// shorts crossing
			[ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// bare legs
			[ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],		// narrowing
			[ 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoe
			[ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 0, 0 ],		// shoe toe
		],
		// Frame 3 — back foot forward
		[
			[ 0, 0, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// shorts
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0, 0 ],		// far shorts forward right
			[ 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0 ],		// bare legs spread
			[ 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0 ],		// spread
			[ 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6 ],		// shoes
			[ 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],		// far shoe toe
		],
	];
}
