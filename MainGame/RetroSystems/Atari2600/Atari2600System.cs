using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.Atari2600;

/// <summary>
/// Atari 2600 visual style.
///
/// Authentic characteristics:
///   • 16-px tile art (NativeTilePixels=16); hardware Atari 2600 tiles are 8×8 in spirit
///   • 1-bit tile color: each tile uses only black (bg) and one foreground color
///   • Sprites limited to one color per horizontal scanline (TIA player register style)
///   • 128-color NTSC palette; sprites are hard-edged silhouettes
///   • Character sprites: 16×16 texture (CharWidth=16, HeadRows+BodyRows+LegsRows=16)
///   • Native screen: 160×192 (double-wide pixels → 80 unique columns × 192 rows)
///   • DefaultTilesTall = 12, MaxTilesTall = 24 (~2× zoom out)
///   • Double-wide pixels: each logical pixel occupies two adjacent horizontal pixels;
///     all odd columns equal the preceding even column.
/// </summary>
public sealed class Atari2600System : RetroSystem
{
    public override string Name        => "Atari 2600";
    public override string Description => "16-px tiles dbl-wide · 16×16 sprites · 12 tiles tall · 1-bit · 1-color/scanline sprites";
    public override int   NativeTilePixels  => 16;
    public override float DefaultTilesTall  => 12f;   // native 160×192 → 12 tiles vertically
    public override float MaxTilesTall      => 24f;   // ~2× zoom out

    protected override bool DoubleWidePixels          => true;
    protected override bool OneBitTiles               => true;
    protected override bool SpriteOneColorPerScanline => true;

    // ── Tile palette ─────────────────────────────────────────────────────────
    // Index 0 = background fill (black).
    // Each tile uses exactly index 0 (black) plus ONE non-zero index below.
    protected override Palette TilePalette { get; } = new Atari2600Palette();

    // ── Tile pixel art (16×16, palette indices) ──────────────────────────────
    // Rules: (1) only index 0 and ONE non-zero foreground index per tile.
    //        (2) double-wide: col[2k+1] == col[2k] for all k (8 logical cols).

    protected override byte[][] GetTilePixels(TileType type, Color accentColor) => type switch
    {
        TileType.WoodFloor     => Atari2600Tiles.WoodFloor,
        TileType.Carpet        => Atari2600Tiles.Carpet,
        TileType.KitchenTile   => Atari2600Tiles.KitchenTile,
        TileType.Wall          => Atari2600Tiles.Wall,
        TileType.Door          => Atari2600Tiles.Door,
        TileType.Window        => Atari2600Tiles.Window,
        TileType.Furniture     => Atari2600Tiles.Furniture,
        TileType.Counter       => Atari2600Tiles.Counter,
        TileType.Bookshelf     => Atari2600Tiles.Bookshelf,
        TileType.Plant         => Atari2600Tiles.Plant,
        TileType.Grass         => Atari2600Tiles.Grass,
        TileType.Road          => Atari2600Tiles.Road,
        TileType.Sidewalk      => Atari2600Tiles.Sidewalk,
        TileType.HouseExterior => Atari2600Tiles.Wall,
        TileType.Accent        => Atari2600Tiles.Accent,
        _ => Atari2600Tiles.Wall
    };

    // ── Sprite dimensions ─────────────────────────────────────────────────────
    // HeadRows=5, BodyRows=7, LegsRows=4  (total 16)
    // 8 logical pixels per row (2 physical pixels each, double-wide).

    public override int CharWidth  => 16;
    public override int HeadRows   => 5;
    public override int BodyRows   => 7;
    public override int LegsRows   => 4;

    // ── Head parts (16 wide × 5 rows, 1 frame) ───────────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory
    // Rule: each row uses at most ONE non-zero semantic index.
    // Double-wide: col[2k+1] == col[2k] for all k.


    public override byte[][][][] HeadParts { get; } = [ Atari2600Sprites.Head0, Atari2600Sprites.Head1, Atari2600Sprites.Head2 ];

    // ── Body parts (16 wide × 7 rows, 1 frame) ───────────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory
    // Rule: each row uses at most ONE non-zero semantic index.
    // Double-wide: col[2k+1] == col[2k] for all k.


    public override byte[][][][] BodyParts { get; } = [ Atari2600Sprites.Body0, Atari2600Sprites.Body1, Atari2600Sprites.Body2 ];

    // ── Legs parts (16 wide × 4 rows, 4 frames) ──────────────────────────────
    // Idle: legs merged at centre; walk: legs spread to outer logical pixels.
    // Rule: each row uses at most ONE non-zero semantic index.
    // Double-wide: col[2k+1] == col[2k] for all k.


    public override byte[][][][] LegsParts { get; } = [ Atari2600Sprites.Legs0, Atari2600Sprites.Legs1, Atari2600Sprites.Legs2 ];

    // ── Back-facing heads (no eye row, extra hair) ────────────────────────────


    public override byte[][][][] HeadPartsBack { get; } = [ Atari2600Sprites.Head0Back, Atari2600Sprites.Head1Back, Atari2600Sprites.Head2Back ];

    // ── Back-facing bodies (no button row) ───────────────────────────────────


    public override byte[][][][] BodyPartsBack { get; } = [ Atari2600Sprites.Body0Back, Atari2600Sprites.Body1Back, Atari2600Sprites.Body2Back ];

    // ── Side-facing legs (profile walk cycle, 4 frames) ──────────────────────
    // Toe extends right; front foot swings right, back foot swings left.
    // One-color-per-scanline rule still applies.


