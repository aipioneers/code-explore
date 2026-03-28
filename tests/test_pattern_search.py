"""Tests for pattern-based search."""

import json

from code_explore.database import init_db, save_project
from code_explore.models import (
    PatternInfo,
    Project,
    ProjectSource,
    ProjectStatus,
    QualityMetrics,
    SearchResult,
)
from code_explore.search.pattern_search import (
    PATTERN_KEYWORDS,
    _extract_pattern_keywords,
    search_by_patterns,
)


# ---------------------------------------------------------------------------
# _extract_pattern_keywords
# ---------------------------------------------------------------------------

class TestExtractPatternKeywords:
    def test_empty_query(self):
        assert _extract_pattern_keywords("") == []

    def test_whitespace_query(self):
        assert _extract_pattern_keywords("   ") == []

    def test_single_keyword_match(self):
        patterns = _extract_pattern_keywords("react")
        assert "react" in patterns

    def test_multi_word_keyword(self):
        patterns = _extract_pattern_keywords("rest api")
        assert "rest api" in patterns

    def test_case_insensitive(self):
        patterns = _extract_pattern_keywords("Docker")
        assert "docker" in patterns

    def test_no_match(self):
        patterns = _extract_pattern_keywords("xyzzy nonsense")
        assert patterns == []

    def test_multiple_matches(self):
        patterns = _extract_pattern_keywords("react docker api")
        assert "react" in patterns
        assert "docker" in patterns
        # "api" should match "rest api" via the single-word keyword "api"
        assert "rest api" in patterns

    def test_substring_not_matched_for_single_word(self):
        """Single-word keywords require token equality, not substring."""
        patterns = _extract_pattern_keywords("reactionary")
        # "reactionary" is not the token "react"
        assert "react" not in patterns

    def test_multi_word_substring_match(self):
        """Multi-word keywords match as substrings of the full query."""
        patterns = _extract_pattern_keywords("I need a rest api framework")
        assert "rest api" in patterns


# ---------------------------------------------------------------------------
# search_by_patterns (integration, uses tmp DB)
# ---------------------------------------------------------------------------

def _make_project_with_patterns(pid, name, pattern_names):
    """Helper: create a Project whose patterns field has the given names."""
    patterns = [
        PatternInfo(name=pn, category="test", confidence=0.8)
        for pn in pattern_names
    ]
    return Project(
        id=pid,
        name=name,
        path=f"/tmp/{name}",
        source=ProjectSource.LOCAL,
        status=ProjectStatus.ANALYZED,
        patterns=patterns,
        quality=QualityMetrics(total_files=1, total_lines=10),
    )


class TestSearchByPatterns:
    def test_returns_empty_when_no_patterns_match_query(self, tmp_db):
        proj = _make_project_with_patterns("p1", "my-proj", ["REST API"])
        save_project(proj, db_path=tmp_db)
        results = search_by_patterns("xyzzy nonsense", db_path=tmp_db)
        assert results == []

    def test_returns_empty_on_empty_query(self, tmp_db):
        proj = _make_project_with_patterns("p1", "my-proj", ["REST API"])
        save_project(proj, db_path=tmp_db)
        results = search_by_patterns("", db_path=tmp_db)
        assert results == []

    def test_matches_single_pattern(self, tmp_db):
        proj = _make_project_with_patterns("p1", "api-service", ["REST API"])
        save_project(proj, db_path=tmp_db)
        results = search_by_patterns("rest api", db_path=tmp_db)
        assert len(results) == 1
        assert results[0].project.id == "p1"
        assert results[0].match_type == "pattern"

    def test_highlights_contain_pattern_name(self, tmp_db):
        proj = _make_project_with_patterns("p1", "api-service", ["REST API"])
        save_project(proj, db_path=tmp_db)
        results = search_by_patterns("api", db_path=tmp_db)
        assert len(results) == 1
        assert any("rest api" in h for h in results[0].highlights)

    def test_scores_higher_for_more_matching_patterns(self, tmp_db):
        proj1 = _make_project_with_patterns("p1", "full-stack", ["REST API", "Docker", "React"])
        proj2 = _make_project_with_patterns("p2", "simple-api", ["REST API"])
        save_project(proj1, db_path=tmp_db)
        save_project(proj2, db_path=tmp_db)
        results = search_by_patterns("docker api react", db_path=tmp_db)
        assert len(results) == 2
        # proj1 matches 3 patterns, proj2 matches 1 -> proj1 scores higher
        assert results[0].project.id == "p1"
        assert results[0].score > results[1].score

    def test_no_match_when_project_has_no_patterns(self, tmp_db):
        proj = Project(
            id="p1",
            name="empty-proj",
            source=ProjectSource.LOCAL,
            status=ProjectStatus.ANALYZED,
            quality=QualityMetrics(total_files=1, total_lines=10),
        )
        save_project(proj, db_path=tmp_db)
        results = search_by_patterns("docker", db_path=tmp_db)
        assert results == []

    def test_respects_limit(self, tmp_db):
        for i in range(5):
            proj = _make_project_with_patterns(f"p{i}", f"proj-{i}", ["Docker"])
            save_project(proj, db_path=tmp_db)
        results = search_by_patterns("docker", limit=2, db_path=tmp_db)
        assert len(results) == 2

    def test_returns_valid_search_results(self, tmp_db):
        proj = _make_project_with_patterns("p1", "my-app", ["JWT", "REST API"])
        save_project(proj, db_path=tmp_db)
        results = search_by_patterns("jwt api", db_path=tmp_db)
        assert len(results) == 1
        r = results[0]
        assert isinstance(r, SearchResult)
        assert r.score > 0
        assert r.match_type == "pattern"
        assert len(r.highlights) > 0

    def test_graceful_on_missing_db(self, tmp_path):
        """Should return empty list when the database does not exist."""
        fake_path = tmp_path / "nonexistent.db"
        results = search_by_patterns("docker", db_path=fake_path)
        assert results == []


