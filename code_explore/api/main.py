"""FastAPI REST API for Code Explore."""

import hashlib
from collections import Counter
from datetime import datetime
from pathlib import Path

from fastapi import FastAPI, HTTPException, Query
from fastapi.middleware.cors import CORSMiddleware
from fastapi.staticfiles import StaticFiles
from pydantic import BaseModel

from code_explore.database import init_db, save_project, get_project, get_all_projects, get_project_count
from code_explore.models import Project, ProjectSource, ProjectStatus, SearchFacets
from code_explore.search.filters import apply_filters, filter_projects, compute_facets

app = FastAPI(title="Code Explore", version="0.4.0", description="Developer knowledge base API")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.on_event("startup")
async def startup():
    init_db()


class ScanRequest(BaseModel):
    path: str
    depth: int = 4


class ScanResponse(BaseModel):
    scanned: int
    projects: list[Project]


class StatsResponse(BaseModel):
    total_projects: int
    languages: dict[str, int]
    frameworks: dict[str, int]
    patterns: dict[str, int]
    ai_tags: dict[str, int]
    total_files: int
    total_lines: int
    statuses: dict[str, int]


class TagInfo(BaseModel):
    value: str
    category: str
    count: int


class TagsResponse(BaseModel):
    tags: list[TagInfo]
    total_tags: int
    categories: dict[str, int]


@app.get("/api/projects", response_model=list[Project])
async def list_projects(
    language: str | None = Query(None, description="Filter by primary language"),
    framework: str | None = Query(None, description="Filter by framework"),
    source: str | None = Query(None, description="Filter by source (local, github, gitlab)"),
    tag: str | None = Query(None, description="Filter by AI tag"),
):
    projects = get_all_projects()

    if source:
        projects = [p for p in projects if p.source.value == source]

    projects = filter_projects(projects, language=language, framework=framework, tag=tag)

    return projects


@app.get("/api/projects/{project_id}", response_model=Project)
async def get_project_detail(project_id: str):
    project = get_project(project_id)
    if not project:
        raise HTTPException(status_code=404, detail="Project not found")
    return project


@app.get("/api/search")
async def search_projects(
    q: str = Query(..., description="Search query"),
    mode: str = Query("hybrid", description="Search mode: fulltext, semantic, or hybrid"),
    limit: int = Query(20, ge=1, le=100, description="Maximum results"),
    language: str | None = Query(None, description="Filter by primary language"),
    framework: str | None = Query(None, description="Filter by framework"),
    pattern: str | None = Query(None, description="Filter by pattern"),
    tag: str | None = Query(None, description="Filter by AI tag"),
):
    if mode == "fulltext":
        from code_explore.search.fulltext import search as fulltext_search
        results = fulltext_search(q, limit=limit)
    elif mode == "semantic":
        from code_explore.search.semantic import search as semantic_search
        results = semantic_search(q, limit=limit)
    else:
        from code_explore.search.hybrid import search as hybrid_search
        results = hybrid_search(q, limit=limit)

    # Apply post-filters
    results = apply_filters(results, language=language, framework=framework, pattern=pattern, tag=tag)

    return [
        {
            "project": r.project.model_dump(),
            "score": r.score,
            "match_type": r.match_type,
            "highlights": r.highlights,
        }
        for r in results
    ]


@app.get("/api/facets", response_model=SearchFacets)
async def get_facets(
    q: str | None = Query(None, description="Search query to scope facets"),
    language: str | None = Query(None, description="Active language filter"),
    framework: str | None = Query(None, description="Active framework filter"),
    pattern: str | None = Query(None, description="Active pattern filter"),
    tag: str | None = Query(None, description="Active tag filter"),
):
    if q:
        # Get search results first, then compute facets from those
        from code_explore.search.hybrid import search as hybrid_search
        results = hybrid_search(q, limit=500)
        projects = [r.project for r in results]
    else:
        projects = get_all_projects()

    # Apply active filters to scope facets
    projects = filter_projects(projects, language=language, framework=framework, pattern=pattern, tag=tag)

    return compute_facets(projects)


