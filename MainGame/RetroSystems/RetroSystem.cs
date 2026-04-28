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

    /// <summary>
    /// When true, each tile is quantized to 1-bit color at build time: palette index 0
    /// maps to the background (black) and every other index maps to the tile's single
    /// foreground color (the lowest non-zero palette index found in that tile's art).
    /// Mimics the Atari 2600 playfield hardware which allowed only one color per tile.
    /// </summary>
    protected virtual bool OneBitTiles => false;

    /// <summary>
    /// When true, each horizontal scanline of a sprite is collapsed to a single
    /// non-transparent color: the first non-transparent pixel's resolved color is used
    /// for every non-transparent pixel on that row.  Mimics the Atari 2600 TIA player
    /// objects which could change color only between scanlines, not within them.
    /// </summary>
    protected virtual bool SpriteOneColorPerScanline => false;

    /// <summary>
    /// Palette index of the one "global" color shared across all tiles (analogous to the
    /// C64 VIC-II border/multicolor register). Every tile may freely use this index in
    /// addition to its own MaxLocalTileColors local indices. -1 = no global constraint.
    /// </summary>
    protected virtual int GlobalTileColorIndex => -1;

    /// <summary>
    /// Maximum distinct non-background, non-global palette indices per tile.
    /// Together with GlobalTileColorIndex this limits each tile to:
    ///   background + global + MaxLocalTileColors colors.
    /// Pixels that use additional indices are remapped to the global color.
    /// 0 = unlimited.
    /// </summary>
    protected virtual int MaxLocalTileColors => 0;

    /// <summary>
    /// Maximum distinct non-transparent semantic indices allowed per sprite part frame.
    /// Indices beyond this limit (sorted numerically, lowest kept) are remapped to the
    /// lowest allowed index.  Mimics the C64 VIC-II 4-color sprite limit (transparent +
    /// 2 global registers + 1 sprite-unique color).  0 = unlimited.
    /// </summary>
    protected virtual int MaxSpriteSemanticColors => 0;

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

            // 1-bit tile mode: find the first (lowest) non-zero palette index in this
            // tile's art and use its color as the sole foreground; index 0 stays black.
            Color? oneBitFg = null;
            if (OneBitTiles)
            {
                byte minIdx = byte.MaxValue;
                foreach (var row in pixels)
                    foreach (var b in row)
                        if (b != 0 && b < minIdx) minIdx = b;
                if (minIdx != byte.MaxValue)
                    oneBitFg = minIdx < effectivePalette.Length
                        ? effectivePalette[minIdx] : effectivePalette[0];
            }

            // Multicolor tile mode: global palette index + up to MaxLocalTileColors locals.
            // Collect local indices in sorted order; pixels beyond the limit remap to global.
            HashSet<byte>? allowedLocalTile = null;
            if (GlobalTileColorIndex >= 0 && MaxLocalTileColors > 0)
            {
                var locals = new SortedSet<byte>();
                foreach (var row in pixels)
                    foreach (var b in row)
                        if (b != 0 && b != (byte)GlobalTileColorIndex)
                            locals.Add(b);
                allowedLocalTile = new HashSet<byte>(locals.Take(MaxLocalTileColors));
            }

            for (int ty = 0; ty < target; ty++)
            {
                int srcRow = Math.Min((int)(ty / ry), nativeH - 1);
                for (int tx = 0; tx < target; tx++)
                {
                    int srcCol = Math.Min((int)(tx / rx), nativeW - 1);
                    if (DoubleWidePixels) srcCol = Math.Min(srcCol & ~1, nativeW - 1);
                    byte idx = pixels[srcRow][srcCol];
                    Color c;
                    if (oneBitFg.HasValue)
                        c = idx == 0 ? effectivePalette[0] : oneBitFg.Value;
                    else if (allowedLocalTile != null)
                        c = idx == 0 ? effectivePalette[0]
                            : (idx == (byte)GlobalTileColorIndex || allowedLocalTile.Contains(idx)
                                ? (idx < effectivePalette.Length ? effectivePalette[idx] : effectivePalette[0])
                                : effectivePalette[GlobalTileColorIndex]);
                    else
                        c = idx < effectivePalette.Length ? effectivePalette[idx] : effectivePalette[0];
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
        // Multicolor sprite mode: collect all distinct non-zero semantics in this frame,
        // keep only the first MaxSpriteSemanticColors (lowest indices), remap the rest
        // to the lowest allowed index.
        SortedSet<byte>? allowedSemantic = null;
        if (MaxSpriteSemanticColors > 0)
        {
            var semantics = new SortedSet<byte>();
            foreach (var row in part)
                foreach (var b in row)
                    if (b != 0) semantics.Add(b);
            if (semantics.Count > MaxSpriteSemanticColors)
                allowedSemantic = new SortedSet<byte>(semantics.Take(MaxSpriteSemanticColors));
        }

        for (int y = 0; y < part.Length; y++)
        {
            var srcRow = part[y];

            // One-color-per-scanline: the first non-transparent pixel's color applies
            // to every non-transparent pixel on this row.
            Color? scanlineColor = null;
            if (SpriteOneColorPerScanline)
            {
                for (int x = 0; x < CharWidth; x++)
                {
                    int sx = DoubleWidePixels ? x & ~1 : x;
                    byte b = sx < srcRow.Length ? srcRow[sx] : (byte)0;
                    if (b != 0) { scanlineColor = resolve(b); break; }
                }
            }

            for (int x = 0; x < CharWidth; x++)
            {
                int srcX = DoubleWidePixels ? x & ~1 : x;
                byte idx = srcX < srcRow.Length ? srcRow[srcX] : (byte)0;
                Color pixel;
                if (idx == 0)
                    pixel = Color.Transparent;
                else if (scanlineColor.HasValue)
                    pixel = scanlineColor.Value;
                else if (allowedSemantic != null && !allowedSemantic.Contains(idx))
                    pixel = resolve(allowedSemantic.Min);
                else
                    pixel = resolve(idx);
                data[(rowOffset + y) * (CharWidth * totalFrames) + frame * CharWidth + x] = pixel;
            }
        }
    }
}
