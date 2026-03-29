"""Integration tests for the plugin contract and loader modules."""

import json
from pathlib import Path

import pytest

from code_explore.plugins.contract import (
    PLUGIN_JSON_SCHEMA,
    PluginInput,
    PluginOutput,
    parse_output,
    serialize_input,
    validate_plugin_json,
)
from code_explore.plugins.loader import (
    PLUGIN_DIR,
    get_plugin,
    install_from_tarball,
    list_installed,
    remove_plugin,
)


# ---------------------------------------------------------------------------
# Contract: validate_plugin_json
# ---------------------------------------------------------------------------


class TestValidatePluginJson:
    """Tests for the manifest validation function."""

    def test_valid_manifest_passes(self):
        manifest = {"name": "my-plugin", "version": "1.0.0", "entry": "python main.py"}
        errors = validate_plugin_json(manifest)
        assert errors == []

    def test_valid_manifest_with_optional_fields(self):
        manifest = {
            "name": "my-plugin",
            "version": "1.0.0",
            "entry": "python main.py",
            "display_name": "My Plugin",
            "description": "A test plugin",
            "tags": ["test", "example"],
            "min_cexp_version": "0.5.0",
        }
        errors = validate_plugin_json(manifest)
        assert errors == []

    def test_missing_name_fails(self):
        manifest = {"version": "1.0.0", "entry": "python main.py"}
        errors = validate_plugin_json(manifest)
        assert len(errors) == 1
        assert "name" in errors[0]

    def test_missing_entry_fails(self):
        manifest = {"name": "my-plugin", "version": "1.0.0"}
        errors = validate_plugin_json(manifest)
        assert len(errors) == 1
        assert "entry" in errors[0]

    def test_missing_version_fails(self):
        manifest = {"name": "my-plugin", "entry": "python main.py"}
        errors = validate_plugin_json(manifest)
        assert len(errors) == 1
        assert "version" in errors[0]

    def test_missing_all_required_fields(self):
        errors = validate_plugin_json({})
        assert len(errors) == 3
        field_names = " ".join(errors)
        assert "name" in field_names
        assert "version" in field_names
        assert "entry" in field_names

    def test_non_dict_input(self):
        errors = validate_plugin_json("not a dict")
        assert errors == ["plugin.json must be a JSON object"]

    def test_empty_name_fails(self):
        manifest = {"name": "   ", "version": "1.0.0", "entry": "python main.py"}
        errors = validate_plugin_json(manifest)
        assert len(errors) == 1
        assert "empty" in errors[0]

    def test_non_string_name_fails(self):
        manifest = {"name": 123, "version": "1.0.0", "entry": "python main.py"}
        errors = validate_plugin_json(manifest)
        assert len(errors) == 1
        assert "string" in errors[0]

    def test_invalid_tags_type(self):
        manifest = {
            "name": "my-plugin",
            "version": "1.0.0",
            "entry": "python main.py",
            "tags": "not-a-list",
        }
        errors = validate_plugin_json(manifest)
        assert len(errors) == 1
        assert "tags" in errors[0]

    def test_invalid_tag_items(self):
        manifest = {
            "name": "my-plugin",
            "version": "1.0.0",
            "entry": "python main.py",
            "tags": ["ok", 42],
        }
        errors = validate_plugin_json(manifest)
        assert len(errors) == 1
        assert "tags" in errors[0]

    def test_invalid_optional_string_fields(self):
        manifest = {
            "name": "my-plugin",
            "version": "1.0.0",
            "entry": "python main.py",
            "display_name": 999,
            "description": False,
            "min_cexp_version": [],
        }
        errors = validate_plugin_json(manifest)
        assert len(errors) == 3


# ---------------------------------------------------------------------------
# Contract: PLUGIN_JSON_SCHEMA
# ---------------------------------------------------------------------------


