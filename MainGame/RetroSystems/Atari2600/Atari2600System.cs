using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.Atari2600;

/// <summary>
/// Atari 2600 visual style.
///
/// Authentic characteristics:
///   • 8×8 native tile resolution (upscaled to 16×16 for the engine)
///   • 1-bit tile color: each tile uses only black (bg) and one foreground color
///   • Sprites limited to one color per horizontal scanline (TIA player register style)
///   • 128-color NTSC palette; sprites are hard-edged silhouettes
///   • Character sprites: 8×16 (Adventure-duck proportions)
///   • Native screen: 160×192 (double-wide pixels → 80 unique columns × 192 rows)
///   • DisplayScale ≈3.125 so a 600px-tall viewport shows ~192 world pixels (native height)
///   • MaxZoomOutArea (320×384) limits zoom-out to 2× native in each direction
///   • Double-wide pixels: each logical pixel occupies two adjacent horizontal pixels;
///     all odd columns equal the preceding even column.
/// </summary>
public sealed class Atari2600System : RetroSystem
{
    public override string Name        => "Atari 2600";
    public override string Description => "8×8 tiles · 1-bit tiles · 1-color/scanline sprites";
    public override int    NativeTileSize    => 8;
    public override float  DisplayScale      => 3.125f;
    protected override bool DoubleWidePixels          => true;
    protected override bool OneBitTiles               => true;
    protected override bool SpriteOneColorPerScanline => true;

    // 2× the Atari's native 160×192 screen — the most the camera will ever reveal.
    public override Vector2? MaxZoomOutArea => new Vector2(320, 384);

    // ── Tile palette ─────────────────────────────────────────────────────────
    // Index 0 = background fill (black).
    // Each tile uses exactly index 0 (black) plus ONE non-zero index below.
    protected override Color[] TilePalette { get; } =
    [
        new Color(  0,   0,   0),   // 0  black          — background
        new Color(188, 140,  56),   // 1  warm amber      — WoodFloor
        new Color(124,  72,   8),   // 2  dark brown      — (sprite palette; unused by tiles)
        new Color(200,  20,  20),   // 3  vivid red       — Carpet, Bookshelf
        new Color(132,   4,   4),   // 4  dark red        — (sprite palette; unused by tiles)
        new Color(220, 220,   0),   // 5  bright yellow   — KitchenTile
        new Color(220, 220, 200),   // 6  near-white      — Wall
        new Color(148, 148, 132),   // 7  light gray      — (sprite palette; unused by tiles)
        new Color( 20,  12,   4),   // 8  near-black      — Door
        new Color( 36,  80, 200),   // 9  bold blue       — Furniture
        new Color(144, 144, 144),   // 10 mid gray        — Counter
        new Color(  0, 200, 220),   // 11 bright cyan     — Window
        new Color(  0, 188,   0),   // 12 vivid green     — Grass, Plant
        new Color( 28,  28,  40),   // 13 near-black road — Road
        new Color(104, 104, 104),   // 14 medium gray     — Sidewalk
    ];

    // ── Tile pixel art (8×8, palette indices) ────────────────────────────────
    // Rules: (1) only index 0 and ONE non-zero foreground index per tile.
    //        (2) double-wide: col[2k+1] == col[2k] for all k.

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

