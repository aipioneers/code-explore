"""Shared test fixtures."""

from datetime import datetime
from pathlib import Path

import pytest

from code_explore.config import reset_config
from code_explore.database import init_db, save_project, get_connection
from code_explore.models import (
    DependencyInfo,
    GitInfo,
    LanguageInfo,
    PatternInfo,
    Project,
    ProjectSource,
    ProjectStatus,
    QualityMetrics,
)


@pytest.fixture(autouse=True)
def _reset_config_singleton():
    """Reset the config singleton before each test to prevent leakage."""
    reset_config()
    yield
    reset_config()


@pytest.fixture
def tmp_db(tmp_path):
    """Create a temporary SQLite database."""
    db_path = tmp_path / "test.db"
    init_db(db_path)
    return db_path


@pytest.fixture
def sample_project():
    """Create a sample project for testing."""
    return Project(
        id="test123",
        name="data-youtube",
        path="/home/user/projects/data-youtube",
        source=ProjectSource.LOCAL,
        status=ProjectStatus.ANALYZED,
        languages=[
            LanguageInfo(name="Python", files=12, lines=1500, percentage=80.0),
            LanguageInfo(name="JavaScript", files=3, lines=200, percentage=10.0),
        ],
        primary_language="Python",
        frameworks=["FastAPI", "SQLAlchemy"],
        dependencies=[
            DependencyInfo(name="fastapi", version="0.100.0"),
            DependencyInfo(name="pytest", version="8.0.0", dev=True),
        ],
        patterns=[
            PatternInfo(name="REST API", category="architecture", confidence=0.9),
        ],
        quality=QualityMetrics(
            total_files=15,
            total_lines=1700,
            avg_file_size=113.0,
            has_tests=True,
            has_readme=True,
            has_license=True,
            has_gitignore=True,
        ),
        git=GitInfo(
            remote_url="https://github.com/user/data-youtube",
            default_branch="main",
            total_commits=42,
            last_commit_date=datetime(2026, 3, 1),
            last_commit_message="fix: handle empty playlists",
            contributors=["user"],
        ),
        summary="YouTube data fetcher using the YouTube Data API v3 to download video metadata and playlists.",
        tags=["youtube", "api", "data"],
        concepts=["REST API", "video metadata", "playlist management"],
        readme_snippet="# data-youtube\nFetch YouTube video data via API v3.",
        git_head="abc123",
        scanned_at=datetime(2026, 3, 1),
        analyzed_at=datetime(2026, 3, 1),
    )


@pytest.fixture
def sample_project_2():
    """Create a second sample project."""
    return Project(
        id="test456",
        name="web-dashboard",
        path="/home/user/projects/web-dashboard",
        source=ProjectSource.LOCAL,
        status=ProjectStatus.INDEXED,
        languages=[
            LanguageInfo(name="TypeScript", files=30, lines=5000, percentage=90.0),
        ],
        primary_language="TypeScript",
        frameworks=["React", "Next.js"],
        quality=QualityMetrics(total_files=35, total_lines=5500, has_tests=True, has_readme=True),
        git=GitInfo(total_commits=120, default_branch="main"),
        summary="Admin dashboard built with React and Next.js for monitoring system metrics.",
        tags=["react", "dashboard", "monitoring"],
        concepts=["SPA", "server-side rendering", "real-time metrics"],
        readme_snippet="# web-dashboard\nAdmin dashboard for system monitoring.",
    )


@pytest.fixture
def populated_db(tmp_db, sample_project, sample_project_2):
    """Database populated with two sample projects."""
    save_project(sample_project, db_path=tmp_db)
    save_project(sample_project_2, db_path=tmp_db)
    return tmp_db
