using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.MSDOSCGA;

/// <summary>
/// MS-DOS CGA visual style.
///
/// Authentic characteristics:
///   • 8×8 tiles at 320×200 (Mode 4) native resolution
///   • CGA Palette 1 High: Black + Cyan + Magenta + White (the classic 4-color look)
///   • High contrast, dithering-heavy — dither patterns approximate mid-tones
///   • Character sprites: 8×16 (EGA/CGA compatible sprite height)
///   • Camera zoom 2×: faithful to 320×200 scaled to a modern display
///
/// Note: only 4 colors are available across the entire scene,
/// so everything uses strategic dithering to imply depth.
/// </summary>
public sealed class CGASystem : RetroSystem
{
    public override string Name        => "MS-DOS CGA";
    public override string Description => "8×8 tiles · 4-color palette (Mode 4 Palette 1)";
    public override int    NativeTileSize => 8;
    public override float  DisplayScale   => 2.0f;

    // ── CGA Palette 1 High (the iconic "cyan/magenta/white/black" set) ────────
    // Index 0 = black (background)
    // Index 1 = cyan
    // Index 2 = magenta
    // Index 3 = white
    // Index 4 = black (used as "very dark" in patterns — same as 0 but distinct intent)
    // Index 5-14 = extended for house exterior variety using dithered approximations
    //   (CGA Mode 4 is 4-color only; we allow 4 base + mapped extras for the engine)
    //   The "extras" are dithered tiles that combine the 4 CGA colors visually.
    protected override Color[] TilePalette { get; } =
    [
        new Color(  0,   0,   0),   //  0 black
        new Color(  0, 170, 170),   //  1 CGA cyan
        new Color(170,   0, 170),   //  2 CGA magenta
        new Color(170, 170, 170),   //  3 CGA light gray (used as "white" here)
        new Color(255, 255, 255),   //  4 white (bright white)
        new Color( 85,  85,  85),   //  5 dark gray (BG for road)
        new Color(  0,  85,  85),   //  6 dark cyan
        new Color( 85,   0,  85),   //  7 dark magenta
        new Color(255,  85,  85),   //  8 light red (for bookshelf)
        new Color( 85, 255,  85),   //  9 light green (for grass/plant)
        new Color(255, 255,  85),   // 10 yellow (for kitchen)
        new Color(255,  85, 255),   // 11 light magenta
        new Color( 85, 255, 255),   // 12 light cyan
        new Color(  0,   0, 170),   // 13 dark blue
        new Color(170,  85,   0),   // 14 brown
    ];

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
        TileType.HouseExterior => HouseExterior,
        TileType.Accent       => Accent,
        _ => Wall
    };

    // CGA uses dithering (checkerboard) to approximate brown/orange with cyan+magenta+black

    // Wood floor — dithered brown approximation (black + magenta checkerboard = dark purple-ish)
    // CGA doesn't have brown; use magenta/black dither for wood, white lines for grain
    private static readonly byte[][] WoodFloor =
    [
        [ 2, 0, 2, 0, 3, 0, 2, 0 ],
        [ 0, 2, 0, 2, 0, 2, 0, 2 ],
        [ 3, 0, 2, 0, 2, 0, 3, 0 ],
        [ 0, 2, 0, 2, 0, 2, 0, 2 ],
        [ 2, 0, 3, 0, 2, 0, 2, 0 ],
        [ 0, 2, 0, 2, 0, 2, 0, 2 ],
        [ 3, 0, 2, 0, 3, 0, 2, 0 ],
        [ 0, 2, 0, 2, 0, 2, 0, 2 ],
    ];

    // Carpet — solid magenta (the closest CGA has to red)
    private static readonly byte[][] Carpet =
    [
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 0, 2, 2, 2, 0, 2, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 2, 0, 2, 0, 2, 2, 2 ],
        [ 2, 2, 0, 2, 0, 2, 2, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 0, 2, 2, 2, 0, 2, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
    ];

    // Kitchen — white tile with cyan grout (CGA grid)
    private static readonly byte[][] KitchenTile =
    [
        [ 4, 4, 1, 4, 4, 4, 1, 4 ],
        [ 4, 4, 1, 4, 4, 4, 1, 4 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 4, 4, 1, 4, 4, 4, 1, 4 ],
        [ 4, 4, 1, 4, 4, 4, 1, 4 ],
        [ 4, 4, 1, 4, 4, 4, 1, 4 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 4, 4, 1, 4, 4, 4, 1, 4 ],
    ];

    // Wall — white with gray dithered shadow band (CGA-style wall texture)
    private static readonly byte[][] Wall =
    [
        [ 4, 4, 4, 4, 4, 4, 4, 4 ],
        [ 4, 3, 4, 4, 4, 3, 4, 4 ],
        [ 4, 4, 4, 4, 4, 4, 4, 4 ],
        [ 4, 4, 4, 4, 4, 4, 4, 4 ],
        [ 3, 4, 4, 4, 4, 4, 4, 3 ],
        [ 4, 4, 4, 4, 4, 4, 4, 4 ],
        [ 4, 4, 4, 4, 4, 4, 4, 4 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
    ];

    // Door — black frame with magenta panels
    private static readonly byte[][] Door =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 2, 2, 0, 0, 2, 2, 0 ],
        [ 0, 2, 2, 0, 0, 2, 2, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 2, 2, 2, 2, 2, 2, 0 ],
        [ 0, 2, 2, 4, 2, 2, 2, 0 ],  // white doorknob
        [ 0, 2, 2, 2, 2, 2, 2, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // Window — cyan panes (the CGA cyan for glass)
    private static readonly byte[][] Window =
    [
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 1, 1, 3, 1, 1, 1, 3 ],
        [ 3, 1, 1, 3, 1, 1, 1, 3 ],
        [ 3, 1, 1, 3, 1, 1, 1, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 1, 1, 1, 1, 1, 1, 3 ],
        [ 3, 1, 1, 1, 1, 1, 1, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
    ];

    // Furniture — cyan block (blue not available, cyan is the closest)
    private static readonly byte[][] Furniture =
    [
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 4, 4, 4, 4, 4, 4, 1 ],
        [ 1, 4, 1, 1, 1, 1, 4, 1 ],
        [ 1, 4, 1, 1, 1, 1, 4, 1 ],
        [ 1, 4, 1, 1, 1, 1, 4, 1 ],
        [ 1, 4, 4, 4, 4, 4, 4, 1 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
    ];

    // Counter — light gray with white top
    private static readonly byte[][] Counter =
    [
        [ 4, 4, 4, 4, 4, 4, 4, 4 ],
        [ 4, 3, 3, 3, 3, 3, 3, 4 ],
        [ 4, 3, 4, 4, 4, 4, 3, 4 ],
        [ 4, 3, 4, 4, 4, 4, 3, 4 ],
        [ 4, 3, 4, 4, 4, 4, 3, 4 ],
        [ 4, 3, 3, 3, 3, 3, 3, 4 ],
        [ 4, 4, 4, 4, 4, 4, 4, 4 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
    ];

    // Bookshelf — magenta spines (CGA closest to red/brown)
    private static readonly byte[][] Bookshelf =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 2, 4, 1, 2, 4, 1, 2, 4 ],
        [ 2, 4, 1, 2, 4, 1, 2, 4 ],
        [ 2, 4, 1, 2, 4, 1, 2, 4 ],
        [ 2, 4, 1, 2, 4, 1, 2, 4 ],
        [ 2, 4, 1, 2, 4, 1, 2, 4 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // Plant — cyan cross shape (CGA has no green; cyan is closest)
    private static readonly byte[][] Plant =
    [
        [ 0, 0, 1, 0, 1, 0, 0, 0 ],
        [ 0, 1, 1, 1, 1, 1, 0, 0 ],
        [ 0, 0, 1, 1, 1, 0, 0, 0 ],
        [ 0, 0, 0, 1, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 2, 3, 2, 0, 0, 0 ],  // magenta pot, white highlight
        [ 0, 0, 2, 2, 2, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // Grass — CGA dithered green approximation (cyan + black dither)
    private static readonly byte[][] Grass =
    [
        [ 1, 0, 1, 0, 1, 0, 1, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1 ],
        [ 1, 0, 1, 0, 1, 0, 1, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1 ],
        [ 1, 0, 1, 0, 1, 0, 1, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1 ],
        [ 1, 0, 1, 0, 1, 0, 1, 0 ],
        [ 0, 1, 0, 1, 0, 1, 0, 1 ],
    ];

    // Road — dark gray (CGA dark gray dithered)
    private static readonly byte[][] Road =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 4, 4, 0, 0, 0, 4, 4, 4 ],   // white center dashes
        [ 4, 4, 0, 0, 0, 4, 4, 4 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // Sidewalk — light gray (white + black dither)
    private static readonly byte[][] Sidewalk =
    [
        [ 4, 3, 4, 3, 4, 3, 4, 3 ],
        [ 3, 4, 3, 4, 3, 4, 3, 4 ],
        [ 4, 3, 4, 3, 4, 3, 4, 3 ],
        [ 3, 4, 3, 4, 3, 4, 3, 4 ],
        [ 4, 3, 4, 3, 4, 3, 4, 3 ],
        [ 3, 4, 3, 4, 3, 4, 3, 4 ],
        [ 4, 3, 4, 3, 4, 3, 4, 3 ],
        [ 3, 4, 3, 4, 3, 4, 3, 4 ],
    ];

    // House exterior — solid cyan (CGA house color)
    private static readonly byte[][] HouseExterior =
    [
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
    ];

    // Accent — dithered magenta/white (closest CGA has to warm colors)
    private static readonly byte[][] Accent =
    [
        [ 2, 4, 2, 4, 2, 4, 2, 4 ],
        [ 4, 2, 4, 2, 4, 2, 4, 2 ],
        [ 2, 4, 2, 4, 2, 4, 2, 4 ],
        [ 4, 2, 4, 2, 4, 2, 4, 2 ],
        [ 2, 4, 2, 4, 2, 4, 2, 4 ],
        [ 4, 2, 4, 2, 4, 2, 4, 2 ],
        [ 2, 4, 2, 4, 2, 4, 2, 4 ],
        [ 4, 2, 4, 2, 4, 2, 4, 2 ],
    ];

    // ── Character sprite (8×16, CGA PC-speaker game style) ───────────────────
    // Only 4 colors; uses white highlights and cyan body

    public override int CharWidth  => 8;
    public override int CharHeight => 16;

    protected override byte[][][] CharFrames { get; } =
    [
        // Frame 0 — idle
        [
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // head (cyan)
            [ 0, 1, 1, 4, 1, 4, 1, 0 ],   // eyes (white)
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],   // shoulder
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],   // body
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 4, 1, 1, 1, 1, 4, 1 ],   // arms/highlight
            [ 0, 4, 1, 1, 1, 1, 4, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],
            [ 0, 4, 1, 0, 0, 1, 4, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        // Frame 1 — walk A
        [
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 4, 1, 4, 1, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 4, 1, 1, 1, 1, 4, 1 ],
            [ 0, 4, 1, 1, 1, 1, 4, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 1, 1, 0, 0, 0, 0, 0 ],
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],
            [ 0, 4, 1, 0, 0, 1, 4, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        // Frame 2 — mid-stride
        [
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 4, 1, 4, 1, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 4, 1, 1, 1, 1, 4, 1 ],
            [ 0, 4, 1, 1, 1, 1, 4, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 0, 0, 0, 0, 1, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        // Frame 3 — walk B
        [
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 4, 1, 4, 1, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 4, 1, 1, 1, 1, 4, 1 ],
            [ 0, 4, 1, 1, 1, 1, 4, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 0, 0, 0, 1, 1, 0 ],
            [ 0, 0, 0, 0, 0, 1, 1, 0 ],
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],
            [ 0, 1, 4, 0, 0, 4, 1, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
    ];
}
