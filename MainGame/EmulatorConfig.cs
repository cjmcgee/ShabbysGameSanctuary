using System.IO;
using System.Text.Json;

namespace ChildhoodAdventure;

/// <summary>
/// Runtime configuration for the in-game console — where to find libretro
/// cores and ROM files.
///
/// Storage: a per-user file under the OS-standard application data dir
/// (Windows: %APPDATA%\ChildhoodAdventure\, Linux: $XDG_CONFIG_HOME or
/// ~/.config/ChildhoodAdventure/, macOS: ~/Library/Application Support/
/// ChildhoodAdventure/). This keeps the install directory read-only, so
/// Windows users don't need to elevate to edit a config file inside
/// Program Files.
///
/// Legacy fallback: if the per-user file doesn't exist, an
/// <c>emulator-config.json</c> next to the executable (or in the project
/// dir during <c>dotnet run</c>) is still read so existing installs keep
/// working through the transition.
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

	/// <summary>
	/// CoreRoot effective value used by <see cref="ResolveCore"/>. If the
	/// user left CoreRoot empty in the JSON we fall back to the executable's
	/// own directory — that's where the build target lands the vendored
	/// libretro cores (e.g. stella_libretro.so).
	/// </summary>
	public string EffectiveCoreRoot =>
		string.IsNullOrEmpty(CoreRoot) ? AppContext.BaseDirectory :	CoreRoot;

	/// <summary>
	/// RomRoot effective value used by <see cref="ResolveRom"/>. If the user
	/// left RomRoot empty, debug builds fall back to a sibling
	/// <c>TestROMs/</c> folder (handy for local dev where ROMs live next to
	/// the repo but aren't checked in). Release builds return empty so the
	/// menu surfaces the "not set" error rather than searching random dirs.
	/// </summary>
	public string EffectiveRomRoot
	{
		get
		{
			if (!string.IsNullOrEmpty(RomRoot))	return RomRoot;
#if DEBUG
			return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
				"..", "..", "..", "..", "..", "TestROMs"));
#else
			return "";
