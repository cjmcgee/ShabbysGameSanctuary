namespace ChildhoodAdventure.RetroSystems.MSDOSCGA;

internal sealed class CGAPalette :	Palette
{
	// ── Tile palette ───────────────────────────────────────────────────────
	public static readonly Color Black			= new(  0,   0,   0);	//  0  black
	public static readonly Color BrightCyan		= new( 85, 255, 255);	//  1  bright cyan
	public static readonly Color BrightMagenta	= new(255,  85, 255);	//  2  bright magenta
	public static readonly Color White			= new(255, 255, 255);	//  3  white

	public CGAPalette()	:	base(
		[
			Black,	
			BrightCyan,	
			BrightMagenta,	
			White,
		])
	{
	}
}
