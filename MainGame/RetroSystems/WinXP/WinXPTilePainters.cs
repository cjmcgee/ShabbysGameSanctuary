using ChildhoodAdventure.RetroSystems.Assets;

namespace ChildhoodAdventure.RetroSystems.WinXP;

/// <summary>
/// Procedural painters for every tile in the WinXP tile sheet. Each routine
/// renders a 32×32 cell into a larger sheet buffer at the requested origin.
/// The art is intentionally simple but uses unrestricted RGBA values, so it
/// reads as "modern pixel art" rather than emulated retro hardware.
///
/// <see cref="TileSentinels.AccentMarker"/> (pure magenta) is used wherever
/// a tile wants the per-house accent color; <c>RetroSystem.BuildTileset</c>
/// substitutes the runtime accent into those pixels.
/// </summary>
internal static class WinXPTilePainters
{
	private const int N = WinXPAssetSeed.TileSize;	// 32

	// ── Color constants (24-bit, no palette) ──────────────────────────────
	private static readonly Color WoodLight  = new(190, 145, 95);
	private static readonly Color WoodMid    = new(155, 110, 70);
	private static readonly Color WoodDark   = new(115,  80, 50);
	private static readonly Color WoodGrain  = new( 95,  65, 40);
	private static readonly Color CarpetBase = new(110,  80, 130);
	private static readonly Color CarpetWeave= new( 95,  65, 115);
	private static readonly Color CarpetEdge = new(170, 140, 180);
	private static readonly Color TileLight  = new(235, 230, 220);
	private static readonly Color TileDark   = new(210, 200, 185);
	private static readonly Color Grout      = new(120, 110,  95);
	private static readonly Color WallBase   = new(230, 225, 215);
	private static readonly Color WallShadow = new(200, 195, 180);
	private static readonly Color WallSpeck  = new(245, 240, 230);
	private static readonly Color DoorFrame  = new( 60,  40,  25);
	private static readonly Color DoorWood   = new(140,  85,  45);
	private static readonly Color DoorPanel  = new(115,  70,  35);
	private static readonly Color DoorKnob   = new(220, 180,  60);
	private static readonly Color WindowFrame= new(245, 240, 230);
	private static readonly Color GlassTop   = new(150, 195, 230);
	private static readonly Color GlassBot   = new(110, 165, 215);
	private static readonly Color FurnFrame  = new( 70,  55,  45);
	private static readonly Color FurnCushion= new(190,  90,  80);
	private static readonly Color FurnHL     = new(230, 130, 110);
	private static readonly Color CounterTop = new(225, 220, 210);
	private static readonly Color CounterVein= new(180, 175, 165);
	private static readonly Color CounterEdge= new(170, 165, 155);
	private static readonly Color ShelfWood  = new(120,  75,  35);
	private static readonly Color ShelfHL    = new(160, 110,  60);
	private static readonly Color Book1      = new(180,  55,  60);
	private static readonly Color Book2      = new( 60, 100, 165);
	private static readonly Color Book3      = new( 80, 150,  90);
	private static readonly Color Book4      = new(210, 170,  60);
	private static readonly Color Book5      = new(120,  80, 170);
	private static readonly Color Pot        = new(180,  90,  60);
	private static readonly Color PotShadow  = new(135,  60,  40);
	private static readonly Color LeafBase   = new( 55, 140,  75);
	private static readonly Color LeafHL     = new( 95, 180, 105);
	private static readonly Color LeafShadow = new( 35, 100,  55);
	private static readonly Color GrassBase  = new( 90, 160,  85);
	private static readonly Color GrassHL    = new(130, 195, 105);
	private static readonly Color GrassShadow= new( 65, 130,  65);
	private static readonly Color FlowerYellow=new(245, 220,  90);
	private static readonly Color FlowerPink = new(245, 160, 195);
	private static readonly Color RoadBase   = new( 55,  55,  60);
	private static readonly Color RoadHL     = new( 80,  80,  85);
	private static readonly Color RoadStripe = new(220, 195, 100);
	private static readonly Color SidewalkBase= new(200, 200, 195);
	private static readonly Color SidewalkHL = new(220, 220, 215);
	private static readonly Color SidewalkLn = new(170, 170, 165);
	private static readonly Color RoofDark   = new( 70,  60,  70);
	private static readonly Color RoofMid    = new(105,  90, 100);
	private static readonly Color RoofHL     = new(130, 115, 125);
	private static readonly Color BushDark   = new( 50, 110,  60);
	private static readonly Color BushMid    = new( 80, 150,  85);
	private static readonly Color BushHL     = new(125, 185, 110);
	private static readonly Color AccentSlot = TileSentinels.AccentMarker;
	private static readonly Color Transparent= new(0, 0, 0, 0);

