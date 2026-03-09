"""Generate AI summaries of projects using local Ollama."""

import logging

import httpx

from code_explore.models import Project

logger = logging.getLogger(__name__)

OLLAMA_BASE_URL = "http://localhost:11434"
DEFAULT_MODEL = "llama3.2:3b"


def _build_prompt(project: Project) -> str:
    parts = [f"Project: {project.name}"]

    if project.primary_language:
        parts.append(f"Primary language: {project.primary_language}")

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
        parts.append(f"Patterns: {', '.join(patterns)}")

    if project.quality.total_files:
        parts.append(f"Total files: {project.quality.total_files}")
        parts.append(f"Total lines: {project.quality.total_lines}")
        parts.append(f"Has tests: {project.quality.has_tests}")
        parts.append(f"Has CI: {project.quality.has_ci}")
        parts.append(f"Has docs: {project.quality.has_docs}")

    if project.path:
        parts.append(f"Path: {project.path}")

    if project.git.remote_url:
        parts.append(f"Remote: {project.git.remote_url}")

    context = "\n".join(parts)

    return f"""Analyze this software project and provide:
1. A concise 2-3 sentence summary of what this project does and its purpose.
2. A list of 5-10 relevant tags (single words or short phrases).
3. A list of 3-5 key concepts or architectural themes.

Project information:
{context}

Respond in exactly this format:
SUMMARY: <your summary>
TAGS: tag1, tag2, tag3, ...
CONCEPTS: concept1, concept2, concept3, ..."""


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
    model: str = DEFAULT_MODEL,
    base_url: str = OLLAMA_BASE_URL,
) -> tuple[str | None, list[str], list[str]]:
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
            timeout=60.0,
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
