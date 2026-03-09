"""Generate AI summaries of projects using local Ollama."""

import logging

import httpx

from code_explore.models import Project

logger = logging.getLogger(__name__)


def _build_prompt(project: Project) -> str:
    parts = [f"Project: {project.name}"]

    if project.git.remote_url:
        parts.append(f"Repository: {project.git.remote_url}")

    if project.readme_snippet:
        parts.append(f"\nREADME (excerpt):\n{project.readme_snippet}")

    if project.key_files:
        parts.append(f"\nKey files: {', '.join(project.key_files[:40])}")

    if project.primary_language:
        parts.append(f"\nPrimary language: {project.primary_language}")

    languages = [lang.name for lang in project.languages]
    if languages:
        parts.append(f"Languages: {', '.join(languages)}")

    if project.frameworks:
        parts.append(f"Frameworks: {', '.join(project.frameworks)}")

    deps = [d.name for d in project.dependencies[:30]]
    if deps:
        parts.append(f"Dependencies: {', '.join(deps)}")

    patterns = [p.name for p in project.patterns]
    if patterns:
        parts.append(f"Detected patterns: {', '.join(patterns)}")

    if project.quality.total_files:
        parts.append(f"Total files: {project.quality.total_files}, Total lines: {project.quality.total_lines}")

    if project.path:
        parts.append(f"Path: {project.path}")

    context = "\n".join(parts)

    return f"""You are analyzing a software project. Your job is to figure out WHAT this project actually does — its concrete purpose and functionality.

RULES:
- Focus on WHAT the project does, not what languages or technologies it uses.
- Be SPECIFIC: mention the actual domain, functionality, or problem it solves.
- NEVER say generic things like "utilizes various programming languages" or "a software project that uses modern technologies".
- The summary must be exactly 2 sentences.
- Tags should be domain-specific (e.g. "youtube-api", "video-download", "data-pipeline", "markdown-parser"), NOT generic (e.g. "javascript", "web", "coding").

Project information:
{context}

Respond in exactly this format (no extra lines):
SUMMARY: <exactly 2 sentences about what this project concretely does>
TAGS: tag1, tag2, tag3, ... (5-10 domain-specific tags)
CONCEPTS: concept1, concept2, concept3, ... (3-5 architectural themes)"""


def _parse_response(text: str) -> tuple[str | None, list[str], list[str]]:
    summary = None
    tags: list[str] = []
    concepts: list[str] = []

    for line in text.strip().split("\n"):
        line = line.strip()
        upper = line.upper()
        if upper.startswith("SUMMARY:"):
            summary = line[len("SUMMARY:"):].strip()
        elif upper.startswith("TAGS:"):
            raw = line[len("TAGS:"):].strip()
            tags = [t.strip() for t in raw.split(",") if t.strip()]
        elif upper.startswith("CONCEPTS:"):
            raw = line[len("CONCEPTS:"):].strip()
            concepts = [c.strip() for c in raw.split(",") if c.strip()]

    return summary, tags, concepts


def summarize_project(
    project: Project,
    model: str | None = None,
    base_url: str | None = None,
) -> tuple[str | None, list[str], list[str]]:
    from code_explore.config import get_config

    cfg = get_config()
    if model is None:
        model = cfg.summary_model
    if base_url is None:
        base_url = cfg.ollama_url
    prompt = _build_prompt(project)

    try:
        resp = httpx.post(
            f"{base_url}/api/generate",
            json={
                "model": model,
                "prompt": prompt,
                "stream": False,
                "options": {"temperature": 0.3, "num_predict": 512},
            },
            timeout=120.0,
        )
        resp.raise_for_status()
    except (httpx.ConnectError, httpx.TimeoutException):
        logger.warning("Ollama is not running at %s. Skipping summarization.", base_url)
        return None, [], []
    except httpx.HTTPStatusError as e:
        logger.error("Ollama request failed: %s", e)
        return None, [], []

    try:
        response_text = resp.json()["response"]
    except (KeyError, ValueError):
        logger.error("Unexpected Ollama response format.")
        return None, [], []

    summary, tags, concepts = _parse_response(response_text)

    if summary:
        logger.info("Generated summary for project '%s'.", project.name)
    else:
        logger.warning("Failed to parse summary from Ollama response for '%s'.", project.name)

    return summary, tags, concepts
