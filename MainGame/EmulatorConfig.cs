using System.IO;
using System.Text.Json;

namespace ChildhoodAdventure;

/// <summary>
/// Runtime configuration for the in-game console — where to find libretro
/// cores and ROM files. The defaults assume nothing exists; the user is
/// expected to point these at their local setup (mounting an SMB share,
/// pointing at a RetroArch install, etc).
///
/// Loaded from <c>emulator-config.json</c> next to the executable. Missing
/// file → defaults; malformed file → defaults + warning to stderr.
/// </summary>
public sealed class EmulatorConfig
{
	/// <summary>Filesystem path that ROM filenames are resolved against.</summary>
	public string RomRoot { get; set; } =	"";

	/// <summary>Filesystem path that core filenames are resolved against.</summary>
	public string CoreRoot { get; set; } =	"";

	/// <summary>
	/// Optional system files directory passed to libretro cores via the
	/// <c>RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY</c> hook. Some cores load
	/// BIOS files from here (Stella does not need one for stock cartridges).
	/// </summary>
	public string SystemRoot { get; set; } =	"";

	private const string FileName =	"emulator-config.json";

	public static EmulatorConfig LoadOrDefault()
	{
		var baseDir =	AppContext.BaseDirectory;
		var path =	Path.Combine(baseDir, FileName);
		if (!File.Exists(path))
		{
			// Also try the project directory (handy when running via `dotnet run`
			// where BaseDirectory is bin/Debug/...).
			var alt =	Path.Combine(baseDir, "..", "..", "..", FileName);
			if (File.Exists(alt))	path =	Path.GetFullPath(alt);
			else return new EmulatorConfig();
		}

		try
		{
			var json =	File.ReadAllText(path);
			return JsonSerializer.Deserialize<EmulatorConfig>(json,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
				?? new EmulatorConfig();
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"[EmulatorConfig] Failed to read {path}: {ex.Message}");
			return new EmulatorConfig();
		}
	}

	/// <summary>
	/// Resolve <paramref name="romFile"/> against <see cref="RomRoot"/>.
	/// If the input is already absolute, it's returned as-is.
	/// </summary>
	public string ResolveRom(string romFile)	=>
		Path.IsPathRooted(romFile) || string.IsNullOrEmpty(RomRoot)
			? romFile
			: Path.Combine(RomRoot, romFile);

	/// <summary>Resolve <paramref name="coreFile"/> against <see cref="CoreRoot"/>.</summary>
	public string ResolveCore(string coreFile)	=>
		Path.IsPathRooted(coreFile) || string.IsNullOrEmpty(CoreRoot)
			? coreFile
			: Path.Combine(CoreRoot, coreFile);
}
