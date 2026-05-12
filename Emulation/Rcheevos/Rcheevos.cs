using System.Runtime.InteropServices;
using TileEngine.Core;

namespace TileEngine.Rcheevos;

/// <summary>
/// Callback signature for reading game memory during formula evaluation.
/// rcheevos asks for 1, 2, or 4 bytes at a given address (little-endian).
/// Implementations should return 0 for out-of-range reads — that's the
/// convention the rcheevos test harness uses and what the library tolerates.
/// </summary>
public delegate uint MemoryPeek(uint address, uint numBytes);

/// <summary>
/// A parsed rcheevos value expression, ready for repeated evaluation.
///
/// Lifecycle:
///   var v =	RcValue.Parse("0xH00aa*100+0xH00ab");
///   int score =	v.Evaluate((addr, n) =&gt; ReadFromEmulatorRam(addr, n));
///   v.Dispose();
///
/// rcheevos allocates the parsed structure into a caller-provided buffer
/// whose size it reports up front via <c>rc_value_size</c>. We pin a
/// managed byte array as that buffer for the lifetime of the
/// <see cref="RcValue"/>, freeing the GC pin in <see cref="Dispose"/>.
/// </summary>
public sealed class RcValue :	IDisposable
{
	private GCHandle _bufferHandle;
	private readonly IntPtr _valuePtr;
	private bool _disposed;

	private RcValue(GCHandle bufferHandle, IntPtr valuePtr)
	{
		_bufferHandle =	bufferHandle;
		_valuePtr =	valuePtr;
	}

	/// <summary>
	/// Compile <paramref name="formula"/> into a reusable <see cref="RcValue"/>.
	/// Throws <see cref="ArgumentException"/> with the rcheevos error code
	/// on parse failure (rc_value_size returns a negative value when the
	/// formula is malformed; that negative number is the error code).
	/// </summary>
	public static RcValue Parse(string formula)
	{
		if (string.IsNullOrEmpty(formula))
			throw new ArgumentException("Formula cannot be null or empty.", nameof(formula));

		// rcheevos tells us up-front exactly how much memory it needs to
		// hold the parsed representation of this formula. Allocate that
		// much in a pinned managed array so we don't need to round-trip
		// through Marshal.AllocHGlobal / FreeHGlobal.
		int size =	Native.rc_value_size(formula);
		if (size < 0)
			throw new ArgumentException(
				$"rcheevos rejected the formula (rc_value_size returned error code {size}): {formula}",
				nameof(formula));
		if (size == 0)
			throw new ArgumentException("rcheevos reported zero size for the formula — unexpected.", nameof(formula));

		var buffer =	new byte[size];
		var handle =	GCHandle.Alloc(buffer, GCHandleType.Pinned);
		try
		{
			IntPtr value =	Native.rc_parse_value(
				handle.AddrOfPinnedObject(), formula, IntPtr.Zero, 0);
			if (value == IntPtr.Zero)
				throw new InvalidOperationException(
					$"rc_parse_value returned null for formula: {formula}");
			return new RcValue(handle, value);
		}
		catch
		{
			handle.Free();
			throw;
		}
	}

	/// <summary>
	/// Evaluate the parsed formula against the memory exposed by
	/// <paramref name="peek"/>. Returns the formula's computed value.
	///
	/// Thread-safety: the peek callback dispatch uses a
	/// <c>[ThreadStatic]</c> slot, so concurrent evaluations from
	/// different threads work as long as each thread uses its own
	/// <paramref name="peek"/>. Re-entrancy on the same thread is not
	/// supported (Evaluate cannot recursively call into another
	/// Evaluate via the peek callback).
	/// </summary>
	public int Evaluate(MemoryPeek peek)
	{
		if (peek == null)	throw new ArgumentNullException(nameof(peek));
		ObjectDisposedException.ThrowIf(_disposed, this);

		// Store the caller's peek into the thread-static so the static
		// trampoline below can find it without a per-call allocation
		// for closure-capture. The native side calls the trampoline
		// once per memory read inside rc_evaluate_value.
		var prev =	_threadPeek;
		_threadPeek =	peek;
		try
		{
			return Native.rc_evaluate_value(_valuePtr, _trampolinePtr, IntPtr.Zero, IntPtr.Zero);
		}
		finally
		{
			_threadPeek =	prev;
		}
	}

	public void Dispose()
	{
		if (_disposed)	return;
		_disposed =	true;
		if (_bufferHandle.IsAllocated)	_bufferHandle.Free();
	}

	// ── Native callback plumbing ─────────────────────────────────────────

	// Per-thread "currently active peek" — the trampoline reads this on
	// each native callback. ThreadStatic so a future ScoreMonitor running
	// on the libretro thread doesn't trip over a different thread's
	// evaluation.
	[ThreadStatic] private static MemoryPeek? _threadPeek;

	// Static trampoline that the native library calls back through.
	// Marked unmanaged-cdecl to match rc_peek_t (RC_CCONV expands to
	// __cdecl on win32 i386 and is a no-op elsewhere).
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate uint RcPeekDelegate(uint address, uint numBytes, IntPtr ud);

	private static readonly RcPeekDelegate _trampoline =	Trampoline;
	private static readonly IntPtr _trampolinePtr =	Marshal.GetFunctionPointerForDelegate(_trampoline);

	private static uint Trampoline(uint address, uint numBytes, IntPtr _)
	{
		var peek =	_threadPeek;
		if (peek == null)
		{
			// rc_evaluate_value shouldn't be calling us outside of an
			// active Evaluate(...) frame, but if it ever does, log and
			// return 0 (rcheevos's "memory not available" convention).
			Log.Warn("Rcheevos", $"Peek trampoline called with no active peek (addr=0x{address:X}, n={numBytes})");
			return 0;
		}
		try		{ return peek(address, numBytes); }
		catch (Exception ex)
		{
			Log.Error("Rcheevos", $"Peek callback threw: {ex.Message}");
			return 0;
		}
	}
}

/// <summary>
/// Raw native bindings to <c>librcheevos</c>. Internal — consumers go through
/// <see cref="RcValue"/>. Naming and signatures mirror the C headers in
/// <c>rcheevos/include/rc_runtime_types.h</c> exactly.
/// </summary>
internal static class Native
{
	// .NET's DllImport name maps to:
	//   Linux   : librcheevos.so
	//   macOS   : librcheevos.dylib
	//   Windows : rcheevos.dll
	// All three are produced by the Native/Rcheevos/Makefile and dropped
	// next to the consumer's exe by its csproj's CopyToOutputDirectory.
	private const string Lib =	"rcheevos";

	[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
	public static extern int rc_value_size(
		[MarshalAs(UnmanagedType.LPStr)] string memaddr);

	[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr rc_parse_value(
		IntPtr buffer,
		[MarshalAs(UnmanagedType.LPStr)] string memaddr,
		IntPtr unused_L,
		int unused_funcs_idx);

	[DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
	public static extern int rc_evaluate_value(
		IntPtr value,
		IntPtr peek_fnptr,
		IntPtr peek_ud,
		IntPtr unused_L);
}
