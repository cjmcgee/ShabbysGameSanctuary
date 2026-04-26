using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.AppleII;

/// <summary>
/// Apple II visual style.
///
/// Authentic characteristics:
///   • 7×8 character-cell tile art (40-column × 24-row text mode: 7px wide × 8px tall)
///   • Hi-res artifact color palette: black, white, green, violet, orange, blue
///     (colors arise from 1-MHz pixel clock and NTSC artifact coloring)
///   • Iconic phosphor-green monochrome aesthetic with occasional color fringing
///   • Character sprites: 7×16 (tall blocky forms in the text-resolution style)
///   • Camera zoom 1.5×: approximates the Apple II's low screen resolution feel
/// </summary>
public sealed class AppleIISystem : RetroSystem
{
    public override string Name        => "Apple II";
    public override string Description => "7×8 char-cells · Hi-res artifact colors";
    public override int    NativeTileSize => 8;  // height reference; width is 7
    public override float  DisplayScale   => 1.5f;

    // ── Tile palette ──────────────────────────────────────────────────────────
    // Apple II hi-res artifact palette (6 usable colors + black bg)
    // Index 0  = black background
    // Index 1  = green (the iconic phosphor-green)
    // Index 2  = violet / magenta (left-shift artifact)
    // Index 3  = white (lit pixel clusters)
    // Index 4  = orange (right-shift artifact)
    // Index 5  = medium blue (NTSC artifact)
    // Index 6  = dark green (shadow)
    // Index 7  = near-black (structural dark)
    // Index 8  = light gray (near-white fade)
    // Index 9  = amber (warm tone for wood)
    // Index 10 = brown (darker wood)
    protected override Color[] TilePalette { get; } =
    [
        new Color(  0,   0,   0),   //  0 black
        new Color( 20, 245,  60),   //  1 phosphor green
        new Color(193,  28, 255),   //  2 violet/magenta
        new Color(255, 255, 255),   //  3 white
        new Color(255, 106,  60),   //  4 orange
        new Color( 20,  88, 255),   //  5 medium blue
        new Color(  0, 100,   0),   //  6 dark green
        new Color( 20,  20,  20),   //  7 near-black
        new Color(192, 192, 192),   //  8 light gray
        new Color(180, 130,  40),   //  9 amber (wood warm)
        new Color(100,  55,   5),   // 10 dark brown (wood grain)
        new Color(140, 220,  80),   // 11 pale green (kitchen)
        new Color(160, 160, 160),   // 12 mid gray (sidewalk)
        new Color( 30,  30,  40),   // 13 dark asphalt
        new Color( 80, 160,  80),   // 14 muted grass green
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
        TileType.HouseExterior => Wall,
        TileType.Accent       => Accent,
        _ => Wall
    };

    // Apple II tiles are 7 wide × 8 tall (character cell proportions)
    // The base BuildTileset handles the non-square upscale automatically.

    // Wood floor — amber planks with brown grain lines (warm, earthy)
    private static readonly byte[][] WoodFloor =
    [
        [  9,  9,  9,  9, 10,  9,  9 ],
        [  9,  9, 10,  9,  9,  9, 10 ],
        [ 10, 10, 10, 10, 10, 10, 10 ],
        [  9, 10,  9,  9, 10,  9,  9 ],
        [  9,  9,  9,  9, 10,  9,  9 ],
        [  9,  9, 10,  9,  9,  9, 10 ],
        [ 10, 10, 10, 10, 10, 10, 10 ],
        [  9, 10,  9,  9, 10,  9,  9 ],
    ];

    // Carpet — green phosphor with violet artifact border (Apple II dither pattern)
    private static readonly byte[][] Carpet =
    [
        [  1,  1,  1,  1,  1,  1,  1 ],
        [  1,  2,  1,  1,  1,  2,  1 ],
        [  1,  1,  1,  1,  1,  1,  1 ],
        [  1,  1,  2,  1,  2,  1,  1 ],
        [  1,  1,  2,  1,  2,  1,  1 ],
        [  1,  1,  1,  1,  1,  1,  1 ],
        [  1,  2,  1,  1,  1,  2,  1 ],
        [  1,  1,  1,  1,  1,  1,  1 ],
    ];

    // Kitchen — white tiles with black grout (Apple II lo-res style)
    private static readonly byte[][] KitchenTile =
    [
        [  3,  3,  7,  3,  3,  3,  7 ],
        [  3,  3,  7,  3,  3,  3,  7 ],
        [  3,  3,  7,  3,  3,  3,  7 ],
        [  7,  7,  7,  7,  7,  7,  7 ],
        [  3,  3,  7,  3,  3,  3,  7 ],
        [  3,  3,  7,  3,  3,  3,  7 ],
        [  3,  3,  7,  3,  3,  3,  7 ],
        [  7,  7,  7,  7,  7,  7,  7 ],
    ];

