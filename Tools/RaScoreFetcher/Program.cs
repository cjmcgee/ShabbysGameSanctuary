using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

// ─────────────────────────────────────────────────────────────────────────
// RaScoreFetcher
//
// Offline content-pipeline tool. Walks a local ROM folder, matches each
// file to one of the canonical specs (by SHA-256), computes that file's
// MD5 (the hash RA uses for Atari 2600 lookups), then asks RA's authed
// "patch" endpoint for the unobfuscated leaderboard / rich-presence data
// for each game, picks the best score formula, and emits AtariScores.json.
//
// The runtime game ships that JSON as an embedded resource and looks up
// formulas by SHA-256. The MD5 is kept in the file for debugging only.
//
// Run once when the spec list changes or when you want fresher RA data.
// ─────────────────────────────────────────────────────────────────────────

string? romRoot =		null;
string specsPath =		"specs.json";
string outPath =		"AtariScores.json";
int delayMs =			200;
bool verbose =			false;

for (int i = 0; i < args.Length; i++)
{
	switch (args[i])
	{
		case "--rom-root":	romRoot =	args[++i];				break;
		case "--specs":		specsPath =	args[++i];				break;
		case "--out":		outPath =	args[++i];				break;
		case "--delay":		delayMs =	int.Parse(args[++i]);	break;
		case "-v":
		case "--verbose":	verbose =	true;					break;
		case "-h":
		case "--help":
			PrintUsage();	return 0;
		default:
			Console.Error.WriteLine($"Unknown argument: {args[i]}");
			PrintUsage();	return 1;
	}
}

if (string.IsNullOrEmpty(romRoot))
{
	Console.Error.WriteLine("ERROR: --rom-root is required.");
	PrintUsage();	return 1;
}

string raUser =		Environment.GetEnvironmentVariable("RA_USER")		?? "";
string raApiKey =	Environment.GetEnvironmentVariable("RA_API_KEY")	?? "";
// RA_PASSWORD is only needed on the first run (or whenever the cached
// connect token expires / is rejected). The token cache stores the result
// of POSTing r=login2 to dorequest.php; subsequent runs reuse it.
string raPassword =	Environment.GetEnvironmentVariable("RA_PASSWORD")	?? "";
if (raUser == "" || raApiKey == "")
{
	Console.Error.WriteLine("ERROR: missing required env vars: RA_USER, RA_API_KEY");
	Console.Error.WriteLine("  RA_USER      — your RetroAchievements username");
	Console.Error.WriteLine("  RA_API_KEY   — your RA Web API Key (account settings → Keys)");
	Console.Error.WriteLine("  RA_PASSWORD  — only required on first run (we cache the connect token after)");
	return 1;
}

// ── Load specs ───────────────────────────────────────────────────────────
var specs =	JsonSerializer.Deserialize<List<Spec>>(File.ReadAllText(specsPath))
	?? throw new Exception($"Failed to load specs from {specsPath}");
Console.WriteLine($"Loaded {specs.Count} specs from {specsPath}");

// ── Phase 1: walk the ROM root, hash every file, match to specs ──────────
var hashes =	new Dictionary<string, (string Sha256, string Md5, string Path)>(StringComparer.OrdinalIgnoreCase);
var specBySha =	specs.ToDictionary(s =>	s.Sha256.ToLowerInvariant(), s =>	s);

Console.WriteLine($"Walking {romRoot} …");
int filesScanned =	0;
foreach (var file in Directory.EnumerateFiles(romRoot, "*", SearchOption.AllDirectories))
{
	filesScanned++;
	string sha256 =	HashFile(file, SHA256.Create()).ToLowerInvariant();
	if (!specBySha.ContainsKey(sha256))	continue;

	string md5 =	HashFile(file, MD5.Create()).ToLowerInvariant();
	hashes[sha256] =	(sha256, md5, file);
	if (verbose)	Console.WriteLine($"  matched {specBySha[sha256].Name,-22} sha256={sha256[..8]}… md5={md5}");
}
Console.WriteLine($"Scanned {filesScanned} files; matched {hashes.Count}/{specs.Count} specs.");

