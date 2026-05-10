using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine.Rendering;

namespace ChildhoodAdventure;

/// <summary>
/// Tiny 16×10 pixel sprite for the in-world Atari 2600 console — wood-grain
/// top with the iconic six switches, black plastic body, joystick cable
/// trailing off to the right.
///
/// The art is hand-encoded as a string grid so it's diffable and easy to
/// tweak without an asset pipeline.
/// </summary>
public static class AtariConsoleSprite
{
	// Palette indices used in the layout below.
	private static readonly Color[]	Palette =
	{
		Color.Transparent,			// 0 — empty
		new(120, 78, 38),			// 1 — wood top
		new(160, 110, 60),			// 2 — wood highlight
		new( 18, 18, 22),			// 3 — black body
		new(200, 200, 210),			// 4 — switches / silver
		new( 80, 60, 30),			// 5 — wood shadow
		new( 32, 32, 36),			// 6 — body shadow
	};

	// 16 wide × 10 tall. Read top-down. Spaces are ignored to make the grid
	// easier to scan; only digits 0-6 contribute.
	private static readonly string[]	Layout =
	{
		"5 1 1 1 2 1 1 2 1 1 2 1 1 1 1 5",
		"5 2 1 2 1 1 2 1 1 1 1 2 1 1 2 5",
		"5 1 1 1 1 4 4 1 4 4 1 4 4 1 1 5",
		"5 1 2 1 1 1 1 2 1 1 1 1 1 2 1 5",
		"6 3 3 3 3 3 3 3 3 3 3 3 3 3 3 6",
		"6 3 4 3 3 3 3 3 3 3 3 3 4 3 3 6",
		"6 3 3 3 3 3 3 3 3 3 3 3 3 3 3 6",
		"6 3 3 3 3 4 4 3 3 4 4 3 3 3 3 6",
		"6 3 3 3 3 3 3 3 3 3 3 3 3 3 3 6",
		"6 6 6 6 6 6 6 6 6 6 6 6 6 6 6 6",
	};

	/// <summary>Build a single-frame <see cref="AnimatedSprite"/> of the console.</summary>
	public static AnimatedSprite Build(GraphicsDevice gd)
	{
		const int W =	16;
		const int H =	10;
		var pixels =	new Color[W * H];
		for (int y = 0; y < H; y++)
		{
			int x =	0;
			foreach (char c in Layout[y])
			{
				if (c is < '0' or > '9')	continue;	// skip spaces
				int idx =	c - '0';
				if (idx < Palette.Length)	pixels[y * W + x]	=	Palette[idx];
				x++;
				if (x >= W)	break;
			}
		}
		var tex =	new Texture2D(gd, W, H);
		tex.SetData(pixels);

		// One tile wide visually; height naturally smaller.
		var sprite =	new AnimatedSprite(tex, W, H, new Vector2(1f, H / (float)W));
		// SpriteComponent's directional dispatch builds keys like
		// "idle_down" / "idle_up" / "idle_right". The console is static and
		// non-directional, so we register the single frame under each.
		sprite.AddAnimation(SpriteAnimation.FromStrip("idle",       tex, W, H, 1, 0, 1f));
		sprite.AddAnimation(SpriteAnimation.FromStrip("idle_down",  tex, W, H, 1, 0, 1f));
		sprite.AddAnimation(SpriteAnimation.FromStrip("idle_up",    tex, W, H, 1, 0, 1f));
		sprite.AddAnimation(SpriteAnimation.FromStrip("idle_right", tex, W, H, 1, 0, 1f));
		return sprite;
	}
}
