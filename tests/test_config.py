"""Tests for configuration module."""

from pathlib import Path

import pytest
import tomllib

from code_explore.config import (
    AppConfig,
    ConfigSource,
    _discover_config_file,
    _load_env_vars,
    _load_file,
    _resolve_config,
    _validate_config,
    get_config,
    get_resolved_settings,
    reset_config,
    write_default_config,
)


class TestAppConfigDefaults:
    """Verify all defaults match current hardcoded values."""

    def test_ollama_url(self):
        cfg = AppConfig()
        assert cfg.ollama_url == "http://localhost:11434"

    def test_summary_model(self):
        cfg = AppConfig()
        assert cfg.summary_model == "llama3.2:3b"

    def test_embedding_model(self):
        cfg = AppConfig()
        assert cfg.embedding_model == "qwen3-embedding:8b"

    def test_embedding_dim(self):
        cfg = AppConfig()
        assert cfg.embedding_dim == 4096

    def test_db_path(self):
        cfg = AppConfig()
        assert cfg.db_path == Path.home() / ".code-explore" / "code-explore.db"

    def test_vector_path(self):
        cfg = AppConfig()
        assert cfg.vector_path == Path.home() / ".code-explore" / "vectors"

    def test_rrf_k(self):
        cfg = AppConfig()
        assert cfg.rrf_k == 60

    def test_result_limit(self):
        cfg = AppConfig()
        assert cfg.result_limit == 20


class TestLoadFile:
    def test_toml_loading(self, tmp_path):
        config_file = tmp_path / "config.toml"
        config_file.write_text(
            '[ollama]\nurl = "http://remote:11434"\nsummary_model = "mistral:7b"\n'
        )
        result = _load_file(config_file)
        assert result["ollama_url"] == "http://remote:11434"
        assert result["summary_model"] == "mistral:7b"

    def test_yaml_loading(self, tmp_path):
        config_file = tmp_path / "config.yaml"
        config_file.write_text(
            "ollama:\n  url: http://remote:11434\n  summary_model: mistral:7b\n"
        )
        result = _load_file(config_file)
        assert result["ollama_url"] == "http://remote:11434"
        assert result["summary_model"] == "mistral:7b"

    def test_malformed_toml_returns_empty(self, tmp_path):
        config_file = tmp_path / "config.toml"
        config_file.write_text("invalid toml {{{{")
        result = _load_file(config_file)
        assert result == {}

    def test_malformed_yaml_returns_empty(self, tmp_path):
        config_file = tmp_path / "config.yaml"
        config_file.write_text(":\n  :\n    - [invalid")
        result = _load_file(config_file)
        assert result == {}

    def test_unsupported_extension_returns_empty(self, tmp_path):
        config_file = tmp_path / "config.ini"
        config_file.write_text("[section]\nkey=value")
        result = _load_file(config_file)
        assert result == {}

    def test_partial_config(self, tmp_path):
        config_file = tmp_path / "config.toml"
        config_file.write_text('[search]\nrrf_k = 100\n')
        result = _load_file(config_file)
        assert result == {"rrf_k": 100}
        assert "ollama_url" not in result


class TestLoadEnvVars:
    def test_all_env_vars(self, monkeypatch):
        monkeypatch.setenv("CEX_OLLAMA_URL", "http://gpu:11434")
        monkeypatch.setenv("CEX_OLLAMA_SUMMARY_MODEL", "mistral:7b")
        monkeypatch.setenv("CEX_OLLAMA_EMBEDDING_MODEL", "nomic-embed-text")
        monkeypatch.setenv("CEX_OLLAMA_EMBEDDING_DIM", "768")
        monkeypatch.setenv("CEX_STORAGE_DB_PATH", "/tmp/test.db")
        monkeypatch.setenv("CEX_STORAGE_VECTOR_PATH", "/tmp/vectors")
        monkeypatch.setenv("CEX_SEARCH_RRF_K", "30")
        monkeypatch.setenv("CEX_SEARCH_RESULT_LIMIT", "50")

        result = _load_env_vars()
        assert result["ollama_url"] == "http://gpu:11434"
        assert result["summary_model"] == "mistral:7b"
        assert result["embedding_model"] == "nomic-embed-text"
        assert result["embedding_dim"] == 768
        assert result["db_path"] == "/tmp/test.db"
        assert result["vector_path"] == "/tmp/vectors"
        assert result["rrf_k"] == 30
        assert result["result_limit"] == 50

    def test_no_env_vars(self, monkeypatch):
        for var in ["CEX_OLLAMA_URL", "CEX_OLLAMA_SUMMARY_MODEL", "CEX_OLLAMA_EMBEDDING_MODEL",
                     "CEX_OLLAMA_EMBEDDING_DIM", "CEX_STORAGE_DB_PATH", "CEX_STORAGE_VECTOR_PATH",
                     "CEX_SEARCH_RRF_K", "CEX_SEARCH_RESULT_LIMIT"]:
            monkeypatch.delenv(var, raising=False)
        result = _load_env_vars()
        assert result == {}

    def test_invalid_int_ignored(self, monkeypatch):
        monkeypatch.setenv("CEX_OLLAMA_EMBEDDING_DIM", "not_a_number")
        result = _load_env_vars()
        assert "embedding_dim" not in result


