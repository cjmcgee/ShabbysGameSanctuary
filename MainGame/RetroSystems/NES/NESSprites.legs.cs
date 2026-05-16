namespace ChildhoodAdventure.RetroSystems.NES;

internal static partial class NESSprites
{
	public const int LegsRows =	6;

	public static LegsPalette[]	LegPalettes { get; } =
		[
			new("blue jeans/brown shoes",
				Skin:			NESPalette.SkinFair,
				Pants:			NESPalette.NesBlue,		// NES blue
				PantsHighlight:	NESPalette.Aqua,
				Belt:			NESPalette.DarkBrown,
				BeltHighlight:	NESPalette.RustOrange,
				Shoes:			NESPalette.DarkestBrown,
				ShoeHighlight:	NESPalette.WarmDoorWood),
			new("black/black",
				Skin:			NESPalette.SkinFair,
				Pants:			NESPalette.DoorBlack,
				PantsHighlight:	NESPalette.RoadGray,
				Belt:			NESPalette.RoadGray,
				BeltHighlight:	NESPalette.SidewalkGray,
				Shoes:			NESPalette.DoorBlack,
				ShoeHighlight:	NESPalette.RoadGray),
			new("khaki/tan",
				Skin:			NESPalette.SkinFair,
				Pants:			NESPalette.SkinTan,
				PantsHighlight:	NESPalette.PaleYellow,
				Belt:			NESPalette.DarkBrown,
				BeltHighlight:	NESPalette.MediumBrown,
				Shoes:			NESPalette.DarkBrown,
				ShoeHighlight:	NESPalette.MediumBrown),
			new("gray/dark",
				Skin:			NESPalette.SkinFair,
				Pants:			NESPalette.SidewalkGray,
				PantsHighlight:	NESPalette.WallGray,
				Belt:			NESPalette.RoadGray,
				BeltHighlight:	NESPalette.SidewalkGray,
				Shoes:			NESPalette.RoadGray,
				ShoeHighlight:	NESPalette.SidewalkGray),
		];

	public static readonly byte[][][]	Legs0 =	// pants + belt
	[
		// Frame 0 — idle
		[
			[ 0, 4, 4, 4, 4, 4, 4, 0 ],		// belt
			[ 0, 2, 2, 5, 5, 2, 2, 0 ],		// buckle
			[ 0, 2, 3, 2, 2, 3, 2, 0 ],		// upper pants + creases
			[ 0, 2, 2, 0, 0, 2, 2, 0 ],		// leg gap
			[ 0, 2, 3, 0, 0, 3, 2, 0 ],		// lower pants
			[ 0, 6, 7, 0, 0, 7, 6, 0 ],		// shoes with toe highlight
		],
		// Frame 1 — left foot forward
		[
			[ 0, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 5, 5, 2, 2, 0 ],
			[ 0, 2, 3, 2, 2, 3, 2, 0 ],
			[ 2, 2, 2, 0, 0, 2, 2, 0 ],		// left leg out
			[ 2, 3, 0, 0, 0, 0, 2, 0 ],
			[ 6, 7, 0, 0, 0, 0, 6, 0 ],
		],
		// Frame 2 — crossing
		[
			[ 0, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 5, 5, 2, 2, 0 ],
			[ 0, 2, 3, 2, 2, 3, 2, 0 ],
			[ 0, 2, 3, 3, 3, 3, 2, 0 ],		// crossing
			[ 0, 0, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 6, 6, 6, 6, 0, 0 ],
		],
		// Frame 3 — right foot forward
		[
			[ 0, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 5, 5, 2, 2, 0 ],
			[ 0, 2, 3, 2, 2, 3, 2, 0 ],
			[ 0, 2, 2, 0, 0, 2, 2, 2 ],		// right leg out
			[ 0, 2, 0, 0, 0, 0, 3, 2 ],
			[ 0, 6, 0, 0, 0, 0, 7, 6 ],
		],
	];

