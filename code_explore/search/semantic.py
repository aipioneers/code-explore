"""LanceDB vector similarity search."""

import logging
from pathlib import Path

import lancedb

from code_explore.database import get_project
from code_explore.indexer.embeddings import (
    TABLE_NAME,
    generate_embedding,
    _ollama_available,
)
from code_explore.models import SearchResult
from code_explore.search.fulltext import search as fulltext_search

logger = logging.getLogger(__name__)


def search(
    query: str, limit: int = 20, db_path: Path | None = None
) -> list[SearchResult]:
    if not _ollama_available():
        logger.warning("Ollama unavailable. Falling back to fulltext search.")
        return fulltext_search(query, limit=limit, db_path=db_path)

    query_vector = generate_embedding(query)
    if query_vector is None:
        logger.warning("Failed to generate query embedding. Falling back to fulltext search.")
        return fulltext_search(query, limit=limit, db_path=db_path)

    from code_explore.config import get_config

    vector_path = get_config().vector_path
    if not vector_path.exists():
        logger.warning("Vector store not found. Falling back to fulltext search.")
        return fulltext_search(query, limit=limit, db_path=db_path)

    try:
        db = lancedb.connect(str(vector_path))
        if TABLE_NAME not in db.table_names():
            logger.warning("Embeddings table not found. Falling back to fulltext search.")
            return fulltext_search(query, limit=limit, db_path=db_path)

        table = db.open_table(TABLE_NAME)
        raw_results = (
            table.search(query_vector)
            .metric("cosine")
            .limit(limit)
            .to_list()
        )
    except Exception as e:
        logger.error("Vector search failed: %s. Falling back to fulltext search.", e)
        return fulltext_search(query, limit=limit, db_path=db_path)

    results = []
    for row in raw_results:
        project_id = row["id"]
        project = get_project(project_id, db_path=db_path)
        if project is None:
            continue

        distance = row.get("_distance", 0.0)
        score = max(0.0, 1.0 - distance)

        text_snippet = row.get("text", "")
        highlights = [text_snippet] if text_snippet else []

        results.append(
            SearchResult(
                project=project,
                score=score,
                match_type="semantic",
                highlights=highlights,
            )
        )

    return results
