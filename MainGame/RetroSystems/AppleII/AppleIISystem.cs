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
///   • Character sprites: 7×16 (HeadRows=4, BodyRows=7, LegsRows=5)
///   • Native screen: 280×192. DisplayScale 3.125 shows exactly 192 world pixels tall at a 600px viewport
///   • MaxZoomOutArea (560×384) limits zoom-out to 2× native in each direction
/// </summary>
public sealed class AppleIISystem : RetroSystem
{
    public override string Name        => "Apple II";
    public override string Description => "7×8 char-cells · 280×192 · Hi-res artifact colors";
    public override int    NativeTileSize => 8;
    public override float  DisplayScale   => 3.125f;

    // Native Apple II Hi-res: 280×192. MaxZoomOutArea caps at 2× native in each direction.
    public override Vector2? MaxZoomOutArea => new Vector2(560, 384);

    // ── Tile palette ──────────────────────────────────────────────────────────
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

    private static readonly byte[][] Door =
    [
        [  7,  7,  7,  7,  7,  7,  7 ],
        [  7, 10,  7,  7,  7, 10,  7 ],
        [  7, 10,  7,  7,  7, 10,  7 ],
        [  7,  7,  7,  7,  7,  7,  7 ],
        [  7, 10, 10, 10, 10, 10,  7 ],
        [  7, 10,  4, 10, 10, 10,  7 ],
        [  7, 10, 10, 10, 10, 10,  7 ],
        [  7,  7,  7,  7,  7,  7,  7 ],
    ];

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

