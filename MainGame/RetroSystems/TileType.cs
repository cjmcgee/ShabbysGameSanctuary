namespace ChildhoodAdventure.RetroSystems;

/// <summary>
/// Semantic tile types shared across all retro systems.
/// Each system provides pixel art for each type; the tile GID ordering
/// in scenes stays constant regardless of active system.
/// </summary>
public enum TileType
{
	// Interior floors
	WoodFloor,
	Carpet,
	KitchenTile,

	// Structural
	Wall,
	Door,
	Window,

	// Furniture / objects
	Furniture,
	Counter,
	Bookshelf,
	Plant,

	// Outdoor
	Grass,
	Grass2,
	Road,
	Sidewalk,
	HouseExterior,		// exterior siding (uses per-house accent colour)
	HouseRoof,			// dark shingled roof
	Bush,				// green decorative shrub on grass

	// Per-house tint (accent color supplied at build time by scene)
	Accent,
}
