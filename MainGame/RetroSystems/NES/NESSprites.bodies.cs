namespace ChildhoodAdventure.RetroSystems.NES;

internal static partial class NESSprites
{
	public const int CharWidth	= 8;
	public const int BodyRows	= 6;

	public static BodyPalette[]	BodyPalettes { get; } =
		[
			new("green",
				Skin:			NESPalette.SkinFair,
				Shirt:			NESPalette.NesGreen,	// NES green
				ShirtHighlight:	NESPalette.Aqua,
				Buttons:		NESPalette.NearWhite,
				Accessory:		NESPalette.NesBlue),
			new("blue",
				Skin:			NESPalette.SkinFair,
				Shirt:			NESPalette.NesBlue,		// NES blue
				ShirtHighlight:	NESPalette.Aqua,
				Buttons:		NESPalette.WallGray,
				Accessory:		NESPalette.BrightYellow),
			new("red",
				Skin:			NESPalette.SkinFair,
				Shirt:			NESPalette.DarkRed,		// NES dark red
				ShirtHighlight:	NESPalette.VividRed,
				Buttons:		NESPalette.NearWhite,
				Accessory:		NESPalette.BrightYellow),
			new("white/light",
				Skin:			NESPalette.SkinFair,
				Shirt:			NESPalette.WallGray,
				ShirtHighlight:	NESPalette.NearWhite,
				Buttons:		NESPalette.NesBlue,
				Accessory:		NESPalette.NesGreen),
			new("teal",
				Skin:			NESPalette.SkinFair,
				Shirt:			NESPalette.Aqua,	// NES aqua
				ShirtHighlight:	NESPalette.PaleCyan,
				Buttons:		NESPalette.NearWhite,
				Accessory:		NESPalette.MidMagenta),
		];

	public static readonly byte[][][]	Body0 =	// casual shirt
	[
		[
			[ 0, 0, 1, 1, 1, 1, 0, 0 ],		// neck skin
			[ 0, 2, 2, 2, 2, 2, 2, 0 ],		// shoulders
			[ 2, 2, 3, 4, 4, 3, 2, 2 ],		// upper chest + buttons + side highlights
			[ 2, 2, 2, 2, 2, 2, 2, 2 ],		// chest
			[ 2, 3, 2, 4, 4, 2, 3, 2 ],		// lower buttons
			[ 2, 2, 2, 2, 2, 2, 2, 2 ],		// lower torso
		],
	];

	public static readonly byte[][][]	Body1 =	// collared / formal
	[
		[
			[ 0, 0, 1, 1, 1, 1, 0, 0 ],		// neck skin
			[ 0, 2, 5, 2, 2, 5, 2, 0 ],		// lapels
			[ 2, 2, 5, 2, 2, 5, 2, 2 ],		// lapels chest
			[ 2, 2, 2, 4, 4, 2, 2, 2 ],		// center buttons
			[ 2, 3, 2, 2, 2, 2, 3, 2 ],		// side highlights
			[ 2, 2, 2, 4, 4, 2, 2, 2 ],		// more buttons
		],
	];

	public static readonly byte[][][]	Body2 =	// jacket / hoodie
	[
		[
			[ 0, 0, 1, 1, 1, 1, 0, 0 ],		// neck skin
			[ 0, 5, 5, 2, 2, 5, 5, 0 ],		// jacket outer + open front
			[ 5, 5, 2, 2, 2, 2, 5, 5 ],		// open front
			[ 5, 5, 2, 4, 4, 2, 5, 5 ],		// buttons
			[ 5, 5, 2, 2, 2, 2, 5, 5 ],		// jacket sides
			[ 5, 5, 2, 2, 2, 2, 5, 5 ],		// jacket bottom
		],
	];

	public static readonly byte[][][]	Body0Back =
	[
		[
			[ 0, 0, 1, 1, 1, 1, 0, 0 ],		// neck
			[ 0, 2, 2, 2, 2, 2, 2, 0 ],		// shoulders
			[ 2, 2, 3, 2, 2, 3, 2, 2 ],		// upper back highlights (no buttons)
			[ 2, 2, 2, 2, 2, 2, 2, 2 ],
			[ 2, 3, 2, 2, 2, 2, 3, 2 ],		// lower back highlights
			[ 2, 2, 2, 2, 2, 2, 2, 2 ],
		],
	];

	public static readonly byte[][][]	Body1Back =
	[
		[
			[ 0, 0, 1, 1, 1, 1, 0, 0 ],		// neck
			[ 0, 2, 5, 2, 2, 5, 2, 0 ],		// collar from behind
			[ 2, 2, 5, 2, 2, 5, 2, 2 ],		// collar
			[ 2, 2, 2, 2, 2, 2, 2, 2 ],		// shirt back (no buttons)
			[ 2, 3, 2, 2, 2, 2, 3, 2 ],
			[ 2, 2, 2, 2, 2, 2, 2, 2 ],
		],
	];

	public static readonly byte[][][]	Body2Back =
	[
		[
			[ 0, 0, 1, 1, 1, 1, 0, 0 ],		// neck
			[ 0, 5, 5, 5, 5, 5, 5, 0 ],		// jacket back
			[ 5, 5, 5, 5, 5, 5, 5, 5 ],
			[ 5, 5, 5, 5, 5, 5, 5, 5 ],
			[ 5, 5, 5, 5, 5, 5, 5, 5 ],
			[ 5, 5, 5, 5, 5, 5, 5, 5 ],
		],
	];

	public static readonly byte[][][]	Body0Side =
	[
		[
			[ 0, 0, 1, 1, 1, 0, 0, 0 ],		// neck (narrower)
			[ 0, 2, 2, 2, 2, 2, 0, 0 ],		// shoulder
			[ 0, 2, 3, 2, 2, 2, 0, 0 ],		// upper torso + highlight
			[ 0, 2, 2, 2, 2, 2, 0, 0 ],
			[ 0, 2, 3, 2, 2, 2, 0, 0 ],
			[ 0, 2, 2, 2, 2, 2, 0, 0 ],
		],
	];

	public static readonly byte[][][]	Body1Side =
	[
		[
			[ 0, 0, 1, 1, 1, 0, 0, 0 ],		// neck
			[ 0, 2, 5, 2, 5, 2, 0, 0 ],		// lapel side
			[ 0, 2, 5, 2, 5, 2, 0, 0 ],		// lapel
			[ 0, 2, 2, 4, 2, 2, 0, 0 ],		// button
			[ 0, 2, 3, 2, 2, 2, 0, 0 ],
			[ 0, 2, 2, 4, 2, 2, 0, 0 ],
		],
	];

	public static readonly byte[][][]	Body2Side =
	[
		[
			[ 0, 0, 1, 1, 1, 0, 0, 0 ],		// neck
			[ 0, 5, 5, 2, 5, 5, 0, 0 ],		// jacket side + open front
			[ 0, 5, 2, 2, 2, 5, 0, 0 ],
			[ 0, 5, 2, 4, 2, 5, 0, 0 ],		// button
			[ 0, 5, 2, 2, 2, 5, 0, 0 ],
			[ 0, 5, 5, 2, 2, 5, 0, 0 ],
		],
	];
}
