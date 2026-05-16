using System.Security.Cryptography;

namespace ChildhoodAdventure;

/// <summary>
/// One required ROM as the game expects it: canonical title, the file
/// size of the official dump, and the SHA-256 of that dump (lower-case
/// hex). The size is used to narrow disk scanning to plausible
/// candidates; the hash confirms identity.
/// </summary>
public sealed record RomSpec(string Name, long ExpectedSize, string ExpectedSha256Hex);

/// <summary>
/// How a <see cref="RomSpec"/> got matched on the user's disk.
/// Order matters in two places: it determines the badge colour shown
/// in the ROM Manager UI, and any future "best of N" tie-breaking
/// (a hash hit always beats a name hit).
/// </summary>
public enum RomResolution
{
	/// <summary>No file matched by hash, name, or override.</summary>
	NotFound,
	/// <summary>Resolved by a SHA-256 match against the size-bucketed candidates.</summary>
	FoundByHash,
	/// <summary>Hash didn't match, but a same-size file's basename starts with the spec name.</summary>
	FoundByName,
	/// <summary>User has manually pointed this spec at a specific file.</summary>
	Override,
}

/// <summary>
/// Result of one resolve attempt. <see cref="ResolvedPath"/> is null
/// when <see cref="Resolution"/> is <see cref="RomResolution.NotFound"/>;
/// otherwise it's an absolute path to a file that existed at resolve
/// time (no guarantees it still does — caller should re-check
/// <c>File.Exists</c> when launching).
/// </summary>
public sealed record RomMatch(RomSpec Spec, RomResolution Resolution, string? ResolvedPath)
{
	public bool IsAvailable =>	Resolution != RomResolution.NotFound;
}