var missing =	specs.Where(s =>	!hashes.ContainsKey(s.Sha256.ToLowerInvariant())).ToList();
if (missing.Count > 0)
{
	Console.Error.WriteLine($"Note: {missing.Count} specs had no matching file under {romRoot}:");
	foreach (var m in missing)	Console.Error.WriteLine($"  - {m.Name}");
	Console.Error.WriteLine("  (output will include these as no-formula entries so the runtime knows about them.)");
}

// ── Phase 2: pull RA's A2600 game list with hashes; map MD5 → game ID ─────
using var http =	new HttpClient();
http.DefaultRequestHeaders.UserAgent.ParseAdd("RaScoreFetcher/1.0");

// Obtain a dorequest.php connect token: try the on-disk cache first, and
// only fall back to a fresh login (which requires RA_PASSWORD) if the
// cache is missing or the cached token has been invalidated server-side.
// The cache is per-user under SpecialFolder.ApplicationData so it survives
// dotnet build cleans and is plain-text by design (the token is the same
// credential RetroArch leaves in cheevos.conf).
string raToken =	await GetConnectToken(http, raUser, raPassword);

Console.WriteLine("Fetching RA Atari 2600 game list (with hashes) …");
var gameListJson =	await http.GetStringAsync(
	$"https://retroachievements.org/API/API_GetGameList.php?i=25&h=1&y={Uri.EscapeDataString(raApiKey)}");
var games =	JsonNode.Parse(gameListJson)!.AsArray();

var md5ToGameId =	new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
var gameTitleById =	new Dictionary<int, string>();
foreach (var g in games)
{
	if (g == null)	continue;
	int gid =	g["ID"]?.GetValue<int>() ?? 0;
	if (gid == 0)	continue;
	string title =	g["Title"]?.GetValue<string>() ?? "";
	gameTitleById[gid] =	title;
	// The Hashes field is a comma-or-array list of MD5 strings the game accepts.
	var hashesNode =	g["Hashes"];
	if (hashesNode is JsonArray arr)
	{
		foreach (var h in arr)
		{
			var hStr =	h?.GetValue<string>()?.ToLowerInvariant();
			if (!string.IsNullOrEmpty(hStr))	md5ToGameId[hStr] =	gid;
		}
	}
}
Console.WriteLine($"RA has {gameTitleById.Count} A2600 games, {md5ToGameId.Count} known hashes.");

// ── Phase 3: per matched spec, fetch unobfuscated patch + pick a formula ──
var results =	new List<ScoreEntry>();
foreach (var spec in specs)
{
	var entry =	new ScoreEntry
	{
		Name =	spec.Name,
		Sha256 =	spec.Sha256.ToLowerInvariant(),
	};

	if (!hashes.TryGetValue(spec.Sha256.ToLowerInvariant(), out var found))
	{
		entry.Source =	"no ROM file matched the spec sha256 under --rom-root";
		results.Add(entry);
		continue;
	}
	entry.Md5 =	found.Md5;

	if (!md5ToGameId.TryGetValue(found.Md5, out var gameId))
	{
		entry.Source =	$"RA has no game registered for md5={found.Md5}";
		results.Add(entry);
		continue;
	}
	entry.RaGameId =	gameId;
	entry.RaGameTitle =	gameTitleById.GetValueOrDefault(gameId, "?");

	Console.WriteLine($"Fetching patch for {spec.Name} (RA gameId={gameId}) …");
	string patchJson;
	try		{ patchJson =	await FetchPatch(http, raUser, raToken, gameId); }
	catch (TokenRejectedException)
	{
		// Server invalidated the cached token mid-run. Force a fresh login
		// and retry once. After that, give up on this game.
		Console.Error.WriteLine("  cached token rejected mid-run; re-authenticating …");
		ClearTokenCache();
		raToken =	await GetConnectToken(http, raUser, raPassword);
		try		{ patchJson =	await FetchPatch(http, raUser, raToken, gameId); }
		catch (Exception ex2)
		{
			entry.Source =	$"patch fetch failed after re-auth: {ex2.Message}";
			results.Add(entry);
			await Task.Delay(delayMs);
			continue;
		}
	}
	catch (Exception ex)
	{
		entry.Source =	$"patch fetch failed: {ex.Message}";
		results.Add(entry);
		await Task.Delay(delayMs);
		continue;
	}

	var (formula, format, source) =	PickScoreFormula(patchJson, verbose);
	entry.ScoreFormula =	formula;
	entry.ScoreFormat =	format;
	entry.Source =	source;
	results.Add(entry);

	await Task.Delay(delayMs);
}

