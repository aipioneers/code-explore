"""Scan local directories to discover git repositories."""

import asyncio
from pathlib import Path

from rich.console import Console

console = Console()

SKIP_DIRS = frozenset({
    ".git",
    "node_modules",
    "venv",
    ".venv",
    "__pycache__",
    ".cache",
    "vendor",
    "dist",
    "build",
    "target",
})


async def scan_local_repos(
    root: str | Path,
    max_depth: int = 4,
) -> list[Path]:
    """Walk root directory recursively and return paths of discovered git repositories.

    Unlike a naive approach, this continues descending into subdirectories even
    after finding a .git directory, so nested repos (submodules, monorepo
    children) are discovered as independent projects.
    """
    root = Path(root).expanduser().resolve()
    if not root.is_dir():
        console.print(f"[red]Root path does not exist: {root}[/red]")
        return []

    console.print(f"[blue]Scanning[/blue] {root} (max depth {max_depth})")
    discovered: list[Path] = await asyncio.to_thread(_walk_for_repos, root, max_depth)
    console.print(f"[green]Found {len(discovered)} repositories[/green]")
    return discovered


def _walk_for_repos(root: Path, max_depth: int) -> list[Path]:
    repos: list[Path] = []
    stack: list[tuple[Path, int]] = [(root, 0)]

    while stack:
        current, depth = stack.pop()

        is_repo = (current / ".git").is_dir()
        if is_repo:
            repos.append(current)

        if depth >= max_depth:
            continue

        try:
            entries = sorted(current.iterdir())
        except PermissionError:
            continue

        for entry in entries:
            if entry.is_symlink() or not entry.is_dir():
                continue
            if entry.name in SKIP_DIRS:
                continue
            stack.append((entry, depth + 1))

        if len(repos) % 50 == 0 and repos:
            console.print(f"  [dim]...discovered {len(repos)} repos so far[/dim]")

    return repos
