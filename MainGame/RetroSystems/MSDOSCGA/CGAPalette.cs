using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.MSDOSCGA;

// CGA mode 4, palette 1 (high intensity) — the iconic IBM CGA look.
public class CGAPalette : Palette
{
	public CGAPalette() : base(
		[
			new Color(  0,   0,   0),   //  0 black
			new Color( 85, 255, 255),   //  1 bright cyan
			new Color(255,  85, 255),   //  2 bright magenta
			new Color(255, 255, 255),   //  3 white
		])
	{
	}
}
