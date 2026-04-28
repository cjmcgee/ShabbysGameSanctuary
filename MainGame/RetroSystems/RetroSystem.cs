using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine.Rendering;

namespace ChildhoodAdventure.RetroSystems;

/// <summary>
/// Defines the visual characteristics of a retro gaming platform.
///
/// Tile canvases are always 16×16 world pixels; subclasses provide native-resolution
/// pixel art which is nearest-neighbour upscaled by BuildTileset.
///
/// Character sprites are assembled from three independent parts — head, body, legs —
/// each defined as pixel arrays using semantic color indices rather than direct palette
/// values.  At runtime the chosen HeadPalette/BodyPalette/LegsPalette maps each
/// semantic index to a concrete Color, so the same pixel art produces a different-
/// looking character depending on the selected palette.
///
/// Semantic indices per part:
///   Head:  0=transparent  1=Skin  2=Hair  3=SkinHighlight  4=Eyes  5=HatAccessory
///   Body:  0=transparent  1=Skin  2=Shirt  3=ShirtHighlight  4=Buttons  5=Accessory
///   Legs:  0=transparent  1=Skin(bare)  2=Pants  3=PantsHighlight  4=Belt
///          5=BeltHighlight  6=Shoes  7=ShoeHighlight
/// </summary>
public abstract class RetroSystem
{
    // ── Identity ─────────────────────────────────────────────────────────────

    public abstract string Name         { get; }
    public abstract string Description  { get; }
    public abstract int    NativeTileSize{ get; }
    public abstract float  DisplayScale  { get; }

    /// <summary>
    /// True for systems (Atari 2600, C64) where each logical pixel occupies two
    /// adjacent horizontal screen pixels.  Enforced at render time: odd columns
    /// always copy the value from the preceding even column.
    /// </summary>
    protected virtual bool DoubleWidePixels => false;

    /// <summary>
    /// Maximum world-pixel area the camera may reveal at once (X=width, Y=height).
    /// Null means no additional constraint beyond map fill. Override to enforce a
    /// zoom-out floor tied to the system's native resolution.
    /// </summary>
    public virtual Vector2? MaxZoomOutArea => null;

    // ── Tile art ─────────────────────────────────────────────────────────────

    protected abstract Color[]  TilePalette { get; }
    protected abstract byte[][] GetTilePixels(TileType tileType, Color accentColor);

    // ── Sprite dimensions ────────────────────────────────────────────────────

    public abstract int CharWidth  { get; }
    public abstract int HeadRows   { get; }
    public abstract int BodyRows   { get; }
    public abstract int LegsRows   { get; }
    public int CharHeight => HeadRows + BodyRows + LegsRows;

    // ── Sprite part pixel art ─────────────────────────────────────────────────
    // Layout: [variantIndex][animFrame][row][col]
    // Head/body variants typically carry 1 frame (static); legs carry 4 walk frames.
    // The base class uses Math.Min(frameIndex, variant.Length-1) so 1-frame parts
    // are safe to use alongside 4-frame legs.

    public abstract byte[][][][] HeadParts { get; }
    public abstract byte[][][][] BodyParts { get; }
    public abstract byte[][][][] LegsParts { get; }

    // ── Sprite palettes ───────────────────────────────────────────────────────
    // Expected counts: 5 HeadPalettes, 5 BodyPalettes, 4 LegsPalettes
    // (matching the indices used in NpcAppearances)

    public abstract HeadPalette[] HeadPalettes { get; }
    public abstract BodyPalette[] BodyPalettes { get; }
    public abstract LegsPalette[] LegsPalettes { get; }

    // ── Public builders ───────────────────────────────────────────────────────

    public Tileset BuildTileset(
        GraphicsDevice gd,
        string         name,
        TileType[]     tileTypes,
        Color          accentColor = default,
        int            firstGid    = 1)
    {
        var effectivePalette = new Color[TilePalette.Length + 1];
        TilePalette.CopyTo(effectivePalette, 0);
        effectivePalette[TilePalette.Length] =
            accentColor == default ? new Color(180, 180, 180) : accentColor;

        const int target = 16;
        int   count   = tileTypes.Length;
        var   texture = new Texture2D(gd, target * count, target);
        var   data    = new Color[target * count * target];

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
                    if (DoubleWidePixels) srcCol = Math.Min(srcCol & ~1, nativeW - 1);
                    byte idx   = pixels[srcRow][srcCol];
                    Color c    = idx < effectivePalette.Length ? effectivePalette[idx] : effectivePalette[0];
                    data[ty * (target * count) + i * target + tx] = c;
                }
            }
        }

        texture.SetData(data);
        return new Tileset(name, texture, target, target, firstGid);
    }

    public AnimatedSprite BuildCharacterSprite(GraphicsDevice gd, CharacterAppearance a)
    {
        var headVariant = HeadParts[a.HeadIndex];
        var bodyVariant = BodyParts[a.BodyIndex];
        var legsVariant = LegsParts[a.LegsIndex];
        var hp = HeadPalettes[a.HeadPaletteIndex];
        var bp = BodyPalettes[a.BodyPaletteIndex];
        var lp = LegsPalettes[a.LegsPaletteIndex];

        const int frames = 4;
        var texture = new Texture2D(gd, CharWidth * frames, CharHeight);
        var data    = new Color[CharWidth * frames * CharHeight];

        for (int f = 0; f < frames; f++)
        {
            var headFrame = headVariant[Math.Min(f, headVariant.Length - 1)];
            var bodyFrame = bodyVariant[Math.Min(f, bodyVariant.Length - 1)];
            var legsFrame = legsVariant[Math.Min(f, legsVariant.Length - 1)];

            PastePart(data, frames, headFrame, f, 0,                    hp.Resolve);
            PastePart(data, frames, bodyFrame, f, HeadRows,             bp.Resolve);
            PastePart(data, frames, legsFrame, f, HeadRows + BodyRows,  lp.Resolve);
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

    private void PastePart(
        Color[]          data,
        int              totalFrames,
        byte[][]         part,
        int              frame,
        int              rowOffset,
        Func<int, Color> resolve)
    {
        for (int y = 0; y < part.Length; y++)
        {
            var srcRow = part[y];
            for (int x = 0; x < CharWidth; x++)
            {
                int srcX = DoubleWidePixels ? x & ~1 : x;
                byte idx = srcX < srcRow.Length ? srcRow[srcX] : (byte)0;
                data[(rowOffset + y) * (CharWidth * totalFrames) + frame * CharWidth + x] =
                    idx == 0 ? Color.Transparent : resolve(idx);
            }
        }
    }
}
