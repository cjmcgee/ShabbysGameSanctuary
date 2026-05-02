using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.Commodore64;

/// <summary>
/// Commodore 64 visual style.
///
/// Authentic characteristics:
///   • 24-px tile art (NativeTilePixels=24); the C64's hardware text/PETSCII unit is 8×8,
///     redrawn here at 24×24 to allow multicolor detail
///   • VIC-II 16-color palette with distinctive color relationships
///   • Multicolor mode: 1 global color + up to 3 local colors per tile (index 7 = grey shared register)
///   • Multicolor sprites: up to 4 semantic colors per part frame (transparent + 2 globals + 1 unique)
///   • Character sprites: 12×21 texture (HeadRows=6, BodyRows=8, LegsRows=7)
///   • Native screen: 160×200 (double-wide pixels → 80 unique columns × 200 rows)
///   • DefaultTilesTall = 12.5, MaxTilesTall = 25 (~2× zoom out)
///   • Double-wide pixels: each logical pixel occupies two adjacent horizontal pixels;
///     all odd columns equal the preceding even column.
/// </summary>
public sealed class C64System : RetroSystem
{
    public override string Name        => "Commodore 64";
    public override string Description => "24-px tiles dbl-wide · 12×21 sprites · 12.5 tiles tall · VIC-II multicolor · 1 global + 3 local / 4 sprite colors";
    public override int   NativeTilePixels  => 24;
    public override float DefaultTilesTall  => 12.5f;   // native 160×200 → 12.5 tiles vertically
    public override float MaxTilesTall      => 25f;     // ~2× zoom out
    protected override bool DoubleWidePixels => true;

    // VIC-II multicolor: palette index 7 (C64 grey) acts as the shared global color register.
    // Each tile may use it freely plus up to 3 additional local indices.
    // Sprites are limited to 4 distinct non-transparent semantic indices per part frame.
    protected override int GlobalTileColorIndex   => 7;
    protected override int MaxLocalTileColors     => 3;
    protected override int MaxSpriteSemanticColors => 4;

    protected override Palette TilePalette { get; } = new C64Palette();

    // ── Tile pixel art (24×24) ───────────────────────────────────────────────
    // All rows obey the double-wide rule: col[2k+1] == col[2k] (12 logical cols).
    // Color constraints: index 0 (bg) + index 7 (global grey) + up to 3 locals per tile.

    protected override byte[][] GetTilePixels(TileType type, Color accentColor) => type switch
    {
        TileType.WoodFloor    => C64Tiles.WoodFloor,
        TileType.Carpet       => C64Tiles.Carpet,
        TileType.KitchenTile  => C64Tiles.KitchenTile,
        TileType.Wall         => C64Tiles.Wall,
        TileType.Door         => C64Tiles.Door,
        TileType.Window       => C64Tiles.Window,
        TileType.Furniture    => C64Tiles.Furniture,
        TileType.Counter      => C64Tiles.Counter,
        TileType.Bookshelf    => C64Tiles.Bookshelf,
        TileType.Plant        => C64Tiles.Plant,
        TileType.Grass        => C64Tiles.Grass,
        TileType.Road         => C64Tiles.Road,
        TileType.Sidewalk     => C64Tiles.Sidewalk,
        TileType.HouseExterior => C64Tiles.Wall,
        TileType.Accent       => C64Tiles.Accent,
        _ => C64Tiles.Wall
    };


    // ── Sprite dimensions (12×21 total) ──────────────────────────────────────
    // 6 logical pixels per row (2 physical pixels each).

    public override int CharWidth  => 12;
    public override int HeadRows   => 6;
    public override int BodyRows   => 8;
    public override int LegsRows   => 7;

    // ── Head variants (1 frame × 6 rows × 12 cols) ───────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory


    public override byte[][][][] HeadParts { get; } = [ C64Sprites.Head0, C64Sprites.Head1, C64Sprites.Head2 ];

    // ── Body variants (1 frame × 8 rows × 12 cols) ───────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory


    public override byte[][][][] BodyParts { get; } = [ C64Sprites.Body0, C64Sprites.Body1, C64Sprites.Body2 ];

    // ── Legs variants (4 frames × 7 rows × 12 cols) ──────────────────────────
    // Idle: legs merged at lp1-lp4; walk: stride to lp0/lp5 outer columns.


    public override byte[][][][] LegsParts { get; } = [ C64Sprites.Legs0, C64Sprites.Legs1, C64Sprites.Legs2 ];

    // ── Back-facing heads (1 frame × 6 rows × 12 cols) ───────────────────────
    // Shows back of head — hair/hat only, no face visible.


    public override byte[][][][] HeadPartsBack { get; } = [ C64Sprites.Head0Back, C64Sprites.Head1Back, C64Sprites.Head2Back ];

    // ── Side-facing heads (1 frame × 6 rows × 12 cols, profile facing right) ─
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory


    public override byte[][][][] HeadPartsSide { get; } = [ C64Sprites.Head0Side, C64Sprites.Head1Side, C64Sprites.Head2Side ];

    // ── Back-facing bodies (1 frame × 8 rows × 12 cols) ──────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory


    public override byte[][][][] BodyPartsBack { get; } = [ C64Sprites.Body0Back, C64Sprites.Body1Back, C64Sprites.Body2Back ];

