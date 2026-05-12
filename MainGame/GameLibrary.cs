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
	private static (string Name, long Size, string HashSHA256Hex)[] _neededRoms = 
		[	("Adventure", 				 4096, "b0326b45e5cab066b2d03f1e683708d82de3ffe60b2e968bf88c5494237f0a96"),
			("Asteroids",				 8192, "d1a6e808412b4535c7e54fc25414943a9418b1095b4d4969dc33290f7eaa72a4"),
			//("Asteroids (no copyright)", 8192, "5ba6f91851b2331a37a3fe6a950c9b464fd764cb035a3d25d4b4388e9b26768f"),
			("Centipede",				 8192, "6def669f54bd886f43026e7fc7f82b9acb92680bb3e3aa68da3767f8797c8cbe"),
			("Combat",					 2048, "509e845d17bb192ba7587cc977411bd43b8ee31385eeefc3fef8c16e249f4820"),
			("Combat Two",				 8192, "2f06583d9d91924fa475b5ed5ce827146e52ea5d4d05a13128c30eaa309074ca"),
			("Defender",				 4096, "e3c7ed7a073fe8f4b2711bebd624d0b249957daa0ebb93d78823fe36b235ba8c"),
			("Demon Attack",			 4096, "8d72feeb267d23cb5d58bb4511566414884c572d2b4dcba8f6893ad7426442d4"),
			//("Demon Attack (fixed)",	 4096, "a181c5368dd648973b1f270999edaa6cb7ef80eaeb442ce72b4a9232a6eee301"),
			("E.T.",					 8192, "e9539483b4f51853888cc5db075eb0155bbb2e6c016a3e6eadd6f19fc1fa0947"),
			("Frogger",					 4096, "63e0229b80f871c6fdda8ba389583451cd5cd54be07ca32560edb752280ae04b"),
			("Frogger II",				 8192, "fd4c43d5246fa17a162c0a09d8be88331f8d408e8e86651609d126e5f9ca385e"),
			("Kaboom!",					 2048, "21363eb5ea5ce8fde85942948212f0ebca62ddd75d9d92238f989ddf8e8de308"),
			("Missile Command",			 4096, "46c444b91854e3652cc00facf52101d3c9fa3528eaba7674cc790acc3ad59086"),
			("Ms. Pac-Man",				 8192, "dde0b43c5dee7ce1a35ac8e2f625b32a16118512d9be4064115053b972280b14"),
			("Pac-Man",					 4096, "58e781b472e0583b851b7e56cbfe87bd8294a291e6d39d49d7c999107ed5b918"),
			("Pitfall II",				10495, "6755784f8f526e33b155e2c3e99e5da14ada6c53e3f203b21a8bb140118d8251"),
			("Pitfall",					 4096, "c56c99d9e00136a015a485851e0e925c6327ea2b20aa7d3daecfbd7f9afcfdf0"),
			("River Raid",				 4096, "4c6842b8af64fc75e599b773821b3e6bee8e5028f5d63414821b0d63f0b2dfe9"),
			("River Raid II",			16384, "6715a2a0cb2db46665b139dbd5c5224238a3df143465334ad873f3ef9c5d4ee5"),
			("Space Invaders",			 4096, "7224b17462b992d67f4e06a3c85f269c9822b06df6015bf038b55f384ced0301"),
			("Yars' Revenge",			 4096, "ff777c8d4ea06569156fcb2f5ba0e00eebaf2c2c509e5829109e5aefed199c66") ];
		
	/// <summary>
	/// The 22 ROMs the game knows how to launch, exposed as a typed list
	/// so <see cref="RomResolver"/> and the ROM-manager UI can iterate
	/// without having to know about the raw <see cref="_neededRoms"/>
	/// tuple shape. Lazily built once at type init.
	/// </summary>
	public static IReadOnlyList<RomSpec> RequiredRoms { get; } =
		Array.ConvertAll(_neededRoms, r =>	new RomSpec(r.Name, r.Size, r.HashSHA256Hex));

	public IReadOnlyList<GameEntry>	Games { get; }

	/// <summary>
	/// Per-spec resolution result from the last build of this library —
	/// the ROM-manager scene reads this to draw status badges and
	/// resolved paths next to each title.
	/// </summary>
	public IReadOnlyList<RomMatch>	RomMatches { get; }

	public GameLibrary( EmulatorConfig config )
	{
		var games =	new List<GameEntry>();

		// Native — always available.
		games.Add( new GameEntry(
			id:				"battleshoot",
			name:			"Battleshoot",
			isEmulated:		false,
			factory:		()	=>	new BattleshootGame() ) );

		// Per-OS libretro core name. The MSBuild target builds the matching
		// one from stella/src/os/libretro and copies it next to the exe.
		string stellaCore =
			OperatingSystem.IsWindows() ? "stella_libretro.dll" :
			OperatingSystem.IsMacOS()   ? "stella_libretro.dylib" :
										  "stella_libretro.so";

		string corePath =	config.ResolveCore( stellaCore );
		bool coreOk =		File.Exists( corePath );
		bool romRootOk =	!string.IsNullOrEmpty( config.EffectiveRomRoot );

		// Single-pass scan + match for every spec the game needs. The
		// resolver hashes only files whose size matches a spec, caches
		// each hash, and rejects name-prefix matches that would belong
		// to a longer spec name.
		RomMatches =	RomResolver.Resolve(
			RequiredRoms,
			config.EffectiveRomRoot,
			config.RomOverrides);

		// Build a GameEntry per spec, marking unavailable whenever
		// either the core or the ROM is missing. The user discovers
		// per-ROM mismatches by opening the ROM Manager (press R on
		// the game-select screen).
		foreach (var match in RomMatches)
		{
			string?	unavailable =	null;
			if (!coreOk)
				unavailable =	$"Emulator core missing — press C to set the core folder. Looked for: {corePath}";
			else if (!romRootOk)
				unavailable =	"ROM folder not set — press C to choose a folder containing your ROMs.";
			else if (match.Resolution == RomResolution.NotFound)
				unavailable =	$"ROM '{match.Spec.Name}' not found in {config.EffectiveRomRoot}. Press R to open the ROM Manager.";

			// Lambda captures `match` by reference into the closure —
			// capture the resolved path locally so the factory keeps
			// the right value even after the iteration variable rolls
			// over (it doesn't here because match is loop-scoped, but
			// a local makes the intent explicit and survives refactors).
			string title =	match.Spec.Name;
			string? path =	match.ResolvedPath;
			games.Add( new GameEntry(
				id:					title.ToLowerInvariant().Replace( ' ', '_' ),
				name:				title,
				isEmulated:			true,
				factory:			() =>	new LibretroMiniGame( corePath, path ?? "",
										title:				title,
										systemDirectory:	config.SystemRoot ),
				unavailableReason:	unavailable ) );
		}

		Games =	games;
	}
}
