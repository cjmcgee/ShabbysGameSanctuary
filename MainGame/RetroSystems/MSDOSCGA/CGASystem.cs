using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.MSDOSCGA;

/// <summary>
/// MS-DOS CGA visual style — mode 4, palette 1 (high intensity).
///
/// Authentic characteristics:
///   • 8×8 tiles at 320×200 native resolution
///   • 4-color fixed palette: black, bright cyan, bright magenta, white
///     (IBM CGA mode 4 palette 1 — the most iconic CGA look)
///   • Character sprites: 8×16 (CGA text-mode character proportions)
///   • DisplayScale 3.0: a 600px-tall viewport shows exactly 200 world pixels (native height)
///   • MaxZoomOutArea (640×400) limits zoom-out to 2× native in each direction
///
/// Sprite split: HeadRows=4, BodyRows=6, LegsRows=6 (total 16)
/// </summary>
public sealed class CGASystem : RetroSystem
{
    public override string Name        => "MS-DOS CGA";
    public override string Description => "8×8 tiles · 320×200 · Mode 4 palette 1 (black/cyan/magenta/white)";
    public override int    NativeTileSize => 8;
    public override float  DisplayScale   => 3.0f;

    // Native CGA: 320×200. MaxZoomOutArea caps at 2× native in each direction.
    public override Vector2? MaxZoomOutArea => new Vector2(640, 400);

    // ── CGA mode-4 palette 1 (high intensity) ────────────────────────────────
    protected override Color[] TilePalette { get; } =
    [
        new Color(  0,   0,   0),   //  0 black
        new Color( 85, 255, 255),   //  1 bright cyan
        new Color(255,  85, 255),   //  2 bright magenta
        new Color(255, 255, 255),   //  3 white
    ];

    // ── Tile pixel art (8×8, indices 0–3 only) ───────────────────────────────

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

    // Magenta planks (2), black grain seams (0)
    private static readonly byte[][] WoodFloor =
    [
        [  2,  2,  2,  0,  2,  2,  2,  0 ],
        [  2,  2,  0,  2,  2,  2,  0,  2 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  2,  0,  2,  2,  0,  2,  2,  0 ],
        [  2,  2,  2,  0,  2,  2,  2,  0 ],
        [  2,  2,  0,  2,  2,  2,  0,  2 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  2,  0,  2,  2,  0,  2,  2,  0 ],
    ];

    // Magenta fill (2), cyan corner-dot detail (1)
    private static readonly byte[][] Carpet =
    [
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
        [  2,  1,  1,  2,  2,  1,  1,  2 ],
        [  2,  1,  2,  2,  2,  2,  1,  2 ],
        [  2,  2,  2,  1,  1,  2,  2,  2 ],
        [  2,  2,  2,  1,  1,  2,  2,  2 ],
        [  2,  1,  2,  2,  2,  2,  1,  2 ],
        [  2,  1,  1,  2,  2,  1,  1,  2 ],
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
    ];

