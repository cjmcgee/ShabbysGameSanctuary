namespace ChildhoodAdventure.RetroSystems.NES;

public static partial class NESSprites
{
	public const int HeadRows =	6;

	public static HeadPalette[]	HeadPalettes { get; } =
		[
			new("fair/blonde",
				Skin:		NESPalette.SkinFair,
				Hair:		NESPalette.BrightYellow,		// NES warm yellow
				Highlight:	NESPalette.SkinPale,
				Eyes:		NESPalette.NesBlue,		// NES blue
				Accessory:	NESPalette.BrightYellow),
			new("fair/brown",
				Skin:		NESPalette.SkinFair,
				Hair:		NESPalette.WarmDoorWood,		// NES wood brown
				Highlight:	NESPalette.SkinPale,
				Eyes:		NESPalette.DoorBlack,
				Accessory:	NESPalette.WarmDoorWood),
			new("medium/black",
				Skin:		NESPalette.SkinTan,
				Hair:		NESPalette.DoorBlack,
				Highlight:	NESPalette.SkinLight,
				Eyes:		NESPalette.DoorBlack,
				Accessory:	NESPalette.DoorBlack),
			new("dark/black",
				Skin:		NESPalette.SkinDark,
				Hair:		NESPalette.DoorBlack,
				Highlight:	NESPalette.SkinMedium,
				Eyes:		NESPalette.Aqua,		// NES aqua
				Accessory:	NESPalette.DoorBlack),
			new("medium/auburn",
				Skin:		NESPalette.SkinTan,
				Hair:		NESPalette.DarkRed,		// NES dark red
				Highlight:	NESPalette.SkinLight,
				Eyes:		NESPalette.NesBlue,
				Accessory:	NESPalette.DarkRed),
		];

	public static readonly byte[][][]	Head0 =	// basic round head
	[
		[
			[ 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],		// hair top
			[ 0, 0, 0, 0, 2, 1, 1, 1, 1, 1, 2, 0, 0, 0, 0, 0 ],		// hair + skin sides
			[ 0, 0, 0, 0, 1, 4, 1, 1, 4, 1, 3, 0, 0, 0, 0, 0 ],		// eyes + highlight
			[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],		// lower face
			[ 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],		// chin
			[ 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],		// neck
		],
	];

	public static readonly byte[][][]	Head1 =	// cap / hat
	[
		[
			[ 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0 ],		// hat top
			[ 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],		// hat brim (wide)
			[ 0, 0, 0, 0, 1, 4, 1, 1, 4, 1, 3, 0, 0, 0, 0, 0 ],		// eyes + highlight
			[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],		// lower face
			[ 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],		// chin
			[ 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],		// neck
		],
	];

	public static readonly byte[][][]	Head2 =	// long / full hair
	[
		[
			[ 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0 ],		// full hair top
			[ 0, 0, 0, 2, 1, 1, 1, 1, 1, 1, 2, 0, 0, 0, 0, 0 ],		// hair framing
			[ 0, 0, 0, 2, 1, 4, 1, 1, 4, 3, 2, 0, 0, 0, 0, 0 ],		// eyes + highlight
			[ 0, 0, 0, 2, 1, 1, 1, 1, 1, 1, 2, 0, 0, 0, 0, 0 ],		// lower face
			[ 0, 0, 0, 2, 2, 1, 1, 1, 1, 2, 2, 0, 0, 0, 0, 0 ],		// chin + hair falling
			[ 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],		// neck
		],
	];

	public static readonly byte[][][]	Head0Back =
	[
		[
			[ 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],		// hair top
			[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0 ],		// hair wider
			[ 0, 0, 0, 0, 2, 1, 1, 1, 1, 1, 2, 0, 0, 0, 0, 0 ],		// hair sides + neck
			[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],		// upper neck
			[ 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],		// neck
			[ 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],		// neck base
		],
	];

	public static readonly byte[][][]	Head1Back =
	[
		[
			[ 0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0 ],		// hat crown
			[ 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],		// hat brim
			[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],		// back of head
			[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],		// upper neck
			[ 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],		// neck
			[ 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],		// neck base
		],
	];

	public static readonly byte[][][]	Head2Back =
	[
		[
			[ 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0 ],		// hair wide
			[ 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0 ],		// hair
			[ 0, 0, 0, 2, 2, 1, 1, 1, 1, 2, 2, 0, 0, 0, 0, 0 ],		// hair sides + neck
			[ 0, 0, 0, 2, 2, 1, 1, 1, 1, 2, 2, 0, 0, 0, 0, 0 ],		// hair sides
			[ 0, 0, 0, 2, 2, 2, 1, 1, 2, 2, 2, 0, 0, 0, 0, 0 ],		// long hair falling
			[ 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],		// neck base
		],
	];

	public static readonly byte[][][]	Head0Side =
	[
		[
			[ 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// hair top
			[ 0, 0, 2, 1, 1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// hair left + face
			[ 0, 0, 2, 1, 1, 4, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// hair + eye + highlight
			[ 0, 0, 2, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// hair + lower face
			[ 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// chin
			[ 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// neck
		],
	];

	public static readonly byte[][][]	Head1Side =
	[
		[
			[ 0, 0, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// hat crown
			[ 0, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0 ],		// hat brim
			[ 0, 0, 1, 1, 1, 4, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// face + eye + highlight
			[ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// lower face
			[ 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// chin
			[ 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// neck
		],
	];

	public static readonly byte[][][]	Head2Side =
	[
		[
			[ 0, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// hair top
			[ 0, 2, 2, 1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// hair + face
			[ 0, 2, 2, 1, 4, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// hair + eye + highlight
			[ 0, 2, 2, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// hair + face
			[ 0, 2, 2, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// hair + chin
			[ 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],		// neck
		],
	];
}
