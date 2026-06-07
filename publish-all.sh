#!/usr/bin/env bash
# Build self-contained release archives of ShabbysGameSanctuary for both
# Linux-x64 and Windows-x64. Drops the resulting archives in ./dist/.
#
# Usage:  ./publish-all.sh           # both platforms
#         ./publish-all.sh linux     # linux only
#         ./publish-all.sh win       # windows only
#
# Stella's libretro core is built natively (linux .so) when the Linux
# profile runs. For the Windows profile, if a prebuilt stella_libretro.dll
# is present under stella/src/os/libretro/, it gets bundled; otherwise the
# Windows archive ships without it and Combat appears greyed out in-game.

set -euo pipefail

REPO_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT="$REPO_DIR/MainGame/MainGame.csproj"
PROPS="$REPO_DIR/Directory.Build.props"

want_linux=1
want_windows=1
case "${1:-both}" in
    linux)   want_windows=0 ;;
    win|windows) want_linux=0 ;;
    both|"") ;;
    *) echo "usage: $0 [linux|win|both]" >&2; exit 1 ;;
esac

# ── Auto-bump the patch segment of <Version> in Directory.Build.props ──
#
# Pull the current X.Y.Z, increment Z by 1, write it back, and feed the
# bumped value into the rest of the script so every "Publishing v…"
# banner and the resulting .exe / setup.exe metadata reflect it.
#
# We bump BEFORE publish so the artifact that lands in dist/ carries
# the freshly-bumped number. A publish that fails mid-flight will still
# have advanced the version — that's intentional, since the rebuild
# attempt should also produce a unique stamp (no "v1.0.2" twice).
#
# Bump only the patch segment. Major/minor bumps remain a manual edit
# to Directory.Build.props, since those carry meaning (breaking change,
# feature release) that the script can't infer.
current_version="$(grep -oE '<Version>[^<]+' "$PROPS" | head -n1 | sed 's/<Version>//' || true)"
if [[ -z "$current_version" ]]; then
    echo "ERROR: could not find <Version> in $PROPS" >&2
    exit 1
fi
if [[ ! "$current_version" =~ ^([0-9]+)\.([0-9]+)\.([0-9]+)$ ]]; then
    echo "ERROR: Version '$current_version' is not Major.Minor.Patch (three numeric parts)." >&2
    echo "       Fix $PROPS by hand before re-running." >&2
    exit 1
fi
major="${BASH_REMATCH[1]}"
minor="${BASH_REMATCH[2]}"
patch="${BASH_REMATCH[3]}"
VERSION="${major}.${minor}.$((patch + 1))"

# Atomic write: render to a temp file in the same directory (so mv is a
# rename, not a copy across filesystems) and swap it in. A sed -i would
# also work, but this pattern is portable to BSD sed without the -i.bak
# dance.
tmp="$(mktemp "${PROPS}.XXXXXX")"
sed -E "s|<Version>${current_version}</Version>|<Version>${VERSION}</Version>|" "$PROPS" > "$tmp"
mv "$tmp" "$PROPS"

echo "Bumped version: $current_version → $VERSION"
echo "  (Directory.Build.props updated)"

publish_one() {
    local profile="$1"
    echo
    echo "=== Publishing $profile (v$VERSION) ==="
    dotnet publish "$PROJECT" \
        -p:PublishProfile="$profile" \
        -c Release \
        --nologo
}

if [[ $want_linux -eq 1 ]];   then publish_one linux-x64; fi
if [[ $want_windows -eq 1 ]]; then publish_one win-x64;   fi

echo
echo "Artifacts:"
ls -lh "$REPO_DIR"/dist/*.tar.gz "$REPO_DIR"/dist/*.zip "$REPO_DIR"/dist/*.exe 2>/dev/null || true
echo
echo "Published version: v$VERSION"
