"""Shared filter logic for faceted search."""

from collections import Counter

from code_explore.models import Project, SearchFacets, SearchResult


def apply_filters(
    results: list[SearchResult],
    language: str | None = None,
    framework: str | None = None,
    pattern: str | None = None,
    tag: str | None = None,
) -> list[SearchResult]:
    """Apply post-filters to search results. Multiple filters combine with AND logic."""
    filtered = results

    if language:
        lang_lower = language.lower()
        filtered = [
            r for r in filtered
            if r.project.primary_language and r.project.primary_language.lower() == lang_lower
        ]

    if framework:
        fw_lower = framework.lower()
        filtered = [
            r for r in filtered
            if any(f.lower() == fw_lower for f in r.project.frameworks)
        ]

    if pattern:
        pat_lower = pattern.lower()
        filtered = [
            r for r in filtered
            if any(p.name.lower() == pat_lower for p in r.project.patterns)
        ]

    if tag:
        tag_lower = tag.lower()
        filtered = [
            r for r in filtered
            if any(t.value.lower() == tag_lower for t in r.project.ai_tags)
        ]

    return filtered


def filter_projects(
    projects: list[Project],
    language: str | None = None,
    framework: str | None = None,
    pattern: str | None = None,
    tag: str | None = None,
) -> list[Project]:
    """Apply filters directly to a list of projects."""
    filtered = projects

    if language:
        lang_lower = language.lower()
        filtered = [
            p for p in filtered
            if p.primary_language and p.primary_language.lower() == lang_lower
        ]

    if framework:
        fw_lower = framework.lower()
        filtered = [
            p for p in filtered
            if any(f.lower() == fw_lower for f in p.frameworks)
        ]

    if pattern:
        pat_lower = pattern.lower()
        filtered = [
            p for p in filtered
            if any(pt.name.lower() == pat_lower for pt in p.patterns)
        ]

    if tag:
        tag_lower = tag.lower()
        filtered = [
            p for p in filtered
            if any(t.value.lower() == tag_lower for t in p.ai_tags)
        ]

    return filtered


def compute_facets(projects: list[Project]) -> SearchFacets:
    """Compute facet counts from a list of projects."""
    languages: Counter[str] = Counter()
    frameworks: Counter[str] = Counter()
    patterns: Counter[str] = Counter()
    tags: Counter[str] = Counter()

    for p in projects:
        if p.primary_language:
            languages[p.primary_language] += 1
        for fw in p.frameworks:
            frameworks[fw] += 1
        for pat in p.patterns:
            patterns[pat.name] += 1
        for t in p.ai_tags:
            tags[t.value] += 1

    return SearchFacets(
        languages=dict(languages.most_common()),
        frameworks=dict(frameworks.most_common()),
        patterns=dict(patterns.most_common()),
        tags=dict(tags.most_common()),
        total=len(projects),
    )
