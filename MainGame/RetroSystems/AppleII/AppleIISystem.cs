using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.AppleII;

/// <summary>
/// Apple II visual style — hi-res graphics mode.
///
/// Authentic characteristics:
///   • 8×8 tile art with double-wide pixels (effective 4 unique columns per tile)
///   • 6-color hi-res palette: black, green, violet, white, orange, blue
///     (NTSC artifact colors from the 1-MHz pixel clock — the defining Apple II look)
///   • Character sprites: 8×16 with double-wide pixels (HeadRows=4, BodyRows=7, LegsRows=5)
///   • Native screen: 280×192, effective 140×192 (double-wide pixels halve unique horizontal resolution)
///   • DisplayScale 3.125 shows exactly 192 world pixels tall at a 600px viewport
///   • MaxZoomOutArea (280×384) limits zoom-out to 2× the effective 140×192 native in each direction
///   • Double-wide pixels: each logical pixel occupies two adjacent horizontal pixels;
///     all odd columns equal the preceding even column.
/// </summary>
public sealed class AppleIISystem : RetroSystem
{
    public override string Name        => "Apple II";
    public override string Description => "8×8 tiles · 140×192 effective · Hi-res 6-color artifact palette";
    public override int    NativeTileSize => 8;
    public override float  DisplayScale   => 3.125f;
    protected override bool DoubleWidePixels => true;

    // Effective Apple II Hi-res: 140×192. MaxZoomOutArea caps at 2× effective native.
    public override Vector2? MaxZoomOutArea => new Vector2(280, 384);

    // TODO: need to simulate color fringing for any text!
    // ── Apple II hi-res 6-color palette ──────────────────────────────────────
    protected override Color[] TilePalette { get; } =
    [
        new Color(  0,   0,   0),   //  0 black
        new Color( 20, 245,  60),   //  1 green
        new Color(193,  28, 255),   //  2 violet
        new Color(255, 255, 255),   //  3 white
        new Color(255, 106,  60),   //  4 orange
        new Color( 20,  88, 255),   //  5 blue
    ];

    // ── Tile pixel art (8×8, double-wide rule: col[2k+1]==col[2k]) ───────────

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

    // Orange planks (4), black grain seams (0)
    private static readonly byte[][] WoodFloor =
    [
        [ 4, 4, 4, 4, 0, 0, 4, 4 ],
        [ 4, 4, 0, 0, 4, 4, 4, 4 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 4, 4, 4, 4, 0, 0, 4, 4 ],
        [ 4, 4, 4, 4, 0, 0, 4, 4 ],
        [ 4, 4, 0, 0, 4, 4, 4, 4 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 4, 4, 0, 0, 4, 4, 4, 4 ],
    ];

    // Green fill (1), violet corner dots (2)
    private static readonly byte[][] Carpet =
    [
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 2, 2, 1, 1, 2, 2 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 2, 2, 2, 2, 1, 1 ],
        [ 1, 1, 2, 2, 2, 2, 1, 1 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 2, 2, 1, 1, 2, 2 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
    ];

