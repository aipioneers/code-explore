"""Chunk-level citation extraction for cex ask answers.

Splits project text (readme_snippet + summary) into paragraphs or sentences
(based on document length), detects section headers for context, scores each
chunk against the query using TF-IDF-like weighting with bigram and proximity
bonuses, and returns the top-N as citations with surrounding context.
"""

from __future__ import annotations

import math
import re
from dataclasses import dataclass, field


@dataclass
class Citation:
    """A single chunk-level citation from a project."""

    project_id: str
    chunk: str
    relevance: float  # 0.0 – 1.0
    source_file: str
    section_title: str | None = None
    context: str | None = None


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
_PARAGRAPH_RE = re.compile(r"\n\s*\n")
_HEADER_RE = re.compile(r"^(#{1,6})\s+(.+)$", re.MULTILINE)
_ALLCAPS_HEADER_RE = re.compile(r"^([A-Z][A-Z _\-]{2,})$", re.MULTILINE)
_WORD_RE = re.compile(r"[a-zA-Z0-9_\-]+")

# Paragraph count threshold: >= this value triggers paragraph-level chunking.
_PARAGRAPH_THRESHOLD = 5


def _tokenize(text: str) -> set[str]:
    """Return lowercased content-word tokens from *text*."""
    return {
        t
        for t in re.findall(r"[a-zA-Z0-9_\-]+", text.lower())
        if t not in _STOP_WORDS and len(t) > 1
    }


def _tokenize_ordered(text: str) -> list[str]:
    """Return lowercased content-word tokens preserving order."""
    return [
        t
        for t in (w.lower() for w in _WORD_RE.findall(text))
        if t not in _STOP_WORDS and len(t) > 1
    ]


def _split_sentences(text: str) -> list[str]:
    """Split *text* into non-empty, stripped sentences."""
    parts = _SENTENCE_RE.split(text)
    return [s.strip() for s in parts if s and s.strip()]


def _split_paragraphs(text: str) -> list[str]:
    """Split *text* into non-empty paragraphs (separated by blank lines)."""
    parts = _PARAGRAPH_RE.split(text)
    return [p.strip() for p in parts if p and p.strip()]


def _detect_sections(text: str) -> list[tuple[str | None, str]]:
    """Detect section headers and return (title, body) pairs.

    Recognises Markdown headers (# Title) and ALL CAPS lines as section
    boundaries.  Text before the first header gets ``title=None``.

    Returns
    -------
    list[tuple[str | None, str]]
        Each element is ``(section_title_or_None, section_body_text)``.
    """
    # Collect header positions: (start_of_line, end_of_line, title_text)
    headers: list[tuple[int, int, str]] = []

    for m in _HEADER_RE.finditer(text):
        headers.append((m.start(), m.end(), m.group(2).strip()))

    for m in _ALLCAPS_HEADER_RE.finditer(text):
        # Avoid matching short words that happen to be caps inside sentences.
        title = m.group(1).strip()
        if len(title) >= 3:
            headers.append((m.start(), m.end(), title))

    # De-duplicate and sort by position.
    headers.sort(key=lambda h: h[0])

    if not headers:
        return [(None, text)]

    sections: list[tuple[str | None, str]] = []

    # Text before the first header.
    pre = text[: headers[0][0]].strip()
    if pre:
        sections.append((None, pre))

    for idx, (_, end, title) in enumerate(headers):
        next_start = headers[idx + 1][0] if idx + 1 < len(headers) else len(text)
        body = text[end:next_start].strip()
        if body:
            sections.append((title, body))

    return sections


def _chunk_text(text: str) -> list[tuple[str | None, str]]:
    """Split *text* into chunks with associated section titles.

    Uses paragraph-level chunking when the document has >= 5 paragraphs,
    otherwise falls back to sentence-level chunking.

    Returns
    -------
    list[tuple[str | None, str]]
        Each element is ``(section_title_or_None, chunk_text)``.
    """
    sections = _detect_sections(text)

    # Decide chunking strategy based on total paragraph count.
    all_paragraphs = _split_paragraphs(text)
    use_paragraphs = len(all_paragraphs) >= _PARAGRAPH_THRESHOLD

    chunks: list[tuple[str | None, str]] = []
    for title, body in sections:
        if use_paragraphs:
            parts = _split_paragraphs(body)
        else:
            parts = _split_sentences(body)

        for part in parts:
            if part.strip():
                chunks.append((title, part.strip()))

    return chunks


def _get_bigrams(tokens: list[str]) -> set[tuple[str, str]]:
    """Return the set of consecutive token pairs."""
    return {(tokens[i], tokens[i + 1]) for i in range(len(tokens) - 1)}


