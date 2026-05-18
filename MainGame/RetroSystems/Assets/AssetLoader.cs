namespace ChildhoodAdventure.RetroSystems.Assets;

/// <summary>
/// Decodes JSON+PNG asset manifests via an <see cref="IAssetSource"/> and
/// produces the data shapes the rest of <see cref="RetroSystem"/> expects.
///
/// All entries within one manifest are looked up by name through the same
/// source. For the WinXP system that source is a renamed zip archive
/// (<c>data.dat</c>) packed at build time; the loader doesn't care whether
/// it's backed by a directory or an archive.
/// </summary>
internal static class AssetLoader
{
	private static readonly JsonSerializerOptions JsonOpts = new()
	{
		PropertyNameCaseInsensitive = true,
		ReadCommentHandling         = JsonCommentHandling.Skip,
		AllowTrailingCommas         = true,
	};

	/// <summary>
	/// Read a <see cref="TileSheetManifest"/> from <paramref name="source"/>,
	/// load its referenced PNG (also from <paramref name="source"/>), and
	/// return a dictionary mapping tile-name → that tile's pixel data.
	///
	/// Pixels matching <see cref="TileSentinels.AccentMarker"/> stay as-is in
	/// the returned array; the per-build accent substitution happens later in
	/// <see cref="RetroSystem.BuildTileset"/>.
	/// </summary>
	public static Dictionary<string, Color[][]> LoadTileSheet(
		GraphicsDevice gd, IAssetSource source, string manifestName)
	{
		var manifest = JsonSerializer.Deserialize<TileSheetManifest>(
			source.ReadText(manifestName), JsonOpts)
			?? throw new InvalidDataException($"Empty manifest: {manifestName}");

		int ts = manifest.TileSize;
		if (ts <= 0)
		{
			throw new InvalidDataException(
				$"{manifestName}: TileSize must be > 0 (got {ts}).");
		}

		var sheet = LoadPng(gd, source, manifest.Sheet);
		try
		{
			var all = new Color[sheet.Width * sheet.Height];
			sheet.GetData(all);

			var result = new Dictionary<string, Color[][]>(manifest.Tiles.Count);
			foreach (var (name, cell) in manifest.Tiles)
			{
				int x = cell.X != 0 ? cell.X : cell.Col * ts;
				int y = cell.Y != 0 ? cell.Y : cell.Row * ts;
				result[name] = Slice(all, sheet.Width, x, y, ts, ts);
			}
			return result;
		}
		finally
		{
			sheet.Dispose();
		}
	}

	/// <summary>
	/// Read a <see cref="SpriteSheetManifest"/> from <paramref name="source"/>,
	/// load its referenced PNG, and return the per-variant parts (with
	/// optional back/side facings). Magic colors in the PNG are converted to
	/// the semantic indices documented on <see cref="SemanticPalette"/>.
	/// </summary>
	public static SpriteSheetData LoadSpriteSheet(
		GraphicsDevice gd, IAssetSource source, string manifestName)
	{
		var manifest = JsonSerializer.Deserialize<SpriteSheetManifest>(
			source.ReadText(manifestName), JsonOpts)
			?? throw new InvalidDataException($"Empty manifest: {manifestName}");

		int pw = manifest.PartWidth;
		int ph = manifest.PartHeight;
		if (pw <= 0 || ph <= 0)
		{
			throw new InvalidDataException(
				$"{manifestName}: PartWidth/PartHeight must be > 0 (got {pw}×{ph}).");
		}

		var sheet = LoadPng(gd, source, manifest.Sheet);
		try
		{
			var all = new Color[sheet.Width * sheet.Height];
			sheet.GetData(all);

			int n = manifest.Variants.Count;
			var front = new byte[n][][][];
			byte[][][][]? back = null;
			byte[][][][]? side = null;

			for (int i = 0; i < n; i++)
			{
				var v = manifest.Variants[i];
				front[i] = ExtractStrip(all, sheet.Width, v.Front, pw, ph);

				if (v.Back is not null)
				{
					back ??= new byte[n][][][];
					back[i] = ExtractStrip(all, sheet.Width, v.Back, pw, ph);
				}
				if (v.Side is not null)
				{
					side ??= new byte[n][][][];
					side[i] = ExtractStrip(all, sheet.Width, v.Side, pw, ph);
				}
			}

			return new SpriteSheetData(pw, ph, front, back, side);
		}
		finally
		{
			sheet.Dispose();
		}
	}

	private static Texture2D LoadPng(GraphicsDevice gd, IAssetSource source, string name)
	{
		using var s = source.OpenStream(name);
		return Texture2D.FromStream(gd, s);
	}

	private static Color[][] Slice(
		Color[] all, int stride, int x0, int y0, int w, int h)
	{
		var rows = new Color[h][];
		for (int y = 0; y < h; y++)
		{
			var row = new Color[w];
			for (int x = 0; x < w; x++)
			{
				row[x] = all[(y0 + y) * stride + (x0 + x)];
			}
			rows[y] = row;
		}
		return rows;
	}

	private static byte[][][] ExtractStrip(
		Color[] all, int stride, SpritePartLocation loc, int pw, int ph)
	{
		int frames = Math.Max(1, loc.Frames);
		var strip = new byte[frames][][];
		for (int f = 0; f < frames; f++)
		{
			int x0 = loc.X + f * pw;
			int y0 = loc.Y;
			var rows = new byte[ph][];
			for (int y = 0; y < ph; y++)
			{
				var row = new byte[pw];
				for (int x = 0; x < pw; x++)
				{
					row[x] = SemanticPalette.ToSemanticIndex(
						all[(y0 + y) * stride + (x0 + x)]);
				}
				rows[y] = row;
			}
			strip[f] = rows;
		}
		return strip;
	}
}

/// <summary>
/// Result of <see cref="AssetLoader.LoadSpriteSheet"/>. Back/Side may be null
/// when the manifest only covers the front facing — callers in
/// <see cref="RetroSystem"/> fall back to the front when those are absent.
/// </summary>
internal sealed record SpriteSheetData(
	int PartWidth,
	int PartHeight,
	byte[][][][] Front,
	byte[][][][]? Back,
	byte[][][][]? Side);
