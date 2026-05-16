namespace ChildhoodAdventure.RetroSystems.Atari2600;

/// <summary>
/// Atari 2600 has "double wide" pixels and a 160×192 screen resolution. 
/// Sprites are 8 pixels wide (double wide pixels, so 16 of our square pixels).
/// Sprite have no inherit height; instead they are defined per scan line and 
/// are limited to two per scan line. 
/// Sprites are limited to 1 color per line (without hacks). 
///
/// Here we will try to keep things very simple by limiting the sprites to 
/// one color per line (in addition to the transparent color). And we will 
/// try to keep them simple and chunky. 
/// </summary>
internal static partial class Atari2600Sprites
{
	public const int CharWidth =	16;
	public const int BodyRows =	7;

	// ── Body palettes (5) ────────────────────────────────────────────────────
	public static BodyPalette[]	BodyPalettes { get; } =
	[
		new("Green",
			Skin:			Atari2600Palette.NearWhite,
			Shirt:			Atari2600Palette.VividGreen,
			ShirtHighlight:	Atari2600Palette.BrightCyan,
			Buttons:		Atari2600Palette.NearBlack,
			Accessory:		Atari2600Palette.DarkGreen),

		new("Blue",
			Skin:			Atari2600Palette.NearWhite,
			Shirt:			Atari2600Palette.BoldBlue,
			ShirtHighlight:	Atari2600Palette.BrightCyan,
			Buttons:		Atari2600Palette.NearBlack,
			Accessory:		Atari2600Palette.NearBlack),

		new("Red",
			Skin:			Atari2600Palette.NearWhite,
			Shirt:			Atari2600Palette.VividRed,
			ShirtHighlight:	Atari2600Palette.NearWhite,
			Buttons:		Atari2600Palette.NearBlack,
			Accessory:		Atari2600Palette.DarkRed),

		new("White",
			Skin:			Atari2600Palette.NearWhite,
			Shirt:			Atari2600Palette.NearWhite,
			ShirtHighlight:	Atari2600Palette.NearWhite,
			Buttons:		Atari2600Palette.MediumGray,
			Accessory:		Atari2600Palette.LightGray),

		new("Teal",
			Skin:			Atari2600Palette.NearWhite,
			Shirt:			Atari2600Palette.BrightCyan,
			ShirtHighlight:	Atari2600Palette.NearWhite,
			Buttons:		Atari2600Palette.NearBlack,
			Accessory:		Atari2600Palette.DarkTeal),
	];


	// ── Body parts (16 wide × 7 rows, 1 frame) ───────────────────────────────
	// Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory
	// Rule: each row uses at most ONE non-zero semantic index.
	// Double-wide: col[2k+1] == col[2k] for all k.
	public static readonly byte[][][]	Body0 =
	[[
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
		[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shirt
		[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shirt
		[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],		// buttons
		[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shirt
		[ 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 0 ],		// full-row highlight
		[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// bottom narrow
	]];

	public static readonly byte[][][]	Body1 =
	[[
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
		[ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],		// lapels / collar
		[ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],		// shirt
		[ 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ],		// buttons
		[ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],		// shirt
		[ 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 ],		// full-row highlight
		[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// bottom narrow
	]];

	public static readonly byte[][][]	Body2 =
	[[
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
		[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket
		[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket
		[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shirt centre visible
		[ 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0 ],		// buttons
		[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket
		[ 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],		// jacket bottom narrow
	]];

	// ── Back-facing bodies (no button row) ───────────────────────────────────
	public static readonly byte[][][]	Body0Back =
	[[
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
		[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shirt back
		[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shirt
		[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shirt (no buttons)
		[ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],		// shirt
		[ 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 0 ],		// highlight
		[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// bottom narrow
	]];

	public static readonly byte[][][]	Body1Back =
	[[
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
		[ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],		// collar back
		[ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],		// shirt
		[ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],		// shirt
		[ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],		// shirt
		[ 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 ],		// highlight
		[ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],		// bottom narrow
	]];

	public static readonly byte[][][]	Body2Back =
	[[
		[ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],		// neck
		[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket back
		[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket
		[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket
		[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket
		[ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],		// jacket
		[ 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],		// bottom narrow
	]];
}
