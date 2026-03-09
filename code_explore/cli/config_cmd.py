"""CLI commands for configuration management."""

import typer
from rich.console import Console
from rich.table import Table

from code_explore.config import (
    _get_config_dir,
    get_config_path,
    get_resolved_settings,
    reset_config,
    write_default_config,
    _discover_config_file,
)

config_app = typer.Typer(
    name="config",
    help="Manage code-explore configuration.",
    no_args_is_help=True,
)
console = Console()


@config_app.command()
def show() -> None:
    """Display all current settings with values and sources."""
    settings = get_resolved_settings()
    config_path = get_config_path()

    table = Table(title="Code Explore Configuration")
    table.add_column("Setting", style="cyan", no_wrap=True)
    table.add_column("Value", style="white")
    table.add_column("Source", style="green")

    for s in settings:
        table.add_row(s.name, s.value, s.source.value)

    table.add_section()
    path_display = str(config_path) if config_path else "(no config file)"
    table.add_row("Config file", path_display, "")

    console.print(table)


@config_app.command()
def init(
    fmt: str = typer.Option("toml", "--format", "-f", help="Config format: toml or yaml"),
    force: bool = typer.Option(False, "--force", help="Overwrite existing config file"),
) -> None:
    """Create a configuration file with all default values."""
    config_dir = _get_config_dir()
    ext = "yaml" if fmt in ("yaml", "yml") else "toml"
    path = config_dir / f"config.{ext}"

    if path.exists() and not force:
        console.print(
            f"[yellow]Config file already exists:[/yellow] {path}\n"
            f"Use [bold]--force[/bold] to overwrite."
        )
        raise typer.Exit(1)

    write_default_config(path, fmt=ext)
    console.print(f"[green]Created configuration file:[/green] {path}")


@config_app.command()
def reset(
    yes: bool = typer.Option(False, "--yes", "-y", help="Skip confirmation prompt"),
) -> None:
    """Delete the configuration file and revert to defaults."""
    config_path = _discover_config_file()

    if not config_path:
        console.print("[yellow]No configuration file found. Already using defaults.[/yellow]")
        raise typer.Exit(0)

    if not yes:
        confirm = typer.confirm(f"Delete {config_path} and revert to defaults?")
        if not confirm:
            console.print("[dim]Cancelled.[/dim]")
            raise typer.Exit(0)

    config_path.unlink()
    reset_config()
    console.print(f"[green]Reset configuration to defaults.[/green] Removed: {config_path}")


@config_app.command()
def path() -> None:
    """Print the path to the active configuration file."""
    config_path = _discover_config_file()
    if config_path:
        console.print(str(config_path))
    else:
        default_path = _get_config_dir() / "config.toml"
        console.print(f"{default_path} [dim](not created yet)[/dim]")
