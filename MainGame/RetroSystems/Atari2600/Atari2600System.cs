using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.Atari2600;

/// <summary>
/// Atari 2600 visual style.
///
/// Authentic characteristics:
///   • 8×8 native tile resolution (upscaled to 16×16 for the engine)
///   • 2-3 colors per tile — scanline hardware allowed very limited simultaneous colors
///   • 128-color NTSC palette; sprites are flat, hard-edged silhouettes
///   • Character sprites: 8×16 (Adventure-duck proportions, single-color fills)
///   • Camera zoom 2×: makes each native pixel appear as a 2×2 block on screen
/// </summary>
public sealed class Atari2600System : RetroSystem
{
    public override string Name        => "Atari 2600";
    public override string Description => "8×8 tiles · NTSC 128-color palette";
    public override int    NativeTileSize => 8;
    public override float  DisplayScale   => 2.0f;

    // ── Tile palette ─────────────────────────────────────────────────────────
    // Index 0  = background fill (black)
    // Indices 1-14 = tile colors drawn from the Atari 2600 NTSC palette
    protected override Color[] TilePalette { get; } =
    [
        new Color(  0,   0,   0),   // 0  black (bg fill)
        new Color(188, 140,  56),   // 1  warm amber       — wood floor main
        new Color(124,  72,   8),   // 2  dark brown       — wood floor grain
        new Color(200,  20,  20),   // 3  vivid red        — carpet
        new Color(132,   4,   4),   // 4  dark red         — carpet shadow / border
        new Color(220, 220,   0),   // 5  bright yellow    — kitchen tile
        new Color(220, 220, 200),   // 6  near-white       — wall plaster
        new Color(148, 148, 132),   // 7  light gray       — wall mortar / shadow
        new Color( 20,  12,   4),   // 8  near-black       — door wood / dark outline
        new Color( 36,  80, 200),   // 9  bold blue        — furniture
        new Color(144, 144, 144),   // 10 mid gray         — counter
        new Color(  0, 200, 220),   // 11 bright cyan      — window glass
        new Color(  0, 188,   0),   // 12 vivid green      — grass
        new Color( 28,  28,  40),   // 13 near-black road  — asphalt
        new Color(104, 104, 104),   // 14 medium gray      — sidewalk
    ];

    // ── Tile pixel art (8×8, palette indices) ────────────────────────────────

    protected override byte[][] GetTilePixels(TileType type, Color accentColor) => type switch
    {
        TileType.WoodFloor => WoodFloor,
        TileType.Carpet    => Carpet,
        TileType.KitchenTile => KitchenTile,
        TileType.Wall      => Wall,
        TileType.Door      => Door,
        TileType.Window    => Window,
        TileType.Furniture => Furniture,
        TileType.Counter   => Counter,
        TileType.Bookshelf => Bookshelf,
        TileType.Plant     => Plant,
        TileType.Grass     => Grass,
        TileType.Road      => Road,
        TileType.Sidewalk  => Sidewalk,
        TileType.HouseExterior => Wall,  // reuse wall art; color varies via tileset
        TileType.Accent    => Accent,
        _ => Wall
    };

    // Warm parquet — alternating amber planks
    private static readonly byte[][] WoodFloor =
    [
        [ 1, 1, 2, 1, 1, 1, 2, 1 ],
        [ 1, 1, 2, 1, 1, 1, 2, 1 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 1, 2, 1, 1, 1, 2, 1, 1 ],
        [ 1, 2, 1, 1, 1, 2, 1, 1 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 1, 1, 1, 2, 1, 1, 1, 2 ],
        [ 1, 1, 1, 2, 1, 1, 1, 2 ],
    ];

    // Bold red carpet with dark corner dots
    private static readonly byte[][] Carpet =
    [
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 4, 3, 3, 3, 3, 4, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 3, 4, 4, 3, 3, 3 ],
        [ 3, 3, 3, 4, 4, 3, 3, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 4, 3, 3, 3, 3, 4, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
    ];

