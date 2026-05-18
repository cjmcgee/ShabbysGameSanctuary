using System.Diagnostics.CodeAnalysis;

namespace ChildhoodAdventure.RetroSystems.Assets;

// ── JSON shapes ──────────────────────────────────────────────────────────────
// These records mirror the on-disk JSON manifest files that describe each
// tile-sheet or sprite-sheet PNG (size, layout, named slots). All fields are
// nullable / defaulted so partial manifests still deserialize cleanly.
//
// CA1812 fires on each one because the analyzer can't see that
// JsonSerializer.Deserialize<T>() reflects them into existence; suppress per
// class rather than refactoring to records, since records' positional ctor
// pattern doesn't fit the optional-everything manifest shape.

/// <summary>
/// Manifest for a tile sheet PNG. The PNG is sliced into a grid of cells of
/// <see cref="TileSize"/>×<see cref="TileSize"/> pixels; each named entry in
/// <see cref="Tiles"/> picks one cell by (column, row) or by absolute (x, y)
/// pixel offset. The tile-name strings match <see cref="TileType"/> enum names.
/// </summary>
[SuppressMessage("Performance", "CA1812", Justification = "Instantiated by JsonSerializer.Deserialize")]
internal sealed class TileSheetManifest
{
	/// <summary>PNG (or other supported image) file, relative to the manifest's directory.</summary>
	public string Sheet { get; set; } = "";

	/// <summary>Cell size in pixels (square). Consumed at load time to slice the sheet.</summary>
	public int TileSize { get; set; }

	/// <summary>
	/// Map of <see cref="TileType"/> name → cell location on the sheet.
	/// Tiles not present here are unavailable from this manifest (a system may
	/// stitch several manifests together to cover all types).
	/// </summary>
	public Dictionary<string, TileCell> Tiles { get; set; } = new();
}

/// <summary>
/// Location of one tile inside a tile sheet. Either (Col, Row) — in TileSize
/// units — or (X, Y) in pixels may be supplied; pixel coordinates win when
/// both are non-zero.
/// </summary>
[SuppressMessage("Performance", "CA1812", Justification = "Instantiated by JsonSerializer.Deserialize")]
internal sealed class TileCell
{
	public int Col { get; set; }
	public int Row { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
}

/// <summary>
/// Manifest for a sprite sheet PNG. Each variant gathers the three facings
/// (front/back/side) for one head/body/legs shape; <see cref="Variants"/>
/// is ordered to match the <see cref="HeadShape"/> / <see cref="BodyShape"/> /
/// <see cref="LegsShape"/> enum values.
///
/// Sprite art uses the indexed semantic-color convention from
/// <see cref="SemanticPalette"/> so the same PNG produces a different
/// character every time the runtime picks a new HeadPalette/BodyPalette/
/// LegsPalette.
/// </summary>
[SuppressMessage("Performance", "CA1812", Justification = "Instantiated by JsonSerializer.Deserialize")]
internal sealed class SpriteSheetManifest
{
	public string Sheet { get; set; } = "";

	/// <summary>Width of a single part frame in pixels.</summary>
	public int PartWidth { get; set; }

	/// <summary>Height of a single part frame in pixels (e.g. HeadRows for heads).</summary>
	public int PartHeight { get; set; }

	/// <summary>One entry per shape variant, in shape-enum order.</summary>
	public List<SpriteVariant> Variants { get; set; } = new();
}

/// <summary>One shape variant: front + back + side facings, each with its own frame strip.</summary>
[SuppressMessage("Performance", "CA1812", Justification = "Instantiated by JsonSerializer.Deserialize")]
internal sealed class SpriteVariant
{
	public SpritePartLocation Front { get; set; } = new();
	public SpritePartLocation? Back  { get; set; }
	public SpritePartLocation? Side  { get; set; }
}

/// <summary>
/// Pixel-offset location of one facing's frame strip on the sheet, plus the
/// frame count (frames are laid out horizontally, left-to-right).
/// </summary>
[SuppressMessage("Performance", "CA1812", Justification = "Instantiated by JsonSerializer.Deserialize")]
internal sealed class SpritePartLocation
{
	public int X { get; set; }
	public int Y { get; set; }
	public int Frames { get; set; } = 1;
}
