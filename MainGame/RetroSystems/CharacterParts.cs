using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems;

// ── Semantic color slot indices (per part) ────────────────────────────────────
//
// Head:  0=transparent  1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory
// Body:  0=transparent  1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory
// Legs:  0=transparent  1=Skin(bare)  2=Pants  3=PantsHighlight  4=Belt
//        5=BeltHighlight  6=Shoes  7=ShoeHighlight
//
// Index 0 always renders as Color.Transparent; higher indices map to the chosen
// palette entry so the same pixel art reads differently for each character.

// ── Shape variants ───────────────────────────────────────────────────────────
// Each system provides exactly these three variants per body section. The enum
// value is the index into <System>Sprites.HeadParts / BodyParts / LegsParts.

public enum HeadShape
{
    Basic    = 0,   // basic round head
    CapHat   = 1,   // cap or hat
    LongHair = 2,   // long / full hair
}

public enum BodyShape
{
    CasualShirt = 0,
    Formal      = 1,   // collared / formal
    Jacket      = 2,   // jacket / hoodie
}

public enum LegsShape
{
    Pants          = 0,   // pants + belt
    FormalTrousers = 1,
    Shorts         = 2,   // shorts + bare skin
}

// ── Palette slots ────────────────────────────────────────────────────────────
// Each enum value is the index into the system's HeadPalettes/BodyPalettes/
// LegPalettes arrays.

public enum HeadPaletteId
{
    FairBlonde   = 0,
    FairBrown    = 1,
    MediumBlack  = 2,
    DarkBlack    = 3,
    MediumAuburn = 4,
}

public enum BodyPaletteId
{
    Green = 0,
    Blue  = 1,
    Red   = 2,
    White = 3,
    Teal  = 4,
}

public enum LegsPaletteId
{
    BlueJeansBrown = 0,
    BlackBlack     = 1,
    KhakiTan       = 2,
    GrayDark       = 3,
}

public sealed record HeadPalette(
    string Name,
    Color Skin, Color Hair, Color Highlight, Color Eyes, Color Accessory)
{
    public Color Resolve(int idx) => idx switch
    {
        1 => Skin, 2 => Hair, 3 => Highlight, 4 => Eyes, 5 => Accessory,
        _ => Color.Transparent
    };
}

public sealed record BodyPalette(
    string Name,
    Color Skin, Color Shirt, Color ShirtHighlight, Color Buttons, Color Accessory)
{
    public Color Resolve(int idx) => idx switch
    {
        1 => Skin, 2 => Shirt, 3 => ShirtHighlight, 4 => Buttons, 5 => Accessory,
        _ => Color.Transparent
    };
}

public sealed record LegsPalette(
    string Name,
    Color Skin, Color Pants, Color PantsHighlight,
    Color Belt, Color BeltHighlight, Color Shoes, Color ShoeHighlight)
{
    public Color Resolve(int idx) => idx switch
    {
        1 => Skin,  2 => Pants,         3 => PantsHighlight,
        4 => Belt,  5 => BeltHighlight, 6 => Shoes, 7 => ShoeHighlight,
        _ => Color.Transparent
    };
}

/// <summary>
/// Selects which shape variant and color palette to use for each body section.
/// Head/Body/Legs operate independently, allowing any combination of shape and colour.
/// </summary>
public sealed record CharacterAppearance(
    HeadShape HeadIndex,  HeadPaletteId HeadPaletteIndex,
    BodyShape BodyIndex,  BodyPaletteId BodyPaletteIndex,
    LegsShape LegsIndex,  LegsPaletteId LegsPaletteIndex);