/// <summary>
/// Scans a ROM folder once and resolves a list of <see cref="RomSpec"/>s
/// against the files found, with three precedence rules layered:
///
///   1. <b>Override</b> wins absolutely. If the user has manually pointed
///      a spec at a file, we use that file (provided it still exists) —
///      even if it doesn't match the expected hash.  Manual choice is
///      a strong signal; we trust it.
///   2. <b>Hash</b> matches beat name matches. Any size-matched file
///      whose SHA-256 equals the spec's expected hash wins.
///   3. <b>Name prefix</b> match is the fallback. Among size-matched
///      files, pick one whose basename (without extension, case-
///      insensitive) starts with the spec name. Files whose basename
///      ALSO starts with a longer spec name are rejected — they belong
///      to that longer spec, not this one. Among remaining candidates,
///      the shortest basename wins (least-decorated filename).
///
/// The directory walk happens once per <see cref="Resolve"/> call, and
/// SHA-256 is computed at most once per file (cached internally). For
/// the current 22-ROM list against a typical ROM collection this
/// completes in well under a frame even on slow disks.
/// </summary>
public static class RomResolver
{
	public static IReadOnlyList<RomMatch> Resolve(
		IReadOnlyList<RomSpec> specs,
		string romRoot,
		IReadOnlyDictionary<string, string>? overrides)
	{
		var found =	new RomMatch?[specs.Count];

		// ── Phase 0: manual overrides ──────────────────────────────────
		// Honoured before any disk scan so a user override never gets
		// hijacked by an auto-resolve. If the override file vanished
		// since the user picked it, fall through to auto-resolution
		// rather than locking the spec into a permanent NotFound (a
		// missing override is more likely "they moved the folder" than
		// "they want this missing").
		if (overrides != null)
		{
			for (int i = 0; i < specs.Count; i++)
			{
				if (!overrides.TryGetValue(specs[i].Name, out var p))	continue;
				if (string.IsNullOrEmpty(p))	continue;
				if (!File.Exists(p))	continue;
				found[i] =	new RomMatch(specs[i], RomResolution.Override, p);
			}
		}

		// ── Phase 1: walk the ROM root, bucketing files by size ────────
		// We only care about files whose size matches some unresolved
		// spec; everything else gets skipped before we touch FileInfo.
		var unresolvedSizes =	new HashSet<long>();
		for (int i = 0; i < specs.Count; i++)
			if (found[i] == null)	unresolvedSizes.Add(specs[i].ExpectedSize);

		var bySize =	new Dictionary<long, List<string>>();
		if (!string.IsNullOrEmpty(romRoot) && Directory.Exists(romRoot) && unresolvedSizes.Count > 0)
		{
			try
			{
				foreach (var path in Directory.EnumerateFiles(romRoot, "*", SearchOption.AllDirectories))
				{
					long size;
					try		{ size =	new FileInfo(path).Length; }
					catch	{ continue; }
					if (!unresolvedSizes.Contains(size))	continue;
					if (!bySize.TryGetValue(size, out var list))	bySize[size] =	list =	new();
					list.Add(path);
				}
			}
			catch (UnauthorizedAccessException)	{ /* skip unreadable */ }
			catch (IOException)					{ /* network hiccup */ }
		}

		// ── Phase 2: hash-then-name pass ───────────────────────────────
		// Hash cache prevents re-hashing the same file when multiple
		// specs share an expected size (e.g. several 4096-byte ROMs).
		var hashCache =	new Dictionary<string, string>(StringComparer.Ordinal);

		string? Hash(string path)
		{
			if (hashCache.TryGetValue(path, out var cached))	return cached;
			try
			{
				using var stream =	File.OpenRead(path);
				using var sha =	SHA256.Create();
				var bytes =	sha.ComputeHash(stream);
				var hex =	Convert.ToHexString(bytes).ToLowerInvariant();
				hashCache[path] =	hex;
				return hex;
			}
			catch (Exception ex)
			{
				Log.Warn("RomResolver", $"Could not hash {path}: {ex.Message}");
				return null;
			}
		}

		// Pre-build the set of spec names once so the "claimed by a
		// longer name" check is a quick loop, not a LINQ allocation
		// per file.
		var allNames =	specs.Select(s =>	s.Name).ToArray();

		// 2a: hash matches first — strongest signal.
		for (int i = 0; i < specs.Count; i++)
		{
			if (found[i] != null)	continue;
			if (!bySize.TryGetValue(specs[i].ExpectedSize, out var candidates))	continue;

			foreach (var path in candidates)
			{
				var h =	Hash(path);
				if (h != null && string.Equals(h, specs[i].ExpectedSha256Hex, StringComparison.OrdinalIgnoreCase))
				{
					found[i] =	new RomMatch(specs[i], RomResolution.FoundByHash, path);
					break;
				}
			}
		}

		// 2b: name-prefix fallback.
		for (int i = 0; i < specs.Count; i++)
		{
			if (found[i] != null)	continue;
			if (!bySize.TryGetValue(specs[i].ExpectedSize, out var candidates))	continue;

			var thisName =	specs[i].Name;
			string? best =	null;
			int bestNameLen =	int.MaxValue;

			foreach (var path in candidates)
			{
				var baseName =	Path.GetFileNameWithoutExtension(path);
				if (!baseName.StartsWith(thisName, StringComparison.OrdinalIgnoreCase))	continue;

				// Reject if a longer spec name also prefixes this file —
				// that file is "claimed" by the longer spec, not this one.
				// Example: "Combat Two.bin" starts with both "Combat" and
				// "Combat Two"; we want it to belong to "Combat Two".
				bool claimedByLonger =	false;
				foreach (var other in allNames)
				{
					if (other.Length <= thisName.Length)	continue;
					if (baseName.StartsWith(other, StringComparison.OrdinalIgnoreCase))
					{
						claimedByLonger =	true;
						break;
					}
				}
				if (claimedByLonger)	continue;

				// Among acceptable matches, shortest basename wins —
				// least-decorated filename, most likely the canonical dump.
				if (baseName.Length < bestNameLen)
				{
					best =	path;
					bestNameLen =	baseName.Length;
				}
			}

			if (best != null)
				found[i] =	new RomMatch(specs[i], RomResolution.FoundByName, best);
		}

		// ── Phase 3: remaining specs are NotFound ──────────────────────
		var results =	new RomMatch[specs.Count];
		for (int i = 0; i < specs.Count; i++)
			results[i] =	found[i] ?? new RomMatch(specs[i], RomResolution.NotFound, null);

		return results;
	}
}
