#!/usr/bin/env bash
# code-explore installer
# Usage: curl -fsSL https://get.code-explore.dev | bash
set -euo pipefail

PACKAGE="code-explore"
MIN_PYTHON="3.11"
BOLD='\033[1m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
RED='\033[0;31m'
RESET='\033[0m'

info()  { echo -e "${BOLD}${GREEN}▸${RESET} $1"; }
warn()  { echo -e "${BOLD}${YELLOW}▸${RESET} $1"; }
error() { echo -e "${BOLD}${RED}✗${RESET} $1" >&2; }
done_msg() { echo -e "${BOLD}${GREEN}✓${RESET} $1"; }

# --- Check Python ---
find_python() {
    for cmd in python3 python; do
        if command -v "$cmd" &>/dev/null; then
            local ver
            ver=$("$cmd" -c "import sys; print(f'{sys.version_info.major}.{sys.version_info.minor}')" 2>/dev/null || echo "0.0")
            if printf '%s\n%s\n' "$MIN_PYTHON" "$ver" | sort -V -C 2>/dev/null; then
                echo "$cmd"
                return 0
            fi
        fi
    done
    return 1
}

PYTHON=$(find_python) || {
    error "Python $MIN_PYTHON or later is required but not found."
    echo ""
    echo "Install Python:"
    if [[ "$OSTYPE" == darwin* ]]; then
        echo "  brew install python@3.12"
    elif command -v apt-get &>/dev/null; then
        echo "  sudo apt-get install python3.12"
    elif command -v dnf &>/dev/null; then
        echo "  sudo dnf install python3.12"
    else
        echo "  https://www.python.org/downloads/"
    fi
    exit 1
}

PYTHON_VER=$("$PYTHON" -c "import sys; print(f'{sys.version_info.major}.{sys.version_info.minor}.{sys.version_info.micro}')")
info "Found Python $PYTHON_VER"

# --- Install ---
install_with_pipx() {
    info "Installing $PACKAGE with pipx (isolated environment)..."
    pipx install "$PACKAGE" 2>&1
}

install_with_pip() {
    info "Installing $PACKAGE with pip..."
    "$PYTHON" -m pip install --user "$PACKAGE" 2>&1
}

install_with_uv() {
    info "Installing $PACKAGE with uv..."
    uv tool install "$PACKAGE" 2>&1
}

# Prefer: uv > pipx > pip
if command -v uv &>/dev/null; then
    install_with_uv || {
        warn "uv install failed, falling back to pip..."
        install_with_pip
    }
elif command -v pipx &>/dev/null; then
    install_with_pipx || {
        warn "pipx install failed, falling back to pip..."
        install_with_pip
    }
else
    warn "pipx not found. Installing with pip --user."
    warn "For better isolation, install pipx: $PYTHON -m pip install --user pipx"
    install_with_pip
fi

# --- Verify ---
echo ""
if command -v cex &>/dev/null; then
    VER=$(cex --version 2>/dev/null || echo "installed")
    done_msg "code-explore installed successfully!"
    echo ""
    echo -e "  ${BOLD}Quick start:${RESET}"
    echo "    cex scan ~/projects      # scan your repositories"
    echo "    cex index                 # generate AI summaries"
    echo "    cex search \"web api\"      # search across projects"
    echo "    cex config init           # create config file"
    echo ""
elif command -v code-explore &>/dev/null; then
    done_msg "code-explore installed successfully!"
    echo ""
    echo -e "  Run: ${BOLD}code-explore --help${RESET}"
    echo ""
else
    warn "Installed but 'cex' not found in PATH."
    echo ""
    echo "  You may need to add the install location to your PATH:"
    echo "    export PATH=\"\$HOME/.local/bin:\$PATH\""
    echo ""
    echo "  Then run: cex --help"
fi
