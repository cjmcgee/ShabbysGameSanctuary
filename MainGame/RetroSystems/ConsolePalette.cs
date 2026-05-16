namespace ChildhoodAdventure.RetroSystems;

/// <summary>
/// Per-system colour mapping for the in-world Atari 2600 console icon.
/// Each retro system supplies its own instance via
/// <see cref="RetroSystem.GetConsolePalette"/> so the sprite participates
/// in the active visual style (Atari's wood + black, CGA's cyan/magenta,
/// Apple II's orange/black, etc).
///
/// Indices line up with <see cref="AtariConsoleArt.Layout"/>:
///   0=transparent  1=Wood  2=WoodLight  3=Body  4=Switch  5=WoodShadow  6=BodyShadow
/// </summary>
public sealed record ConsolePalette(
	Color Wood,
	Color WoodLight,
	Color WoodShadow,
	Color Body,
	Color BodyShadow,
	Color Switch);
