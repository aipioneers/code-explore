"""Parse dependency files to extract project dependencies."""

import json
import re
from pathlib import Path

from code_explore.models import DependencyInfo


def _read_file(path: Path) -> str:
    try:
        return path.read_text(encoding="utf-8", errors="replace")
    except OSError:
        return ""


def _parse_package_json(path: Path) -> list[DependencyInfo]:
    content = _read_file(path)
    if not content:
        return []
    try:
        data = json.loads(content)
    except (json.JSONDecodeError, ValueError):
        return []

    deps: list[DependencyInfo] = []
    for name, version in (data.get("dependencies") or {}).items():
        deps.append(DependencyInfo(name=name, version=version, dev=False, source="package.json"))
    for name, version in (data.get("devDependencies") or {}).items():
        deps.append(DependencyInfo(name=name, version=version, dev=True, source="package.json"))
    return deps


def _parse_requirements_txt(path: Path) -> list[DependencyInfo]:
    content = _read_file(path)
    if not content:
        return []

    deps: list[DependencyInfo] = []
    for line in content.splitlines():
        line = line.strip()
        if not line or line.startswith("#") or line.startswith("-"):
            continue
        match = re.match(r"^([A-Za-z0-9_][A-Za-z0-9._-]*)\s*(?:([><=!~]+.*))?", line)
        if match:
            name = match.group(1)
            version = match.group(2)
            if version:
                version = version.strip()
            deps.append(DependencyInfo(
                name=name, version=version or None, dev=False, source=path.name,
            ))
    return deps


def _parse_pyproject_toml(path: Path) -> list[DependencyInfo]:
    content = _read_file(path)
    if not content:
        return []

    deps: list[DependencyInfo] = []

    in_deps = False
    in_dev_deps = False
    for line in content.splitlines():
        stripped = line.strip()

        if stripped.startswith("["):
            in_deps = stripped in (
                "[project]",
                "[tool.poetry.dependencies]",
            )
            in_dev_deps = stripped in (
                "[tool.poetry.dev-dependencies]",
                "[tool.poetry.group.dev.dependencies]",
            )
            if stripped == "[project]":
                in_deps = False
            continue

        if stripped == "dependencies = [":
            in_deps = True
            continue

        if in_deps and stripped.startswith("]"):
            in_deps = False
            continue

        if in_deps:
            match = re.match(r'"([A-Za-z0-9_][A-Za-z0-9._-]*)\s*(?:([><=!~]+[^"]*))?"', stripped)
            if match:
                deps.append(DependencyInfo(
                    name=match.group(1),
                    version=match.group(2) or None,
                    dev=False,
                    source="pyproject.toml",
                ))
                continue

            toml_match = re.match(
                r'([A-Za-z0-9_][A-Za-z0-9._-]*)\s*=\s*["\{]([^"}\n]*)', stripped,
            )
            if toml_match:
                name = toml_match.group(1)
                if name == "python":
                    continue
                version_str = toml_match.group(2).strip().rstrip('"')
                deps.append(DependencyInfo(
                    name=name,
                    version=version_str or None,
                    dev=in_dev_deps,
                    source="pyproject.toml",
                ))

        if in_dev_deps:
            toml_match = re.match(
                r'([A-Za-z0-9_][A-Za-z0-9._-]*)\s*=\s*["\{]([^"}\n]*)', stripped,
            )
            if toml_match:
                name = toml_match.group(1)
                if name == "python":
                    continue
                version_str = toml_match.group(2).strip().rstrip('"')
                deps.append(DependencyInfo(
                    name=name,
                    version=version_str or None,
                    dev=True,
                    source="pyproject.toml",
                ))

    return deps


def _parse_setup_py(path: Path) -> list[DependencyInfo]:
    content = _read_file(path)
    if not content:
        return []

    deps: list[DependencyInfo] = []
    for match in re.finditer(r"install_requires\s*=\s*\[(.*?)]", content, re.DOTALL):
        block = match.group(1)
        for pkg in re.findall(r"['\"]([A-Za-z0-9_][A-Za-z0-9._-]*(?:\s*[><=!~]+[^'\"]*)?)['\"]", block):
            parts = re.match(r"([A-Za-z0-9_][A-Za-z0-9._-]*)\s*(.*)", pkg)
            if parts:
                deps.append(DependencyInfo(
                    name=parts.group(1),
                    version=parts.group(2).strip() or None,
                    dev=False,
                    source="setup.py",
                ))
    return deps


