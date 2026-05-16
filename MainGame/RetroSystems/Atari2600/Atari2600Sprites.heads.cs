namespace ChildhoodAdventure.RetroSystems.Atari2600;

internal static partial class Atari2600Sprites
{
	public const int HeadRows =	5;

	// ── Head palettes (5) — all colours sourced from Atari2600Palette ────────
	// Highlight slots that requested pure white are snapped to NearWhite, the
	// brightest authentic Atari NTSC entry.
	public static HeadPalette[]	HeadPalettes { get; } =
	[
		new("Fair/Blonde",
			Skin:		Atari2600Palette.NearWhite,
			Hair:		Atari2600Palette.BrightYellow,
			Highlight:	Atari2600Palette.NearWhite,
			Eyes:		Atari2600Palette.DarkSlate,
			Accessory:	Atari2600Palette.WarmAmber),

		new("Fair/Brown",
			Skin:		Atari2600Palette.NearWhite,
			Hair:		Atari2600Palette.DarkBrown,
			Highlight:	Atari2600Palette.NearWhite,
			Eyes:		Atari2600Palette.DarkSlate,
			Accessory:	Atari2600Palette.DarkBrown),

		new("Medium/Black",
			Skin:		Atari2600Palette.WarmAmber,
			Hair:		Atari2600Palette.NearBlack,
			Highlight:	Atari2600Palette.NearWhite,
			Eyes:		Atari2600Palette.NearBlack,
			Accessory:	Atari2600Palette.MediumGray),

		new("Dark/Black",
			Skin:		Atari2600Palette.DarkRed,
			Hair:		Atari2600Palette.NearBlack,
			Highlight:	Atari2600Palette.WarmAmber,
			Eyes:		Atari2600Palette.DarkSlate,
			Accessory:	Atari2600Palette.MediumGray),

		new("Medium/Auburn",
			Skin:		Atari2600Palette.WarmAmber,
			Hair:		Atari2600Palette.VividRed,
			Highlight:	Atari2600Palette.NearWhite,
			Eyes:		Atari2600Palette.DarkSlate,
			Accessory:	Atari2600Palette.MediumGray),
	];

	// ── Head parts (16 wide × 5 rows, 1 frame) ───────────────────────────────
	// Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory
	// Rule: each row uses at most ONE non-zero semantic index.
	// Double-wide: col[2k+1] == col[2k] for all k.
	public static readonly byte[][][]	Head0 =
	[[
		[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// hair
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// upper face
		[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// eyes
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// chin
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
	]];

	public static readonly byte[][][]	Head1 =
	[[
		[ 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],		// hat crown
		[ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],		// hat brim
		[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// eyes
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// face
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
	]];

	public static readonly byte[][][]	Head2 =
	[[
		[ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],		// hair top
		[ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],		// hair wide
		[ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],		// eyes
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// face/chin
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
	]];

	// ── Back-facing heads (no eye row, extra hair) ────────────────────────────
	public static readonly byte[][][]	Head0Back =
	[[
		[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// hair
		[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// hair (back)
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// upper neck
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck base
	]];

	public static readonly byte[][][]	Head1Back =
	[[
		[ 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],		// hat crown
		[ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],		// hat brim
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// upper neck
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck base
	]];

	public static readonly byte[][][]	Head2Back =
	[[
		[ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],		// full hair
		[ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],		// full hair
		[ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],		// long hair continues
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck base
	]];
}