class TestPluginJsonSchema:
    """Tests for the JSON schema constant."""

    def test_schema_has_required_fields(self):
        assert "required" in PLUGIN_JSON_SCHEMA
        assert set(PLUGIN_JSON_SCHEMA["required"]) == {"name", "version", "entry"}

    def test_schema_type_is_object(self):
        assert PLUGIN_JSON_SCHEMA["type"] == "object"

    def test_schema_properties_include_all_fields(self):
        expected = {
            "name",
            "version",
            "entry",
            "display_name",
            "description",
            "tags",
            "min_cexp_version",
        }
        assert set(PLUGIN_JSON_SCHEMA["properties"].keys()) == expected

    def test_schema_is_json_serializable(self):
        """The schema itself should round-trip through JSON without errors."""
        raw = json.dumps(PLUGIN_JSON_SCHEMA)
        restored = json.loads(raw)
        assert restored == PLUGIN_JSON_SCHEMA


# ---------------------------------------------------------------------------
# Contract: PluginInput
# ---------------------------------------------------------------------------


class TestPluginInput:
    """Tests for the PluginInput dataclass."""

    def test_create_minimal(self):
        pi = PluginInput(project_path="/tmp/proj", project_name="proj")
        assert pi.project_path == "/tmp/proj"
        assert pi.project_name == "proj"
        assert pi.languages == {}
        assert pi.dependencies == []
        assert pi.tags == []
        assert pi.scan_id == ""

    def test_create_full(self):
        pi = PluginInput(
            project_path="/code/app",
            project_name="app",
            languages={"Python": 80.0, "JS": 20.0},
            dependencies=["fastapi", "httpx"],
            tags=["api", "web"],
            scan_id="scan-001",
        )
        assert pi.languages == {"Python": 80.0, "JS": 20.0}
        assert pi.dependencies == ["fastapi", "httpx"]
        assert pi.tags == ["api", "web"]
        assert pi.scan_id == "scan-001"

    def test_serialize_input_roundtrip(self):
        pi = PluginInput(
            project_path="/code/app",
            project_name="app",
            languages={"Python": 90.0},
            dependencies=["typer"],
            tags=["cli"],
            scan_id="s1",
        )
        raw = serialize_input(pi)
        data = json.loads(raw)
        assert data["project_path"] == "/code/app"
        assert data["project_name"] == "app"
        assert data["languages"] == {"Python": 90.0}
        assert data["dependencies"] == ["typer"]
        assert data["tags"] == ["cli"]
        assert data["scan_id"] == "s1"


# ---------------------------------------------------------------------------
# Contract: PluginOutput
# ---------------------------------------------------------------------------


class TestPluginOutput:
    """Tests for the PluginOutput dataclass."""

    def test_create_minimal(self):
        po = PluginOutput(plugin_name="test", plugin_version="1.0.0")
        assert po.plugin_name == "test"
        assert po.plugin_version == "1.0.0"
        assert po.results == []
        assert po.metrics == {}
        assert po.errors == []

    def test_create_full(self):
        po = PluginOutput(
            plugin_name="analyzer",
            plugin_version="2.1.0",
            results=[{"score": 95}],
            metrics={"duration_ms": 120},
            errors=["minor warning"],
        )
        assert po.results == [{"score": 95}]
        assert po.metrics == {"duration_ms": 120}
        assert po.errors == ["minor warning"]

    def test_parse_output_valid(self):
        raw = json.dumps(
            {
                "plugin_name": "lint",
                "plugin_version": "0.2.0",
                "results": [{"file": "a.py", "issues": 0}],
                "metrics": {"files_checked": 1},
                "errors": [],
            }
        )
        po = parse_output(raw)
        assert po.plugin_name == "lint"
        assert po.plugin_version == "0.2.0"
        assert len(po.results) == 1
        assert po.errors == []

    def test_parse_output_defaults_for_missing_fields(self):
        raw = json.dumps({})
        po = parse_output(raw)
        assert po.plugin_name == "unknown"
        assert po.plugin_version == "0.0.0"
        assert po.results == []
        assert po.metrics == {}
        assert po.errors == []

    def test_parse_output_invalid_json_raises(self):
        with pytest.raises(ValueError, match="not valid JSON"):
            parse_output("not json {{{")

    def test_parse_output_non_object_raises(self):
        with pytest.raises(ValueError, match="must be a JSON object"):
            parse_output(json.dumps([1, 2, 3]))


