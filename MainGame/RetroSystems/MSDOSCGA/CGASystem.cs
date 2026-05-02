using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.MSDOSCGA;

/// <summary>
/// MS-DOS CGA visual style — mode 4, palette 1 (high intensity).
///
/// Authentic characteristics:
///   • 16-px tile art (NativeTilePixels=16); the IBM CGA hardware text-mode tile is 8×8,
///     redrawn here at 16×16 to give space for the 4-color palette
///   • 4-color fixed palette: black, bright cyan, bright magenta, white
///     (IBM CGA mode 4 palette 1 — the most iconic CGA look)
///   • Character sprites: 16×16 texture (CharWidth=16, HeadRows=4, BodyRows=6, LegsRows=6)
///   • Native screen 320×200 → DefaultTilesTall = 12.5
///   • MaxTilesTall = 25 (~2× zoom out)
/// </summary>
public sealed class CGASystem : RetroSystem
{
    public override string Name        => "MS-DOS CGA";
    public override string Description => "16-px tiles · 16×16 sprites · 12.5 tiles tall · Mode 4 palette 1 (black/cyan/magenta/white)";
    public override int   NativeTilePixels => 16;
    public override float DefaultTilesTall => 12.5f;   // native 320×200 → 12.5 tiles vertically
    public override float MaxTilesTall     => 25f;     // ~2× zoom out

    protected override Palette TilePalette { get; } = new CGAPalette();

    // ── Tile pixel art (16×16, indices 0–3 only) ─────────────────────────────

    protected override byte[][] GetTilePixels(TileType type, Color accentColor) => type switch
    {
        TileType.WoodFloor     => CGATiles.WoodFloor,
        TileType.Carpet        => CGATiles.Carpet,
        TileType.KitchenTile   => CGATiles.KitchenTile,
        TileType.Wall          => CGATiles.Wall,
        TileType.Door          => CGATiles.Door,
        TileType.Window        => CGATiles.Window,
        TileType.Furniture     => CGATiles.Furniture,
        TileType.Counter       => CGATiles.Counter,
        TileType.Bookshelf     => CGATiles.Bookshelf,
        TileType.Plant         => CGATiles.Plant,
        TileType.Grass         => CGATiles.Grass,
        TileType.Road          => CGATiles.Road,
        TileType.Sidewalk      => CGATiles.Sidewalk,
        TileType.HouseExterior => CGATiles.Wall,
        TileType.Accent        => CGATiles.Accent,
        _ => CGATiles.Wall
    };


    // ── Sprite dimensions ─────────────────────────────────────────────────────

    public override int CharWidth  => 16;
    public override int HeadRows   => 4;
    public override int BodyRows   => 6;
    public override int LegsRows   => 6;

    // ── Head parts (16 wide × 4 rows, 1 frame each) ──────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory
    // Each column doubled from 8-wide art for authentic CGA wide-pixel look.


    public override byte[][][][] HeadParts { get; } = [ CGASprites.Head0, CGASprites.Head1, CGASprites.Head2 ];

    // ── Body parts (16 wide × 6 rows, 1 frame each) ──────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory


    public override byte[][][][] BodyParts { get; } = [ CGASprites.Body0, CGASprites.Body1, CGASprites.Body2 ];

    // ── Legs parts (16 wide × 6 rows, 4 walk frames) ─────────────────────────
    // Semantic: 1=Skin  2=Pants  3=PantsHighlight  4=Belt  6=Shoes  7=ShoeHighlight


    public override byte[][][][] LegsParts { get; } = [ CGASprites.Legs0, CGASprites.Legs1, CGASprites.Legs2 ];

    // ── Back-facing heads ─────────────────────────────────────────────────────


    public override byte[][][][] HeadPartsBack { get; } = [ CGASprites.Head0Back, CGASprites.Head1Back, CGASprites.Head2Back ];

    // ── Side-facing heads (profile right) ────────────────────────────────────


    public override byte[][][][] HeadPartsSide { get; } = [ CGASprites.Head0Side, CGASprites.Head1Side, CGASprites.Head2Side ];

    // ── Back-facing bodies ────────────────────────────────────────────────────


    public override byte[][][][] BodyPartsBack { get; } = [ CGASprites.Body0Back, CGASprites.Body1Back, CGASprites.Body2Back ];

    // ── Side-facing bodies (profile right) ───────────────────────────────────


    public override byte[][][][] BodyPartsSide { get; } = [ CGASprites.Body0Side, CGASprites.Body1Side, CGASprites.Body2Side ];

    // ── Side-facing legs (profile walk, 4 frames) ────────────────────────────


    public override byte[][][][] LegsPartsSide { get; } = [ CGASprites.Legs0Side, CGASprites.Legs1Side, CGASprites.Legs2Side ];

    // ── Head palettes — all resolved colors are from the 4-color CGA set ─────
    // Black=(0,0,0)  Cyan=(85,255,255)  Magenta=(255,85,255)  White=(255,255,255)

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("fair/blonde",
            Skin:      CGAPalette.White,   // white
            Hair:      CGAPalette.BrightMagenta,   // magenta (closest to blonde)
            Highlight: CGAPalette.White,   // white
            Eyes:      CGAPalette.BrightCyan,   // cyan
            Accessory: CGAPalette.BrightMagenta),  // magenta