@app.get("/api/tags", response_model=TagsResponse)
async def get_tags(
    category: str | None = Query(None, description="Filter by category: domain, technology-role, maturity"),
):
    projects = get_all_projects()

    tag_counts: Counter[str] = Counter()
    tag_categories: dict[str, str] = {}

    for p in projects:
        for t in p.ai_tags:
            cat = t.category.value if hasattr(t.category, "value") else str(t.category)
            if category and cat != category:
                continue
            tag_counts[t.value] += 1
            tag_categories[t.value] = cat

    tags_list = [
        TagInfo(value=value, category=tag_categories[value], count=count)
        for value, count in tag_counts.most_common()
    ]

    category_counts: Counter[str] = Counter()
    for t in tags_list:
        category_counts[t.category] += 1

    return TagsResponse(
        tags=tags_list,
        total_tags=len(tags_list),
        categories=dict(category_counts),
    )


@app.get("/api/stats", response_model=StatsResponse)
async def get_stats():
    projects = get_all_projects()

    languages: dict[str, int] = {}
    frameworks: dict[str, int] = {}
    patterns: dict[str, int] = {}
    ai_tags: dict[str, int] = {}
    statuses: dict[str, int] = {}
    total_files = 0
    total_lines = 0

    for p in projects:
        if p.primary_language:
            languages[p.primary_language] = languages.get(p.primary_language, 0) + 1
        for fw in p.frameworks:
            frameworks[fw] = frameworks.get(fw, 0) + 1
        for pat in p.patterns:
            patterns[pat.name] = patterns.get(pat.name, 0) + 1
        for t in p.ai_tags:
            ai_tags[t.value] = ai_tags.get(t.value, 0) + 1
        statuses[p.status.value] = statuses.get(p.status.value, 0) + 1
        total_files += p.quality.total_files
        total_lines += p.quality.total_lines

    return StatsResponse(
        total_projects=len(projects),
        languages=dict(sorted(languages.items(), key=lambda x: x[1], reverse=True)),
        frameworks=dict(sorted(frameworks.items(), key=lambda x: x[1], reverse=True)),
        patterns=dict(sorted(patterns.items(), key=lambda x: x[1], reverse=True)),
        ai_tags=dict(sorted(ai_tags.items(), key=lambda x: x[1], reverse=True)),
        total_files=total_files,
        total_lines=total_lines,
        statuses=statuses,
    )


@app.post("/api/scan", response_model=ScanResponse)
async def trigger_scan(request: ScanRequest):
    from code_explore.scanner.local import scan_local_repos
    from code_explore.scanner.git_info import extract_git_info
    from code_explore.analyzer.language import detect_languages
    from code_explore.analyzer.metrics import calculate_metrics
    from code_explore.analyzer.dependencies import detect_dependencies
    from code_explore.analyzer.patterns import detect_patterns

    root = Path(request.path).expanduser().resolve()
    if not root.is_dir():
        raise HTTPException(status_code=400, detail=f"Path does not exist: {root}")

    repos = await scan_local_repos(root, max_depth=request.depth)
    results: list[Project] = []

    for repo_path in repos:
        pid = hashlib.md5(str(repo_path).encode()).hexdigest()[:12]

        existing = get_project(pid)
        if existing:
            results.append(existing)
            continue

        languages, primary_language = detect_languages(repo_path)
        quality = calculate_metrics(repo_path)
        dependencies = detect_dependencies(repo_path)
        detected_patterns, detected_frameworks = detect_patterns(repo_path)
        git_info = extract_git_info(repo_path)

        project = Project(
            id=pid,
            name=repo_path.name,
            path=str(repo_path),
            source=ProjectSource.LOCAL,
            status=ProjectStatus.ANALYZED,
            languages=languages,
            primary_language=primary_language,
            frameworks=detected_frameworks,
            dependencies=dependencies,
            patterns=detected_patterns,
            quality=quality,
            git=git_info,
            scanned_at=datetime.now(),
            analyzed_at=datetime.now(),
        )

        save_project(project)
        results.append(project)

    return ScanResponse(scanned=len(results), projects=results)


# Mount static files for the dashboard (must be after API routes)
_static_dir = Path(__file__).parent.parent / "static"
if _static_dir.is_dir():
    app.mount("/", StaticFiles(directory=str(_static_dir), html=True), name="static")
