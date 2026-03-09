"""Configuration management for Code Explore.

Loads settings from: built-in defaults → config file → environment variables.
Config file supports TOML and YAML formats, auto-detected by file extension.
"""

import logging
import os
import tomllib
from enum import Enum
from pathlib import Path
from typing import Any

from pydantic import BaseModel, Field

logger = logging.getLogger(__name__)


class ConfigSource(str, Enum):
    """Where a configuration value came from."""

    DEFAULT = "default"
    FILE = "file"
    ENV = "env"
    CLI = "cli"


class ResolvedSetting(BaseModel):
    """A single setting with its resolved value and source."""

    name: str
    key: str
    value: str
    source: ConfigSource


class AppConfig(BaseModel):
    """Application configuration with all settings and their defaults."""

    ollama_url: str = Field(default="http://localhost:11434")
    summary_model: str = Field(default="llama3.2:3b")
    embedding_model: str = Field(default="qwen3-embedding:8b")
    embedding_dim: int = Field(default=4096)
    db_path: Path = Field(default_factory=lambda: Path.home() / ".code-explore" / "code-explore.db")
    vector_path: Path = Field(default_factory=lambda: Path.home() / ".code-explore" / "vectors")
    rrf_k: int = Field(default=60)
    result_limit: int = Field(default=20)


# Mapping: env var name → (config file section.key, AppConfig field name)
_ENV_MAP: dict[str, tuple[str, str]] = {
    "CEX_OLLAMA_URL": ("ollama.url", "ollama_url"),
    "CEX_OLLAMA_SUMMARY_MODEL": ("ollama.summary_model", "summary_model"),
    "CEX_OLLAMA_EMBEDDING_MODEL": ("ollama.embedding_model", "embedding_model"),
    "CEX_OLLAMA_EMBEDDING_DIM": ("ollama.embedding_dim", "embedding_dim"),
    "CEX_STORAGE_DB_PATH": ("storage.db_path", "db_path"),
    "CEX_STORAGE_VECTOR_PATH": ("storage.vector_path", "vector_path"),
    "CEX_SEARCH_RRF_K": ("search.rrf_k", "rrf_k"),
    "CEX_SEARCH_RESULT_LIMIT": ("search.result_limit", "result_limit"),
}

# Mapping: AppConfig field name → (config file section, key within section)
_FIELD_TO_FILE_KEY: dict[str, tuple[str, str]] = {
    "ollama_url": ("ollama", "url"),
    "summary_model": ("ollama", "summary_model"),
    "embedding_model": ("ollama", "embedding_model"),
    "embedding_dim": ("ollama", "embedding_dim"),
    "db_path": ("storage", "db_path"),
    "vector_path": ("storage", "vector_path"),
    "rrf_k": ("search", "rrf_k"),
    "result_limit": ("search", "result_limit"),
}

# Human-readable display names
_FIELD_DISPLAY_NAMES: dict[str, str] = {
    "ollama_url": "Ollama URL",
    "summary_model": "Summary model",
    "embedding_model": "Embedding model",
    "embedding_dim": "Embedding dimension",
    "db_path": "Database path",
    "vector_path": "Vector store path",
    "rrf_k": "Search fusion (k)",
    "result_limit": "Result limit",
}


def _get_config_dir() -> Path:
    """Return the configuration directory, respecting XDG_CONFIG_HOME."""
    xdg = os.environ.get("XDG_CONFIG_HOME")
    if xdg:
        return Path(xdg) / "code-explore"
    return Path.home() / ".config" / "code-explore"


def _discover_config_file(config_dir: Path | None = None) -> Path | None:
    """Search for config file in priority order: .toml → .yaml → .yml."""
    d = config_dir or _get_config_dir()
    candidates = [d / "config.toml", d / "config.yaml", d / "config.yml"]
    found: list[Path] = [p for p in candidates if p.is_file()]

    if not found:
        return None

    if len(found) > 1:
        logger.warning(
            "Multiple config files found. Using %s, ignoring: %s",
            found[0],
            ", ".join(str(p) for p in found[1:]),
        )

    return found[0]


def _load_file(path: Path) -> dict[str, Any]:
    """Load a config file (TOML or YAML) and return a flat field dict."""
    try:
        raw = path.read_bytes()
    except OSError as e:
        logger.warning("Cannot read config file %s: %s", path, e)
        return {}

    suffix = path.suffix.lower()
    nested: dict[str, Any] = {}

    if suffix == ".toml":
        try:
            nested = tomllib.loads(raw.decode("utf-8"))
        except (tomllib.TOMLDecodeError, UnicodeDecodeError) as e:
            logger.warning("Failed to parse TOML config %s: %s. Using defaults.", path, e)
            return {}
    elif suffix in (".yaml", ".yml"):
        try:
            import yaml

            nested = yaml.safe_load(raw.decode("utf-8")) or {}
        except Exception as e:
            logger.warning("Failed to parse YAML config %s: %s. Using defaults.", path, e)
            return {}
    else:
        logger.warning("Unsupported config file format: %s. Expected .toml, .yaml, or .yml.", path)
        return {}

    # Flatten nested dict to field names
    flat: dict[str, Any] = {}
    for field_name, (section, key) in _FIELD_TO_FILE_KEY.items():
        if section in nested and isinstance(nested[section], dict):
            if key in nested[section]:
                flat[field_name] = nested[section][key]

    return flat


