using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.MSDOSCGA;

/// <summary>
/// MS-DOS CGA visual style — mode 4, palette 1 (high intensity).
///
/// Authentic characteristics:
///   • 16-px tile art (NativeTilePixels=16); the IBM CGA hardware text-mode tile is 8×8,
///     redrawn here at 16×16 to give space for the 4-color palette
///   • 4-color fixed palette: black, bright cyan, bright magenta, white
///     (IBM CGA mode 4 palette 1 — the most iconic CGA look)
///   • Character sprites: 16×16 texture (CharWidth=16, HeadRows=4, BodyRows=6, LegsRows=6)
///   • Native screen 320×200 → DefaultTilesTall = 12.5
///   • MaxTilesTall = 25 (~2× zoom out)
/// </summary>
public sealed class CGASystem : RetroSystem
{
    public override string Name        => "MS-DOS CGA";
    public override string Description => "16-px tiles · 16×16 sprites · 12.5 tiles tall · Mode 4 palette 1 (black/cyan/magenta/white)";
    public override int   NativeTilePixels => 16;
    public override float DefaultTilesTall => 12.5f;   // native 320×200 → 12.5 tiles vertically
    public override float MaxTilesTall     => 25f;     // ~2× zoom out

    protected override Palette TilePalette { get; } = new CGAPalette();

    // ── Tile pixel art (16×16, indices 0–3 only) ─────────────────────────────

    protected override byte[][] GetTilePixels(TileType type, Color accentColor) => type switch
    {
        TileType.WoodFloor     => CGATiles.WoodFloor,
        TileType.Carpet        => CGATiles.Carpet,
        TileType.KitchenTile   => CGATiles.KitchenTile,
        TileType.Wall          => CGATiles.Wall,
        TileType.Door          => CGATiles.Door,
        TileType.Window        => CGATiles.Window,
        TileType.Furniture     => CGATiles.Furniture,
        TileType.Counter       => CGATiles.Counter,
        TileType.Bookshelf     => CGATiles.Bookshelf,
        TileType.Plant         => CGATiles.Plant,
        TileType.Grass         => CGATiles.Grass,
        TileType.Road          => CGATiles.Road,
        TileType.Sidewalk      => CGATiles.Sidewalk,
        TileType.HouseExterior => CGATiles.Wall,
        TileType.Accent        => CGATiles.Accent,
        _ => CGATiles.Wall
    };


    // ── Sprite dimensions ─────────────────────────────────────────────────────

    public override int CharWidth  => 16;
    public override int HeadRows   => 4;
    public override int BodyRows   => 6;
    public override int LegsRows   => 6;

    // ── Head parts (16 wide × 4 rows, 1 frame each) ──────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory
    // Each column doubled from 8-wide art for authentic CGA wide-pixel look.

