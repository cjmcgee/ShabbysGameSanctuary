namespace ChildhoodAdventure.Demographics;

/// <summary>
/// Childhood growth curve — percent of adult height by age, per gender.
/// Data is the median-of-range from NPCSIZE.md "Kids estimated Percentage
/// of Adult Height (Ages 0–18)". Values between table entries are linearly
/// interpolated.
/// </summary>
public static class GrowthCurve
{
	private readonly record struct Row(float Age, float GirlPct, float BoyPct);

	// Median-of-range from NPCSIZE.md. Where the source gives a single value (e.g. "~45%"),
	// that value is used directly; where it gives a range ("~30–31%"), the midpoint is used.
	private static readonly Row[]	Table =
	{
		new( 0f,    0.305f,	0.285f),	// Birth
		new( 1f,    0.45f,	0.42f),
		new( 1.5f,  0.50f,	0.46f),
		new( 2f,    0.53f,	0.50f),
		new( 3f,    0.58f,	0.55f),
		new( 4f,    0.63f,	0.60f),
		new( 5f,    0.66f,	0.63f),
		new( 6f,    0.70f,	0.67f),
		new( 7f,    0.74f,	0.71f),
		new( 8f,    0.785f,	0.765f),
		new( 9f,    0.82f,	0.80f),
		new(10f,    0.85f,	0.83f),
		new(11f,    0.89f,	0.86f),
		new(12f,    0.93f,	0.89f),
		new(13f,    0.97f,	0.92f),
		new(14f,    0.985f,	0.95f),
		new(15f,    0.99f,	0.975f),
		new(16f,    1.00f,	0.985f),
		new(17f,    1.00f,	0.99f),
		new(18f,    1.00f,	1.00f),
	};

	/// <summary>
	/// Fraction of adult height at the given age (linearly interpolated
	/// between the table's entries). Ages above 18 are clamped to 1.0.
	/// </summary>
	public static float PercentOfAdultHeight(float ageYears, Gender gender)
	{
		if (ageYears <= Table[0].Age)	return PickPercent(Table[0], gender);
		if (ageYears >= Table[^1].Age)	return 1f;

		for (int i = 1; i < Table.Length; i++)
		{
			if (Table[i].Age >= ageYears)
			{
				var lo =	Table[i - 1];
				var hi =	Table[i];
				float t =	(ageYears - lo.Age)	/ (hi.Age - lo.Age);
				return Lerp(PickPercent(lo, gender),	PickPercent(hi, gender),	t);
			}
		}
		return 1f;
	}

	/// <summary>Current height in inches given age, gender, and the NPC's adult height.</summary>
	public static float HeightInchesAt(float ageYears, Gender gender, float adultHeightInches)
		=>	adultHeightInches * PercentOfAdultHeight(ageYears, gender);

	private static float PickPercent(Row row, Gender g)
		=>	g == Gender.Male ? row.BoyPct :	row.GirlPct;

	private static float Lerp(float a, float b, float t)	=>	a + (b - a)	* t;
}