def _load_env_vars() -> dict[str, Any]:
    """Load configuration overrides from CEX_* environment variables."""
    result: dict[str, Any] = {}
    int_fields = {"embedding_dim", "rrf_k", "result_limit"}

    for env_var, (_file_key, field_name) in _ENV_MAP.items():
        value = os.environ.get(env_var)
        if value is not None:
            if field_name in int_fields:
                try:
                    result[field_name] = int(value)
                except ValueError:
                    logger.warning(
                        "Invalid integer value for %s=%s. Ignoring.", env_var, value
                    )
            else:
                result[field_name] = value

    return result


def _resolve_config(
    file_dict: dict[str, Any],
    env_dict: dict[str, Any],
) -> tuple[AppConfig, dict[str, ConfigSource]]:
    """Layer defaults → file → env and track provenance."""
    sources: dict[str, ConfigSource] = {
        field: ConfigSource.DEFAULT for field in _FIELD_DISPLAY_NAMES
    }

    merged: dict[str, Any] = {}

    # Layer file values
    for key, value in file_dict.items():
        merged[key] = value
        sources[key] = ConfigSource.FILE

    # Layer env values (override file)
    for key, value in env_dict.items():
        merged[key] = value
        sources[key] = ConfigSource.ENV

    config = AppConfig(**merged) if merged else AppConfig()
    return config, sources


def _validate_config(config: AppConfig) -> None:
    """Validate configuration values. Raises ValueError on invalid settings."""
    errors: list[str] = []

    if config.embedding_dim <= 0:
        errors.append("Embedding dimension must be a positive integer.")
    if config.rrf_k <= 0:
        errors.append("Search fusion constant must be a positive integer.")
    if not 1 <= config.result_limit <= 1000:
        errors.append("Result limit must be between 1 and 1000.")
    if not config.ollama_url.startswith(("http://", "https://")):
        errors.append(f"Ollama URL must start with http:// or https://, got: {config.ollama_url}")

    if errors:
        raise ValueError("Configuration errors:\n" + "\n".join(f"  - {e}" for e in errors))


def write_default_config(path: Path, fmt: str = "toml") -> None:
    """Write a config file with all default values."""
    path.parent.mkdir(parents=True, exist_ok=True)
    defaults = AppConfig()

    if fmt == "toml":
        import tomli_w

        data = {
            "ollama": {
                "url": defaults.ollama_url,
                "summary_model": defaults.summary_model,
                "embedding_model": defaults.embedding_model,
                "embedding_dim": defaults.embedding_dim,
            },
            "storage": {
                "db_path": str(defaults.db_path),
                "vector_path": str(defaults.vector_path),
            },
            "search": {
                "rrf_k": defaults.rrf_k,
                "result_limit": defaults.result_limit,
            },
        }
        path.write_bytes(tomli_w.dumps(data).encode("utf-8"))
    elif fmt in ("yaml", "yml"):
        import yaml

        data = {
            "ollama": {
                "url": defaults.ollama_url,
                "summary_model": defaults.summary_model,
                "embedding_model": defaults.embedding_model,
                "embedding_dim": defaults.embedding_dim,
            },
            "storage": {
                "db_path": str(defaults.db_path),
                "vector_path": str(defaults.vector_path),
            },
            "search": {
                "rrf_k": defaults.rrf_k,
                "result_limit": defaults.result_limit,
            },
        }
        path.write_text(yaml.dump(data, default_flow_style=False, sort_keys=False))


# --- Singleton ---

_cached_config: AppConfig | None = None
_cached_sources: dict[str, ConfigSource] | None = None
_cached_config_path: Path | None = None


def reset_config() -> None:
    """Clear the cached configuration. Used for testing and after config reset."""
    global _cached_config, _cached_sources, _cached_config_path
    _cached_config = None
    _cached_sources = None
    _cached_config_path = None


def get_config() -> AppConfig:
    """Load and return the application configuration (cached after first call)."""
    global _cached_config, _cached_sources, _cached_config_path

    if _cached_config is not None:
        return _cached_config

    file_dict: dict[str, Any] = {}
    config_path = _discover_config_file()
    _cached_config_path = config_path

    if config_path:
        file_dict = _load_file(config_path)

    env_dict = _load_env_vars()
    config, sources = _resolve_config(file_dict, env_dict)

    try:
        _validate_config(config)
    except ValueError as e:
        logger.warning("%s", e)

    _cached_config = config
    _cached_sources = sources
    return config


def get_resolved_settings() -> list[ResolvedSetting]:
    """Return all settings with their values and sources for display."""
    config = get_config()
    sources = _cached_sources or {f: ConfigSource.DEFAULT for f in _FIELD_DISPLAY_NAMES}

    settings = []
    for field_name, display_name in _FIELD_DISPLAY_NAMES.items():
        value = getattr(config, field_name)
        section, key = _FIELD_TO_FILE_KEY[field_name]
        settings.append(
            ResolvedSetting(
                name=display_name,
                key=f"{section}.{key}",
                value=str(value),
                source=sources.get(field_name, ConfigSource.DEFAULT),
            )
        )

    return settings


def get_config_path() -> Path | None:
    """Return the path to the active config file, or None."""
    get_config()  # ensure discovery has run
    return _cached_config_path
