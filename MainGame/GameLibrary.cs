using System.IO;
using Battleshoot;
using TileEngine.MiniGames;
using TileEngine.MiniGames.Libretro;

namespace ChildhoodAdventure;

/// <summary>
/// One game the in-world Atari console can run. Either a native
/// implementation (a class that implements <see cref="IEmbeddedMiniGame"/>
/// directly, like <see cref="BattleshootGame"/>) or an emulated cartridge
/// loaded by a libretro core.
/// </summary>
public sealed class GameEntry
{
	public string	Id { get; }
	public string	Name { get; }
	public bool		IsEmulated { get; }
	public string?	UnavailableReason { get; }

	private readonly Func<IEmbeddedMiniGame> _factory;

	public bool IsAvailable => UnavailableReason == null;
	public IEmbeddedMiniGame Create() => _factory();

	public GameEntry(
		string id,
		string name,
		bool isEmulated,
		Func<IEmbeddedMiniGame> factory,
		string? unavailableReason = null )
	{
		Id			= id;
		Name		= name;
		IsEmulated	= isEmulated;
		_factory	= factory;
		UnavailableReason =	unavailableReason;
	}
}

/// <summary>
/// Catalog of games the Atari console offers. Built from the user's
/// <see cref="EmulatorConfig"/>; emulated entries are flagged unavailable
/// at construction time if the core or ROM file can't be found, so the
/// menu can grey them out instead of crashing on launch.
/// </summary>
public sealed class GameLibrary
{
	public IReadOnlyList<GameEntry>	Games { get; }

	public GameLibrary( EmulatorConfig config )
	{
		var games =	new List<GameEntry>();

		// Native — always available.
		games.Add(new GameEntry(
			id:				"battleshoot",
			name:			"Battleshoot",
			isEmulated:		false,
			factory:		()	=>	new BattleshootGame()));

		// Emulated — Combat for Atari 2600 (Stella libretro core).
		// The user's reference ROM filename is long; keep it as a constant so
		// it's obvious which dump we expect.
		const string CombatRom =	"Combat - Tank-Plus.bin";
		// Per-OS library name. The MSBuild target builds the matching one
		// from stella/src/os/libretro and copies it next to the exe.
		string stellaCore =
			OperatingSystem.IsWindows() ?	"stella_libretro.dll" :
			OperatingSystem.IsMacOS()   ?	"stella_libretro.dylib" :
											"stella_libretro.so";

		string corePath =	config.ResolveCore(stellaCore);
		// Atari 2600 cartridges top out at 32 KB (large bank-switched carts).
		// Capping the recursive ROM search avoids picking up same-name files
		// from other systems (e.g. a 512 KB homebrew "Combat" elsewhere).
		const long Atari2600MaxRomBytes =	32 * 1024;
		string romPath =	config.ResolveRom(CombatRom, maxBytes:	Atari2600MaxRomBytes);

		// CoreRoot may legitimately be empty — EmulatorConfig falls back to
		// the exe directory in that case, which is where the build dropped
		// the vendored core. RomRoot may also be empty: in debug builds it
		// falls back to a sibling TestROMs/ folder. In release builds an
		// empty RomRoot really does mean "user hasn't configured it".
		string?	unavailable =	null;
		if( !File.Exists( corePath ) )
		{
			unavailable =	$"Emulator core missing — press C to set the core folder. Looked for: {corePath}";
		}
		else if( string.IsNullOrEmpty( config.EffectiveRomRoot ) )
		{
			unavailable =	"ROM folder not set — press C to choose a folder containing your ROMs.";
		}
		else if( string.IsNullOrEmpty( romPath ) || !File.Exists( romPath ) )
		{
			// Either no file matched the name + size cap, or it was found
			// but vanished between resolution and now (race / unmount).
			unavailable =
				$"ROM '{CombatRom}' (≤ {Atari2600MaxRomBytes / 1024} KB) not found under {config.EffectiveRomRoot}. " +
				"Press C to pick a different folder, or check for stray same-name copies in other system folders.";
		}

		games.Add( new GameEntry(
			id:					"combat",
			name:				"Combat",
			isEmulated:			true,
			factory:			()	=>	new LibretroMiniGame(corePath, romPath,
									title:			"Combat",
									systemDirectory:	config.SystemRoot),
			unavailableReason:	unavailable ) );

		Games =	games;
	}
}
