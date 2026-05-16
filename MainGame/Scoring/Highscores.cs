using System.Text.Json;
using TileEngine.Core;

namespace ChildhoodAdventure.Scoring;

/// <summary>
/// Per-game best score plus a capped history of session end-of-play scores.
/// Serialised to <c>highscores.json</c> next to the emulator config under
/// the per-user app-data folder.
/// </summary>
public sealed class GameScoreRecord
{
	public long Best { get; set; }
	public string BestAt { get; set; } =	"";
	public List<SessionScore> Sessions { get; set; } =	new();
}

public sealed class SessionScore
{
	public long Score { get; set; }
	public string EndedAt { get; set; } =	"";
}

/// <summary>
/// File-backed high-score persistence.
///
/// Storage: <c>%APPDATA%\ChildhoodAdventure\highscores.json</c> (or the
/// XDG equivalent on Linux/macOS) — same per-user folder as
/// EmulatorConfig. Kept as a separate file so a score update doesn't
/// rewrite the path-config every time.
///
/// Writes are atomic: render to <c>highscores.json.tmp</c>, then move
/// over the live file. Process crashes during the write leave at most
/// the previous good copy on disk; partial writes never appear in
/// <c>highscores.json</c>.
///
/// In-memory cache: the JSON is read once at load and held in
/// <see cref="_byGame"/>. Updates mutate the cache and then re-serialise
/// — fine for our scale (22 games, low write frequency).
/// </summary>
public sealed class Highscores
{
	private const int MaxSessionHistory =	100;

	private readonly object _lock =	new();
	private readonly string _filePath;
	private readonly Dictionary<string, GameScoreRecord> _byGame =	new();

	private Highscores(string filePath, Dictionary<string, GameScoreRecord> initial)
	{
		_filePath =	filePath;
		_byGame =	initial;
	}

	public static string DefaultPath =>	Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
		"ChildhoodAdventure",
		"highscores.json");

	/// <summary>
	/// Process-wide instance, lazily initialised on first access. Game
	/// code reads/writes the same on-disk file via this singleton; the
	/// internal lock guards concurrent updates from the libretro frame
	/// thread (rare in practice but cheap insurance).
	/// </summary>
	public static Highscores Instance =>	_instance.Value;
	private static readonly Lazy<Highscores> _instance =	new(() =>	LoadOrCreate());

	public static Highscores LoadOrCreate(string? overridePath = null)
	{
		string path =	overridePath ?? DefaultPath;
		var loaded =	new Dictionary<string, GameScoreRecord>();
		if (File.Exists(path))
		{
			try
			{
				var json =	File.ReadAllText(path);
				var file =	JsonSerializer.Deserialize<HighscoresFile>(json,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				if (file?.Scores != null)	loaded =	file.Scores;
				Log.Info("Highscores", $"Loaded {loaded.Count} game record(s) from {path}.");
			}
			catch (Exception ex)
			{
				// Don't blow up the game over a corrupt highscores file —
				// log it and continue with an empty in-memory state. The
				// next save will overwrite the broken file.
				Log.Error("Highscores", $"Failed to read {path}: {ex.Message}. Starting fresh.");
			}
		}
		return new Highscores(path, loaded);
	}

	/// <summary>Best score we've ever seen for <paramref name="gameName"/>, or 0 if none.</summary>
	public long GetBest(string gameName)
	{
		lock (_lock)
		{
			return _byGame.TryGetValue(gameName, out var rec) ? rec.Best :	0;
		}
	}

	/// <summary>
	/// Update the per-game best if <paramref name="score"/> exceeds the
	/// current record. Called from inside an emulation session every
	/// time the session's HWM ticks up — guarantees the new best survives
	/// even if the game crashes before the session-end save.
	/// </summary>
	public void RecordNewBestIfHigher(string gameName, long score)
	{
		lock (_lock)
		{
			if (!_byGame.TryGetValue(gameName, out var rec))
				_byGame[gameName] =	rec =	new GameScoreRecord();

			if (score <= rec.Best)	return;

			rec.Best =	score;
			rec.BestAt =	DateTime.UtcNow.ToString("O");
			Save();
			Log.Info("Highscores", $"New best for {gameName}: {score}");
		}
	}

	/// <summary>
	/// Append a session-end entry with the session's best score and
	/// timestamp. Always saved, regardless of whether it beat the
	/// all-time best (the "history" view will want every session).
	/// History is capped at <see cref="MaxSessionHistory"/> per game so
	/// the file doesn't grow unbounded over years of play.
	/// </summary>
	public void RecordSessionEnd(string gameName, long sessionBest)
	{
		lock (_lock)
		{
			if (!_byGame.TryGetValue(gameName, out var rec))
				_byGame[gameName] =	rec =	new GameScoreRecord();

			rec.Sessions.Add(new SessionScore
			{
				Score =		sessionBest,
				EndedAt =	DateTime.UtcNow.ToString("O"),
			});
			// Rolling window — drop oldest entries past the cap.
			if (rec.Sessions.Count > MaxSessionHistory)
				rec.Sessions.RemoveRange(0, rec.Sessions.Count - MaxSessionHistory);

			// Defensive: if a session ends with a higher score than
			// we've recorded (e.g. the new-best mid-session save was
			// missed for some reason), promote it now.
			if (sessionBest > rec.Best)
			{
				rec.Best =	sessionBest;
				rec.BestAt =	DateTime.UtcNow.ToString("O");
			}

			Save();
			Log.Info("Highscores", $"Session end for {gameName}: best={sessionBest}");
		}
	}

	// MUST be called inside _lock — we don't recursively lock here.
	private void Save()
	{
		try
		{
			var dir =	Path.GetDirectoryName(_filePath);
			if (!string.IsNullOrEmpty(dir))	Directory.CreateDirectory(dir);

			var payload =	new HighscoresFile
			{
				Version =	1,
				Scores =	_byGame,
			};
			var json =	JsonSerializer.Serialize(payload,
				new JsonSerializerOptions { WriteIndented = true });

			// Atomic write: tmp + rename. On Linux/macOS rename is atomic
			// within a filesystem; on Windows .NET's File.Move(..., true)
			// uses MoveFileEx with MOVEFILE_REPLACE_EXISTING which is
			// effectively atomic against same-process readers.
			var tmpPath =	_filePath + ".tmp";
			File.WriteAllText(tmpPath, json);
			File.Move(tmpPath, _filePath, overwrite: true);
		}
		catch (Exception ex)
		{
			Log.Error("Highscores", $"Failed to save {_filePath}: {ex.Message}");
		}
	}

	private sealed class HighscoresFile
	{
		public int Version { get; set; } =	1;
		public Dictionary<string, GameScoreRecord> Scores { get; set; } =	new();
	}
}
