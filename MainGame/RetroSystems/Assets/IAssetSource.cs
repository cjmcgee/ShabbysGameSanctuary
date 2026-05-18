using System.IO.Compression;

namespace ChildhoodAdventure.RetroSystems.Assets;

/// <summary>
/// Where the asset loader reads JSON manifests and image bytes from. The
/// only current implementation is <see cref="ZipAssetSource"/> — assets ship
/// inside a renamed zip so the raw PNG/JSON aren't visible on disk. The
/// abstraction is here mainly to keep <see cref="AssetLoader"/> independent
/// of the storage strategy (so a future loose-directory variant for dev
/// iteration, or a stream-backed embedded resource, drops in cleanly).
/// </summary>
internal interface IAssetSource
{
	/// <summary>Read a UTF-8 text file (typically a JSON manifest).</summary>
	string ReadText(string name);

	/// <summary>Open a binary stream (typically a PNG sheet).</summary>
	Stream OpenStream(string name);
}

/// <summary>
/// Backs an asset source with a renamed ZIP archive (entries are looked up by
/// name, flat — matching how the bundler stores them). The archive stays open
/// for the lifetime of this source so per-entry reads don't repeatedly open
/// the file; <c>Dispose</c> closes it.
/// </summary>
internal sealed class ZipAssetSource : IAssetSource, IDisposable
{
	private readonly ZipArchive _archive;
	private readonly FileStream _file;

	public ZipAssetSource(string archivePath)
	{
		_file = File.OpenRead(archivePath);
		_archive = new ZipArchive(_file, ZipArchiveMode.Read, leaveOpen: false);
	}

	public string ReadText(string name)
	{
		using var s = OpenStream(name);
		using var r = new StreamReader(s);
		return r.ReadToEnd();
	}

	public Stream OpenStream(string name)
	{
		var e = _archive.GetEntry(name)
			?? throw new FileNotFoundException($"Asset '{name}' not found in archive.", name);
		// Copy into a MemoryStream so callers can seek (Texture2D.FromStream
		// rewinds during PNG decode) — ZipArchive entry streams are forward-only.
		using var src = e.Open();
		var ms = new MemoryStream((int)e.Length);
		src.CopyTo(ms);
		ms.Position = 0;
		return ms;
	}

	public void Dispose()
	{
		_archive.Dispose();
		_file.Dispose();
	}
}
