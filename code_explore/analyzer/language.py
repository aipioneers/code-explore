"""Detect programming languages in a project directory."""

from pathlib import Path

from code_explore.models import LanguageInfo

EXTENSION_MAP: dict[str, str] = {
    ".py": "Python",
    ".pyw": "Python",
    ".pyi": "Python",
    ".pyx": "Python",
    ".js": "JavaScript",
    ".mjs": "JavaScript",
    ".cjs": "JavaScript",
    ".jsx": "JavaScript",
    ".ts": "TypeScript",
    ".tsx": "TypeScript",
    ".mts": "TypeScript",
    ".cts": "TypeScript",
    ".java": "Java",
    ".kt": "Kotlin",
    ".kts": "Kotlin",
    ".scala": "Scala",
    ".groovy": "Groovy",
    ".gradle": "Groovy",
    ".c": "C",
    ".h": "C",
    ".cpp": "C++",
    ".cc": "C++",
    ".cxx": "C++",
    ".hpp": "C++",
    ".hh": "C++",
    ".hxx": "C++",
    ".cs": "C#",
    ".fs": "F#",
    ".fsx": "F#",
    ".vb": "Visual Basic",
    ".go": "Go",
    ".rs": "Rust",
    ".rb": "Ruby",
    ".erb": "Ruby",
    ".rake": "Ruby",
    ".php": "PHP",
    ".swift": "Swift",
    ".m": "Objective-C",
    ".mm": "Objective-C",
    ".r": "R",
    ".R": "R",
    ".rmd": "R",
    ".jl": "Julia",
    ".lua": "Lua",
    ".pl": "Perl",
    ".pm": "Perl",
    ".ex": "Elixir",
    ".exs": "Elixir",
    ".erl": "Erlang",
    ".hrl": "Erlang",
    ".hs": "Haskell",
    ".lhs": "Haskell",
    ".ml": "OCaml",
    ".mli": "OCaml",
    ".clj": "Clojure",
    ".cljs": "Clojure",
    ".cljc": "Clojure",
    ".dart": "Dart",
    ".zig": "Zig",
    ".nim": "Nim",
    ".v": "V",
    ".d": "D",
    ".pas": "Pascal",
    ".pp": "Pascal",
    ".f90": "Fortran",
    ".f95": "Fortran",
    ".f03": "Fortran",
    ".f": "Fortran",
    ".for": "Fortran",
    ".asm": "Assembly",
    ".s": "Assembly",
    ".sh": "Shell",
    ".bash": "Shell",
    ".zsh": "Shell",
    ".fish": "Shell",
    ".ps1": "PowerShell",
    ".psm1": "PowerShell",
    ".bat": "Batch",
    ".cmd": "Batch",
    ".html": "HTML",
    ".htm": "HTML",
    ".css": "CSS",
    ".scss": "SCSS",
    ".sass": "Sass",
    ".less": "Less",
    ".styl": "Stylus",
    ".vue": "Vue",
    ".svelte": "Svelte",
    ".sql": "SQL",
    ".graphql": "GraphQL",
    ".gql": "GraphQL",
    ".proto": "Protocol Buffers",
    ".yaml": "YAML",
    ".yml": "YAML",
    ".json": "JSON",
    ".xml": "XML",
    ".toml": "TOML",
    ".ini": "INI",
    ".cfg": "INI",
    ".md": "Markdown",
    ".rst": "reStructuredText",
    ".tex": "LaTeX",
    ".tf": "Terraform",
    ".hcl": "HCL",
    ".sol": "Solidity",
    ".vy": "Vyper",
    ".wasm": "WebAssembly",
    ".wat": "WebAssembly",
    ".dockerfile": "Dockerfile",
    ".cmake": "CMake",
    ".makefile": "Makefile",
    ".mk": "Makefile",
}

