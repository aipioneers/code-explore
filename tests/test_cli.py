"""Tests for CLI commands."""

from typer.testing import CliRunner

from code_explore.cli.main import app

runner = CliRunner()


class TestCliBasics:
    def test_help(self):
        result = runner.invoke(app, ["--help"])
        assert result.exit_code == 0
        assert "knowledge base" in result.stdout.lower() or "code-explore" in result.stdout.lower()

    def test_no_args_shows_help(self):
        result = runner.invoke(app)
        assert result.exit_code in (0, 2)  # Typer returns 2 for no_args_is_help
        assert "Usage" in result.stdout or "usage" in result.stdout

    def test_scan_help(self):
        result = runner.invoke(app, ["scan", "--help"])
        assert result.exit_code == 0
        assert "scan" in result.stdout.lower()

    def test_search_help(self):
        result = runner.invoke(app, ["search", "--help"])
        assert result.exit_code == 0
        assert "query" in result.stdout.lower()

    def test_show_help(self):
        result = runner.invoke(app, ["show", "--help"])
        assert result.exit_code == 0

    def test_index_help(self):
        result = runner.invoke(app, ["index", "--help"])
        assert result.exit_code == 0

    def test_stats_help(self):
        result = runner.invoke(app, ["stats", "--help"])
        assert result.exit_code == 0

    def test_update_help(self):
        result = runner.invoke(app, ["update", "--help"])
        assert result.exit_code == 0

    def test_scan_nonexistent_path(self):
        result = runner.invoke(app, ["scan", "/nonexistent/path/12345"])
        assert result.exit_code == 1
        assert "error" in result.stdout.lower() or "not exist" in result.stdout.lower()
