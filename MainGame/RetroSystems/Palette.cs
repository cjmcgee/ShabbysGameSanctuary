using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems;

/// <summary>
/// A fixed list of palette colours indexed by byte. Tile and sprite art store
/// indices into this palette rather than direct <see cref="Color"/> values.
/// </summary>
public class Palette(Color[] colors)
{
    public Color[] Colors => colors;
    public int Count => colors.Length;
    public Color this[int index] => colors[index];
}
