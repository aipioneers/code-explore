"""Tests for the heuristic reranker (keyword density, freshness, quality signals)."""

from datetime import datetime, timedelta

from code_explore.models import (
    DependencyInfo,
    GitInfo,
    PatternInfo,
    Project,
    QualityMetrics,
    SearchResult,
)
from code_explore.search.reranker import rerank


def _make_project(
    pid: str,
    name: str,
    *,
    summary: str | None = None,
    readme_snippet: str | None = None,
    tags: list[str] | None = None,
    patterns: list[PatternInfo] | None = None,
    dependencies: list[DependencyInfo] | None = None,
    last_commit_date: datetime | None = None,
) -> Project:
    return Project(
        id=pid,
        name=name,
        summary=summary,
        readme_snippet=readme_snippet,
        tags=tags or [],
        patterns=patterns or [],
        dependencies=dependencies or [],
        git=GitInfo(last_commit_date=last_commit_date),
    )


def _make_result(
    pid: str,
    name: str,
    score: float,
    *,
    summary: str | None = None,
    readme_snippet: str | None = None,
    tags: list[str] | None = None,
    patterns: list[PatternInfo] | None = None,
    dependencies: list[DependencyInfo] | None = None,
    last_commit_date: datetime | None = None,
) -> SearchResult:
    project = _make_project(
        pid,
        name,
        summary=summary,
        readme_snippet=readme_snippet,
        tags=tags,
        patterns=patterns,
        dependencies=dependencies,
        last_commit_date=last_commit_date,
    )
    return SearchResult(project=project, score=score, match_type="hybrid")


class TestKeywordDensity:
    """Keyword matches in high-weight fields should boost ranking."""

    def test_name_match_ranked_higher(self):
        """A project whose name contains the query keyword should rank above one that doesn't."""
        r1 = _make_result("a", "generic-tool", 1.0, summary="A fastapi web service")
        r2 = _make_result("b", "fastapi-server", 0.9, summary="A generic web service")

        results = rerank([r1, r2], "fastapi", enable_freshness=False, enable_quality=False)

        assert results[0].project.id == "b", "Name match should outrank summary-only match"

    def test_tag_match_ranked_higher_than_readme(self):
        """A project with the query keyword in tags should rank above readme-only match."""
        r1 = _make_result("a", "proj-a", 1.0, readme_snippet="Uses fastapi for routing")
        r2 = _make_result("b", "proj-b", 0.9, tags=["fastapi", "python"])

        results = rerank([r1, r2], "fastapi", enable_freshness=False, enable_quality=False)

        assert results[0].project.id == "b", "Tag match should outrank readme-only match"

    def test_pattern_match_boost(self):
        """A project with query keyword in pattern names should get a boost."""
        r1 = _make_result("a", "proj-a", 1.0, summary="A web application")
        r2 = _make_result(
            "b",
            "proj-b",
            0.9,
            patterns=[PatternInfo(name="REST API", category="architecture", confidence=0.9)],
        )

        results = rerank([r1, r2], "REST", enable_freshness=False, enable_quality=False)

        assert results[0].project.id == "b", "Pattern match should boost ranking"

    def test_multiple_keyword_matches(self):
        """Multiple keywords matching across fields should compound the boost."""
        r1 = _make_result("a", "proj-a", 1.0, summary="A project")
        r2 = _make_result(
            "b",
            "fastapi-server",
            0.8,
            tags=["python", "api"],
            summary="A fastapi python server",
        )

        results = rerank(
            [r1, r2], "fastapi python", enable_freshness=False, enable_quality=False
        )

        assert results[0].project.id == "b"

    def test_case_insensitive_matching(self):
        """Keyword matching should be case-insensitive."""
        r1 = _make_result("a", "proj-a", 1.0)
        r2 = _make_result("b", "FastAPI-App", 0.9)

        results = rerank([r1, r2], "fastapi", enable_freshness=False, enable_quality=False)

        assert results[0].project.id == "b"

    def test_no_keywords_preserves_order(self):
        """When query keywords don't match any fields, original order is preserved."""
        r1 = _make_result("a", "proj-a", 1.0)
        r2 = _make_result("b", "proj-b", 0.9)

        results = rerank(
            [r1, r2], "xyznonexistent", enable_freshness=False, enable_quality=False
        )

        assert results[0].project.id == "a"
        assert results[1].project.id == "b"