    // Yellow kitchen tile — 4×4 blocks with dark grout line
    private static readonly byte[][] KitchenTile =
    [
        [ 5, 5, 5, 4, 5, 5, 5, 4 ],
        [ 5, 5, 5, 4, 5, 5, 5, 4 ],
        [ 5, 5, 5, 4, 5, 5, 5, 4 ],
        [ 4, 4, 4, 4, 4, 4, 4, 4 ],
        [ 5, 5, 5, 4, 5, 5, 5, 4 ],
        [ 5, 5, 5, 4, 5, 5, 5, 4 ],
        [ 5, 5, 5, 4, 5, 5, 5, 4 ],
        [ 4, 4, 4, 4, 4, 4, 4, 4 ],
    ];

    // Simple brick-pattern wall
    private static readonly byte[][] Wall =
    [
        [ 6, 6, 6, 7, 6, 6, 6, 7 ],
        [ 6, 6, 6, 7, 6, 6, 6, 7 ],
        [ 7, 7, 7, 7, 7, 7, 7, 7 ],
        [ 6, 7, 6, 6, 6, 7, 6, 6 ],
        [ 6, 7, 6, 6, 6, 7, 6, 6 ],
        [ 7, 7, 7, 7, 7, 7, 7, 7 ],
        [ 6, 6, 6, 7, 6, 6, 6, 7 ],
        [ 6, 6, 6, 7, 6, 6, 6, 7 ],
    ];

    // Dark door with raised-panel detail
    private static readonly byte[][] Door =
    [
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
        [ 8, 2, 2, 8, 8, 2, 2, 8 ],
        [ 8, 2, 2, 8, 8, 2, 2, 8 ],
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
        [ 8, 2, 2, 2, 2, 2, 2, 8 ],
        [ 8, 2, 2, 2, 2, 2, 2, 8 ],
        [ 8, 2, 8, 2, 2, 8, 2, 8 ],
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
    ];

    // Cyan window — gray sash, bright glass panes
    private static readonly byte[][] Window =
    [
        [ 7, 7, 7, 7, 7, 7, 7, 7 ],
        [ 7,11,11, 7,11,11, 7, 7 ],
        [ 7,11,11, 7,11,11, 7, 7 ],
        [ 7, 7, 7, 7, 7, 7, 7, 7 ],
        [ 7,11,11,11,11,11, 7, 7 ],
        [ 7,11,11,11,11,11, 7, 7 ],
        [ 7, 7, 7, 7, 7, 7, 7, 7 ],
        [ 7, 7, 7, 7, 7, 7, 7, 7 ],
    ];

    // Bold blue furniture block
    private static readonly byte[][] Furniture =
    [
        [ 9, 9, 9, 9, 9, 9, 9, 9 ],
        [ 9, 9, 9, 9, 9, 9, 9, 9 ],
        [ 9, 8, 8, 8, 8, 8, 8, 9 ],
        [ 9, 8, 9, 9, 9, 9, 8, 9 ],
        [ 9, 8, 9, 9, 9, 9, 8, 9 ],
        [ 9, 8, 8, 8, 8, 8, 8, 9 ],
        [ 9, 9, 9, 9, 9, 9, 9, 9 ],
        [ 9, 9, 9, 9, 9, 9, 9, 9 ],
    ];

    // Light gray counter with border
    private static readonly byte[][] Counter =
    [
        [10,10,10,10,10,10,10,10 ],
        [10, 7, 7, 7, 7, 7, 7,10 ],
        [10, 7,10,10,10,10, 7,10 ],
        [10, 7,10,10,10,10, 7,10 ],
        [10, 7,10,10,10,10, 7,10 ],
        [10, 7, 7, 7, 7, 7, 7,10 ],
        [10,10,10,10,10,10,10,10 ],
        [10,10,10,10,10,10,10,10 ],
    ];

