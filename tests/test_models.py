"""Tests for data models."""

from code_explore.models import (
    DependencyInfo,
    GitInfo,
    LanguageInfo,
    PatternInfo,
    Project,
    ProjectSource,
    ProjectStatus,
    QualityMetrics,
    SearchResult,
)


class TestProjectModel:
    def test_create_minimal_project(self):
        p = Project(id="abc", name="test-project")
        assert p.id == "abc"
        assert p.name == "test-project"
        assert p.source == ProjectSource.LOCAL
        assert p.status == ProjectStatus.PENDING
        assert p.languages == []
        assert p.frameworks == []
        assert p.summary is None

    def test_create_full_project(self, sample_project):
        assert sample_project.id == "test123"
        assert sample_project.name == "data-youtube"
        assert sample_project.primary_language == "Python"
        assert len(sample_project.languages) == 2
        assert len(sample_project.dependencies) == 2
        assert sample_project.quality.has_tests is True

    def test_project_serialization_roundtrip(self, sample_project):
        json_str = sample_project.model_dump_json()
        restored = Project.model_validate_json(json_str)
        assert restored.id == sample_project.id
        assert restored.name == sample_project.name
        assert restored.primary_language == sample_project.primary_language
        assert len(restored.languages) == len(sample_project.languages)
        assert restored.summary == sample_project.summary

    def test_project_status_enum(self):
        assert ProjectStatus.PENDING.value == "pending"
        assert ProjectStatus.INDEXED.value == "indexed"
        assert ProjectStatus.ERROR.value == "error"

    def test_project_source_enum(self):
        assert ProjectSource.LOCAL.value == "local"
        assert ProjectSource.GITHUB.value == "github"


class TestLanguageInfo:
    def test_defaults(self):
        lang = LanguageInfo(name="Python")
        assert lang.files == 0
        assert lang.lines == 0
        assert lang.percentage == 0.0

    def test_with_values(self):
        lang = LanguageInfo(name="Rust", files=10, lines=2000, percentage=75.5)
        assert lang.name == "Rust"
        assert lang.lines == 2000


class TestQualityMetrics:
    def test_defaults(self):
        q = QualityMetrics()
        assert q.total_files == 0
        assert q.has_tests is False
        assert q.has_ci is False

    def test_with_values(self):
        q = QualityMetrics(total_files=50, total_lines=10000, has_tests=True, has_ci=True)
        assert q.total_files == 50
        assert q.has_tests is True


class TestSearchResult:
    def test_create(self, sample_project):
        result = SearchResult(
            project=sample_project,
            score=0.95,
            match_type="hybrid",
            highlights=["YouTube API"],
        )
        assert result.score == 0.95
        assert result.match_type == "hybrid"
        assert "YouTube API" in result.highlights
