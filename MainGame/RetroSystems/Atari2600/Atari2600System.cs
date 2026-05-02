using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.Atari2600;

/// <summary>
/// Atari 2600 visual style.
///
/// Authentic characteristics:
///   • 16-px tile art (NativeTilePixels=16); hardware Atari 2600 tiles are 8×8 in spirit
///   • 1-bit tile color: each tile uses only black (bg) and one foreground color
///   • Sprites limited to one color per horizontal scanline (TIA player register style)
///   • 128-color NTSC palette; sprites are hard-edged silhouettes
///   • Character sprites: 16×16 texture (CharWidth=16, HeadRows+BodyRows+LegsRows=16)
///   • Native screen: 160×192 (double-wide pixels → 80 unique columns × 192 rows)
///   • DefaultTilesTall = 12, MaxTilesTall = 24 (~2× zoom out)
///   • Double-wide pixels: each logical pixel occupies two adjacent horizontal pixels;
///     all odd columns equal the preceding even column.
/// </summary>
public sealed class Atari2600System : RetroSystem
{
    public override string Name        => "Atari 2600";
    public override string Description => "16-px tiles dbl-wide · 16×16 sprites · 12 tiles tall · 1-bit · 1-color/scanline sprites";
    public override int   NativeTilePixels  => 16;
    public override float DefaultTilesTall  => 12f;   // native 160×192 → 12 tiles vertically
    public override float MaxTilesTall      => 24f;   // ~2× zoom out

    protected override bool DoubleWidePixels          => true;
    protected override bool OneBitTiles               => true;
    protected override bool SpriteOneColorPerScanline => true;

    // ── Tile palette ─────────────────────────────────────────────────────────
    // Index 0 = background fill (black).
    // Each tile uses exactly index 0 (black) plus ONE non-zero index below.
    protected override Palette TilePalette { get; } = new Atari2600Palette();

    // ── Tile pixel art (16×16, palette indices) ──────────────────────────────
    // Rules: (1) only index 0 and ONE non-zero foreground index per tile.
    //        (2) double-wide: col[2k+1] == col[2k] for all k (8 logical cols).

    protected override byte[][] GetTilePixels(TileType type, Color accentColor) => type switch
    {
        TileType.WoodFloor     => Atari2600Tiles.WoodFloor,
        TileType.Carpet        => Atari2600Tiles.Carpet,
        TileType.KitchenTile   => Atari2600Tiles.KitchenTile,
        TileType.Wall          => Atari2600Tiles.Wall,
        TileType.Door          => Atari2600Tiles.Door,
        TileType.Window        => Atari2600Tiles.Window,
        TileType.Furniture     => Atari2600Tiles.Furniture,
        TileType.Counter       => Atari2600Tiles.Counter,
        TileType.Bookshelf     => Atari2600Tiles.Bookshelf,
        TileType.Plant         => Atari2600Tiles.Plant,
        TileType.Grass         => Atari2600Tiles.Grass,
        TileType.Road          => Atari2600Tiles.Road,
        TileType.Sidewalk      => Atari2600Tiles.Sidewalk,
        TileType.HouseExterior => Atari2600Tiles.Wall,
        TileType.Accent        => Atari2600Tiles.Accent,
        _ => Atari2600Tiles.Wall
    };

    // ── Sprite dimensions ─────────────────────────────────────────────────────
    // HeadRows=5, BodyRows=7, LegsRows=4  (total 16)
    // 8 logical pixels per row (2 physical pixels each, double-wide).

    public override int CharWidth  => 16;
    public override int HeadRows   => 5;
    public override int BodyRows   => 7;
    public override int LegsRows   => 4;

    // ── Head parts (16 wide × 5 rows, 1 frame) ───────────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory
    // Rule: each row uses at most ONE non-zero semantic index.
    // Double-wide: col[2k+1] == col[2k] for all k.

    // Head 0: hair on top, dedicated eye row
    private static readonly byte[][][] _head0 =
    [[
        [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // hair
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // upper face
        [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // eyes
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
    ]];

    // Head 1: cap / hat with full brim
    private static readonly byte[][][] _head1 =
    [[
        [ 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],   // hat crown
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // hat brim
        [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // eyes
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // face
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
    ]];

    // Head 2: full hair — wide hair rows then face rows
    private static readonly byte[][][] _head2 =
    [[
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // hair top
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // hair wide
        [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // eyes
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // face/chin
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
    ]];

    public override byte[][][][] HeadParts { get; } = [ _head0, _head1, _head2 ];

    // ── Body parts (16 wide × 7 rows, 1 frame) ───────────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory
    // Rule: each row uses at most ONE non-zero semantic index.
    // Double-wide: col[2k+1] == col[2k] for all k.

    // Body 0: casual shirt — dedicated rows for buttons and highlight
    private static readonly byte[][][] _body0 =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // buttons
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 ],   // full-row highlight
        [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // bottom narrow
    ]];

    // Body 1: collared / formal — full-row lapels, then shirt, buttons, highlight
    private static readonly byte[][][] _body1 =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // lapels / collar
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // buttons
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 ],   // full-row highlight
        [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // bottom narrow
    ]];

