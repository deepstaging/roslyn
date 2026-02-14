#!/bin/bash
# SPDX-FileCopyrightText: 2024-present Deepstaging
# SPDX-License-Identifier: RPL-1.5
# Download package icons from the deepstaging/assets repo (dist branch)
#
# Usage:
#   ./build/sync-icons.sh              # Download latest icons
#   ./build/sync-icons.sh --check      # Verify icons exist (for CI)

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

BASE_URL="https://raw.githubusercontent.com/deepstaging/assets/dist/icons"

# Parallel arrays: icon name and project directory
ICON_NAMES=(
  "Deepstaging.Roslyn"
  "Deepstaging.Roslyn.Scriban"
  "Deepstaging.Roslyn.Workspace"
  "Deepstaging.Roslyn.Testing"
)
ICON_DIRS=(
  "src/Deepstaging.Roslyn"
  "src/Deepstaging.Roslyn.Scriban"
  "src/Deepstaging.Roslyn.Workspace"
  "src/Deepstaging.Roslyn.Testing"
)

CHECK_ONLY=false
if [[ "${1:-}" == "--check" ]]; then
  CHECK_ONLY=true
fi

FAILED=0

for i in "${!ICON_NAMES[@]}"; do
  name="${ICON_NAMES[$i]}"
  dir="${ICON_DIRS[$i]}"
  dest="$REPO_ROOT/$dir/icon.png"
  url="$BASE_URL/${name}.png"

  if [[ "$CHECK_ONLY" == true ]]; then
    if [[ ! -f "$dest" ]]; then
      echo "MISSING: $dest"
      FAILED=1
    else
      echo "OK: $dir/icon.png"
    fi
  else
    echo "Downloading ${name}.png -> $dir/icon.png"
    curl -fsSL "$url" -o "$dest" || {
      echo "  FAILED to download $url"
      FAILED=1
    }
  fi
done

if [[ $FAILED -ne 0 ]]; then
  echo ""
  echo "Some icons are missing. Run './build/sync-icons.sh' to download them."
  exit 1
fi

echo "All icons synced."