        new("fair/dark-hair",
            Skin:      CGAPalette.White,   // white
            Hair:      CGAPalette.Black,   // black
            Highlight: CGAPalette.White,   // white
            Eyes:      CGAPalette.Black,   // black
            Accessory: CGAPalette.BrightCyan),  // cyan

        new("medium/black",
            Skin:      CGAPalette.BrightCyan,   // cyan
            Hair:      CGAPalette.Black,   // black
            Highlight: CGAPalette.White,   // white
            Eyes:      CGAPalette.Black,   // black
            Accessory: CGAPalette.Black),  // black

        new("dark/black",
            Skin:      CGAPalette.BrightMagenta,   // magenta
            Hair:      CGAPalette.Black,   // black
            Highlight: CGAPalette.BrightCyan,   // cyan
            Eyes:      CGAPalette.White,   // white (contrast)
            Accessory: CGAPalette.Black),  // black

        new("medium/magenta-hair",
            Skin:      CGAPalette.BrightCyan,   // cyan
            Hair:      CGAPalette.BrightMagenta,   // magenta
            Highlight: CGAPalette.White,   // white
            Eyes:      CGAPalette.Black,   // black
            Accessory: CGAPalette.BrightMagenta),  // magenta
    ];

    // ── Body palettes ─────────────────────────────────────────────────────────

    public override BodyPalette[] BodyPalettes { get; } =
    [
        new("cyan",
            Skin:           CGAPalette.White,   // white
            Shirt:          CGAPalette.BrightCyan,   // cyan
            ShirtHighlight: CGAPalette.White,   // white
            Buttons:        CGAPalette.Black,   // black
            Accessory:      CGAPalette.BrightMagenta),  // magenta

        new("magenta",
            Skin:           CGAPalette.White,   // white
            Shirt:          CGAPalette.BrightMagenta,   // magenta
            ShirtHighlight: CGAPalette.White,   // white
            Buttons:        CGAPalette.Black,   // black
            Accessory:      CGAPalette.BrightCyan),  // cyan

        new("white",
            Skin:           CGAPalette.White,   // white
            Shirt:          CGAPalette.White,   // white
            ShirtHighlight: CGAPalette.BrightCyan,   // cyan (highlight distinguishable)
            Buttons:        CGAPalette.Black,   // black
            Accessory:      CGAPalette.BrightMagenta),  // magenta

        new("black",
            Skin:           CGAPalette.White,   // white
            Shirt:          CGAPalette.Black,   // black
            ShirtHighlight: CGAPalette.BrightCyan,   // cyan
            Buttons:        CGAPalette.White,   // white
            Accessory:      CGAPalette.BrightMagenta),  // magenta

        new("magenta jacket",
            Skin:           CGAPalette.White,   // white
            Shirt:          CGAPalette.White,   // white shirt under jacket
            ShirtHighlight: CGAPalette.BrightCyan,   // cyan
            Buttons:        CGAPalette.Black,   // black
            Accessory:      CGAPalette.BrightMagenta),  // magenta jacket
    ];

    // ── Legs palettes ─────────────────────────────────────────────────────────

    public override LegsPalette[] LegsPalettes { get; } =
    [
        new("cyan pants",
            Skin:           CGAPalette.White,   // white
            Pants:          CGAPalette.BrightCyan,   // cyan
            PantsHighlight: CGAPalette.White,   // white
            Belt:           CGAPalette.Black,   // black
            BeltHighlight:  CGAPalette.White,   // white
            Shoes:          CGAPalette.Black,   // black
            ShoeHighlight:  CGAPalette.BrightMagenta),  // magenta

        new("magenta pants",
            Skin:           CGAPalette.White,   // white
            Pants:          CGAPalette.BrightMagenta,   // magenta
            PantsHighlight: CGAPalette.White,   // white
            Belt:           CGAPalette.Black,   // black
            BeltHighlight:  CGAPalette.White,   // white
            Shoes:          CGAPalette.Black,   // black
            ShoeHighlight:  CGAPalette.BrightCyan),  // cyan

        new("black pants",
            Skin:           CGAPalette.White,   // white
            Pants:          CGAPalette.Black,   // black
            PantsHighlight: CGAPalette.BrightCyan,   // cyan
            Belt:           CGAPalette.BrightMagenta,   // magenta
            BeltHighlight:  CGAPalette.White,   // white
            Shoes:          CGAPalette.Black,   // black
            ShoeHighlight:  CGAPalette.White),  // white

        new("white pants",
            Skin:           CGAPalette.White,   // white
            Pants:          CGAPalette.White,   // white
            PantsHighlight: CGAPalette.BrightCyan,   // cyan
            Belt:           CGAPalette.Black,   // black
            BeltHighlight:  CGAPalette.White,   // white
            Shoes:          CGAPalette.BrightMagenta,   // magenta
            ShoeHighlight:  CGAPalette.White),  // white
    ];
}
