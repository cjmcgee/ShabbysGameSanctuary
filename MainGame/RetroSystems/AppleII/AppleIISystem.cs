using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.AppleII;

/// <summary>
/// Apple II visual style — hi-res graphics mode.
///
/// Tile art is 14×14 — exactly two 7-pixel hi-res palette segments wide — and
/// follows the actual Apple II hi-res color rules rather than a simple double-
/// wide-pixel approximation:
///   • Each row splits into two 7-pixel segments (cols 0-6 and 7-13). Each
///     segment selects ONE palette — palette 0 (black/green/violet/white) or
///     palette 1 (black/orange/blue/white). Black and white work in either.
///   • Green / orange (palette odd-column color): only at odd x, with the
///     previous and following pixels both 0.
///   • Violet / blue  (palette even-column color): only at even x, with the
///     previous and following pixels both 0.
///   • White = run of ≥2 adjacent non-zero pixels. Black = run of ≥2 zeros.
///   • Cols 0 and 13 (left/right tile borders) are forced to 0 so that
///     horizontally adjacent tiles always meet at predictable black, instead
///     of bleeding their boundary pixels into each other.
///   • Tiles are designed "mostly black with simple motifs" so the above
///     constraints stay satisfiable without dense color masses.
///
/// Other characteristics:
///   • 6-color hi-res palette: black, green, violet, white, orange, blue
///     (NTSC artifact colors from the 1-MHz pixel clock — the defining Apple II look)
///   • Character sprites: 14×14 (HeadRows=4, BodyRows=6, LegsRows=4). Sprite
///     art is hand-authored to obey the same hi-res palette rules as the tiles.
///   • Native screen: 280×192 → DefaultTilesTall = 12
///   • MaxTilesTall = 24 (~2× zoom out)
/// </summary>
public sealed class AppleIISystem : RetroSystem
{
    public override string Name        => "Apple II";
    public override string Description => "14-px tiles · 14×14 sprites · 12 tiles tall · Hi-res 6-color w/ NTSC palette rules";
    public override int   NativeTilePixels => 14;
    public override float DefaultTilesTall => 12f;     // native 280×192 → 12 tiles vertically
    public override float MaxTilesTall     => 24f;     // ~2× zoom out

    // TODO: need to simulate color fringing for any text!
    protected override Palette TilePalette { get; } = new AppleIIPalette();

    // ── Tile pixel art (14×14, Apple II hi-res rules) ────────────────────────
    //   col 0 / col 13 always 0           (predictable black at tile borders)
    //   palette 0 (green/violet) OR palette 1 (orange/blue) per 7-pixel segment
    //   1 (green)  / 4 (orange):  odd  x, with 0 neighbors on both sides
    //   2 (violet) / 5 (blue):    even x, with 0 neighbors on both sides
    //   3 (white):  any x, in a run of ≥2 set pixels
    //   0 (black):  any x, in a run of ≥2 zero pixels

    protected override byte[][] GetTilePixels(TileType type, Color accentColor) => type switch
    {
        TileType.WoodFloor    => AppleIITiles.WoodFloor,
        TileType.Carpet       => AppleIITiles.Carpet,
        TileType.KitchenTile  => AppleIITiles.KitchenTile,
        TileType.Wall         => AppleIITiles.Wall,
        TileType.Door         => AppleIITiles.Door,
        TileType.Window       => AppleIITiles.Window,
        TileType.Furniture    => AppleIITiles.Furniture,
        TileType.Counter      => AppleIITiles.Counter,
        TileType.Bookshelf    => AppleIITiles.Bookshelf,
        TileType.Plant        => AppleIITiles.Plant,
        TileType.Grass        => AppleIITiles.Grass,
        TileType.Road         => AppleIITiles.Road,
        TileType.Sidewalk     => AppleIITiles.Sidewalk,
        TileType.HouseExterior => AppleIITiles.Wall,
        TileType.Accent       => AppleIITiles.Accent,
        _ => AppleIITiles.Wall
    };


    // ── Sprite dimensions (14×14 total) ───────────────────────────────────────
    // Sprite art follows the same Apple II discipline as tile art (loose form):
    //   • 14 wide × 14 tall (head 4 + body 6 + legs 4)
    //   • Features rendered as runs of ≥2 set pixels (the "white-rule" form)
    //     so single isolated colored pixels — which would impose strict even/
    //     odd column rules per palette — never appear.
    //   • Cols 0 and 13 are typically 0 to keep the silhouette inside the
    //     visible 12-col band, mirroring the tile-edge convention.

