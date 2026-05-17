using ChildhoodAdventure.RetroSystems.Assets;

namespace ChildhoodAdventure.RetroSystems.WinXP;

/// <summary>
/// Character sprite data for the WinXP system. The pixel art lives in PNG
/// sheets that <see cref="AssetLoader"/> slices into byte semantic-index
/// arrays at first-use; this class then exposes those arrays plus the
/// head/body/legs palettes used to recolor characters at draw time.
///
/// Palettes use 24-bit RGB colors (no 256-color limit) since the system is
/// designed for modern direct-color rendering.
/// </summary>
internal sealed class WinXPSprites
{
	public byte[][][][] HeadParts { get; }
	public byte[][][][] HeadPartsBack { get; }
	public byte[][][][] HeadPartsSide { get; }
	public byte[][][][] BodyParts { get; }
	public byte[][][][] BodyPartsBack { get; }
	public byte[][][][] BodyPartsSide { get; }
	public byte[][][][] LegsParts { get; }
	public byte[][][][] LegsPartsSide { get; }

	public int CharWidth { get; }
	public int HeadRows { get; }
	public int BodyRows { get; }
	public int LegsRows { get; }

	public WinXPSprites(GraphicsDevice gd, string assetDir)
	{
		var heads = AssetLoader.LoadSpriteSheet(gd, Path.Combine(assetDir, "WinXPHeads.json"));
		var bodies = AssetLoader.LoadSpriteSheet(gd, Path.Combine(assetDir, "WinXPBodies.json"));
		var legs = AssetLoader.LoadSpriteSheet(gd, Path.Combine(assetDir, "WinXPLegs.json"));

		HeadParts     = heads.Front;
		HeadPartsBack = heads.Back ?? heads.Front;
		HeadPartsSide = heads.Side ?? heads.Front;
		BodyParts     = bodies.Front;
		BodyPartsBack = bodies.Back ?? bodies.Front;
		BodyPartsSide = bodies.Side ?? bodies.Front;
		LegsParts     = legs.Front;
		// No LegsPartsBack override — the base class default reuses LegsParts.
		LegsPartsSide = legs.Side ?? legs.Front;

		CharWidth = heads.PartWidth;
		HeadRows  = heads.PartHeight;
		BodyRows  = bodies.PartHeight;
		LegsRows  = legs.PartHeight;
	}

	// ── Per-character palettes ──────────────────────────────────────────
	// The 24-bit color choices skew "modern pixel art" (Stardew/Celeste
	// adjacent) rather than emulating a specific 8/16-bit machine.

	public static HeadPalette[] HeadPalettes { get; } =
	[
		new("fair/blonde",
			Skin:		new(238, 198, 162),
			Hair:		new(232, 200,  95),
			Highlight:	new(252, 225, 192),
			Eyes:		new( 65, 110, 175),
			Accessory:	new(220,  90,  85)),
		new("fair/brown",
			Skin:		new(238, 198, 162),
			Hair:		new(110,  70,  40),
			Highlight:	new(252, 225, 192),
			Eyes:		new( 75,  55,  30),
			Accessory:	new( 95, 150, 200)),
		new("medium/black",
			Skin:		new(190, 140,  95),
			Hair:		new( 40,  30,  30),
			Highlight:	new(220, 175, 130),
			Eyes:		new( 40,  30,  30),
			Accessory:	new( 90, 170, 110)),
		new("dark/black",
			Skin:		new(115,  75,  50),
			Hair:		new( 30,  20,  20),
			Highlight:	new(150, 100,  70),
			Eyes:		new(110, 170, 200),
			Accessory:	new(230, 200,  95)),
		new("medium/auburn",
			Skin:		new(190, 140,  95),
			Hair:		new(150,  60,  35),
			Highlight:	new(220, 175, 130),
			Eyes:		new( 65, 110, 175),
			Accessory:	new(200,  85, 105)),
	];

	public static BodyPalette[] BodyPalettes { get; } =
	[
		new("forest green",
			Skin:			new(238, 198, 162),
			Shirt:			new( 55, 135,  85),
			ShirtHighlight:	new(105, 180, 130),
			Buttons:		new(240, 235, 220),
			Accessory:		new( 35,  95,  60)),
		new("ocean blue",
			Skin:			new(238, 198, 162),
			Shirt:			new( 55, 105, 175),
			ShirtHighlight:	new(115, 170, 220),
			Buttons:		new(220, 220, 220),
			Accessory:		new(230, 200,  95)),
		new("crimson",
			Skin:			new(238, 198, 162),
			Shirt:			new(180,  55,  60),
			ShirtHighlight:	new(225, 110, 105),
			Buttons:		new(240, 235, 220),
			Accessory:		new(120,  35,  40)),
		new("slate",
			Skin:			new(238, 198, 162),
			Shirt:			new(160, 165, 175),
			ShirtHighlight:	new(210, 215, 225),
			Buttons:		new( 90, 100, 115),
			Accessory:		new( 70,  85, 100)),
		new("plum",
			Skin:			new(238, 198, 162),
			Shirt:			new(120,  65, 145),
			ShirtHighlight:	new(175, 115, 200),
			Buttons:		new(240, 235, 220),
			Accessory:		new( 85,  35, 110)),
	];

	public static LegsPalette[] LegPalettes { get; } =
	[
		new("blue jeans/brown shoes",
			Skin:			new(238, 198, 162),
			Pants:			new( 60,  90, 160),
			PantsHighlight:	new(115, 150, 215),
			Belt:			new( 75,  50,  30),
			BeltHighlight:	new(170, 130,  85),
			Shoes:			new( 60,  40,  25),
			ShoeHighlight:	new(120,  85,  55)),
		new("black/black",
			Skin:			new(238, 198, 162),
			Pants:			new( 40,  40,  45),
			PantsHighlight:	new( 90,  90,  95),
			Belt:			new( 30,  30,  35),
			BeltHighlight:	new( 70,  70,  75),
			Shoes:			new( 25,  25,  30),
			ShoeHighlight:	new( 75,  75,  80)),
		new("khaki/tan",
			Skin:			new(238, 198, 162),
			Pants:			new(190, 165, 110),
			PantsHighlight:	new(225, 205, 155),
			Belt:			new(115,  75,  35),
			BeltHighlight:	new(165, 115,  60),
			Shoes:			new(110,  70,  35),
			ShoeHighlight:	new(160, 115,  70)),
		new("charcoal/dark",
			Skin:			new(238, 198, 162),
			Pants:			new( 75,  80,  90),
			PantsHighlight:	new(125, 130, 145),
			Belt:			new( 55,  60,  70),
			BeltHighlight:	new(100, 105, 115),
			Shoes:			new( 50,  50,  60),
			ShoeHighlight:	new( 95,  95, 105)),
	];
}