    // Body 2: jacket — jacket rows wrap shirt/button rows
    private static readonly byte[][][] _body2 =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // jacket
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // jacket
        [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // shirt centre visible
        [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // buttons
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // jacket
        [ 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],   // jacket bottom narrow
    ]];

    public override byte[][][][] BodyParts { get; } = [ _body0, _body1, _body2 ];

    // ── Legs parts (16 wide × 4 rows, 4 frames) ──────────────────────────────
    // Idle: legs merged at centre; walk: legs spread to outer logical pixels.
    // Rule: each row uses at most ONE non-zero semantic index.
    // Double-wide: col[2k+1] == col[2k] for all k.

    // Legs 0: pants + belt
    private static readonly byte[][][] _legs0 =
    [
        [   // idle
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // belt
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // pants
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // pants
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0 ],   // shoes
        ],
        [   // walk A — left foot forward
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // belt
            [ 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],   // left leg out
            [ 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2 ],   // legs spread
            [ 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],   // shoes spread
        ],
        [   // mid
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // belt
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // pants
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B — right foot forward
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // belt
            [ 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2 ],   // right leg out
            [ 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2 ],   // legs spread
            [ 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],   // shoes spread
        ],
    ];

    // Legs 1: formal trousers — single shoe color per scanline
    private static readonly byte[][][] _legs1 =
    [
        [   // idle
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0 ],   // shoes
        ],
        [   // walk A
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],   // left leg out
            [ 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2 ],   // legs spread
            [ 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],   // shoes
        ],
        [   // mid
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2 ],   // right leg out
            [ 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2 ],   // legs spread
            [ 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],   // shoes
        ],
    ];

    // Legs 2: shorts + bare skin below knee
    private static readonly byte[][][] _legs2 =
    [
        [   // idle
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // belt
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // shorts
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // bare legs
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0, 0 ],   // shoes
        ],
        [   // walk A
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // belt
            [ 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],   // left shorts out
            [ 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 ],   // bare legs spread
            [ 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],   // shoes
        ],
        [   // mid
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // belt
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // shorts
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // bare legs
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],   // belt
            [ 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2 ],   // right shorts out
            [ 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 ],   // bare legs spread
            [ 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],   // shoes
        ],
    ];

    public override byte[][][][] LegsParts { get; } = [ _legs0, _legs1, _legs2 ];

    // ── Back-facing heads (no eye row, extra hair) ────────────────────────────

    private static readonly byte[][][] _head0Back =
    [[
        [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // hair
        [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // hair (back)
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // upper neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck base
    ]];

    private static readonly byte[][][] _head1Back =
    [[
        [ 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],   // hat crown
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // hat brim
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // upper neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck base
    ]];

    private static readonly byte[][][] _head2Back =
    [[
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // full hair
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // full hair
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // long hair continues
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck base
    ]];

    public override byte[][][][] HeadPartsBack { get; } = [ _head0Back, _head1Back, _head2Back ];

    // ── Back-facing bodies (no button row) ───────────────────────────────────

    private static readonly byte[][][] _body0Back =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt back
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt (no buttons)
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 ],   // highlight
        [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // bottom narrow
    ]];

    private static readonly byte[][][] _body1Back =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // collar back
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 ],   // highlight
        [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // bottom narrow
    ]];

    private static readonly byte[][][] _body2Back =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // jacket back
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // jacket
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // jacket
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // jacket
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // jacket
        [ 0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],   // bottom narrow
    ]];

    public override byte[][][][] BodyPartsBack { get; } = [ _body0Back, _body1Back, _body2Back ];

    // ── Side-facing legs (profile walk cycle, 4 frames) ──────────────────────
    // Toe extends right; front foot swings right, back foot swings left.
    // One-color-per-scanline rule still applies.

    private static readonly byte[][][] _legs0Side =
    [
        [   // idle — feet together, toe extends right
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
        [   // walk A — front foot forward (right)
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
        [   // mid — feet passing
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B — back foot forward (left)
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs1Side =
    [
        [   // idle
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
        [   // walk A
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
        [   // mid
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs2Side =
    [
        [   // idle
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
        [   // walk A
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
        [   // mid
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
    ];

    public override byte[][][][] LegsPartsSide { get; } = [ _legs0Side, _legs1Side, _legs2Side ];

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
