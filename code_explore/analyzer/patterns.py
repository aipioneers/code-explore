"""Detect architectural patterns and concepts from project structure."""

import json
import re
from pathlib import Path

from code_explore.models import PatternInfo

SKIP_DIRS: set[str] = {
    "node_modules", ".git", ".svn", ".hg", "__pycache__", ".mypy_cache",
    ".pytest_cache", ".tox", ".venv", "venv", "env", "dist", "build",
    "out", "target", ".next", ".nuxt", "vendor", ".cache", "coverage",
}


class _PatternRule:
    __slots__ = ("name", "category", "file_patterns", "dir_patterns",
                 "content_patterns", "content_files")

    def __init__(
        self,
        name: str,
        category: str,
        file_patterns: list[str] | None = None,
        dir_patterns: list[str] | None = None,
        content_patterns: list[str] | None = None,
        content_files: list[str] | None = None,
    ):
        self.name = name
        self.category = category
        self.file_patterns = [p.lower() for p in (file_patterns or [])]
        self.dir_patterns = [p.lower() for p in (dir_patterns or [])]
        self.content_patterns = content_patterns or []
        self.content_files = [f.lower() for f in (content_files or [])]


PATTERN_RULES: list[_PatternRule] = [
    # APIs
    _PatternRule("REST API", "API",
                 file_patterns=["routes", "router", "endpoint", "controller", "api"],
                 dir_patterns=["routes", "controllers", "endpoints", "api"],
                 content_patterns=[r"@(app|router)\.(get|post|put|delete|patch)", r"express\.Router",
                                   r"@GetMapping|@PostMapping|@RestController",
                                   r"@api_view|@action"],
                 content_files=["*.py", "*.js", "*.ts", "*.java"]),
    _PatternRule("GraphQL", "API",
                 file_patterns=["schema.graphql", "resolvers", ".graphql", "typedefs"],
                 dir_patterns=["graphql", "resolvers"],
                 content_patterns=[r"type Query", r"gql`", r"@Query\(", r"graphql"],
                 content_files=["*.graphql", "*.gql", "*.py", "*.js", "*.ts"]),
    _PatternRule("gRPC", "API",
                 file_patterns=[".proto"],
                 dir_patterns=["proto", "protos", "grpc"],
                 content_patterns=[r"service\s+\w+\s*\{", r"rpc\s+\w+"],
                 content_files=["*.proto"]),
    _PatternRule("WebSocket", "API",
                 content_patterns=[r"websocket|ws://|wss://|socket\.io|WebSocket",
                                   r"@websocket|channels"],
                 content_files=["*.py", "*.js", "*.ts", "*.java"]),

    # Auth
    _PatternRule("OAuth", "Auth",
                 content_patterns=[r"oauth|OAuth|passport\.authenticate",
                                   r"oauth2|authorization_code|client_credentials"],
                 content_files=["*.py", "*.js", "*.ts", "*.java", "*.json"]),
    _PatternRule("JWT", "Auth",
                 content_patterns=[r"jsonwebtoken|jwt\.|PyJWT|jose|JWT",
                                   r"access_token|refresh_token|Bearer"],
                 content_files=["*.py", "*.js", "*.ts", "*.java", "*.json"]),
    _PatternRule("Auth0", "Auth",
                 content_patterns=[r"auth0|@auth0"],
                 content_files=["*.py", "*.js", "*.ts", "*.json", "*.env*"]),
    _PatternRule("Firebase Auth", "Auth",
                 content_patterns=[r"firebase.*auth|firebaseAuth|firebase\.auth"],
                 content_files=["*.py", "*.js", "*.ts", "*.json"]),

    # Databases
    _PatternRule("PostgreSQL", "Database",
                 content_patterns=[r"postgres|postgresql|pg\.|psycopg|asyncpg"],
                 content_files=["*.py", "*.js", "*.ts", "*.java", "*.json", "*.yml", "*.yaml",
                                "*.toml", "*.env*", "docker-compose*"]),
    _PatternRule("MongoDB", "Database",
                 content_patterns=[r"mongodb|mongoose|mongo\.|pymongo|MongoClient"],
                 content_files=["*.py", "*.js", "*.ts", "*.java", "*.json", "*.yml"]),
    _PatternRule("Redis", "Database",
                 content_patterns=[r"redis|ioredis|aioredis|Redis\("],
                 content_files=["*.py", "*.js", "*.ts", "*.java", "*.json", "*.yml"]),
    _PatternRule("SQLite", "Database",
                 file_patterns=[".sqlite", ".sqlite3", ".db"],
                 content_patterns=[r"sqlite3|sqlite|better-sqlite"],
                 content_files=["*.py", "*.js", "*.ts"]),
    _PatternRule("Prisma", "Database",
                 file_patterns=["schema.prisma", "prisma"],
                 dir_patterns=["prisma"],
                 content_patterns=[r"@prisma/client|prisma generate"],
                 content_files=["*.ts", "*.js", "*.json"]),
    _PatternRule("TypeORM", "Database",
                 content_patterns=[r"typeorm|@Entity|@Column|createConnection"],
                 content_files=["*.ts", "*.js"]),

    # Frameworks
    _PatternRule("React", "Framework",
                 content_patterns=[r"from ['\"]react['\"]|import React|useState|useEffect|jsx"],
                 content_files=["*.js", "*.jsx", "*.ts", "*.tsx"]),
    _PatternRule("Vue", "Framework",
                 file_patterns=[".vue"],
                 content_patterns=[r"createApp|Vue\.component|defineComponent"],
                 content_files=["*.vue", "*.js", "*.ts"]),
    _PatternRule("Angular", "Framework",
                 file_patterns=["angular.json", ".angular"],
                 dir_patterns=["angular"],
                 content_patterns=[r"@angular/core|@Component|@NgModule"],
                 content_files=["*.ts"]),
    _PatternRule("Next.js", "Framework",
                 file_patterns=["next.config.js", "next.config.mjs", "next.config.ts"],
                 content_patterns=[r"from ['\"]next|next/link|next/router|getServerSideProps|getStaticProps"],
                 content_files=["*.js", "*.jsx", "*.ts", "*.tsx"]),
    _PatternRule("FastAPI", "Framework",
                 content_patterns=[r"from fastapi|FastAPI\(|@app\.(get|post|put|delete)"],
                 content_files=["*.py"]),
    _PatternRule("Django", "Framework",
                 file_patterns=["manage.py", "wsgi.py", "asgi.py"],
                 dir_patterns=["templates", "migrations"],
                 content_patterns=[r"from django|django\.conf|INSTALLED_APPS"],
                 content_files=["*.py"]),
    _PatternRule("Express", "Framework",
                 content_patterns=[r"express\(\)|require\(['\"]express['\"]\)|from ['\"]express['\"]"],
                 content_files=["*.js", "*.ts"]),
    _PatternRule("Spring", "Framework",
                 file_patterns=["pom.xml", "build.gradle"],
                 content_patterns=[r"@SpringBootApplication|spring-boot|org\.springframework"],
                 content_files=["*.java", "*.kt", "*.xml", "*.gradle"]),
    _PatternRule("Flask", "Framework",
                 content_patterns=[r"from flask|Flask\(__name__\)"],
                 content_files=["*.py"]),
    _PatternRule("Svelte", "Framework",
                 file_patterns=["svelte.config.js", "svelte.config.ts"],
                 content_patterns=[r"from ['\"]svelte|import.*svelte"],
                 content_files=["*.svelte", "*.js", "*.ts"]),
    _PatternRule("NestJS", "Framework",
                 file_patterns=["nest-cli.json"],
                 content_patterns=[r"@nestjs/|@Module|@Controller|@Injectable"],
                 content_files=["*.ts"]),
    _PatternRule("Ruby on Rails", "Framework",
                 file_patterns=["Gemfile", "Rakefile"],
                 dir_patterns=["app/models", "app/controllers", "app/views", "db/migrate"],
                 content_patterns=[r"rails|ActiveRecord|ApplicationController"],
                 content_files=["*.rb"]),
    _PatternRule("Laravel", "Framework",
                 file_patterns=["artisan", "composer.json"],
                 dir_patterns=["app/Http", "resources/views"],
                 content_patterns=[r"laravel|Illuminate|Artisan"],
                 content_files=["*.php", "*.json"]),

    # Cloud
    _PatternRule("AWS", "Cloud",
                 file_patterns=["serverless.yml", "sam-template.yaml", "cdk.json",
                                "cloudformation.yml", "cloudformation.yaml"],
                 content_patterns=[r"aws-sdk|boto3|amazonaws\.com|AWS::|aws-cdk"],
                 content_files=["*.py", "*.js", "*.ts", "*.java", "*.yml", "*.yaml", "*.json"]),
    _PatternRule("GCP", "Cloud",
                 file_patterns=["app.yaml"],
                 content_patterns=[r"google-cloud|googleapis|gcloud|@google-cloud"],
                 content_files=["*.py", "*.js", "*.ts", "*.java", "*.yml", "*.json"]),
    _PatternRule("Azure", "Cloud",
                 content_patterns=[r"azure|@azure|microsoft\.azure|WindowsAzure"],
                 content_files=["*.py", "*.js", "*.ts", "*.java", "*.json", "*.yml"]),
    _PatternRule("Docker", "Cloud",
                 file_patterns=["Dockerfile", "docker-compose.yml", "docker-compose.yaml",
                                ".dockerignore"],
                 dir_patterns=["docker"]),
    _PatternRule("Kubernetes", "Cloud",
                 file_patterns=["k8s", "kubernetes"],
                 dir_patterns=["k8s", "kubernetes", "helm", "charts"],
                 content_patterns=[r"apiVersion.*kind|kubectl|helm"],
                 content_files=["*.yml", "*.yaml"]),

    # Concepts
    _PatternRule("Web Scraping", "Concept",
                 content_patterns=[r"beautifulsoup|scrapy|puppeteer|playwright|cheerio|selenium|crawl"],
                 content_files=["*.py", "*.js", "*.ts"]),
    _PatternRule("ML/AI", "Concept",
                 content_patterns=[r"tensorflow|pytorch|torch|sklearn|scikit-learn|keras|transformers|openai|langchain|huggingface"],
                 content_files=["*.py", "*.js", "*.ts", "*.ipynb"]),
    _PatternRule("CLI Tool", "Concept",
                 content_patterns=[r"argparse|click|typer|commander|yargs|clap::"],
                 content_files=["*.py", "*.js", "*.ts", "*.rs"]),
    _PatternRule("Browser Extension", "Concept",
                 dir_patterns=["extension", "addon"],
                 content_patterns=[r'"browser_action"', r'"content_scripts"',
                                   r"chrome\.runtime", r"browser\.tabs",
                                   r"chrome\.tabs", r"browser\.runtime"],
                 content_files=["manifest.json"]),
    _PatternRule("Mobile App", "Concept",
                 file_patterns=["Podfile", "pubspec.yaml", "AndroidManifest.xml",
                                "*.swift", "*.kt", "*.dart"],
                 dir_patterns=["ios", "android", "maui", "flutter"],
                 content_patterns=[r"react-native", r"from ['\"]expo['\"]",
                                   r"import\s+Flutter", r"SwiftUI",
                                   r"UIApplicationDelegate"],
                 content_files=["app.json", "*.js", "*.ts", "*.dart", "*.swift", "*.kt"]),
    _PatternRule("Testing", "Concept",
                 file_patterns=["jest.config.js", "jest.config.ts", "jest.config.mjs",
                                "pytest.ini", "pyproject.toml", "setup.cfg",
                                "vitest.config.ts", "vitest.config.js",
                                ".rspec", "karma.conf.js", "cypress.config.ts",
                                "cypress.config.js"],
                 dir_patterns=["e2e", "cypress", "__tests__"]),
    _PatternRule("Microservices", "Concept",
                 dir_patterns=["microservices"],
                 content_patterns=[r"service-discovery|consul|eureka|api-gateway|service-mesh|istio"],
                 content_files=["*.yml", "*.yaml", "*.json", "docker-compose*"]),
]

