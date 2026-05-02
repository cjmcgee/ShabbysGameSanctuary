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

    public override int CharWidth  => 16;
    public override int HeadRows   => 6;
    public override int BodyRows   => 9;
    public override int LegsRows   => 9;

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

    public override HeadPalette[] HeadPalettes { get; } =
    [
        new("fair/blonde",
            Skin:      NESPalette.SkinFair,
            Hair:      NESPalette.BrightYellow,   // NES warm yellow
            Highlight: NESPalette.SkinPale,
            Eyes:      NESPalette.NesBlue,   // NES blue
            Accessory: NESPalette.BrightYellow),
        new("fair/brown",
            Skin:      NESPalette.SkinFair,
            Hair:      NESPalette.WarmDoorWood,   // NES wood brown
            Highlight: NESPalette.SkinPale,
            Eyes:      NESPalette.DoorBlack,
            Accessory: NESPalette.WarmDoorWood),
        new("medium/black",
            Skin:      NESPalette.SkinTan,
            Hair:      NESPalette.DoorBlack,
            Highlight: NESPalette.SkinLight,
            Eyes:      NESPalette.DoorBlack,
            Accessory: NESPalette.DoorBlack),
        new("dark/black",
            Skin:      NESPalette.SkinDark,
            Hair:      NESPalette.DoorBlack,
            Highlight: NESPalette.SkinMedium,
            Eyes:      NESPalette.Aqua,   // NES aqua
            Accessory: NESPalette.DoorBlack),
        new("medium/auburn",
            Skin:      NESPalette.SkinTan,
            Hair:      NESPalette.DarkRed,   // NES dark red
            Highlight: NESPalette.SkinLight,
            Eyes:      NESPalette.NesBlue,
            Accessory: NESPalette.DarkRed),
    ];

    // ── Body palettes ─────────────────────────────────────────────────────────

    public override BodyPalette[] BodyPalettes { get; } =
    [
        new("green",
            Skin:           NESPalette.SkinFair,
            Shirt:          NESPalette.NesGreen,   // NES green
            ShirtHighlight: NESPalette.Aqua,
            Buttons:        NESPalette.NearWhite,
            Accessory:      NESPalette.NesBlue),
        new("blue",
            Skin:           NESPalette.SkinFair,
            Shirt:          NESPalette.NesBlue,   // NES blue
            ShirtHighlight: NESPalette.Aqua,
            Buttons:        NESPalette.WallGray,
            Accessory:      NESPalette.BrightYellow),
        new("red",
            Skin:           NESPalette.SkinFair,
            Shirt:          NESPalette.DarkRed,   // NES dark red
            ShirtHighlight: NESPalette.VividRed,
            Buttons:        NESPalette.NearWhite,
            Accessory:      NESPalette.BrightYellow),
        new("white/light",
            Skin:           NESPalette.SkinFair,
            Shirt:          NESPalette.WallGray,
            ShirtHighlight: NESPalette.NearWhite,
            Buttons:        NESPalette.NesBlue,
            Accessory:      NESPalette.NesGreen),
        new("teal",
            Skin:           NESPalette.SkinFair,
            Shirt:          NESPalette.Aqua,   // NES aqua
            ShirtHighlight: NESPalette.PaleCyan,
            Buttons:        NESPalette.NearWhite,
            Accessory:      NESPalette.MidMagenta),
    ];

    // ── Legs palettes ─────────────────────────────────────────────────────────

    public override LegsPalette[] LegsPalettes { get; } =
    [
        new("blue jeans/brown shoes",
            Skin:           NESPalette.SkinFair,
            Pants:          NESPalette.NesBlue,   // NES blue
            PantsHighlight: NESPalette.Aqua,
            Belt:           NESPalette.DarkBrown,
            BeltHighlight:  NESPalette.RustOrange,
            Shoes:          NESPalette.DarkestBrown,
            ShoeHighlight:  NESPalette.WarmDoorWood),
        new("black/black",
            Skin:           NESPalette.SkinFair,
            Pants:          NESPalette.DoorBlack,
            PantsHighlight: NESPalette.RoadGray,
            Belt:           NESPalette.RoadGray,
            BeltHighlight:  NESPalette.SidewalkGray,
            Shoes:          NESPalette.DoorBlack,
            ShoeHighlight:  NESPalette.RoadGray),
        new("khaki/tan",
            Skin:           NESPalette.SkinFair,
            Pants:          NESPalette.SkinTan,
            PantsHighlight: NESPalette.PaleYellow,
            Belt:           NESPalette.DarkBrown,
            BeltHighlight:  NESPalette.MediumBrown,
            Shoes:          NESPalette.DarkBrown,
            ShoeHighlight:  NESPalette.MediumBrown),
        new("gray/dark",
            Skin:           NESPalette.SkinFair,
            Pants:          NESPalette.SidewalkGray,
            PantsHighlight: NESPalette.WallGray,
            Belt:           NESPalette.RoadGray,
            BeltHighlight:  NESPalette.SidewalkGray,
            Shoes:          NESPalette.RoadGray,
            ShoeHighlight:  NESPalette.SidewalkGray),
    ];
}