    // White tile faces (3), black grout (0)
    private static readonly byte[][] KitchenTile =
    [
        [  3,  3,  0,  3,  3,  3,  0,  3 ],
        [  3,  3,  0,  3,  3,  3,  0,  3 ],
        [  3,  3,  0,  3,  3,  3,  0,  3 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  3,  3,  0,  3,  3,  3,  0,  3 ],
        [  3,  3,  0,  3,  3,  3,  0,  3 ],
        [  3,  3,  0,  3,  3,  3,  0,  3 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
    ];

    // Cyan brick faces (1), black mortar (0)
    private static readonly byte[][] Wall =
    [
        [  1,  1,  1,  0,  1,  1,  1,  0 ],
        [  1,  1,  1,  0,  1,  1,  1,  0 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  0,  1,  1,  1,  0,  1,  1,  1 ],
        [  0,  1,  1,  1,  0,  1,  1,  1 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  1,  1,  1,  0,  1,  1,  1,  0 ],
        [  1,  1,  1,  0,  1,  1,  1,  0 ],
    ];

    // Magenta frame (2), black recessed panels (0), white knob (3)
    private static readonly byte[][] Door =
    [
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
        [  2,  0,  0,  2,  2,  0,  0,  2 ],
        [  2,  0,  0,  2,  2,  0,  0,  2 ],
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
        [  2,  0,  0,  0,  0,  0,  0,  2 ],
        [  2,  0,  3,  0,  0,  0,  0,  2 ],
        [  2,  0,  0,  0,  0,  0,  0,  2 ],
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
    ];

    // Black frame (0), cyan glass panes (1)
    private static readonly byte[][] Window =
    [
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  0,  1,  1,  0,  1,  1,  1,  0 ],
        [  0,  1,  1,  0,  1,  1,  1,  0 ],
        [  0,  1,  1,  0,  1,  1,  1,  0 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  0,  1,  1,  1,  1,  1,  1,  0 ],
        [  0,  1,  1,  1,  1,  1,  1,  0 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
    ];

    // Cyan outer border (1), black interior (0)
    private static readonly byte[][] Furniture =
    [
        [  1,  1,  1,  1,  1,  1,  1,  1 ],
        [  1,  0,  0,  0,  0,  0,  0,  1 ],
        [  1,  0,  1,  1,  1,  1,  0,  1 ],
        [  1,  0,  1,  0,  0,  1,  0,  1 ],
        [  1,  0,  1,  0,  0,  1,  0,  1 ],
        [  1,  0,  0,  0,  0,  0,  0,  1 ],
        [  1,  1,  1,  1,  1,  1,  1,  1 ],
        [  1,  1,  1,  1,  1,  1,  1,  1 ],
    ];

    // White surface (3), cyan inset border (1), black base (0)
    private static readonly byte[][] Counter =
    [
        [  3,  3,  3,  3,  3,  3,  3,  3 ],
        [  3,  1,  1,  1,  1,  1,  1,  3 ],
        [  3,  1,  3,  3,  3,  3,  1,  3 ],
        [  3,  1,  3,  3,  3,  3,  1,  3 ],
        [  3,  1,  3,  3,  3,  3,  1,  3 ],
        [  3,  1,  1,  1,  1,  1,  1,  3 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
    ];

    // Black shelves (0), cyan/magenta/white book spines cycling
    private static readonly byte[][] Bookshelf =
    [
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  1,  2,  1,  3,  1,  2,  3,  1 ],
        [  1,  2,  1,  3,  1,  2,  3,  1 ],
        [  1,  2,  1,  3,  1,  2,  3,  1 ],
        [  1,  2,  1,  3,  1,  2,  3,  1 ],
        [  1,  2,  1,  3,  1,  2,  3,  1 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
    ];

    // Cyan foliage (1), magenta pot (2), white soil strip (3)
    private static readonly byte[][] Plant =
    [
        [  0,  0,  0,  1,  1,  0,  0,  0 ],
        [  0,  0,  1,  1,  1,  1,  0,  0 ],
        [  0,  0,  1,  1,  1,  1,  0,  0 ],
        [  0,  0,  0,  1,  1,  0,  0,  0 ],
        [  0,  0,  2,  2,  2,  2,  0,  0 ],
        [  0,  0,  2,  3,  3,  2,  0,  0 ],
        [  0,  0,  2,  2,  2,  2,  0,  0 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
    ];

    // Cyan fill (1), black texture spots (0)
    private static readonly byte[][] Grass =
    [
        [  1,  1,  1,  1,  1,  1,  1,  1 ],
        [  1,  1,  1,  1,  1,  1,  1,  1 ],
        [  1,  1,  0,  1,  1,  1,  0,  1 ],
        [  1,  0,  1,  1,  1,  0,  1,  1 ],
        [  1,  1,  1,  1,  1,  1,  1,  1 ],
        [  1,  1,  1,  0,  1,  1,  1,  1 ],
        [  1,  1,  0,  1,  1,  1,  0,  1 ],
        [  1,  0,  1,  1,  1,  0,  1,  1 ],
    ];

    // Black surface (0), white centre line (3)
    private static readonly byte[][] Road =
    [
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  3,  3,  3,  3,  3,  3,  3,  3 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
    ];

    // White slabs (3), black expansion joints (0)
    private static readonly byte[][] Sidewalk =
    [
        [  3,  3,  3,  3,  3,  3,  3,  3 ],
        [  3,  3,  3,  3,  3,  3,  3,  3 ],
        [  3,  3,  3,  3,  3,  3,  3,  3 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
        [  3,  3,  3,  3,  3,  3,  3,  3 ],
        [  3,  3,  3,  3,  3,  3,  3,  3 ],
        [  3,  3,  3,  3,  3,  3,  3,  3 ],
        [  0,  0,  0,  0,  0,  0,  0,  0 ],
    ];

    // Solid magenta accent
    private static readonly byte[][] Accent =
    [
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
        [  2,  2,  2,  2,  2,  2,  2,  2 ],
    ];

    // ── Sprite dimensions ─────────────────────────────────────────────────────

    public override int CharWidth  => 8;
    public override int HeadRows   => 4;
    public override int BodyRows   => 6;
    public override int LegsRows   => 6;

    // ── Head parts (8 wide × 4 rows, 1 frame each) ───────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory

    private static readonly byte[][][] _head0 =
    [[
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],
        [ 0, 1, 1, 4, 1, 4, 1, 0 ],
        [ 0, 1, 1, 1, 1, 1, 1, 0 ],
        [ 0, 0, 1, 2, 2, 1, 0, 0 ],
    ]];

    private static readonly byte[][][] _head1 =
    [[
        [ 0, 5, 5, 5, 5, 5, 5, 0 ],
        [ 0, 5, 1, 1, 1, 1, 5, 0 ],
        [ 0, 1, 4, 1, 1, 4, 1, 0 ],
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],
    ]];

    private static readonly byte[][][] _head2 =
    [[
        [ 0, 2, 2, 2, 2, 2, 2, 0 ],
        [ 0, 2, 1, 1, 1, 1, 2, 0 ],
        [ 0, 2, 4, 1, 1, 4, 2, 0 ],
        [ 0, 2, 1, 1, 1, 1, 2, 0 ],
    ]];

    public override byte[][][][] HeadParts { get; } = [ _head0, _head1, _head2 ];

    // ── Body parts (8 wide × 6 rows, 1 frame each) ───────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory

    private static readonly byte[][][] _body0 =
    [[
        [ 0, 1, 1, 1, 1, 1, 1, 0 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 3, 2, 2, 2, 2, 3, 2 ],
        [ 0, 2, 2, 4, 4, 2, 2, 0 ],
    ]];

    private static readonly byte[][][] _body1 =
    [[
        [ 0, 1, 5, 1, 1, 5, 1, 0 ],
        [ 2, 5, 2, 4, 4, 2, 5, 2 ],
        [ 2, 5, 2, 4, 4, 2, 5, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 3, 2, 2, 2, 2, 3, 2 ],
        [ 0, 2, 2, 2, 2, 2, 2, 0 ],
    ]];

    private static readonly byte[][][] _body2 =
    [[
        [ 0, 1, 1, 1, 1, 1, 1, 0 ],
        [ 5, 5, 2, 2, 2, 2, 5, 5 ],
        [ 5, 2, 2, 3, 3, 2, 2, 5 ],
        [ 5, 2, 2, 4, 4, 2, 2, 5 ],
        [ 5, 5, 2, 2, 2, 2, 5, 5 ],
        [ 0, 5, 2, 2, 2, 2, 5, 0 ],
    ]];

    public override byte[][][][] BodyParts { get; } = [ _body0, _body1, _body2 ];

    // ── Legs parts (8 wide × 6 rows, 4 walk frames) ──────────────────────────
    // Semantic: 1=Skin  2=Pants  3=PantsHighlight  4=Belt  6=Shoes  7=ShoeHighlight

    private static readonly byte[][][] _legs0 =
    [
        [   // idle
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 3, 0, 0, 3, 2, 0 ],
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 6, 7, 0, 0, 6, 7, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk A
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 3, 0, 0, 0, 2, 0 ],
            [ 2, 2, 0, 0, 0, 0, 2, 0 ],
            [ 2, 2, 0, 0, 0, 0, 2, 2 ],
            [ 7, 0, 0, 0, 0, 0, 6, 7 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // mid-stride
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 3, 0, 0, 3, 2, 0 ],
            [ 0, 2, 0, 0, 0, 0, 2, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 0, 0, 0, 3, 2, 0 ],
            [ 0, 2, 0, 0, 0, 0, 2, 2 ],
            [ 0, 2, 2, 0, 0, 0, 2, 2 ],
            [ 0, 7, 6, 0, 0, 0, 7, 7 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs1 =
    [
        [   // idle
            [ 0, 2, 2, 2, 2, 2, 2, 0 ],
            [ 0, 2, 3, 0, 0, 3, 2, 0 ],
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 6, 6, 0, 0, 6, 6, 0 ],
            [ 0, 7, 7, 0, 0, 7, 7, 0 ],
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

    private static readonly byte[][][] _legs2 =
    [
        [   // idle
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 3, 1, 1, 3, 2, 0 ],
            [ 0, 2, 2, 1, 1, 2, 2, 0 ],
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],
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

    // ── Head palettes — all resolved colors are from the 4-color CGA set ─────
    // Black=(0,0,0)  Cyan=(85,255,255)  Magenta=(255,85,255)  White=(255,255,255)

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("fair/blonde",
            Skin:      new Color(255, 255, 255),   // white
            Hair:      new Color(255,  85, 255),   // magenta (closest to blonde)
            Highlight: new Color(255, 255, 255),   // white
            Eyes:      new Color( 85, 255, 255),   // cyan
            Accessory: new Color(255,  85, 255)),  // magenta

        new("fair/dark-hair",
            Skin:      new Color(255, 255, 255),   // white
            Hair:      new Color(  0,   0,   0),   // black
            Highlight: new Color(255, 255, 255),   // white
            Eyes:      new Color(  0,   0,   0),   // black
            Accessory: new Color( 85, 255, 255)),  // cyan

        new("medium/black",
            Skin:      new Color( 85, 255, 255),   // cyan
            Hair:      new Color(  0,   0,   0),   // black
            Highlight: new Color(255, 255, 255),   // white
            Eyes:      new Color(  0,   0,   0),   // black
            Accessory: new Color(  0,   0,   0)),  // black

        new("dark/black",
            Skin:      new Color(255,  85, 255),   // magenta
            Hair:      new Color(  0,   0,   0),   // black
            Highlight: new Color( 85, 255, 255),   // cyan
            Eyes:      new Color(255, 255, 255),   // white (contrast)
            Accessory: new Color(  0,   0,   0)),  // black

        new("medium/magenta-hair",
            Skin:      new Color( 85, 255, 255),   // cyan
            Hair:      new Color(255,  85, 255),   // magenta
            Highlight: new Color(255, 255, 255),   // white
            Eyes:      new Color(  0,   0,   0),   // black
            Accessory: new Color(255,  85, 255)),  // magenta
    ];

    // ── Body palettes ─────────────────────────────────────────────────────────

    public override BodyPalette[] BodyPalettes { get; } =
    [
        new("cyan",
            Skin:           new Color(255, 255, 255),   // white
            Shirt:          new Color( 85, 255, 255),   // cyan
            ShirtHighlight: new Color(255, 255, 255),   // white
            Buttons:        new Color(  0,   0,   0),   // black
            Accessory:      new Color(255,  85, 255)),  // magenta

        new("magenta",
            Skin:           new Color(255, 255, 255),   // white
            Shirt:          new Color(255,  85, 255),   // magenta
            ShirtHighlight: new Color(255, 255, 255),   // white
            Buttons:        new Color(  0,   0,   0),   // black
            Accessory:      new Color( 85, 255, 255)),  // cyan

        new("white",
            Skin:           new Color(255, 255, 255),   // white
            Shirt:          new Color(255, 255, 255),   // white
            ShirtHighlight: new Color( 85, 255, 255),   // cyan (highlight distinguishable)
            Buttons:        new Color(  0,   0,   0),   // black
            Accessory:      new Color(255,  85, 255)),  // magenta

        new("black",
            Skin:           new Color(255, 255, 255),   // white
            Shirt:          new Color(  0,   0,   0),   // black
            ShirtHighlight: new Color( 85, 255, 255),   // cyan
            Buttons:        new Color(255, 255, 255),   // white
            Accessory:      new Color(255,  85, 255)),  // magenta

        new("magenta jacket",
            Skin:           new Color(255, 255, 255),   // white
            Shirt:          new Color(255, 255, 255),   // white shirt under jacket
            ShirtHighlight: new Color( 85, 255, 255),   // cyan
            Buttons:        new Color(  0,   0,   0),   // black
            Accessory:      new Color(255,  85, 255)),  // magenta jacket
    ];

    // ── Legs palettes ─────────────────────────────────────────────────────────

    public override LegsPalette[] LegsPalettes { get; } =
    [
        new("cyan pants",
            Skin:           new Color(255, 255, 255),   // white
            Pants:          new Color( 85, 255, 255),   // cyan
            PantsHighlight: new Color(255, 255, 255),   // white
            Belt:           new Color(  0,   0,   0),   // black
            BeltHighlight:  new Color(255, 255, 255),   // white
            Shoes:          new Color(  0,   0,   0),   // black
            ShoeHighlight:  new Color(255,  85, 255)),  // magenta

        new("magenta pants",
            Skin:           new Color(255, 255, 255),   // white
            Pants:          new Color(255,  85, 255),   // magenta
            PantsHighlight: new Color(255, 255, 255),   // white
            Belt:           new Color(  0,   0,   0),   // black
            BeltHighlight:  new Color(255, 255, 255),   // white
            Shoes:          new Color(  0,   0,   0),   // black
            ShoeHighlight:  new Color( 85, 255, 255)),  // cyan

        new("black pants",
            Skin:           new Color(255, 255, 255),   // white
            Pants:          new Color(  0,   0,   0),   // black
            PantsHighlight: new Color( 85, 255, 255),   // cyan
            Belt:           new Color(255,  85, 255),   // magenta
            BeltHighlight:  new Color(255, 255, 255),   // white
            Shoes:          new Color(  0,   0,   0),   // black
            ShoeHighlight:  new Color(255, 255, 255)),  // white

        new("white pants",
            Skin:           new Color(255, 255, 255),   // white
            Pants:          new Color(255, 255, 255),   // white
            PantsHighlight: new Color( 85, 255, 255),   // cyan
            Belt:           new Color(  0,   0,   0),   // black
            BeltHighlight:  new Color(255, 255, 255),   // white
            Shoes:          new Color(255,  85, 255),   // magenta
            ShoeHighlight:  new Color(255, 255, 255)),  // white
    ];
}
