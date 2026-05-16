namespace ChildhoodAdventure.RetroSystems;

/// <summary>
/// Predefined character appearances for every named character in the game.
/// Each field selects a shape variant and a palette mapping independently for
/// head, body, and legs — mixing them freely produces a unique look per NPC.
/// </summary>
internal static class NpcAppearances
{
	// ── Player ───────────────────────────────────────────────────────────────
	public static readonly CharacterAppearance Player =
		new(HeadShape.Basic,			HeadPaletteId.FairBlonde,
			BodyShape.CasualShirt,		BodyPaletteId.Green,
			LegsShape.Pants,			LegsPaletteId.BlueJeansBrown);

	// ── Home interior ────────────────────────────────────────────────────────
	public static readonly CharacterAppearance Dad =
		new(HeadShape.Basic,			HeadPaletteId.MediumBlack,
			BodyShape.Formal,			BodyPaletteId.Blue,
			LegsShape.Pants,			LegsPaletteId.BlackBlack);

	public static readonly CharacterAppearance Mom =
		new(HeadShape.LongHair,			HeadPaletteId.FairBlonde,
			BodyShape.Jacket,			BodyPaletteId.White,
			LegsShape.FormalTrousers,	LegsPaletteId.KhakiTan);

	public static readonly CharacterAppearance Jamie =
		new(HeadShape.Basic,			HeadPaletteId.FairBrown,
			BodyShape.CasualShirt,		BodyPaletteId.Teal,
			LegsShape.Shorts,			LegsPaletteId.BlueJeansBrown);

	// ── Neighbourhood ────────────────────────────────────────────────────────
	public static readonly CharacterAppearance Sam =
		new(HeadShape.CapHat,			HeadPaletteId.DarkBlack,
			BodyShape.CasualShirt,		BodyPaletteId.Green,
			LegsShape.Pants,			LegsPaletteId.GrayDark);

	public static readonly CharacterAppearance Lucia =
		new(HeadShape.LongHair,			HeadPaletteId.MediumAuburn,
			BodyShape.CasualShirt,		BodyPaletteId.Red,
			LegsShape.Shorts,			LegsPaletteId.KhakiTan);

	public static readonly CharacterAppearance Nadia =
		new(HeadShape.Basic,			HeadPaletteId.FairBrown,
			BodyShape.Formal,			BodyPaletteId.White,
			LegsShape.Pants,			LegsPaletteId.BlackBlack);

	// ── Chen house ───────────────────────────────────────────────────────────
	public static readonly CharacterAppearance MrChen =
		new(HeadShape.Basic,			HeadPaletteId.MediumBlack,
			BodyShape.Formal,			BodyPaletteId.Blue,
			LegsShape.Pants,			LegsPaletteId.BlackBlack);

	public static readonly CharacterAppearance MrsChen =
		new(HeadShape.LongHair,			HeadPaletteId.MediumBlack,
			BodyShape.Jacket,			BodyPaletteId.Teal,
			LegsShape.FormalTrousers,	LegsPaletteId.KhakiTan);

	// ── Devon's house ────────────────────────────────────────────────────────
	public static readonly CharacterAppearance Devon =
		new(HeadShape.Basic,			HeadPaletteId.DarkBlack,
			BodyShape.CasualShirt,		BodyPaletteId.Green,
			LegsShape.Pants,			LegsPaletteId.BlueJeansBrown);

	// ── Jake & Emma ──────────────────────────────────────────────────────────
	public static readonly CharacterAppearance Emma =
		new(HeadShape.LongHair,			HeadPaletteId.FairBlonde,
			BodyShape.CasualShirt,		BodyPaletteId.Red,
			LegsShape.Shorts,			LegsPaletteId.KhakiTan);

	public static readonly CharacterAppearance Jake =
		new(HeadShape.Basic,			HeadPaletteId.FairBrown,
			BodyShape.CasualShirt,		BodyPaletteId.Green,
			LegsShape.Pants,			LegsPaletteId.GrayDark);

	// ── Thompson house ───────────────────────────────────────────────────────
	public static readonly CharacterAppearance MrThompson =
		new(HeadShape.Basic,			HeadPaletteId.MediumBlack,
			BodyShape.Formal,			BodyPaletteId.White,
			LegsShape.Pants,			LegsPaletteId.BlackBlack);

	// ── Santos house ─────────────────────────────────────────────────────────
	public static readonly CharacterAppearance Maria =
		new(HeadShape.LongHair,			HeadPaletteId.MediumAuburn,
			BodyShape.Jacket,			BodyPaletteId.Red,
			LegsShape.FormalTrousers,	LegsPaletteId.KhakiTan);

	// ── Petrov house ─────────────────────────────────────────────────────────
	public static readonly CharacterAppearance MrPetrov =
		new(HeadShape.Basic,			HeadPaletteId.FairBrown,
			BodyShape.Formal,			BodyPaletteId.Blue,
			LegsShape.Pants,			LegsPaletteId.BlackBlack);

	public static readonly CharacterAppearance MrsPetrov =
		new(HeadShape.LongHair,			HeadPaletteId.FairBrown,
			BodyShape.Jacket,			BodyPaletteId.Teal,
			LegsShape.FormalTrousers,	LegsPaletteId.KhakiTan);

	// ── Sam's house ──────────────────────────────────────────────────────────
	public static readonly CharacterAppearance Linda =
		new(HeadShape.LongHair,			HeadPaletteId.FairBlonde,
			BodyShape.Jacket,			BodyPaletteId.Teal,
			LegsShape.FormalTrousers,	LegsPaletteId.KhakiTan);

	// ── Johnson house ────────────────────────────────────────────────────────
	public static readonly CharacterAppearance DeShonda =
		new(HeadShape.LongHair,			HeadPaletteId.DarkBlack,
			BodyShape.Jacket,			BodyPaletteId.Red,
			LegsShape.FormalTrousers,	LegsPaletteId.BlackBlack);

	public static readonly CharacterAppearance Destiny =
		new(HeadShape.CapHat,			HeadPaletteId.DarkBlack,
			BodyShape.CasualShirt,		BodyPaletteId.Red,
			LegsShape.Shorts,			LegsPaletteId.KhakiTan);

	public static readonly CharacterAppearance Tyler =
		new(HeadShape.CapHat,			HeadPaletteId.DarkBlack,
			BodyShape.CasualShirt,		BodyPaletteId.Green,
			LegsShape.Pants,			LegsPaletteId.BlackBlack);
}
