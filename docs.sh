#!/usr/bin/env bash
set -e

cd "$(dirname "$0")"

# Create virtual environment if it doesn't exist
if [ ! -d ".venv" ]; then
    echo "Creating virtual environment..."
    python3 -m venv .venv
fi

# Activate virtual environment
source .venv/bin/activate

# Install/update dependencies
echo "Installing dependencies..."
pip install -q -r requirements.txt

# Parse command
CMD="${1:-serve}"

case "$CMD" in
    serve)
        echo "Starting dev server at http://127.0.0.1:8000"
        mkdocs serve
        ;;
    build)
        echo "Building site..."
        mkdocs build --strict
        echo "Site built to ./site"
        ;;
    *)
        echo "Usage: $0 [serve|build]"
        echo "  serve  - Start local dev server (default)"
        echo "  build  - Build static site"
        exit 1
        ;;
esac
