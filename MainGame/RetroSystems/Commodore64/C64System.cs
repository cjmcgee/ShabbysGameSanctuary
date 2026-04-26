using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.Commodore64;

/// <summary>
/// Commodore 64 visual style.
///
/// Authentic characteristics:
///   • 8×8 character-cell tiles — the C64's text/PETSCII tile unit
///   • VIC-II 16-color palette with distinctive color relationships
///   • Character-mode graphics: each 8×8 cell typically uses 2-3 colors
///   • Character sprites: 12×21 double-width pixels (C64 multi-color sprite)
///   • Camera zoom 1.5×: faithful to the C64's 320×200 native resolution feel
/// </summary>
public sealed class C64System : RetroSystem
{
    public override string Name        => "Commodore 64";
    public override string Description => "8×8 char-cells · VIC-II 16-color palette";
    public override int    NativeTileSize => 8;
    public override float  DisplayScale   => 1.5f;

    // ── Tile palette (VIC-II colors) ─────────────────────────────────────────
    // Index 0  = black background
    // Indices 1-14 drawn from the C64 color palette (Palettes.cs C64Palette values)
    protected override Color[] TilePalette { get; } =
    [
        new Color(  0,   0,   0),   //  0 black       (bg)
        new Color(132,  96,  60),   //  1 brown        — wood floor
        new Color( 72,  40,   0),   //  2 dark brown   — wood grain
        new Color(245,  12,   0),   //  3 C64 red      — carpet
        new Color(140,   0,   0),   //  4 dark red     — carpet border
        new Color(191, 235,   0),   //  5 C64 yellow   — kitchen tile
        new Color(255, 255, 255),   //  6 white        — wall light
        new Color(120, 120, 120),   //  7 C64 grey     — wall mortar
        new Color( 64,  20, 255),   //  8 C64 blue     — door frame
        new Color(  0,   0,   0),   //  9 black        — door panel dark
        new Color(172, 172, 172),   // 10 C64 lt grey  — counter
        new Color(  0, 227, 246),   // 11 C64 cyan     — window / plant accent
        new Color(  1, 223,   0),   // 12 C64 green    — grass
        new Color( 40,  40,  40),   // 13 near-black   — road
        new Color( 80,  80,  80),   // 14 C64 dark grey — sidewalk
    ];

    // ── Tile pixel art (8×8) ─────────────────────────────────────────────────

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

    // C64 character-style art uses PETSCII-inspired patterns
    // Each tile uses 2-3 palette colors

    private static readonly byte[][] WoodFloor =
    [
        [ 1, 1, 1, 2, 1, 1, 1, 2 ],
        [ 1, 1, 2, 1, 1, 1, 2, 1 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 1, 2, 1, 1, 2, 1, 1, 2 ],
        [ 1, 1, 1, 2, 1, 1, 1, 2 ],
        [ 1, 1, 2, 1, 1, 1, 2, 1 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 1, 2, 1, 1, 2, 1, 1, 2 ],
    ];

    private static readonly byte[][] Carpet =
    [
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 4, 4, 3, 3, 4, 4, 3 ],
        [ 3, 4, 3, 3, 3, 3, 4, 3 ],
        [ 3, 3, 3, 4, 4, 3, 3, 3 ],
        [ 3, 3, 3, 4, 4, 3, 3, 3 ],
        [ 3, 4, 3, 3, 3, 3, 4, 3 ],
        [ 3, 4, 4, 3, 3, 4, 4, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
    ];

    // Classic C64 checkered kitchen
    private static readonly byte[][] KitchenTile =
    [
        [ 5, 5, 5, 7, 5, 5, 5, 7 ],
        [ 5, 5, 5, 7, 5, 5, 5, 7 ],
        [ 5, 5, 5, 7, 5, 5, 5, 7 ],
        [ 7, 7, 7, 7, 7, 7, 7, 7 ],
        [ 5, 5, 5, 7, 5, 5, 5, 7 ],
        [ 5, 5, 5, 7, 5, 5, 5, 7 ],
        [ 5, 5, 5, 7, 5, 5, 5, 7 ],
        [ 7, 7, 7, 7, 7, 7, 7, 7 ],
    ];

    // Wall with C64 PETSCII-style block shading
    private static readonly byte[][] Wall =
    [
        [ 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 7, 6, 6, 6, 6, 7, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 7, 6, 6, 6, 6, 6, 6, 7 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 7, 7, 7, 7, 7, 7, 7, 7 ],
    ];

    // C64-style door (blue frame, dark panels — typical C64 game door)
    private static readonly byte[][] Door =
    [
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
        [ 8, 9, 9, 8, 8, 9, 9, 8 ],
        [ 8, 9, 9, 8, 8, 9, 9, 8 ],
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
        [ 8, 9, 9, 9, 9, 9, 9, 8 ],
        [ 8, 9, 9, 9, 9, 9, 9, 8 ],
        [ 8, 9, 8, 9, 9, 8, 9, 8 ],
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
    ];

    // Cyan window (cyan is the C64's iconic accent color)
    private static readonly byte[][] Window =
    [
        [ 7, 7, 7, 7, 7, 7, 7, 7 ],
        [ 7,11,11, 7,11,11,11, 7 ],
        [ 7,11,11, 7,11,11,11, 7 ],
        [ 7,11,11, 7,11,11,11, 7 ],
        [ 7, 7, 7, 7, 7, 7, 7, 7 ],
        [ 7,11,11,11,11,11,11, 7 ],
        [ 7,11,11,11,11,11,11, 7 ],
        [ 7, 7, 7, 7, 7, 7, 7, 7 ],
    ];

