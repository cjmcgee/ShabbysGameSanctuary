using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using TileEngine.Core;

namespace ChildhoodAdventure.Scoring;

/// <summary>
/// One entry in <c>AtariScores.json</c>: a game we know how to score,
/// keyed by ROM SHA-256. <see cref="ScoreFormula"/> is the rcheevos
/// value expression we'll hand to <see cref="TileEngine.Rcheevos.RcValue.Parse"/>;
/// null on entries where the RA fetcher couldn't find a usable formula
/// (e.g. Combat — multiplayer, no high score concept).
/// </summary>
public sealed class ScoreCatalogEntry
{
	public string Name { get; set; } =	"";
	public string Sha256 { get; set; } =	"";
	public string? Md5 { get; set; }
	public int? RaGameId { get; set; }
	public string? RaGameTitle { get; set; }
	public string? ScoreFormula { get; set; }
	public string? ScoreFormat { get; set; }
	public string? Source { get; set; }
}

/// <summary>
/// Loads the offline-generated <c>AtariScores.json</c> embedded by the
/// csproj as a managed resource and exposes <see cref="LookupBySha256"/>.
/// One instance per process — the catalog is read-only at runtime.
///
/// Missing or unparseable resource is not fatal: the catalog comes up
/// empty, every lookup returns null, and the game runs without score
/// tracking. The user's logs will flag the parse failure but the rest
/// of the game keeps working.
/// </summary>
public sealed class ScoreCatalog
{
	private const string ResourceName =	"ChildhoodAdventure.AtariScores.json";

	private readonly Dictionary<string, ScoreCatalogEntry> _bySha256 =
		new(StringComparer.OrdinalIgnoreCase);

	public int Count =>	_bySha256.Count;

	/// <summary>
	/// Process-wide instance, lazily initialised on first access. The
	/// catalog is read-only at runtime so a singleton is fine and saves
	/// us re-parsing the embedded JSON every time a GameLibrary is built.
	/// </summary>
	public static ScoreCatalog Instance =>	_instance.Value;
	private static readonly Lazy<ScoreCatalog> _instance =	new(Load);

	public static ScoreCatalog Load()
	{
		var catalog =	new ScoreCatalog();
		try
		{
			var asm =	Assembly.GetExecutingAssembly();
			using var stream =	asm.GetManifestResourceStream(ResourceName);
			if (stream == null)
			{
				Log.Warn("ScoreCatalog", $"Embedded resource '{ResourceName}' not found — score tracking disabled.");
				return catalog;
			}

			var file =	JsonSerializer.Deserialize<ScoresFile>(stream,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			if (file?.Games == null)
			{
				Log.Warn("ScoreCatalog", "AtariScores.json deserialised to null Games — score tracking disabled.");
				return catalog;
			}

			foreach (var entry in file.Games)
			{
				if (string.IsNullOrEmpty(entry.Sha256))	continue;
				catalog._bySha256[entry.Sha256.ToLowerInvariant()] =	entry;
			}

			int withFormula =	catalog._bySha256.Values.Count(e =>	!string.IsNullOrEmpty(e.ScoreFormula));
			Log.Info("ScoreCatalog", $"Loaded {catalog._bySha256.Count} entries ({withFormula} with score formula).");
		}
		catch (Exception ex)
		{
			Log.Error("ScoreCatalog", $"Failed to load AtariScores.json: {ex.Message}");
		}
		return catalog;
	}

	/// <summary>
	/// Find the catalog entry whose <see cref="ScoreCatalogEntry.Sha256"/>
	/// matches <paramref name="sha256"/> (case-insensitive). Returns null
	/// when no match or when the formula is null/empty (no point handing
	/// the caller a non-scoreable entry — they'd just check the same
	/// fields and skip).
	/// </summary>
	public ScoreCatalogEntry? LookupBySha256(string sha256)
	{
		if (string.IsNullOrEmpty(sha256))	return null;
		if (!_bySha256.TryGetValue(sha256.ToLowerInvariant(), out var entry))	return null;
		if (string.IsNullOrEmpty(entry.ScoreFormula))	return null;
		return entry;
	}

	private sealed class ScoresFile
	{
		public string? System { get; set; }
		public string? FetchedAt { get; set; }
		public List<ScoreCatalogEntry>? Games { get; set; }
	}
}
