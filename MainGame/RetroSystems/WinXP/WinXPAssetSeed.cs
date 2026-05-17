using ChildhoodAdventure.RetroSystems.Assets;

namespace ChildhoodAdventure.RetroSystems.WinXP;

/// <summary>
/// First-run generator for the WinXP system's PNG + JSON asset files.
///
/// The repo doesn't ship hand-painted WinXP art yet, so this class encodes a
/// procedural baseline (modern-pixel-art look at 32×32 tiles / 16×32 total
/// sprites) and writes it to the asset directory the loader is going to read.
/// Once a file is present on disk the seeder leaves it alone — the intent is
/// that designers can later replace any of these PNGs with hand-painted art
/// without touching code.
///
/// The seeder is only invoked when the loader can't find a needed file.
/// Production builds that ship checked-in WinXP/Assets/*.{png,json} skip
/// the generator entirely.
/// </summary>
internal static class WinXPAssetSeed
{
	public const int TileSize = 32;
	public const int CharWidth = 16;
	public const int HeadH = 8;
	public const int BodyH = 12;
	public const int LegsH = 12;
	public const int WalkFrames = 4;

	/// <summary>
	/// Names of every asset file that must be present for the WinXP system to
	/// initialize. Used by callers to check whether seeding is needed.
	/// </summary>
	public static readonly string[] RequiredFiles =
	[
		"WinXPTiles.json",      "WinXPTiles.png",
		"WinXPHeads.json",      "WinXPHeads.png",
		"WinXPBodies.json",     "WinXPBodies.png",
		"WinXPLegs.json",       "WinXPLegs.png",
	];

	public static bool AllPresent(string dir) =>
		RequiredFiles.All(f => File.Exists(Path.Combine(dir, f)));

	public static void EnsureSeeded(string dir)
	{
		Directory.CreateDirectory(dir);
		if (AllPresent(dir)) return;
		SeedTiles(dir);
		SeedHeads(dir);
		SeedBodies(dir);
		SeedLegs(dir);
	}

	// ── Tile sheet ──────────────────────────────────────────────────────────
	// Layout: 6 columns × 3 rows of 32×32 cells = 192×96 PNG.

	private static readonly (string Name, Action<Color[], int, int, int> Paint)[] TilePainters =
	[
		("WoodFloor",     WinXPTilePainters.WoodFloor),
		("Carpet",        WinXPTilePainters.Carpet),
		("KitchenTile",   WinXPTilePainters.KitchenTile),
		("Wall",          WinXPTilePainters.Wall),
		("Door",          WinXPTilePainters.Door),
		("Window",        WinXPTilePainters.Window),
		("Furniture",     WinXPTilePainters.Furniture),
		("Counter",       WinXPTilePainters.Counter),
		("Bookshelf",     WinXPTilePainters.Bookshelf),
		("Plant",         WinXPTilePainters.Plant),
		("Grass",         WinXPTilePainters.Grass),
		("Grass2",        WinXPTilePainters.Grass2),
		("Road",          WinXPTilePainters.Road),
		("Sidewalk",      WinXPTilePainters.Sidewalk),
		("HouseExterior", WinXPTilePainters.HouseExterior),
		("HouseRoof",     WinXPTilePainters.HouseRoof),
		("Bush",          WinXPTilePainters.Bush),
		("Accent",        WinXPTilePainters.Accent),
	];

	private static void SeedTiles(string dir)
	{
		const int cols = 6;
		const int rows = 3;
		int sheetW = cols * TileSize;
		int sheetH = rows * TileSize;
		var buf = new Color[sheetW * sheetH];

		var manifest = new TileSheetManifest
		{
			Sheet = "WinXPTiles.png",
			TileSize = TileSize,
		};

		for (int i = 0; i < TilePainters.Length; i++)
		{
			int col = i % cols;
			int row = i / cols;
			int x = col * TileSize;
			int y = row * TileSize;
			TilePainters[i].Paint(buf, sheetW, x, y);
			manifest.Tiles[TilePainters[i].Name] = new TileCell { Col = col, Row = row };
		}

		File.WriteAllBytes(Path.Combine(dir, "WinXPTiles.png"),
			PngWriter.Encode(sheetW, sheetH, buf));
		WriteJson(Path.Combine(dir, "WinXPTiles.json"), manifest);
	}

	// ── Sprite sheets ───────────────────────────────────────────────────────
	// Each sheet packs all 3 variants × 3 facings (× walk frames for legs).
	// The painter writes semantic-color markers per pixel; the loader converts
	// those to indices that <see cref="RetroSystem.BuildCharacterSprite"/>
	// will resolve through the active HeadPalette/BodyPalette/LegsPalette.

