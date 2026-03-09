"""SQLite FTS5 fulltext search."""

import logging
import sqlite3
from pathlib import Path

from code_explore.database import get_connection, get_db_path, get_project
from code_explore.database import search_fulltext as db_search_fulltext
from code_explore.models import SearchResult

logger = logging.getLogger(__name__)

# Minimum word length to try as an individual search term
_MIN_WORD_LEN = 3


def _extract_snippets(
    query: str, project_id: str, db_path: Path | None = None
) -> list[str]:
    conn = get_connection(db_path)
    try:
        rows = conn.execute(
            """
            SELECT
                snippet(projects_fts, 0, '**', '**', '...', 32) as name_snip,
                snippet(projects_fts, 1, '**', '**', '...', 64) as summary_snip,
                snippet(projects_fts, 2, '**', '**', '...', 32) as tags_snip,
                snippet(projects_fts, 3, '**', '**', '...', 64) as readme_snip
            FROM projects_fts fts
            JOIN projects p ON p.rowid = fts.rowid
            WHERE projects_fts MATCH ? AND p.id = ?
            LIMIT 1
            """,
            (query, project_id),
        ).fetchall()
    except sqlite3.OperationalError:
        return []
    finally:
        conn.close()

    if not rows:
        return []

    snippets = []
    row = rows[0]
    for key in ("name_snip", "summary_snip", "tags_snip", "readme_snip"):
        val = row[key]
        if val and "**" in val:
            snippets.append(val)
    return snippets


def _or_query(query: str) -> str:
    """Convert a multi-word query to OR-separated FTS5 query."""
    tokens = query.split()
    if len(tokens) <= 1:
        return query
    return " OR ".join(tokens)


def _significant_words(query: str) -> list[str]:
    """Return words from the query that are long enough to search individually."""
    return [w for w in query.split() if len(w) >= _MIN_WORD_LEN]


def search(
    query: str, limit: int = 20, db_path: Path | None = None
) -> list[SearchResult]:
    # Strategy 1: Try the original query (AND logic)
    raw_results = _safe_fts_search(query, limit=limit, db_path=db_path)

    # Strategy 2: If no results, try OR between words
    if not raw_results:
        or_query = _or_query(query)
        if or_query != query:
            raw_results = _safe_fts_search(or_query, limit=limit, db_path=db_path)

    # Strategy 3: If still no results, try each significant word individually
    if not raw_results:
        seen_ids: set[str] = set()
        merged: list[tuple[str, float]] = []
        score_penalty = 0.0

        for word in _significant_words(query):
            word_results = _safe_fts_search(word, limit=limit, db_path=db_path)
            for project_id, rank in word_results:
                if project_id not in seen_ids:
                    seen_ids.add(project_id)
                    # Apply increasing penalty so earlier word matches rank higher
                    adjusted_rank = rank - score_penalty if rank < 0 else rank + score_penalty
                    merged.append((project_id, adjusted_rank))
            score_penalty += 1.0

        raw_results = merged[:limit]

    # Build SearchResult objects
    # For snippets, try the original query first, then fall back to OR query
    results = []
    for project_id, rank in raw_results:
        project = get_project(project_id, db_path=db_path)
        if project is None:
            continue

        score = -rank if rank < 0 else rank
        highlights = _extract_snippets(query, project_id, db_path=db_path)
        if not highlights:
            or_q = _or_query(query)
            if or_q != query:
                highlights = _extract_snippets(or_q, project_id, db_path=db_path)
        if not highlights:
            # Try individual significant words for snippet extraction
            for word in _significant_words(query):
                highlights = _extract_snippets(word, project_id, db_path=db_path)
                if highlights:
                    break

        results.append(
            SearchResult(
                project=project,
                score=score,
                match_type="fulltext",
                highlights=highlights,
            )
        )

    return results


def _safe_fts_search(
    query: str, limit: int = 20, db_path: Path | None = None
) -> list[tuple[str, float]]:
    """Run FTS5 search, returning empty list on any error."""
    try:
        return db_search_fulltext(query, limit=limit, db_path=db_path)
    except sqlite3.OperationalError as e:
        logger.debug("FTS5 search failed for query %r: %s", query, e)
        return []