	// ── Drawing primitives ────────────────────────────────────────────────
	private static void Plot(Color[] buf, int stride, int x0, int y0, int x, int y, Color c)
	{
		if (x < 0 || x >= N || y < 0 || y >= N) return;
		buf[(y0 + y) * stride + (x0 + x)] = c;
	}

	private static void Fill(Color[] buf, int stride, int x0, int y0, Color c)
	{
		for (int y = 0; y < N; y++)
			for (int x = 0; x < N; x++)
				buf[(y0 + y) * stride + (x0 + x)] = c;
	}

	private static void Rect(Color[] buf, int stride, int x0, int y0,
		int x1, int y1, int x2, int y2, Color c)
	{
		for (int y = y1; y <= y2; y++)
			for (int x = x1; x <= x2; x++)
				Plot(buf, stride, x0, y0, x, y, c);
	}

	private static void HLine(Color[] buf, int stride, int x0, int y0,
		int x1, int x2, int y, Color c)
	{
		for (int x = x1; x <= x2; x++) Plot(buf, stride, x0, y0, x, y, c);
	}

	private static void VLine(Color[] buf, int stride, int x0, int y0,
		int x, int y1, int y2, Color c)
	{
		for (int y = y1; y <= y2; y++) Plot(buf, stride, x0, y0, x, y, c);
	}

	private static void Border(Color[] buf, int stride, int x0, int y0,
		int x1, int y1, int x2, int y2, Color c)
	{
		HLine(buf, stride, x0, y0, x1, x2, y1, c);
		HLine(buf, stride, x0, y0, x1, x2, y2, c);
		VLine(buf, stride, x0, y0, x1, y1, y2, c);
		VLine(buf, stride, x0, y0, x2, y1, y2, c);
	}

	/// <summary>
	/// Splatter pseudo-random pixels in a region; deterministic per (tile, seed)
	/// so a repaint produces identical art. Uses a small linear-congruential
	/// generator inline to avoid System.Random allocations.
	/// </summary>
	private static void Sprinkle(Color[] buf, int stride, int x0, int y0,
		int x1, int y1, int x2, int y2, Color c, int every, uint seed)
	{
		uint s = seed | 1u;
		for (int y = y1; y <= y2; y++)
		{
			for (int x = x1; x <= x2; x++)
			{
				s = s * 1664525u + 1013904223u;
				if ((s & 0xFF) % (uint)every == 0)
					Plot(buf, stride, x0, y0, x, y, c);
			}
		}
	}

	// ── Per-tile painters ─────────────────────────────────────────────────