def _parse_cargo_toml(path: Path) -> list[DependencyInfo]:
    content = _read_file(path)
    if not content:
        return []

    deps: list[DependencyInfo] = []
    in_deps = False
    in_dev_deps = False

    for line in content.splitlines():
        stripped = line.strip()
        if stripped.startswith("["):
            in_deps = stripped == "[dependencies]"
            in_dev_deps = stripped in ("[dev-dependencies]", "[build-dependencies]")
            continue

        if in_deps or in_dev_deps:
            match = re.match(r'([A-Za-z0-9_][A-Za-z0-9_-]*)\s*=\s*"([^"]*)"', stripped)
            if match:
                deps.append(DependencyInfo(
                    name=match.group(1),
                    version=match.group(2),
                    dev=in_dev_deps,
                    source="Cargo.toml",
                ))
                continue
            match = re.match(r'([A-Za-z0-9_][A-Za-z0-9_-]*)\s*=\s*\{.*?version\s*=\s*"([^"]*)"', stripped)
            if match:
                deps.append(DependencyInfo(
                    name=match.group(1),
                    version=match.group(2),
                    dev=in_dev_deps,
                    source="Cargo.toml",
                ))

    return deps


def _parse_go_mod(path: Path) -> list[DependencyInfo]:
    content = _read_file(path)
    if not content:
        return []

    deps: list[DependencyInfo] = []
    in_require = False

    for line in content.splitlines():
        stripped = line.strip()
        if stripped.startswith("require ("):
            in_require = True
            continue
        if stripped == ")":
            in_require = False
            continue
        if stripped.startswith("require "):
            parts = stripped[len("require "):].strip().split()
            if len(parts) >= 2:
                deps.append(DependencyInfo(
                    name=parts[0], version=parts[1], dev=False, source="go.mod",
                ))
            continue
        if in_require:
            parts = stripped.split()
            if len(parts) >= 2 and not parts[0].startswith("//"):
                deps.append(DependencyInfo(
                    name=parts[0], version=parts[1], dev=False, source="go.mod",
                ))

    return deps


def _parse_pom_xml(path: Path) -> list[DependencyInfo]:
    content = _read_file(path)
    if not content:
        return []

    deps: list[DependencyInfo] = []
    for match in re.finditer(
        r"<dependency>\s*"
        r"<groupId>([^<]+)</groupId>\s*"
        r"<artifactId>([^<]+)</artifactId>\s*"
        r"(?:<version>([^<]+)</version>)?",
        content,
    ):
        group_id = match.group(1).strip()
        artifact_id = match.group(2).strip()
        version = match.group(3).strip() if match.group(3) else None
        scope_match = re.search(r"<scope>([^<]+)</scope>", content[match.start():match.start() + 500])
        is_dev = scope_match and scope_match.group(1).strip() in ("test", "provided")
        deps.append(DependencyInfo(
            name=f"{group_id}:{artifact_id}",
            version=version,
            dev=bool(is_dev),
            source="pom.xml",
        ))

    return deps


def _parse_gemfile(path: Path) -> list[DependencyInfo]:
    content = _read_file(path)
    if not content:
        return []

    deps: list[DependencyInfo] = []
    in_dev_group = False

    for line in content.splitlines():
        stripped = line.strip()
        if stripped.startswith("group :development") or stripped.startswith("group :test"):
            in_dev_group = True
            continue
        if stripped == "end" and in_dev_group:
            in_dev_group = False
            continue

        match = re.match(r"gem\s+['\"]([^'\"]+)['\"](?:\s*,\s*['\"]([^'\"]*)['\"])?", stripped)
        if match:
            deps.append(DependencyInfo(
                name=match.group(1),
                version=match.group(2) or None,
                dev=in_dev_group,
                source="Gemfile",
            ))

    return deps


def _parse_composer_json(path: Path) -> list[DependencyInfo]:
    content = _read_file(path)
    if not content:
        return []
    try:
        data = json.loads(content)
    except (json.JSONDecodeError, ValueError):
        return []

    deps: list[DependencyInfo] = []
    for name, version in (data.get("require") or {}).items():
        if name == "php" or name.startswith("ext-"):
            continue
        deps.append(DependencyInfo(name=name, version=version, dev=False, source="composer.json"))
    for name, version in (data.get("require-dev") or {}).items():
        deps.append(DependencyInfo(name=name, version=version, dev=True, source="composer.json"))
    return deps


_PARSERS: dict[str, callable] = {
    "package.json": _parse_package_json,
    "requirements.txt": _parse_requirements_txt,
    "requirements-dev.txt": _parse_requirements_txt,
    "requirements_dev.txt": _parse_requirements_txt,
    "pyproject.toml": _parse_pyproject_toml,
    "setup.py": _parse_setup_py,
    "Cargo.toml": _parse_cargo_toml,
    "go.mod": _parse_go_mod,
    "pom.xml": _parse_pom_xml,
    "Gemfile": _parse_gemfile,
    "composer.json": _parse_composer_json,
}


def detect_dependencies(project_path: str | Path) -> list[DependencyInfo]:
    root = Path(project_path)
    if not root.is_dir():
        return []

    all_deps: list[DependencyInfo] = []

    for filename, parser in _PARSERS.items():
        dep_file = root / filename
        if dep_file.is_file():
            all_deps.extend(parser(dep_file))

    return all_deps