    // White tile faces (3), black grout (0)
    private static readonly byte[][] KitchenTile =
    [
        [ 3, 3, 3, 3, 0, 0, 3, 3 ],
        [ 3, 3, 3, 3, 0, 0, 3, 3 ],
        [ 3, 3, 3, 3, 0, 0, 3, 3 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 3, 3, 3, 3, 0, 0, 3, 3 ],
        [ 3, 3, 3, 3, 0, 0, 3, 3 ],
        [ 3, 3, 3, 3, 0, 0, 3, 3 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // White brick faces (3), blue mortar (5)
    private static readonly byte[][] Wall =
    [
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 5, 5, 3, 3, 5, 5 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 5, 5, 3, 3, 3, 3, 5, 5 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],
    ];

    // Black frame (0), violet recessed panels (2), white knob (3)
    private static readonly byte[][] Door =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 2, 2, 2, 2, 0, 0 ],
        [ 0, 0, 2, 2, 2, 2, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 2, 2, 2, 2, 0, 0 ],
        [ 0, 0, 3, 3, 2, 2, 0, 0 ],
        [ 0, 0, 2, 2, 2, 2, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // Black frame (0), blue glass panes (5)
    private static readonly byte[][] Window =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 5, 5, 5, 5, 0, 0 ],
        [ 0, 0, 5, 5, 5, 5, 0, 0 ],
        [ 0, 0, 5, 5, 5, 5, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 5, 5, 5, 5, 0, 0 ],
        [ 0, 0, 5, 5, 5, 5, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // Blue border (5), black interior (0)
    private static readonly byte[][] Furniture =
    [
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],
        [ 5, 5, 0, 0, 0, 0, 5, 5 ],
        [ 5, 5, 0, 0, 0, 0, 5, 5 ],
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],
        [ 5, 5, 0, 0, 0, 0, 5, 5 ],
        [ 5, 5, 0, 0, 0, 0, 5, 5 ],
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],
    ];

    // Blue outer border (5), white inner surface (3), black base (0)
    private static readonly byte[][] Counter =
    [
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],
        [ 5, 5, 3, 3, 3, 3, 5, 5 ],
        [ 5, 5, 3, 3, 3, 3, 5, 5 ],
        [ 5, 5, 3, 3, 3, 3, 5, 5 ],
        [ 5, 5, 3, 3, 3, 3, 5, 5 ],
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // Black shelves (0), cycling green/violet/orange book spines
    private static readonly byte[][] Bookshelf =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 1, 1, 2, 2, 4, 4, 1, 1 ],
        [ 1, 1, 2, 2, 4, 4, 1, 1 ],
        [ 1, 1, 2, 2, 4, 4, 1, 1 ],
        [ 1, 1, 2, 2, 4, 4, 1, 1 ],
        [ 1, 1, 2, 2, 4, 4, 1, 1 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // Green foliage (1), orange pot (4)
    private static readonly byte[][] Plant =
    [
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],
        [ 0, 0, 4, 4, 4, 4, 0, 0 ],
        [ 0, 0, 4, 4, 4, 4, 0, 0 ],
        [ 0, 0, 4, 4, 4, 4, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // Green fill (1), black texture spots (0)
    private static readonly byte[][] Grass =
    [
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 0, 0, 1, 1, 0, 0 ],
        [ 0, 0, 1, 1, 0, 0, 1, 1 ],
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 0, 0, 1, 1, 1, 1 ],
        [ 1, 1, 1, 1, 0, 0, 1, 1 ],
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],
    ];

    // Black road (0), white centre line (3)
    private static readonly byte[][] Road =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // White slabs (3), blue expansion joints (5)
    private static readonly byte[][] Sidewalk =
    [
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 5, 5, 3, 3, 5, 5 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 5, 5, 3, 3, 5, 5 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
    ];

    // Violet accent with green dot pattern
    private static readonly byte[][] Accent =
    [
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 2, 1, 1, 2, 2, 1, 1 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 1, 1, 2, 2, 1, 1, 2, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 2, 1, 1, 2, 2, 1, 1 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 1, 1, 2, 2, 1, 1, 2, 2 ],
    ];

    // ── Sprite dimensions (8×16 total, double-wide: 4 unique columns) ─────────

    public override int CharWidth  => 8;
    public override int HeadRows   => 4;
    public override int BodyRows   => 7;
    public override int LegsRows   => 5;

    // ── Head variants (1 frame × 4 rows × 8 cols) ────────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory
    // Double-wide: col[2k+1] == col[2k]

    private static readonly byte[][][] _head0 =   // basic round head
    [
        [
            [ 2, 2, 2, 2, 2, 2, 0, 0 ],   // hair crown
            [ 2, 2, 1, 1, 1, 1, 0, 0 ],   // hair + face
            [ 2, 2, 4, 4, 1, 1, 0, 0 ],   // hair + eye + face
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // chin
        ],
    ];

    private static readonly byte[][][] _head1 =   // cap / hat
    [
        [
            [ 5, 5, 5, 5, 5, 5, 5, 5 ],   // hat top
            [ 5, 5, 5, 5, 5, 5, 5, 5 ],   // hat brim
            [ 0, 0, 4, 4, 1, 1, 0, 0 ],   // eye + face
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // chin
        ],
    ];

    private static readonly byte[][][] _head2 =   // long / full hair
    [
        [
            [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // full hair row
            [ 2, 2, 1, 1, 1, 1, 2, 2 ],   // hair sides + face
            [ 2, 2, 4, 4, 1, 1, 2, 2 ],   // hair + eye + face
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // chin
        ],
    ];

    public override byte[][][][] HeadParts { get; } = [ _head0, _head1, _head2 ];

    // ── Body variants (1 frame × 7 rows × 8 cols) ────────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory

    private static readonly byte[][][] _body0 =   // casual shirt
    [
        [
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // neck
            [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // shoulders
            [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // chest
            [ 2, 2, 3, 3, 3, 3, 2, 2 ],   // highlights
            [ 2, 2, 4, 4, 4, 4, 2, 2 ],   // buttons
            [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // mid
            [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // lower
        ],
    ];

    private static readonly byte[][][] _body1 =   // collared / formal
    [
        [
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // neck
            [ 5, 5, 2, 2, 2, 2, 5, 5 ],   // lapels top
            [ 5, 5, 4, 4, 4, 4, 5, 5 ],   // lapels + buttons
            [ 5, 5, 2, 2, 2, 2, 5, 5 ],   // lapels lower
            [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // torso
            [ 2, 2, 4, 4, 4, 4, 2, 2 ],   // lower buttons
            [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // bottom
        ],
    ];

    private static readonly byte[][][] _body2 =   // jacket / hoodie
    [
        [
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // neck
            [ 5, 5, 5, 5, 5, 5, 5, 5 ],   // jacket full
            [ 5, 5, 2, 2, 2, 2, 5, 5 ],   // open jacket / shirt
            [ 5, 5, 3, 3, 4, 4, 5, 5 ],   // highlight + button
            [ 5, 5, 2, 2, 2, 2, 5, 5 ],   // shirt
            [ 5, 5, 3, 3, 3, 3, 5, 5 ],   // highlights
            [ 5, 5, 5, 5, 5, 5, 5, 5 ],   // jacket hem
        ],
    ];

    public override byte[][][][] BodyParts { get; } = [ _body0, _body1, _body2 ];

    // ── Legs variants (4 frames × 5 rows × 8 cols) ───────────────────────────
    // Semantic: 1=Skin(bare)  2=Pants  3=PantsHighlight  4=Belt
    //           5=BeltHighlight  6=Shoes  7=ShoeHighlight

    private static readonly byte[][][] _legs0 =   // pants + belt
    [
        [   // idle
            [ 4, 4, 4, 4, 4, 4, 4, 4 ],
            [ 2, 2, 5, 5, 5, 5, 2, 2 ],   // buckle
            [ 2, 2, 0, 0, 0, 0, 2, 2 ],
            [ 6, 6, 0, 0, 0, 0, 6, 6 ],
            [ 7, 7, 0, 0, 0, 0, 6, 6 ],
        ],
        [   // left foot forward
            [ 4, 4, 4, 4, 4, 4, 4, 4 ],
            [ 2, 2, 5, 5, 5, 5, 2, 2 ],
            [ 2, 2, 2, 2, 0, 0, 2, 2 ],
            [ 2, 2, 0, 0, 0, 0, 6, 6 ],
            [ 6, 6, 0, 0, 0, 0, 7, 7 ],
        ],
        [   // crossing
            [ 4, 4, 4, 4, 4, 4, 4, 4 ],
            [ 2, 2, 5, 5, 5, 5, 2, 2 ],
            [ 0, 0, 3, 3, 3, 3, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 0, 0 ],
            [ 0, 0, 7, 7, 6, 6, 0, 0 ],
        ],
        [   // right foot forward
            [ 4, 4, 4, 4, 4, 4, 4, 4 ],
            [ 2, 2, 5, 5, 5, 5, 2, 2 ],
            [ 2, 2, 0, 0, 2, 2, 2, 2 ],
            [ 6, 6, 0, 0, 0, 0, 2, 2 ],
            [ 7, 7, 0, 0, 0, 0, 6, 6 ],
        ],
    ];

    private static readonly byte[][][] _legs1 =   // formal trousers (crease)
    [
        [   // idle
            [ 4, 4, 4, 4, 4, 4, 4, 4 ],
            [ 2, 2, 3, 3, 3, 3, 2, 2 ],   // crease
            [ 2, 2, 3, 3, 3, 3, 2, 2 ],   // crease continues
            [ 6, 6, 0, 0, 0, 0, 6, 6 ],
            [ 7, 7, 0, 0, 0, 0, 6, 6 ],
        ],
        [   // left foot forward
            [ 4, 4, 4, 4, 4, 4, 4, 4 ],
            [ 2, 2, 3, 3, 3, 3, 2, 2 ],
            [ 2, 2, 2, 2, 0, 0, 2, 2 ],
            [ 2, 2, 0, 0, 0, 0, 6, 6 ],
            [ 6, 6, 0, 0, 0, 0, 7, 7 ],
        ],
        [   // crossing
            [ 4, 4, 4, 4, 4, 4, 4, 4 ],
            [ 2, 2, 3, 3, 3, 3, 2, 2 ],
            [ 0, 0, 3, 3, 3, 3, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 0, 0 ],
            [ 0, 0, 7, 7, 6, 6, 0, 0 ],
        ],
        [   // right foot forward
            [ 4, 4, 4, 4, 4, 4, 4, 4 ],
            [ 2, 2, 3, 3, 3, 3, 2, 2 ],
            [ 2, 2, 0, 0, 2, 2, 2, 2 ],
            [ 6, 6, 0, 0, 0, 0, 2, 2 ],
            [ 7, 7, 0, 0, 0, 0, 6, 6 ],
        ],
    ];

    private static readonly byte[][][] _legs2 =   // shorts + bare skin
    [
        [   // idle
            [ 4, 4, 4, 4, 4, 4, 4, 4 ],
            [ 2, 2, 5, 5, 5, 5, 2, 2 ],   // shorts
            [ 1, 1, 0, 0, 0, 0, 1, 1 ],   // bare legs
            [ 6, 6, 0, 0, 0, 0, 6, 6 ],
            [ 7, 7, 0, 0, 0, 0, 6, 6 ],
        ],
        [   // left foot forward
            [ 4, 4, 4, 4, 4, 4, 4, 4 ],
            [ 2, 2, 5, 5, 5, 5, 2, 2 ],
            [ 1, 1, 1, 1, 0, 0, 1, 1 ],
            [ 1, 1, 0, 0, 0, 0, 6, 6 ],
            [ 6, 6, 0, 0, 0, 0, 7, 7 ],
        ],
        [   // crossing
            [ 4, 4, 4, 4, 4, 4, 4, 4 ],
            [ 2, 2, 5, 5, 5, 5, 2, 2 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 0, 0 ],
            [ 0, 0, 7, 7, 6, 6, 0, 0 ],
        ],
        [   // right foot forward
            [ 4, 4, 4, 4, 4, 4, 4, 4 ],
            [ 2, 2, 5, 5, 5, 5, 2, 2 ],
            [ 1, 1, 0, 0, 1, 1, 1, 1 ],
            [ 6, 6, 0, 0, 0, 0, 1, 1 ],
            [ 7, 7, 0, 0, 0, 0, 6, 6 ],
        ],
    ];

    public override byte[][][][] LegsParts { get; } = [ _legs0, _legs1, _legs2 ];

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
