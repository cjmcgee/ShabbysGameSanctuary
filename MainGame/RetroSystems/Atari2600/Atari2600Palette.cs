using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.Atari2600;

public class Atari2600Palette : Palette
{
	public Atari2600Palette() : base(
		[
			new Color(  0,   0,   0),   // 0  black          — background
			new Color(188, 140,  56),   // 1  warm amber      — WoodFloor
			new Color(124,  72,   8),   // 2  dark brown      — (sprite palette; unused by tiles)
			new Color(200,  20,  20),   // 3  vivid red       — Carpet, Bookshelf
			new Color(132,   4,   4),   // 4  dark red        — (sprite palette; unused by tiles)
			new Color(220, 220,   0),   // 5  bright yellow   — KitchenTile
			new Color(220, 220, 200),   // 6  near-white      — Wall
			new Color(148, 148, 132),   // 7  light gray      — (sprite palette; unused by tiles)
			new Color( 20,  12,   4),   // 8  near-black      — Door
			new Color( 36,  80, 200),   // 9  bold blue       — Furniture
			new Color(144, 144, 144),   // 10 mid gray        — Counter
			new Color(  0, 200, 220),   // 11 bright cyan     — Window
			new Color(  0, 188,   0),   // 12 vivid green     — Grass, Plant
			new Color( 28,  28,  40),   // 13 near-black road — Road
			new Color(104, 104, 104),   // 14 medium gray     — Sidewalk
		] )
	{
	}
}