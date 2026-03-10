"""Data models for Code Explore."""

from datetime import datetime
from enum import Enum

from pydantic import BaseModel, Field


class ProjectSource(str, Enum):
    LOCAL = "local"
    GITHUB = "github"
    GITLAB = "gitlab"


class ProjectStatus(str, Enum):
    PENDING = "pending"
    SCANNING = "scanning"
    ANALYZED = "analyzed"
    INDEXED = "indexed"
    ERROR = "error"


class TagCategory(str, Enum):
    DOMAIN = "domain"
    TECHNOLOGY_ROLE = "technology-role"
    MATURITY = "maturity"


class AiTag(BaseModel):
    value: str
    category: TagCategory = TagCategory.DOMAIN
    confidence: float = 0.8


class LanguageInfo(BaseModel):
    name: str
    files: int = 0
    lines: int = 0
    percentage: float = 0.0


class DependencyInfo(BaseModel):
    name: str
    version: str | None = None
    dev: bool = False
    source: str = ""


class PatternInfo(BaseModel):
    name: str
    category: str
    confidence: float = 0.0
    evidence: list[str] = Field(default_factory=list)


class QualityMetrics(BaseModel):
    total_files: int = 0
    total_lines: int = 0
    avg_file_size: float = 0.0
    max_file_size: int = 0
    has_tests: bool = False
    has_ci: bool = False
    has_docs: bool = False
    has_readme: bool = False
    has_license: bool = False
    has_gitignore: bool = False


class GitInfo(BaseModel):
    remote_url: str | None = None
    default_branch: str | None = None
    total_commits: int = 0
    last_commit_date: datetime | None = None
    last_commit_message: str | None = None
    contributors: list[str] = Field(default_factory=list)


class Project(BaseModel):
    id: str
    name: str
    path: str | None = None
    remote_url: str | None = None
    source: ProjectSource = ProjectSource.LOCAL
    status: ProjectStatus = ProjectStatus.PENDING

    languages: list[LanguageInfo] = Field(default_factory=list)
    primary_language: str | None = None
    frameworks: list[str] = Field(default_factory=list)
    dependencies: list[DependencyInfo] = Field(default_factory=list)
    patterns: list[PatternInfo] = Field(default_factory=list)
    quality: QualityMetrics = Field(default_factory=QualityMetrics)
    git: GitInfo = Field(default_factory=GitInfo)

    summary: str | None = None
    tags: list[str] = Field(default_factory=list)
    concepts: list[str] = Field(default_factory=list)
    ai_tags: list[AiTag] = Field(default_factory=list)

    readme_snippet: str | None = None
    key_files: list[str] = Field(default_factory=list)
    git_head: str | None = None

    scanned_at: datetime | None = None
    analyzed_at: datetime | None = None
    indexed_at: datetime | None = None


class SearchResult(BaseModel):
    project: Project
    score: float
    match_type: str
    highlights: list[str] = Field(default_factory=list)


class SearchQuery(BaseModel):
    query: str
    mode: str = "hybrid"
    limit: int = 20
    filters: dict = Field(default_factory=dict)
    language: str | None = None
    framework: str | None = None
    pattern: str | None = None
    tag: str | None = None


class SearchFacets(BaseModel):
    languages: dict[str, int] = Field(default_factory=dict)
    frameworks: dict[str, int] = Field(default_factory=dict)
    patterns: dict[str, int] = Field(default_factory=dict)
    tags: dict[str, int] = Field(default_factory=dict)
    total: int = 0
