using Emulation.Rcheevos;

// ─────────────────────────────────────────────────────────────────────────
// RcheevosSmokeTest
//
// Drives the C# P/Invoke wrapper through a handful of hand-crafted
// formula → expected-value cases, against synthetic memory. Run after
// any change to the Makefile, the wrapper, or the rcheevos submodule.
//
// rcheevos formula notation cheat sheet (just what these cases need):
//   0xH<addr>   read 1 byte (8-bit) from <addr>
//   0x <addr>   read 2 bytes (16-bit LE)
//   0xX<addr>   read 4 bytes (32-bit LE)
//   B0xH<addr>  read 1 byte and treat it as BCD (e.g. 0x42 → 42)
//   _          operand separator — operands joined by '_' are SUMMED
//             (NOT '+' — '+' isn't a valid value-formula operator)
//   *N         multiply the immediately-preceding operand by N
//   M: / A:    explicit "Measured" / "Add" markers (used inside
//             leaderboards; bare-value formulas don't need them)
// ─────────────────────────────────────────────────────────────────────────

// Synthetic system RAM. Indices match the rcheevos peek address.
var ram =	new byte[256];
ram[0x80] =	5;
ram[0x81] =	42;
ram[0x82] =	0x99;	// 0x99 = decimal 153 raw, decimal 99 if read as BCD
ram[0xAA] =	0x12;
ram[0xAB] =	0x34;	// at 0xAA as 16-bit LE = 0x3412 = 13330

// Peek callback. rcheevos asks for 1, 2, or 4 bytes; we honour the
// width and return little-endian. Returning 0 for out-of-bounds matches
// the convention of the rcheevos test harness.
uint Peek(uint address, uint numBytes)
{
	if (address >= ram.Length)	return 0;
	uint value =	0;
	for (int i = 0; i < numBytes && address + i < ram.Length; i++)
		value |=	(uint)ram[address + i] << (8 * i);
	return value;
}

int passed =	0;
int failed =	0;

void Check(string formula, int expected, string note = "")
{
	try
	{
		using var v =	RcValue.Parse(formula);
		int actual =	v.Evaluate(Peek);
		bool ok =	actual == expected;
		string marker =	ok ? "OK  " :	"FAIL";
		Console.WriteLine($"  [{marker}] {formula,-32}  expected={expected,10}  got={actual,10}  {note}");
		if (ok)	passed++; else failed++;
	}
	catch (Exception ex)
	{
		Console.WriteLine($"  [FAIL] {formula,-32}  threw: {ex.Message}");
		failed++;
	}
}

Console.WriteLine("rcheevos P/Invoke smoke test");
Console.WriteLine("============================");
Console.WriteLine();

// Single byte read.
Check("0xH0080", 5, "byte at 0x80");

// Two bytes summed via the `_` operand separator. The result is the sum
// of all operands; this is the bread-and-butter A2600 score pattern
// (e.g. Asteroids: "b0xh03e*10_b0xh03d*1000"). ram[0x80]=5, ram[0x81]=42.
Check("0xH0080*100_0xH0081", 542, "5*100 + 42 (operand sum)");

// The same arithmetic written with explicit A:/M: markers, the style
// real RA leaderboard `VAL:` expressions tend to use.
Check("A:0xH0080*100_M:0xH0081", 542, "Add 5*100, Measure 42 → same sum");

// 16-bit little-endian read. ram[0xAA..0xAB] = 0x12, 0x34 → 0x3412 = 13330.
Check("0x 00aa", 0x3412, "16-bit LE at 0xAA");

// BCD: 0x99 byte read as BCD = 99.
Check("B0xH0082", 99, "BCD-decoded byte at 0x82");

// Out-of-range read → peek returns 0.
Check("0xH00ff", 0, "byte at 0xFF (last index, RAM cleared)");

// Measured-style formula. The "M:" prefix marks the measured operand;
// for a bare value expression rcheevos returns that operand's value.
Check("M:0xH0080*10", 50, "measured 5*10");

Console.WriteLine();
Console.WriteLine($"Result: {passed} passed, {failed} failed");
return failed == 0 ? 0 :	1;
