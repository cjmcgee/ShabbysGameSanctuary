using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems;

/// <summary>
/// Semantic colours used by scenes for world-content tinting (e.g. per-house
/// facade and accent colours) and dialog UI rendering. Each
/// <see cref="RetroSystem"/> maps these roles onto its own palette so scenes
/// never embed raw RGB values.
///
/// Restricted-palette systems (CGA, Apple II) may map several roles to the
/// same colour — that's the authentic constraint of the hardware.
/// </summary>
public sealed record ScenePalette(
	// ── House facade tones ──────────────────────────────────────────────────
	Color HouseBeige,		// player home — light/cream
	Color HouseYellow,		// Sam
	Color HousePink,		// Santos — magenta-pink
	Color HouseTeal,		// Chen — cyan / teal
	Color HouseGray,		// Thompson
	Color HouseBlue,		// Petrov
	Color HouseLime,		// Jake & Emma — light green
	Color HousePurple,		// Devon — violet
	Color HouseOrange,		// Johnson
	Color Door,				// near-black door / frame

	// ── Dialog UI ────────────────────────────────────────────────────────────
	// Scenes apply alpha (e.g. * 0.85f) for translucent backgrounds; the
	// colours stored here are fully opaque palette entries.
	Color UiBackground,		// dialog box background fill
	Color UiText,			// body / main text
	Color UiAccent,			// speaker name + selected choice
	Color UiChoice,			// unselected choice
	Color UiDim );			// disabled choice + advance prompt