    public override int CharWidth  => 14;
    public override int HeadRows   => 4;
    public override int BodyRows   => 6;
    public override int LegsRows   => 4;

    // ── Head variants (1 frame × 4 rows × 14 cols) ───────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory

    private static readonly byte[][][] _head0 =   // basic round head
    [
        [
            [ 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],   // hair crown
            [ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0 ],   // hair sides + face
            [ 0, 0, 1, 1, 4, 4, 1, 1, 4, 4, 1, 1, 0, 0 ],   // face + two eyes
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
        ],
    ];

    private static readonly byte[][][] _head1 =   // cap / hat
    [
        [
            [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // hat top
            [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // hat brim
            [ 0, 0, 1, 1, 4, 4, 1, 1, 4, 4, 1, 1, 0, 0 ],   // face + eyes
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
        ],
    ];

    private static readonly byte[][][] _head2 =   // long / full hair
    [
        [
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // full hair top
            [ 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0 ],   // hair sides + face
            [ 0, 2, 2, 1, 4, 4, 1, 1, 4, 4, 1, 2, 2, 0 ],   // hair sides + eyes + face
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
        ],
    ];

    public override byte[][][][] HeadParts { get; } = [ _head0, _head1, _head2 ];

    // ── Body variants (1 frame × 6 rows × 14 cols) ───────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory

    private static readonly byte[][][] _body0 =   // casual shirt
    [
        [
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // shoulders
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // chest
            [ 0, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 0 ],   // highlight band
            [ 0, 2, 2, 2, 4, 4, 2, 2, 4, 4, 2, 2, 2, 0 ],   // two buttons
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // lower
        ],
    ];

    private static readonly byte[][][] _body1 =   // collared / formal
    [
        [
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
            [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // lapels around tie
            [ 0, 5, 5, 5, 5, 4, 4, 4, 4, 5, 5, 5, 5, 0 ],   // lapels + tie buttons
            [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // lapels lower
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // torso
            [ 0, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 0 ],   // lower buttons
        ],
    ];

    private static readonly byte[][][] _body2 =   // jacket / hoodie
    [
        [
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
            [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // jacket full
            [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // open jacket + shirt
            [ 0, 5, 5, 5, 5, 3, 3, 4, 4, 3, 3, 5, 5, 0 ],   // highlight + button
            [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // shirt
            [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // jacket hem
        ],
    ];

    public override byte[][][][] BodyParts { get; } = [ _body0, _body1, _body2 ];

    // ── Legs variants (4 frames × 4 rows × 14 cols) ──────────────────────────
    // Semantic: 1=Skin(bare)  2=Pants  3=PantsHighlight  4=Belt
    //           5=BeltHighlight  6=Shoes  7=ShoeHighlight

    private static readonly byte[][][] _legs0 =   // pants + belt
    [
        [   // idle
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],   // belt
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],   // pants + buckle
            [ 0, 2, 2, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 0 ],   // legs split
            [ 0, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6, 0 ],   // shoes
        ],
        [   // left foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
            [ 0, 2, 2, 2, 2, 2, 2, 2, 0, 0, 2, 2, 2, 0 ],   // left ahead
            [ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6, 6, 0 ],
        ],
        [   // crossing / passing
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // legs together
            [ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
        [   // right foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
            [ 0, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 2, 2, 0 ],   // right ahead
            [ 0, 6, 6, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs1 =   // formal trousers (crease)
    [
        [   // idle
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],   // belt
            [ 0, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 0 ],   // crease band
            [ 0, 2, 2, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 0 ],   // legs split
            [ 0, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6, 0 ],   // shoes
        ],
        [   // left foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 0 ],
            [ 0, 2, 2, 2, 2, 2, 2, 2, 0, 0, 2, 2, 2, 0 ],
            [ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6, 6, 0 ],
        ],
        [   // crossing
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
        [   // right foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 0 ],
            [ 0, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 2, 2, 0 ],
            [ 0, 6, 6, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs2 =   // shorts + bare skin
    [
        [   // idle
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],   // belt
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],   // shorts + buckle
            [ 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0 ],   // bare legs
            [ 0, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 6, 6, 0 ],   // shoes
        ],
        [   // left foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
            [ 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0 ],
            [ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 6, 6, 0 ],
        ],
        [   // crossing
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
            [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0 ],
        ],
        [   // right foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2, 2, 2, 0 ],
            [ 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0 ],
            [ 0, 6, 6, 0, 0, 0, 0, 0, 6, 6, 6, 6, 6, 0 ],
        ],
    ];

    public override byte[][][][] LegsParts { get; } = [ _legs0, _legs1, _legs2 ];

    // ── Back-facing heads (no eye row) ────────────────────────────────────────

    private static readonly byte[][][] _head0Back =
    [[
        [ 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],   // hair crown
        [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // hair back (no eyes)
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // upper neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
    ]];

    private static readonly byte[][][] _head1Back =
    [[
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // hat top (full brim from behind)
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // hat brim
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // upper neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
    ]];

    private static readonly byte[][][] _head2Back =
    [[
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // full hair
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // full hair back
        [ 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0 ],   // hair sides + neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
    ]];

    public override byte[][][][] HeadPartsBack { get; } = [ _head0Back, _head1Back, _head2Back ];

    // ── Back-facing bodies (no buttons) ──────────────────────────────────────

    private static readonly byte[][][] _body0Back =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // back shoulders
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // back
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // back (no buttons)
        [ 0, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 0 ],   // highlight band
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // lower
    ]];

    private static readonly byte[][][] _body1Back =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // collar back
        [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // collar
        [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // lapels back
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // torso
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // lower
    ]];

    private static readonly byte[][][] _body2Back =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // jacket back
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],
    ]];

    public override byte[][][][] BodyPartsBack { get; } = [ _body0Back, _body1Back, _body2Back ];

    // ── Side-facing legs (4 frames × 4 rows × 14 cols) ───────────────────────
    // Profile facing right; front foot swings forward (right), back foot trails.

    private static readonly byte[][][] _legs0Side =
    [
        [   // idle — both legs centered, toe extends right
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],   // belt
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // pants
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],   // shoe extends right (toe)
        ],
        [   // walk A — front foot forward
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2 ],   // legs split fwd/back
            [ 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6 ],   // back heel + front toe
        ],
        [   // mid — legs together, lifted
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // feet lifted
        ],
        [   // walk B — back foot forward (heel kick)
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 0, 0 ],   // both feet planted
        ],
    ];

    private static readonly byte[][][] _legs1Side =
    [
        [   // idle
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        ],
        [   // walk A
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],
            [ 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2 ],
            [ 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6 ],
        ],
        [   // mid
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 0, 0 ],
        ],
    ];

    private static readonly byte[][][] _legs2Side =
    [
        [   // idle (shorts + bare legs)
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 0, 0, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 ],
        ],
        [   // walk A
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 ],
            [ 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1 ],
            [ 0, 0, 6, 6, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6 ],
        ],
        [   // mid
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
        [   // walk B
            [ 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 ],
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0 ],
            [ 0, 6, 6, 6, 6, 6, 0, 0, 0, 0, 6, 6, 0, 0 ],
        ],
    ];