FRAMEWORK_NAMES: set[str] = {
    "React", "Vue", "Angular", "Next.js", "FastAPI", "Django", "Express",
    "Spring", "Flask", "Svelte", "NestJS", "Ruby on Rails", "Laravel",
}


def _should_skip_dir(name: str) -> bool:
    return name in SKIP_DIRS or name.startswith(".")


def _collect_project_files(root: Path) -> tuple[list[Path], set[str], set[str]]:
    files: list[Path] = []
    all_filenames: set[str] = set()
    all_dirnames: set[str] = set()

    for item in root.rglob("*"):
        rel = item.relative_to(root)
        parts = rel.parts

        if any(_should_skip_dir(p) for p in parts[:-1] if item.is_file()):
            continue
        if any(_should_skip_dir(p) for p in parts if item.is_dir()):
            continue

        if item.is_file():
            files.append(item)
            all_filenames.add(item.name.lower())
            for part in parts[:-1]:
                all_dirnames.add(part.lower())
            if len(parts) > 1:
                all_dirnames.add("/".join(p.lower() for p in parts[:-1]))
        elif item.is_dir():
            all_dirnames.add(item.name.lower())

    return files, all_filenames, all_dirnames


def _matches_glob(filename: str, pattern: str) -> bool:
    if pattern.startswith("*."):
        return filename.endswith(pattern[1:])
    return filename == pattern.lower()


