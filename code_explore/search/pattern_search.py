"""Pattern-based search — matches queries against detected architectural patterns."""

import json
import logging
import re
import sqlite3
from pathlib import Path

from code_explore.database import get_connection, get_db_path, get_project
from code_explore.models import SearchResult

logger = logging.getLogger(__name__)

# Maps known pattern names (from analyzer/patterns.py PATTERN_RULES) to query
# keywords that should trigger a match.  Keys are lowercased pattern names;
# values are sets of lowercase keywords/phrases.
PATTERN_KEYWORDS: dict[str, set[str]] = {
    # APIs
    "rest api": {"rest", "api", "endpoint", "rest api", "restful", "http api"},
    "graphql": {"graphql", "graph ql", "gql", "query language"},
    "grpc": {"grpc", "protobuf", "rpc", "proto"},
    "websocket": {"websocket", "ws", "socket", "real-time", "realtime"},
    # Auth
    "oauth": {"oauth", "oauth2", "authorization", "auth"},
    "jwt": {"jwt", "json web token", "token", "bearer"},
    "auth0": {"auth0"},
    "firebase auth": {"firebase auth", "firebase"},
    # Databases
    "postgresql": {"postgresql", "postgres", "pg"},
    "mongodb": {"mongodb", "mongo", "nosql"},
    "redis": {"redis", "cache", "caching"},
    "sqlite": {"sqlite"},
    "prisma": {"prisma", "orm"},
    "typeorm": {"typeorm", "orm"},
    # Frameworks
    "react": {"react", "jsx", "hooks", "component"},
    "vue": {"vue", "vuejs"},
    "angular": {"angular"},
    "next.js": {"next.js", "nextjs", "next", "ssr"},
    "fastapi": {"fastapi", "fast api"},
    "django": {"django"},
    "express": {"express", "expressjs"},
    "spring": {"spring", "spring boot", "springboot"},
    "flask": {"flask"},
    "svelte": {"svelte", "sveltekit"},
    "nestjs": {"nestjs", "nest"},
    "ruby on rails": {"rails", "ruby on rails", "ror"},
    "laravel": {"laravel", "php"},
    # Cloud
    "aws": {"aws", "amazon", "lambda", "s3", "ec2", "dynamodb"},
    "gcp": {"gcp", "google cloud", "gcloud"},
    "azure": {"azure", "microsoft azure"},
    "docker": {"docker", "container", "containerization", "dockerfile"},
    "kubernetes": {"kubernetes", "k8s", "helm", "kubectl"},
    # Concepts
    "web scraping": {"scraping", "web scraping", "crawler", "scraper", "crawl"},
    "ml/ai": {"ml", "ai", "machine learning", "deep learning", "neural",
              "tensorflow", "pytorch", "model training"},
    "cli tool": {"cli", "command line", "terminal"},
    "browser extension": {"browser extension", "chrome extension", "addon"},
    "mobile app": {"mobile", "ios", "android", "react native", "flutter"},
    "testing": {"testing", "test", "tests", "jest", "pytest", "cypress", "e2e"},
    "microservices": {"microservices", "microservice", "service mesh"},
}


def _extract_pattern_keywords(query: str) -> list[str]:
    """Map a query string to known pattern names.

    Returns a list of pattern names (cased as stored in PATTERN_KEYWORDS keys)
    that have at least one keyword matching a token or substring of the query.
    """
    query_lower = query.lower().strip()
    if not query_lower:
        return []

    # Tokenise the query for single-word matching.
    query_tokens = set(re.split(r"\s+", query_lower))

    matched: list[str] = []
    for pattern_name, keywords in PATTERN_KEYWORDS.items():
        for kw in keywords:
            # Multi-word keywords: check substring match against the full query.
            # Single-word keywords: check membership in the token set.
            if " " in kw:
                if kw in query_lower:
                    matched.append(pattern_name)
                    break
            else:
                if kw in query_tokens:
                    matched.append(pattern_name)
                    break

    return matched


def search_by_patterns(
    query: str, limit: int = 10, db_path: Path | None = None
) -> list[SearchResult]:
    """Search for projects whose detected patterns match query keywords.

    Queries the ``data`` JSON column of the projects table for pattern names
    that correspond to the query.  Returns an empty list when no patterns
    match the query or when the database is unavailable.
    """
    pattern_names = _extract_pattern_keywords(query)
    if not pattern_names:
        return []

    path = db_path or get_db_path()
    try:
        conn = get_connection(path)
    except Exception as exc:
        logger.debug("Pattern search: cannot connect to DB: %s", exc)
        return []

    try:
        rows = conn.execute("SELECT id, data FROM projects").fetchall()
    except sqlite3.OperationalError as exc:
        logger.debug("Pattern search query failed: %s", exc)
        conn.close()
        return []

    # Score each project by how many queried patterns it has.
    scored: list[tuple[str, float, list[str]]] = []
    for row in rows:
        try:
            data = json.loads(row["data"])
        except (json.JSONDecodeError, TypeError):
            continue

        project_patterns = data.get("patterns", [])
        if not project_patterns:
            continue

        # Build a set of this project's pattern names (lowercased).
        project_pattern_names = {
            p["name"].lower()
            for p in project_patterns
            if isinstance(p, dict) and "name" in p
        }

        # Count matches and collect highlights.
        hits: list[str] = []
        for pn in pattern_names:
            if pn in project_pattern_names:
                hits.append(f"pattern: {pn}")

        if not hits:
            continue

        # Score: fraction of queried patterns that matched, weighted slightly
        # by the number of absolute matches so more-relevant projects rank
        # higher.
        score = len(hits) / len(pattern_names)
        scored.append((row["id"], score, hits))

    conn.close()

    # Sort by score descending, then limit.
    scored.sort(key=lambda x: x[1], reverse=True)
    scored = scored[:limit]

    results: list[SearchResult] = []
    for project_id, score, highlights in scored:
        project = get_project(project_id, db_path=db_path)
        if project is None:
            continue
        results.append(
            SearchResult(
                project=project,
                score=score,
                match_type="pattern",
                highlights=highlights,
            )
        )

    return results
