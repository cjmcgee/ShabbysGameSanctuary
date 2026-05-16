namespace ChildhoodAdventure.Demographics;

/// <summary>
/// Demographic profiles for every named character in the game. Indexed by
/// the same string id used to register NPCs in
/// <see cref="TileEngine.Npc.NpcRegistry"/>, so a SpawnNpc call site looks
/// the profile up by the NPC's name.
///
/// Adult heights are NOT listed here — they're derived deterministically
/// from each id via <see cref="AdultHeightSampler"/>. To override an
/// individual NPC's adult height (e.g. the very tall basketball coach),
/// pass the optional <c>adultHeightInches</c> argument to its
/// <see cref="NpcProfile"/> constructor.
/// </summary>
internal static class NpcProfiles
{
	private static readonly Dictionary<string,	NpcProfile>	Table =	new()
	{
		// ── Player & immediate family ────────────────────────────────────────
		[ "Player" ] =	new("Player", Gender.Male,	ageYears:  9f),
		[ "Dad"    ] =	new("Dad",    Gender.Male,	ageYears: 38f),
		[ "Mom"    ] =	new("Mom",    Gender.Female,	ageYears: 36f),
		[ "Jamie"  ] =	new("Jamie",  Gender.Female,	ageYears:  6f),

		// ── Neighbourhood kids (open-air) ────────────────────────────────────
		[ "Sam"    ] =	new("Sam",    Gender.Male,	ageYears: 10f),
		[ "Lucia"  ] =	new("Lucia",  Gender.Female,	ageYears:  9f),
		[ "Nadia"  ] =	new("Nadia",  Gender.Female,	ageYears:  8f),

		// ── Chen house ───────────────────────────────────────────────────────
		[ "Mr. Chen"  ] =	new("Mr. Chen",  Gender.Male,	ageYears: 50f),
		[ "Mrs. Chen" ] =	new("Mrs. Chen", Gender.Female,	ageYears: 48f),

		// ── Devon's house (adult young man) ──────────────────────────────────
		[ "Devon" ] =	new("Devon", Gender.Male,	ageYears: 12f),

		// ── Jake & Emma (siblings, kids) ─────────────────────────────────────
		[ "Jake" ] =	new("Jake", Gender.Male,	ageYears: 10f),
		[ "Emma" ] =	new("Emma", Gender.Female,	ageYears:  8f),

		// ── Thompson house ───────────────────────────────────────────────────
		[ "Mr. Thompson" ] =	new("Mr. Thompson", Gender.Male,	ageYears: 55f),

		// ── Santos house ─────────────────────────────────────────────────────
		[ "Maria" ] =	new("Maria", Gender.Female,	ageYears: 38f),

		// ── Petrov house ─────────────────────────────────────────────────────
		[ "Mr. Petrov"  ] =	new("Mr. Petrov",  Gender.Male,	ageYears: 60f),
		[ "Mrs. Petrov" ] =	new("Mrs. Petrov", Gender.Female,	ageYears: 58f),

		// ── Sam's house ──────────────────────────────────────────────────────
		[ "Linda" ] =	new("Linda", Gender.Female,	ageYears: 42f),

		// ── Johnson house ────────────────────────────────────────────────────
		[ "DeShonda" ] =	new("DeShonda", Gender.Female,	ageYears: 36f),
		[ "Destiny"  ] =	new("Destiny",  Gender.Female,	ageYears:  9f),
		[ "Tyler"    ] =	new("Tyler",    Gender.Male,	ageYears: 11f),
	};

	/// <summary>Look up a profile by NPC id; null if no profile is authored.</summary>
	public static NpcProfile?	Get(string id)	=>
		Table.TryGetValue(id, out var p) ? p :	null;

	/// <summary>
	/// Look up a profile, falling back to a generic adult of the given gender
	/// if no profile is authored. Useful for ambient / unnamed NPCs.
	/// </summary>
	public static NpcProfile GetOrDefault(string id, Gender fallbackGender = Gender.Male, float fallbackAge = 30f)
		=>	Get(id) ?? new NpcProfile(id, fallbackGender, fallbackAge);

	/// <summary>All authored profiles, for diagnostics / debug overlays.</summary>
	public static IEnumerable<NpcProfile>	All =>	Table.Values;
}
