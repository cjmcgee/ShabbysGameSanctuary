using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.AppleII;

/// <summary>
/// Apple II visual style — hi-res graphics mode.
///
/// Tile art is 14×14 — exactly two 7-pixel hi-res palette segments wide — and
/// follows the actual Apple II hi-res color rules rather than a simple double-
/// wide-pixel approximation:
///   • Each row splits into two 7-pixel segments (cols 0-6 and 7-13). Each
///     segment selects ONE palette — palette 0 (black/green/violet/white) or
///     palette 1 (black/orange/blue/white). Black and white work in either.
///   • Green / orange (palette odd-column color): only at odd x, with the
///     previous and following pixels both 0.
///   • Violet / blue  (palette even-column color): only at even x, with the
///     previous and following pixels both 0.
///   • White = run of ≥2 adjacent non-zero pixels. Black = run of ≥2 zeros.
///   • Cols 0 and 13 (left/right tile borders) are forced to 0 so that
///     horizontally adjacent tiles always meet at predictable black, instead
///     of bleeding their boundary pixels into each other.
///   • Tiles are designed "mostly black with simple motifs" so the above
///     constraints stay satisfiable without dense color masses.
///
/// Other characteristics:
///   • 6-color hi-res palette: black, green, violet, white, orange, blue
///     (NTSC artifact colors from the 1-MHz pixel clock — the defining Apple II look)
///   • Character sprites: 14×14 (HeadRows=4, BodyRows=6, LegsRows=4). Sprite
///     art is hand-authored to obey the same hi-res palette rules as the tiles.
///   • Native screen: 280×192 → DefaultTilesTall = 12
///   • MaxTilesTall = 24 (~2× zoom out)
/// </summary>
public sealed class AppleIISystem : RetroSystem
{
    public override string Name        => "Apple II";
    public override string Description => "14-px tiles · 14×14 sprites · 12 tiles tall · Hi-res 6-color w/ NTSC palette rules";
    public override int   NativeTilePixels => 14;
    public override float DefaultTilesTall => 12f;     // native 280×192 → 12 tiles vertically
    public override float MaxTilesTall     => 24f;     // ~2× zoom out

    // TODO: need to simulate color fringing for any text!
    protected override Palette TilePalette { get; } = new AppleIIPalette();

    // ── Tile pixel art (14×14, Apple II hi-res rules) ────────────────────────
    //   col 0 / col 13 always 0           (predictable black at tile borders)
    //   palette 0 (green/violet) OR palette 1 (orange/blue) per 7-pixel segment
    //   1 (green)  / 4 (orange):  odd  x, with 0 neighbors on both sides
    //   2 (violet) / 5 (blue):    even x, with 0 neighbors on both sides
    //   3 (white):  any x, in a run of ≥2 set pixels
    //   0 (black):  any x, in a run of ≥2 zero pixels

    protected override byte[][] GetTilePixels(TileType type, Color accentColor) => type switch
    {
        TileType.WoodFloor    => AppleIITiles.WoodFloor,
        TileType.Carpet       => AppleIITiles.Carpet,
        TileType.KitchenTile  => AppleIITiles.KitchenTile,
        TileType.Wall         => AppleIITiles.Wall,
        TileType.Door         => AppleIITiles.Door,
        TileType.Window       => AppleIITiles.Window,
        TileType.Furniture    => AppleIITiles.Furniture,
        TileType.Counter      => AppleIITiles.Counter,
        TileType.Bookshelf    => AppleIITiles.Bookshelf,
        TileType.Plant        => AppleIITiles.Plant,
        TileType.Grass        => AppleIITiles.Grass,
        TileType.Road         => AppleIITiles.Road,
        TileType.Sidewalk     => AppleIITiles.Sidewalk,
        TileType.HouseExterior => AppleIITiles.Wall,
        TileType.Accent       => AppleIITiles.Accent,
        _ => AppleIITiles.Wall
    };


