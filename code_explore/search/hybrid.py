"""Hybrid search combining fulltext and semantic search with reciprocal rank fusion."""

import logging
from concurrent.futures import ThreadPoolExecutor, as_completed
from pathlib import Path

from code_explore.models import SearchResult
from code_explore.search.fulltext import search as fulltext_search
from code_explore.search.semantic import search as semantic_search

logger = logging.getLogger(__name__)


def _reciprocal_rank_fusion(
    fulltext_results: list[SearchResult],
    semantic_results: list[SearchResult],
) -> list[SearchResult]:
    from code_explore.config import get_config

    rrf_k = get_config().rrf_k

    scores: dict[str, float] = {}
    results_map: dict[str, SearchResult] = {}
    highlights_map: dict[str, list[str]] = {}

    for rank, result in enumerate(fulltext_results):
        pid = result.project.id
        scores[pid] = scores.get(pid, 0.0) + 1.0 / (rrf_k + rank + 1)
        if pid not in results_map:
            results_map[pid] = result
            highlights_map[pid] = list(result.highlights)
        else:
            for h in result.highlights:
                if h not in highlights_map[pid]:
                    highlights_map[pid].append(h)

    for rank, result in enumerate(semantic_results):
        pid = result.project.id
        scores[pid] = scores.get(pid, 0.0) + 1.0 / (rrf_k + rank + 1)
        if pid not in results_map:
            results_map[pid] = result
            highlights_map[pid] = list(result.highlights)
        else:
            for h in result.highlights:
                if h not in highlights_map[pid]:
                    highlights_map[pid].append(h)

    ranked = sorted(scores.items(), key=lambda x: x[1], reverse=True)

    merged = []
    for pid, score in ranked:
        base = results_map[pid]
        merged.append(
            SearchResult(
                project=base.project,
                score=score,
                match_type="hybrid",
                highlights=highlights_map.get(pid, []),
            )
        )

    return merged


def search(
    query: str, limit: int = 20, db_path: Path | None = None
) -> list[SearchResult]:
    fulltext_results: list[SearchResult] = []
    semantic_results: list[SearchResult] = []

    with ThreadPoolExecutor(max_workers=2) as executor:
        ft_future = executor.submit(fulltext_search, query, limit=limit, db_path=db_path)
        sem_future = executor.submit(semantic_search, query, limit=limit, db_path=db_path)

        for future in as_completed([ft_future, sem_future]):
            try:
                if future is ft_future:
                    fulltext_results = future.result()
                else:
                    semantic_results = future.result()
            except Exception as e:
                logger.error("Search component failed: %s", e)

    if not semantic_results and fulltext_results:
        return fulltext_results[:limit]

    if not fulltext_results and semantic_results:
        return semantic_results[:limit]

    if not fulltext_results and not semantic_results:
        return []

    merged = _reciprocal_rank_fusion(fulltext_results, semantic_results)
    return merged[:limit]
