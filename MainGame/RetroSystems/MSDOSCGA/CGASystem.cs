using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.MSDOSCGA;

/// <summary>
/// MS-DOS CGA visual style.
///
/// Authentic characteristics:
///   • 8×8 tiles at 320×200 native resolution
///   • Full IBM CGA 16-color RGBI palette
///   • Clean, hard-edged art — 2-4 colors per tile, no dithering
///   • Character sprites: 8×16 (CGA text-mode character proportions)
///   • DisplayScale 3.0: a 600px-tall viewport shows exactly 200 world pixels (native height)
///   • MaxZoomOutArea (640×400) limits zoom-out to 2× native in each direction
///
/// Sprite split: HeadRows=4, BodyRows=6, LegsRows=6 (total 16)
/// </summary>
public sealed class CGASystem : RetroSystem
{
    public override string Name        => "MS-DOS CGA";
    public override string Description => "8×8 tiles · 320×200 · 16-color RGBI palette";
    public override int    NativeTileSize => 8;
    public override float  DisplayScale   => 3.0f;

    // Native CGA: 320×200. MaxZoomOutArea caps at 2× native in each direction.
    public override Vector2? MaxZoomOutArea => new Vector2(640, 400);

    // ── Full 16-color CGA RGBI palette ───────────────────────────────────────
    protected override Color[] TilePalette { get; } =
    [
        new Color(  0,   0,   0),   //  0 black           (bg fill)
        new Color(  0,   0, 170),   //  1 dark blue
        new Color(  0, 170,   0),   //  2 dark green
        new Color(  0, 170, 170),   //  3 dark cyan
        new Color(170,   0,   0),   //  4 dark red
        new Color(170,   0, 170),   //  5 dark magenta
        new Color(170,  85,   0),   //  6 brown
        new Color(170, 170, 170),   //  7 light gray
        new Color( 85,  85,  85),   //  8 dark gray
        new Color( 85,  85, 255),   //  9 bright blue
        new Color( 85, 255,  85),   // 10 bright green
        new Color( 85, 255, 255),   // 11 bright cyan
        new Color(255,  85,  85),   // 12 bright red
        new Color(255,  85, 255),   // 13 bright magenta
        new Color(255, 255,  85),   // 14 yellow
        new Color(255, 255, 255),   // 15 white
    ];
    // Index 16 = accentColor appended at build time by base BuildTileset

    // ── Tile pixel art (8×8) ─────────────────────────────────────────────────

    protected override byte[][] GetTilePixels(TileType type, Color accentColor) => type switch
    {
        TileType.WoodFloor     => WoodFloor,
        TileType.Carpet        => Carpet,
        TileType.KitchenTile   => KitchenTile,
        TileType.Wall          => Wall,
        TileType.Door          => Door,
        TileType.Window        => Window,
        TileType.Furniture     => Furniture,
        TileType.Counter       => Counter,
        TileType.Bookshelf     => Bookshelf,
        TileType.Plant         => Plant,
        TileType.Grass         => Grass,
        TileType.Road          => Road,
        TileType.Sidewalk      => Sidewalk,
        TileType.HouseExterior => Wall,
        TileType.Accent        => Accent,
        _ => Wall
    };

