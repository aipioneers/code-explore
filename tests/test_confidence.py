"""Tests for confidence scoring in hybrid search results."""

from code_explore.models import Project, QualityMetrics, SearchResult
from code_explore.search.hybrid import (
    _compute_confidence,
    _reciprocal_rank_fusion,
    _SINGLE_SOURCE_CONFIDENCE,
)


def _make_result(pid: str, name: str, score: float, match_type: str = "fulltext") -> SearchResult:
    project = Project(
        id=pid,
        name=name,
        quality=QualityMetrics(total_files=1, total_lines=10),
    )
    return SearchResult(project=project, score=score, match_type=match_type)


class TestComputeConfidence:
    """Unit tests for the _compute_confidence helper."""

    def test_both_sources_high_rank(self):
        """Result at rank 0 in both lists should get confidence near 1.0."""
        conf = _compute_confidence(
            pid="a",
            fulltext_ids={"a"},
            semantic_ids={"a"},
            fulltext_ranks={"a": 0},
            semantic_ranks={"a": 0},
            total_results=10,
        )
        assert conf >= 0.9
        assert conf <= 1.0

    def test_both_sources_low_rank(self):
        """Result at the bottom of both lists should get ~0.7 (base, no rank bonus)."""
        conf = _compute_confidence(
            pid="a",
            fulltext_ids={"a"},
            semantic_ids={"a"},
            fulltext_ranks={"a": 10},
            semantic_ranks={"a": 10},
            total_results=10,
        )
        assert 0.65 <= conf <= 0.75

    def test_single_source_fulltext(self):
        """Result only in fulltext should get around 0.5-0.7."""
        conf = _compute_confidence(
            pid="a",
            fulltext_ids={"a"},
            semantic_ids=set(),
            fulltext_ranks={"a": 0},
            semantic_ranks={},
            total_results=10,
        )
        assert conf >= _SINGLE_SOURCE_CONFIDENCE
        assert conf < 0.75

    def test_single_source_semantic(self):
        """Result only in semantic should get around 0.5-0.7."""
        conf = _compute_confidence(
            pid="b",
            fulltext_ids=set(),
            semantic_ids={"b"},
            fulltext_ranks={},
            semantic_ranks={"b": 0},
            total_results=10,
        )
        assert conf >= _SINGLE_SOURCE_CONFIDENCE
        assert conf < 0.75

    def test_single_source_low_rank(self):
        """Single-source result at low rank should be near 0.5."""
        conf = _compute_confidence(
            pid="a",
            fulltext_ids={"a"},
            semantic_ids=set(),
            fulltext_ranks={"a": 10},
            semantic_ranks={},
            total_results=10,
        )
        assert abs(conf - _SINGLE_SOURCE_CONFIDENCE) < 0.05

    def test_confidence_range(self):
        """Confidence should always be between 0.0 and 1.0."""
        for total in [1, 5, 100]:
            for rank in range(total + 1):
                # Both sources
                c = _compute_confidence(
                    "x", {"x"}, {"x"}, {"x": rank}, {"x": rank}, total
                )
                assert 0.0 <= c <= 1.0
                # Single source
                c = _compute_confidence(
                    "x", {"x"}, set(), {"x": rank}, {}, total
                )
                assert 0.0 <= c <= 1.0


class TestConfidenceInRRF:
    """Verify that _reciprocal_rank_fusion populates the confidence field."""

    def test_overlapping_results_high_confidence(self):
        ft = [_make_result("a", "proj-a", 1.0)]
        sem = [_make_result("a", "proj-a", 0.9, "semantic")]
        result = _reciprocal_rank_fusion(ft, sem)
        assert len(result) == 1
        # Both sources agree at rank 0 -> high confidence
        assert result[0].confidence >= 0.9

    def test_disjoint_results_lower_confidence(self):
        ft = [_make_result("a", "proj-a", 1.0)]
        sem = [_make_result("b", "proj-b", 0.9, "semantic")]
        result = _reciprocal_rank_fusion(ft, sem)
        assert len(result) == 2
        # Each only in one source -> confidence around 0.5-0.7
        for r in result:
            assert r.confidence >= _SINGLE_SOURCE_CONFIDENCE
            assert r.confidence < 0.75

    def test_mixed_overlap(self):
        ft = [
            _make_result("a", "proj-a", 1.0),
            _make_result("b", "proj-b", 0.8),
        ]
        sem = [
            _make_result("a", "proj-a", 0.95, "semantic"),
            _make_result("c", "proj-c", 0.7, "semantic"),
        ]
        result = _reciprocal_rank_fusion(ft, sem)
        conf = {r.project.id: r.confidence for r in result}
        # "a" in both -> higher confidence than "b" or "c" (single source each)
        assert conf["a"] > conf["b"]
        assert conf["a"] > conf["c"]

    def test_empty_inputs(self):
        result = _reciprocal_rank_fusion([], [])
        assert result == []

    def test_confidence_field_present(self):
        """Every SearchResult from RRF must have a confidence field."""
        ft = [
            _make_result("a", "proj-a", 1.0),
            _make_result("b", "proj-b", 0.8),
        ]
        sem = [
            _make_result("b", "proj-b", 0.9, "semantic"),
            _make_result("c", "proj-c", 0.7, "semantic"),
        ]
        results = _reciprocal_rank_fusion(ft, sem)
        for r in results:
            assert hasattr(r, "confidence")
            assert 0.0 <= r.confidence <= 1.0


class TestSearchResultModel:
    """Verify the SearchResult model accepts and defaults confidence."""

    def test_default_confidence(self):
        r = _make_result("x", "proj-x", 0.5)
        assert r.confidence == 0.0

    def test_explicit_confidence(self):
        project = Project(id="x", name="proj-x")
        r = SearchResult(
            project=project,
            score=0.8,
            match_type="hybrid",
            confidence=0.95,
        )
        assert r.confidence == 0.95

    def test_serialization_roundtrip(self):
        project = Project(id="x", name="proj-x")
        r = SearchResult(
            project=project,
            score=0.8,
            match_type="hybrid",
            confidence=0.85,
        )
        json_str = r.model_dump_json()
        restored = SearchResult.model_validate_json(json_str)
        assert restored.confidence == 0.85
