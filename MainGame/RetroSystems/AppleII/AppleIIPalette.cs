namespace ChildhoodAdventure.RetroSystems.AppleII;

internal sealed class AppleIIPalette :	Palette
{
	// ── Tile palette ───────────────────────────────────────────────────────
	public static readonly Color Black	= new(  0,   0,   0);	//  0  black
	public static readonly Color Green	= new( 20, 245,  60);	//  1  green
	public static readonly Color Violet	= new(193,  28, 255);	//  2  violet
	public static readonly Color White	= new(255, 255, 255);	//  3  white
	public static readonly Color Orange	= new(255, 106,  60);	//  4  orange
	public static readonly Color Blue	= new( 20,  88, 255);	//  5  blue

	public AppleIIPalette()	:	base(
		[
			Black,	
			Green,	
			Violet,	
			White,	
			Orange,	
			Blue,
		])
	{
	}
}