class TestResolveConfig:
    def test_defaults_only(self):
        config, sources = _resolve_config({}, {})
        assert config.ollama_url == "http://localhost:11434"
        assert all(s == ConfigSource.DEFAULT for s in sources.values())

    def test_file_overrides_default(self):
        config, sources = _resolve_config({"summary_model": "mistral:7b"}, {})
        assert config.summary_model == "mistral:7b"
        assert sources["summary_model"] == ConfigSource.FILE
        assert sources["ollama_url"] == ConfigSource.DEFAULT

    def test_env_overrides_file(self):
        config, sources = _resolve_config(
            {"summary_model": "mistral:7b"},
            {"summary_model": "llama3:8b"},
        )
        assert config.summary_model == "llama3:8b"
        assert sources["summary_model"] == ConfigSource.ENV

    def test_source_tracking(self):
        config, sources = _resolve_config(
            {"rrf_k": 100},
            {"result_limit": 50},
        )
        assert sources["rrf_k"] == ConfigSource.FILE
        assert sources["result_limit"] == ConfigSource.ENV
        assert sources["ollama_url"] == ConfigSource.DEFAULT


class TestValidateConfig:
    def test_valid_config(self):
        _validate_config(AppConfig())  # should not raise

    def test_invalid_embedding_dim(self):
        with pytest.raises(ValueError, match="Embedding dimension"):
            _validate_config(AppConfig(embedding_dim=0))

    def test_negative_rrf_k(self):
        with pytest.raises(ValueError, match="fusion constant"):
            _validate_config(AppConfig(rrf_k=-1))

    def test_result_limit_zero(self):
        with pytest.raises(ValueError, match="Result limit"):
            _validate_config(AppConfig(result_limit=0))

    def test_result_limit_too_high(self):
        with pytest.raises(ValueError, match="Result limit"):
            _validate_config(AppConfig(result_limit=5000))

    def test_bad_url(self):
        with pytest.raises(ValueError, match="http://"):
            _validate_config(AppConfig(ollama_url="ftp://bad"))


class TestDiscoverConfigFile:
    def test_finds_toml(self, tmp_path):
        (tmp_path / "config.toml").write_text("[ollama]\n")
        result = _discover_config_file(tmp_path)
        assert result == tmp_path / "config.toml"

    def test_finds_yaml(self, tmp_path):
        (tmp_path / "config.yaml").write_text("ollama:\n  url: x\n")
        result = _discover_config_file(tmp_path)
        assert result == tmp_path / "config.yaml"

    def test_toml_priority_over_yaml(self, tmp_path):
        (tmp_path / "config.toml").write_text("[ollama]\n")
        (tmp_path / "config.yaml").write_text("ollama:\n  url: x\n")
        result = _discover_config_file(tmp_path)
        assert result == tmp_path / "config.toml"

    def test_no_config(self, tmp_path):
        result = _discover_config_file(tmp_path)
        assert result is None

    def test_missing_dir(self, tmp_path):
        result = _discover_config_file(tmp_path / "nonexistent")
        assert result is None


