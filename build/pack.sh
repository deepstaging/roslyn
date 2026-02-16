#!/bin/bash
# SPDX-FileCopyrightText: 2024-present Deepstaging
# SPDX-License-Identifier: RPL-1.5
# Build and package Deepstaging.Roslyn NuGet packages
#
# Usage:
#   ./build/pack.sh                    # Build Release with dev version suffix
#   ./build/pack.sh --configuration Debug
#   ./build/pack.sh --version-suffix dev.42
#   ./build/pack.sh --no-version-suffix  # Pack without version suffix (release)
#
# Output: ../../artifacts/packages/
#   - Deepstaging.Roslyn.{version}.nupkg
#   - Deepstaging.Roslyn.Testing.{version}.nupkg
#
# After packing, automatically updates Deepstaging.Roslyn.Versions.props
# with the new version so consumer repos pick it up on next restore.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

# Default values
CONFIGURATION="Release"
OUTPUT_DIR="$REPO_ROOT/../../artifacts/packages"
UPDATE_VERSIONS=false

# Version suffix: local uses timestamp, CI uses git commit count
if [ -n "${CI:-}" ]; then
    VERSION_SUFFIX="dev.$(git -C "$REPO_ROOT" rev-list --count HEAD)"
else
    VERSION_SUFFIX="local.$(date -u +%Y%m%d%H%M%S)"
fi

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--configuration)
            CONFIGURATION="$2"
            shift 2
            ;;
        --version-suffix)
            VERSION_SUFFIX="$2"
            shift 2
            ;;
        --no-version-suffix)
            VERSION_SUFFIX=""
            shift
            ;;
        --update-versions)
            UPDATE_VERSIONS=true
            shift
            ;;
        --ci)
            VERSION_SUFFIX="dev.$(git -C "$REPO_ROOT" rev-list --count HEAD)"
            UPDATE_VERSIONS=true
            shift
            ;;
        -o|--output)
            OUTPUT_DIR="$2"
            shift 2
            ;;
        -h|--help)
            echo "Usage: $0 [options]"
            echo ""
            echo "Options:"
            echo "  -c, --configuration <config>  Build configuration (default: Release)"
            echo "  --version-suffix <suffix>     Version suffix (default: local.TIMESTAMP or dev.N in CI)"
            echo "  --no-version-suffix           Pack without version suffix (for release)"
            echo "  --update-versions             Update committed Versions.props (used by CI)"
            echo "  --ci                          Shorthand for dev.N suffix + --update-versions"
            echo "  -o, --output <dir>            Output directory (default: ../../artifacts/packages)"
            echo "  -h, --help                    Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Build common pack args
build_pack_args() {
    local project="$1"
    local args=(
        "$project"
        --configuration "$CONFIGURATION"
        --output "$OUTPUT_DIR"
        --no-build
    )
    if [[ -n "$VERSION_SUFFIX" ]]; then
        args+=(--version-suffix "$VERSION_SUFFIX")
    fi
    echo "${args[@]}"
}

echo "Building Deepstaging.Roslyn ($CONFIGURATION)..."
dotnet build "$REPO_ROOT/Deepstaging.Roslyn.slnx" --configuration "$CONFIGURATION"

echo ""
echo "Syncing package icons..."
"$SCRIPT_DIR/sync-icons.sh" || echo "Warning: Icon sync failed (icons may be missing from packages)"

echo ""
echo "Creating output directory..."
mkdir -p "$OUTPUT_DIR"

echo ""
echo "Packing Deepstaging.Roslyn..."
dotnet pack $(build_pack_args "$REPO_ROOT/src/Deepstaging.Roslyn/Deepstaging.Roslyn.csproj")

echo ""
echo "Packing Deepstaging.Roslyn.LanguageExt..."
dotnet pack $(build_pack_args "$REPO_ROOT/src/Deepstaging.Roslyn.LanguageExt/Deepstaging.Roslyn.LanguageExt.csproj")

echo ""
echo "Packing Deepstaging.Roslyn.Testing..."
dotnet pack $(build_pack_args "$REPO_ROOT/src/Deepstaging.Roslyn.Testing/Deepstaging.Roslyn.Testing.csproj")

echo ""
echo "Packages created in: $OUTPUT_DIR"