# ---------------------------------------------------------------------------
# RRF integration: pattern results fused with other sources
# ---------------------------------------------------------------------------

class TestPatternInRRF:
    """Verify pattern results participate correctly in reciprocal rank fusion."""

    def test_three_source_fusion(self):
        from code_explore.search.hybrid import _reciprocal_rank_fusion

        def _make(pid, name, score, match_type):
            proj = Project(id=pid, name=name, quality=QualityMetrics(total_files=1, total_lines=1))
            return SearchResult(project=proj, score=score, match_type=match_type)

        ft = [_make("a", "proj-a", 1.0, "fulltext")]
        sem = [_make("a", "proj-a", 0.9, "semantic")]
        pat = [_make("a", "proj-a", 0.8, "pattern")]

        result = _reciprocal_rank_fusion(ft, sem, pat)
        assert len(result) == 1
        assert result[0].project.id == "a"
        assert result[0].match_type == "hybrid"
        # Score should be sum of 3 RRF contributions at rank 0
        # 3 * (1 / (60 + 0 + 1)) = 3/61 ≈ 0.04918
        expected = 3.0 / 61.0
        assert abs(result[0].score - expected) < 0.001
        # Three sources agree -> high confidence
        assert result[0].confidence >= 0.9

    def test_pattern_only_source(self):
        from code_explore.search.hybrid import _reciprocal_rank_fusion

        def _make(pid, name, score, match_type):
            proj = Project(id=pid, name=name, quality=QualityMetrics(total_files=1, total_lines=1))
            return SearchResult(project=proj, score=score, match_type=match_type)

        pat = [_make("x", "proj-x", 0.7, "pattern")]
        result = _reciprocal_rank_fusion([], [], pat)
        assert len(result) == 1
        assert result[0].project.id == "x"

    def test_pattern_boosts_existing_result(self):
        from code_explore.search.hybrid import _reciprocal_rank_fusion

        def _make(pid, name, score, match_type):
            proj = Project(id=pid, name=name, quality=QualityMetrics(total_files=1, total_lines=1))
            return SearchResult(project=proj, score=score, match_type=match_type)

        ft = [_make("a", "proj-a", 1.0, "fulltext"), _make("b", "proj-b", 0.8, "fulltext")]
        sem = [_make("b", "proj-b", 0.9, "semantic")]
        pat = [_make("a", "proj-a", 0.7, "pattern")]

        result = _reciprocal_rank_fusion(ft, sem, pat)
        scores = {r.project.id: r.score for r in result}
        # "a" appears in ft(rank 0) + pat(rank 0) = 2 sources
        # "b" appears in ft(rank 1) + sem(rank 0) = 2 sources
        # Both have 2 source hits, but "a" has rank 0 in both -> should score >= "b"
        assert scores["a"] >= scores["b"] or abs(scores["a"] - scores["b"]) < 0.01
