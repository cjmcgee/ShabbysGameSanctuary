namespace ChildhoodAdventure.RetroSystems;

/// <summary>
/// Predefined character appearances for every named character in the game.
/// Each field selects a shape variant and a palette mapping independently for
/// head, body, and legs — mixing them freely produces a unique look per NPC.
///
/// Shape variants (all systems provide exactly these three per part):
///   Head:  0 = basic round head   1 = cap/hat   2 = long/full hair
///   Body:  0 = casual shirt       1 = collared/formal   2 = jacket/hoodie
///   Legs:  0 = pants + belt       1 = formal trousers   2 = shorts + bare skin
///
/// Palette slots:
///   Head palettes:  0 = fair/blonde   1 = fair/brown   2 = medium/black
///                   3 = dark/black    4 = medium/auburn
///   Body palettes:  0 = green   1 = blue   2 = red   3 = white/light   4 = teal
///   Legs palettes:  0 = blue jeans + brown shoes   1 = black + black
///                   2 = khaki + tan   3 = gray + dark
/// </summary>
public static class NpcAppearances
{
    // ── Player ───────────────────────────────────────────────────────────────
    public static readonly CharacterAppearance Player =
        new(HeadIndex: 0, HeadPaletteIndex: 0,   // basic, fair/blonde
            BodyIndex: 0, BodyPaletteIndex: 0,    // casual, green
            LegsIndex: 0, LegsPaletteIndex: 0);   // pants, blue jeans

    // ── Home interior ────────────────────────────────────────────────────────
    public static readonly CharacterAppearance Dad =
        new(0, 2,  1, 1,  0, 1);  // medium/black, formal blue, black

    public static readonly CharacterAppearance Mom =
        new(2, 0,  2, 3,  1, 2);  // fair/long-hair, jacket white, formal khaki

    public static readonly CharacterAppearance Jamie =
        new(0, 1,  0, 4,  2, 0);  // fair/brown kid, casual teal, shorts jeans

    // ── Neighbourhood ────────────────────────────────────────────────────────
    public static readonly CharacterAppearance Sam =
        new(1, 3,  0, 0,  0, 3);  // dark/cap, casual green, gray

    public static readonly CharacterAppearance Lucia =
        new(2, 4,  0, 2,  2, 2);  // medium/auburn long, casual red, shorts khaki

    public static readonly CharacterAppearance Nadia =
        new(0, 1,  1, 3,  0, 1);  // fair/brown, formal white, black

    // ── Chen house ───────────────────────────────────────────────────────────
    public static readonly CharacterAppearance MrChen =
        new(0, 2,  1, 1,  0, 1);  // medium/black, formal blue, black

    public static readonly CharacterAppearance MrsChen =
        new(2, 2,  2, 4,  1, 2);  // medium/long-black, jacket teal, formal khaki

    // ── Devon's house ────────────────────────────────────────────────────────
    public static readonly CharacterAppearance Devon =
        new(0, 3,  0, 0,  0, 0);  // dark/black, casual green, jeans (stressed student)

    // ── Jake & Emma ──────────────────────────────────────────────────────────
    public static readonly CharacterAppearance Emma =
        new(2, 0,  0, 2,  2, 2);  // fair/long-blonde, casual red, shorts

    public static readonly CharacterAppearance Jake =
        new(0, 1,  0, 0,  0, 3);  // fair/brown, casual green, gray

    // ── Thompson house ───────────────────────────────────────────────────────
    public static readonly CharacterAppearance MrThompson =
        new(0, 2,  1, 3,  0, 1);  // medium/black, formal white, black (muted)

    // ── Santos house ─────────────────────────────────────────────────────────
    public static readonly CharacterAppearance Maria =
        new(2, 4,  2, 2,  1, 2);  // medium/auburn long, jacket red, formal khaki

    // ── Petrov house ─────────────────────────────────────────────────────────
    public static readonly CharacterAppearance MrPetrov =
        new(0, 1,  1, 1,  0, 1);  // fair/brown, formal blue, black

    public static readonly CharacterAppearance MrsPetrov =
        new(2, 1,  2, 4,  1, 2);  // fair/long-brown, jacket teal, formal

    // ── Sam's house ──────────────────────────────────────────────────────────
    public static readonly CharacterAppearance Linda =
        new(2, 0,  2, 4,  1, 2);  // fair/long-blonde, jacket teal, formal

    // ── Johnson house ────────────────────────────────────────────────────────
    public static readonly CharacterAppearance DeShonda =
        new(2, 3,  2, 2,  1, 1);  // dark/long-black, jacket red, black formal

    public static readonly CharacterAppearance Destiny =
        new(1, 3,  0, 2,  2, 2);  // dark/cap, casual red, shorts (energetic musician)

    public static readonly CharacterAppearance Tyler =
        new(1, 3,  0, 0,  0, 1);  // dark/cap, casual green, black (moody teen)
}
