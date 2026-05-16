namespace ChildhoodAdventure.RetroSystems.AppleII;

internal static partial class AppleIISprites
{
	public const int HeadRows =	4;

	public static HeadPalette[]	HeadPalettes { get; } =
		[
			new("fair/light-hair",
				Skin:		AppleIIPalette.White,	// white
				Hair:		AppleIIPalette.Orange,	// orange (blonde approximation)
				Highlight:	AppleIIPalette.White,	// white
				Eyes:		AppleIIPalette.Blue,	// blue
				Accessory:	AppleIIPalette.Orange),	// orange

			new("fair/dark-hair",
				Skin:		AppleIIPalette.White,	// white
				Hair:		AppleIIPalette.Black,	// black
				Highlight:	AppleIIPalette.White,	// white
				Eyes:		AppleIIPalette.Black,	// black
				Accessory:	AppleIIPalette.Violet),	// violet

			new("medium/black-hair",
				Skin:		AppleIIPalette.Orange,	// orange
				Hair:		AppleIIPalette.Black,	// black
				Highlight:	AppleIIPalette.White,	// white
				Eyes:		AppleIIPalette.Blue,	// blue
				Accessory:	AppleIIPalette.Black),	// black

			new("dark/black-hair",
				Skin:		AppleIIPalette.Violet,	// violet
				Hair:		AppleIIPalette.Black,	// black
				Highlight:	AppleIIPalette.Orange,	// orange
				Eyes:		AppleIIPalette.White,	// white (contrast)
				Accessory:	AppleIIPalette.Black),	// black

			new("medium/violet-hair",
				Skin:		AppleIIPalette.Orange,	// orange
				Hair:		AppleIIPalette.Violet,	// violet
				Highlight:	AppleIIPalette.White,	// white
				Eyes:		AppleIIPalette.Blue,	// blue
				Accessory:	AppleIIPalette.Violet),	// violet
		];

	public static readonly byte[][][]	Head0 =	// basic round head
	[
		[
			[ 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],	// hair crown
			[ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0 ],	// hair sides + face
			[ 0, 0, 1, 1, 4, 4, 1, 1, 4, 4, 1, 1, 0, 0 ],	// face + two eyes
			[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],	// chin
		],
	];

	public static readonly byte[][][]	Head1 =	// cap / hat
	[
		[
			[ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],	// hat top
			[ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],	// hat brim
			[ 0, 0, 1, 1, 4, 4, 1, 1, 4, 4, 1, 1, 0, 0 ],	// face + eyes
			[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],	// chin
		],
	];

	public static readonly byte[][][]	Head2 =	// long / full hair
	[
		[
			[ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],	// full hair top
			[ 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0 ],	// hair sides + face
			[ 0, 2, 2, 1, 4, 4, 1, 1, 4, 4, 1, 2, 2, 0 ],	// hair sides + eyes + face
			[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],	// chin
		],
	];

	public static readonly byte[][][]	Head0Back =
	[[
		[ 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],	// hair crown
		[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],	// hair back (no eyes)
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],	// upper neck
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],	// neck
	]];

	public static readonly byte[][][]	Head1Back =
	[[
		[ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],	// hat top (full brim from behind)
		[ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],	// hat brim
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],	// upper neck
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],	// neck
	]];

	public static readonly byte[][][]	Head2Back =
	[[
		[ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],	// full hair
		[ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],	// full hair back
		[ 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0 ],	// hair sides + neck
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],	// neck
	]];
}
