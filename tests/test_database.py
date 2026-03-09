"""Tests for database operations."""

from code_explore.database import (
    delete_project,
    get_all_projects,
    get_project,
    get_project_count,
    init_db,
    save_project,
    search_fulltext,
)
from code_explore.models import Project, ProjectStatus


class TestInitDb:
    def test_creates_database(self, tmp_path):
        db_path = tmp_path / "new.db"
        assert not db_path.exists()
        init_db(db_path)
        assert db_path.exists()

    def test_idempotent(self, tmp_db):
        init_db(tmp_db)
        init_db(tmp_db)
        assert tmp_db.exists()


class TestSaveAndGetProject:
    def test_save_and_retrieve(self, tmp_db, sample_project):
        save_project(sample_project, db_path=tmp_db)
        retrieved = get_project("test123", db_path=tmp_db)
        assert retrieved is not None
        assert retrieved.id == "test123"
        assert retrieved.name == "data-youtube"
        assert retrieved.primary_language == "Python"

    def test_get_nonexistent(self, tmp_db):
        result = get_project("nonexistent", db_path=tmp_db)
        assert result is None

    def test_upsert_overwrites(self, tmp_db, sample_project):
        save_project(sample_project, db_path=tmp_db)
        sample_project.summary = "Updated summary"
        save_project(sample_project, db_path=tmp_db)
        retrieved = get_project("test123", db_path=tmp_db)
        assert retrieved.summary == "Updated summary"

    def test_preserves_all_fields(self, tmp_db, sample_project):
        save_project(sample_project, db_path=tmp_db)
        retrieved = get_project("test123", db_path=tmp_db)
        assert retrieved.summary == sample_project.summary
        assert retrieved.tags == sample_project.tags
        assert retrieved.concepts == sample_project.concepts
        assert len(retrieved.languages) == 2
        assert len(retrieved.dependencies) == 2
        assert retrieved.git.total_commits == 42


class TestGetAllProjects:
    def test_empty_db(self, tmp_db):
        projects = get_all_projects(db_path=tmp_db)
        assert projects == []

    def test_returns_all(self, populated_db):
        projects = get_all_projects(db_path=populated_db)
        assert len(projects) == 2
        names = {p.name for p in projects}
        assert names == {"data-youtube", "web-dashboard"}

    def test_sorted_by_name(self, populated_db):
        projects = get_all_projects(db_path=populated_db)
        assert projects[0].name == "data-youtube"
        assert projects[1].name == "web-dashboard"


class TestDeleteProject:
    def test_delete_existing(self, tmp_db, sample_project):
        save_project(sample_project, db_path=tmp_db)
        assert get_project("test123", db_path=tmp_db) is not None
        delete_project("test123", db_path=tmp_db)
        assert get_project("test123", db_path=tmp_db) is None

    def test_delete_nonexistent(self, tmp_db):
        delete_project("nonexistent", db_path=tmp_db)  # should not raise


class TestProjectCount:
    def test_empty(self, tmp_db):
        assert get_project_count(db_path=tmp_db) == 0

    def test_with_projects(self, populated_db):
        assert get_project_count(db_path=populated_db) == 2


class TestFulltextSearch:
    def test_search_by_name(self, populated_db):
        results = search_fulltext("youtube", db_path=populated_db)
        assert len(results) >= 1
        ids = [r[0] for r in results]
        assert "test123" in ids

    def test_search_by_summary(self, populated_db):
        results = search_fulltext("dashboard monitoring", db_path=populated_db)
        assert len(results) >= 1
        ids = [r[0] for r in results]
        assert "test456" in ids

    def test_search_by_tags(self, populated_db):
        results = search_fulltext("react", db_path=populated_db)
        assert len(results) >= 1

    def test_no_results(self, populated_db):
        results = search_fulltext("nonexistentterm12345", db_path=populated_db)
        assert results == []

    def test_special_characters(self, populated_db):
        results = search_fulltext("test & special | chars", db_path=populated_db)
        assert isinstance(results, list)  # should not raise