def _search_content(files: list[Path], rule: _PatternRule, root: Path) -> list[str]:
    evidence: list[str] = []
    if not rule.content_patterns or not rule.content_files:
        return evidence

    target_files = [
        f for f in files
        if any(_matches_glob(f.name.lower(), cf) for cf in rule.content_files)
    ]

    compiled = [re.compile(p, re.IGNORECASE) for p in rule.content_patterns]

    for f in target_files[:200]:
        try:
            content = f.read_text(encoding="utf-8", errors="replace")[:50_000]
        except OSError:
            continue

        for pattern in compiled:
            if pattern.search(content):
                rel_path = str(f.relative_to(root))
                evidence.append(rel_path)
                break

    return evidence


def detect_patterns(project_path: str | Path) -> tuple[list[PatternInfo], list[str]]:
    root = Path(project_path)
    if not root.is_dir():
        return [], []

    files, all_filenames, all_dirnames = _collect_project_files(root)

    detected: list[PatternInfo] = []
    frameworks: list[str] = []

    for rule in PATTERN_RULES:
        evidence: list[str] = []
        file_hits = 0
        dir_hits = 0

        for fp in rule.file_patterns:
            if fp.startswith("*."):
                # Extension match (e.g. *.swift) — require exact suffix
                ext = fp[1:]  # e.g. ".swift"
                matches = [f for f in all_filenames if f.endswith(ext)]
            elif fp.startswith("."):
                matches = [f for f in all_filenames if f.endswith(fp)]
            else:
                # Exact filename match only (no substring matching)
                matches = [f for f in all_filenames if f == fp.lower()]
            if matches:
                evidence.extend(matches[:3])
                file_hits += 1

        for dp in rule.dir_patterns:
            if dp in all_dirnames:
                evidence.append(f"{dp}/")
                dir_hits += 1

        content_evidence = _search_content(files, rule, root)
        if content_evidence:
            evidence.extend(content_evidence[:5])

        if not evidence:
            continue

        # Require at least 2 different types of evidence to reduce false positives.
        # Types: file matches, directory matches, content matches.
        evidence_types = sum([
            file_hits > 0,
            dir_hits > 0,
            len(content_evidence) > 0,
        ])

        # For most patterns, require at least 2 evidence types.
        # Content-only patterns (those with no file/dir patterns defined) can
        # match on content alone but need multiple content hits.
        has_structural_rules = bool(rule.file_patterns or rule.dir_patterns)

        if has_structural_rules and evidence_types < 2:
            # Single evidence type is too weak for patterns that define
            # structural rules — skip to avoid false positives.
            continue

        if not has_structural_rules and len(content_evidence) < 2:
            # Content-only patterns need at least 2 file hits.
            continue

        # Calculate confidence based on accumulated evidence
        score = 0.0
        if file_hits:
            score += min(0.35, file_hits * 0.15)
        if dir_hits:
            score += min(0.25, dir_hits * 0.15)
        if content_evidence:
            score += 0.2 + min(0.2, len(content_evidence) * 0.04)

        confidence = min(1.0, score)
        unique_evidence = list(dict.fromkeys(evidence))

        detected.append(PatternInfo(
            name=rule.name,
            category=rule.category,
            confidence=round(confidence, 2),
            evidence=unique_evidence[:10],
        ))

        if rule.name in FRAMEWORK_NAMES and confidence >= 0.3:
            frameworks.append(rule.name)

    detected.sort(key=lambda p: p.confidence, reverse=True)
    return detected, frameworks