    public override byte[][][][] LegsPartsSide { get; } = [ _legs0Side, _legs1Side, _legs2Side ];

    // ── Head palettes — all resolved colors are from the 6-color hi-res set ──
    // Black=(0,0,0)  Green=(20,245,60)  Violet=(193,28,255)
    // White=(255,255,255)  Orange=(255,106,60)  Blue=(20,88,255)

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("fair/light-hair",
            Skin:      AppleIIPalette.White,   // white
            Hair:      AppleIIPalette.Orange,   // orange (blonde approximation)
            Highlight: AppleIIPalette.White,   // white
            Eyes:      AppleIIPalette.Blue,   // blue
            Accessory: AppleIIPalette.Orange),  // orange

        new("fair/dark-hair",
            Skin:      AppleIIPalette.White,   // white
            Hair:      AppleIIPalette.Black,   // black
            Highlight: AppleIIPalette.White,   // white
            Eyes:      AppleIIPalette.Black,   // black
            Accessory: AppleIIPalette.Violet),  // violet

        new("medium/black-hair",
            Skin:      AppleIIPalette.Orange,   // orange
            Hair:      AppleIIPalette.Black,   // black
            Highlight: AppleIIPalette.White,   // white
            Eyes:      AppleIIPalette.Blue,   // blue
            Accessory: AppleIIPalette.Black),  // black