    // Wall — white with green phosphor accent lines (Apple II screen style)
    private static readonly byte[][] Wall =
    [
        [  3,  3,  3,  3,  3,  3,  3 ],
        [  3,  8,  3,  3,  3,  8,  3 ],
        [  3,  3,  3,  3,  3,  3,  3 ],
        [  3,  3,  3,  3,  3,  3,  3 ],
        [  8,  3,  3,  3,  3,  3,  8 ],
        [  3,  3,  3,  3,  3,  3,  3 ],
        [  3,  3,  3,  3,  3,  3,  3 ],
        [  8,  8,  8,  8,  8,  8,  8 ],
    ];

    // Door — orange artifact color framing (distinctive Apple II look)
    private static readonly byte[][] Door =
    [
        [  7,  7,  7,  7,  7,  7,  7 ],
        [  7, 10,  7,  7,  7, 10,  7 ],
        [  7, 10,  7,  7,  7, 10,  7 ],
        [  7,  7,  7,  7,  7,  7,  7 ],
        [  7, 10, 10, 10, 10, 10,  7 ],
        [  7, 10,  4, 10, 10, 10,  7 ],  // doorknob in orange
        [  7, 10, 10, 10, 10, 10,  7 ],
        [  7,  7,  7,  7,  7,  7,  7 ],
    ];

    // Window — blue artifact glass panes (Apple II blue artifact color)
    private static readonly byte[][] Window =
    [
        [  8,  8,  8,  8,  8,  8,  8 ],
        [  8,  5,  5,  8,  5,  5,  8 ],
        [  8,  5,  5,  8,  5,  5,  8 ],
        [  8,  5,  5,  8,  5,  5,  8 ],
        [  8,  8,  8,  8,  8,  8,  8 ],
        [  8,  5,  5,  5,  5,  5,  8 ],
        [  8,  5,  5,  5,  5,  5,  8 ],
        [  8,  8,  8,  8,  8,  8,  8 ],
    ];

    // Furniture — blue block (blue artifact)
    private static readonly byte[][] Furniture =
    [
        [  5,  5,  5,  5,  5,  5,  5 ],
        [  5,  7,  7,  7,  7,  7,  5 ],
        [  5,  7,  5,  5,  5,  7,  5 ],
        [  5,  7,  5,  5,  5,  7,  5 ],
        [  5,  7,  5,  5,  5,  7,  5 ],
        [  5,  7,  7,  7,  7,  7,  5 ],
        [  5,  5,  5,  5,  5,  5,  5 ],
        [  5,  5,  5,  5,  5,  5,  5 ],
    ];

    // Counter — white/gray
    private static readonly byte[][] Counter =
    [
        [  8,  8,  8,  8,  8,  8,  8 ],
        [  8,  3,  3,  3,  3,  3,  8 ],
        [  8,  3,  8,  8,  8,  3,  8 ],
        [  8,  3,  8,  8,  8,  3,  8 ],
        [  8,  3,  8,  8,  8,  3,  8 ],
        [  8,  3,  3,  3,  3,  3,  8 ],
        [  8,  8,  8,  8,  8,  8,  8 ],
        [ 12, 12, 12, 12, 12, 12, 12 ],
    ];

    // Bookshelf — green phosphor spine slots
    private static readonly byte[][] Bookshelf =
    [
        [ 10, 10, 10, 10, 10, 10, 10 ],
        [  1,  7,  1,  7,  1,  7,  1 ],
        [  1,  7,  1,  7,  1,  7,  1 ],
        [  1,  7,  1,  7,  1,  7,  1 ],
        [  1,  7,  1,  7,  1,  7,  1 ],
        [  1,  7,  1,  7,  1,  7,  1 ],
        [ 10, 10, 10, 10, 10, 10, 10 ],
        [ 10, 10, 10, 10, 10, 10, 10 ],
    ];

    // Plant — green phosphor leaves
    private static readonly byte[][] Plant =
    [
        [  0,  0,  1,  6,  1,  0,  0 ],
        [  0,  1,  1,  1,  1,  1,  0 ],
        [  0,  0,  1,  1,  1,  0,  0 ],
        [  0,  0,  0,  1,  0,  0,  0 ],
        [  0,  0, 10, 10, 10,  0,  0 ],
        [  0,  0, 10,  9, 10,  0,  0 ],
        [  0,  0, 10, 10, 10,  0,  0 ],
        [  0,  0,  0,  0,  0,  0,  0 ],
    ];

    // Grass — phosphor green (Apple II's iconic color)
    private static readonly byte[][] Grass =
    [
        [ 14, 14, 14, 14, 14, 14, 14 ],
        [ 14,  1, 14, 14, 14,  1, 14 ],
        [ 14,  1, 14, 14,  1, 14, 14 ],
        [ 14, 14, 14, 14, 14, 14, 14 ],
        [ 14, 14, 14, 14, 14, 14, 14 ],
        [ 14,  1, 14, 14, 14,  1, 14 ],
        [ 14,  1, 14, 14,  1, 14, 14 ],
        [ 14, 14, 14, 14, 14, 14, 14 ],
    ];

