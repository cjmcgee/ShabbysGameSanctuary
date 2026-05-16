using ChildhoodAdventure.RetroSystems.Atari2600;
using ChildhoodAdventure.RetroSystems.Commodore64;
using ChildhoodAdventure.RetroSystems.AppleII;
using ChildhoodAdventure.RetroSystems.MSDOSCGA;
using ChildhoodAdventure.RetroSystems.NES;

namespace ChildhoodAdventure.RetroSystems;

/// <summary>
/// Holds the catalogue of available retro systems and tracks the active one.
/// Switch systems with <see cref="SetSystem(int)"/>; subscribe to
/// <see cref="SystemChanged"/> to reload visuals on switch.
/// </summary>
internal static class RetroSystemRegistry
{
	public static readonly RetroSystem[] All =
	[
		new Atari2600System(),
		new C64System(),
		new AppleIISystem(),
		new CGASystem(),
		new NESSystem(),
	];

	public static RetroSystem Current { get; private set; } = All[4];	// default: NES

	/// <summary>Fired after Current changes; scene code reloads visuals in response.</summary>
	public static event Action<RetroSystem>? SystemChanged;

	public static void SetSystem( int index )
	{
		Current = All[ Math.Clamp(index, 0, All.Length - 1) ];
		SystemChanged?.Invoke( Current );
	}

	public static int CurrentIndex => Array.IndexOf( All, Current );
}
