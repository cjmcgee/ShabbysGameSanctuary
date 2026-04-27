using ChildhoodAdventure.RetroSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine.Rendering;

namespace ChildhoodAdventure
{
    /// <summary>
    /// Programmatic sprite builders — no content pipeline needed.
    /// Delegates character sprite creation to the active <see cref="RetroSystem"/>,
    /// so every system produces proportionally correct, palette-appropriate sprites.
    /// </summary>
    public static class SpriteFactory
    {
        /// <summary>
        /// Creates a character sprite assembled from head, body, and legs parts,
        /// using the shape variants and palette indices specified by <paramref name="appearance"/>.
        /// </summary>
        public static AnimatedSprite BuildCharacter(GraphicsDevice gd, CharacterAppearance appearance)
            => RetroSystemRegistry.Current.BuildCharacterSprite(gd, appearance);

        /// <summary>Creates a static flat-color sprite (for map objects / items).</summary>
        public static AnimatedSprite BuildStatic(GraphicsDevice gd, int w, int h, Color color)
        {
            var tex  = new Texture2D(gd, w, h);
            tex.SetData(Enumerable.Repeat(color, w * h).ToArray());

            var idle = new SpriteAnimation("idle",
                new[] { new AnimationFrame(new Rectangle(0, 0, w, h), 1f) });

            var sprite = new AnimatedSprite(tex, w, h) { Origin = new Vector2(w / 2f, h / 2f) };
            sprite.AddAnimation(idle);
            sprite.Play("idle");
            return sprite;
        }
    }
}
