"""Hybrid search combining fulltext, semantic, and pattern search with reciprocal rank fusion."""

import logging
from concurrent.futures import ThreadPoolExecutor, as_completed
from pathlib import Path

from code_explore.models import SearchResult
from code_explore.search.fulltext import search as fulltext_search
from code_explore.search.pattern_search import search_by_patterns
from code_explore.search.semantic import search as semantic_search

logger = logging.getLogger(__name__)

# Confidence when a result appears in only one search source.
_SINGLE_SOURCE_CONFIDENCE = 0.5

# Maximum confidence when multiple sources agree (rank-weighted).
_BOTH_SOURCES_MAX = 1.0


def _compute_confidence(
    pid: str,
    source_ids: list[set[str]],
    source_ranks: list[dict[str, int]],
    total_results: int,
) -> float:
    """Score confidence 0.0-1.0 based on agreement between search sources.

    Higher when more sources return the result, especially at high ranks.
    Lower when only one source matched.

    ``source_ids`` and ``source_ranks`` are parallel lists — one entry per
    search source (fulltext, semantic, pattern).
    """
    present_count = sum(1 for ids in source_ids if pid in ids)

    if present_count >= 2:
        # Multiple sources agree — base confidence starts at 0.7.
        # Bonus up to 0.3 based on average rank across matching sources.
        ranks = [
            sr.get(pid, total_results)
            for sr, ids in zip(source_ranks, source_ids)
            if pid in ids
        ]
        avg_rank = sum(ranks) / len(ranks) if ranks else float(total_results)
        denom = max(total_results, 1)
        rank_bonus = 0.3 * max(0.0, 1.0 - avg_rank / denom)
        return min(_BOTH_SOURCES_MAX, 0.7 + rank_bonus)

    # Only one source matched.
    for sr, ids in zip(source_ranks, source_ids):
        if pid in ids:
            rank = sr.get(pid, total_results)
            break
    else:
        rank = total_results

    denom = max(total_results, 1)
    rank_bonus = 0.2 * max(0.0, 1.0 - rank / denom)
    return _SINGLE_SOURCE_CONFIDENCE + rank_bonus


def _reciprocal_rank_fusion(
    *result_lists: list[SearchResult],
) -> list[SearchResult]:
    """Fuse an arbitrary number of ranked result lists using RRF."""
    from code_explore.config import get_config

    rrf_k = get_config().rrf_k

    scores: dict[str, float] = {}
    results_map: dict[str, SearchResult] = {}
    highlights_map: dict[str, list[str]] = {}

    # Per-source id sets and rank maps (parallel lists).
    source_ids: list[set[str]] = []
    source_ranks: list[dict[str, int]] = []

    for result_list in result_lists:
        ids: set[str] = set()
        ranks: dict[str, int] = {}
        for rank, result in enumerate(result_list):
            pid = result.project.id
            ids.add(pid)
            ranks.setdefault(pid, rank)
            scores[pid] = scores.get(pid, 0.0) + 1.0 / (rrf_k + rank + 1)
            if pid not in results_map:
                results_map[pid] = result
                highlights_map[pid] = list(result.highlights)
            else:
                for h in result.highlights:
                    if h not in highlights_map[pid]:
                        highlights_map[pid].append(h)
        source_ids.append(ids)
        source_ranks.append(ranks)

    total_results = max((len(rl) for rl in result_lists), default=0)
    ranked = sorted(scores.items(), key=lambda x: x[1], reverse=True)

    merged = []
    for pid, score in ranked:
        base = results_map[pid]
        confidence = _compute_confidence(
            pid, source_ids, source_ranks, total_results,
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
    pattern_results: list[SearchResult] = []

    with ThreadPoolExecutor(max_workers=3) as executor:
        ft_future = executor.submit(fulltext_search, query, limit=limit, db_path=db_path)
        sem_future = executor.submit(semantic_search, query, limit=limit, db_path=db_path)
        pat_future = executor.submit(search_by_patterns, query, limit=limit, db_path=db_path)

        for future in as_completed([ft_future, sem_future, pat_future]):
            try:
                if future is ft_future:
                    fulltext_results = future.result()
                elif future is sem_future:
                    semantic_results = future.result()
                else:
                    pattern_results = future.result()
            except Exception as e:
                logger.error("Search component failed: %s", e)

    # Collect all non-empty result lists.
    available: list[list[SearchResult]] = [
        rl for rl in [fulltext_results, semantic_results, pattern_results] if rl
    ]

    if not available:
        return []

    if len(available) == 1:
        results = available[0]
        for r in results:
            r.confidence = _SINGLE_SOURCE_CONFIDENCE
        return results[:limit]

    merged = _reciprocal_rank_fusion(*available)
    return merged[:limit]
