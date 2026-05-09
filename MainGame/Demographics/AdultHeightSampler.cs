namespace ChildhoodAdventure.Demographics;

/// <summary>
/// Samples adult heights from the gender-specific normal distributions in
/// NPCSIZE.md:
///   • Men:   mean 69.15 in (5'9"), σ ≈ 2.9 in
///   • Women: mean 63.55 in (5'3.5"), σ ≈ 2.5 in
/// </summary>
public static class AdultHeightSampler
{
	public const float MaleMean	=	69.15f;	// inches
	public const float MaleStdDev	=	2.9f;
	public const float FemaleMean	=	63.55f;
	public const float FemaleStdDev	=	2.5f;

	/// <summary>
	/// Reference adult-male median height. Used as the "1.0 sprite scale"
	/// reference: an average adult man renders at scale 1.0; everything else
	/// scales relative to him.
	/// </summary>
	public const float ReferenceHeightInches =	MaleMean;

	/// <summary>
	/// Sample one adult height from the appropriate normal distribution
	/// using Box–Muller. Result is in inches.
	/// </summary>
	public static float Sample(Gender gender, Random rng)
	{
		// Box–Muller transform: two uniforms → one standard normal.
		double u1 =	Math.Max(rng.NextDouble(),	1e-9);	// avoid log(0)
		double u2 =	rng.NextDouble();
		double z =	Math.Sqrt(-2.0 * Math.Log(u1))	* Math.Cos(2.0 * Math.PI * u2);

		float mean =	gender == Gender.Male ? MaleMean :	FemaleMean;
		float sd =	gender == Gender.Male ? MaleStdDev :	FemaleStdDev;
		return (float)(mean + z * sd);
	}

	/// <summary>
	/// Deterministic per-NPC sampling: the same id always produces the same
	/// adult height. Use this so authored NPCs have stable proportions
	/// across runs without hand-picking each height.
	/// </summary>
	public static float SampleFor(string npcId, Gender gender)
	{
		var rng =	new Random(StableHash(npcId));
		return Sample(gender, rng);
	}

	// Stable string hash so seeding is reproducible across .NET versions
	// (string.GetHashCode is randomized per-process by default).
	private static int StableHash(string s)
	{
		unchecked
		{
			int hash =	23;
			foreach (char c in s)
				hash =	hash * 31 + c;
			return hash;
		}
	}
}