        new("dark/black-hair",
            Skin:      AppleIIPalette.Violet,   // violet
            Hair:      AppleIIPalette.Black,   // black
            Highlight: AppleIIPalette.Orange,   // orange
            Eyes:      AppleIIPalette.White,   // white (contrast)
            Accessory: AppleIIPalette.Black),  // black

        new("medium/violet-hair",
            Skin:      AppleIIPalette.Orange,   // orange
            Hair:      AppleIIPalette.Violet,   // violet
            Highlight: AppleIIPalette.White,   // white
            Eyes:      AppleIIPalette.Blue,   // blue
            Accessory: AppleIIPalette.Violet),  // violet
    ];

    // ── Body palettes ─────────────────────────────────────────────────────────

    public override BodyPalette[] BodyPalettes { get; } =
    [
        new("green",
            Skin:           AppleIIPalette.Orange,   // orange
            Shirt:          AppleIIPalette.Green,   // green
            ShirtHighlight: AppleIIPalette.White,   // white
            Buttons:        AppleIIPalette.Black,   // black
            Accessory:      AppleIIPalette.Blue),  // blue

        new("blue",
            Skin:           AppleIIPalette.Orange,   // orange
            Shirt:          AppleIIPalette.Blue,   // blue
            ShirtHighlight: AppleIIPalette.White,   // white
            Buttons:        AppleIIPalette.Black,   // black
            Accessory:      AppleIIPalette.Orange),  // orange

        new("violet",
            Skin:           AppleIIPalette.Orange,   // orange
            Shirt:          AppleIIPalette.Violet,   // violet
            ShirtHighlight: AppleIIPalette.White,   // white
            Buttons:        AppleIIPalette.Black,   // black
            Accessory:      AppleIIPalette.Green),  // green

        new("orange",
            Skin:           AppleIIPalette.White,   // white
            Shirt:          AppleIIPalette.Orange,   // orange
            ShirtHighlight: AppleIIPalette.White,   // white
            Buttons:        AppleIIPalette.Black,   // black
            Accessory:      AppleIIPalette.Blue),  // blue

        new("white",
            Skin:           AppleIIPalette.Orange,   // orange
            Shirt:          AppleIIPalette.White,   // white
            ShirtHighlight: AppleIIPalette.Blue,   // blue (distinguishable highlight)
            Buttons:        AppleIIPalette.Black,   // black
            Accessory:      AppleIIPalette.Green),  // green
    ];

    // ── Legs palettes ─────────────────────────────────────────────────────────

    public override LegsPalette[] LegsPalettes { get; } =
    [
        new("blue pants",
            Skin:           AppleIIPalette.Orange,   // orange
            Pants:          AppleIIPalette.Blue,   // blue
            PantsHighlight: AppleIIPalette.White,   // white
            Belt:           AppleIIPalette.Black,   // black
            BeltHighlight:  AppleIIPalette.White,   // white
            Shoes:          AppleIIPalette.Black,   // black
            ShoeHighlight:  AppleIIPalette.Orange),  // orange

        new("violet pants",
            Skin:           AppleIIPalette.Orange,   // orange
            Pants:          AppleIIPalette.Violet,   // violet
            PantsHighlight: AppleIIPalette.White,   // white
            Belt:           AppleIIPalette.Black,   // black
            BeltHighlight:  AppleIIPalette.Orange,   // orange
            Shoes:          AppleIIPalette.Black,   // black
            ShoeHighlight:  AppleIIPalette.White),  // white

        new("black pants",
            Skin:           AppleIIPalette.Orange,   // orange
            Pants:          AppleIIPalette.Black,   // black
            PantsHighlight: AppleIIPalette.Blue,   // blue
            Belt:           AppleIIPalette.Orange,   // orange
            BeltHighlight:  AppleIIPalette.White,   // white
            Shoes:          AppleIIPalette.Black,   // black
            ShoeHighlight:  AppleIIPalette.White),  // white

        new("green pants",
            Skin:           AppleIIPalette.Orange,   // orange
            Pants:          AppleIIPalette.Green,   // green
            PantsHighlight: AppleIIPalette.White,   // white
            Belt:           AppleIIPalette.Black,   // black
            BeltHighlight:  AppleIIPalette.Orange,   // orange
            Shoes:          AppleIIPalette.Black,   // black
            ShoeHighlight:  AppleIIPalette.White),  // white
    ];
}
