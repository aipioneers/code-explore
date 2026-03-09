# code-explore installer for Windows
# Usage: irm https://get.code-explore.dev/install.ps1 | iex
$ErrorActionPreference = "Stop"

$Package = "code-explore"
$MinPython = [version]"3.11"

function Write-Info($msg) { Write-Host "▸ $msg" -ForegroundColor Green }
function Write-Warn($msg) { Write-Host "▸ $msg" -ForegroundColor Yellow }
function Write-Err($msg) { Write-Host "✗ $msg" -ForegroundColor Red }
function Write-Done($msg) { Write-Host "✓ $msg" -ForegroundColor Green }

# --- Check Python ---
$PythonCmd = $null
foreach ($cmd in @("python3", "python", "py")) {
    try {
        $ver = & $cmd -c "import sys; print(f'{sys.version_info.major}.{sys.version_info.minor}.{sys.version_info.micro}')" 2>$null
        if ($ver) {
            $parsed = [version]$ver
            if ($parsed -ge $MinPython) {
                $PythonCmd = $cmd
                Write-Info "Found Python $ver"
                break
            }
        }
    } catch { }
}

if (-not $PythonCmd) {
    Write-Err "Python $MinPython or later is required but not found."
    Write-Host ""
    Write-Host "Install Python from: https://www.python.org/downloads/"
    Write-Host "Or via winget:  winget install Python.Python.3.12"
    Write-Host "Or via scoop:   scoop install python"
    exit 1
}

# --- Install ---
$Installed = $false

# Try uv first
try {
    $null = Get-Command uv -ErrorAction Stop
    Write-Info "Installing $Package with uv..."
    & uv tool install $Package
    $Installed = $true
} catch { }

# Try pipx
if (-not $Installed) {
    try {
        $null = Get-Command pipx -ErrorAction Stop
        Write-Info "Installing $Package with pipx (isolated environment)..."
        & pipx install $Package
        $Installed = $true
    } catch { }
}

# Fall back to pip
if (-not $Installed) {
    Write-Warn "pipx not found. Installing with pip --user."
    Write-Warn "For better isolation: $PythonCmd -m pip install --user pipx"
    & $PythonCmd -m pip install --user $Package
}

# --- Verify ---
Write-Host ""
try {
    $null = Get-Command cex -ErrorAction Stop
    Write-Done "code-explore installed successfully!"
    Write-Host ""
    Write-Host "  Quick start:" -ForegroundColor White
    Write-Host "    cex scan C:\Projects       # scan your repositories"
    Write-Host "    cex index                   # generate AI summaries"
    Write-Host '    cex search "web api"        # search across projects'
    Write-Host "    cex config init             # create config file"
    Write-Host ""
} catch {
    Write-Warn "Installed but 'cex' not found in PATH."
    Write-Host ""
    Write-Host "  You may need to restart your terminal or add the install location to PATH."
    Write-Host "  Then run: cex --help"
    Write-Host ""
}
