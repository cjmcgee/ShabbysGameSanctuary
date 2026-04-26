using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine.Rendering;

namespace ChildhoodAdventure.RetroSystems;

/// <summary>
/// Defines the visual characteristics of a retro gaming platform.
///
/// Tile canvases are always 16×16 world pixels; subclasses provide native-resolution
/// pixel art (e.g. 8×8 for Atari/C64/CGA, 16×16 for NES) which is nearest-neighbour
/// upscaled to fill the 16×16 tile texture slot.  This keeps all world coordinates
/// and collision logic system-agnostic.
///
/// Character sprites vary in canvas size per system to capture authentic proportions.
///
/// Palette indices used in pixel-art byte arrays:
///   0  = transparent (Color.Transparent for sprites; background color for tiles)
///   1+ = colors from the system's palette array
/// </summary>
public abstract class RetroSystem
{
    // ── Identity ─────────────────────────────────────────────────────────────

    public abstract string Name          { get; }
    public abstract string Description  { get; }  // e.g. "8×8 tiles · 16 colors"

    // Native tile resolution (informational / for HUD display)
    public abstract int NativeTileSize  { get; }

    // Camera zoom applied when this system is active
    public abstract float DisplayScale  { get; }

    // ── Tile art ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Palette for tile pixel art.
    /// Index 0 = background fill (used when a pixel byte is 0).
    /// Indices 1..N = actual tile colors.
    /// </summary>
    protected abstract Color[] TilePalette { get; }

    /// <summary>
    /// Returns pixel art for <paramref name="tileType"/> at NativeTileSize resolution.
    /// Array is [row][col], values are 1-based palette indices (0 = transparent/bg fill).
    /// <paramref name="accentColor"/> is used only for TileType.Accent tiles.
    /// </summary>
    protected abstract byte[][] GetTilePixels(TileType tileType, Color accentColor);

    // ── Sprite art ────────────────────────────────────────────────────────────

    /// <summary>Pixel width of one character animation frame.</summary>
    public abstract int CharWidth  { get; }

    /// <summary>Pixel height of one character animation frame.</summary>
    public abstract int CharHeight { get; }

    /// <summary>
    /// Walk-cycle frames: [frame][row][col], 4 frames.
    /// Palette index 1 = primary color (replaced per character by tint).
    /// Index 2 = highlight, index 3 = shadow (auto-derived from tint).
    /// </summary>
    protected abstract byte[][][] CharFrames { get; }

    // ── Public builders ───────────────────────────────────────────────────────

    /// <summary>
    /// Creates a Tileset texture with one 16×16 tile per entry in <paramref name="tileTypes"/>.
    /// Native pixel art is upscaled via nearest-neighbour to fill the 16×16 canvas.
    /// </summary>
    public Tileset BuildTileset(
        GraphicsDevice gd,
        string         name,
        TileType[]     tileTypes,
        Color          accentColor = default,
        int            firstGid    = 1)
    {
        // Append the runtime accent color at the slot after the declared palette,
        // so pixel art can reference it as index TilePalette.Length.
        var effectivePalette = new Color[TilePalette.Length + 1];
        TilePalette.CopyTo(effectivePalette, 0);
        effectivePalette[TilePalette.Length] =
            accentColor == default ? new Color(180, 180, 180) : accentColor;

        const int target = 16;
        int   count    = tileTypes.Length;
        var   texture  = new Texture2D(gd, target * count, target);
        var   data     = new Color[target * count * target];

        for (int i = 0; i < count; i++)
        {
            var pixels  = GetTilePixels(tileTypes[i], accentColor);
            int nativeW = pixels[0].Length;
            int nativeH = pixels.Length;
            float rx    = target / (float)nativeW;
            float ry    = target / (float)nativeH;

            for (int ty = 0; ty < target; ty++)
            {
                int srcRow = Math.Min((int)(ty / ry), nativeH - 1);
                for (int tx = 0; tx < target; tx++)
                {
                    int srcCol = Math.Min((int)(tx / rx), nativeW - 1);
                    byte idx   = pixels[srcRow][srcCol];
                    Color c    = idx < effectivePalette.Length ? effectivePalette[idx] : effectivePalette[0];
                    data[ty * (target * count) + i * target + tx] = c;
                }
            }
        }

        texture.SetData(data);
        return new Tileset(name, texture, target, target, firstGid);
    }

    /// <summary>
    /// Creates an <see cref="AnimatedSprite"/> for a character or NPC,
    /// substituting palette index 1 with <paramref name="mainColor"/>
    /// and auto-deriving highlight (index 2) and shadow (index 3).
    /// </summary>
    public AnimatedSprite BuildCharacterSprite(GraphicsDevice gd, Color mainColor)
    {
        int    frames  = CharFrames.Length;
        var    texture = new Texture2D(gd, CharWidth * frames, CharHeight);
        var    data    = new Color[CharWidth * frames * CharHeight];
        var    palette = BuildCharPalette(mainColor);

        for (int f = 0; f < frames; f++)
        {
            var frame = CharFrames[f];
            for (int y = 0; y < CharHeight; y++)
            {
                for (int x = 0; x < CharWidth; x++)
                {
                    byte idx = (y < frame.Length && x < frame[y].Length) ? frame[y][x] : (byte)0;
                    data[y * (CharWidth * frames) + f * CharWidth + x] =
                        idx == 0 ? Color.Transparent : palette[idx];
                }
            }
        }

        texture.SetData(data);

        var walk   = SpriteAnimation.FromStrip("walk", texture, CharWidth, CharHeight, frames, 0, 0.15f);
        var idle   = SpriteAnimation.FromStrip("idle", texture, CharWidth, CharHeight, 1,      0, 1.0f);
        var sprite = new AnimatedSprite(texture, CharWidth, CharHeight);
        sprite.AddAnimation(walk);
        sprite.AddAnimation(idle);
        sprite.Play("idle");
        return sprite;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Color[] BuildCharPalette(Color main)
    {
        // Slot 0 = transparent (unused here), 1 = main, 2 = highlight, 3 = shadow
        return
        [
            Color.Transparent,
            main,
            new Color(Math.Min(255,(int)(main.R*1.35f)), Math.Min(255,(int)(main.G*1.35f)), Math.Min(255,(int)(main.B*1.35f))),
            new Color((int)(main.R*0.50f), (int)(main.G*0.50f), (int)(main.B*0.50f)),
        ];
    }
}