    // ── Side-facing bodies (1 frame × 8 rows × 12 cols, profile facing right) ─


    public override byte[][][][] BodyPartsSide { get; } = [ C64Sprites.Body0Side, C64Sprites.Body1Side, C64Sprites.Body2Side ];

    // ── Side-facing legs (4 frames × 7 rows × 12 cols, profile facing right) ─
    // Legs stride forward (right) / backward (left) in alternating walk frames.


    public override byte[][][][] LegsPartsSide { get; } = [ C64Sprites.Legs0Side, C64Sprites.Legs1Side, C64Sprites.Legs2Side ];

    // ── Head palettes (VIC-II skin/hair tones) ────────────────────────────────

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("fair/blonde",
            Skin:      C64Palette.SkinLight,
            Hair:      C64Palette.BrightYellow,   // VIC-II yellow
            Highlight: C64Palette.SkinPale,
            Eyes:      C64Palette.Blue,   // VIC-II blue
            Accessory: C64Palette.Yellow),
        new("fair/brown",
            Skin:      C64Palette.SkinLight,
            Hair:      C64Palette.DarkOlive,   // brown
            Highlight: C64Palette.SkinPale,
            Eyes:      C64Palette.DarkGray,
            Accessory: C64Palette.DarkOlive),
        new("medium/black",
            Skin:      C64Palette.SkinTan,
            Hair:      C64Palette.DoorBlack,
            Highlight: C64Palette.SkinTan2,
            Eyes:      C64Palette.DarkGray,
            Accessory: C64Palette.DoorBlack),
        new("dark/black",
            Skin:      C64Palette.MidBrown,
            Hair:      C64Palette.DoorBlack,
            Highlight: C64Palette.SkinMedium,
            Eyes:      C64Palette.Cyan,   // VIC-II cyan
            Accessory: C64Palette.DoorBlack),
        new("medium/auburn",
            Skin:      C64Palette.SkinTan,
            Hair:      C64Palette.DeepRed,   // VIC-II red — auburn
            Highlight: C64Palette.SkinTan2,
            Eyes:      C64Palette.Blue,
            Accessory: C64Palette.DeepRed),
    ];

    // ── Body palettes ─────────────────────────────────────────────────────────

    public override BodyPalette[] BodyPalettes { get; } =
    [
        new("green",
            Skin:           C64Palette.SkinLight,
            Shirt:          C64Palette.Green,   // VIC-II green
            ShirtHighlight: C64Palette.LightGreen,
            Buttons:        C64Palette.White,
            Accessory:      C64Palette.Cyan),
        new("blue",
            Skin:           C64Palette.SkinLight,
            Shirt:          C64Palette.Blue,   // VIC-II blue
            ShirtHighlight: C64Palette.MidBlue,
            Buttons:        C64Palette.LightGrey,
            Accessory:      C64Palette.BrightYellow),
        new("red",
            Skin:           C64Palette.SkinLight,
            Shirt:          C64Palette.DeepRed,   // VIC-II red
            ShirtHighlight: C64Palette.Salmon,
            Buttons:        C64Palette.White,
            Accessory:      C64Palette.BrightYellow),
        new("white/light",
            Skin:           C64Palette.SkinLight,
            Shirt:          C64Palette.LightGrey,
            ShirtHighlight: C64Palette.White,
            Buttons:        C64Palette.Blue,
            Accessory:      C64Palette.Green),
        new("teal",
            Skin:           C64Palette.SkinLight,
            Shirt:          C64Palette.MidBlue,
            ShirtHighlight: C64Palette.Cyan,
            Buttons:        C64Palette.White,
            Accessory:      C64Palette.MidMagenta),
    ];

    // ── Legs palettes ─────────────────────────────────────────────────────────

    public override LegsPalette[] LegsPalettes { get; } =
    [
        new("blue jeans/brown shoes",
            Skin:           C64Palette.SkinLight,
            Pants:          C64Palette.Blue,   // VIC-II blue
            PantsHighlight: C64Palette.MidBlue,
            Belt:           C64Palette.DarkOlive,
            BeltHighlight:  C64Palette.SkinTan3,
            Shoes:          C64Palette.DarkBrown,
            ShoeHighlight:  C64Palette.Brown),
        new("black/black",
            Skin:           C64Palette.SkinLight,
            Pants:          C64Palette.DoorBlack,
            PantsHighlight: C64Palette.DarkGray,
            Belt:           C64Palette.DarkGray,
            BeltHighlight:  C64Palette.Grey,
            Shoes:          C64Palette.DoorBlack,
            ShoeHighlight:  C64Palette.DarkGray),
        new("khaki/tan",
            Skin:           C64Palette.SkinLight,
            Pants:          C64Palette.SkinTan3,
            PantsHighlight: C64Palette.BrightYellow,
            Belt:           C64Palette.DarkOlive,
            BeltHighlight:  C64Palette.BurntOrange,
            Shoes:          C64Palette.DarkOlive,
            ShoeHighlight:  C64Palette.SkinMedium2),
        new("gray/dark",
            Skin:           C64Palette.SkinLight,
            Pants:          C64Palette.Grey,
            PantsHighlight: C64Palette.LightGrey,
            Belt:           C64Palette.DarkGray,
            BeltHighlight:  C64Palette.Grey,
            Shoes:          C64Palette.DarkGray,
            ShoeHighlight:  C64Palette.Grey),
    ];
}
