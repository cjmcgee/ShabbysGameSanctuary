namespace ChildhoodAdventure.RetroSystems.Atari2600;

public static partial class Atari2600Sprites
{
	public const int LegsRows =	4;

	// ── Legs palettes (4) ────────────────────────────────────────────────────
	public static LegsPalette[]	LegPalettes { get; } =
	[
		new("Blue Jeans/Brown",
			Skin:			Atari2600Palette.NearWhite,
			Pants:			Atari2600Palette.BoldBlue,
			PantsHighlight:	Atari2600Palette.BrightCyan,
			Belt:			Atari2600Palette.WarmAmber,
			BeltHighlight:	Atari2600Palette.BrightYellow,
			Shoes:			Atari2600Palette.DarkBrown,
			ShoeHighlight:	Atari2600Palette.WarmAmber),

		new("Black/Black",
			Skin:			Atari2600Palette.NearWhite,
			Pants:			Atari2600Palette.NearBlack,
			PantsHighlight:	Atari2600Palette.MediumGray,
			Belt:			Atari2600Palette.MediumGray,
			BeltHighlight:	Atari2600Palette.LightGray,
			Shoes:			Atari2600Palette.NearBlack,
			ShoeHighlight:	Atari2600Palette.MediumGray),

		new("Khaki/Tan",
			Skin:			Atari2600Palette.NearWhite,
			Pants:			Atari2600Palette.BrightYellow,
			PantsHighlight:	Atari2600Palette.NearWhite,
			Belt:			Atari2600Palette.DarkBrown,
			BeltHighlight:	Atari2600Palette.BrightYellow,
			Shoes:			Atari2600Palette.DarkBrown,
			ShoeHighlight:	Atari2600Palette.WarmAmber),

		new("Gray/Dark",
			Skin:			Atari2600Palette.NearWhite,
			Pants:			Atari2600Palette.LightGray,
			PantsHighlight:	Atari2600Palette.NearWhite,
			Belt:			Atari2600Palette.MediumGray,
			BeltHighlight:	Atari2600Palette.LightGray,
			Shoes:			Atari2600Palette.MediumGray,
			ShoeHighlight:	Atari2600Palette.NearBlack),
	];

	// ── Legs parts (16 wide × 4 rows, 4 frames) ──────────────────────────────
	// Idle: legs merged at centre; walk: legs spread to outer logical pixels.
	// Rule: each row uses at most ONE non-zero semantic index.
	// Double-wide: col[2k+1] == col[2k] for all k.
	public static readonly byte[][][]	Legs0 =
	[
		[   // idle
			[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// pants
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// pants
			[ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoes
		],
		[   // walk A — left foot forward
			[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// left leg out
			[ 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0 ],		// legs spread
			[ 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoes spread
		],
		[   // walk B — right foot forward
			[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// pants
			[ 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0 ],
		],
	];

	public static readonly byte[][][]	Legs1 =
	[
		[   // idle
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoes
		],
		[   // walk A
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// left leg out
			[ 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],		// legs spread
			[ 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0 ],		// shoes
		],
		[   // walk B
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// right leg out
			[ 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0 ],		// legs spread
			[ 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoes
		],
	];

	public static readonly byte[][][]	Legs2 =
	[
		[   // idle
			[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// shorts
			[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// bare legs
			[ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0 ],		// shoes
		],
		[   // walk A
			[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// left shorts out
			[ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 ],		// bare legs spread
			[ 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0 ],		// shoes
		],
		[   // walk B
			[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// belt
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// right shorts out
			[ 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 ],		// bare legs spread
			[ 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 0, 0, 0, 0 ],		// shoes
		],
	];

	// ── Side-facing legs (profile walk cycle, 4 frames) ──────────────────────
	// Toe extends right; front foot swings right, back foot swings left.
	// One-color-per-scanline rule still applies.
	public static readonly byte[][][]	Legs0Side =
	[
		[   // idle — feet together, toe extends right
			[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
		],
		[   // walk A — front foot forward (right)
			[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 2, 2, 2, 2, 2, 2, 2, 2, 6, 6, 6, 6, 6, 6, 6, 6 ],
			[ 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0 ],
		],
		[   // walk B — back foot forward (left)
			[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 6, 6, 6, 6, 6, 6, 6, 6, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6 ],
		],
	];

	public static readonly byte[][][]	Legs1Side =
	[
		[   // idle
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
		],
		[   // walk A
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 2, 2, 2, 2, 2, 2, 2, 2, 6, 6, 6, 6, 6, 6, 6, 6 ],
			[ 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0 ],
		],
		[   // walk B
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 6, 6, 6, 6, 6, 6, 6, 6, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6 ],
		],
	];

	public static readonly byte[][][]	Legs2Side =
	[
		[   // idle
			[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
			[ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
		],
		[   // walk A
			[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
			[ 1, 1, 1, 1, 1, 1, 1, 1, 6, 6, 6, 6, 6, 6, 6, 6 ],
			[ 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0 ],
		],
		[   // walk B
			[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
			[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
			[ 6, 6, 6, 6, 6, 6, 6, 6, 1, 1, 1, 1, 1, 1, 0, 0 ],
			[ 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6 ],
		],
	];
}