# Clean up old package versions (keep only last 3 per package)
for prefix in $(ls "$OUTPUT_DIR"/*.nupkg 2>/dev/null | xargs -n1 basename | sed 's/\.[0-9][0-9]*\..*//' | sort -u); do
    ls -t "$OUTPUT_DIR/$prefix".[0-9]*.nupkg 2>/dev/null | tail -n +4 | xargs rm -f
done

# Update centralized version props file
VERSIONS_FILE="$REPO_ROOT/Deepstaging.Roslyn.Versions.props"
VERSIONS_LOCAL_FILE="$REPO_ROOT/Deepstaging.Roslyn.Versions.local.props"

# Extract the full version from a generated .nupkg filename
NUPKG=$(ls "$OUTPUT_DIR"/Deepstaging.Roslyn.1*.nupkg 2>/dev/null | tail -1)
if [[ -n "$NUPKG" ]]; then
    PACK_VERSION=$(basename "$NUPKG" | sed 's/^Deepstaging\.Roslyn\.\(.*\)\.nupkg$/\1/')

    if [[ "$UPDATE_VERSIONS" == "true" ]]; then
        # CI: update the committed Versions.props
        echo ""
        echo "Updating $VERSIONS_FILE with version $PACK_VERSION..."

        cat > "$VERSIONS_FILE" << EOF
<!-- SPDX-FileCopyrightText: 2024-present Deepstaging -->
<!-- SPDX-License-Identifier: RPL-1.5 -->
<!--
  Deepstaging.Roslyn.Versions.props

  Centralized version definitions for Deepstaging.Roslyn NuGet packages.
  This file is automatically updated by CI after publishing to nuget.org.

  Consumer repos import this conditionally in their Directory.Packages.props:
    <Import Project="../roslyn/Deepstaging.Roslyn.Versions.props" Condition="Exists('...')" />

  For local development, pack.sh writes Deepstaging.Roslyn.Versions.local.props
  (gitignored) which overrides the version below with a local.TIMESTAMP suffix.
-->
<Project>
  <PropertyGroup>
    <DeepstagingRoslynVersion>$PACK_VERSION</DeepstagingRoslynVersion>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="Deepstaging.Roslyn" Version="\$(DeepstagingRoslynVersion)" />
    <PackageVersion Include="Deepstaging.Roslyn.LanguageExt" Version="\$(DeepstagingRoslynVersion)" />
    <PackageVersion Include="Deepstaging.Roslyn.Testing" Version="\$(DeepstagingRoslynVersion)" />
  </ItemGroup>
</Project>
EOF

        echo "Updated Deepstaging.Roslyn version to $PACK_VERSION"

        # Update template Directory.Packages.props with the new version
        TEMPLATE_PACKAGES="$REPO_ROOT/templates/templates/roslynkit/Directory.Packages.props"
        if [[ -f "$TEMPLATE_PACKAGES" ]]; then
            sed -i.bak "s|Include=\"Deepstaging\.Roslyn\" Version=\"[^\"]*\"|Include=\"Deepstaging.Roslyn\" Version=\"$PACK_VERSION\"|g" "$TEMPLATE_PACKAGES"
            sed -i.bak "s|Include=\"Deepstaging\.Roslyn\.LanguageExt\" Version=\"[^\"]*\"|Include=\"Deepstaging.Roslyn.LanguageExt\" Version=\"$PACK_VERSION\"|g" "$TEMPLATE_PACKAGES"
            sed -i.bak "s|Include=\"Deepstaging\.Roslyn\.Testing\" Version=\"[^\"]*\"|Include=\"Deepstaging.Roslyn.Testing\" Version=\"$PACK_VERSION\"|g" "$TEMPLATE_PACKAGES"
            rm -f "$TEMPLATE_PACKAGES.bak"
            echo "Updated template Directory.Packages.props to $PACK_VERSION"
        fi
    else
        # Local: write the gitignored local override
        echo ""
        echo "Writing $VERSIONS_LOCAL_FILE with version $PACK_VERSION..."

        cat > "$VERSIONS_LOCAL_FILE" << EOF
<!-- Generated by pack.sh — overrides Versions.props for local development -->
<!-- This file is gitignored. Do not commit. -->
<Project>
  <PropertyGroup>
    <DeepstagingRoslynVersion>$PACK_VERSION</DeepstagingRoslynVersion>
  </PropertyGroup>
</Project>
EOF

        echo "Local Deepstaging.Roslyn version set to $PACK_VERSION"
    fi
else
    echo ""
    echo "Warning: Could not determine package version from .nupkg files"
fi