    private static readonly byte[][] Road =
    [
        [ 13, 13, 13, 13, 13, 13, 13 ],
        [ 13, 13, 13, 13, 13, 13, 13 ],
        [ 13, 13, 13, 13, 13, 13, 13 ],
        [  3,  3,  0,  0,  0,  3,  3 ],
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

    // ── Sprite dimensions (7×16 total) ───────────────────────────────────────

    public override int CharWidth  => 7;
    public override int HeadRows   => 4;
    public override int BodyRows   => 7;
    public override int LegsRows   => 5;

    // ── Head variants (1 frame × 4 rows × 7 cols) ────────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory

    private static readonly byte[][][] _head0 =   // basic round head
    [
        [
            [ 0, 2, 1, 1, 2, 0, 0 ],   // hair + skin sides
            [ 0, 1, 4, 1, 4, 1, 0 ],   // eyes
            [ 0, 1, 3, 1, 1, 1, 0 ],   // highlight + lower face
            [ 0, 0, 1, 1, 1, 0, 0 ],   // chin + neck
        ],
    ];

    private static readonly byte[][][] _head1 =   // cap / hat
    [
        [
            [ 0, 5, 5, 5, 5, 5, 0 ],   // hat top
            [ 5, 5, 5, 5, 5, 5, 5 ],   // hat brim
            [ 0, 1, 4, 1, 4, 1, 0 ],   // eyes (no hair)
            [ 0, 0, 1, 1, 1, 0, 0 ],   // chin + neck
        ],
    ];

    private static readonly byte[][][] _head2 =   // long / full hair
    [
        [
            [ 2, 2, 2, 2, 2, 2, 0 ],   // full hair top
            [ 2, 1, 4, 1, 4, 2, 0 ],   // eyes framed by hair
            [ 2, 1, 3, 1, 1, 2, 0 ],   // lower face + highlight
            [ 0, 0, 1, 1, 1, 0, 0 ],   // neck
        ],
    ];

    public override byte[][][][] HeadParts { get; } = [ _head0, _head1, _head2 ];

    // ── Body variants (1 frame × 7 rows × 7 cols) ────────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory

    private static readonly byte[][][] _body0 =   // casual shirt
    [
        [
            [ 0, 0, 1, 1, 1, 0, 0 ],   // neck skin
            [ 0, 2, 2, 2, 2, 2, 0 ],   // shoulders
            [ 2, 2, 2, 2, 2, 2, 0 ],   // upper chest
            [ 2, 3, 2, 4, 2, 3, 0 ],   // highlights + button
            [ 2, 2, 2, 2, 2, 2, 0 ],
            [ 2, 2, 2, 4, 2, 2, 0 ],   // lower button
            [ 2, 2, 2, 2, 2, 2, 0 ],
        ],
    ];

    private static readonly byte[][][] _body1 =   // collared / formal
    [
        [
            [ 0, 0, 1, 1, 1, 0, 0 ],   // neck skin
            [ 0, 5, 2, 2, 5, 0, 0 ],   // lapels top
            [ 0, 5, 2, 2, 5, 2, 0 ],   // lapels chest
            [ 0, 2, 4, 4, 2, 2, 0 ],   // buttons
            [ 0, 2, 2, 2, 2, 2, 0 ],
            [ 0, 2, 4, 4, 2, 2, 0 ],   // more buttons
            [ 0, 2, 2, 2, 2, 2, 0 ],
        ],
    ];

    private static readonly byte[][][] _body2 =   // jacket / hoodie
    [
        [
            [ 0, 0, 1, 1, 1, 0, 0 ],   // neck skin
            [ 5, 5, 2, 2, 5, 5, 0 ],   // jacket outer
            [ 5, 2, 2, 2, 2, 5, 0 ],   // open front
            [ 5, 2, 3, 4, 2, 5, 0 ],   // highlight + button
            [ 5, 2, 2, 2, 2, 5, 0 ],
            [ 5, 2, 3, 2, 3, 5, 0 ],
            [ 5, 5, 2, 2, 5, 5, 0 ],
        ],
    ];

    public override byte[][][][] BodyParts { get; } = [ _body0, _body1, _body2 ];

    // ── Legs variants (4 frames × 5 rows × 7 cols) ───────────────────────────
    // Semantic: 1=Skin(bare)  2=Pants  3=PantsHighlight  4=Belt
    //           5=BeltHighlight  6=Shoes  7=ShoeHighlight

    private static readonly byte[][][] _legs0 =   // pants + belt
    [
        // Frame 0 — idle
        [
            [ 4, 4, 4, 4, 4, 4, 0 ],   // belt
            [ 2, 2, 5, 5, 2, 2, 0 ],   // pants + buckle
            [ 2, 2, 0, 0, 2, 2, 0 ],   // leg gap
            [ 6, 6, 0, 0, 6, 6, 0 ],   // shoes
            [ 7, 6, 0, 0, 6, 7, 0 ],
        ],
        // Frame 1 — left foot forward
        [
            [ 4, 4, 4, 4, 4, 4, 0 ],
            [ 2, 2, 5, 5, 2, 2, 0 ],
            [ 2, 2, 0, 0, 2, 2, 0 ],
            [ 2, 0, 0, 0, 6, 6, 0 ],   // left leg out
            [ 6, 0, 0, 0, 6, 7, 0 ],
        ],
        // Frame 2 — crossing
        [
            [ 4, 4, 4, 4, 4, 4, 0 ],
            [ 2, 2, 5, 5, 2, 2, 0 ],
            [ 0, 2, 3, 3, 2, 0, 0 ],   // crossing
            [ 0, 6, 6, 6, 6, 0, 0 ],
            [ 0, 7, 6, 6, 7, 0, 0 ],
        ],
        // Frame 3 — right foot forward
        [
            [ 4, 4, 4, 4, 4, 4, 0 ],
            [ 2, 2, 5, 5, 2, 2, 0 ],
            [ 2, 2, 0, 0, 2, 2, 0 ],
            [ 6, 6, 0, 0, 0, 2, 0 ],   // right leg out
            [ 6, 7, 0, 0, 0, 6, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs1 =   // formal trousers (crease)
    [
        // Frame 0 — idle
        [
            [ 4, 4, 4, 4, 4, 4, 0 ],
            [ 2, 3, 5, 5, 3, 2, 0 ],   // crease on each leg
            [ 2, 3, 0, 0, 3, 2, 0 ],
            [ 6, 6, 0, 0, 6, 6, 0 ],
            [ 7, 6, 0, 0, 6, 7, 0 ],
        ],
        // Frame 1 — left foot forward
        [
            [ 4, 4, 4, 4, 4, 4, 0 ],
            [ 2, 3, 5, 5, 3, 2, 0 ],
            [ 2, 3, 0, 0, 3, 2, 0 ],
            [ 2, 0, 0, 0, 6, 6, 0 ],
            [ 6, 0, 0, 0, 6, 7, 0 ],
        ],
        // Frame 2 — crossing
        [
            [ 4, 4, 4, 4, 4, 4, 0 ],
            [ 2, 3, 5, 5, 3, 2, 0 ],
            [ 0, 3, 3, 3, 3, 0, 0 ],
            [ 0, 6, 6, 6, 6, 0, 0 ],
            [ 0, 7, 6, 6, 7, 0, 0 ],
        ],
        // Frame 3 — right foot forward
        [
            [ 4, 4, 4, 4, 4, 4, 0 ],
            [ 2, 3, 5, 5, 3, 2, 0 ],
            [ 2, 3, 0, 0, 3, 2, 0 ],
            [ 6, 6, 0, 0, 0, 2, 0 ],
            [ 6, 7, 0, 0, 0, 6, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs2 =   // shorts + bare skin
    [
        // Frame 0 — idle
        [
            [ 4, 4, 4, 4, 4, 4, 0 ],
            [ 2, 2, 5, 5, 2, 2, 0 ],   // shorts
            [ 1, 1, 0, 0, 1, 1, 0 ],   // bare skin
            [ 6, 6, 0, 0, 6, 6, 0 ],   // shoes
            [ 7, 6, 0, 0, 6, 7, 0 ],
        ],
        // Frame 1 — left foot forward
        [
            [ 4, 4, 4, 4, 4, 4, 0 ],
            [ 2, 2, 5, 5, 2, 2, 0 ],
            [ 1, 1, 0, 0, 1, 1, 0 ],
            [ 1, 0, 0, 0, 6, 6, 0 ],
            [ 6, 0, 0, 0, 6, 7, 0 ],
        ],
        // Frame 2 — crossing
        [
            [ 4, 4, 4, 4, 4, 4, 0 ],
            [ 2, 2, 5, 5, 2, 2, 0 ],
            [ 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 6, 6, 6, 6, 0, 0 ],
            [ 0, 7, 6, 6, 7, 0, 0 ],
        ],
        // Frame 3 — right foot forward
        [
            [ 4, 4, 4, 4, 4, 4, 0 ],
            [ 2, 2, 5, 5, 2, 2, 0 ],
            [ 1, 1, 0, 0, 1, 1, 0 ],
            [ 6, 6, 0, 0, 0, 1, 0 ],
            [ 6, 7, 0, 0, 0, 6, 0 ],
        ],
    ];

    public override byte[][][][] LegsParts { get; } = [ _legs0, _legs1, _legs2 ];

    // ── Head palettes (skin/hair tones in Apple II color gamut) ───────────────

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("fair/blonde",
            Skin:      new Color(220, 175, 125),
            Hair:      new Color(255, 210, 100),
            Highlight: new Color(240, 205, 155),
            Eyes:      new Color( 20,  88, 255),
            Accessory: new Color(255, 210, 100)),
        new("fair/brown",
            Skin:      new Color(220, 175, 125),
            Hair:      new Color(100,  65,  20),
            Highlight: new Color(240, 205, 155),
            Eyes:      new Color( 30,  30,  30),
            Accessory: new Color(100,  65,  20)),
        new("medium/black",
            Skin:      new Color(175, 120,  65),
            Hair:      new Color( 20,  20,  20),
            Highlight: new Color(200, 145,  90),
            Eyes:      new Color( 30,  30,  30),
            Accessory: new Color( 20,  20,  20)),
        new("dark/black",
            Skin:      new Color(115,  80,  35),
            Hair:      new Color( 20,  20,  20),
            Highlight: new Color(135,  95,  45),
            Eyes:      new Color( 20,  88, 255),
            Accessory: new Color( 20,  20,  20)),
        new("medium/auburn",
            Skin:      new Color(175, 120,  65),
            Hair:      new Color(180,  50,  20),
            Highlight: new Color(200, 145,  90),
            Eyes:      new Color( 20,  88, 255),
            Accessory: new Color(180,  50,  20)),
    ];

    // ── Body palettes ─────────────────────────────────────────────────────────

    public override BodyPalette[] BodyPalettes { get; } =
    [
        new("green",
            Skin:           new Color(220, 175, 125),
            Shirt:          new Color( 20, 245,  60),   // phosphor green
            ShirtHighlight: new Color(120, 255, 120),
            Buttons:        new Color(255, 255, 255),
            Accessory:      new Color( 20,  88, 255)),
        new("blue",
            Skin:           new Color(220, 175, 125),
            Shirt:          new Color( 20,  88, 255),   // artifact blue
            ShirtHighlight: new Color(100, 150, 255),
            Buttons:        new Color(192, 192, 192),
            Accessory:      new Color(255, 210, 100)),
        new("red",
            Skin:           new Color(220, 175, 125),
            Shirt:          new Color(220,  40,  40),
            ShirtHighlight: new Color(255, 130, 130),
            Buttons:        new Color(255, 255, 255),
            Accessory:      new Color(255, 210, 100)),
        new("white/light",
            Skin:           new Color(220, 175, 125),
            Shirt:          new Color(192, 192, 192),
            ShirtHighlight: new Color(255, 255, 255),
            Buttons:        new Color( 20,  88, 255),
            Accessory:      new Color( 20, 245,  60)),
        new("teal",
            Skin:           new Color(220, 175, 125),
            Shirt:          new Color(  0, 180, 180),
            ShirtHighlight: new Color(100, 240, 240),
            Buttons:        new Color(255, 255, 255),
            Accessory:      new Color(193,  28, 255)),
    ];

    // ── Legs palettes ─────────────────────────────────────────────────────────

    public override LegsPalette[] LegsPalettes { get; } =
    [
        new("blue jeans/brown shoes",
            Skin:           new Color(220, 175, 125),
            Pants:          new Color( 20,  88, 255),
            PantsHighlight: new Color(100, 150, 255),
            Belt:           new Color(100,  65,  20),
            BeltHighlight:  new Color(180, 130,  60),
            Shoes:          new Color( 80,  45,  10),
            ShoeHighlight:  new Color(130,  85,  30)),
        new("black/black",
            Skin:           new Color(220, 175, 125),
            Pants:          new Color( 20,  20,  20),
            PantsHighlight: new Color( 80,  80,  80),
            Belt:           new Color( 60,  60,  60),
            BeltHighlight:  new Color(130, 130, 130),
            Shoes:          new Color( 20,  20,  20),
            ShoeHighlight:  new Color( 70,  70,  70)),
        new("khaki/tan",
            Skin:           new Color(220, 175, 125),
            Pants:          new Color(200, 165, 100),
            PantsHighlight: new Color(230, 215, 160),
            Belt:           new Color(100,  65,  20),
            BeltHighlight:  new Color(170, 120,  50),
            Shoes:          new Color(100,  65,  20),
            ShoeHighlight:  new Color(155, 110,  50)),
        new("gray/dark",
            Skin:           new Color(220, 175, 125),
            Pants:          new Color(130, 130, 130),
            PantsHighlight: new Color(192, 192, 192),
            Belt:           new Color( 60,  60,  60),
            BeltHighlight:  new Color(130, 130, 130),
            Shoes:          new Color( 60,  60,  60),
            ShoeHighlight:  new Color(130, 130, 130)),
    ];
}
