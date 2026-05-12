using TileEngine.MiniGames.Libretro;

// ─────────────────────────────────────────────────────────────────────────
// LibretroMemorySmokeTest
//
// Boots Stella + a real Atari 2600 ROM, runs the core for a few frames,
// then peeks the emulated system RAM via the new LibretroCore.ReadSystemRam
// path. Used to confirm Phase 2 plumbing is correct end-to-end before
// hooking it up to rcheevos in Phase 4.
//
// The test does NOT render or output audio — the libretro callbacks are
// no-ops. We just want the core to populate its TIA RAM, which Stella
// does within the first frame.
// ─────────────────────────────────────────────────────────────────────────

string? corePath =	null;
string? romPath =	null;
int frames =	8;

for (int i = 0; i < args.Length; i++)
{
	switch (args[i])
	{
		case "--core":	corePath =	args[++i];				break;
		case "--rom":	romPath =	args[++i];				break;
		case "--frames":	frames =	int.Parse(args[++i]);	break;
		case "-h":
		case "--help":
			Usage();	return 0;
		default:
			Console.Error.WriteLine($"Unknown argument: {args[i]}");
			Usage();	return 1;
	}
}

if (string.IsNullOrEmpty(corePath) || string.IsNullOrEmpty(romPath))
{
	Console.Error.WriteLine("ERROR: --core and --rom are required.");
	Usage();
	return 1;
}
if (!File.Exists(corePath))	{ Console.Error.WriteLine($"Core not found: {corePath}"); return 1; }
if (!File.Exists(romPath))	{ Console.Error.WriteLine($"ROM not found: {romPath}"); return 1; }

Console.WriteLine($"Libretro memory peek smoke test");
Console.WriteLine($"  core   = {corePath}");
Console.WriteLine($"  rom    = {romPath}");
Console.WriteLine($"  frames = {frames}");
Console.WriteLine();

using var core =	new LibretroCore();
core.Open(corePath);
Console.WriteLine($"  loaded core: {core.LibraryName} {core.LibraryVersion}");

core.Init();
if (!core.LoadGame(romPath))
{
	Console.Error.WriteLine("LoadGame returned false — Stella refused the ROM.");
	return 1;
}
Console.WriteLine($"  game loaded; av: {core.BaseWidth}x{core.BaseHeight} @ {core.Fps:F2} Hz, audio {core.SampleRate:F0} Hz");
Console.WriteLine($"  system_ram_size = {core.SystemRamSize} bytes");
Console.WriteLine();

if (core.SystemRamSize == 0)
{
	Console.Error.WriteLine("FAIL: core reports SystemRamSize == 0 (no SYSTEM_RAM exposed).");
	return 1;
}
if (core.SystemRamSize != 128)
{
	Console.Error.WriteLine($"WARN: SystemRamSize is {core.SystemRamSize}, expected 128 for Atari 2600. Continuing anyway.");
}

// Run a few frames so Stella has a chance to populate its initial state.
// Without this the RAM may all be zero on the first peek.
for (int i = 0; i < frames; i++)	core.Run();

// Dump the full A2600 RAM as a 16-wide hex grid. If we got a stable
// pointer to real memory, this should look like emulator state (not
// all-zero, not garbage).
Console.WriteLine($"System RAM contents after {frames} frames:");
for (uint row = 0; row < core.SystemRamSize; row += 16)
{
	Console.Write($"  {row:X4}:");
	for (uint col = 0; col < 16 && row + col < core.SystemRamSize; col++)
		Console.Write($" {core.ReadSystemRam(row + col, 1):X2}");
	Console.WriteLine();
}
Console.WriteLine();

// Exercise the multi-byte read path. ReadSystemRam(addr, 2) should be
// little-endian: same as ReadByte(addr) | (ReadByte(addr+1) << 8).
uint b0 =	core.ReadSystemRam(0, 1);
uint b1 =	core.ReadSystemRam(1, 1);
uint w =	core.ReadSystemRam(0, 2);
uint expectedW =	b0 | (b1 << 8);
Console.WriteLine($"Endianness check at 0x0000:");
Console.WriteLine($"  byte[0] = 0x{b0:X2}, byte[1] = 0x{b1:X2}");
Console.WriteLine($"  ReadSystemRam(0,2) = 0x{w:X4}");
Console.WriteLine($"  expected (LE)      = 0x{expectedW:X4}");
if (w != expectedW)
{
	Console.Error.WriteLine("FAIL: 16-bit read isn't little-endian.");
	return 1;
}

// Bounds: reading past end should return 0.
uint past =	core.ReadSystemRam(core.SystemRamSize, 1);
uint straddling =	core.ReadSystemRam(core.SystemRamSize - 1, 2);	// 1 byte in-range, next out
if (past != 0)	{ Console.Error.WriteLine($"FAIL: past-end peek should be 0, got 0x{past:X2}"); return 1; }
if (straddling != 0)	{ Console.Error.WriteLine($"FAIL: straddling-end peek should be 0, got 0x{straddling:X4}"); return 1; }

Console.WriteLine();
Console.WriteLine("All checks passed.");
return 0;

static void Usage()
{
	Console.Error.WriteLine("Usage: LibretroMemorySmokeTest --core <path> --rom <path> [--frames N]");
}