def _proximity_score(chunk_text: str, query_tokens: set[str]) -> float:
    """Score how close together query terms appear in *chunk_text*.

    Returns a float in [0.0, 1.0].  Higher means the matching query terms
    are clustered more tightly in the chunk.
    """
    ordered = _tokenize_ordered(chunk_text)
    if not ordered or len(query_tokens) < 2:
        return 0.0

    # Find positions of query-term occurrences.
    positions = [i for i, t in enumerate(ordered) if t in query_tokens]
    if len(positions) < 2:
        return 0.0

    # Average gap between consecutive matched positions.
    gaps = [positions[i + 1] - positions[i] for i in range(len(positions) - 1)]
    avg_gap = sum(gaps) / len(gaps)

    # Normalise: gap of 1 (adjacent) -> 1.0, larger gaps decay.
    return 1.0 / (1.0 + avg_gap - 1.0)  # == 1.0 / avg_gap


def _compute_idf(term: str, all_chunks: list[str]) -> float:
    """Compute inverse document frequency of *term* across *all_chunks*."""
    if not all_chunks:
        return 1.0
    doc_count = sum(1 for c in all_chunks if term in c.lower())
    if doc_count == 0:
        return 1.0
    return math.log(len(all_chunks) / doc_count) + 1.0


def _score_chunk(
    chunk_tokens: set[str],
    query_tokens: set[str],
    *,
    chunk_text: str = "",
    query_text: str = "",
    idf_map: dict[str, float] | None = None,
) -> float:
    """Compute relevance between *chunk_tokens* and *query_tokens*.

    Uses TF-IDF-like weighting, bigram matching bonus, and proximity bonus.
    Returns a float in [0.0, 1.0].
    """
    if not query_tokens or not chunk_tokens:
        return 0.0

    overlap = chunk_tokens & query_tokens
    if not overlap:
        return 0.0

    # --- Base score: IDF-weighted overlap ---
    if idf_map:
        weighted = sum(idf_map.get(t, 1.0) for t in overlap)
        max_possible = sum(idf_map.get(t, 1.0) for t in query_tokens)
        base = weighted / max_possible if max_possible else 0.0
    else:
        base = len(overlap) / len(query_tokens)

    # --- Bigram bonus ---
    bigram_bonus = 0.0
    if chunk_text and query_text:
        query_ordered = _tokenize_ordered(query_text)
        chunk_ordered = _tokenize_ordered(chunk_text)
        if len(query_ordered) >= 2 and len(chunk_ordered) >= 2:
            q_bigrams = _get_bigrams(query_ordered)
            c_bigrams = _get_bigrams(chunk_ordered)
            shared = q_bigrams & c_bigrams
            if shared:
                bigram_bonus = len(shared) / len(q_bigrams)

    # --- Proximity bonus ---
    prox_bonus = 0.0
    if chunk_text and len(overlap) >= 2:
        prox_bonus = _proximity_score(chunk_text, query_tokens)

    # Combine: base (weight 0.6) + bigram (weight 0.25) + proximity (weight 0.15)
    combined = 0.6 * base + 0.25 * bigram_bonus + 0.15 * prox_bonus

    return min(combined, 1.0)


def _build_context(
    chunks: list[str],
    chunk_idx: int,
) -> str:
    """Build a context window: 1 chunk before + the chunk + 1 chunk after."""
    parts: list[str] = []
    if chunk_idx > 0:
        parts.append(chunks[chunk_idx - 1])
    parts.append(chunks[chunk_idx])
    if chunk_idx < len(chunks) - 1:
        parts.append(chunks[chunk_idx + 1])
    return " ".join(parts)


def extract_citations(
    query: str,
    projects: list[dict],
    max_citations: int = 5,
) -> list[Citation]:
    """Extract the most relevant chunk-level citations from *projects*.

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

    # First pass: collect all chunk texts for IDF computation.
    all_chunk_texts: list[str] = []
    project_chunks: list[
        tuple[str, str, list[tuple[str | None, str]], list[str]]
    ] = []  # (pid, name, chunks_with_titles, raw_chunk_texts)

    for proj in projects:
        pid = proj.get("id", "")
        name = proj.get("name", pid)
        readme = proj.get("readme_snippet") or ""
        summary = proj.get("summary") or ""

        combined = f"{readme} {summary}".strip()
        if not combined:
            continue

        titled_chunks = _chunk_text(combined)
        raw_texts = [c[1] for c in titled_chunks]
        all_chunk_texts.extend(raw_texts)
        project_chunks.append((pid, name, titled_chunks, raw_texts))

    # Build IDF map for query terms.
    idf_map = {t: _compute_idf(t, all_chunk_texts) for t in query_tokens}

    candidates: list[Citation] = []

    for pid, name, titled_chunks, raw_texts in project_chunks:
        for idx, (section_title, chunk_text) in enumerate(titled_chunks):
            chunk_tokens = _tokenize(chunk_text)
            score = _score_chunk(
                chunk_tokens,
                query_tokens,
                chunk_text=chunk_text,
                query_text=query,
                idf_map=idf_map,
            )
            if score > 0.0:
                context = _build_context(raw_texts, idx)
                candidates.append(
                    Citation(
                        project_id=pid,
                        chunk=chunk_text,
                        relevance=round(score, 4),
                        source_file=name,
                        section_title=section_title,
                        context=context,
                    )
                )

    # Sort by relevance descending, then alphabetically by chunk for stability.
    candidates.sort(key=lambda c: (-c.relevance, c.chunk))
    return candidates[:max_citations]
