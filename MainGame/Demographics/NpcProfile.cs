namespace ChildhoodAdventure.Demographics;

/// <summary>
/// Per-NPC demographic profile: gender, age, and adult (fully-grown) height.
/// Drives sprite scaling so children render smaller than adults and an
/// adult woman is a hair shorter than an adult man.
///
/// AdultHeightInches defaults to a deterministic sample from the
/// gender-appropriate distribution in <see cref="AdultHeightSampler"/>,
/// seeded by the NPC's id — so the same authored NPC always has the same
/// height across runs without hand-picking each one.
/// </summary>
public class NpcProfile
{
	public string Id { get; }
	public Gender Gender { get; }
	public float AgeYears { get; }
	public float AdultHeightInches { get; }

	public NpcProfile(string id, Gender gender, float ageYears, float? adultHeightInches = null)
	{
		Id =	id;
		Gender =	gender;
		AgeYears =	ageYears;
		AdultHeightInches =	adultHeightInches ?? AdultHeightSampler.SampleFor(id, gender);
	}

	/// <summary>True when the NPC has reached adult height (age &gt;= 18).</summary>
	public bool IsAdult =>	AgeYears >= 18f;

	/// <summary>Current height in inches at <see cref="AgeYears"/>.</summary>
	public float CurrentHeightInches =>
		GrowthCurve.HeightInchesAt(AgeYears, Gender, AdultHeightInches);

	/// <summary>
	/// Sprite scale factor relative to <see cref="AdultHeightSampler.ReferenceHeightInches"/>.
	/// An average adult man renders at 1.0; an average adult woman at ~0.92;
	/// a 6-year-old at ~0.65–0.67.
	/// </summary>
	public float SpriteScale =>
		CurrentHeightInches / AdultHeightSampler.ReferenceHeightInches;

	public override string ToString() =>
		$"{Id}: {Gender}, age {AgeYears:0.#}, current {CurrentHeightInches:0.#}\" / adult {AdultHeightInches:0.#}\"";
}