// ── Phase 4: write output ────────────────────────────────────────────────
var output =	new ScoresFile
{
	System =	"Atari 2600",
	FetchedAt =	DateTime.UtcNow.ToString("O"),
	Games =	results,
};
File.WriteAllText(outPath, JsonSerializer.Serialize(output, new JsonSerializerOptions
{
	WriteIndented =	true,
	DefaultIgnoreCondition =	System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
}));

int withFormula =	results.Count(r =>	!string.IsNullOrEmpty(r.ScoreFormula));
Console.WriteLine();
Console.WriteLine($"Wrote {outPath}: {withFormula}/{results.Count} games have a score formula.");
return 0;

// ─────────────────────────────────────────────────────────────────────────
// Helpers
// ─────────────────────────────────────────────────────────────────────────

static void PrintUsage()
{
	Console.Error.WriteLine("Usage: RaScoreFetcher --rom-root <path> [--specs specs.json] [--out AtariScores.json] [--delay 200] [-v]");
	Console.Error.WriteLine("Env vars: RA_USER, RA_TOKEN, RA_API_KEY");
}

// Connect-token cache location: same per-user app-data folder convention as
// EmulatorConfig in the game. Plain text — the token is the same credential
// RetroArch keeps in cheevos.conf, and the file inherits user-private mode
// on Linux/macOS automatically since Directory.CreateDirectory respects the
// process umask.
static string TokenCachePath() =>	Path.Combine(
	Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
	"RaScoreFetcher",
	"connect-token");

static void ClearTokenCache()
{
	try		{ File.Delete(TokenCachePath()); }
	catch	{ /* best-effort */ }
}

// Resolve a usable connect token: prefer a cached one (and revalidate it),
// otherwise log in with RA_PASSWORD. The revalidation step is a second
// r=login2 call with t=<cached>; RA returns Success=false if the token has
// been rotated server-side, which makes this safer than blindly trusting
// whatever's on disk.
static async Task<string> GetConnectToken(HttpClient http, string user, string password)
{
	string cachePath =	TokenCachePath();

	if (File.Exists(cachePath))
	{
		string cached =	(await File.ReadAllTextAsync(cachePath)).Trim();
		if (cached.Length > 0)
		{
			if (await ValidateToken(http, user, cached))
			{
				Console.WriteLine($"Using cached connect token from {cachePath}.");
				return cached;
			}
			Console.WriteLine("Cached connect token was rejected; re-authenticating …");
		}
	}

	if (string.IsNullOrEmpty(password))
	{
		throw new Exception(
			"No usable cached token and RA_PASSWORD is not set. " +
			"Set RA_PASSWORD once so we can fetch a connect token (it will be cached at " +
			cachePath + " for next time).");
	}

	var form =	new FormUrlEncodedContent(new[]
	{
		new KeyValuePair<string, string>("r",	"login2"),
		new KeyValuePair<string, string>("u",	user),
		new KeyValuePair<string, string>("p",	password),
	});
	using var resp =	await http.PostAsync("https://retroachievements.org/dorequest.php", form);
	string body =	await resp.Content.ReadAsStringAsync();
	var node =	JsonNode.Parse(body)	?? throw new Exception("Login response was not JSON");
	if (node["Success"]?.GetValue<bool>() != true)
	{
		string code =	node["Code"]?.GetValue<string>()	?? "";
		string err =	node["Error"]?.GetValue<string>()	?? body;
		throw new Exception($"Login failed ({code}): {err}");
	}
	string token =	node["Token"]?.GetValue<string>()
		?? throw new Exception("Login succeeded but response had no Token field.");

	Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);
	await File.WriteAllTextAsync(cachePath, token);
	Console.WriteLine($"Logged in; token cached to {cachePath}.");
	return token;
}