# ---------------------------------------------------------------------------
# Loader: PLUGIN_DIR
# ---------------------------------------------------------------------------


class TestPluginDir:
    """Tests for the default plugin directory path."""

    def test_default_path(self):
        assert PLUGIN_DIR == Path.home() / ".pioneers" / "plugins"

    def test_path_is_absolute(self):
        assert PLUGIN_DIR.is_absolute()


# ---------------------------------------------------------------------------
# Loader: list_installed / get_plugin / remove_plugin
# ---------------------------------------------------------------------------


class TestLoaderFilesystem:
    """Integration tests exercising the loader against a temp filesystem."""

    @pytest.fixture
    def plugin_dir(self, tmp_path, monkeypatch):
        """Redirect PLUGIN_DIR to a temporary directory."""
        import code_explore.plugins.loader as loader_mod

        monkeypatch.setattr(loader_mod, "PLUGIN_DIR", tmp_path)
        return tmp_path

    @pytest.fixture
    def installed_plugin(self, plugin_dir):
        """Create a valid plugin on disk and return its name."""
        name = "example-plugin"
        plugin_path = plugin_dir / name
        plugin_path.mkdir()
        manifest = {
            "name": name,
            "version": "1.0.0",
            "entry": "python main.py",
            "description": "An example plugin",
        }
        (plugin_path / "plugin.json").write_text(json.dumps(manifest), encoding="utf-8")
        return name

    def test_list_installed_empty(self, plugin_dir):
        assert list_installed() == []

    def test_list_installed_finds_valid_plugin(self, plugin_dir, installed_plugin):
        plugins = list_installed()
        assert len(plugins) == 1
        assert plugins[0]["name"] == installed_plugin
        assert "_install_path" in plugins[0]

    def test_list_installed_skips_invalid_plugin(self, plugin_dir):
        bad = plugin_dir / "bad-plugin"
        bad.mkdir()
        (bad / "plugin.json").write_text("{}", encoding="utf-8")
        assert list_installed() == []

    def test_list_installed_skips_non_directories(self, plugin_dir):
        (plugin_dir / "stray-file.txt").write_text("hi")
        assert list_installed() == []

    def test_get_plugin_found(self, plugin_dir, installed_plugin):
        meta = get_plugin(installed_plugin)
        assert meta is not None
        assert meta["name"] == installed_plugin
        assert meta["version"] == "1.0.0"
        assert "_install_path" in meta

    def test_get_plugin_not_found(self, plugin_dir):
        assert get_plugin("nonexistent") is None

    def test_remove_plugin_success(self, plugin_dir, installed_plugin):
        assert remove_plugin(installed_plugin) is True
        assert not (plugin_dir / installed_plugin).exists()

    def test_remove_plugin_not_found(self, plugin_dir):
        assert remove_plugin("nonexistent") is False

    def test_install_from_tarball(self, plugin_dir, tmp_path):
        """Create a tarball containing a valid plugin and install it."""
        import tarfile

        name = "tarball-plugin"
        src = tmp_path / "src" / name
        src.mkdir(parents=True)
        manifest = {"name": name, "version": "0.1.0", "entry": "node index.js"}
        (src / "plugin.json").write_text(json.dumps(manifest), encoding="utf-8")
        (src / "index.js").write_text("console.log('hello');")

        tarball = tmp_path / f"{name}.tar.gz"
        with tarfile.open(tarball, "w:gz") as tar:
            for item in src.iterdir():
                tar.add(item, arcname=item.name)

        install_from_tarball(tarball, name)

        meta = get_plugin(name)
        assert meta is not None
        assert meta["name"] == name
        assert meta["version"] == "0.1.0"
