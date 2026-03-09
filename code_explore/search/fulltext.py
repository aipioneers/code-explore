"""SQLite FTS5 fulltext search."""

import logging
import sqlite3
from pathlib import Path

from code_explore.database import get_connection, get_db_path, get_project
from code_explore.database import search_fulltext as db_search_fulltext
from code_explore.models import SearchResult

logger = logging.getLogger(__name__)


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
                snippet(projects_fts, 2, '**', '**', '...', 32) as tags_snip
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
    for key in ("name_snip", "summary_snip", "tags_snip"):
        val = row[key]
        if val and "**" in val:
            snippets.append(val)
    return snippets


def search(
    query: str, limit: int = 20, db_path: Path | None = None
) -> list[SearchResult]:
    try:
        raw_results = db_search_fulltext(query, limit=limit, db_path=db_path)
    except sqlite3.OperationalError as e:
        logger.error("FTS5 search failed: %s", e)
        return []

    results = []
    for project_id, rank in raw_results:
        project = get_project(project_id, db_path=db_path)
        if project is None:
            continue

        score = -rank if rank < 0 else rank
        highlights = _extract_snippets(query, project_id, db_path=db_path)

        results.append(
            SearchResult(
                project=project,
                score=score,
                match_type="fulltext",
                highlights=highlights,
            )
        )

    return results