// Server-side validate a token by attempting r=login2 with t=<token>. RA
// treats this as a token-refresh call and returns Success=true (with the
// same or rotated token) if the token is still good. Cheap and avoids
// needing a real game ID just to probe auth.
static async Task<bool> ValidateToken(HttpClient http, string user, string token)
{
	var form =	new FormUrlEncodedContent(new[]
	{
		new KeyValuePair<string, string>("r",	"login2"),
		new KeyValuePair<string, string>("u",	user),
		new KeyValuePair<string, string>("t",	token),
	});
	using var resp =	await http.PostAsync("https://retroachievements.org/dorequest.php", form);
	string body =	await resp.Content.ReadAsStringAsync();
	var node =	JsonNode.Parse(body);
	return node?["Success"]?.GetValue<bool>() == true;
}

// Wraps the GET so a credential-rejection comes back as a strongly-typed
// exception the loop can catch and react to (clear cache + re-login),
// distinct from a generic network failure.
static async Task<string> FetchPatch(HttpClient http, string user, string token, int gameId)
{
	string url =
		"https://retroachievements.org/dorequest.php?r=patch" +
		$"&u={Uri.EscapeDataString(user)}" +
		$"&t={Uri.EscapeDataString(token)}" +
		$"&g={gameId}";
	string body =	await http.GetStringAsync(url);

	// dorequest.php returns 200 OK even on auth failure, with Success=false
	// in the body. Inspect the JSON so we can distinguish credential
	// rejection from a usable response.
	JsonNode? probe;
	try		{ probe =	JsonNode.Parse(body); }
	catch	{ return body; }	// not JSON; let the caller handle it

	if (probe?["Success"]?.GetValue<bool>() == false)
	{
		string code =	probe["Code"]?.GetValue<string>() ?? "";
		if (code == "invalid_credentials")
			throw new TokenRejectedException(probe["Error"]?.GetValue<string>() ?? "invalid token");
		throw new Exception($"patch returned {code}: {probe["Error"]?.GetValue<string>()}");
	}

	return body;
}

static string HashFile(string path, HashAlgorithm algo)
{
	using (algo)
	using (var stream = File.OpenRead(path))
	{
		var bytes =	algo.ComputeHash(stream);
		return Convert.ToHexString(bytes);
	}
}

