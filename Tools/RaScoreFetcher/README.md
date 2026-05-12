# RaScoreFetcher

Offline content-pipeline tool. Pulls the unobfuscated leaderboard / rich-presence
data from RetroAchievements for each ROM in our curated spec list, picks the best
score-reading formula, and writes a JSON data file that the game ships with as an
embedded resource. Run once when the spec list changes or when you want fresher
data — **not** at game runtime.

## Why this exists

The public RA API (the one keyed by `?y=APIKEY`) obfuscates score formulas — the
`MemAddr` and `RichPresencePatch` fields come back as 32-char hashes, not as the
actual rcheevos expressions. The unobfuscated content is only served from the
authenticated `dorequest.php?r=patch` endpoint that RA-aware emulators use,
which needs your **login token** in addition to the web API key.

## What you need

Set these env vars before running:

| Var | Where to get it |
|---|---|
| `RA_USER` | Your RetroAchievements username |
| `RA_TOKEN` | Account settings → "Keys" → "Web API Connect Token" (the long alphanumeric one — not your password and not your API key) |
| `RA_API_KEY` | Account settings → "Keys" → "Web API Key" |

You also need a folder containing the ROMs whose SHA-256 hashes appear in
[specs.json](specs.json). The tool walks that folder, computes SHA-256 to match
each spec, then computes MD5 (the hash RA uses for Atari 2600 lookups) to find
the matching RA game.

## Usage

```bash
export RA_USER=YourUser
export RA_TOKEN=...
export RA_API_KEY=...

dotnet run --project Tools/RaScoreFetcher -- \
    --rom-root /path/to/your/atari/roms \
    --specs Tools/RaScoreFetcher/specs.json \
    --out ChildhoodAdventure/AtariScores.json
```

Flags:

| Flag | Default | Description |
|---|---|---|
| `--rom-root` | (required) | Folder to walk for ROM matching. Recursive. |
| `--specs` | `specs.json` | The (name, sha256) input list. |
| `--out` | `AtariScores.json` | Where to write the output. |
| `--delay` | `200` | Milliseconds between RA patch requests. Be polite. |
| `-v` / `--verbose` | off | Print per-file matches as we go. |

## Output shape

```json
{
  "System": "Atari 2600",
  "FetchedAt": "2026-05-12T19:34:00.0000000Z",
  "Games": [
    {
      "Name": "Pitfall",
      "Sha256": "c56c99d9...",
      "Md5": "...",
      "RaGameId": 11191,
      "RaGameTitle": "Pitfall!",
      "ScoreFormula": "M:0xH0080*65536+0xH0081*256+0xH0082",
      "ScoreFormat": "SCORE",
      "Source": "leaderboard #2573 \"High Score\""
    },
    ...
  ]
}
```

The `Source` field documents where each formula came from so you can audit /
override. Entries with no matching ROM file or no recognised RA game still
appear in the output (with most fields null) so the runtime knows which specs
are intentionally unscoreable.
