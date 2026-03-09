"""Tests for CLI config commands."""

from pathlib import Path

import pytest
from typer.testing import CliRunner

from code_explore.cli.main import app
from code_explore.config import reset_config

runner = CliRunner()


class TestConfigShow:
    def test_exit_code(self, monkeypatch, tmp_path):
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        reset_config()
        result = runner.invoke(app, ["config", "show"])
        assert result.exit_code == 0

    def test_contains_all_settings(self, monkeypatch, tmp_path):
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        reset_config()
        result = runner.invoke(app, ["config", "show"])
        assert "Ollama URL" in result.stdout
        assert "Summary model" in result.stdout
        assert "Embedding model" in result.stdout
        assert "Embedding dimension" in result.stdout
        assert "Database path" in result.stdout
        assert "Vector store path" in result.stdout
        assert "Search fusion" in result.stdout
        assert "Result limit" in result.stdout

    def test_shows_default_source(self, monkeypatch, tmp_path):
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        reset_config()
        result = runner.invoke(app, ["config", "show"])
        assert "default" in result.stdout


class TestConfigInit:
    def test_creates_toml(self, monkeypatch, tmp_path):
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        reset_config()
        result = runner.invoke(app, ["config", "init"])
        assert result.exit_code == 0
        assert (tmp_path / "code-explore" / "config.toml").exists()

    def test_creates_yaml(self, monkeypatch, tmp_path):
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        reset_config()
        result = runner.invoke(app, ["config", "init", "--format", "yaml"])
        assert result.exit_code == 0
        assert (tmp_path / "code-explore" / "config.yaml").exists()

    def test_fails_if_exists(self, monkeypatch, tmp_path):
        config_dir = tmp_path / "code-explore"
        config_dir.mkdir()
        (config_dir / "config.toml").write_text("[ollama]\n")
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        reset_config()
        result = runner.invoke(app, ["config", "init"])
        assert result.exit_code == 1
        assert "already exists" in result.stdout

    def test_force_overwrites(self, monkeypatch, tmp_path):
        config_dir = tmp_path / "code-explore"
        config_dir.mkdir()
        (config_dir / "config.toml").write_text("[ollama]\n")
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        reset_config()
        result = runner.invoke(app, ["config", "init", "--force"])
        assert result.exit_code == 0
        content = (config_dir / "config.toml").read_text()
        assert "localhost" in content


class TestConfigReset:
    def test_deletes_config_file(self, monkeypatch, tmp_path):
        config_dir = tmp_path / "code-explore"
        config_dir.mkdir()
        config_file = config_dir / "config.toml"
        config_file.write_text("[ollama]\n")
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        reset_config()
        result = runner.invoke(app, ["config", "reset", "--yes"])
        assert result.exit_code == 0
        assert not config_file.exists()

    def test_no_file_to_reset(self, monkeypatch, tmp_path):
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        reset_config()
        result = runner.invoke(app, ["config", "reset", "--yes"])
        assert result.exit_code == 0
        assert "defaults" in result.stdout.lower()


class TestConfigPath:
    def test_shows_path(self, monkeypatch, tmp_path):
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        reset_config()
        result = runner.invoke(app, ["config", "path"])
        assert result.exit_code == 0
        assert "code-explore" in result.stdout

    def test_shows_existing_path(self, monkeypatch, tmp_path):
        config_dir = tmp_path / "code-explore"
        config_dir.mkdir()
        (config_dir / "config.toml").write_text("[ollama]\n")
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        reset_config()
        result = runner.invoke(app, ["config", "path"])
        assert result.exit_code == 0
        assert "config.toml" in result.stdout


class TestConfigHelp:
    def test_config_help(self):
        result = runner.invoke(app, ["config", "--help"])
        assert result.exit_code == 0
        assert "config" in result.stdout.lower()
