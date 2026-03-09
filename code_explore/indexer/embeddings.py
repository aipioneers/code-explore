"""Generate and store embeddings using Ollama + LanceDB."""

import logging
from pathlib import Path

import httpx
import lancedb
import pyarrow as pa

from code_explore.models import Project

logger = logging.getLogger(__name__)

OLLAMA_BASE_URL = "http://localhost:11434"
EMBEDDING_MODEL = "qwen3-embedding:8b"
EMBEDDING_DIM = 4096
VECTOR_DB_PATH = Path.home() / ".code-explore" / "vectors"
TABLE_NAME = "project_embeddings"

SCHEMA = pa.schema([
    pa.field("id", pa.string()),
    pa.field("text", pa.string()),
    pa.field("vector", pa.list_(pa.float32(), EMBEDDING_DIM)),
])


def _ollama_available() -> bool:
    try:
        resp = httpx.get(f"{OLLAMA_BASE_URL}/api/tags", timeout=5.0)
        return resp.status_code == 200
    except (httpx.ConnectError, httpx.TimeoutException):
        return False


def generate_embedding(text: str) -> list[float] | None:
    try:
        resp = httpx.post(
            f"{OLLAMA_BASE_URL}/api/embeddings",
            json={"model": EMBEDDING_MODEL, "prompt": text},
            timeout=30.0,
        )
        resp.raise_for_status()
        return resp.json()["embedding"]
    except (httpx.ConnectError, httpx.TimeoutException):
        logger.warning("Ollama is not running at %s. Skipping embedding generation.", OLLAMA_BASE_URL)
        return None
    except (httpx.HTTPStatusError, KeyError) as e:
        logger.error("Failed to generate embedding: %s", e)
        return None


def _generate_embeddings_batch(texts: list[str]) -> list[list[float] | None]:
    results = []
    for text in texts:
        results.append(generate_embedding(text))
    return results


def _project_to_text(project: Project) -> str:
    parts: list[str] = []

    # Project name - repeated for weighting
    parts.append(project.name)

    # Name with summary for context
    if project.summary:
        parts.append(f"{project.name} - {project.summary}")

    # README snippet - most valuable signal
    if project.readme_snippet:
        parts.append(f"README: {project.readme_snippet}")

    # Summary standalone
    if project.summary:
        parts.append(f"Summary: {project.summary}")

    # Dependency names (without versions) - critical for discovery
    dep_names = [dep.name for dep in project.dependencies]
    if dep_names:
        parts.append(f"Dependencies: {', '.join(dep_names)}")

    # Tags and concepts
    if project.tags:
        parts.append(f"Tags: {', '.join(project.tags)}")

    if project.concepts:
        parts.append(f"Concepts: {', '.join(project.concepts)}")

    # Language names
    languages = [lang.name for lang in project.languages]
    if languages:
        parts.append(f"Languages: {', '.join(languages)}")

    # Framework names
    if project.frameworks:
        parts.append(f"Frameworks: {', '.join(project.frameworks)}")

    # Pattern names
    patterns = [p.name for p in project.patterns]
    if patterns:
        parts.append(f"Patterns: {', '.join(patterns)}")

    # Key file names
    if project.key_files:
        parts.append(f"Files: {', '.join(project.key_files)}")

    # Git remote URL contains useful project name info
    remote = project.remote_url or (project.git.remote_url if project.git else None)
    if remote:
        parts.append(f"Remote: {remote}")

    return "\n".join(parts)


def _get_table(db: lancedb.DBConnection) -> lancedb.table.Table:
    if TABLE_NAME in db.table_names():
        return db.open_table(TABLE_NAME)
    return db.create_table(TABLE_NAME, schema=SCHEMA)


def index_project(project: Project) -> None:
    if not _ollama_available():
        logger.warning("Ollama is not available. Skipping indexing for project '%s'.", project.name)
        return

    text = _project_to_text(project)
    vector = generate_embedding(text)
    if vector is None:
        return

    VECTOR_DB_PATH.mkdir(parents=True, exist_ok=True)
    db = lancedb.connect(str(VECTOR_DB_PATH))
    table = _get_table(db)

    data = [{"id": project.id, "text": text, "vector": vector}]

    existing = table.search().where(f"id = '{project.id}'", prefilter=True).limit(1).to_list()
    if existing:
        table.delete(f"id = '{project.id}'")

    table.add(data)
    logger.info("Indexed project '%s' in vector store.", project.name)


def index_all_projects(projects: list[Project]) -> None:
    if not projects:
        return

    if not _ollama_available():
        logger.warning(
            "Ollama is not available. Skipping vector indexing for %d projects.", len(projects)
        )
        return

    texts = [_project_to_text(p) for p in projects]
    vectors = _generate_embeddings_batch(texts)

    data = []
    for project, text, vector in zip(projects, texts, vectors):
        if vector is not None:
            data.append({"id": project.id, "text": text, "vector": vector})

    if not data:
        logger.warning("No embeddings generated. Skipping vector store update.")
        return

    VECTOR_DB_PATH.mkdir(parents=True, exist_ok=True)
    db = lancedb.connect(str(VECTOR_DB_PATH))
    table = _get_table(db)

    existing_ids = {item["id"] for item in data}
    for pid in existing_ids:
        try:
            found = table.search().where(f"id = '{pid}'", prefilter=True).limit(1).to_list()
            if found:
                table.delete(f"id = '{pid}'")
        except Exception:
            pass

    table.add(data)
    logger.info("Indexed %d/%d projects in vector store.", len(data), len(projects))
