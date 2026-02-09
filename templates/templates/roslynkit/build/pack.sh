#!/bin/bash
# SPDX-FileCopyrightText: 2024-present Deepstaging
# SPDX-License-Identifier: RPL-1.5
# Build and package Deepstaging.RoslynKit NuGet packages
#
# Usage:
#   ./build/pack.sh                    # Build Release with dev version suffix
#   ./build/pack.sh --configuration Debug
#   ./build/pack.sh --version-suffix dev-20260202
#   ./build/pack.sh --no-version-suffix  # Pack without version suffix (release)
#
# Output: artifacts/packages/
#   - Deepstaging.RoslynKit.{version}.nupkg
#   - Deepstaging.RoslynKit.Generators.{version}.nupkg
#   - Deepstaging.RoslynKit.Analyzers.{version}.nupkg
#   - Deepstaging.RoslynKit.CodeFixes.{version}.nupkg

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

# Default values
CONFIGURATION="Release"
VERSION_SUFFIX="dev-$(date +%Y%m%d%H%M%S)"
OUTPUT_DIR="$REPO_ROOT/artifacts/packages"

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
        -o|--output)
            OUTPUT_DIR="$2"
            shift 2
            ;;
        -h|--help)
            echo "Usage: $0 [options]"
            echo ""
            echo "Options:"
            echo "  -c, --configuration <config>  Build configuration (default: Release)"
            echo "  --version-suffix <suffix>     Version suffix (default: dev-YYYYMMDDHHMMSS)"
            echo "  --no-version-suffix           Pack without version suffix (for release)"
            echo "  -o, --output <dir>            Output directory (default: artifacts/packages)"
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

echo "Building Deepstaging.RoslynKit ($CONFIGURATION)..."
HUSKY=0 dotnet build "$REPO_ROOT/Deepstaging.RoslynKit.slnx" --configuration "$CONFIGURATION"

echo ""
echo "Creating output directory..."
mkdir -p "$OUTPUT_DIR"

echo ""
echo "Packing Deepstaging.RoslynKit..."
dotnet pack $(build_pack_args "$REPO_ROOT/src/Deepstaging.RoslynKit/Deepstaging.RoslynKit.csproj")
dotnet pack $(build_pack_args "$REPO_ROOT/src/Deepstaging.RoslynKit.Generators/Deepstaging.RoslynKit.Generators.csproj")
dotnet pack $(build_pack_args "$REPO_ROOT/src/Deepstaging.RoslynKit.Analyzers/Deepstaging.RoslynKit.Analyzers.csproj")
dotnet pack $(build_pack_args "$REPO_ROOT/src/Deepstaging.RoslynKit.CodeFixes/Deepstaging.RoslynKit.CodeFixes.csproj")

echo ""
echo "Packages created in: $OUTPUT_DIR"
ls -la "$OUTPUT_DIR"/Deepstaging.RoslynKit*.nupkg 2>/dev/null || echo "No .nupkg files found"
