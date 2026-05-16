using TileEngine.Rendering;

namespace ChildhoodAdventure.RetroSystems;

/// <summary>
/// Pixel art for the in-world Atari 2600 console icon. The shape is the
/// same across every retro system; the colours come from the system's
/// <see cref="ConsolePalette"/> and the system's
/// <see cref="RetroSystem.DoubleWidePixels"/> flag is honoured at build
/// time (Atari 2600 / C64 etc. render every odd column as the previous
/// column's value).
/// </summary>
public static class AtariConsoleArt
{
	public const int Width =	16;
	public const int Height =	10;

	// Layout: 16 wide × 10 tall. Spaces are skipped so the grid is easy to
	// scan; only digits 0-6 contribute. Indices map to ConsolePalette roles:
	//   0=transparent  1=Wood  2=WoodLight  3=Body  4=Switch  5=WoodShadow  6=BodyShadow
	public static readonly string[]	Layout =
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

	/// <summary>
	/// Build a single-frame <see cref="AnimatedSprite"/> of the console using
	/// <paramref name="palette"/>. When <paramref name="doubleWidePixels"/>
	/// is true (Atari 2600, C64), every odd column copies the value of the
	/// preceding even column so the sprite reads at the system's native
	/// horizontal resolution.
	/// </summary>
	public static AnimatedSprite Build(GraphicsDevice gd, ConsolePalette palette, bool doubleWidePixels = false)
	{
		var pixels =	new Color[Width * Height];
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				int srcCol =	doubleWidePixels ? (x & ~1) : x;
				int srcIdx =	IndexAt(Layout[y], srcCol);
				pixels[y * Width + x]	=	Resolve(srcIdx, palette);
			}
		}

		var tex =	new Texture2D(gd, Width, Height);
		tex.SetData(pixels);

		var sprite =	new AnimatedSprite(tex, Width, Height, new Vector2(1f, Height / (float)Width));
		sprite.AddAnimation(SpriteAnimation.FromStrip("idle",       tex, Width, Height, 1, 0, 1f));
		sprite.AddAnimation(SpriteAnimation.FromStrip("idle_down",  tex, Width, Height, 1, 0, 1f));
		sprite.AddAnimation(SpriteAnimation.FromStrip("idle_up",    tex, Width, Height, 1, 0, 1f));
		sprite.AddAnimation(SpriteAnimation.FromStrip("idle_right", tex, Width, Height, 1, 0, 1f));
		return sprite;
	}

	private static int IndexAt(string row, int col)
	{
		int x =	0;
		foreach (char c in row)
		{
			if (c is < '0' or > '9')	continue;
			if (x == col)	return c - '0';
			x++;
		}
		return 0;
	}

	private static Color Resolve(int idx, ConsolePalette p)	=>	idx switch
	{
		1 => p.Wood,
		2 => p.WoodLight,
		3 => p.Body,
		4 => p.Switch,
		5 => p.WoodShadow,
		6 => p.BodyShadow,
		_ => Color.Transparent,
	};
}