	public static void WoodFloor(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, WoodMid);
		// Horizontal planks every 8 rows with a dark seam between them.
		for (int plank = 0; plank < 4; plank++)
		{
			int top = plank * 8;
			Color shade = plank % 2 == 0 ? WoodMid : WoodLight;
			Rect(buf, stride, x0, y0, 0, top, N - 1, top + 6, shade);
			HLine(buf, stride, x0, y0, 0, N - 1, top + 7, WoodDark);
		}
		// Grain streaks: short dark dashes per plank, scattered.
		Sprinkle(buf, stride, x0, y0, 0, 0, N - 1, N - 1, WoodGrain, 22, 0xA1B2C3D4u);
	}

	public static void Carpet(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, CarpetBase);
		// Soft inner border to suggest a rug edge.
		Border(buf, stride, x0, y0, 1, 1, N - 2, N - 2, CarpetEdge);
		Border(buf, stride, x0, y0, 3, 3, N - 4, N - 4, CarpetWeave);
		// Diagonal weave dots evenly spaced.
		for (int y = 5; y < N - 5; y += 4)
		{
			for (int x = 5 + (y / 4) % 2 * 2; x < N - 5; x += 4)
			{
				Plot(buf, stride, x0, y0, x, y, CarpetWeave);
			}
		}
	}

	public static void KitchenTile(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, TileLight);
		// 4 tiles in a 2×2 grid with grout lines.
		HLine(buf, stride, x0, y0, 0, N - 1, N / 2 - 1, Grout);
		HLine(buf, stride, x0, y0, 0, N - 1, N / 2, Grout);
		VLine(buf, stride, x0, y0, N / 2 - 1, 0, N - 1, Grout);
		VLine(buf, stride, x0, y0, N / 2, 0, N - 1, Grout);
		// Subtle dark corner on each tile to suggest ceramic shading.
		for (int by = 0; by < 2; by++)
		{
			for (int bx = 0; bx < 2; bx++)
			{
				int ox = bx * (N / 2) + 1;
				int oy = by * (N / 2) + 1;
				Rect(buf, stride, x0, y0, ox + 11, oy + 11, ox + 12, oy + 12, TileDark);
			}
		}
	}

	public static void Wall(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, WallBase);
		// Faint horizontal banding to suggest paint roller marks.
		for (int y = 4; y < N; y += 9)
		{
			HLine(buf, stride, x0, y0, 0, N - 1, y, WallShadow);
		}
		Sprinkle(buf, stride, x0, y0, 0, 0, N - 1, N - 1, WallSpeck, 20, 0x14253647u);
	}

	public static void Door(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, DoorWood);
		Border(buf, stride, x0, y0, 0, 0, N - 1, N - 1, DoorFrame);
		Border(buf, stride, x0, y0, 1, 1, N - 2, N - 2, DoorFrame);
		// Two stacked panels.
		Rect(buf, stride, x0, y0, 5, 4, N - 6, 13, DoorPanel);
		Border(buf, stride, x0, y0, 5, 4, N - 6, 13, DoorFrame);
		Rect(buf, stride, x0, y0, 5, 17, N - 6, N - 6, DoorPanel);
		Border(buf, stride, x0, y0, 5, 17, N - 6, N - 6, DoorFrame);
		// Knob.
		Plot(buf, stride, x0, y0, N - 5, N / 2 + 1, DoorKnob);
		Plot(buf, stride, x0, y0, N - 5, N / 2 + 2, DoorKnob);
		Plot(buf, stride, x0, y0, N - 4, N / 2 + 1, DoorKnob);
		Plot(buf, stride, x0, y0, N - 4, N / 2 + 2, DoorKnob);
	}

	public static void Window(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, WallBase);
		// Frame
		Border(buf, stride, x0, y0, 2, 2, N - 3, N - 3, WallShadow);
		Rect(buf, stride, x0, y0, 3, 3, N - 4, N - 4, WindowFrame);
		// Glass with vertical gradient.
		for (int y = 4; y <= N - 5; y++)
		{
			float t = (y - 4) / (float)(N - 9);
			var c = Lerp(GlassTop, GlassBot, t);
			for (int x = 4; x <= N - 5; x++) Plot(buf, stride, x0, y0, x, y, c);
		}
		// Mullion cross.
		HLine(buf, stride, x0, y0, 3, N - 4, N / 2 - 1, WindowFrame);
		HLine(buf, stride, x0, y0, 3, N - 4, N / 2, WindowFrame);
		VLine(buf, stride, x0, y0, N / 2 - 1, 3, N - 4, WindowFrame);
		VLine(buf, stride, x0, y0, N / 2, 3, N - 4, WindowFrame);
	}

	public static void Furniture(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, WoodMid);
		// Cushion seat (rounded rectangle approximation).
		Rect(buf, stride, x0, y0, 3, 6, N - 4, N - 7, FurnCushion);
		Border(buf, stride, x0, y0, 3, 6, N - 4, N - 7, FurnFrame);
		// Highlight along the top of the cushion.
		HLine(buf, stride, x0, y0, 5, N - 6, 7, FurnHL);
		HLine(buf, stride, x0, y0, 5, N - 6, 8, FurnHL);
		// Arms / sides.
		Rect(buf, stride, x0, y0, 1, 8, 2, N - 4, FurnFrame);
		Rect(buf, stride, x0, y0, N - 3, 8, N - 2, N - 4, FurnFrame);
		// Legs.
		Plot(buf, stride, x0, y0, 4, N - 2, FurnFrame);
		Plot(buf, stride, x0, y0, 4, N - 1, FurnFrame);
		Plot(buf, stride, x0, y0, N - 5, N - 2, FurnFrame);
		Plot(buf, stride, x0, y0, N - 5, N - 1, FurnFrame);
	}

	public static void Counter(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, CounterTop);
		// Veining (organic-ish curves)
		for (int i = 0; i < 12; i++)
		{
			int x = (i * 7 + 3) % N;
			int y = (i * 5 + 1) % N;
			Plot(buf, stride, x0, y0, x, y, CounterVein);
			Plot(buf, stride, x0, y0, (x + 1) % N, (y + 1) % N, CounterVein);
		}
		// Bottom edge band (counter depth).
		HLine(buf, stride, x0, y0, 0, N - 1, N - 3, CounterEdge);
		HLine(buf, stride, x0, y0, 0, N - 1, N - 2, CounterEdge);
		HLine(buf, stride, x0, y0, 0, N - 1, N - 1, CounterEdge);
	}

	public static void Bookshelf(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, ShelfWood);
		// Two shelves of books.
		Color[] bookPal = [Book1, Book2, Book3, Book4, Book5];
		for (int shelf = 0; shelf < 2; shelf++)
		{
			int top = 3 + shelf * 14;
			int bot = top + 10;
			Rect(buf, stride, x0, y0, 1, top, N - 2, bot, ShelfWood);
			int bx = 1;
			int p = 0;
			while (bx < N - 1)
			{
				int w = 2 + (shelf * 3 + p) % 3;
				if (bx + w >= N - 1) w = N - 2 - bx;
				if (w < 1) break;
				Rect(buf, stride, x0, y0, bx, top, bx + w - 1, bot, bookPal[(shelf * 7 + p) % bookPal.Length]);
				bx += w + 1;
				p++;
			}
			// Top edge of each shelf board.
			HLine(buf, stride, x0, y0, 0, N - 1, bot + 1, ShelfHL);
		}
	}

	public static void Plant(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, Transparent);
		// Pot at the bottom.
		Rect(buf, stride, x0, y0, 10, 22, N - 11, N - 3, Pot);
		HLine(buf, stride, x0, y0, 9, N - 10, 22, PotShadow);
		HLine(buf, stride, x0, y0, 9, N - 10, 23, Pot);
		// Leaves (irregular cluster).
		int[] leafXs = [8, 10, 11, 13, 15, 17, 19, 21, 23];
		int[] leafYs = [10, 14, 8, 12, 6, 12, 8, 14, 10];
		for (int i = 0; i < leafXs.Length; i++)
		{
			DrawLeaf(buf, stride, x0, y0, leafXs[i], leafYs[i]);
		}
	}

	private static void DrawLeaf(Color[] buf, int stride, int x0, int y0, int cx, int cy)
	{
		Plot(buf, stride, x0, y0, cx, cy, LeafBase);
		Plot(buf, stride, x0, y0, cx + 1, cy, LeafHL);
		Plot(buf, stride, x0, y0, cx, cy + 1, LeafShadow);
		Plot(buf, stride, x0, y0, cx - 1, cy, LeafBase);
		Plot(buf, stride, x0, y0, cx, cy - 1, LeafBase);
	}

	public static void Grass(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, GrassBase);
		Sprinkle(buf, stride, x0, y0, 0, 0, N - 1, N - 1, GrassShadow, 12, 0xDEADBEEFu);
		Sprinkle(buf, stride, x0, y0, 0, 0, N - 1, N - 1, GrassHL, 14, 0xCAFEBABEu);
	}

	public static void Grass2(Color[] buf, int stride, int x0, int y0)
	{
		Grass(buf, stride, x0, y0);
		// One small flower at a fixed spot.
		int fx = 18, fy = 10;
		Plot(buf, stride, x0, y0, fx,     fy,     FlowerYellow);
		Plot(buf, stride, x0, y0, fx - 1, fy,     FlowerPink);
		Plot(buf, stride, x0, y0, fx + 1, fy,     FlowerPink);
		Plot(buf, stride, x0, y0, fx,     fy - 1, FlowerPink);
		Plot(buf, stride, x0, y0, fx,     fy + 1, FlowerPink);
		Plot(buf, stride, x0, y0, fx,     fy + 2, LeafShadow);
	}

	public static void Road(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, RoadBase);
		Sprinkle(buf, stride, x0, y0, 0, 0, N - 1, N - 1, RoadHL, 18, 0x12345678u);
		// Dashed center stripe across the horizontal midline.
		for (int x = 4; x < N - 4; x++)
		{
			bool dash = ((x / 4) % 2) == 0;
			if (dash)
			{
				Plot(buf, stride, x0, y0, x, N / 2 - 1, RoadStripe);
				Plot(buf, stride, x0, y0, x, N / 2, RoadStripe);
			}
		}
	}

	public static void Sidewalk(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, SidewalkBase);
		Sprinkle(buf, stride, x0, y0, 0, 0, N - 1, N - 1, SidewalkHL, 22, 0x9ABCDEF0u);
		// Cross seams to mimic concrete sections.
		HLine(buf, stride, x0, y0, 0, N - 1, N / 2, SidewalkLn);
		VLine(buf, stride, x0, y0, N / 2, 0, N - 1, SidewalkLn);
	}

	public static void HouseExterior(Color[] buf, int stride, int x0, int y0)
	{
		// Whole tile gets accent-marker; shadow lines mark each siding course.
		Fill(buf, stride, x0, y0, AccentSlot);
		for (int y = 7; y < N; y += 8)
		{
			HLine(buf, stride, x0, y0, 0, N - 1, y, DoorFrame);
		}
	}

	public static void HouseRoof(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, RoofMid);
		// Shingle rows: dark line every 6 rows + speckled highlights.
		for (int y = 5; y < N; y += 6)
		{
			HLine(buf, stride, x0, y0, 0, N - 1, y, RoofDark);
		}
		Sprinkle(buf, stride, x0, y0, 0, 0, N - 1, N - 1, RoofHL, 18, 0x55667788u);
	}

	public static void Bush(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, Transparent);
		// Filled "circle" approximation by distance check.
		int cx = N / 2, cy = N / 2;
		float rOuter = 14f, rMid = 10f;
		for (int y = 0; y < N; y++)
		{
			for (int x = 0; x < N; x++)
			{
				float dx = x - cx + 0.5f;
				float dy = y - cy + 0.5f;
				float d = MathF.Sqrt(dx * dx + dy * dy);
				if (d > rOuter) continue;
				Color c = d > rMid ? BushDark : (d > rMid - 4 ? BushMid : BushHL);
				Plot(buf, stride, x0, y0, x, y, c);
			}
		}
		// Touch of darker leaf clumps.
		Plot(buf, stride, x0, y0, cx - 4, cy - 2, BushDark);
		Plot(buf, stride, x0, y0, cx + 3, cy + 3, BushDark);
	}

	public static void Accent(Color[] buf, int stride, int x0, int y0)
	{
		Fill(buf, stride, x0, y0, AccentSlot);
	}

	// ── Color math helpers ────────────────────────────────────────────────
	private static Color Lerp(Color a, Color b, float t)
	{
		t = Math.Clamp(t, 0f, 1f);
		return new Color(
			(byte)(a.R + (b.R - a.R) * t),
			(byte)(a.G + (b.G - a.G) * t),
			(byte)(a.B + (b.B - a.B) * t),
			(byte)(a.A + (b.A - a.A) * t));
	}
}