    // Amber planks — horizontal dividers staggered grain dots (fg=1)
    private static readonly byte[][] WoodFloor =
    [
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 0, 0, 1, 1, 1, 1 ],   // left-half grain dot
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],   // plank divider
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 1, 1, 1, 1, 0, 0, 1, 1 ],   // right-half grain dot (staggered)
        [ 1, 1, 1, 1, 1, 1, 1, 1 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],   // plank divider
    ];

    // Solid vivid red carpet (fg=3)
    private static readonly byte[][] Carpet =
    [
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],
    ];

    // Yellow ceramic tiles — 2×2 grid with black grout (fg=5)
    private static readonly byte[][] KitchenTile =
    [
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],
        [ 5, 5, 0, 0, 5, 5, 0, 0 ],   // vertical grout at logical cols 1 and 3
        [ 5, 5, 0, 0, 5, 5, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],   // horizontal grout
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],
        [ 5, 5, 0, 0, 5, 5, 0, 0 ],
        [ 5, 5, 0, 0, 5, 5, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],   // horizontal grout
    ];

    // Near-white brick wall — rows of bricks with horizontal mortar (fg=6)
    private static readonly byte[][] Wall =
    [
        [ 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],   // mortar
        [ 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 6, 6, 6, 6, 6, 6, 6, 6 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],   // mortar
        [ 6, 6, 6, 6, 6, 6, 6, 6 ],
    ];

    // Near-black door — two recessed panels (fg=8)
    private static readonly byte[][] Door =
    [
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
        [ 8, 8, 0, 0, 0, 0, 8, 8 ],   // upper panel
        [ 8, 8, 0, 0, 0, 0, 8, 8 ],
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
        [ 8, 8, 0, 0, 0, 0, 8, 8 ],   // lower panel
        [ 8, 8, 0, 0, 0, 0, 8, 8 ],
        [ 8, 8, 8, 8, 8, 8, 8, 8 ],
    ];

    // Cyan window — black frame, two glass panes (fg=11)
    private static readonly byte[][] Window =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],   // top frame
        [ 0, 0,11,11,11,11, 0, 0 ],   // upper pane
        [ 0, 0,11,11,11,11, 0, 0 ],
        [ 0, 0,11,11,11,11, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],   // crossbar
        [ 0, 0,11,11,11,11, 0, 0 ],   // lower pane
        [ 0, 0,11,11,11,11, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],   // bottom frame
    ];

    // Bold blue furniture — dark inset panel (fg=9)
    private static readonly byte[][] Furniture =
    [
        [ 9, 9, 9, 9, 9, 9, 9, 9 ],
        [ 9, 9, 9, 9, 9, 9, 9, 9 ],
        [ 9, 9, 0, 0, 0, 0, 9, 9 ],
        [ 9, 9, 0, 0, 0, 0, 9, 9 ],
        [ 9, 9, 0, 0, 0, 0, 9, 9 ],
        [ 9, 9, 0, 0, 0, 0, 9, 9 ],
        [ 9, 9, 9, 9, 9, 9, 9, 9 ],
        [ 9, 9, 9, 9, 9, 9, 9, 9 ],
    ];

    // Solid mid-gray counter (fg=10)
    private static readonly byte[][] Counter =
    [
        [10,10,10,10,10,10,10,10 ],
        [10,10,10,10,10,10,10,10 ],
        [10,10,10,10,10,10,10,10 ],
        [10,10,10,10,10,10,10,10 ],
        [10,10,10,10,10,10,10,10 ],
        [10,10,10,10,10,10,10,10 ],
        [10,10,10,10,10,10,10,10 ],
        [10,10,10,10,10,10,10,10 ],
    ];

    // Red bookshelf — alternating vertical book columns + shelf dividers (fg=3)
    private static readonly byte[][] Bookshelf =
    [
        [ 3, 3, 0, 0, 3, 3, 0, 0 ],
        [ 3, 3, 0, 0, 3, 3, 0, 0 ],
        [ 3, 3, 0, 0, 3, 3, 0, 0 ],
        [ 3, 3, 0, 0, 3, 3, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],   // shelf divider
        [ 3, 3, 0, 0, 3, 3, 0, 0 ],
        [ 3, 3, 0, 0, 3, 3, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],   // base shelf
    ];

    // Green plant silhouette — bushy top, narrow stem/pot column (fg=12)
    private static readonly byte[][] Plant =
    [
        [ 0, 0, 0, 0,12,12, 0, 0 ],
        [12,12,12,12,12,12, 0, 0 ],
        [12,12,12,12,12,12, 0, 0 ],
        [ 0, 0,12,12,12,12, 0, 0 ],
        [ 0, 0, 0, 0,12,12, 0, 0 ],   // stem
        [ 0, 0, 0, 0,12,12, 0, 0 ],
        [ 0, 0, 0, 0,12,12, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];

    // Solid vivid green grass (fg=12)
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

    // Solid near-black asphalt road (fg=13)
    private static readonly byte[][] Road =
    [
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
        [13,13,13,13,13,13,13,13 ],
    ];

    // Medium gray concrete slabs — horizontal crack joints (fg=14)
    private static readonly byte[][] Sidewalk =
    [
        [14,14,14,14,14,14,14,14 ],
        [14,14,14,14,14,14,14,14 ],
        [14,14,14,14,14,14,14,14 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],   // crack joint
        [14,14,14,14,14,14,14,14 ],
        [14,14,14,14,14,14,14,14 ],
        [14,14,14,14,14,14,14,14 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0 ],   // crack joint
    ];

    // Solid runtime accent color (fg=15)
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
    // 4 logical pixels per row (2 physical pixels each).

    public override int CharWidth  => 8;
    public override int HeadRows   => 5;
    public override int BodyRows   => 7;
    public override int LegsRows   => 4;

    // ── Head parts (8 wide × 5 rows, 1 frame) ────────────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory
    // Rule: each row uses at most ONE non-zero semantic index.

    // Head 0: hair on top, dedicated eye row
    private static readonly byte[][][] _head0 =
    [[
        [ 0, 0, 2, 2, 2, 2, 0, 0 ],   // hair
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // upper face
        [ 0, 0, 4, 4, 4, 4, 0, 0 ],   // eyes
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // chin
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // neck
    ]];

    // Head 1: cap / hat with full brim (already compliant)
    private static readonly byte[][][] _head1 =
    [[
        [ 0, 0, 5, 5, 5, 5, 0, 0 ],   // hat top
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],   // hat brim
        [ 0, 0, 4, 4, 4, 4, 0, 0 ],   // eyes
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // face
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // neck
    ]];

    // Head 2: full hair — wide hair rows then face rows
    private static readonly byte[][][] _head2 =
    [[
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // hair top
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // hair wide
        [ 0, 0, 4, 4, 4, 4, 0, 0 ],   // eyes
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // face/chin
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // neck
    ]];

    public override byte[][][][] HeadParts { get; } = [ _head0, _head1, _head2 ];

    // ── Body parts (8 wide × 7 rows, 1 frame) ────────────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory
    // Rule: each row uses at most ONE non-zero semantic index.

    // Body 0: casual shirt — dedicated rows for buttons and highlight
    private static readonly byte[][][] _body0 =
    [[
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // neck
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 0, 0, 4, 4, 4, 4, 0, 0 ],   // buttons
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],   // full-row highlight
        [ 0, 0, 2, 2, 2, 2, 0, 0 ],   // bottom narrow
    ]];

    // Body 1: collared / formal — full-row lapels, then shirt, buttons, highlight
    private static readonly byte[][][] _body1 =
    [[
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // neck
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],   // lapels / collar (full row)
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 0, 0, 4, 4, 4, 4, 0, 0 ],   // buttons
        [ 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 3, 3, 3, 3, 3, 3, 3, 3 ],   // full-row highlight
        [ 0, 0, 2, 2, 2, 2, 0, 0 ],   // bottom narrow
    ]];

    // Body 2: jacket — jacket rows wrap shirt/button rows
    private static readonly byte[][][] _body2 =
    [[
        [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // neck
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],   // jacket
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],   // jacket
        [ 0, 0, 2, 2, 2, 2, 0, 0 ],   // shirt centre visible
        [ 0, 0, 4, 4, 4, 4, 0, 0 ],   // buttons
        [ 5, 5, 5, 5, 5, 5, 5, 5 ],   // jacket
        [ 0, 0, 5, 5, 5, 5, 0, 0 ],   // jacket bottom narrow
    ]];

    public override byte[][][][] BodyParts { get; } = [ _body0, _body1, _body2 ];

    // ── Legs parts (8 wide × 4 rows, 4 frames) ───────────────────────────────
    // Idle: legs merged at centre; walk: legs spread to outer logical pixels.
    // Rule: each row uses at most ONE non-zero semantic index.

    // Legs 0: pants + belt (already compliant)
    private static readonly byte[][][] _legs0 =
    [
        [   // idle
            [ 0, 0, 4, 4, 4, 4, 0, 0 ],   // belt
            [ 0, 0, 2, 2, 2, 2, 0, 0 ],   // pants
            [ 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 0, 0 ],   // shoes
        ],
        [   // walk A — left foot forward
            [ 0, 0, 4, 4, 4, 4, 0, 0 ],
            [ 2, 2, 2, 2, 0, 0, 0, 0 ],   // left leg out
            [ 2, 2, 0, 0, 0, 0, 2, 2 ],
            [ 6, 6, 0, 0, 0, 0, 6, 6 ],   // shoes spread
        ],
        [   // mid
            [ 0, 0, 4, 4, 4, 4, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B — right foot forward
            [ 0, 0, 4, 4, 4, 4, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2 ],   // right leg out
            [ 2, 2, 0, 0, 0, 0, 2, 2 ],
            [ 6, 6, 0, 0, 0, 0, 6, 6 ],
        ],
    ];

    // Legs 1: formal trousers — single shoe color per scanline
    private static readonly byte[][][] _legs1 =
    [
        [   // idle
            [ 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 0, 0 ],   // shoes (single color)
        ],
        [   // walk A
            [ 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 2, 2, 0, 0, 0, 0, 2, 2 ],
            [ 6, 6, 0, 0, 0, 0, 6, 6 ],   // shoes (single color)
        ],
        [   // mid
            [ 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2 ],
            [ 2, 2, 0, 0, 0, 0, 2, 2 ],
            [ 6, 6, 0, 0, 0, 0, 6, 6 ],   // shoes (single color)
        ],
    ];

    // Legs 2: shorts + bare skin below knee (already compliant)
    private static readonly byte[][][] _legs2 =
    [
        [   // idle
            [ 0, 0, 4, 4, 4, 4, 0, 0 ],   // belt
            [ 0, 0, 2, 2, 2, 2, 0, 0 ],   // shorts
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],   // bare legs
            [ 0, 0, 6, 6, 6, 6, 0, 0 ],   // shoes
        ],
        [   // walk A
            [ 0, 0, 4, 4, 4, 4, 0, 0 ],
            [ 2, 2, 2, 2, 0, 0, 0, 0 ],   // left shorts out
            [ 1, 1, 0, 0, 0, 0, 1, 1 ],   // bare legs spread
            [ 6, 6, 0, 0, 0, 0, 6, 6 ],
        ],
        [   // mid
            [ 0, 0, 4, 4, 4, 4, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 0, 4, 4, 4, 4, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2 ],   // right shorts out
            [ 1, 1, 0, 0, 0, 0, 1, 1 ],
            [ 6, 6, 0, 0, 0, 0, 6, 6 ],
        ],
    ];

    public override byte[][][][] LegsParts { get; } = [ _legs0, _legs1, _legs2 ];

    // ── Head palettes (5) — drawn from NTSC Atari palette ────────────────────

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("Fair/Blonde",
            Skin:      new Color(220, 220, 200),
            Hair:      new Color(220, 220,   0),   // bright yellow
            Highlight: new Color(255, 255, 255),
            Eyes:      new Color( 28,  28,  40),
            Accessory: new Color(188, 140,  56)),  // amber hat

        new("Fair/Brown",
            Skin:      new Color(220, 220, 200),
            Hair:      new Color(124,  72,   8),   // dark brown
            Highlight: new Color(255, 255, 255),
            Eyes:      new Color( 28,  28,  40),
            Accessory: new Color(124,  72,   8)),

        new("Medium/Black",
            Skin:      new Color(188, 140,  56),   // warm amber
            Hair:      new Color( 20,  12,   4),   // near-black
            Highlight: new Color(220, 220, 200),
            Eyes:      new Color( 20,  12,   4),
            Accessory: new Color(104, 104, 104)),

        new("Dark/Black",
            Skin:      new Color(132,   4,   4),   // dark red
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
            Shirt:          new Color(  0, 188,   0),
            ShirtHighlight: new Color(  0, 200, 220),
            Buttons:        new Color( 20,  12,   4),
            Accessory:      new Color(  0, 100,   0)),

        new("Blue",
            Skin:           new Color(220, 220, 200),
            Shirt:          new Color( 36,  80, 200),
            ShirtHighlight: new Color(  0, 200, 220),
            Buttons:        new Color( 20,  12,   4),
            Accessory:      new Color( 20,  12,   4)),

        new("Red",
            Skin:           new Color(220, 220, 200),
            Shirt:          new Color(200,  20,  20),
            ShirtHighlight: new Color(220, 220, 200),
            Buttons:        new Color( 20,  12,   4),
            Accessory:      new Color(132,   4,   4)),

        new("White",
            Skin:           new Color(220, 220, 200),
            Shirt:          new Color(220, 220, 200),
            ShirtHighlight: new Color(255, 255, 255),
            Buttons:        new Color(104, 104, 104),
            Accessory:      new Color(148, 148, 132)),

        new("Teal",
            Skin:           new Color(220, 220, 200),
            Shirt:          new Color(  0, 200, 220),
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
