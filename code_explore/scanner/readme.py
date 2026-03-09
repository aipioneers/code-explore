"""Read README files and list key files from repositories."""

import logging
from pathlib import Path

logger = logging.getLogger(__name__)

README_CANDIDATES = ("README.md", "readme.md", "README.rst", "README", "README.txt", "Readme.md")

SKIP_ENTRIES = frozenset({
    ".git",
    "node_modules",
    "__pycache__",
    ".DS_Store",
    ".gitignore",
    ".editorconfig",
    ".vscode",
    ".idea",
    ".mypy_cache",
    ".pytest_cache",
    ".ruff_cache",
    ".tox",
    ".eggs",
    ".env",
})

MAX_KEY_FILES = 50
MAX_README_CHARS = 1500


def read_readme(repo_path: Path) -> str | None:
    """Read the README file from a repository and return its first 1500 characters.

    Tries several common README filenames. Returns None if no README is found
    or if the file cannot be read.
    """
    for candidate in README_CANDIDATES:
        readme_path = repo_path / candidate
        try:
            if readme_path.is_file():
                text = readme_path.read_text(encoding="utf-8", errors="replace")
                return text[:MAX_README_CHARS] if text else None
        except (OSError, PermissionError) as e:
            logger.debug("Could not read %s: %s", readme_path, e)
            continue
    return None


def list_key_files(repo_path: Path) -> list[str]:
    """List important top-level files and directories in a repository.

    Skips common non-informative entries like .git, node_modules, etc.
    Returns up to 50 file/directory names (not full paths).
    """
    try:
        entries = sorted(repo_path.iterdir())
    except (OSError, PermissionError) as e:
        logger.debug("Could not list directory %s: %s", repo_path, e)
        return []

    names: list[str] = []
    for entry in entries:
        if entry.name in SKIP_ENTRIES:
            continue
        name = entry.name + "/" if entry.is_dir() and not entry.is_symlink() else entry.name
        names.append(name)
        if len(names) >= MAX_KEY_FILES:
            break

    return names