	private static void SeedHeads(string dir)
	{
		const int variants = 3;
		const int facings = 3;	// front, back, side
		int sheetW = facings * CharWidth;
		int sheetH = variants * HeadH;
		var buf = new Color[sheetW * sheetH];
		ClearTransparent(buf);

		for (int v = 0; v < variants; v++)
		{
			int yBase = v * HeadH;
			WinXPSpritePainters.PaintHead(v, Facing.Front, buf, sheetW, 0 * CharWidth,        yBase);
			WinXPSpritePainters.PaintHead(v, Facing.Back,  buf, sheetW, 1 * CharWidth,        yBase);
			WinXPSpritePainters.PaintHead(v, Facing.Side,  buf, sheetW, 2 * CharWidth,        yBase);
		}

		var manifest = new SpriteSheetManifest
		{
			Sheet = "WinXPHeads.png",
			PartWidth = CharWidth,
			PartHeight = HeadH,
		};
		for (int v = 0; v < variants; v++)
		{
			manifest.Variants.Add(new SpriteVariant
			{
				Front = new SpritePartLocation { X = 0 * CharWidth, Y = v * HeadH, Frames = 1 },
				Back  = new SpritePartLocation { X = 1 * CharWidth, Y = v * HeadH, Frames = 1 },
				Side  = new SpritePartLocation { X = 2 * CharWidth, Y = v * HeadH, Frames = 1 },
			});
		}

		File.WriteAllBytes(Path.Combine(dir, "WinXPHeads.png"),
			PngWriter.Encode(sheetW, sheetH, buf));
		WriteJson(Path.Combine(dir, "WinXPHeads.json"), manifest);
	}

	private static void SeedBodies(string dir)
	{
		const int variants = 3;
		const int facings = 3;
		int sheetW = facings * CharWidth;
		int sheetH = variants * BodyH;
		var buf = new Color[sheetW * sheetH];
		ClearTransparent(buf);

		for (int v = 0; v < variants; v++)
		{
			int yBase = v * BodyH;
			WinXPSpritePainters.PaintBody(v, Facing.Front, buf, sheetW, 0 * CharWidth, yBase);
			WinXPSpritePainters.PaintBody(v, Facing.Back,  buf, sheetW, 1 * CharWidth, yBase);
			WinXPSpritePainters.PaintBody(v, Facing.Side,  buf, sheetW, 2 * CharWidth, yBase);
		}

		var manifest = new SpriteSheetManifest
		{
			Sheet = "WinXPBodies.png",
			PartWidth = CharWidth,
			PartHeight = BodyH,
		};
		for (int v = 0; v < variants; v++)
		{
			manifest.Variants.Add(new SpriteVariant
			{
				Front = new SpritePartLocation { X = 0 * CharWidth, Y = v * BodyH, Frames = 1 },
				Back  = new SpritePartLocation { X = 1 * CharWidth, Y = v * BodyH, Frames = 1 },
				Side  = new SpritePartLocation { X = 2 * CharWidth, Y = v * BodyH, Frames = 1 },
			});
		}

		File.WriteAllBytes(Path.Combine(dir, "WinXPBodies.png"),
			PngWriter.Encode(sheetW, sheetH, buf));
		WriteJson(Path.Combine(dir, "WinXPBodies.json"), manifest);
	}

	private static void SeedLegs(string dir)
	{
		const int variants = 3;
		// Front strip (4 walk frames) followed by side strip (4 walk frames),
		// laid out horizontally for each variant row.
		int frontW = WalkFrames * CharWidth;
		int sideW  = WalkFrames * CharWidth;
		int sheetW = frontW + sideW;
		int sheetH = variants * LegsH;
		var buf = new Color[sheetW * sheetH];
		ClearTransparent(buf);

		for (int v = 0; v < variants; v++)
		{
			int yBase = v * LegsH;
			for (int f = 0; f < WalkFrames; f++)
			{
				WinXPSpritePainters.PaintLegs(v, Facing.Front, f, buf, sheetW, f * CharWidth, yBase);
				WinXPSpritePainters.PaintLegs(v, Facing.Side,  f, buf, sheetW, frontW + f * CharWidth, yBase);
			}
		}

		var manifest = new SpriteSheetManifest
		{
			Sheet = "WinXPLegs.png",
			PartWidth = CharWidth,
			PartHeight = LegsH,
		};
		for (int v = 0; v < variants; v++)
		{
			manifest.Variants.Add(new SpriteVariant
			{
				Front = new SpritePartLocation { X = 0,      Y = v * LegsH, Frames = WalkFrames },
				Side  = new SpritePartLocation { X = frontW, Y = v * LegsH, Frames = WalkFrames },
				// Back facing reuses Front via the RetroSystem base default.
			});
		}

		File.WriteAllBytes(Path.Combine(dir, "WinXPLegs.png"),
			PngWriter.Encode(sheetW, sheetH, buf));
		WriteJson(Path.Combine(dir, "WinXPLegs.json"), manifest);
	}

	private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };
	private static void WriteJson<T>(string path, T value)
	{
		File.WriteAllText(path, JsonSerializer.Serialize(value, JsonOpts));
	}

	private static void ClearTransparent(Color[] buf)
	{
		var t = new Color(0, 0, 0, 0);
		for (int i = 0; i < buf.Length; i++) buf[i] = t;
	}
}

internal enum Facing { Front, Back, Side }
