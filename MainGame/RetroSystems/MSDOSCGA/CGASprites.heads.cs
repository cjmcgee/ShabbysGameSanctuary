namespace ChildhoodAdventure.RetroSystems.MSDOSCGA;

public static partial class CGASprites
{
	public const int HeadRows =	4;

	public static HeadPalette[]	HeadPalettes { get; } =
		[
			new("fair/blonde",
				Skin:		CGAPalette.White,			// white
				Hair:		CGAPalette.BrightMagenta,	// magenta (closest to blonde)
				Highlight:	CGAPalette.White,			// white
				Eyes:		CGAPalette.BrightCyan,		// cyan
				Accessory:	CGAPalette.BrightMagenta),	// magenta

			new("fair/dark-hair",
				Skin:		CGAPalette.White,			// white
				Hair:		CGAPalette.Black,			// black
				Highlight:	CGAPalette.White,			// white
				Eyes:		CGAPalette.Black,			// black
				Accessory:	CGAPalette.BrightCyan),		// cyan

			new("medium/black",
				Skin:		CGAPalette.BrightCyan,		// cyan
				Hair:		CGAPalette.Black,			// black
				Highlight:	CGAPalette.White,			// white
				Eyes:		CGAPalette.Black,			// black
				Accessory:	CGAPalette.Black),			// black

			new("dark/black",
				Skin:		CGAPalette.BrightMagenta,	// magenta
				Hair:		CGAPalette.Black,			// black
				Highlight:	CGAPalette.BrightCyan,		// cyan
				Eyes:		CGAPalette.White,			// white (contrast)
				Accessory:	CGAPalette.Black),			// black

			new("medium/magenta-hair",
				Skin:		CGAPalette.BrightCyan,		// cyan
				Hair:		CGAPalette.BrightMagenta,	// magenta
				Highlight:	CGAPalette.White,			// white
				Eyes:		CGAPalette.Black,			// black
				Accessory:	CGAPalette.BrightMagenta),	// magenta
		];

	public static readonly byte[][][]	Head0 =
	[[
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
		[ 0, 0, 1, 1, 1, 1, 4, 4, 1, 1, 4, 4, 1, 1, 0, 0 ],
		[ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
		[ 0, 0, 0, 0, 1, 1, 2, 2, 2, 2, 1, 1, 0, 0, 0, 0 ],
	]];

	public static readonly byte[][][]	Head1 =
	[[
		[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],
		[ 0, 0, 5, 5, 1, 1, 1, 1, 1, 1, 1, 1, 5, 5, 0, 0 ],
		[ 0, 0, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 0, 0 ],
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
	]];

	public static readonly byte[][][]	Head2 =
	[[
		[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
		[ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0 ],
		[ 0, 0, 2, 2, 4, 4, 1, 1, 1, 1, 4, 4, 2, 2, 0, 0 ],
		[ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0 ],
	]];

	public static readonly byte[][][]	Head0Back =
	[[
		[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// hair
		[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// hair (no eyes)
		[ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],		// neck
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck base
	]];

	public static readonly byte[][][]	Head1Back =
	[[
		[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// hat crown
		[ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],		// hat brim
		[ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],		// neck
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck base
	]];

	public static readonly byte[][][]	Head2Back =
	[[
		[ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// full hair
		[ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// hair (longer from back)
		[ 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0 ],		// hair sides + neck
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
	]];

	public static readonly byte[][][]	Head0Side =
	[[
		[ 0, 0, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// hair left + face
		[ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 4, 4, 0, 0, 0, 0 ],		// hair + face + eye right
		[ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// hair + lower face
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],		// chin (narrower)
	]];

	public static readonly byte[][][]	Head1Side =
	[[
		[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],		// hat
		[ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// hat brim
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 4, 4, 0, 0, 0, 0 ],		// face + eye
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],		// chin
	]];

	public static readonly byte[][][]	Head2Side =
	[[
		[ 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// hair left + face
		[ 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 4, 4, 0, 0, 0, 0 ],		// hair + face + eye
		[ 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// hair + lower face
		[ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],		// hair tail + chin
	]];
}