    // ── Sprite dimensions (14×14 total) ───────────────────────────────────────
    // Sprite art follows the same Apple II discipline as tile art (loose form):
    //   • 14 wide × 14 tall (head 4 + body 6 + legs 4)
    //   • Features rendered as runs of ≥2 set pixels (the "white-rule" form)
    //     so single isolated colored pixels — which would impose strict even/
    //     odd column rules per palette — never appear.
    //   • Cols 0 and 13 are typically 0 to keep the silhouette inside the
    //     visible 12-col band, mirroring the tile-edge convention.

    public override int CharWidth  => 14;
    public override int HeadRows   => 4;
    public override int BodyRows   => 6;
    public override int LegsRows   => 4;

    // ── Head variants (1 frame × 4 rows × 14 cols) ───────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory


    public override byte[][][][] HeadParts { get; } = [ AppleIISprites.Head0, AppleIISprites.Head1, AppleIISprites.Head2 ];

    // ── Body variants (1 frame × 6 rows × 14 cols) ───────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory


    public override byte[][][][] BodyParts { get; } = [ AppleIISprites.Body0, AppleIISprites.Body1, AppleIISprites.Body2 ];

    // ── Legs variants (4 frames × 4 rows × 14 cols) ──────────────────────────
    // Semantic: 1=Skin(bare)  2=Pants  3=PantsHighlight  4=Belt
    //           5=BeltHighlight  6=Shoes  7=ShoeHighlight


    public override byte[][][][] LegsParts { get; } = [ AppleIISprites.Legs0, AppleIISprites.Legs1, AppleIISprites.Legs2 ];

    // ── Back-facing heads (no eye row) ────────────────────────────────────────


    public override byte[][][][] HeadPartsBack { get; } = [ AppleIISprites.Head0Back, AppleIISprites.Head1Back, AppleIISprites.Head2Back ];

    // ── Back-facing bodies (no buttons) ──────────────────────────────────────


    public override byte[][][][] BodyPartsBack { get; } = [ AppleIISprites.Body0Back, AppleIISprites.Body1Back, AppleIISprites.Body2Back ];

    // ── Side-facing legs (4 frames × 4 rows × 14 cols) ───────────────────────
    // Profile facing right; front foot swings forward (right), back foot trails.


    public override byte[][][][] LegsPartsSide { get; } = [ AppleIISprites.Legs0Side, AppleIISprites.Legs1Side, AppleIISprites.Legs2Side ];

    // ── Head palettes — all resolved colors are from the 6-color hi-res set ──
    // Black=(0,0,0)  Green=(20,245,60)  Violet=(193,28,255)
    // White=(255,255,255)  Orange=(255,106,60)  Blue=(20,88,255)

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("fair/light-hair",
            Skin:      AppleIIPalette.White,   // white
            Hair:      AppleIIPalette.Orange,   // orange (blonde approximation)
            Highlight: AppleIIPalette.White,   // white
            Eyes:      AppleIIPalette.Blue,   // blue
            Accessory: AppleIIPalette.Orange),  // orange

        new("fair/dark-hair",
            Skin:      AppleIIPalette.White,   // white
            Hair:      AppleIIPalette.Black,   // black
            Highlight: AppleIIPalette.White,   // white
            Eyes:      AppleIIPalette.Black,   // black
            Accessory: AppleIIPalette.Violet),  // violet

        new("medium/black-hair",
            Skin:      AppleIIPalette.Orange,   // orange
            Hair:      AppleIIPalette.Black,   // black
            Highlight: AppleIIPalette.White,   // white
            Eyes:      AppleIIPalette.Blue,   // blue
            Accessory: AppleIIPalette.Black),  // black

        new("dark/black-hair",
            Skin:      AppleIIPalette.Violet,   // violet
            Hair:      AppleIIPalette.Black,   // black
            Highlight: AppleIIPalette.Orange,   // orange
            Eyes:      AppleIIPalette.White,   // white (contrast)
            Accessory: AppleIIPalette.Black),  // black

