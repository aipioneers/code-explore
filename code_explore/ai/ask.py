"""AI-powered codebase Q&A — local search and cloud proxy."""

import json
import re
import sqlite3
from datetime import date
from pathlib import Path

import httpx

from code_explore.database import get_connection, get_db_path


# ---------------------------------------------------------------------------
# Local query engine (no LLM — keyword-based aggregation over project DB)
# ---------------------------------------------------------------------------


async def ask_local(question: str, db_path: str | None = None) -> dict:
    """Answer a question by finding relevant projects in the local database.

    Extracts keywords from the question, queries the SQLite FTS5 index for
    matching projects, and assembles a structured answer from their summaries.
    No LLM is invoked — this is a deterministic retrieval-only approach.
    """
    db = Path(db_path) if db_path else get_db_path()
    conn = get_connection(db)

    # Extract meaningful keywords (strip common stop-words)
    stop_words = {
        "a", "an", "the", "is", "are", "was", "were", "be", "been", "being",
        "have", "has", "had", "do", "does", "did", "will", "would", "shall",
        "should", "may", "might", "must", "can", "could", "about", "above",
        "after", "before", "between", "into", "through", "during", "for",
        "with", "at", "by", "from", "of", "on", "in", "to", "and", "but",
        "or", "not", "no", "so", "if", "then", "than", "that", "this",
        "what", "which", "who", "whom", "how", "when", "where", "why",
        "all", "each", "every", "any", "few", "more", "most", "my", "your",
        "i", "me", "we", "us", "you", "he", "she", "it", "they", "them",
        "its", "his", "her", "our", "their",
    }
    tokens = re.findall(r"[a-zA-Z0-9_\-]+", question.lower())
    keywords = [t for t in tokens if t not in stop_words and len(t) > 1]

    if not keywords:
        conn.close()
        return {
            "answer": "I couldn't extract meaningful keywords from your question. Please rephrase.",
            "references": [],
            "confidence": "low",
        }

    # Build FTS5 query — OR-combine keywords so partial matches still rank
    fts_query = " OR ".join(f'"{kw}"' for kw in keywords)

    try:
        rows = conn.execute(
            """
            SELECT p.id, p.name, p.summary, p.tags, p.path,
                   p.readme_snippet, fts.rank
            FROM projects_fts fts
            JOIN projects p ON p.rowid = fts.rowid
            WHERE projects_fts MATCH ?
            ORDER BY fts.rank
            LIMIT 10
            """,
            (fts_query,),
        ).fetchall()
    except sqlite3.OperationalError:
        rows = []
    conn.close()

    if not rows:
        return {
            "answer": f"No projects matched your question about: {', '.join(keywords)}.",
            "references": [],
            "confidence": "low",
        }

    # Build answer by aggregating summaries of top matches
    references: list[dict] = []
    matched_projects: list[dict] = []
    summary_parts: list[str] = []

    for row in rows:
        name = row["name"]
        summary = row["summary"] or ""
        tags = row["tags"] or ""
        path = row["path"] or ""
        readme_snippet = row["readme_snippet"] or ""
        rank = abs(row["rank"])  # FTS5 rank is negative; lower magnitude = better

        # Normalise relevance to 0-1 (best match ≈ 1.0)
        relevance = round(min(1.0, 1.0 / (1.0 + rank)), 2)

        references.append({
            "file": path or name,
            "relevance": relevance,
        })

        matched_projects.append({
            "id": row["id"],
            "name": name,
            "summary": summary,
            "readme_snippet": readme_snippet,
        })

        if summary:
            summary_parts.append(f"- **{name}**: {summary.strip()}")

    # Determine confidence from match quality
    top_relevance = references[0]["relevance"] if references else 0
    if top_relevance >= 0.6:
        confidence = "high"
    elif top_relevance >= 0.3:
        confidence = "medium"
    else:
        confidence = "low"

    answer_text = (
        f"Based on {len(references)} matching project(s) in your local knowledge base:\n\n"
        + "\n".join(summary_parts[:5])
    )

    return {
        "answer": answer_text,
        "references": references,
        "confidence": confidence,
        "projects": matched_projects,
    }


# ---------------------------------------------------------------------------
# Cloud query proxy
# ---------------------------------------------------------------------------


async def ask_cloud(
    question: str,
    api_url: str,
    token: str,
    project_id: str | None = None,
    conversation_id: str | None = None,
) -> dict:
    """Forward a question to the Pioneers cloud AI endpoint.

    POST {api_url}/api/ai/ask with the question payload and return
    the response JSON as-is.
    """
    payload: dict = {"question": question}
    if project_id:
        payload["project_id"] = project_id
    if conversation_id:
        payload["conversation_id"] = conversation_id

    async with httpx.AsyncClient(timeout=30) as client:
        resp = await client.post(
            f"{api_url}/api/ai/ask",
            json=payload,
            headers={"Authorization": f"Bearer {token}"},
        )
        resp.raise_for_status()
        return resp.json()


# ---------------------------------------------------------------------------
# Local quota tracking  (free tier: 10 questions / day)
# ---------------------------------------------------------------------------

_DEFAULT_QUOTA_PATH = Path.home() / ".pioneers" / "ai_quota.json"
_FREE_TIER_LIMIT = 10


def _read_quota(quota_path: Path) -> dict:
    """Read quota file, returning a dict with 'date' and 'count' keys."""
    if not quota_path.is_file():
        return {"date": str(date.today()), "count": 0}
    try:
        data = json.loads(quota_path.read_text(encoding="utf-8"))
        return data
    except (json.JSONDecodeError, KeyError):
        return {"date": str(date.today()), "count": 0}


def check_local_quota(
    quota_path: Path = _DEFAULT_QUOTA_PATH,
) -> tuple[bool, int, int]:
    """Check whether the user is within the free-tier daily quota.

    Returns (allowed, used_today, daily_limit).
    Resets automatically when the date rolls over.
    """
    data = _read_quota(quota_path)
    today = str(date.today())

    if data.get("date") != today:
        # New day — reset
        return True, 0, _FREE_TIER_LIMIT

    used = int(data.get("count", 0))
    return used < _FREE_TIER_LIMIT, used, _FREE_TIER_LIMIT


def increment_local_quota(
    quota_path: Path = _DEFAULT_QUOTA_PATH,
) -> None:
    """Increment today's question count, creating the file/dir as needed."""
    quota_path.parent.mkdir(parents=True, exist_ok=True)

    data = _read_quota(quota_path)
    today = str(date.today())

    if data.get("date") != today:
        data = {"date": today, "count": 0}

    data["count"] = int(data.get("count", 0)) + 1
    quota_path.write_text(json.dumps(data), encoding="utf-8")