#endif
		}
	}

	private const string FileName =	"emulator-config.json";
	private const string AppDirName =	"ChildhoodAdventure";

	/// <summary>
	/// Absolute path of the per-user config file. The containing directory
	/// is created on demand by <see cref="Save"/>; this property is safe to
	/// read even when nothing has been written yet.
	/// </summary>
	public static string UserConfigPath
	{
		get
		{
			// SpecialFolder.ApplicationData maps to:
			//   Windows : %APPDATA%   (e.g. C:\Users\me\AppData\Roaming)
			//   Linux   : $XDG_CONFIG_HOME (or ~/.config)
			//   macOS   : ~/.config (since .NET picks XDG semantics here too,
			//             which is fine — it's writable without elevation).
			var baseDir =	Environment.GetFolderPath(
				Environment.SpecialFolder.ApplicationData);
			return Path.Combine(baseDir, AppDirName, FileName);
		}
	}

	public static EmulatorConfig LoadOrDefault()
	{
		// Preferred: per-user file. Always writable, no admin required.
		var userPath =	UserConfigPath;
		if (File.Exists(userPath) && TryLoad(userPath, out var fromUser))
			return fromUser;

		// Legacy: file next to the executable (pre-migration installs).
		var exeDir =	AppContext.BaseDirectory;
		var legacyExe =	Path.Combine(exeDir, FileName);
		if (File.Exists(legacyExe) && TryLoad(legacyExe, out var fromExe))
			return fromExe;

		// Dev: file in the project directory when running via `dotnet run`
		// (BaseDirectory is bin/Debug/..., the source file sits five levels up).
		var legacyProject =	Path.GetFullPath(Path.Combine(exeDir, "..", "..", "..", FileName));
		if (File.Exists(legacyProject) && TryLoad(legacyProject, out var fromProject))
			return fromProject;

		return new EmulatorConfig();
	}

	private static bool TryLoad(string path, out EmulatorConfig cfg)
	{
		try
		{
			var json =	File.ReadAllText(path);
			cfg =	JsonSerializer.Deserialize<EmulatorConfig>(json,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
				?? new EmulatorConfig();
			return true;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"[EmulatorConfig] Failed to read {path}: {ex.Message}");
			cfg =	new EmulatorConfig();
			return false;
		}
	}

	/// <summary>
	/// Persist the current values to <see cref="UserConfigPath"/>. Creates
	/// the parent directory if it doesn't exist. Returns true on success.
	/// </summary>
	public bool Save()
	{
		try
		{
			var path =	UserConfigPath;
			var dir =	Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(dir))	Directory.CreateDirectory(dir);

			var json =	JsonSerializer.Serialize(this,
				new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(path, json);
			return true;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"[EmulatorConfig] Failed to save: {ex.Message}");
			return false;
		}
	}

	/// <summary>
	/// Resolve <paramref name="romFile"/> to an absolute path. Tries:
	///   1. The exact join <c>EffectiveRomRoot/romFile</c>.
	///   2. A recursive subdirectory search for the filename underneath
	///      <c>EffectiveRomRoot</c>, optionally constrained by
	///      <paramref name="maxBytes"/> so a same-name file from a
	///      different system (e.g. a 512 KB homebrew "Combat" cart in
	///      another folder) doesn't get picked over the real one.
	/// If nothing matches, returns empty so callers can show an error.
	///
	/// The recursive search lets the user organise ROMs into any nested
	/// folder layout (e.g. <c>Atari2600/Sports/Combat.bin</c>) without
	/// updating game-side filename constants.
	/// </summary>
	/// <param name="maxBytes">
	/// Optional sanity cap. When &gt; 0, files larger than this are skipped
	/// during the recursive search. Useful when systems share game titles —
	/// pass <c>32768</c> for Atari 2600 to ignore non-2600 same-name hits.
	/// </param>
	public string ResolveRom(string romFile, long maxBytes = 0)
	{
		if (Path.IsPathRooted(romFile))	return romFile;

		string root =	EffectiveRomRoot;
		if (string.IsNullOrEmpty(root))	return romFile;

		bool SizeOk(string path)
		{
			if (maxBytes <= 0)	return true;
			try		{ return new FileInfo(path).Length <= maxBytes; }
			catch	{ return false; }
		}

		string requested =	Path.GetFileName(romFile);
		string direct =	Path.Combine(root, romFile);

		// Level 1: the literal join. Honour the size cap so an oversized
		// file at this exact path doesn't satisfy the lookup.
		if (File.Exists(direct) && SizeOk(direct))
		{
			Console.Error.WriteLine($"[ResolveRom] {romFile} -> {direct} (direct)");
			return direct;
		}

		// Level 2: recursive search by filename, also size-capped.
		if (Directory.Exists(root))
		{
			try
			{
				foreach (var hit in Directory.EnumerateFiles(
					root, requested, SearchOption.AllDirectories))
				{
					// Defend against pattern-matching quirks: require an
					// exact case-insensitive basename match.
					if (!Path.GetFileName(hit).Equals(requested,
							StringComparison.OrdinalIgnoreCase))	continue;
					if (!SizeOk(hit))	continue;
					Console.Error.WriteLine($"[ResolveRom] {romFile} -> {hit} (recursive)");
					return hit;
				}
			}
			catch (UnauthorizedAccessException)	{ /* skip unreadable dirs */ }
			catch (IOException)	{ /* network share hiccup, etc */ }
		}

		Console.Error.WriteLine(
			$"[ResolveRom] {romFile} not found under {root}" +
			(maxBytes > 0 ? $" within {maxBytes:N0} bytes." :	"."));
		return "";
	}

	/// <summary>
	/// Resolve <paramref name="coreFile"/> against <see cref="CoreRoot"/>,
	/// falling back to the exe directory when CoreRoot is unset (so the
	/// libretro core built by MSBuild and copied next to the exe is found
	/// automatically).
	/// </summary>
	public string ResolveCore(string coreFile)	=>
		Path.IsPathRooted(coreFile)
			? coreFile
			: Path.Combine(EffectiveCoreRoot, coreFile);
}
