@echo off
REM code-explore installer for Windows CMD
REM Usage: curl -fsSL https://code.pioneers.ai/install.cmd -o install.cmd && install.cmd && del install.cmd
setlocal enabledelayedexpansion

set "PACKAGE=code-explore"

echo [code-explore installer]
echo.

REM --- Check Python ---
set "PYTHON_CMD="
for %%P in (python3 python py) do (
    if not defined PYTHON_CMD (
        %%P --version >nul 2>&1
        if !errorlevel! equ 0 (
            for /f "tokens=*" %%V in ('%%P -c "import sys; v=sys.version_info; print(f'{v.major}.{v.minor}')" 2^>nul') do (
                set "PYTHON_CMD=%%P"
                echo Found Python %%V
            )
        )
    )
)

if not defined PYTHON_CMD (
    echo ERROR: Python 3.11 or later is required but not found.
    echo.
    echo Install Python from: https://www.python.org/downloads/
    echo Or via winget:  winget install Python.Python.3.12
    exit /b 1
)

REM --- Install ---
where pipx >nul 2>&1
if %errorlevel% equ 0 (
    echo Installing %PACKAGE% with pipx...
    pipx install %PACKAGE%
) else (
    echo Installing %PACKAGE% with pip --user...
    %PYTHON_CMD% -m pip install --user %PACKAGE%
)

REM --- Verify ---
echo.
where cex >nul 2>&1
if %errorlevel% equ 0 (
    echo code-explore installed successfully!
    echo.
    echo   Quick start:
    echo     cex scan C:\Projects
    echo     cex index
    echo     cex search "web api"
    echo     cex config init
) else (
    echo Installed but 'cex' not found in PATH.
    echo You may need to restart your terminal.
)
echo.
