namespace ChildhoodAdventure.RetroSystems.Assets;

/// <summary>
/// Convention for mapping RGB colors in a sprite PNG to the semantic indices
/// used by <see cref="RetroSystem"/>'s palette-based character assembly.
///
/// Sprite art is authored as an indexed-style PNG where each of seven primary
/// hues represents one semantic slot. The loader converts those magic colors
/// into byte indices so the existing <see cref="HeadPalette"/> /
/// <see cref="BodyPalette"/> / <see cref="LegsPalette"/> system still controls
/// the per-character recolouring.
///
/// Conventions:
///   alpha 0           → 0  (transparent)
///   (255,   0,   0)   → 1  Skin
///   (  0, 255,   0)   → 2  Hair / Shirt / Pants
///   (  0,   0, 255)   → 3  Highlight
///   (255, 255,   0)   → 4  Eyes / Buttons / Belt
///   (255,   0, 255)   → 5  Accessory / BeltHighlight
///   (  0, 255, 255)   → 6  Shoes
///   (255, 255, 255)   → 7  ShoeHighlight
///
/// Pixels that match none of these (including off-by-a-bit shades) are treated
/// as transparent. The asset seeder uses exactly these RGB values, so the
/// round trip is lossless.
/// </summary>
internal static class SemanticPalette
{
	public static readonly Color SlotSkin      = new(255,   0,   0);
	public static readonly Color SlotHair      = new(  0, 255,   0);
	public static readonly Color SlotHighlight = new(  0,   0, 255);
	public static readonly Color SlotEyes      = new(255, 255,   0);
	public static readonly Color SlotAccessory = new(255,   0, 255);
	public static readonly Color SlotShoes     = new(  0, 255, 255);
	public static readonly Color SlotShoeHi    = new(255, 255, 255);

	public static byte ToSemanticIndex(Color c)
	{
		if (c.A == 0) return 0;
		if (c.R == 255 && c.G ==   0 && c.B ==   0) return 1;
		if (c.R ==   0 && c.G == 255 && c.B ==   0) return 2;
		if (c.R ==   0 && c.G ==   0 && c.B == 255) return 3;
		if (c.R == 255 && c.G == 255 && c.B ==   0) return 4;
		if (c.R == 255 && c.G ==   0 && c.B == 255) return 5;
		if (c.R ==   0 && c.G == 255 && c.B == 255) return 6;
		if (c.R == 255 && c.G == 255 && c.B == 255) return 7;
		return 0;
	}
}

/// <summary>
/// Sentinel colors used in TILE sheets. Tile art is direct-color RGBA, but a
/// few cells need runtime substitution: the AccentMarker color becomes the
/// per-house accent at <c>BuildTileset</c> time.
/// </summary>
internal static class TileSentinels
{
	/// <summary>
	/// Pure magenta. Every pixel matching this RGB in a tile PNG is replaced
	/// with the per-build accent color. Chosen because it doesn't occur in
	/// natural-looking modern pixel art and is easy to spot in image editors.
	/// </summary>
	public static readonly Color AccentMarker = new(255, 0, 255);
}