class TestFreshnessBoost:
    """Recently updated projects should get a freshness multiplier."""

    def test_recent_project_boosted(self):
        """A project updated within 30 days should outrank an older one with equal base score."""
        now = datetime.now()
        r1 = _make_result("old", "old-proj", 1.0, last_commit_date=now - timedelta(days=200))
        r2 = _make_result("new", "new-proj", 1.0, last_commit_date=now - timedelta(days=5))

        results = rerank([r1, r2], "proj", enable_quality=False)

        assert results[0].project.id == "new"

    def test_90_day_moderate_boost(self):
        """A project updated 60 days ago should get moderate freshness boost (1.1x)."""
        now = datetime.now()
        r1 = _make_result("stale", "stale-proj", 1.0, last_commit_date=now - timedelta(days=200))
        r2 = _make_result("recent", "recent-proj", 1.0, last_commit_date=now - timedelta(days=60))

        results = rerank([r1, r2], "proj", enable_quality=False)

        assert results[0].project.id == "recent"

    def test_no_commit_date_no_boost(self):
        """Projects without a last_commit_date should get no freshness boost (1.0x)."""
        now = datetime.now()
        r1 = _make_result("nodate", "nodate-proj", 1.0)
        r2 = _make_result("fresh", "fresh-proj", 1.0, last_commit_date=now - timedelta(days=5))

        results = rerank([r1, r2], "proj", enable_quality=False)

        assert results[0].project.id == "fresh"

    def test_freshness_disabled(self):
        """When enable_freshness=False, commit dates should not affect ordering."""
        now = datetime.now()
        r1 = _make_result("old", "old-proj", 1.0, last_commit_date=now - timedelta(days=500))
        r2 = _make_result("new", "new-proj", 0.8, last_commit_date=now - timedelta(days=1))

        results = rerank([r1, r2], "proj", enable_freshness=False, enable_quality=False)

        # Without freshness, r1 should still be first due to higher base score
        assert results[0].project.id == "old"


class TestQualitySignals:
    """Completeness signals should provide small additive boosts."""

    def test_complete_project_boosted(self):
        """A project with readme, summary, tags, and deps should outrank a bare one."""
        r1 = _make_result("bare", "bare-proj", 1.0)
        r2 = _make_result(
            "full",
            "full-proj",
            1.0,
            readme_snippet="# Full project",
            summary="A complete project with docs",
            tags=["python", "cli"],
            dependencies=[DependencyInfo(name="click")],
        )

        results = rerank([r1, r2], "proj", enable_freshness=False)

        assert results[0].project.id == "full"

    def test_partial_quality_signals(self):
        """A project with some signals should rank between full and bare."""
        r_bare = _make_result("bare", "bare-proj", 1.0)
        r_partial = _make_result(
            "partial",
            "partial-proj",
            1.0,
            readme_snippet="# Partial project",
            tags=["python"],
        )
        r_full = _make_result(
            "full",
            "full-proj",
            1.0,
            readme_snippet="# Full project",
            summary="A complete project",
            tags=["python", "cli"],
            dependencies=[DependencyInfo(name="click")],
        )

        results = rerank([r_bare, r_partial, r_full], "proj", enable_freshness=False)

        scores = {r.project.id: r.score for r in results}
        assert scores["full"] > scores["partial"]
        assert scores["partial"] > scores["bare"]

    def test_quality_disabled(self):
        """When enable_quality=False, completeness should not affect ordering."""
        r1 = _make_result("bare", "bare-tool", 1.0)
        r2 = _make_result(
            "full",
            "full-tool",
            0.8,
            readme_snippet="# Full readme here",
            summary="A complete CLI application",
            tags=["python"],
            dependencies=[DependencyInfo(name="click")],
        )

        results = rerank([r1, r2], "tool", enable_freshness=False, enable_quality=False)

        # Without quality signals, r1 should be first due to higher base score
        # Both match "tool" in name equally, so base score decides.
        assert results[0].project.id == "bare"


class TestCombinedReranking:
    """All signals combined should produce reasonable orderings."""

    def test_combined_reranking_preserves_correct_ordering(self):
        """A project matching keyword, recent, and complete should clearly top-rank."""
        now = datetime.now()

        r_weak = _make_result("weak", "generic-tool", 1.0, last_commit_date=now - timedelta(days=365))
        r_strong = _make_result(
            "strong",
            "fastapi-server",
            0.9,
            summary="A FastAPI server for data processing",
            readme_snippet="# fastapi-server\nFast API for data",
            tags=["fastapi", "python", "api"],
            patterns=[PatternInfo(name="REST API", category="architecture", confidence=0.9)],
            dependencies=[DependencyInfo(name="fastapi")],
            last_commit_date=now - timedelta(days=3),
        )

        results = rerank([r_weak, r_strong], "fastapi")

        assert results[0].project.id == "strong"

    def test_empty_results(self):
        """Reranking an empty list should return an empty list."""
        results = rerank([], "anything")
        assert results == []

    def test_single_result(self):
        """A single result should be returned as-is (but with updated score)."""
        r = _make_result("only", "only-proj", 1.0)
        results = rerank([r], "proj")
        assert len(results) == 1
        assert results[0].project.id == "only"

    def test_scores_are_updated(self):
        """Reranked results should have updated score values reflecting the boosts."""
        r = _make_result(
            "a",
            "fastapi-app",
            1.0,
            tags=["fastapi"],
            readme_snippet="# fastapi app",
            summary="A fastapi application",
            dependencies=[DependencyInfo(name="fastapi")],
        )
        results = rerank([r], "fastapi", enable_freshness=False)

        # Score should be higher than original due to keyword + quality boosts
        assert results[0].score > 1.0

    def test_original_results_not_mutated(self):
        """Reranking should not mutate the original SearchResult objects."""
        r = _make_result("a", "fastapi-app", 1.0, tags=["fastapi"])
        original_score = r.score
        rerank([r], "fastapi")
        assert r.score == original_score