	public static readonly byte[][][]	Legs1 =	// formal trousers (crease line)
	[
		// Frame 0 — idle
		[
			[ 0, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 5, 5, 2, 2, 0 ],
			[ 0, 2, 3, 2, 2, 3, 2, 0 ],
			[ 0, 2, 3, 0, 0, 3, 2, 0 ],		// crease down each leg
			[ 0, 2, 3, 0, 0, 3, 2, 0 ],
			[ 0, 6, 7, 0, 0, 7, 6, 0 ],
		],
		// Frame 1 — left foot forward
		[
			[ 0, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 5, 5, 2, 2, 0 ],
			[ 0, 2, 3, 2, 2, 3, 2, 0 ],
			[ 2, 2, 3, 0, 0, 3, 2, 0 ],
			[ 2, 3, 0, 0, 0, 0, 3, 0 ],
			[ 6, 7, 0, 0, 0, 0, 6, 0 ],
		],
		// Frame 2 — crossing
		[
			[ 0, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 5, 5, 2, 2, 0 ],
			[ 0, 2, 3, 2, 2, 3, 2, 0 ],
			[ 0, 2, 3, 3, 3, 3, 2, 0 ],		// crossing with creases
			[ 0, 0, 2, 3, 3, 2, 0, 0 ],
			[ 0, 0, 6, 6, 6, 6, 0, 0 ],
		],
		// Frame 3 — right foot forward
		[
			[ 0, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 5, 5, 2, 2, 0 ],
			[ 0, 2, 3, 2, 2, 3, 2, 0 ],
			[ 0, 2, 3, 0, 0, 3, 2, 2 ],
			[ 0, 3, 0, 0, 0, 0, 3, 2 ],
			[ 0, 6, 0, 0, 0, 0, 7, 6 ],
		],
	];

	public static readonly byte[][][]	Legs2 =	// shorts + bare skin
	[
		// Frame 0 — idle
		[
			[ 0, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 5, 5, 2, 2, 0 ],
			[ 0, 2, 3, 2, 2, 3, 2, 0 ],		// shorts
			[ 0, 1, 1, 0, 0, 1, 1, 0 ],		// bare skin
			[ 0, 1, 1, 0, 0, 1, 1, 0 ],
			[ 0, 6, 7, 0, 0, 7, 6, 0 ],		// shoes
		],
		// Frame 1 — left foot forward
		[
			[ 0, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 5, 5, 2, 2, 0 ],
			[ 0, 2, 3, 2, 2, 3, 2, 0 ],
			[ 1, 1, 1, 0, 0, 1, 1, 0 ],		// left bare leg out
			[ 1, 1, 0, 0, 0, 0, 1, 0 ],
			[ 6, 7, 0, 0, 0, 0, 6, 0 ],
		],
		// Frame 2 — crossing
		[
			[ 0, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 5, 5, 2, 2, 0 ],
			[ 0, 2, 3, 2, 2, 3, 2, 0 ],
			[ 0, 1, 1, 1, 1, 1, 1, 0 ],		// bare legs crossing
			[ 0, 0, 1, 1, 1, 1, 0, 0 ],
			[ 0, 0, 6, 6, 6, 6, 0, 0 ],
		],
		// Frame 3 — right foot forward
		[
			[ 0, 4, 4, 4, 4, 4, 4, 0 ],
			[ 0, 2, 2, 5, 5, 2, 2, 0 ],
			[ 0, 2, 3, 2, 2, 3, 2, 0 ],
			[ 0, 1, 1, 0, 0, 1, 1, 1 ],		// right bare leg out
			[ 0, 1, 0, 0, 0, 0, 1, 1 ],
			[ 0, 6, 0, 0, 0, 0, 7, 6 ],
		],
	];