    public override byte[][][][] LegsPartsSide { get; } = [ Atari2600Sprites.Legs0Side, Atari2600Sprites.Legs1Side, Atari2600Sprites.Legs2Side ];

    // ── Head palettes (5) — all colours sourced from Atari2600Palette ────────
    // Highlight slots that requested pure white are snapped to NearWhite, the
    // brightest authentic Atari NTSC entry.

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("Fair/Blonde",
            Skin:      Atari2600Palette.NearWhite,
            Hair:      Atari2600Palette.BrightYellow,
            Highlight: Atari2600Palette.NearWhite,
            Eyes:      Atari2600Palette.DarkSlate,
            Accessory: Atari2600Palette.WarmAmber),

        new("Fair/Brown",
            Skin:      Atari2600Palette.NearWhite,
            Hair:      Atari2600Palette.DarkBrown,
            Highlight: Atari2600Palette.NearWhite,
            Eyes:      Atari2600Palette.DarkSlate,
            Accessory: Atari2600Palette.DarkBrown),

        new("Medium/Black",
            Skin:      Atari2600Palette.WarmAmber,
            Hair:      Atari2600Palette.NearBlack,
            Highlight: Atari2600Palette.NearWhite,
            Eyes:      Atari2600Palette.NearBlack,
            Accessory: Atari2600Palette.MediumGray),

        new("Dark/Black",
            Skin:      Atari2600Palette.DarkRed,
            Hair:      Atari2600Palette.NearBlack,
            Highlight: Atari2600Palette.WarmAmber,
            Eyes:      Atari2600Palette.DarkSlate,
            Accessory: Atari2600Palette.MediumGray),

        new("Medium/Auburn",
            Skin:      Atari2600Palette.WarmAmber,
            Hair:      Atari2600Palette.VividRed,
            Highlight: Atari2600Palette.NearWhite,
            Eyes:      Atari2600Palette.DarkSlate,
            Accessory: Atari2600Palette.MediumGray),
    ];

    // ── Body palettes (5) ────────────────────────────────────────────────────

    public override BodyPalette[] BodyPalettes { get; } =
    [
        new("Green",
            Skin:           Atari2600Palette.NearWhite,
            Shirt:          Atari2600Palette.VividGreen,
            ShirtHighlight: Atari2600Palette.BrightCyan,
            Buttons:        Atari2600Palette.NearBlack,
            Accessory:      Atari2600Palette.DarkGreen),

        new("Blue",
            Skin:           Atari2600Palette.NearWhite,
            Shirt:          Atari2600Palette.BoldBlue,
            ShirtHighlight: Atari2600Palette.BrightCyan,
            Buttons:        Atari2600Palette.NearBlack,
            Accessory:      Atari2600Palette.NearBlack),

        new("Red",
            Skin:           Atari2600Palette.NearWhite,
            Shirt:          Atari2600Palette.VividRed,
            ShirtHighlight: Atari2600Palette.NearWhite,
            Buttons:        Atari2600Palette.NearBlack,
            Accessory:      Atari2600Palette.DarkRed),

        new("White",
            Skin:           Atari2600Palette.NearWhite,
            Shirt:          Atari2600Palette.NearWhite,
            ShirtHighlight: Atari2600Palette.NearWhite,
            Buttons:        Atari2600Palette.MediumGray,
            Accessory:      Atari2600Palette.LightGray),

        new("Teal",
            Skin:           Atari2600Palette.NearWhite,
            Shirt:          Atari2600Palette.BrightCyan,
            ShirtHighlight: Atari2600Palette.NearWhite,
            Buttons:        Atari2600Palette.NearBlack,
            Accessory:      Atari2600Palette.DarkTeal),
    ];

    // ── Legs palettes (4) ────────────────────────────────────────────────────

    public override LegsPalette[] LegsPalettes { get; } =
    [
        new("Blue Jeans/Brown",
            Skin:           Atari2600Palette.NearWhite,
            Pants:          Atari2600Palette.BoldBlue,
            PantsHighlight: Atari2600Palette.BrightCyan,
            Belt:           Atari2600Palette.WarmAmber,
            BeltHighlight:  Atari2600Palette.BrightYellow,
            Shoes:          Atari2600Palette.DarkBrown,
            ShoeHighlight:  Atari2600Palette.WarmAmber),

        new("Black/Black",
            Skin:           Atari2600Palette.NearWhite,
            Pants:          Atari2600Palette.NearBlack,
            PantsHighlight: Atari2600Palette.MediumGray,
            Belt:           Atari2600Palette.MediumGray,
            BeltHighlight:  Atari2600Palette.LightGray,
            Shoes:          Atari2600Palette.NearBlack,
            ShoeHighlight:  Atari2600Palette.MediumGray),

        new("Khaki/Tan",
            Skin:           Atari2600Palette.NearWhite,
            Pants:          Atari2600Palette.BrightYellow,
            PantsHighlight: Atari2600Palette.NearWhite,
            Belt:           Atari2600Palette.DarkBrown,
            BeltHighlight:  Atari2600Palette.BrightYellow,
            Shoes:          Atari2600Palette.DarkBrown,
            ShoeHighlight:  Atari2600Palette.WarmAmber),

        new("Gray/Dark",
            Skin:           Atari2600Palette.NearWhite,
            Pants:          Atari2600Palette.LightGray,
            PantsHighlight: Atari2600Palette.NearWhite,
            Belt:           Atari2600Palette.MediumGray,
            BeltHighlight:  Atari2600Palette.LightGray,
            Shoes:          Atari2600Palette.MediumGray,
            ShoeHighlight:  Atari2600Palette.NearBlack),
    ];
}
