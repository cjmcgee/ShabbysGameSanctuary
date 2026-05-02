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
        TileType.WoodFloor    => WoodFloor,
        TileType.Carpet       => Carpet,
        TileType.KitchenTile  => KitchenTile,
        TileType.Wall         => Wall,
        TileType.Door         => Door,
        TileType.Window       => Window,
        TileType.Furniture    => Furniture,
        TileType.Counter      => Counter,
        TileType.Bookshelf    => Bookshelf,
        TileType.Plant        => Plant,
        TileType.Grass        => Grass,
        TileType.Road         => Road,
        TileType.Sidewalk     => Sidewalk,
        TileType.HouseExterior => Wall,
        TileType.Accent       => Accent,
        _ => Wall
    };

    // Orange planks (4) — odd-col orange dots (palette 1), black plank dividers
    private static readonly byte[][] WoodFloor =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // plank divider
        [ 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 0 ],   // plank 1
        [ 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 0 ],
        [ 0, 4, 0, 0, 0, 4, 0, 4, 0, 4, 0, 4, 0, 0 ],   // grain at col 3
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // divider
        [ 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 0 ],   // plank 2
        [ 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 0 ],
        [ 0, 4, 0, 4, 0, 4, 0, 4, 0, 0, 0, 4, 0, 0 ],   // grain at col 9
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // divider
        [ 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 0 ],   // plank 3
        [ 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 0 ],
        [ 0, 4, 0, 4, 0, 0, 0, 4, 0, 4, 0, 4, 0, 0 ],   // grain at col 5
        [ 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // divider
    ];

    // Green carpet field (1) with violet (2) decorative dot rows — palette 0
    private static readonly byte[][] Carpet =
    [
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],   // violet dot row
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],   // violet dot row
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
    ];

    // White tile faces (3) with black grout (0) — palette-agnostic
    private static readonly byte[][] KitchenTile =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // top grout
        [ 0, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3, 0 ],   // tile row
        [ 0, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // horiz grout
        [ 0, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // horiz grout
        [ 0, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3, 0 ],
    ];

    // White brick faces (3) with dotted blue (5) mortar lines — palette 1 mortar
    // Real mortar can't be a continuous blue run (would render white), so the
    // mortar courses are single dotted scanlines and brick rows separate via
    // single-column 0 grout lines.
    private static readonly byte[][] Wall =
    [
        [ 0, 3, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 0 ],   // brick row (col 6 grout)
        [ 0, 3, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // blue mortar (dotted)
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 0 ],   // staggered (col 9 grout)
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 0 ],
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // mortar
        [ 0, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],   // staggered (col 4 grout)
        [ 0, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // mortar
        [ 0, 3, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 0 ],   // brick row (col 6 grout)
        [ 0, 3, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // mortar
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 0 ],   // staggered
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 0 ],
    ];

    // Black frame (0), violet (2) recessed panel dots, white knob (3) — palette 0
    // Violet "panels" are dotted (every even col with 0 between) — solid violet
    // is impossible under Apple II rules.
    private static readonly byte[][] Door =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // top frame
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],   // upper panel (violet dots)
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // mid rail
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 0, 0, 0 ],   // white knob (cols 9-10)
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],   // lower panel
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // bottom frame
    ];

    // Black frame (0), blue (5) dotted glass panes — palette 1, two panes
    private static readonly byte[][] Window =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // top frame
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],   // upper panes (blue dots)
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // crossbar
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],   // lower panes
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // bottom frame
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // Blue (5) dotted outline + mid rail, black interior — palette 1
    private static readonly byte[][] Furniture =
    [
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // top dotted edge
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0 ],   // sides
        [ 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0 ],
        [ 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0 ],
        [ 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // mid divider
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0 ],
        [ 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0 ],
        [ 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // bottom dotted edge
    ];

    // White (3) work surface bordered top/bottom by dotted blue (5), black base
    private static readonly byte[][] Counter =
    [
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // top blue trim
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],   // white surface
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // bottom blue trim
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // black base
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // Black shelves (0) with single-pixel "book" spines — palette varies per
    // segment per row: green (palette 0) and orange (palette 1) book sections.
    private static readonly byte[][] Bookshelf =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // top shelf
        [ 0, 1, 0, 1, 0, 1, 0, 4, 0, 4, 0, 4, 0, 0 ],   // green | orange books
        [ 0, 1, 0, 1, 0, 1, 0, 4, 0, 4, 0, 4, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 4, 0, 4, 0, 4, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 4, 0, 4, 0, 4, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 4, 0, 4, 0, 4, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // mid shelf
        [ 0, 4, 0, 4, 0, 4, 0, 1, 0, 1, 0, 1, 0, 0 ],   // orange | green books
        [ 0, 4, 0, 4, 0, 4, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 4, 0, 4, 0, 4, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 4, 0, 4, 0, 4, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 4, 0, 4, 0, 4, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 4, 0, 4, 0, 4, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // bottom shelf
    ];

    // Green canopy (1) on a green stem (palette 0) with an orange pot (4,
    // palette 1) at the bottom. All foliage/pot pixels are at odd cols.
    private static readonly byte[][] Plant =
    [
        [ 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0 ],   // canopy tip
        [ 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],   // canopy mid
        [ 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 ],   // stem (col 7)
        [ 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 4, 0, 4, 0, 4, 0, 4, 0, 0, 0, 0 ],   // pot top (orange)
        [ 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 0 ],   // pot body
        [ 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 0 ],
        [ 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 4, 0, 0 ],
        [ 0, 0, 0, 4, 0, 4, 0, 4, 0, 4, 0, 0, 0, 0 ],   // pot base
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // Green dot field (palette 0) with scattered missing dots for texture.
    private static readonly byte[][] Grass =
    [
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0 ],   // missing cols 3, 11
        [ 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],   // missing col 1
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0 ],   // missing col 7
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0 ],   // missing col 5
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],   // missing col 3
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0 ],   // missing col 9
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0 ],   // missing col 11
        [ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 ],
    ];

    // Black road (0) with white centre stripe (3) — palette agnostic
    private static readonly byte[][] Road =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],   // centre stripe
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // White (3) slabs separated by dotted blue (5) expansion joints — palette 1
    private static readonly byte[][] Sidewalk =
    [
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // expansion joint
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // expansion joint
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
    ];

    // Solid accent fill (index 6 = appended accentColor from BuildTileset).
    // Used by the neighborhood map to render house exterior tiles in arbitrary
    // colors and to distinguish doors (near-black accent) from walls. The
    // Apple II hi-res palette discipline doesn't apply to index 6 because the
    // accent color is a user-supplied RGB outside the 6-color hi-res set;
    // tiles tile seamlessly because there are no zero-edge borders to bleed.
    private static readonly byte[][] Accent =
    [
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
    ];

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

    private static readonly byte[][][] _head0 =   // basic round head
    [
        [
            [ 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],   // hair crown
            [ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0 ],   // hair sides + face
            [ 0, 0, 1, 1, 4, 4, 1, 1, 4, 4, 1, 1, 0, 0 ],   // face + two eyes
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
        ],
    ];

    private static readonly byte[][][] _head1 =   // cap / hat
    [
        [
            [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // hat top
            [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // hat brim
            [ 0, 0, 1, 1, 4, 4, 1, 1, 4, 4, 1, 1, 0, 0 ],   // face + eyes
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
        ],
    ];

    private static readonly byte[][][] _head2 =   // long / full hair
    [
        [
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // full hair top
            [ 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0 ],   // hair sides + face
            [ 0, 2, 2, 1, 4, 4, 1, 1, 4, 4, 1, 2, 2, 0 ],   // hair sides + eyes + face
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
        ],
    ];

    public override byte[][][][] HeadParts { get; } = [ _head0, _head1, _head2 ];

    // ── Body variants (1 frame × 6 rows × 14 cols) ───────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory

    private static readonly byte[][][] _body0 =   // casual shirt
    [
        [
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // shoulders
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // chest
            [ 0, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 0 ],   // highlight band
            [ 0, 2, 2, 2, 4, 4, 2, 2, 4, 4, 2, 2, 2, 0 ],   // two buttons
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // lower
        ],
    ];

    private static readonly byte[][][] _body1 =   // collared / formal
    [
        [
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
            [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // lapels around tie
            [ 0, 5, 5, 5, 5, 4, 4, 4, 4, 5, 5, 5, 5, 0 ],   // lapels + tie buttons
            [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // lapels lower
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // torso
            [ 0, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 0 ],   // lower buttons
        ],
    ];

    private static readonly byte[][][] _body2 =   // jacket / hoodie
    [
        [
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
            [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // jacket full
            [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // open jacket + shirt
            [ 0, 5, 5, 5, 5, 3, 3, 4, 4, 3, 3, 5, 5, 0 ],   // highlight + button
            [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // shirt
            [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // jacket hem
        ],
    ];

    public override byte[][][][] BodyParts { get; } = [ _body0, _body1, _body2 ];

    // ── Legs variants (4 frames × 4 rows × 14 cols) ──────────────────────────
    // Semantic: 1=Skin(bare)  2=Pants  3=PantsHighlight  4=Belt
    //           5=BeltHighlight  6=Shoes  7=ShoeHighlight

    private static readonly byte[][][] _legs0 =   // pants + belt
    [
        [   // idle
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],   // belt
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],   // pants + buckle
            [ 0, 2, 2, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 0 ],   // legs split
            [ 0, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6, 0 ],   // shoes
        ],
        [   // left foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
            [ 0, 2, 2, 2, 2, 2, 2, 2, 0, 0, 2, 2, 2, 0 ],   // left ahead
            [ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6, 6, 0 ],
        ],
        [   // crossing / passing
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // legs together
            [ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
        [   // right foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
            [ 0, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 2, 2, 0 ],   // right ahead
            [ 0, 6, 6, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs1 =   // formal trousers (crease)
    [
        [   // idle
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],   // belt
            [ 0, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 0 ],   // crease band
            [ 0, 2, 2, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 0 ],   // legs split
            [ 0, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6, 0 ],   // shoes
        ],
        [   // left foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 0 ],
            [ 0, 2, 2, 2, 2, 2, 2, 2, 0, 0, 2, 2, 2, 0 ],
            [ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6, 6, 0 ],
        ],
        [   // crossing
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
        [   // right foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 0 ],
            [ 0, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 2, 2, 0 ],
            [ 0, 6, 6, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs2 =   // shorts + bare skin
    [
        [   // idle
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],   // belt
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],   // shorts + buckle
            [ 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0 ],   // bare legs
            [ 0, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6, 0 ],   // shoes
        ],
        [   // left foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0 ],
            [ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6, 6, 0 ],
        ],
        [   // crossing
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
        [   // right foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
            [ 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 6, 6, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0 ],
        ],
    ];

    public override byte[][][][] LegsParts { get; } = [ _legs0, _legs1, _legs2 ];

    // ── Back-facing heads (no eye row) ────────────────────────────────────────

    private static readonly byte[][][] _head0Back =
    [[
        [ 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],   // hair crown
        [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // hair back (no eyes)
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // upper neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
    ]];

    private static readonly byte[][][] _head1Back =
    [[
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // hat top (full brim from behind)
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // hat brim
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // upper neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
    ]];

    private static readonly byte[][][] _head2Back =
    [[
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // full hair
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // full hair back
        [ 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0 ],   // hair sides + neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
    ]];

    public override byte[][][][] HeadPartsBack { get; } = [ _head0Back, _head1Back, _head2Back ];

    // ── Back-facing bodies (no buttons) ──────────────────────────────────────

    private static readonly byte[][][] _body0Back =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // back shoulders
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // back
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // back (no buttons)
        [ 0, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 0 ],   // highlight band
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // lower
    ]];

    private static readonly byte[][][] _body1Back =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // collar back
        [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // collar
        [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // lapels back
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // torso
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // lower
    ]];

    private static readonly byte[][][] _body2Back =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // jacket back
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],
    ]];

    public override byte[][][][] BodyPartsBack { get; } = [ _body0Back, _body1Back, _body2Back ];

    // ── Side-facing legs (4 frames × 4 rows × 14 cols) ───────────────────────
    // Profile facing right; front foot swings forward (right), back foot trails.

    private static readonly byte[][][] _legs0Side =
    [
        [   // idle — both legs centered, toe extends right
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],   // belt
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // pants
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],   // shoe extends right (toe)
        ],
        [   // walk A — front foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2 ],   // legs split fwd/back
            [ 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6 ],   // back heel + front toe
        ],
        [   // mid — legs together, lifted
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // feet lifted
        ],
        [   // walk B — back foot forward (heel kick)
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 0, 0 ],   // both feet planted
        ],
    ];

    private static readonly byte[][][] _legs1Side =
    [
        [   // idle
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        ],
        [   // walk A
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2 ],
            [ 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6 ],
        ],
        [   // mid
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 0, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs2Side =
    [
        [   // idle (shorts + bare legs)
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        ],
        [   // walk A
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1 ],
            [ 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6 ],
        ],
        [   // mid
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 0, 0 ],
        ],
    ];

    public override byte[][][][] LegsPartsSide { get; } = [ _legs0Side, _legs1Side, _legs2Side ];

    // ── Head palettes — all resolved colors are from the 6-color hi-res set ──
    // Black=(0,0,0)  Green=(20,245,60)  Violet=(193,28,255)
    // White=(255,255,255)  Orange=(255,106,60)  Blue=(20,88,255)

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("fair/light-hair",
            Skin:      new Color(255, 255, 255),   // white
            Hair:      new Color(255, 106,  60),   // orange (blonde approximation)
            Highlight: new Color(255, 255, 255),   // white
            Eyes:      new Color( 20,  88, 255),   // blue
            Accessory: new Color(255, 106,  60)),  // orange

        new("fair/dark-hair",
            Skin:      new Color(255, 255, 255),   // white
            Hair:      new Color(  0,   0,   0),   // black
            Highlight: new Color(255, 255, 255),   // white
            Eyes:      new Color(  0,   0,   0),   // black
            Accessory: new Color(193,  28, 255)),  // violet

        new("medium/black-hair",
            Skin:      new Color(255, 106,  60),   // orange
            Hair:      new Color(  0,   0,   0),   // black
            Highlight: new Color(255, 255, 255),   // white
            Eyes:      new Color( 20,  88, 255),   // blue
            Accessory: new Color(  0,   0,   0)),  // black

        new("dark/black-hair",
            Skin:      new Color(193,  28, 255),   // violet
            Hair:      new Color(  0,   0,   0),   // black
            Highlight: new Color(255, 106,  60),   // orange
            Eyes:      new Color(255, 255, 255),   // white (contrast)
            Accessory: new Color(  0,   0,   0)),  // black

        new("medium/violet-hair",
            Skin:      new Color(255, 106,  60),   // orange
            Hair:      new Color(193,  28, 255),   // violet
            Highlight: new Color(255, 255, 255),   // white
            Eyes:      new Color( 20,  88, 255),   // blue
            Accessory: new Color(193,  28, 255)),  // violet
    ];

    // ── Body palettes ─────────────────────────────────────────────────────────

    public override BodyPalette[] BodyPalettes { get; } =
    [
        new("green",
            Skin:           new Color(255, 106,  60),   // orange
            Shirt:          new Color( 20, 245,  60),   // green
            ShirtHighlight: new Color(255, 255, 255),   // white
            Buttons:        new Color(  0,   0,   0),   // black
            Accessory:      new Color( 20,  88, 255)),  // blue

        new("blue",
            Skin:           new Color(255, 106,  60),   // orange
            Shirt:          new Color( 20,  88, 255),   // blue
            ShirtHighlight: new Color(255, 255, 255),   // white
            Buttons:        new Color(  0,   0,   0),   // black
            Accessory:      new Color(255, 106,  60)),  // orange

        new("violet",
            Skin:           new Color(255, 106,  60),   // orange
            Shirt:          new Color(193,  28, 255),   // violet
            ShirtHighlight: new Color(255, 255, 255),   // white
            Buttons:        new Color(  0,   0,   0),   // black
            Accessory:      new Color( 20, 245,  60)),  // green

        new("orange",
            Skin:           new Color(255, 255, 255),   // white
            Shirt:          new Color(255, 106,  60),   // orange
            ShirtHighlight: new Color(255, 255, 255),   // white
            Buttons:        new Color(  0,   0,   0),   // black
            Accessory:      new Color( 20,  88, 255)),  // blue

        new("white",
            Skin:           new Color(255, 106,  60),   // orange
            Shirt:          new Color(255, 255, 255),   // white
            ShirtHighlight: new Color( 20,  88, 255),   // blue (distinguishable highlight)
            Buttons:        new Color(  0,   0,   0),   // black
            Accessory:      new Color( 20, 245,  60)),  // green
    ];

    // ── Legs palettes ─────────────────────────────────────────────────────────

    public override LegsPalette[] LegsPalettes { get; } =
    [
        new("blue pants",
            Skin:           new Color(255, 106,  60),   // orange
            Pants:          new Color( 20,  88, 255),   // blue
            PantsHighlight: new Color(255, 255, 255),   // white
            Belt:           new Color(  0,   0,   0),   // black
            BeltHighlight:  new Color(255, 255, 255),   // white
            Shoes:          new Color(  0,   0,   0),   // black
            ShoeHighlight:  new Color(255, 106,  60)),  // orange

        new("violet pants",
            Skin:           new Color(255, 106,  60),   // orange
            Pants:          new Color(193,  28, 255),   // violet
            PantsHighlight: new Color(255, 255, 255),   // white
            Belt:           new Color(  0,   0,   0),   // black
            BeltHighlight:  new Color(255, 106,  60),   // orange
            Shoes:          new Color(  0,   0,   0),   // black
            ShoeHighlight:  new Color(255, 255, 255)),  // white

        new("black pants",
            Skin:           new Color(255, 106,  60),   // orange
            Pants:          new Color(  0,   0,   0),   // black
            PantsHighlight: new Color( 20,  88, 255),   // blue
            Belt:           new Color(255, 106,  60),   // orange
            BeltHighlight:  new Color(255, 255, 255),   // white
            Shoes:          new Color(  0,   0,   0),   // black
            ShoeHighlight:  new Color(255, 255, 255)),  // white

        new("green pants",
            Skin:           new Color(255, 106,  60),   // orange
            Pants:          new Color( 20, 245,  60),   // green
            PantsHighlight: new Color(255, 255, 255),   // white
            Belt:           new Color(  0,   0,   0),   // black
            BeltHighlight:  new Color(255, 106,  60),   // orange
            Shoes:          new Color(  0,   0,   0),   // black
            ShoeHighlight:  new Color(255, 255, 255)),  // white
    ];
}