	public static readonly byte[][][]	Legs0Side =
	[
		// Frame 0 — idle (feet together, toe extends right)
		[
			[ 0, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 2, 2, 5, 5, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 2, 0, 0 ],
			[ 0, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 2, 0, 0 ],
			[ 0, 6, 7, 6, 6, 6, 7, 0 ],		// shoe + toe right
		],
		// Frame 1 — front foot forward
		[
			[ 0, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 2, 2, 5, 5, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 2, 0, 0 ],
			[ 2, 2, 2, 2, 2, 2, 2, 0 ],		// pants splay
			[ 2, 2, 0, 0, 0, 2, 2, 0 ],		// legs split
			[ 6, 6, 0, 0, 0, 6, 6, 7 ],		// shoes
		],
		// Frame 2 — crossing
		[
			[ 0, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 2, 2, 5, 5, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 2, 0, 0 ],
			[ 0, 2, 3, 2, 3, 2, 0, 0 ],
			[ 0, 0, 2, 3, 2, 0, 0, 0 ],
			[ 0, 0, 6, 6, 6, 6, 0, 0 ],
		],
		// Frame 3 — back foot forward
		[
			[ 0, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 2, 2, 5, 5, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 2, 0, 0 ],
			[ 0, 2, 2, 2, 2, 2, 2, 0 ],
			[ 0, 2, 0, 0, 0, 2, 2, 0 ],
			[ 0, 6, 0, 0, 0, 6, 6, 7 ],
		],
	];

	public static readonly byte[][][]	Legs1Side =
	[
		// Frame 0 — idle
		[
			[ 0, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 2, 2, 5, 5, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 3, 0, 0 ],		// crease both legs
			[ 0, 2, 3, 2, 2, 3, 0, 0 ],
			[ 0, 2, 3, 2, 2, 3, 0, 0 ],
			[ 0, 6, 7, 6, 6, 6, 7, 0 ],
		],
		// Frame 1 — front foot forward
		[
			[ 0, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 2, 2, 5, 5, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 3, 0, 0 ],
			[ 2, 2, 3, 2, 3, 2, 2, 0 ],
			[ 2, 3, 0, 0, 0, 3, 2, 0 ],
			[ 6, 7, 0, 0, 0, 6, 7, 0 ],
		],
		// Frame 2 — crossing
		[
			[ 0, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 2, 2, 5, 5, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 3, 0, 0 ],
			[ 0, 2, 3, 3, 3, 3, 0, 0 ],
			[ 0, 0, 2, 3, 2, 0, 0, 0 ],
			[ 0, 0, 6, 6, 6, 6, 0, 0 ],
		],
		// Frame 3 — back foot forward
		[
			[ 0, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 2, 2, 5, 5, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 3, 0, 0 ],
			[ 0, 2, 3, 2, 3, 2, 2, 0 ],
			[ 0, 3, 0, 0, 0, 3, 2, 0 ],
			[ 0, 6, 0, 0, 0, 6, 7, 0 ],
		],
	];

	public static readonly byte[][][]	Legs2Side =
	[
		// Frame 0 — idle
		[
			[ 0, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 2, 2, 5, 5, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 2, 0, 0 ],		// shorts
			[ 0, 1, 1, 1, 1, 1, 0, 0 ],		// bare skin
			[ 0, 1, 1, 1, 1, 1, 0, 0 ],
			[ 0, 6, 7, 6, 6, 6, 7, 0 ],
		],
		// Frame 1 — front foot forward
		[
			[ 0, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 2, 2, 5, 5, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 2, 0, 0 ],
			[ 1, 1, 1, 1, 1, 1, 1, 0 ],		// bare splay
			[ 1, 1, 0, 0, 0, 1, 1, 0 ],
			[ 6, 6, 0, 0, 0, 6, 6, 7 ],
		],
		// Frame 2 — crossing
		[
			[ 0, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 2, 2, 5, 5, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 2, 0, 0 ],
			[ 0, 1, 1, 2, 1, 1, 0, 0 ],
			[ 0, 0, 1, 1, 1, 0, 0, 0 ],
			[ 0, 0, 6, 6, 6, 6, 0, 0 ],
		],
		// Frame 3 — back foot forward
		[
			[ 0, 4, 4, 4, 4, 4, 0, 0 ],
			[ 0, 2, 2, 5, 5, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 2, 0, 0 ],
			[ 0, 1, 1, 1, 1, 1, 1, 0 ],
			[ 0, 1, 0, 0, 0, 1, 1, 0 ],
			[ 0, 6, 0, 0, 0, 6, 6, 7 ],
		],
	];
}
