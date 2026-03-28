"""Lightweight heuristic reranker for hybrid search results.

Applies keyword density scoring, freshness boosts, and quality signal boosts
after Reciprocal Rank Fusion to improve result relevance.  No ML models —
pure heuristic scoring.
"""

import logging
import re
from datetime import datetime, timedelta

from code_explore.models import SearchResult

logger = logging.getLogger(__name__)

# --- Field weights for keyword density scoring ---
_FIELD_WEIGHTS = {
    "name": 3.0,
    "tags": 2.5,
    "patterns": 2.0,
    "readme_snippet": 1.5,
    "summary": 1.0,
}

# --- Freshness tiers ---
_FRESHNESS_30_DAYS = 1.2
_FRESHNESS_90_DAYS = 1.1
_FRESHNESS_DEFAULT = 1.0

# --- Quality signal bonuses ---
_QUALITY_README = 0.1
_QUALITY_SUMMARY = 0.1
_QUALITY_TAGS = 0.05
_QUALITY_DEPENDENCIES = 0.05


def _extract_keywords(query: str) -> list[str]:
    """Split query into lowercase keywords, dropping very short tokens."""
    tokens = re.split(r"\s+", query.strip().lower())
    return [t for t in tokens if len(t) >= 1]


def _keyword_density_score(result: SearchResult, keywords: list[str]) -> float:
    """Score a result by how many query keywords appear in weighted fields.

    Returns an additive boost (can be 0.0 if no keywords match).
    """
    project = result.project
    score = 0.0

    name_lower = project.name.lower() if project.name else ""
    tags_lower = [t.lower() for t in project.tags]
    pattern_names_lower = [p.name.lower() for p in project.patterns]
    readme_lower = (project.readme_snippet or "").lower()
    summary_lower = (project.summary or "").lower()

    for kw in keywords:
        if kw in name_lower:
            score += _FIELD_WEIGHTS["name"]
        if any(kw in tag for tag in tags_lower):
            score += _FIELD_WEIGHTS["tags"]
        if any(kw in pn for pn in pattern_names_lower):
            score += _FIELD_WEIGHTS["patterns"]
        if kw in readme_lower:
            score += _FIELD_WEIGHTS["readme_snippet"]
        if kw in summary_lower:
            score += _FIELD_WEIGHTS["summary"]

    return score


def _freshness_multiplier(result: SearchResult) -> float:
    """Return a freshness multiplier based on how recently the project was updated."""
    last_commit = result.project.git.last_commit_date if result.project.git else None
    if last_commit is None:
        return _FRESHNESS_DEFAULT

    now = datetime.now()
    # Handle timezone-aware datetimes by comparing naive
    if last_commit.tzinfo is not None:
        last_commit = last_commit.replace(tzinfo=None)

    age = now - last_commit

    if age <= timedelta(days=30):
        return _FRESHNESS_30_DAYS
    elif age <= timedelta(days=90):
        return _FRESHNESS_90_DAYS
    else:
        return _FRESHNESS_DEFAULT


def _quality_bonus(result: SearchResult) -> float:
    """Return an additive quality bonus based on project completeness."""
    project = result.project
    bonus = 0.0

    if project.readme_snippet:
        bonus += _QUALITY_README
    if project.summary:
        bonus += _QUALITY_SUMMARY
    if project.tags:
        bonus += _QUALITY_TAGS
    if project.dependencies:
        bonus += _QUALITY_DEPENDENCIES

    return bonus


def rerank(
    results: list[SearchResult],
    query: str,
    *,
    enable_freshness: bool = True,
    enable_quality: bool = True,
) -> list[SearchResult]:
    """Rerank search results using heuristic signals.

    Applies, in order:
    1. Keyword density scoring — additive boost based on field-weighted keyword matches.
    2. Freshness boost — multiplicative factor for recently updated projects.
    3. Quality signals — additive bonus for project completeness (readme, summary, tags, deps).

    Returns a new list of ``SearchResult`` objects sorted by the reranked score.
    The original result objects are not mutated.
    """
    if not results:
        return []

    keywords = _extract_keywords(query)
    reranked: list[SearchResult] = []

    for result in results:
        new_score = result.score

        # 1. Keyword density (additive)
        new_score += _keyword_density_score(result, keywords)

        # 2. Freshness (multiplicative)
        if enable_freshness:
            new_score *= _freshness_multiplier(result)

        # 3. Quality signals (additive)
        if enable_quality:
            new_score += _quality_bonus(result)

        # Create a new SearchResult to avoid mutating the original.
        reranked.append(
            SearchResult(
                project=result.project,
                score=new_score,
                match_type=result.match_type,
                highlights=list(result.highlights),
                confidence=result.confidence,
            )
        )

    reranked.sort(key=lambda r: r.score, reverse=True)
    return reranked
