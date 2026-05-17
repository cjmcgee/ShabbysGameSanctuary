using System.Buffers.Binary;
using System.IO.Compression;

namespace ChildhoodAdventure.RetroSystems.Assets;

/// <summary>
/// Minimal PNG encoder used by the asset seeder. Writes 8-bit RGBA images.
/// Reading PNGs is handled natively by MonoGame's <see cref="Texture2D.FromStream"/>;
/// this writer only exists so the seeder can produce on-disk asset files that the
/// loader (and a human editor) can later open with any standard image tool.
/// </summary>
internal static class PngWriter
{
	private static readonly byte[] Signature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

	public static byte[] Encode(int width, int height, Color[] pixels)
	{
		ArgumentNullException.ThrowIfNull(pixels);
		if (pixels.Length != width * height)
		{
			throw new ArgumentException(
				$"Pixel buffer length {pixels.Length} doesn't match {width}×{height}.",
				nameof(pixels));
		}

		using var ms = new MemoryStream();
		ms.Write(Signature);

		// IHDR
		Span<byte> ihdr = stackalloc byte[13];
		BinaryPrimitives.WriteInt32BigEndian(ihdr[0..4],	width);
		BinaryPrimitives.WriteInt32BigEndian(ihdr[4..8],	height);
		ihdr[8] = 8;	// bit depth
		ihdr[9] = 6;	// color type: truecolour + alpha
		ihdr[10] = 0;	// compression: deflate
		ihdr[11] = 0;	// filter: adaptive
		ihdr[12] = 0;	// interlace: none
		WriteChunk(ms, "IHDR", ihdr);

		// IDAT (filter byte 0 on each scanline, then RGBA pixels)
		byte[] raw = new byte[height * (1 + width * 4)];
		for (int y = 0; y < height; y++)
		{
			int rowStart = y * (1 + width * 4);
			raw[rowStart] = 0;
			for (int x = 0; x < width; x++)
			{
				var c = pixels[y * width + x];
				int p = rowStart + 1 + x * 4;
				raw[p + 0] = c.R;
				raw[p + 1] = c.G;
				raw[p + 2] = c.B;
				raw[p + 3] = c.A;
			}
		}

		byte[] compressed;
		using (var cms = new MemoryStream())
		{
			using (var z = new ZLibStream(cms, CompressionLevel.Optimal, leaveOpen: true))
			{
				z.Write(raw, 0, raw.Length);
			}
			compressed = cms.ToArray();
		}
		WriteChunk(ms, "IDAT", compressed);

		WriteChunk(ms, "IEND", []);

		return ms.ToArray();
	}

	private static void WriteChunk(Stream s, string type, ReadOnlySpan<byte> data)
	{
		Span<byte> lenBytes = stackalloc byte[4];
		BinaryPrimitives.WriteInt32BigEndian(lenBytes, data.Length);
		s.Write(lenBytes);

		Span<byte> typeBytes = stackalloc byte[4];
		typeBytes[0] = (byte)type[0];
		typeBytes[1] = (byte)type[1];
		typeBytes[2] = (byte)type[2];
		typeBytes[3] = (byte)type[3];
		s.Write(typeBytes);
		s.Write(data);

		uint crc = 0xFFFFFFFFu;
		crc = Crc32Update(crc, typeBytes);
		crc = Crc32Update(crc, data);
		crc ^= 0xFFFFFFFFu;

		Span<byte> crcBytes = stackalloc byte[4];
		BinaryPrimitives.WriteUInt32BigEndian(crcBytes, crc);
		s.Write(crcBytes);
	}

	private static readonly uint[] Crc32Table = BuildCrc32Table();

	private static uint[] BuildCrc32Table()
	{
		var t = new uint[256];
		for (uint i = 0; i < 256; i++)
		{
			uint c = i;
			for (int k = 0; k < 8; k++)
			{
				c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
			}
			t[i] = c;
		}
		return t;
	}

	private static uint Crc32Update(uint crc, ReadOnlySpan<byte> data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			crc = Crc32Table[(crc ^ data[i]) & 0xFF] ^ (crc >> 8);
		}
		return crc;
	}
}
