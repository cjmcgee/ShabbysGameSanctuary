namespace Battleshoot;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		using var host =	new StandaloneHost();
		host.Run();
	}
}