        new("medium/violet-hair",
            Skin:      AppleIIPalette.Orange,   // orange
            Hair:      AppleIIPalette.Violet,   // violet
            Highlight: AppleIIPalette.White,   // white
            Eyes:      AppleIIPalette.Blue,   // blue
            Accessory: AppleIIPalette.Violet),  // violet
    ];

    // ── Body palettes ─────────────────────────────────────────────────────────

    public override BodyPalette[] BodyPalettes { get; } =
    [
        new("green",
            Skin:           AppleIIPalette.Orange,   // orange
            Shirt:          AppleIIPalette.Green,   // green
            ShirtHighlight: AppleIIPalette.White,   // white
            Buttons:        AppleIIPalette.Black,   // black
            Accessory:      AppleIIPalette.Blue),  // blue

        new("blue",
            Skin:           AppleIIPalette.Orange,   // orange
            Shirt:          AppleIIPalette.Blue,   // blue
            ShirtHighlight: AppleIIPalette.White,   // white
            Buttons:        AppleIIPalette.Black,   // black
            Accessory:      AppleIIPalette.Orange),  // orange

        new("violet",
            Skin:           AppleIIPalette.Orange,   // orange
            Shirt:          AppleIIPalette.Violet,   // violet
            ShirtHighlight: AppleIIPalette.White,   // white
            Buttons:        AppleIIPalette.Black,   // black
            Accessory:      AppleIIPalette.Green),  // green

        new("orange",
            Skin:           AppleIIPalette.White,   // white
            Shirt:          AppleIIPalette.Orange,   // orange
            ShirtHighlight: AppleIIPalette.White,   // white
            Buttons:        AppleIIPalette.Black,   // black
            Accessory:      AppleIIPalette.Blue),  // blue

        new("white",
            Skin:           AppleIIPalette.Orange,   // orange
            Shirt:          AppleIIPalette.White,   // white
            ShirtHighlight: AppleIIPalette.Blue,   // blue (distinguishable highlight)
            Buttons:        AppleIIPalette.Black,   // black
            Accessory:      AppleIIPalette.Green),  // green
    ];

    // ── Legs palettes ─────────────────────────────────────────────────────────

    public override LegsPalette[] LegsPalettes { get; } =
    [
        new("blue pants",
            Skin:           AppleIIPalette.Orange,   // orange
            Pants:          AppleIIPalette.Blue,   // blue
            PantsHighlight: AppleIIPalette.White,   // white
            Belt:           AppleIIPalette.Black,   // black
            BeltHighlight:  AppleIIPalette.White,   // white
            Shoes:          AppleIIPalette.Black,   // black
            ShoeHighlight:  AppleIIPalette.Orange),  // orange

        new("violet pants",
            Skin:           AppleIIPalette.Orange,   // orange
            Pants:          AppleIIPalette.Violet,   // violet
            PantsHighlight: AppleIIPalette.White,   // white
            Belt:           AppleIIPalette.Black,   // black
            BeltHighlight:  AppleIIPalette.Orange,   // orange
            Shoes:          AppleIIPalette.Black,   // black
            ShoeHighlight:  AppleIIPalette.White),  // white

        new("black pants",
            Skin:           AppleIIPalette.Orange,   // orange
            Pants:          AppleIIPalette.Black,   // black
            PantsHighlight: AppleIIPalette.Blue,   // blue
            Belt:           AppleIIPalette.Orange,   // orange
            BeltHighlight:  AppleIIPalette.White,   // white
            Shoes:          AppleIIPalette.Black,   // black
            ShoeHighlight:  AppleIIPalette.White),  // white

        new("green pants",
            Skin:           AppleIIPalette.Orange,   // orange
            Pants:          AppleIIPalette.Green,   // green
            PantsHighlight: AppleIIPalette.White,   // white
            Belt:           AppleIIPalette.Black,   // black
            BeltHighlight:  AppleIIPalette.Orange,   // orange
            Shoes:          AppleIIPalette.Black,   // black
            ShoeHighlight:  AppleIIPalette.White),  // white
    ];
}