    private static readonly byte[][] WoodFloor =
    [
        [  6,  6,  6,  8,  6,  6,  6,  8 ],
        [  6,  6,  8,  6,  6,  6,  8,  6 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
        [  6,  8,  6,  6,  8,  6,  6,  8 ],
        [  6,  6,  6,  8,  6,  6,  6,  8 ],
        [  6,  6,  8,  6,  6,  6,  8,  6 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
        [  6,  8,  6,  6,  8,  6,  6,  8 ],
    ];

    private static readonly byte[][] Carpet =
    [
        [  4,  4,  4,  4,  4,  4,  4,  4 ],
        [  4,  5,  5,  4,  4,  5,  5,  4 ],
        [  4,  5,  4,  4,  4,  4,  5,  4 ],
        [  4,  4,  4,  5,  5,  4,  4,  4 ],
        [  4,  4,  4,  5,  5,  4,  4,  4 ],
        [  4,  5,  4,  4,  4,  4,  5,  4 ],
        [  4,  5,  5,  4,  4,  5,  5,  4 ],
        [  4,  4,  4,  4,  4,  4,  4,  4 ],
    ];

    private static readonly byte[][] KitchenTile =
    [
        [ 15, 15,  8, 15, 15, 15,  8, 15 ],
        [ 15, 15,  8, 15, 15, 15,  8, 15 ],
        [ 15, 15,  8, 15, 15, 15,  8, 15 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
        [ 15, 15,  8, 15, 15, 15,  8, 15 ],
        [ 15, 15,  8, 15, 15, 15,  8, 15 ],
        [ 15, 15,  8, 15, 15, 15,  8, 15 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
    ];

    private static readonly byte[][] Wall =
    [
        [  7,  7,  7,  8,  7,  7,  7,  8 ],
        [  7,  7,  7,  8,  7,  7,  7,  8 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
        [  8,  7,  7,  7,  8,  7,  7,  7 ],
        [  8,  7,  7,  7,  8,  7,  7,  7 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
        [  7,  7,  7,  8,  7,  7,  7,  8 ],
        [  7,  7,  7,  8,  7,  7,  7,  8 ],
    ];

    private static readonly byte[][] Door =
    [
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
        [  8,  6,  6,  8,  8,  6,  6,  8 ],
        [  8,  6,  6,  8,  8,  6,  6,  8 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
        [  8,  6,  6,  6,  6,  6,  6,  8 ],
        [  8,  6, 14,  6,  6,  6,  6,  8 ],
        [  8,  6,  6,  6,  6,  6,  6,  8 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
    ];

    private static readonly byte[][] Window =
    [
        [  7,  7,  7,  7,  7,  7,  7,  7 ],
        [  7, 11, 11,  7, 11, 11, 11,  7 ],
        [  7, 11, 11,  7, 11, 11, 11,  7 ],
        [  7, 11, 11,  7, 11, 11, 11,  7 ],
        [  7,  7,  7,  7,  7,  7,  7,  7 ],
        [  7, 11, 11, 11, 11, 11, 11,  7 ],
        [  7, 11, 11, 11, 11, 11, 11,  7 ],
        [  7,  7,  7,  7,  7,  7,  7,  7 ],
    ];

    private static readonly byte[][] Furniture =
    [
        [  1,  1,  1,  1,  1,  1,  1,  1 ],
        [  1,  9,  9,  9,  9,  9,  9,  1 ],
        [  1,  9,  1,  1,  1,  1,  9,  1 ],
        [  1,  9,  1,  1,  1,  1,  9,  1 ],
        [  1,  9,  1,  1,  1,  1,  9,  1 ],
        [  1,  9,  9,  9,  9,  9,  9,  1 ],
        [  1,  1,  1,  1,  1,  1,  1,  1 ],
        [  1,  1,  1,  1,  1,  1,  1,  1 ],
    ];

    private static readonly byte[][] Counter =
    [
        [ 15, 15, 15, 15, 15, 15, 15, 15 ],
        [ 15,  7,  7,  7,  7,  7,  7, 15 ],
        [ 15,  7, 15, 15, 15, 15,  7, 15 ],
        [ 15,  7, 15, 15, 15, 15,  7, 15 ],
        [ 15,  7, 15, 15, 15, 15,  7, 15 ],
        [ 15,  7,  7,  7,  7,  7,  7, 15 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
    ];

    private static readonly byte[][] Bookshelf =
    [
        [  6,  6,  6,  6,  6,  6,  6,  6 ],
        [  6,  7,  6,  7,  6,  7,  6,  7 ],
        [  4, 12,  1,  9,  2, 10,  5, 13 ],
        [  4, 12,  1,  9,  2, 10,  5, 13 ],
        [  4, 12,  1,  9,  2, 10,  5, 13 ],
        [  4, 12,  1,  9,  2, 10,  5, 13 ],
        [  6,  6,  6,  6,  6,  6,  6,  6 ],
        [  6,  6,  6,  6,  6,  6,  6,  6 ],
    ];

    private static readonly byte[][] Plant =
    [
        [  0,  0,  2,  0,  2,  0,  0,  0 ],
        [  0,  2, 10,  2, 10,  2,  0,  0 ],
        [  0,  0, 10,  2, 10,  0,  0,  0 ],
        [  0,  0,  0,  2,  0,  0,  0,  0 ],
        [  0,  0,  6,  6,  6,  0,  0,  0 ],
        [  0,  0,  6, 15,  6,  0,  0,  0 ],
        [  0,  0,  6,  6,  6,  0,  0,  0 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
    ];

    private static readonly byte[][] Grass =
    [
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
        [  2,  2, 10,  2,  2,  2, 10,  2 ],
        [  2, 10,  2,  2,  2, 10,  2,  2 ],
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
        [  2,  2,  2, 10,  2,  2,  2,  2 ],
        [  2,  2, 10,  2,  2,  2, 10,  2 ],
        [  2, 10,  2,  2,  2, 10,  2,  2 ],
    ];

    private static readonly byte[][] Road =
    [
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
        [ 15, 15,  0,  0,  0, 15, 15, 15 ],
        [ 15, 15,  0,  0,  0, 15, 15, 15 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
        [  8,  8,  8,  8,  8,  8,  8,  8 ],
    ];

    private static readonly byte[][] Sidewalk =
    [
        [  7,  7,  7,  7,  7,  7,  7,  7 ],
        [  7, 15, 15, 15, 15, 15, 15,  7 ],
        [  7, 15,  7,  7,  7,  7, 15,  7 ],
        [  7, 15,  7,  7,  7,  7, 15,  7 ],
        [  7,  7,  7,  7,  7,  7,  7,  7 ],
        [  7, 15,  7,  7,  7,  7, 15,  7 ],
        [  7, 15, 15, 15, 15, 15, 15,  7 ],
        [  7,  7,  7,  7,  7,  7,  7,  7 ],
    ];

    private static readonly byte[][] Accent =
    [
        [ 16, 16, 16, 16, 16, 16, 16, 16 ],
        [ 16, 16, 16, 16, 16, 16, 16, 16 ],
        [ 16, 16, 16, 16, 16, 16, 16, 16 ],
        [ 16, 16, 16, 16, 16, 16, 16, 16 ],
        [ 16, 16, 16, 16, 16, 16, 16, 16 ],
        [ 16, 16, 16, 16, 16, 16, 16, 16 ],
        [ 16, 16, 16, 16, 16, 16, 16, 16 ],
        [ 16, 16, 16, 16, 16, 16, 16, 16 ],
    ];

    // ── Sprite dimensions ─────────────────────────────────────────────────────

    public override int CharWidth  => 8;
    public override int HeadRows   => 4;
    public override int BodyRows   => 6;
    public override int LegsRows   => 6;

    // ── Head parts (8 wide × 4 rows, 1 frame each) ───────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory

    // Head 0: basic round head
    private static readonly byte[][][] _head0 =
    [[
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],
        [ 0, 1, 1, 4, 1, 4, 1, 0 ],   // eyes (4)
        [ 0, 1, 1, 1, 1, 1, 1, 0 ],
        [ 0, 0, 1, 2, 2, 1, 0, 0 ],   // hair base (2)
    ]];

    // Head 1: cap / hat
    private static readonly byte[][][] _head1 =
    [[
        [ 0, 5, 5, 5, 5, 5, 5, 0 ],   // hat top (5)
        [ 0, 5, 1, 1, 1, 1, 5, 0 ],   // hat brim + face
        [ 0, 1, 4, 1, 1, 4, 1, 0 ],   // eyes (4)
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // chin
    ]];

    // Head 2: full / long hair
    private static readonly byte[][][] _head2 =
    [[
        [ 0, 2, 2, 2, 2, 2, 2, 0 ],   // hair (2)
        [ 0, 2, 1, 1, 1, 1, 2, 0 ],   // hair sides + face
        [ 0, 2, 4, 1, 1, 4, 2, 0 ],   // hair sides + eyes (4)
        [ 0, 2, 1, 1, 1, 1, 2, 0 ],   // hair sides + chin
    ]];

    public override byte[][][][] HeadParts { get; } = [ _head0, _head1, _head2 ];

    // ── Body parts (8 wide × 6 rows, 1 frame each) ───────────────────────────
    // Semantic: 1=Skin(neck)  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory

    // Body 0: casual shirt
    private static readonly byte[][][] _body0 =
    [[
        [ 0, 1, 1, 1, 1, 1, 1, 0 ],   // neck (skin=1)
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt (2)
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 3, 2, 2, 2, 2, 3, 2 ],   // side highlights (3)
        [ 0, 2, 2, 4, 4, 2, 2, 0 ],   // buttons (4)
    ]];

    // Body 1: collared / formal shirt
    private static readonly byte[][][] _body1 =
    [[
        [ 0, 1, 5, 1, 1, 5, 1, 0 ],   // collar tips (5)
        [ 2, 5, 2, 4, 4, 2, 5, 2 ],   // lapels (5) + buttons (4)
        [ 2, 5, 2, 4, 4, 2, 5, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 3, 2, 2, 2, 2, 3, 2 ],   // highlights
        [ 0, 2, 2, 2, 2, 2, 2, 0 ],
    ]];

    // Body 2: jacket / hoodie
    private static readonly byte[][][] _body2 =
    [[
        [ 0, 1, 1, 1, 1, 1, 1, 0 ],   // neck (skin=1)
        [ 5, 5, 2, 2, 2, 2, 5, 5 ],   // jacket outer (5) + shirt (2)
        [ 5, 2, 2, 3, 3, 2, 2, 5 ],   // shirt highlight (3)
        [ 5, 2, 2, 4, 4, 2, 2, 5 ],   // buttons (4)
        [ 5, 5, 2, 2, 2, 2, 5, 5 ],
        [ 0, 5, 2, 2, 2, 2, 5, 0 ],   // jacket hem
    ]];

    public override byte[][][][] BodyParts { get; } = [ _body0, _body1, _body2 ];

    // ── Legs parts (8 wide × 6 rows, 4 walk frames) ──────────────────────────
    // Semantic: 2=Pants  3=PantsHighlight  4=Belt  6=Shoes  7=ShoeHighlight
    //           1=Skin(bare, used in shorts variant)

    // Legs 0: pants + belt — 4 frames
    private static readonly byte[][][] _legs0 =
    [
        [   // frame 0 — idle
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],   // belt (4) + pants (2)
            [ 0, 2, 3, 0, 0, 3, 2, 0 ],   // thigh highlights (3)
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 6, 7, 0, 0, 6, 7, 0 ],   // shoes (6) + highlight (7)
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // frame 1 — walk A (left leg forward)
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 3, 0, 0, 0, 2, 0 ],
            [ 2, 2, 0, 0, 0, 0, 2, 0 ],
            [ 2, 2, 0, 0, 0, 0, 2, 2 ],
            [ 7, 0, 0, 0, 0, 0, 6, 7 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // frame 2 — mid-stride
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 3, 0, 0, 3, 2, 0 ],
            [ 0, 2, 0, 0, 0, 0, 2, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // frame 3 — walk B (right leg forward)
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 0, 0, 0, 3, 2, 0 ],
            [ 0, 2, 0, 0, 0, 0, 2, 2 ],
            [ 0, 2, 2, 0, 0, 0, 2, 2 ],
            [ 0, 7, 6, 0, 0, 0, 7, 7 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
    ];

    // Legs 1: formal trousers (no belt, neater shoe)
    private static readonly byte[][][] _legs1 =
    [
        [   // idle
            [ 0, 2, 2, 2, 2, 2, 2, 0 ],   // pants hip (no belt)
            [ 0, 2, 3, 0, 0, 3, 2, 0 ],
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 6, 6, 0, 0, 6, 6, 0 ],
            [ 0, 7, 7, 0, 0, 7, 7, 0 ],   // shoe tips
        ],
        [   // walk A
            [ 0, 2, 2, 2, 2, 2, 2, 0 ],
            [ 0, 2, 3, 0, 0, 0, 2, 0 ],
            [ 2, 2, 0, 0, 0, 0, 2, 0 ],
            [ 2, 2, 0, 0, 0, 0, 2, 2 ],
            [ 7, 0, 0, 0, 0, 0, 6, 6 ],
            [ 0, 0, 0, 0, 0, 0, 7, 7 ],
        ],
        [   // mid
            [ 0, 2, 2, 2, 2, 2, 2, 0 ],
            [ 0, 2, 3, 0, 0, 3, 2, 0 ],
            [ 0, 2, 0, 0, 0, 0, 2, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 2, 2, 2, 2, 2, 2, 0 ],
            [ 0, 2, 0, 0, 0, 3, 2, 0 ],
            [ 0, 2, 0, 0, 0, 0, 2, 2 ],
            [ 0, 2, 2, 0, 0, 0, 2, 2 ],
            [ 0, 6, 6, 0, 0, 0, 7, 7 ],
            [ 0, 7, 7, 0, 0, 0, 0, 0 ],
        ],
    ];

    // Legs 2: shorts — bare skin below shorts
    private static readonly byte[][][] _legs2 =
    [
        [   // idle
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 3, 1, 1, 3, 2, 0 ],   // shorts with skin visible (1)
            [ 0, 2, 2, 1, 1, 2, 2, 0 ],
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],   // bare legs
            [ 0, 6, 7, 0, 0, 6, 7, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk A
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 3, 1, 0, 0, 2, 0 ],
            [ 1, 1, 0, 0, 0, 0, 1, 0 ],
            [ 1, 1, 0, 0, 0, 0, 1, 1 ],
            [ 7, 0, 0, 0, 0, 0, 6, 7 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // mid
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 3, 1, 1, 3, 2, 0 ],
            [ 0, 1, 0, 0, 0, 0, 1, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 0, 0, 1, 3, 2, 0 ],
            [ 0, 1, 0, 0, 0, 0, 1, 1 ],
            [ 0, 1, 1, 0, 0, 0, 1, 1 ],
            [ 0, 7, 6, 0, 0, 0, 7, 7 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
    ];

    public override byte[][][][] LegsParts { get; } = [ _legs0, _legs1, _legs2 ];

    // ── Head palettes (5) ────────────────────────────────────────────────────
    // Colors drawn from the CGA 16-color RGBI set for authenticity.
    // Skin tones: fair=#7(ltGray), medium=#6(brown), dark=#4(dkRed)

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("Fair/Blonde",
            Skin:      new Color(170, 170, 170),   // #7 light gray (fair skin)
            Hair:      new Color(255, 255,  85),   // #14 yellow
            Highlight: new Color(255, 255, 255),   // #15 white
            Eyes:      new Color( 85,  85,  85),   // #8 dark gray
            Accessory: new Color(170,  85,   0)),  // #6 brown

        new("Fair/Brown",
            Skin:      new Color(170, 170, 170),
            Hair:      new Color(170,  85,   0),   // #6 brown
            Highlight: new Color(255, 255, 255),
            Eyes:      new Color( 85,  85,  85),
            Accessory: new Color(170,  85,   0)),

        new("Medium/Black",
            Skin:      new Color(170,  85,   0),   // #6 brown (medium skin)
            Hair:      new Color(  0,   0,   0),   // #0 black
            Highlight: new Color(170, 170, 170),   // #7 light gray
            Eyes:      new Color( 85,  85,  85),
            Accessory: new Color( 85,  85,  85)),

        new("Dark/Black",
            Skin:      new Color(170,   0,   0),   // #4 dark red (dark skin)
            Hair:      new Color(  0,   0,   0),
            Highlight: new Color(170,  85,   0),
            Eyes:      new Color( 85,  85,  85),
            Accessory: new Color( 85,  85,  85)),

        new("Medium/Auburn",
            Skin:      new Color(170,  85,   0),
            Hair:      new Color(255,  85,  85),   // #12 bright red (auburn)
            Highlight: new Color(170, 170, 170),
            Eyes:      new Color( 85,  85,  85),
            Accessory: new Color( 85,  85,  85)),
    ];

    // ── Body palettes (5) ────────────────────────────────────────────────────

    public override BodyPalette[] BodyPalettes { get; } =
    [
        new("Green",
            Skin:           new Color(170, 170, 170),
            Shirt:          new Color( 85, 255,  85),   // #10 bright green
            ShirtHighlight: new Color(255, 255, 255),
            Buttons:        new Color(  0,   0,   0),
            Accessory:      new Color(  0, 170,   0)),  // #2 dark green

        new("Blue",
            Skin:           new Color(170, 170, 170),
            Shirt:          new Color( 85,  85, 255),   // #9 bright blue
            ShirtHighlight: new Color(255, 255, 255),
            Buttons:        new Color(  0,   0,   0),
            Accessory:      new Color(  0,   0, 170)),  // #1 dark blue

        new("Red",
            Skin:           new Color(170, 170, 170),
            Shirt:          new Color(255,  85,  85),   // #12 bright red
            ShirtHighlight: new Color(255, 255, 255),
            Buttons:        new Color(  0,   0,   0),
            Accessory:      new Color(170,   0,   0)),  // #4 dark red

        new("White",
            Skin:           new Color(170, 170, 170),
            Shirt:          new Color(255, 255, 255),   // #15 white
            ShirtHighlight: new Color(255, 255, 255),
            Buttons:        new Color( 85,  85,  85),
            Accessory:      new Color(170, 170, 170)),  // #7 light gray

        new("Teal",
            Skin:           new Color(170, 170, 170),
            Shirt:          new Color( 85, 255, 255),   // #11 bright cyan
            ShirtHighlight: new Color(255, 255, 255),
            Buttons:        new Color(  0,   0,   0),
            Accessory:      new Color(  0, 170, 170)),  // #3 dark cyan
    ];

    // ── Legs palettes (4) ────────────────────────────────────────────────────

    public override LegsPalette[] LegsPalettes { get; } =
    [
        new("Blue Jeans/Brown",
            Skin:          new Color(170, 170, 170),
            Pants:         new Color( 85,  85, 255),   // #9 bright blue
            PantsHighlight:new Color( 85, 255, 255),   // #11 bright cyan
            Belt:          new Color(170,  85,   0),   // #6 brown
            BeltHighlight: new Color(255, 255,  85),   // #14 yellow
            Shoes:         new Color(170,  85,   0),
            ShoeHighlight: new Color(170, 170, 170)),

        new("Black/Black",
            Skin:          new Color(170, 170, 170),
            Pants:         new Color(  0,   0,   0),   // #0 black
            PantsHighlight:new Color( 85,  85,  85),   // #8 dark gray
            Belt:          new Color( 85,  85,  85),
            BeltHighlight: new Color(170, 170, 170),
            Shoes:         new Color(  0,   0,   0),
            ShoeHighlight: new Color( 85,  85,  85)),

        new("Khaki/Tan",
            Skin:          new Color(170, 170, 170),
            Pants:         new Color(255, 255,  85),   // #14 yellow (khaki)
            PantsHighlight:new Color(255, 255, 255),
            Belt:          new Color(170,  85,   0),
            BeltHighlight: new Color(255, 255,  85),
            Shoes:         new Color(170,  85,   0),
            ShoeHighlight: new Color(170, 170, 170)),

        new("Gray/Dark",
            Skin:          new Color(170, 170, 170),
            Pants:         new Color(170, 170, 170),   // #7 light gray
            PantsHighlight:new Color(255, 255, 255),
            Belt:          new Color( 85,  85,  85),
            BeltHighlight: new Color(170, 170, 170),
            Shoes:         new Color( 85,  85,  85),
            ShoeHighlight: new Color(  0,   0,   0)),
    ];
}