    private static readonly byte[][][] _head0 =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
        [ 0, 0, 1, 1, 1, 1, 4, 4, 1, 1, 4, 4, 1, 1, 0, 0 ],
        [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
        [ 0, 0, 0, 0, 1, 1, 2, 2, 2, 2, 1, 1, 0, 0, 0, 0 ],
    ]];

    private static readonly byte[][][] _head1 =
    [[
        [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],
        [ 0, 0, 5, 5, 1, 1, 1, 1, 1, 1, 1, 1, 5, 5, 0, 0 ],
        [ 0, 0, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 0, 0 ],
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],
    ]];

    private static readonly byte[][][] _head2 =
    [[
        [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
        [ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0 ],
        [ 0, 0, 2, 2, 4, 4, 1, 1, 1, 1, 4, 4, 2, 2, 0, 0 ],
        [ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0 ],
    ]];

    public override byte[][][][] HeadParts { get; } = [ _head0, _head1, _head2 ];

    // ── Body parts (16 wide × 6 rows, 1 frame each) ──────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory

    private static readonly byte[][][] _body0 =
    [[
        [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 2, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 2, 2 ],
        [ 0, 0, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 0, 0 ],
    ]];

    private static readonly byte[][][] _body1 =
    [[
        [ 0, 0, 1, 1, 5, 5, 1, 1, 1, 1, 5, 5, 1, 1, 0, 0 ],
        [ 2, 2, 5, 5, 2, 2, 4, 4, 4, 4, 2, 2, 5, 5, 2, 2 ],
        [ 2, 2, 5, 5, 2, 2, 4, 4, 4, 4, 2, 2, 5, 5, 2, 2 ],
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],
        [ 2, 2, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 2, 2 ],
        [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
    ]];

    private static readonly byte[][][] _body2 =
    [[
        [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
        [ 5, 5, 5, 5, 2, 2, 2, 2, 2, 2, 2, 2, 5, 5, 5, 5 ],
        [ 5, 5, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 5, 5 ],
        [ 5, 5, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 5, 5 ],
        [ 5, 5, 5, 5, 2, 2, 2, 2, 2, 2, 2, 2, 5, 5, 5, 5 ],
        [ 0, 0, 5, 5, 2, 2, 2, 2, 2, 2, 2, 2, 5, 5, 0, 0 ],
    ]];

    public override byte[][][][] BodyParts { get; } = [ _body0, _body1, _body2 ];

    // ── Legs parts (16 wide × 6 rows, 4 walk frames) ─────────────────────────
    // Semantic: 1=Skin  2=Pants  3=PantsHighlight  4=Belt  6=Shoes  7=ShoeHighlight

    private static readonly byte[][][] _legs0 =
    [
        [   // idle
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 2, 2, 3, 3, 0, 0, 0, 0, 3, 3, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 6, 6, 7, 7, 0, 0, 0, 0, 6, 6, 7, 7, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk A
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 2, 2, 3, 3, 0, 0, 0, 0, 0, 0, 2, 2, 0, 0 ],
            [ 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 0, 0 ],
            [ 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2 ],
            [ 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 7 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // mid-stride
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 2, 2, 3, 3, 0, 0, 0, 0, 3, 3, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 2, 2, 0, 0, 0, 0, 0, 0, 3, 3, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2 ],
            [ 0, 0, 7, 7, 6, 6, 0, 0, 0, 0, 0, 0, 7, 7, 7, 7 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs1 =
    [
        [   // idle
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 3, 3, 0, 0, 0, 0, 3, 3, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0 ],
            [ 0, 0, 7, 7, 7, 7, 0, 0, 0, 0, 7, 7, 7, 7, 0, 0 ],
        ],
        [   // walk A
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 3, 3, 0, 0, 0, 0, 0, 0, 2, 2, 0, 0 ],
            [ 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 0, 0 ],
            [ 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2 ],
            [ 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 6 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 7, 7 ],
        ],
        [   // mid
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 3, 3, 0, 0, 0, 0, 3, 3, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 0, 0, 0, 0, 0, 0, 3, 3, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2 ],
            [ 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 7, 7, 7, 7 ],
            [ 0, 0, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs2 =
    [
        [   // idle
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 2, 2, 3, 3, 1, 1, 1, 1, 3, 3, 2, 2, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 1, 1, 1, 1, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 6, 6, 7, 7, 0, 0, 0, 0, 6, 6, 7, 7, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk A
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 2, 2, 3, 3, 1, 1, 0, 0, 0, 0, 2, 2, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0 ],
            [ 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 ],
            [ 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6, 7, 7 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // mid
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 2, 2, 3, 3, 1, 1, 1, 1, 3, 3, 2, 2, 0, 0 ],
            [ 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 2, 2, 0, 0, 0, 0, 1, 1, 3, 3, 2, 2, 0, 0 ],
            [ 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 ],
            [ 0, 0, 7, 7, 6, 6, 0, 0, 0, 0, 0, 0, 7, 7, 7, 7 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
    ];

    public override byte[][][][] LegsParts { get; } = [ _legs0, _legs1, _legs2 ];

    // ── Back-facing heads ─────────────────────────────────────────────────────

    private static readonly byte[][][] _head0Back =
    [[
        [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // hair
        [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // hair (no eyes)
        [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],   // neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck base
    ]];

    private static readonly byte[][][] _head1Back =
    [[
        [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],   // hat crown
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // hat brim
        [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],   // neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck base
    ]];

    private static readonly byte[][][] _head2Back =
    [[
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // full hair
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // hair (longer from back)
        [ 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0 ],   // hair sides + neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
    ]];

    public override byte[][][][] HeadPartsBack { get; } = [ _head0Back, _head1Back, _head2Back ];

    // ── Side-facing heads (profile right) ────────────────────────────────────

    private static readonly byte[][][] _head0Side =
    [[
        [ 0, 0, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // hair left + face
        [ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 4, 4, 0, 0, 0, 0 ],   // hair + face + eye right
        [ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // hair + lower face
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],   // chin (narrower)
    ]];

    private static readonly byte[][][] _head1Side =
    [[
        [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],   // hat
        [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],   // hat brim
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 4, 4, 0, 0, 0, 0 ],   // face + eye
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],   // chin
    ]];

    private static readonly byte[][][] _head2Side =
    [[
        [ 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // hair left + face
        [ 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 4, 4, 0, 0, 0, 0 ],   // hair + face + eye
        [ 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // hair + lower face
        [ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],   // hair tail + chin
    ]];

    public override byte[][][][] HeadPartsSide { get; } = [ _head0Side, _head1Side, _head2Side ];

    // ── Back-facing bodies ────────────────────────────────────────────────────

    private static readonly byte[][][] _body0Back =
    [[
        [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],   // neck
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt back
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt (no buttons)
        [ 2, 2, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 2, 2 ],   // back highlights
        [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // lower
    ]];

    private static readonly byte[][][] _body1Back =
    [[
        [ 0, 0, 1, 1, 5, 5, 1, 1, 1, 1, 5, 5, 1, 1, 0, 0 ],   // neck + collar
        [ 2, 2, 5, 5, 2, 2, 2, 2, 2, 2, 2, 2, 5, 5, 2, 2 ],   // collar back
        [ 2, 2, 5, 5, 2, 2, 2, 2, 2, 2, 2, 2, 5, 5, 2, 2 ],   // collar
        [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],   // shirt
        [ 2, 2, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 2, 2 ],   // highlights
        [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // lower
    ]];

    private static readonly byte[][][] _body2Back =
    [[
        [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],   // neck
        [ 5, 5, 5, 5, 2, 2, 2, 2, 2, 2, 2, 2, 5, 5, 5, 5 ],   // jacket back
        [ 5, 5, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 5, 5 ],   // jacket
        [ 5, 5, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 5, 5 ],   // jacket
        [ 5, 5, 5, 5, 2, 2, 2, 2, 2, 2, 2, 2, 5, 5, 5, 5 ],   // jacket lower
        [ 0, 0, 5, 5, 2, 2, 2, 2, 2, 2, 2, 2, 5, 5, 0, 0 ],   // jacket bottom
    ]];

    public override byte[][][][] BodyPartsBack { get; } = [ _body0Back, _body1Back, _body2Back ];

    // ── Side-facing bodies (profile right) ───────────────────────────────────

    private static readonly byte[][][] _body0Side =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],   // neck (narrower)
        [ 0, 0, 1, 1, 2, 2, 2, 2, 2, 2, 1, 1, 0, 0, 0, 0 ],   // shoulder + arm stub
        [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],   // torso
        [ 0, 0, 3, 3, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],   // highlight
        [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],   // mid
        [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],   // lower
    ]];

    private static readonly byte[][][] _body1Side =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],   // neck
        [ 0, 0, 5, 5, 2, 2, 2, 2, 5, 5, 0, 0, 0, 0, 0, 0 ],   // lapel side
        [ 0, 0, 2, 2, 4, 4, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],   // button
        [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 3, 3, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],
    ]];

    private static readonly byte[][][] _body2Side =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],   // neck
        [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0 ],   // jacket side
        [ 0, 0, 5, 5, 2, 2, 2, 2, 5, 5, 0, 0, 0, 0, 0, 0 ],   // open jacket
        [ 0, 0, 5, 5, 4, 4, 2, 2, 5, 5, 0, 0, 0, 0, 0, 0 ],   // button
        [ 0, 0, 5, 5, 2, 2, 2, 2, 5, 5, 0, 0, 0, 0, 0, 0 ],
        [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0, 0 ],
    ]];

    public override byte[][][][] BodyPartsSide { get; } = [ _body0Side, _body1Side, _body2Side ];

    // ── Side-facing legs (profile walk, 4 frames) ────────────────────────────

    private static readonly byte[][][] _legs0Side =
    [
        [   // idle — toe extends right
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 3, 3, 3, 3, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk A — front foot forward (right)
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 3, 3, 2, 2, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 2, 2, 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 7, 7, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 7, 7, 0, 0 ],
        ],
        [   // mid — legs crossing
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 3, 3, 3, 3, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B — back foot forward (left)
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 3, 3, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0, 2, 2, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 0, 0, 6, 6, 7, 7, 0, 0, 0, 0 ],
            [ 0, 0, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs1Side =
    [
        [   // idle
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 3, 3, 3, 3, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
            [ 0, 0, 0, 0, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk A
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 3, 3, 2, 2, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 2, 2, 0, 0, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0 ],
            [ 0, 0, 7, 7, 7, 7, 0, 0, 0, 0, 7, 7, 7, 7, 0, 0 ],
        ],
        [   // mid
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 3, 3, 3, 3, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 3, 3, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0, 2, 2, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0 ],
            [ 0, 0, 7, 7, 7, 7, 0, 0, 7, 7, 7, 7, 0, 0, 0, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs2Side =
    [
        [   // idle
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 3, 3, 1, 1, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 1, 1, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk A
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 3, 3, 1, 1, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 7, 7, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 7, 7, 0, 0 ],
        ],
        [   // mid
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 3, 3, 1, 1, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 0, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 3, 3, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 0, 0, 6, 6, 7, 7, 0, 0, 0, 0 ],
            [ 0, 0, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
    ];

    public override byte[][][][] LegsPartsSide { get; } = [ _legs0Side, _legs1Side, _legs2Side ];

    // ── Head palettes — all resolved colors are from the 4-color CGA set ─────
    // Black=(0,0,0)  Cyan=(85,255,255)  Magenta=(255,85,255)  White=(255,255,255)

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("fair/blonde",
            Skin:      CGAPalette.White,   // white
            Hair:      CGAPalette.BrightMagenta,   // magenta (closest to blonde)
            Highlight: CGAPalette.White,   // white
            Eyes:      CGAPalette.BrightCyan,   // cyan
            Accessory: CGAPalette.BrightMagenta),  // magenta

        new("fair/dark-hair",
            Skin:      CGAPalette.White,   // white
            Hair:      CGAPalette.Black,   // black
            Highlight: CGAPalette.White,   // white
            Eyes:      CGAPalette.Black,   // black
            Accessory: CGAPalette.BrightCyan),  // cyan

        new("medium/black",
            Skin:      CGAPalette.BrightCyan,   // cyan
            Hair:      CGAPalette.Black,   // black
            Highlight: CGAPalette.White,   // white
            Eyes:      CGAPalette.Black,   // black
            Accessory: CGAPalette.Black),  // black

        new("dark/black",
            Skin:      CGAPalette.BrightMagenta,   // magenta
            Hair:      CGAPalette.Black,   // black
            Highlight: CGAPalette.BrightCyan,   // cyan
            Eyes:      CGAPalette.White,   // white (contrast)
            Accessory: CGAPalette.Black),  // black

        new("medium/magenta-hair",
            Skin:      CGAPalette.BrightCyan,   // cyan
            Hair:      CGAPalette.BrightMagenta,   // magenta
            Highlight: CGAPalette.White,   // white
            Eyes:      CGAPalette.Black,   // black
            Accessory: CGAPalette.BrightMagenta),  // magenta
    ];

    // ── Body palettes ─────────────────────────────────────────────────────────

    public override BodyPalette[] BodyPalettes { get; } =
    [
        new("cyan",
            Skin:           CGAPalette.White,   // white
            Shirt:          CGAPalette.BrightCyan,   // cyan
            ShirtHighlight: CGAPalette.White,   // white
            Buttons:        CGAPalette.Black,   // black
            Accessory:      CGAPalette.BrightMagenta),  // magenta

        new("magenta",
            Skin:           CGAPalette.White,   // white
            Shirt:          CGAPalette.BrightMagenta,   // magenta
            ShirtHighlight: CGAPalette.White,   // white
            Buttons:        CGAPalette.Black,   // black
            Accessory:      CGAPalette.BrightCyan),  // cyan

        new("white",
            Skin:           CGAPalette.White,   // white
            Shirt:          CGAPalette.White,   // white
            ShirtHighlight: CGAPalette.BrightCyan,   // cyan (highlight distinguishable)
            Buttons:        CGAPalette.Black,   // black
            Accessory:      CGAPalette.BrightMagenta),  // magenta

        new("black",
            Skin:           CGAPalette.White,   // white
            Shirt:          CGAPalette.Black,   // black
            ShirtHighlight: CGAPalette.BrightCyan,   // cyan
            Buttons:        CGAPalette.White,   // white
            Accessory:      CGAPalette.BrightMagenta),  // magenta

        new("magenta jacket",
            Skin:           CGAPalette.White,   // white
            Shirt:          CGAPalette.White,   // white shirt under jacket
            ShirtHighlight: CGAPalette.BrightCyan,   // cyan
            Buttons:        CGAPalette.Black,   // black
            Accessory:      CGAPalette.BrightMagenta),  // magenta jacket
    ];

    // ── Legs palettes ─────────────────────────────────────────────────────────

    public override LegsPalette[] LegsPalettes { get; } =
    [
        new("cyan pants",
            Skin:           CGAPalette.White,   // white
            Pants:          CGAPalette.BrightCyan,   // cyan
            PantsHighlight: CGAPalette.White,   // white
            Belt:           CGAPalette.Black,   // black
            BeltHighlight:  CGAPalette.White,   // white
            Shoes:          CGAPalette.Black,   // black
            ShoeHighlight:  CGAPalette.BrightMagenta),  // magenta

        new("magenta pants",
            Skin:           CGAPalette.White,   // white
            Pants:          CGAPalette.BrightMagenta,   // magenta
            PantsHighlight: CGAPalette.White,   // white
            Belt:           CGAPalette.Black,   // black
            BeltHighlight:  CGAPalette.White,   // white
            Shoes:          CGAPalette.Black,   // black
            ShoeHighlight:  CGAPalette.BrightCyan),  // cyan

        new("black pants",
            Skin:           CGAPalette.White,   // white
            Pants:          CGAPalette.Black,   // black
            PantsHighlight: CGAPalette.BrightCyan,   // cyan
            Belt:           CGAPalette.BrightMagenta,   // magenta
            BeltHighlight:  CGAPalette.White,   // white
            Shoes:          CGAPalette.Black,   // black
            ShoeHighlight:  CGAPalette.White),  // white

        new("white pants",
            Skin:           CGAPalette.White,   // white
            Pants:          CGAPalette.White,   // white
            PantsHighlight: CGAPalette.BrightCyan,   // cyan
            Belt:           CGAPalette.Black,   // black
            BeltHighlight:  CGAPalette.White,   // white
            Shoes:          CGAPalette.BrightMagenta,   // magenta
            ShoeHighlight:  CGAPalette.White),  // white
    ];
}