    // Red bookshelf with spine slots
    private static readonly byte[][] Bookshelf =
    [
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 8, 3, 8, 3, 8, 3, 3 ],
        [ 3, 8, 3, 8, 3, 8, 3, 3 ],
        [ 3, 8, 3, 8, 3, 8, 3, 3 ],
        [ 3, 8, 3, 8, 3, 8, 3, 3 ],
        [ 3, 8, 3, 8, 3, 8, 3, 3 ],
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
    ];

    // Green plant with pot
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

    // Vivid green grass
    private static readonly byte[][] Grass =
    [
        [12,12,12,12,12,12,12,12 ],
        [12,12,12,12,12,12,12,12 ],
        [12,12,12,12,12,12,12,12 ],
        [12,12,12,12,12,12,12,12 ],
        [12,12,12,12,12,12,12,12 ],
        [12,12,12,12,12,12,12,12 ],
        [12,12,12,12,12,12,12,12 ],
        [12,12,12,12,12,12,12,12 ],
    ];

    // Near-black asphalt road
    private static readonly byte[][] Road =
    [
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
        [14,14,14,14,14,14,14,14 ],  // center line (lighter)
        [14,14,14,14,14,14,14,14 ],
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
    ];

    // Concrete sidewalk
    private static readonly byte[][] Sidewalk =
    [
        [14,14,14,14,14,14,14,14 ],
        [14,14,14,14,14,14,14,14 ],
        [14, 7,14,14,14, 7,14,14 ],
        [14,14,14,14,14,14,14,14 ],
        [14,14,14,14,14,14,14,14 ],
        [14, 7,14,14,14, 7,14,14 ],
        [14,14,14,14,14,14,14,14 ],
        [14,14,14,14,14,14,14,14 ],
    ];

    // Accent tile: solid fill using runtime accent color.
    // Index 15 = TilePalette.Length (base class appends it from accentColor parameter).
    private static readonly byte[][] Accent =
    [
        [15,15,15,15,15,15,15,15 ],
        [15,15,15,15,15,15,15,15 ],
        [15,15,15,15,15,15,15,15 ],
        [15,15,15,15,15,15,15,15 ],
        [15,15,15,15,15,15,15,15 ],
        [15,15,15,15,15,15,15,15 ],
        [15,15,15,15,15,15,15,15 ],
        [15,15,15,15,15,15,15,15 ],
    ];

    // ── Character sprite (8×16) ───────────────────────────────────────────────
    // Flat Atari Adventure-style silhouette: single-color fills, blocky limbs.
    // Palette: 1=main, 2=highlight, 3=shadow

    public override int CharWidth  => 8;
    public override int CharHeight => 16;

    protected override byte[][][] CharFrames { get; } =
    [
        // Frame 0 — idle / stand
        [
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // head top
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 1, 2, 1, 1, 2, 1, 0 ],   // eyes
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // head bottom
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],   // neck/shoulder
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],   // body
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 3, 1, 1, 1, 1, 1, 1, 3 ],   // body lower (shadow sides)
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // hips
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],   // legs split
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],
        ],
        // Frame 1 — walk A (left leg forward)
        [
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 1, 2, 1, 1, 2, 1, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 3, 1, 1, 1, 1, 1, 1, 3 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 0, 0, 0, 0, 0 ],   // only left leg visible
            [ 0, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],   // right leg shorter
            [ 0, 1, 1, 0, 0, 1, 0, 0 ],
        ],
        // Frame 2 — walk B (both at mid)
        [
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 1, 2, 1, 1, 2, 1, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 3, 1, 1, 1, 1, 1, 1, 3 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],
            [ 0, 1, 0, 0, 0, 0, 1, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        // Frame 3 — walk C (right leg forward)
        [
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 1, 2, 1, 1, 2, 1, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 3, 1, 1, 1, 1, 1, 1, 3 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 0, 0, 0, 1, 1, 0 ],   // only right leg
            [ 0, 0, 0, 0, 0, 1, 1, 0 ],
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],   // left leg shorter
            [ 0, 0, 1, 0, 0, 1, 1, 0 ],
        ],
    ];
}
