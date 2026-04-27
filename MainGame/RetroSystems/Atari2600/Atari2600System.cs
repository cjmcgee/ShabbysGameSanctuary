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

    // ── Sprite dimensions ─────────────────────────────────────────────────────
    // HeadRows=5, BodyRows=7, LegsRows=4  (total 16)

    public override int CharWidth  => 8;
    public override int HeadRows   => 5;
    public override int BodyRows   => 7;
    public override int LegsRows   => 4;

    // ── Head parts (8 wide × 5 rows, 1 frame) ────────────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory

    // Head 0: basic Adventure-style round head
    private static readonly byte[][][] _head0 =
    [[
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],
        [ 0, 1, 1, 1, 1, 1, 1, 0 ],
        [ 0, 1, 4, 1, 1, 4, 1, 0 ],   // eyes (4)
        [ 0, 1, 1, 1, 1, 1, 1, 0 ],
        [ 0, 0, 1, 2, 2, 1, 0, 0 ],   // hair base (2)
    ]];

    // Head 1: cap / hat
    private static readonly byte[][][] _head1 =
    [[
        [ 0, 5, 5, 5, 5, 5, 5, 0 ],   // hat (5)
        [ 0, 5, 5, 5, 5, 5, 5, 0 ],   // hat brim
        [ 0, 1, 4, 1, 1, 4, 1, 0 ],   // eyes (4)
        [ 0, 1, 1, 1, 1, 1, 1, 0 ],
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],
    ]];

    // Head 2: full hair
    private static readonly byte[][][] _head2 =
    [[
        [ 0, 2, 2, 2, 2, 2, 2, 0 ],   // hair top (2)
        [ 0, 2, 1, 1, 1, 1, 2, 0 ],
        [ 0, 2, 4, 1, 1, 4, 2, 0 ],   // eyes (4)
        [ 0, 2, 1, 1, 1, 1, 2, 0 ],
        [ 0, 2, 1, 1, 1, 1, 2, 0 ],
    ]];

    public override byte[][][][] HeadParts { get; } = [ _head0, _head1, _head2 ];

    // ── Body parts (8 wide × 7 rows, 1 frame) ────────────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory

    // Body 0: casual shirt
    private static readonly byte[][][] _body0 =
    [[
        [ 0, 1, 1, 1, 1, 1, 1, 0 ],   // neck (skin=1)
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt (2)
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 3, 2, 2, 2, 2, 3, 2 ],   // side highlights (3)
        [ 2, 3, 2, 2, 2, 2, 3, 2 ],
        [ 0, 2, 2, 4, 4, 2, 2, 0 ],   // buttons (4)
    ]];

    // Body 1: collared / formal
    private static readonly byte[][][] _body1 =
    [[
        [ 0, 1, 5, 1, 1, 5, 1, 0 ],   // collar (5)
        [ 2, 5, 2, 4, 4, 2, 5, 2 ],   // lapels + buttons
        [ 2, 5, 2, 4, 4, 2, 5, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 3, 2, 2, 2, 2, 3, 2 ],
        [ 0, 2, 2, 2, 2, 2, 2, 0 ],
    ]];

    // Body 2: jacket / hoodie
    private static readonly byte[][][] _body2 =
    [[
        [ 0, 1, 1, 1, 1, 1, 1, 0 ],   // neck (skin=1)
        [ 5, 5, 2, 2, 2, 2, 5, 5 ],   // jacket (5) + shirt (2)
        [ 5, 5, 2, 2, 2, 2, 5, 5 ],
        [ 5, 2, 2, 3, 3, 2, 2, 5 ],   // shirt highlight (3)
        [ 5, 2, 2, 4, 4, 2, 2, 5 ],   // buttons (4)
        [ 5, 5, 2, 2, 2, 2, 5, 5 ],
        [ 0, 5, 2, 2, 2, 2, 5, 0 ],
    ]];

    public override byte[][][][] BodyParts { get; } = [ _body0, _body1, _body2 ];

    // ── Legs parts (8 wide × 4 rows, 4 frames) ───────────────────────────────
    // Atari legs are very blocky — only 4 pixel rows

    // Legs 0: pants + belt
    private static readonly byte[][][] _legs0 =
    [
        [   // idle
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],   // belt (4)
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 6, 6, 0, 0, 6, 6, 0 ],   // shoes (6)
        ],
        [   // walk A
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 2, 0, 0, 0, 2, 0 ],
            [ 2, 2, 0, 0, 0, 0, 2, 2 ],
            [ 7, 6, 0, 0, 0, 0, 6, 7 ],   // shoe highlight (7)
        ],
        [   // mid
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 0, 0, 0, 2, 2, 0 ],
            [ 0, 2, 2, 0, 0, 0, 2, 2 ],
            [ 0, 6, 7, 0, 0, 0, 7, 6 ],
        ],
    ];

    // Legs 1: formal (no belt)
    private static readonly byte[][][] _legs1 =
    [
        [   // idle
            [ 0, 2, 2, 2, 2, 2, 2, 0 ],
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 6, 7, 0, 0, 6, 7, 0 ],
        ],
        [   // walk A
            [ 0, 2, 2, 2, 2, 2, 2, 0 ],
            [ 0, 2, 2, 0, 0, 0, 2, 0 ],
            [ 2, 2, 0, 0, 0, 0, 2, 2 ],
            [ 7, 6, 0, 0, 0, 0, 6, 7 ],
        ],
        [   // mid
            [ 0, 2, 2, 2, 2, 2, 2, 0 ],
            [ 0, 2, 2, 0, 0, 2, 2, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 2, 2, 2, 2, 2, 2, 0 ],
            [ 0, 2, 0, 0, 0, 2, 2, 0 ],
            [ 0, 2, 2, 0, 0, 0, 2, 2 ],
            [ 0, 6, 7, 0, 0, 0, 7, 6 ],
        ],
    ];

    // Legs 2: shorts (bare skin = 1)
    private static readonly byte[][][] _legs2 =
    [
        [   // idle
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 2, 1, 1, 2, 2, 0 ],   // shorts + bare skin (1)
            [ 0, 1, 1, 0, 0, 1, 1, 0 ],   // bare legs
            [ 0, 6, 6, 0, 0, 6, 6, 0 ],
        ],
        [   // walk A
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 1, 2, 0, 0, 0, 1, 0 ],
            [ 1, 1, 0, 0, 0, 0, 1, 1 ],
            [ 7, 6, 0, 0, 0, 0, 6, 7 ],
        ],
        [   // mid
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 2, 1, 1, 1, 1, 2, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 4, 2, 2, 2, 2, 4, 0 ],
            [ 0, 1, 0, 0, 0, 2, 1, 0 ],
            [ 0, 1, 1, 0, 0, 0, 1, 1 ],
            [ 0, 6, 7, 0, 0, 0, 7, 6 ],
        ],
    ];

    public override byte[][][][] LegsParts { get; } = [ _legs0, _legs1, _legs2 ];

    // ── Head palettes (5) — drawn from NTSC Atari palette ────────────────────

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("Fair/Blonde",
            Skin:      new Color(220, 220, 200),   // near-white (fair skin)
            Hair:      new Color(220, 220,   0),   // bright yellow (blonde)
            Highlight: new Color(255, 255, 255),
            Eyes:      new Color( 28,  28,  40),
            Accessory: new Color(188, 140,  56)),  // amber (brown hat)

        new("Fair/Brown",
            Skin:      new Color(220, 220, 200),
            Hair:      new Color(124,  72,   8),   // dark brown
            Highlight: new Color(255, 255, 255),
            Eyes:      new Color( 28,  28,  40),
            Accessory: new Color(124,  72,   8)),

        new("Medium/Black",
            Skin:      new Color(188, 140,  56),   // warm amber (medium skin)
            Hair:      new Color( 20,  12,   4),   // near-black
            Highlight: new Color(220, 220, 200),
            Eyes:      new Color( 20,  12,   4),
            Accessory: new Color(104, 104, 104)),

        new("Dark/Black",
            Skin:      new Color(132,   4,   4),   // dark red (dark skin)
            Hair:      new Color( 20,  12,   4),
            Highlight: new Color(188, 140,  56),
            Eyes:      new Color( 28,  28,  40),
            Accessory: new Color(104, 104, 104)),

        new("Medium/Auburn",
            Skin:      new Color(188, 140,  56),
            Hair:      new Color(200,  20,  20),   // vivid red
            Highlight: new Color(220, 220, 200),
            Eyes:      new Color( 28,  28,  40),
            Accessory: new Color(104, 104, 104)),
    ];

    // ── Body palettes (5) ────────────────────────────────────────────────────

    public override BodyPalette[] BodyPalettes { get; } =
    [
        new("Green",
            Skin:           new Color(220, 220, 200),
            Shirt:          new Color(  0, 188,   0),   // vivid green
            ShirtHighlight: new Color(  0, 200, 220),   // cyan highlight
            Buttons:        new Color( 20,  12,   4),
            Accessory:      new Color(  0, 100,   0)),

        new("Blue",
            Skin:           new Color(220, 220, 200),
            Shirt:          new Color( 36,  80, 200),   // bold blue
            ShirtHighlight: new Color(  0, 200, 220),
            Buttons:        new Color( 20,  12,   4),
            Accessory:      new Color( 20,  12,   4)),

        new("Red",
            Skin:           new Color(220, 220, 200),
            Shirt:          new Color(200,  20,  20),   // vivid red
            ShirtHighlight: new Color(220, 220, 200),
            Buttons:        new Color( 20,  12,   4),
            Accessory:      new Color(132,   4,   4)),

        new("White",
            Skin:           new Color(220, 220, 200),
            Shirt:          new Color(220, 220, 200),   // near-white
            ShirtHighlight: new Color(255, 255, 255),
            Buttons:        new Color(104, 104, 104),
            Accessory:      new Color(148, 148, 132)),

        new("Teal",
            Skin:           new Color(220, 220, 200),
            Shirt:          new Color(  0, 200, 220),   // bright cyan
            ShirtHighlight: new Color(220, 220, 200),
            Buttons:        new Color( 20,  12,   4),
            Accessory:      new Color(  0, 100, 120)),
    ];

    // ── Legs palettes (4) ────────────────────────────────────────────────────

    public override LegsPalette[] LegsPalettes { get; } =
    [
        new("Blue Jeans/Brown",
            Skin:          new Color(220, 220, 200),
            Pants:         new Color( 36,  80, 200),
            PantsHighlight:new Color(  0, 200, 220),
            Belt:          new Color(188, 140,  56),
            BeltHighlight: new Color(220, 220,   0),
            Shoes:         new Color(124,  72,   8),
            ShoeHighlight: new Color(188, 140,  56)),

        new("Black/Black",
            Skin:          new Color(220, 220, 200),
            Pants:         new Color( 20,  12,   4),
            PantsHighlight:new Color(104, 104, 104),
            Belt:          new Color(104, 104, 104),
            BeltHighlight: new Color(148, 148, 132),
            Shoes:         new Color( 20,  12,   4),
            ShoeHighlight: new Color(104, 104, 104)),

        new("Khaki/Tan",
            Skin:          new Color(220, 220, 200),
            Pants:         new Color(220, 220,   0),
            PantsHighlight:new Color(220, 220, 200),
            Belt:          new Color(124,  72,   8),
            BeltHighlight: new Color(220, 220,   0),
            Shoes:         new Color(124,  72,   8),
            ShoeHighlight: new Color(188, 140,  56)),

        new("Gray/Dark",
            Skin:          new Color(220, 220, 200),
            Pants:         new Color(148, 148, 132),
            PantsHighlight:new Color(220, 220, 200),
            Belt:          new Color(104, 104, 104),
            BeltHighlight: new Color(148, 148, 132),
            Shoes:         new Color(104, 104, 104),
            ShoeHighlight: new Color( 20,  12,   4)),
    ];
}