class TestWriteDefaultConfig:
    def test_toml_output(self, tmp_path):
        path = tmp_path / "config.toml"
        write_default_config(path, fmt="toml")
        assert path.exists()
        data = tomllib.loads(path.read_text())
        assert data["ollama"]["url"] == "http://localhost:11434"
        assert data["ollama"]["embedding_dim"] == 4096
        assert data["search"]["rrf_k"] == 60

    def test_yaml_output(self, tmp_path):
        import yaml

        path = tmp_path / "config.yaml"
        write_default_config(path, fmt="yaml")
        assert path.exists()
        data = yaml.safe_load(path.read_text())
        assert data["ollama"]["url"] == "http://localhost:11434"
        assert data["search"]["result_limit"] == 20

    def test_creates_parent_dirs(self, tmp_path):
        path = tmp_path / "deep" / "nested" / "config.toml"
        write_default_config(path, fmt="toml")
        assert path.exists()


class TestGetConfigSingleton:
    def test_returns_defaults_without_file(self, monkeypatch, tmp_path):
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        for var in ["CEX_OLLAMA_URL", "CEX_OLLAMA_SUMMARY_MODEL", "CEX_OLLAMA_EMBEDDING_MODEL",
                     "CEX_OLLAMA_EMBEDDING_DIM", "CEX_STORAGE_DB_PATH", "CEX_STORAGE_VECTOR_PATH",
                     "CEX_SEARCH_RRF_K", "CEX_SEARCH_RESULT_LIMIT"]:
            monkeypatch.delenv(var, raising=False)
        reset_config()
        cfg = get_config()
        assert cfg.ollama_url == "http://localhost:11434"
        assert cfg.summary_model == "llama3.2:3b"

    def test_reads_from_file(self, monkeypatch, tmp_path):
        config_dir = tmp_path / "code-explore"
        config_dir.mkdir()
        (config_dir / "config.toml").write_text(
            '[ollama]\nsummary_model = "phi3:mini"\n'
        )
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        for var in ["CEX_OLLAMA_URL", "CEX_OLLAMA_SUMMARY_MODEL"]:
            monkeypatch.delenv(var, raising=False)
        reset_config()
        cfg = get_config()
        assert cfg.summary_model == "phi3:mini"

    def test_env_overrides_file(self, monkeypatch, tmp_path):
        config_dir = tmp_path / "code-explore"
        config_dir.mkdir()
        (config_dir / "config.toml").write_text(
            '[ollama]\nsummary_model = "phi3:mini"\n'
        )
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        monkeypatch.setenv("CEX_OLLAMA_SUMMARY_MODEL", "gemma:2b")
        reset_config()
        cfg = get_config()
        assert cfg.summary_model == "gemma:2b"


class TestGetResolvedSettings:
    def test_returns_all_settings(self, monkeypatch, tmp_path):
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        for var in ["CEX_OLLAMA_URL", "CEX_OLLAMA_SUMMARY_MODEL", "CEX_OLLAMA_EMBEDDING_MODEL",
                     "CEX_OLLAMA_EMBEDDING_DIM", "CEX_STORAGE_DB_PATH", "CEX_STORAGE_VECTOR_PATH",
                     "CEX_SEARCH_RRF_K", "CEX_SEARCH_RESULT_LIMIT"]:
            monkeypatch.delenv(var, raising=False)
        reset_config()
        settings = get_resolved_settings()
        assert len(settings) == 8
        names = {s.name for s in settings}
        assert "Ollama URL" in names
        assert "Embedding model" in names
        assert "Result limit" in names

    def test_source_tracking(self, monkeypatch, tmp_path):
        config_dir = tmp_path / "code-explore"
        config_dir.mkdir()
        (config_dir / "config.toml").write_text('[search]\nrrf_k = 100\n')
        monkeypatch.setenv("XDG_CONFIG_HOME", str(tmp_path))
        monkeypatch.setenv("CEX_SEARCH_RESULT_LIMIT", "50")
        for var in ["CEX_OLLAMA_URL", "CEX_OLLAMA_SUMMARY_MODEL", "CEX_OLLAMA_EMBEDDING_MODEL",
                     "CEX_OLLAMA_EMBEDDING_DIM", "CEX_STORAGE_DB_PATH", "CEX_STORAGE_VECTOR_PATH",
                     "CEX_SEARCH_RRF_K"]:
            monkeypatch.delenv(var, raising=False)
        reset_config()
        settings = get_resolved_settings()
        by_name = {s.name: s for s in settings}
        assert by_name["Search fusion (k)"].source == ConfigSource.FILE
        assert by_name["Result limit"].source == ConfigSource.ENV
        assert by_name["Ollama URL"].source == ConfigSource.DEFAULT
