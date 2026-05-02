using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.NES;

/// <summary>
/// Nintendo Entertainment System visual style.
///
/// Authentic characteristics:
///   • 16-px tile art (NativeTilePixels=16); each engine tile maps to 2×2 NES hardware
///     8×8 tiles in spirit, so the art is hand-drawn at the 16×16 super-tile size
///   • NES PPU palette: 54 available colors; each background palette has 3 colors + shared bg
///   • Rich multi-color pixel art — the most detailed system in this set
///   • Character sprites: 16×24 (HeadRows=6, BodyRows=9, LegsRows=9)
///   • Native screen: 256×224 → DefaultTilesTall = 14
///   • MaxTilesTall = 28 (~2× zoom out)
/// </summary>
public sealed class NESSystem : RetroSystem
{
    public override string Name        => "NES";
    public override string Description => "16-px tiles · 16×24 sprites · 14 tiles tall · NES PPU palette";
    public override int   NativeTilePixels  => 16;
    public override float DefaultTilesTall  => 14f;   // native NES: 256×224 → 14 tiles vertically
    public override float MaxTilesTall      => 28f;   // ~2× zoom out

    protected override Palette TilePalette { get; } = new NESPalette();

    public override ScenePalette ScenePalette { get; } = new(
        HouseBeige:   NESPalette.NearWhite,
        HouseYellow:  NESPalette.PaleYellow,
        HousePink:    NESPalette.VividRed,           // no pink → vivid red
        HouseTeal:    NESPalette.Aqua,
        HouseGray:    NESPalette.SidewalkLight,
        HouseBlue:    NESPalette.NesBlue,
        HouseLime:    NESPalette.LeafGreen,
        HousePurple:  NESPalette.MidMagenta,
        HouseOrange:  NESPalette.RustOrange,
        Door:         NESPalette.DoorBlack,
        UiBackground: NESPalette.DarkBackground,
        UiText:       NESPalette.NearWhite,
        UiAccent:     NESPalette.PaleYellow,
        UiChoice:     NESPalette.SidewalkLight,
        UiDim:        NESPalette.SidewalkGray);

    protected override byte[][] GetTilePixels(TileType type, Color accentColor) => type switch
    {
        TileType.WoodFloor    => NESTiles.WoodFloor,
        TileType.Carpet       => NESTiles.Carpet,
        TileType.KitchenTile  => NESTiles.KitchenTile,
        TileType.Wall         => NESTiles.Wall,
        TileType.Door         => NESTiles.Door,
        TileType.Window       => NESTiles.Window,
        TileType.Furniture    => NESTiles.Furniture,
        TileType.Counter      => NESTiles.Counter,
        TileType.Bookshelf    => NESTiles.Bookshelf,
        TileType.Plant        => NESTiles.Plant,
        TileType.Grass        => NESTiles.Grass,
        TileType.Road         => NESTiles.Road,
        TileType.Sidewalk     => NESTiles.Sidewalk,
        TileType.HouseExterior => NESTiles.HouseWall,
        TileType.Accent       => NESTiles.Accent,
        _ => NESTiles.Wall
    };

    // ── Sprite dimensions (16×24 total) ──────────────────────────────────────

    public override int CharWidth  => NESSprites.CharWidth;
    public override int HeadRows   => NESSprites.HeadRows;
    public override int BodyRows   => NESSprites.BodyRows;
    public override int LegsRows   => NESSprites.LegsRows;

    // ── Head variants (1 frame × 6 rows × 16 cols) ───────────────────────────
    // Semantic: 1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory
    public override byte[][][][] HeadParts { get; } = [ NESSprites.Head0, NESSprites.Head1, NESSprites.Head2 ];

    // ── Body variants (1 frame × 9 rows × 16 cols) ───────────────────────────
    // Semantic: 1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory
    public override byte[][][][] BodyParts { get; } = [ NESSprites.Body0, NESSprites.Body1, NESSprites.Body2 ];

    // ── Legs variants (4 frames × 9 rows × 16 cols) ──────────────────────────
    // Semantic: 1=Skin(bare)  2=Pants  3=PantsHighlight  4=Belt
    //           5=BeltHighlight  6=Shoes  7=ShoeHighlight
    public override byte[][][][] LegsParts { get; } = [ NESSprites.Legs0, NESSprites.Legs1, NESSprites.Legs2 ];

    // ── Back-facing heads ─────────────────────────────────────────────────────
    public override byte[][][][] HeadPartsBack { get; } = [ NESSprites.Head0Back, NESSprites.Head1Back, NESSprites.Head2Back ];

    // ── Side-facing heads (profile right) ────────────────────────────────────
    public override byte[][][][] HeadPartsSide { get; } = [ NESSprites.Head0Side, NESSprites.Head1Side, NESSprites.Head2Side ];

    // ── Back-facing bodies ────────────────────────────────────────────────────
    public override byte[][][][] BodyPartsBack { get; } = [ NESSprites.Body0Back, NESSprites.Body1Back, NESSprites.Body2Back ];

    // ── Side-facing bodies (profile right) ───────────────────────────────────
    public override byte[][][][] BodyPartsSide { get; } = [ NESSprites.Body0Side, NESSprites.Body1Side, NESSprites.Body2Side ];

    // ── Side-facing legs (profile walk, 4 frames) ────────────────────────────
    public override byte[][][][] LegsPartsSide { get; } = [ NESSprites.Legs0Side, NESSprites.Legs1Side, NESSprites.Legs2Side ];

    // ── Head palettes (NES PPU warm skin/hair tones) ──────────────────────────
    public override HeadPalette[] HeadPalettes => NESSprites.HeadPalettes;

    // ── Body palettes ─────────────────────────────────────────────────────────
    public override BodyPalette[] BodyPalettes => NESSprites.BodyPalettes;

    // ── Legs palettes ─────────────────────────────────────────────────────────
    public override LegsPalette[] LegsPalettes => NESSprites.LegPalettes;
}