    // C64 blue furniture (iconic color)
    private static readonly byte[][] Furniture =
    [
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
        [ 8, 6, 6, 6, 6, 6, 6, 8 ],
        [ 8, 6, 9, 9, 9, 9, 6, 8 ],
        [ 8, 6, 9, 8, 8, 9, 6, 8 ],
        [ 8, 6, 9, 8, 8, 9, 6, 8 ],
        [ 8, 6, 9, 9, 9, 9, 6, 8 ],
        [ 8, 6, 6, 6, 6, 6, 6, 8 ],
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
    ];

    // Light grey counter with C64 shading
    private static readonly byte[][] Counter =
    [
        [10,10,10,10,10,10,10,10 ],
        [10, 6, 6, 6, 6, 6, 6,10 ],
        [10, 6,10,10,10,10, 6,10 ],
        [10, 6,10,10,10,10, 6,10 ],
        [10, 6,10,10,10,10, 6,10 ],
        [10, 6, 6, 6, 6, 6, 6,10 ],
        [10,10,10,10,10,10,10,10 ],
        [10, 7, 7, 7, 7, 7, 7,10 ],
    ];

    // Bookshelf with colored spines (C64 multi-color style)
    private static readonly byte[][] Bookshelf =
    [
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 3, 8,12, 3, 8,12, 3, 8 ],
        [ 3, 8,12, 3, 8,12, 3, 8 ],
        [ 3, 8,12, 3, 8,12, 3, 8 ],
        [ 3, 8,12, 3, 8,12, 3, 8 ],
        [ 3, 8,12, 3, 8,12, 3, 8 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
    ];

    // Green plant (C64 green)
    private static readonly byte[][] Plant =
    [
        [ 0, 0,12, 0,12, 0, 0, 0 ],
        [ 0,12,12,12,12,12, 0, 0 ],
        [ 0, 0,12,12,12, 0, 0, 0 ],
        [ 0, 0, 0,12, 0, 0, 0, 0 ],
        [ 0, 0, 2, 2, 2, 0, 0, 0 ],
        [ 0, 0, 2, 1, 2, 0, 0, 0 ],
        [ 0, 0, 2, 2, 2, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    private static readonly byte[][] Grass =
    [
        [12,12,12,12,12,12,12,12 ],
        [12,12, 0,12,12,12, 0,12 ],
        [12, 0,12,12,12, 0,12,12 ],
        [12,12,12,12,12,12,12,12 ],
        [12,12,12,12,12,12,12,12 ],
        [12, 0,12,12,12, 0,12,12 ],
        [12,12, 0,12,12,12, 0,12 ],
        [12,12,12,12,12,12,12,12 ],
    ];

    private static readonly byte[][] Road =
    [
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6 ],   // white dashed center line
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
    ];

    private static readonly byte[][] Sidewalk =
    [
        [14,14,14,14,14,14,14,14 ],
        [14, 7,14,14,14, 7,14,14 ],
        [14,14,14,14,14,14,14,14 ],
        [14,14,14,14,14,14,14,14 ],
        [14,14,14,14,14,14,14,14 ],
        [14, 7,14,14,14, 7,14,14 ],
        [14,14,14,14,14,14,14,14 ],
        [14,14,14,14,14,14,14,14 ],
    ];

    // Accent: index 15 = accentColor (appended by base BuildTileset)
    private static readonly byte[][] Accent =
    [
        [15,15,15,15,15,15,15,15 ],
        [15, 7,15,15,15, 7,15,15 ],
        [15,15,15,15,15,15,15,15 ],
        [15, 7,15,15,15, 7,15,15 ],
        [15,15,15,15,15,15,15,15 ],
        [15, 7,15,15,15, 7,15,15 ],
        [15,15,15,15,15,15,15,15 ],
        [15, 7,15,15,15, 7,15,15 ],
    ];

    // ── Character sprite (12×21, C64 multi-color sprite proportions) ──────────
    // Palette: 1=main body, 2=highlight, 3=shadow

    public override int CharWidth  => 12;
    public override int CharHeight => 21;

    protected override byte[][][] CharFrames { get; } =
    [
        // Frame 0 — idle
        [
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 2, 1, 1, 2, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 1, 2, 1, 1, 1, 1, 1, 1, 2, 1, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 3, 1, 1, 1, 1, 1, 1, 1, 1, 3, 0, 0 ],
            [ 0, 3, 1, 1, 1, 1, 1, 1, 3, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0 ],
            [ 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0 ],
            [ 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0 ],
        ],
        // Frame 1 — walk A
        [
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 2, 1, 1, 2, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 1, 2, 1, 1, 1, 1, 1, 1, 2, 1, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 3, 1, 1, 1, 1, 1, 1, 1, 1, 3, 0, 0 ],
            [ 0, 3, 1, 1, 1, 1, 1, 1, 3, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0 ],  // left stride forward
            [ 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0 ],
            [ 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0 ],
            [ 0, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0 ],
        ],
        // Frame 2 — mid-stride
        [
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 2, 1, 1, 2, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 1, 2, 1, 1, 1, 1, 1, 1, 2, 1, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 3, 1, 1, 1, 1, 1, 1, 1, 1, 3, 0, 0 ],
            [ 0, 3, 1, 1, 1, 1, 1, 1, 3, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0 ],  // crossing
            [ 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        // Frame 3 — walk B (right stride forward, mirror of frame 1)
        [
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 2, 1, 1, 2, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 1, 2, 1, 1, 1, 1, 1, 1, 2, 1, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 3, 1, 1, 1, 1, 1, 1, 1, 1, 3, 0, 0 ],
            [ 0, 3, 1, 1, 1, 1, 1, 1, 3, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0 ],  // right stride forward
            [ 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0 ],
            [ 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0 ],
            [ 0, 0, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0 ],
            [ 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],
        ],
    ];
}
