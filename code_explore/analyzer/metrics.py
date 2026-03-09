"""Calculate code quality metrics for a project."""

from pathlib import Path

from code_explore.models import QualityMetrics

SKIP_DIRS: set[str] = {
    "node_modules", ".git", ".svn", ".hg", "__pycache__", ".mypy_cache",
    ".pytest_cache", ".tox", ".nox", ".venv", "venv", "env", ".env",
    "dist", "build", "out", "target", ".next", ".nuxt", ".output",
    "vendor", "third_party", ".gradle", ".idea", ".vscode",
    ".vs", "bin", "obj", ".cache", "coverage", ".terraform",
}

BINARY_EXTENSIONS: set[str] = {
    ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".ico", ".webp",
    ".mp3", ".mp4", ".wav", ".avi", ".mov",
    ".zip", ".tar", ".gz", ".bz2", ".xz", ".rar", ".7z",
    ".exe", ".dll", ".so", ".dylib", ".o", ".obj",
    ".class", ".jar", ".war",
    ".pyc", ".pyo", ".whl",
    ".pdf", ".doc", ".docx", ".xls", ".xlsx",
    ".ttf", ".otf", ".woff", ".woff2",
    ".db", ".sqlite", ".sqlite3",
    ".lock",
}

TEST_INDICATORS: set[str] = {
    "test", "tests", "spec", "specs", "__tests__", "test_", "_test",
    "testing", "e2e", "integration_tests", "unit_tests",
}

CI_FILES: set[str] = {
    ".github/workflows",
    ".gitlab-ci.yml",
    ".gitlab-ci.yaml",
    "Jenkinsfile",
    ".circleci",
    ".travis.yml",
    ".travis.yaml",
    "azure-pipelines.yml",
    "bitbucket-pipelines.yml",
    ".buildkite",
    "Taskfile.yml",
    ".drone.yml",
}


def _should_skip_dir(name: str) -> bool:
    return name in SKIP_DIRS or name.startswith(".")


def _count_lines(path: Path) -> int:
    try:
        return sum(1 for _ in path.open("r", encoding="utf-8", errors="replace"))
    except (OSError, ValueError):
        return 0


def _file_size(path: Path) -> int:
    try:
        return path.stat().st_size
    except OSError:
        return 0


def calculate_metrics(project_path: str | Path) -> QualityMetrics:
    root = Path(project_path)
    if not root.is_dir():
        return QualityMetrics()

    total_files = 0
    total_lines = 0
    max_size = 0
    sizes: list[int] = []
    has_tests = False
    has_ci = False
    has_docs = False
    has_readme = False
    has_license = False
    has_gitignore = False

    for item in root.iterdir():
        name_lower = item.name.lower()
        if name_lower.startswith("readme"):
            has_readme = True
        if name_lower.startswith("license") or name_lower.startswith("licence"):
            has_license = True
        if name_lower == ".gitignore":
            has_gitignore = True
        if name_lower in ("docs", "doc", "documentation"):
            if item.is_dir():
                has_docs = True

    for ci_indicator in CI_FILES:
        ci_path = root / ci_indicator
        if ci_path.exists():
            has_ci = True
            break

    for item in root.rglob("*"):
        if not item.is_file():
            continue

        rel_parts = item.relative_to(root).parts
        if any(_should_skip_dir(p) for p in rel_parts[:-1]):
            continue

        if item.suffix.lower() in BINARY_EXTENSIONS:
            continue

        if not has_tests:
            for part in rel_parts:
                part_lower = part.lower().replace(".", "_")
                if part_lower in TEST_INDICATORS or any(
                    part_lower.startswith(t) or part_lower.endswith(t)
                    for t in ("test_", "_test", "spec_", "_spec")
                ):
                    has_tests = True
                    break

        size = _file_size(item)
        lines = _count_lines(item)

        total_files += 1
        total_lines += lines
        sizes.append(size)
        if size > max_size:
            max_size = size

    avg_size = round(sum(sizes) / len(sizes), 1) if sizes else 0.0

    return QualityMetrics(
        total_files=total_files,
        total_lines=total_lines,
        avg_file_size=avg_size,
        max_file_size=max_size,
        has_tests=has_tests,
        has_ci=has_ci,
        has_docs=has_docs,
        has_readme=has_readme,
        has_license=has_license,
        has_gitignore=has_gitignore,
    )
