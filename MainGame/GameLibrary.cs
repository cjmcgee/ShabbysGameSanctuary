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
		const string CombatRom =
			"Combat - Tank-Plus (Tank) (1977) (Atari, Joe Decuir, Larry Kaplan, Steve Mayer, Larry Wagner - Sears) (CX2601 - 99801, 6-99801, 49-75101, 49-75124) ~.bin";
		// Per-OS library name. The MSBuild target builds the matching one
		// from stella/src/os/libretro and copies it next to the exe.
		string stellaCore =
			OperatingSystem.IsWindows() ?	"stella_libretro.dll" :
			OperatingSystem.IsMacOS()   ?	"stella_libretro.dylib" :
											"stella_libretro.so";

		string corePath =	config.ResolveCore(stellaCore);
		string romPath =	config.ResolveRom(CombatRom);

		// CoreRoot may legitimately be empty — EmulatorConfig falls back to
		// the exe directory in that case, which is where the build dropped
		// the vendored core. So we only need to check the resolved file.
		string?	unavailable =	null;
		if( !File.Exists( corePath ) )
		{
			unavailable =	$"core not found: {corePath}";
		}
		else if( string.IsNullOrEmpty( config.RomRoot ) )
		{
			unavailable =	"emulator-config.json: RomRoot not set";
		}
		else if( !File.Exists( romPath ) )
		{
			unavailable =	$"ROM not found: {romPath}";
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
