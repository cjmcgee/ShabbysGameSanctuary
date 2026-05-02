using Microsoft.Xna.Framework;

namespace ChildhoodAdventure.RetroSystems.AppleII;

// Apple II hi-res 6-color artifact palette — black, green, violet, white, orange, blue.
public class AppleIIPalette : Palette
{
	public AppleIIPalette() : base(
		[
			new Color(  0,   0,   0),   //  0 black
			new Color( 20, 245,  60),   //  1 green
			new Color(193,  28, 255),   //  2 violet
			new Color(255, 255, 255),   //  3 white
			new Color(255, 106,  60),   //  4 orange
			new Color( 20,  88, 255),   //  5 blue
		])
	{
	}
}
