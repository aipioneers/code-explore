"""Hybrid search combining fulltext and semantic search with reciprocal rank fusion."""

import logging
from concurrent.futures import ThreadPoolExecutor, as_completed
from pathlib import Path

from code_explore.models import SearchResult
from code_explore.search.fulltext import search as fulltext_search
from code_explore.search.semantic import search as semantic_search

logger = logging.getLogger(__name__)

# Confidence when a result appears in only one search source.
_SINGLE_SOURCE_CONFIDENCE = 0.5

# Maximum confidence when both sources agree (rank-weighted).
_BOTH_SOURCES_MAX = 1.0


def _compute_confidence(
    pid: str,
    fulltext_ids: set[str],
    semantic_ids: set[str],
    fulltext_ranks: dict[str, int],
    semantic_ranks: dict[str, int],
    total_results: int,
) -> float:
    """Score confidence 0.0-1.0 based on agreement between search sources.

    Higher when both fulltext and semantic return the result, especially at
    high ranks.  Lower when only one source matched.
    """
    in_fulltext = pid in fulltext_ids
    in_semantic = pid in semantic_ids

    if in_fulltext and in_semantic:
        # Both sources agree -- base confidence starts at 0.7.
        # Bonus up to 0.3 based on how high the result ranks in each list.
        ft_rank = fulltext_ranks.get(pid, total_results)
        sem_rank = semantic_ranks.get(pid, total_results)
        avg_rank = (ft_rank + sem_rank) / 2.0
        # Normalise: rank 0 -> bonus 0.3, rank >= total_results -> bonus 0.0
        denom = max(total_results, 1)
        rank_bonus = 0.3 * max(0.0, 1.0 - avg_rank / denom)
        return min(_BOTH_SOURCES_MAX, 0.7 + rank_bonus)

    # Only one source matched.
    if in_fulltext:
        rank = fulltext_ranks.get(pid, total_results)
    else:
        rank = semantic_ranks.get(pid, total_results)
    denom = max(total_results, 1)
    rank_bonus = 0.2 * max(0.0, 1.0 - rank / denom)
    return _SINGLE_SOURCE_CONFIDENCE + rank_bonus


def _reciprocal_rank_fusion(
    fulltext_results: list[SearchResult],
    semantic_results: list[SearchResult],
) -> list[SearchResult]:
    from code_explore.config import get_config

    rrf_k = get_config().rrf_k

    scores: dict[str, float] = {}
    results_map: dict[str, SearchResult] = {}
    highlights_map: dict[str, list[str]] = {}

    fulltext_ids: set[str] = set()
    semantic_ids: set[str] = set()
    fulltext_ranks: dict[str, int] = {}
    semantic_ranks: dict[str, int] = {}

    for rank, result in enumerate(fulltext_results):
        pid = result.project.id
        fulltext_ids.add(pid)
        fulltext_ranks.setdefault(pid, rank)
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
        semantic_ids.add(pid)
        semantic_ranks.setdefault(pid, rank)
        scores[pid] = scores.get(pid, 0.0) + 1.0 / (rrf_k + rank + 1)
        if pid not in results_map:
            results_map[pid] = result
            highlights_map[pid] = list(result.highlights)
        else:
            for h in result.highlights:
                if h not in highlights_map[pid]:
                    highlights_map[pid].append(h)

    total_results = max(len(fulltext_results), len(semantic_results))
    ranked = sorted(scores.items(), key=lambda x: x[1], reverse=True)

    merged = []
    for pid, score in ranked:
        base = results_map[pid]
        confidence = _compute_confidence(
            pid, fulltext_ids, semantic_ids,
            fulltext_ranks, semantic_ranks, total_results,
        )
        merged.append(
            SearchResult(
                project=base.project,
                score=score,
                match_type="hybrid",
                highlights=highlights_map.get(pid, []),
                confidence=confidence,
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
        for r in fulltext_results:
            r.confidence = _SINGLE_SOURCE_CONFIDENCE
        return fulltext_results[:limit]

    if not fulltext_results and semantic_results:
        for r in semantic_results:
            r.confidence = _SINGLE_SOURCE_CONFIDENCE
        return semantic_results[:limit]

    if not fulltext_results and not semantic_results:
        return []

    merged = _reciprocal_rank_fusion(fulltext_results, semantic_results)
    return merged[:limit]
