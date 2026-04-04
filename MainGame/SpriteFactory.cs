using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine.Rendering;

namespace ChildhoodAdventure
{
    /// <summary>
    /// Programmatic sprite builders — no content pipeline needed.
    /// Characters are simple silhouettes (head + body + animated legs)
    /// in the flat-color Atari 2600 spirit.
    /// </summary>
    public static class SpriteFactory
    {
        /// <summary>
        /// Creates a 16×16 character sprite with a 4-frame walk cycle and idle animation.
        /// </summary>
        public static AnimatedSprite BuildCharacter(GraphicsDevice gd, Color color)
        {
            const int w = 16, h = 16, frames = 4;
            var tex = new Texture2D(gd, w * frames, h);
            var data = new Color[w * frames * h];

            // Atari 2600 Adventure style: bold single flat colour, blocky silhouette.
            // Every visible pixel is exactly `color` — zero shading or gradient.
            for (int f = 0; f < frames; f++)
            {
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        bool show = false;

                        // Head — square block centred at top
                        if (y <= 4 && x >= 5 && x <= 10) show = true;

                        // Body — wide solid block
                        if (y >= 5 && y <= 11 && x >= 3 && x <= 12) show = true;

                        // Legs — two separate columns; alternate which foot is lowest
                        if (y >= 12 && y <= 15)
                        {
                            bool left  = x >= 3 && x <= 5;
                            bool right = x >= 10 && x <= 12;
                            if (f < 2)
                            {
                                if (left)             show = true;       // left leg full length
                                if (right && y <= 14) show = true;       // right leg shorter
                            }
                            else
                            {
                                if (left  && y <= 14) show = true;       // left leg shorter
                                if (right)            show = true;       // right leg full length
                            }
                        }

                        data[y * (w * frames) + f * w + x] = show ? color : Color.Transparent;
                    }
                }
            }

            tex.SetData(data);

            var walk  = SpriteAnimation.FromStrip("walk",  tex, w, h, frames, 0, 0.15f);
            var idle  = SpriteAnimation.FromStrip("idle",  tex, w, h, 1,      0, 1.0f);

            var sprite = new AnimatedSprite(tex, w, h);
            sprite.AddAnimation(walk);
            sprite.AddAnimation(idle);
            sprite.Play("idle");
            return sprite;
        }

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
