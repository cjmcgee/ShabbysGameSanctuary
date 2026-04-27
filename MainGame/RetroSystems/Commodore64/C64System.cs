using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.Commodore64;

/// <summary>
/// Commodore 64 visual style.
///
/// Authentic characteristics:
///   • 8×8 character-cell tiles — the C64's text/PETSCII tile unit
///   • VIC-II 16-color palette with distinctive color relationships
///   • Character-mode graphics: each 8×8 cell typically uses 2-3 colors
///   • Character sprites: 12×21 (HeadRows=6, BodyRows=8, LegsRows=7)
///   • Camera zoom 1.5×: faithful to the C64's 320×200 native resolution feel
/// </summary>
public sealed class C64System : RetroSystem
{
    public override string Name        => "Commodore 64";
    public override string Description => "8×8 char-cells · VIC-II 16-color palette";
    public override int    NativeTileSize => 8;
    public override float  DisplayScale   => 1.5f;

    // ── Tile palette (VIC-II colors) ─────────────────────────────────────────
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
        [ 6, 6, 6, 6, 6, 6, 6, 6 ],
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

    // ── Sprite dimensions (12×21 total) ──────────────────────────────────────

    public override int CharWidth  => 12;
    public override int HeadRows   => 6;
    public override int BodyRows   => 8;
    public override int LegsRows   => 7;

    // ── Head variants (1 frame × 6 rows × 12 cols) ───────────────────────────
    // Semantic indices: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory

