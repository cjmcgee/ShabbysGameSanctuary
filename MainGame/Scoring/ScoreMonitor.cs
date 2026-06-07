using Emulation.Libretro;
using Emulation.Rcheevos;

namespace ChildhoodAdventure.Scoring;

/// <summary>
/// One emulated-session's worth of score tracking. Owns a parsed
/// rcheevos value formula, polls the core's SYSTEM_RAM every
/// <see cref="PollFrameInterval"/> emulated frames, tracks the session
/// high-water mark, and pushes updates to <see cref="Highscores"/>
/// — atomically on every new HWM (crash-safe) and again as a session
/// record on session end (history).
///
/// Attach via <see cref="AttachTo"/> — wires the three
/// <see cref="LibretroMiniGame"/> hooks. After that, the monitor is
/// driven entirely by callbacks; no explicit ticking needed.
/// </summary>
internal sealed class ScoreMonitor :	IDisposable
{
	// Stella runs NTSC at ~60 Hz. Polling every 60 emulated frames gives
	// us roughly 1 Hz score updates — frequent enough to catch the
	// session's true peak, sparse enough that the rcheevos eval cost is
	// negligible (sub-millisecond × once a second).
	private const int PollFrameInterval =	60;

	private readonly string _gameName;
	private readonly Highscores _highscores;
	private readonly RcValue _value;

	private LibretroCore?	_core;
	private long	_sessionBest;
	private int		_frameCount;
	private bool	_disposed;

	/// <summary>
	/// Highest score observed during the current session, in whatever
	/// units rcheevos's formula returns. 0 until the first poll.
	/// </summary>
	public long SessionBest =>	_sessionBest;

	/// <summary>
	/// Build a monitor for one play of <paramref name="gameName"/>. The
	/// rcheevos formula is parsed eagerly so an invalid formula fails
	/// loudly here rather than silently during gameplay.
	/// </summary>
	public ScoreMonitor(string gameName, string scoreFormula, Highscores highscores)
	{
		_gameName =		gameName;
		_highscores =	highscores;
		_value =		RcValue.Parse(scoreFormula);
	}

	/// <summary>
	/// Hook this monitor into a fresh <see cref="LibretroMiniGame"/>.
	/// Idempotent against a single mini-game — calling it twice on the
	/// same instance would overwrite the previous handlers, which is
	/// almost certainly a bug, so we no-op if hooks are already
	/// installed by us.
	/// </summary>
	public void AttachTo(LibretroMiniGame miniGame)
	{
		// Defensive: cling to the monitor's lifetime via the hook
		// closures. If someone forgets to dispose the monitor we'd
		// rather keep tracking score than have it silently die.
		miniGame.OnCoreReady +=	HandleCoreReady;
		miniGame.OnFrameTick +=	HandleFrameTick;
		miniGame.OnSessionEnd +=	HandleSessionEnd;
	}

	private void HandleCoreReady(LibretroCore core)
	{
		_core =	core;
		_sessionBest =	0;
		_frameCount =	0;
		Log.Info("ScoreMonitor",
			$"Starting session for {_gameName} (RAM={core.SystemRamSize} bytes, all-time best={_highscores.GetBest(_gameName)}).");
	}

	private void HandleFrameTick(LibretroCore core)
	{
		if (_disposed)	return;
		_frameCount++;
		if (_frameCount < PollFrameInterval)	return;
		_frameCount =	0;

		// Evaluate the formula against current RAM. rcheevos may issue
		// 1/2/4-byte peeks per evaluation; ReadSystemRam matches the
		// MemoryPeek signature directly.
		int score;
		try		{ score =	_value.Evaluate(core.ReadSystemRam); }
		catch (Exception ex)
		{
			// One-shot warn; if the formula is bad we shouldn't spam
			// the log every poll. Stop polling for this session.
			Log.Error("ScoreMonitor", $"{_gameName}: formula evaluation threw ({ex.Message}). Disabling for this session.");
			_disposed =	true;
			return;
		}

		// rcheevos returns int32; we widen to long for storage
		// consistency with Highscores (some games can exceed int range
		// over multiple loops, though A2600 BCD scores top out around
		// 999,999).
		long widened =	score;
		if (widened > _sessionBest)
		{
			_sessionBest =	widened;
			// Persist immediately on every new high to survive crashes
			// mid-session. The atomic write inside Highscores keeps the
			// on-disk file consistent even if the process dies during
			// the rename.
			_highscores.RecordNewBestIfHigher(_gameName, _sessionBest);
		}
	}

	private void HandleSessionEnd(LibretroCore core)
	{
		if (_disposed)	{ _disposed =	false; return; }	// disabled mid-session, skip persisting

		// One last peek before the core tears down. Catches a final
		// score bump that happened between the last poll and the user
		// exiting. Cheap, and means we never under-report a session.
		try
		{
			long final =	_value.Evaluate(core.ReadSystemRam);
			if (final > _sessionBest)	_sessionBest =	final;
		}
		catch (Exception ex)
		{
			Log.Warn("ScoreMonitor", $"{_gameName}: final-peek eval threw ({ex.Message}); using last polled value.");
		}

		_highscores.RecordSessionEnd(_gameName, _sessionBest);
	}

	public void Dispose()
	{
		if (_disposed)	return;
		_disposed =	true;
		_value.Dispose();
	}
}