// Picks the best score formula out of a RA patch payload. Preference order:
//   1. Leaderboard with Format == "SCORE" and a title that reads as the
//      canonical high-score board (matches /^high\s*score$/i).
//   2. Any other SCORE-format leaderboard, preferring titles containing
//      "score".
//   3. First SCORE-format leaderboard (whatever order RA returned).
//   4. Rich-presence "Score" macro definition (heuristic — RP scripts
//      vary widely).
// Returns (formulaOrNull, formatOrNull, sourceDescription).
static (string? Formula, string? Format, string Source) PickScoreFormula(string patchJson, bool verbose)
{
	JsonNode? root;
	try		{ root =	JsonNode.Parse(patchJson); }
	catch (Exception ex)	{ return (null, null, $"patch JSON unparseable: {ex.Message}"); }
	if (root == null)	return (null, null, "patch JSON was null");

	// dorequest.php?r=patch usually wraps in { Success, PatchData: {...} }
	// but historically has also returned the patch object at top level.
	// Handle both.
	var patch =	root["PatchData"] ?? root;

	var lbs =	patch["Leaderboards"] as JsonArray;
	if (lbs != null)
	{
		var scoreLbs =	new List<(int Id, string Title, string Mem)>();
		foreach (var lb in lbs)
		{
			if (lb == null)	continue;
			string fmt =	lb["Format"]?.GetValue<string>()
				?? lb["format"]?.GetValue<string>() ?? "";
			if (!string.Equals(fmt, "SCORE", StringComparison.OrdinalIgnoreCase))	continue;

			int id =	lb["ID"]?.GetValue<int>() ?? lb["id"]?.GetValue<int>() ?? 0;
			string title =	lb["Title"]?.GetValue<string>()	?? lb["title"]?.GetValue<string>() ?? "";
			string mem =	lb["Mem"]?.GetValue<string>()		?? lb["mem"]?.GetValue<string>() ?? "";
			if (string.IsNullOrEmpty(mem))	continue;
			scoreLbs.Add((id, title, mem));
		}

		// Priority: exact "high score" match, then "*score*", then first.
		(int Id, string Title, string Mem)? pick = null;
		var highScoreRx =	new Regex(@"^\s*high\s*score\s*$", RegexOptions.IgnoreCase);
		foreach (var lb in scoreLbs)	if (highScoreRx.IsMatch(lb.Title)) { pick =	lb; break; }
		if (pick == null)
			foreach (var lb in scoreLbs)	if (lb.Title.Contains("score", StringComparison.OrdinalIgnoreCase)) { pick =	lb; break; }
		if (pick == null && scoreLbs.Count > 0)	pick =	scoreLbs[0];

		if (pick != null)
		{
			string? value =	ExtractValueFromMem(pick.Value.Mem);
			if (value != null)
				return (value, "SCORE", $"leaderboard #{pick.Value.Id} \"{pick.Value.Title}\"");
		}
	}

	// Fall back to RichPresencePatch. Common shape:
	//   Format:Score
	//   FormatType=VALUE
	//   Display:
	//   Score: @Score(0xH0080)
	// The @Foo macro definition lives elsewhere with a value formula.
	string rp =	patch["RichPresencePatch"]?.GetValue<string>() ?? "";
	if (!string.IsNullOrEmpty(rp))
	{
		// Look for a Format block named (case-insensitively) "score" and
		// pair it with a `@Score(...)` macro in the Display section.
		// Best-effort — if anything looks off, we return null and let the
		// developer fill it in by hand.
		var formatMatch =	Regex.Match(rp,
			@"Format:\s*Score\s*\nFormatType=VALUE\s*\n",
			RegexOptions.IgnoreCase);
		if (formatMatch.Success)
		{
			var displayMatch =	Regex.Match(rp,
				@"@Score\(([^)]+)\)",
				RegexOptions.IgnoreCase);
			if (displayMatch.Success)
				return (displayMatch.Groups[1].Value.Trim(), "SCORE", "rich-presence @Score()");
		}
	}

	return (null, null, "no leaderboard or rich-presence score formula found");
}

// A leaderboard Mem string is segmented "STA:…::CAN:…::SUB:…::VAL:…".
// The VAL segment is the rcheevos value expression we hand to rc_parse_value.
// Some boards use lower-case or omit non-VAL segments — we just grab whatever
// follows "VAL:".
static string? ExtractValueFromMem(string mem)
{
	var m =	Regex.Match(mem, @"(?:^|::)VAL:(?<val>.+?)(?=::|$)", RegexOptions.IgnoreCase);
	if (m.Success)	return m.Groups["val"].Value;
	// No segment markers at all → assume the whole string is a bare value.
	if (!mem.Contains("::"))	return mem;
	return null;
}

// ─────────────────────────────────────────────────────────────────────────
// DTOs
// ─────────────────────────────────────────────────────────────────────────

sealed class TokenRejectedException :	Exception
{
	public TokenRejectedException(string message) :	base(message) {}
}

sealed class Spec
{
	[JsonPropertyName("name")]
	public string Name { get; set; } =	"";

	[JsonPropertyName("sha256")]
	public string Sha256 { get; set; } =	"";
}

sealed class ScoreEntry
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

sealed class ScoresFile
{
	public string System { get; set; } =	"";
	public string FetchedAt { get; set; } =	"";
	public List<ScoreEntry> Games { get; set; } =	new();
}
