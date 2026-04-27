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
/// All three systems (Head/Body/Legs) operate independently, allowing any
/// combination of shape and color.
/// </summary>
public sealed record CharacterAppearance(
    int HeadIndex,  int HeadPaletteIndex,
    int BodyIndex,  int BodyPaletteIndex,
    int LegsIndex,  int LegsPaletteIndex);