SKIP_DIRS: set[str] = {
    "node_modules", ".git", ".svn", ".hg", "__pycache__", ".mypy_cache",
    ".pytest_cache", ".tox", ".nox", ".venv", "venv", "env", ".env",
    "dist", "build", "out", "target", ".next", ".nuxt", ".output",
    "vendor", "third_party", "3rdparty", ".gradle", ".idea", ".vscode",
    ".vs", "bin", "obj", ".cache", ".parcel-cache", "coverage",
    ".nyc_output", ".terraform", ".eggs", "*.egg-info",
}

GENERATED_PATTERNS: set[str] = {
    ".min.js", ".min.css", ".bundle.js", ".chunk.js",
    ".Designer.cs", ".generated.cs", ".g.cs", ".g.i.cs",
    ".pb.go", ".pb.cc", ".pb.h", "_pb2.py", "_pb2_grpc.py",
    ".d.ts",
}

BINARY_EXTENSIONS: set[str] = {
    ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".ico", ".svg", ".webp",
    ".mp3", ".mp4", ".wav", ".avi", ".mov", ".mkv", ".flac",
    ".zip", ".tar", ".gz", ".bz2", ".xz", ".rar", ".7z",
    ".exe", ".dll", ".so", ".dylib", ".a", ".o", ".obj",
    ".class", ".jar", ".war", ".ear",
    ".pyc", ".pyo", ".whl",
    ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
    ".ttf", ".otf", ".woff", ".woff2", ".eot",
    ".db", ".sqlite", ".sqlite3",
    ".lock",
}


def _is_generated(filename: str) -> bool:
    lower = filename.lower()
    return any(lower.endswith(pat) for pat in GENERATED_PATTERNS)


def _is_binary(ext: str) -> bool:
    return ext.lower() in BINARY_EXTENSIONS


def _count_lines(path: Path) -> int:
    try:
        return sum(1 for _ in path.open("r", encoding="utf-8", errors="replace"))
    except (OSError, ValueError):
        return 0


def _should_skip_dir(name: str) -> bool:
    return name in SKIP_DIRS or name.startswith(".")


def _get_language(path: Path) -> str | None:
    name_lower = path.name.lower()
    if name_lower == "dockerfile" or name_lower.startswith("dockerfile."):
        return "Dockerfile"
    if name_lower == "makefile" or name_lower == "gnumakefile":
        return "Makefile"
    if name_lower == "cmakelists.txt":
        return "CMake"
    if name_lower == "jenkinsfile":
        return "Groovy"
    if name_lower == "vagrantfile":
        return "Ruby"
    if name_lower == "rakefile":
        return "Ruby"
    if name_lower == "gemfile":
        return "Ruby"

    ext = path.suffix
    if not ext:
        return None
    return EXTENSION_MAP.get(ext) or EXTENSION_MAP.get(ext.lower())


def detect_languages(project_path: str | Path) -> tuple[list[LanguageInfo], str | None]:
    root = Path(project_path)
    if not root.is_dir():
        return [], None

    stats: dict[str, dict[str, int]] = {}

    for item in root.rglob("*"):
        if not item.is_file():
            continue

        rel_parts = item.relative_to(root).parts
        if any(_should_skip_dir(p) for p in rel_parts[:-1]):
            continue

        if _is_binary(item.suffix):
            continue

        if _is_generated(item.name):
            continue

        language = _get_language(item)
        if language is None:
            continue

        lines = _count_lines(item)
        if language not in stats:
            stats[language] = {"files": 0, "lines": 0}
        stats[language]["files"] += 1
        stats[language]["lines"] += lines

    total_lines = sum(s["lines"] for s in stats.values())

    results = [
        LanguageInfo(
            name=lang,
            files=data["files"],
            lines=data["lines"],
            percentage=round((data["lines"] / total_lines * 100) if total_lines > 0 else 0, 1),
        )
        for lang, data in stats.items()
    ]
    results.sort(key=lambda x: x.lines, reverse=True)

    primary = results[0].name if results else None
    return results, primary
