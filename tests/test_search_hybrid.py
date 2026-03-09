"""Tests for hybrid search / reciprocal rank fusion."""

from code_explore.models import Project, QualityMetrics, SearchResult
from code_explore.search.hybrid import _reciprocal_rank_fusion, RRF_K


def _make_result(pid: str, name: str, score: float, match_type: str = "fulltext") -> SearchResult:
    project = Project(
        id=pid,
        name=name,
        quality=QualityMetrics(total_files=1, total_lines=10),
    )
    return SearchResult(project=project, score=score, match_type=match_type)


class TestReciprocalRankFusion:
    def test_empty_inputs(self):
        result = _reciprocal_rank_fusion([], [])
        assert result == []

    def test_fulltext_only(self):
        ft = [_make_result("a", "proj-a", 1.0)]
        result = _reciprocal_rank_fusion(ft, [])
        assert len(result) == 1
        assert result[0].project.id == "a"
        assert result[0].match_type == "hybrid"

    def test_semantic_only(self):
        sem = [_make_result("b", "proj-b", 0.9, "semantic")]
        result = _reciprocal_rank_fusion([], sem)
        assert len(result) == 1
        assert result[0].project.id == "b"

    def test_overlapping_results_boosted(self):
        ft = [
            _make_result("a", "proj-a", 1.0),
            _make_result("b", "proj-b", 0.8),
        ]
        sem = [
            _make_result("a", "proj-a", 0.95, "semantic"),
            _make_result("c", "proj-c", 0.7, "semantic"),
        ]
        result = _reciprocal_rank_fusion(ft, sem)
        # "a" appears in both → should be ranked first (higher combined score)
        assert result[0].project.id == "a"
        # "a" gets score from both lists
        expected_score_a = 1.0 / (RRF_K + 1) + 1.0 / (RRF_K + 1)
        assert abs(result[0].score - expected_score_a) < 0.001

    def test_disjoint_results(self):
        ft = [_make_result("a", "proj-a", 1.0)]
        sem = [_make_result("b", "proj-b", 0.9, "semantic")]
        result = _reciprocal_rank_fusion(ft, sem)
        assert len(result) == 2
        # Both at rank 0 in their respective lists → same RRF score
        assert abs(result[0].score - result[1].score) < 0.001

    def test_ranking_order(self):
        ft = [
            _make_result("a", "proj-a", 1.0),
            _make_result("b", "proj-b", 0.8),
            _make_result("c", "proj-c", 0.6),
        ]
        sem = [
            _make_result("b", "proj-b", 0.95, "semantic"),
            _make_result("c", "proj-c", 0.7, "semantic"),
            _make_result("a", "proj-a", 0.5, "semantic"),
        ]
        result = _reciprocal_rank_fusion(ft, sem)
        # All appear in both lists.
        # a: rank 0 in ft (1/61) + rank 2 in sem (1/63)
        # b: rank 1 in ft (1/62) + rank 0 in sem (1/61)
        # c: rank 2 in ft (1/63) + rank 1 in sem (1/62)
        # b and a should have highest scores (a≈b > c)
        scores = {r.project.id: r.score for r in result}
        assert scores["b"] > scores["c"]
        assert scores["a"] > scores["c"]

    def test_highlights_merged(self):
        ft = [_make_result("a", "proj-a", 1.0)]
        ft[0].highlights = ["matched in name"]
        sem = [_make_result("a", "proj-a", 0.9, "semantic")]
        sem[0].highlights = ["matched in summary"]
        result = _reciprocal_rank_fusion(ft, sem)
        assert len(result) == 1
        assert "matched in name" in result[0].highlights
        assert "matched in summary" in result[0].highlights

    def test_no_duplicate_highlights(self):
        ft = [_make_result("a", "proj-a", 1.0)]
        ft[0].highlights = ["same match"]
        sem = [_make_result("a", "proj-a", 0.9, "semantic")]
        sem[0].highlights = ["same match"]
        result = _reciprocal_rank_fusion(ft, sem)
        assert result[0].highlights.count("same match") == 1