    private static readonly byte[][][] _head0 =   // basic round head
    [
        [
            [ 0, 0, 0, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // hair top
            [ 0, 0, 2, 1, 1, 1, 1, 1, 2, 0, 0, 0 ],   // hair side + skin
            [ 0, 0, 1, 4, 1, 1, 4, 1, 3, 0, 0, 0 ],   // eyes + highlight
            [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 ],   // lower face
            [ 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
            [ 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0 ],   // neck
        ],
    ];

    private static readonly byte[][][] _head1 =   // cap / hat
    [
        [
            [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0 ],   // hat top
            [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],   // hat brim
            [ 0, 0, 1, 4, 1, 1, 4, 1, 3, 0, 0, 0 ],   // eyes + highlight
            [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 ],   // lower face
            [ 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
            [ 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0 ],   // neck
        ],
    ];

    private static readonly byte[][][] _head2 =   // long / full hair
    [
        [
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],   // hair top full width
            [ 0, 2, 1, 1, 1, 1, 1, 1, 2, 0, 0, 0 ],   // hair frame + skin
            [ 0, 2, 1, 4, 1, 1, 4, 3, 2, 0, 0, 0 ],   // eyes + highlight
            [ 0, 2, 1, 1, 1, 1, 1, 1, 2, 0, 0, 0 ],   // lower face
            [ 0, 2, 2, 1, 1, 1, 1, 2, 2, 0, 0, 0 ],   // chin + hair falling
            [ 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0 ],   // neck
        ],
    ];

    public override byte[][][][] HeadParts { get; } = [ _head0, _head1, _head2 ];

    // ── Body variants (1 frame × 8 rows × 12 cols) ───────────────────────────
    // Semantic indices: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory

    private static readonly byte[][][] _body0 =   // casual shirt
    [
        [
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],   // neck skin
            [ 0, 1, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0 ],   // shoulders
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],   // upper chest
            [ 0, 2, 3, 2, 4, 4, 2, 3, 2, 0, 0, 0 ],   // highlights + buttons
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],   // mid torso
            [ 0, 2, 3, 2, 2, 2, 2, 3, 2, 0, 0, 0 ],   // lower torso highlights
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],
        ],
    ];

    private static readonly byte[][][] _body1 =   // collared / formal
    [
        [
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],   // neck skin
            [ 0, 1, 2, 5, 2, 2, 5, 2, 1, 0, 0, 0 ],   // lapels start
            [ 0, 2, 2, 5, 2, 2, 5, 2, 2, 0, 0, 0 ],   // lapels chest
            [ 0, 2, 2, 2, 4, 4, 2, 2, 2, 0, 0, 0 ],   // center buttons
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 4, 4, 2, 3, 2, 0, 0, 0 ],   // lower buttons + highlights
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],
        ],
    ];

    private static readonly byte[][][] _body2 =   // jacket / hoodie
    [
        [
            [ 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 ],   // neck skin
            [ 0, 5, 5, 5, 2, 2, 5, 5, 5, 0, 0, 0 ],   // jacket outer
            [ 0, 5, 2, 2, 2, 2, 2, 2, 5, 0, 0, 0 ],   // jacket open front
            [ 0, 5, 2, 3, 4, 4, 3, 2, 5, 0, 0, 0 ],   // highlights + buttons
            [ 0, 5, 2, 2, 2, 2, 2, 2, 5, 0, 0, 0 ],
            [ 0, 5, 2, 3, 2, 2, 3, 2, 5, 0, 0, 0 ],
            [ 0, 5, 5, 2, 2, 2, 2, 5, 5, 0, 0, 0 ],
            [ 0, 5, 5, 2, 2, 2, 2, 5, 5, 0, 0, 0 ],
        ],
    ];

    public override byte[][][][] BodyParts { get; } = [ _body0, _body1, _body2 ];

    // ── Legs variants (4 frames × 7 rows × 12 cols) ──────────────────────────
    // Semantic: 1=Skin(bare)  2=Pants  3=PantsHighlight  4=Belt
    //           5=BeltHighlight  6=Shoes  7=ShoeHighlight

    private static readonly byte[][][] _legs0 =   // pants + belt
    [
        // Frame 0 — idle
        [
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0 ],   // belt
            [ 0, 2, 2, 2, 5, 5, 2, 2, 2, 0, 0, 0 ],   // buckle
            [ 0, 2, 3, 2, 2, 2, 2, 3, 2, 0, 0, 0 ],   // upper pants
            [ 0, 2, 2, 2, 0, 0, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 2, 2, 0, 0, 2, 2, 2, 0, 0, 0 ],
            [ 0, 6, 6, 6, 0, 0, 6, 6, 6, 0, 0, 0 ],   // shoes
            [ 0, 6, 7, 6, 0, 0, 6, 7, 6, 0, 0, 0 ],
        ],
        // Frame 1 — left foot forward
        [
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0 ],
            [ 0, 2, 2, 2, 5, 5, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 2, 2, 2, 3, 2, 0, 0, 0 ],
            [ 2, 2, 2, 2, 0, 0, 2, 2, 2, 0, 0, 0 ],   // left leg out
            [ 2, 2, 2, 0, 0, 0, 0, 2, 2, 0, 0, 0 ],
            [ 6, 6, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0 ],
            [ 6, 7, 0, 0, 0, 0, 0, 6, 7, 0, 0, 0 ],
        ],
        // Frame 2 — crossing
        [
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0 ],
            [ 0, 2, 2, 2, 5, 5, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 2, 2, 2, 3, 2, 0, 0, 0 ],
            [ 0, 2, 2, 3, 2, 2, 3, 2, 2, 0, 0, 0 ],   // legs crossing
            [ 0, 0, 2, 2, 3, 3, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0 ],
            [ 0, 0, 7, 6, 6, 6, 6, 7, 0, 0, 0, 0 ],
        ],
        // Frame 3 — right foot forward
        [
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0 ],
            [ 0, 2, 2, 2, 5, 5, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 2, 2, 2, 3, 2, 0, 0, 0 ],
            [ 0, 2, 2, 2, 0, 0, 2, 2, 2, 2, 0, 0 ],   // right leg out
            [ 0, 2, 2, 0, 0, 0, 0, 2, 2, 2, 0, 0 ],
            [ 0, 6, 6, 0, 0, 0, 0, 0, 6, 6, 0, 0 ],
            [ 0, 6, 7, 0, 0, 0, 0, 0, 7, 6, 0, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs1 =   // formal trousers (crease line)
    [
        // Frame 0 — idle
        [
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0 ],
            [ 0, 2, 2, 2, 5, 5, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 2, 2, 2, 3, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 0, 0, 2, 3, 2, 0, 0, 0 ],   // crease highlights on edges
            [ 0, 2, 3, 2, 0, 0, 2, 3, 2, 0, 0, 0 ],
            [ 0, 6, 6, 6, 0, 0, 6, 6, 6, 0, 0, 0 ],
            [ 0, 6, 7, 6, 0, 0, 6, 7, 6, 0, 0, 0 ],
        ],
        // Frame 1 — left foot forward
        [
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0 ],
            [ 0, 2, 2, 2, 5, 5, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 2, 2, 2, 3, 2, 0, 0, 0 ],
            [ 2, 2, 3, 2, 0, 0, 2, 3, 2, 0, 0, 0 ],
            [ 2, 2, 3, 0, 0, 0, 0, 2, 3, 0, 0, 0 ],
            [ 6, 6, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0 ],
            [ 6, 7, 0, 0, 0, 0, 0, 6, 7, 0, 0, 0 ],
        ],
        // Frame 2 — crossing
        [
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0 ],
            [ 0, 2, 2, 2, 5, 5, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 2, 2, 2, 3, 2, 0, 0, 0 ],
            [ 0, 2, 3, 3, 2, 2, 3, 3, 2, 0, 0, 0 ],
            [ 0, 0, 2, 3, 3, 3, 3, 2, 0, 0, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0 ],
            [ 0, 0, 7, 6, 6, 6, 6, 7, 0, 0, 0, 0 ],
        ],
        // Frame 3 — right foot forward
        [
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0 ],
            [ 0, 2, 2, 2, 5, 5, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 2, 2, 2, 3, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 0, 0, 2, 3, 2, 2, 0, 0 ],
            [ 0, 2, 3, 0, 0, 0, 0, 2, 3, 2, 0, 0 ],
            [ 0, 6, 6, 0, 0, 0, 0, 0, 6, 6, 0, 0 ],
            [ 0, 6, 7, 0, 0, 0, 0, 0, 7, 6, 0, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs2 =   // shorts + bare skin below knee
    [
        // Frame 0 — idle
        [
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0 ],
            [ 0, 2, 2, 2, 5, 5, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 2, 2, 2, 3, 2, 0, 0, 0 ],   // shorts
            [ 0, 2, 2, 2, 0, 0, 2, 2, 2, 0, 0, 0 ],   // shorts end
            [ 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0 ],   // bare legs
            [ 0, 6, 6, 6, 0, 0, 6, 6, 6, 0, 0, 0 ],   // shoes
            [ 0, 6, 7, 6, 0, 0, 6, 7, 6, 0, 0, 0 ],
        ],
        // Frame 1 — left foot forward
        [
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0 ],
            [ 0, 2, 2, 2, 5, 5, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 2, 2, 2, 3, 2, 0, 0, 0 ],
            [ 1, 1, 1, 2, 0, 0, 2, 1, 1, 0, 0, 0 ],   // left bare leg out
            [ 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0 ],
            [ 6, 6, 0, 0, 0, 0, 0, 6, 6, 0, 0, 0 ],
            [ 6, 7, 0, 0, 0, 0, 0, 6, 7, 0, 0, 0 ],
        ],
        // Frame 2 — crossing
        [
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0 ],
            [ 0, 2, 2, 2, 5, 5, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 2, 2, 2, 3, 2, 0, 0, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 ],   // bare legs crossing
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0 ],
            [ 0, 0, 7, 6, 6, 6, 6, 7, 0, 0, 0, 0 ],
        ],
        // Frame 3 — right foot forward
        [
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0 ],
            [ 0, 2, 2, 2, 5, 5, 2, 2, 2, 0, 0, 0 ],
            [ 0, 2, 3, 2, 2, 2, 2, 3, 2, 0, 0, 0 ],
            [ 0, 1, 1, 2, 0, 0, 2, 1, 1, 1, 0, 0 ],   // right bare leg out
            [ 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0 ],
            [ 0, 6, 6, 0, 0, 0, 0, 0, 6, 6, 0, 0 ],
            [ 0, 6, 7, 0, 0, 0, 0, 0, 7, 6, 0, 0 ],
        ],
    ];

    public override byte[][][][] LegsParts { get; } = [ _legs0, _legs1, _legs2 ];

    // ── Head palettes (VIC-II skin/hair tones) ────────────────────────────────

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("fair/blonde",
            Skin:      new Color(221, 179, 130),
            Hair:      new Color(238, 238, 119),   // VIC-II yellow
            Highlight: new Color(235, 205, 155),
            Eyes:      new Color( 64,  20, 255),   // VIC-II blue
            Accessory: new Color(191, 235,   0)),
        new("fair/brown",
            Skin:      new Color(221, 179, 130),
            Hair:      new Color(102,  68,   0),   // brown
            Highlight: new Color(235, 205, 155),
            Eyes:      new Color( 51,  51,  51),
            Accessory: new Color(102,  68,   0)),
        new("medium/black",
            Skin:      new Color(180, 130,  80),
            Hair:      new Color(  0,   0,   0),
            Highlight: new Color(200, 155, 100),
            Eyes:      new Color( 51,  51,  51),
            Accessory: new Color(  0,   0,   0)),
        new("dark/black",
            Skin:      new Color(119,  85,  34),
            Hair:      new Color(  0,   0,   0),
            Highlight: new Color(145, 100,  50),
            Eyes:      new Color(  0, 227, 246),   // VIC-II cyan
            Accessory: new Color(  0,   0,   0)),
        new("medium/auburn",
            Skin:      new Color(180, 130,  80),
            Hair:      new Color(136,   0,   0),   // VIC-II red — auburn
            Highlight: new Color(200, 155, 100),
            Eyes:      new Color( 64,  20, 255),
            Accessory: new Color(136,   0,   0)),
    ];

    // ── Body palettes ─────────────────────────────────────────────────────────

    public override BodyPalette[] BodyPalettes { get; } =
    [
        new("green",
            Skin:           new Color(221, 179, 130),
            Shirt:          new Color(  1, 223,   0),   // VIC-II green
            ShirtHighlight: new Color(170, 255, 102),
            Buttons:        new Color(255, 255, 255),
            Accessory:      new Color(  0, 227, 246)),
        new("blue",
            Skin:           new Color(221, 179, 130),
            Shirt:          new Color( 64,  20, 255),   // VIC-II blue
            ShirtHighlight: new Color(  0, 136, 255),
            Buttons:        new Color(172, 172, 172),
            Accessory:      new Color(238, 238, 119)),
        new("red",
            Skin:           new Color(221, 179, 130),
            Shirt:          new Color(136,   0,   0),   // VIC-II red
            ShirtHighlight: new Color(255, 119, 119),
            Buttons:        new Color(255, 255, 255),
            Accessory:      new Color(238, 238, 119)),
        new("white/light",
            Skin:           new Color(221, 179, 130),
            Shirt:          new Color(172, 172, 172),
            ShirtHighlight: new Color(255, 255, 255),
            Buttons:        new Color( 64,  20, 255),
            Accessory:      new Color(  1, 223,   0)),
        new("teal",
            Skin:           new Color(221, 179, 130),
            Shirt:          new Color(  0, 136, 255),
            ShirtHighlight: new Color(  0, 227, 246),
            Buttons:        new Color(255, 255, 255),
            Accessory:      new Color(204,  68, 204)),
    ];

    // ── Legs palettes ─────────────────────────────────────────────────────────

    public override LegsPalette[] LegsPalettes { get; } =
    [
        new("blue jeans/brown shoes",
            Skin:           new Color(221, 179, 130),
            Pants:          new Color( 64,  20, 255),   // VIC-II blue
            PantsHighlight: new Color(  0, 136, 255),
            Belt:           new Color(102,  68,   0),
            BeltHighlight:  new Color(221, 136,  85),
            Shoes:          new Color( 72,  40,   0),
            ShoeHighlight:  new Color(132,  96,  60)),
        new("black/black",
            Skin:           new Color(221, 179, 130),
            Pants:          new Color(  0,   0,   0),
            PantsHighlight: new Color( 51,  51,  51),
            Belt:           new Color( 51,  51,  51),
            BeltHighlight:  new Color(120, 120, 120),
            Shoes:          new Color(  0,   0,   0),
            ShoeHighlight:  new Color( 51,  51,  51)),
        new("khaki/tan",
            Skin:           new Color(221, 179, 130),
            Pants:          new Color(221, 136,  85),
            PantsHighlight: new Color(238, 238, 119),
            Belt:           new Color(102,  68,   0),
            BeltHighlight:  new Color(180,  90,   0),
            Shoes:          new Color(102,  68,   0),
            ShoeHighlight:  new Color(150, 105,  40)),
        new("gray/dark",
            Skin:           new Color(221, 179, 130),
            Pants:          new Color(120, 120, 120),
            PantsHighlight: new Color(172, 172, 172),
            Belt:           new Color( 51,  51,  51),
            BeltHighlight:  new Color(120, 120, 120),
            Shoes:          new Color( 51,  51,  51),
            ShoeHighlight:  new Color(120, 120, 120)),
    ];
}
