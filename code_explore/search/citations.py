"""Chunk-level citation extraction for cex ask answers.

Splits project text (readme_snippet + summary) into sentences, scores each
against the query by word overlap, and returns the top-N as citations.
"""

from __future__ import annotations

import re
from dataclasses import dataclass, field


@dataclass
class Citation:
    """A single chunk-level citation from a project."""

    project_id: str
    chunk: str
    relevance: float  # 0.0 – 1.0
    source_file: str


# Lightweight stop-words shared with ai/ask.py keyword extraction.
_STOP_WORDS: frozenset[str] = frozenset({
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
})

_SENTENCE_RE = re.compile(r"(?<=[.!?])\s+|\n+")


def _tokenize(text: str) -> set[str]:
    """Return lowercased content-word tokens from *text*."""
    return {
        t
        for t in re.findall(r"[a-zA-Z0-9_\-]+", text.lower())
        if t not in _STOP_WORDS and len(t) > 1
    }


def _split_sentences(text: str) -> list[str]:
    """Split *text* into non-empty, stripped sentences."""
    parts = _SENTENCE_RE.split(text)
    return [s.strip() for s in parts if s and s.strip()]


def _score_chunk(chunk_tokens: set[str], query_tokens: set[str]) -> float:
    """Compute word-overlap relevance between *chunk_tokens* and *query_tokens*.

    Returns a float in [0.0, 1.0].
    """
    if not query_tokens or not chunk_tokens:
        return 0.0
    overlap = chunk_tokens & query_tokens
    return len(overlap) / len(query_tokens)


def extract_citations(
    query: str,
    projects: list[dict],
    max_citations: int = 5,
) -> list[Citation]:
    """Extract the most relevant sentence-level citations from *projects*.

    Parameters
    ----------
    query:
        The user's question.
    projects:
        Dicts with at least ``id``, ``name``, and optionally
        ``readme_snippet`` and ``summary``.
    max_citations:
        Maximum number of citations to return.

    Returns
    -------
    list[Citation]
        Top citations sorted by descending relevance.
    """
    query_tokens = _tokenize(query)
    if not query_tokens:
        return []

    candidates: list[Citation] = []

    for proj in projects:
        pid = proj.get("id", "")
        name = proj.get("name", pid)
        readme = proj.get("readme_snippet") or ""
        summary = proj.get("summary") or ""

        # Combine text sources and split into sentence chunks.
        combined = f"{readme} {summary}".strip()
        if not combined:
            continue

        sentences = _split_sentences(combined)

        for sentence in sentences:
            chunk_tokens = _tokenize(sentence)
            score = _score_chunk(chunk_tokens, query_tokens)
            if score > 0.0:
                candidates.append(Citation(
                    project_id=pid,
                    chunk=sentence,
                    relevance=round(score, 2),
                    source_file=name,
                ))

    # Sort by relevance descending, then alphabetically by chunk for stability.
    candidates.sort(key=lambda c: (-c.relevance, c.chunk))
    return candidates[:max_citations]
