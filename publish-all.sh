#!/usr/bin/env bash
# Build self-contained release archives of ChildhoodAdventure for both
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
PROJECT="$REPO_DIR/ChildhoodAdventure/ChildhoodAdventure.csproj"

want_linux=1
want_windows=1
case "${1:-both}" in
    linux)   want_windows=0 ;;
    win|windows) want_linux=0 ;;
    both|"") ;;
    *) echo "usage: $0 [linux|win|both]" >&2; exit 1 ;;
esac

publish_one() {
    local profile="$1"
    echo
    echo "=== Publishing $profile ==="
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