    // Road — near-black with white center dashes
    private static readonly byte[][] Road =
    [
        [ 13, 13, 13, 13, 13, 13, 13 ],
        [ 13, 13, 13, 13, 13, 13, 13 ],
        [ 13, 13, 13, 13, 13, 13, 13 ],
        [  3,  3,  0,  0,  0,  3,  3 ],   // dashed white center line
        [  3,  3,  0,  0,  0,  3,  3 ],
        [ 13, 13, 13, 13, 13, 13, 13 ],
        [ 13, 13, 13, 13, 13, 13, 13 ],
        [ 13, 13, 13, 13, 13, 13, 13 ],
    ];

    private static readonly byte[][] Sidewalk =
    [
        [ 12, 12, 12, 12, 12, 12, 12 ],
        [ 12,  8, 12, 12,  8, 12, 12 ],
        [ 12, 12, 12, 12, 12, 12, 12 ],
        [ 12, 12, 12, 12, 12, 12, 12 ],
        [ 12, 12, 12, 12, 12, 12, 12 ],
        [ 12,  8, 12, 12,  8, 12, 12 ],
        [ 12, 12, 12, 12, 12, 12, 12 ],
        [ 12, 12, 12, 12, 12, 12, 12 ],
    ];

    private static readonly byte[][] Accent =
    [
        [ 15, 15, 15, 15, 15, 15, 15 ],
        [ 15,  8, 15, 15,  8, 15, 15 ],
        [ 15, 15, 15, 15, 15, 15, 15 ],
        [ 15,  8, 15, 15,  8, 15, 15 ],
        [ 15, 15, 15, 15, 15, 15, 15 ],
        [ 15,  8, 15, 15,  8, 15, 15 ],
        [ 15, 15, 15, 15, 15, 15, 15 ],
        [ 15,  8, 15, 15,  8, 15, 15 ],
    ];

    // ── Character sprite (7×16 — tall, skinny Apple II text-mode figure) ──────
    // Phosphor green body with white highlight and dark shadow

    public override int CharWidth  => 7;
    public override int CharHeight => 16;

    protected override byte[][][] CharFrames { get; } =
    [
        // Frame 0 — idle (phosphor-green silhouette, white highlight)
        [
            [ 0, 0, 1, 1, 1, 0, 0 ],   // head
            [ 0, 1, 1, 3, 1, 1, 0 ],
            [ 0, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 0 ],   // shoulders
            [ 1, 1, 1, 1, 1, 1, 1 ],   // body
            [ 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1 ],
            [ 2, 1, 1, 1, 1, 1, 2 ],   // body shadow sides
            [ 0, 2, 1, 1, 1, 2, 0 ],
            [ 0, 0, 1, 1, 1, 0, 0 ],   // hips
            [ 0, 1, 1, 0, 1, 1, 0 ],   // legs
            [ 0, 1, 1, 0, 1, 1, 0 ],
            [ 0, 1, 1, 0, 1, 1, 0 ],
            [ 0, 1, 1, 0, 1, 1, 0 ],
            [ 0, 2, 1, 0, 1, 2, 0 ],
        ],
        // Frame 1 — walk A (left leg forward)
        [
            [ 0, 0, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 3, 1, 1, 0 ],
            [ 0, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1 ],
            [ 2, 1, 1, 1, 1, 1, 2 ],
            [ 0, 2, 1, 1, 1, 2, 0 ],
            [ 0, 0, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 0, 0, 1, 0 ],   // left wide, right narrow
            [ 1, 1, 0, 0, 0, 1, 0 ],
            [ 1, 1, 0, 0, 0, 1, 1 ],
            [ 2, 0, 0, 0, 0, 1, 1 ],
            [ 0, 0, 0, 0, 0, 2, 1 ],
        ],
        // Frame 2 — mid-stride
        [
            [ 0, 0, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 3, 1, 1, 0 ],
            [ 0, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1 ],
            [ 2, 1, 1, 1, 1, 1, 2 ],
            [ 0, 2, 1, 1, 1, 2, 0 ],
            [ 0, 0, 1, 1, 1, 0, 0 ],
            [ 0, 1, 0, 0, 0, 1, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0 ],
        ],
        // Frame 3 — walk B (right leg forward)
        [
            [ 0, 0, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 3, 1, 1, 0 ],
            [ 0, 1, 1, 1, 1, 1, 0 ],
            [ 0, 0, 1, 1, 1, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 0 ],
            [ 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1 ],
            [ 1, 1, 1, 1, 1, 1, 1 ],
            [ 2, 1, 1, 1, 1, 1, 2 ],
            [ 0, 2, 1, 1, 1, 2, 0 ],
            [ 0, 0, 1, 1, 1, 0, 0 ],
            [ 0, 1, 0, 0, 1, 1, 0 ],   // right wide, left narrow
            [ 0, 1, 0, 0, 0, 1, 1 ],
            [ 0, 1, 1, 0, 0, 1, 1 ],
            [ 0, 1, 1, 0, 0, 0, 2 ],
            [ 0, 1, 2, 0, 0, 0, 0 ],
        ],
    ];
}
