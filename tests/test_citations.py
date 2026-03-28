"""Tests for chunk-level citation extraction."""

from code_explore.search.citations import (
    Citation,
    _score_chunk,
    _split_sentences,
    _tokenize,
    extract_citations,
)


class TestTokenize:
    def test_basic(self):
        tokens = _tokenize("REST API video metadata")
        assert "rest" in tokens
        assert "api" in tokens
        assert "video" in tokens
        assert "metadata" in tokens

    def test_stop_words_removed(self):
        tokens = _tokenize("the quick and the fox")
        assert "the" not in tokens
        assert "and" not in tokens
        assert "quick" in tokens
        assert "fox" in tokens

    def test_single_char_removed(self):
        tokens = _tokenize("a b c data")
        assert "a" not in tokens
        assert "b" not in tokens
        assert "c" not in tokens
        assert "data" in tokens

    def test_empty_string(self):
        assert _tokenize("") == set()


class TestSplitSentences:
    def test_period_split(self):
        sentences = _split_sentences("Hello world. Goodbye world.")
        assert len(sentences) == 2
        assert sentences[0] == "Hello world."
        assert sentences[1] == "Goodbye world."

    def test_newline_split(self):
        sentences = _split_sentences("Line one\nLine two\nLine three")
        assert len(sentences) == 3

    def test_empty(self):
        assert _split_sentences("") == []

    def test_single_sentence(self):
        sentences = _split_sentences("Just one sentence")
        assert len(sentences) == 1
        assert sentences[0] == "Just one sentence"


class TestScoreChunk:
    def test_full_overlap(self):
        query = {"api", "rest"}
        chunk = {"api", "rest", "server"}
        assert _score_chunk(chunk, query) == 1.0

    def test_partial_overlap(self):
        query = {"api", "rest", "server"}
        chunk = {"api", "database"}
        score = _score_chunk(chunk, query)
        assert abs(score - 1 / 3) < 0.01

    def test_no_overlap(self):
        assert _score_chunk({"foo"}, {"bar"}) == 0.0

    def test_empty_query(self):
        assert _score_chunk({"foo"}, set()) == 0.0

    def test_empty_chunk(self):
        assert _score_chunk(set(), {"foo"}) == 0.0


class TestExtractCitations:
    def _make_projects(self):
        return [
            {
                "id": "p1",
                "name": "data-youtube",
                "readme_snippet": "# data-youtube\nFetch YouTube video data via API v3.",
                "summary": "YouTube data fetcher using the YouTube Data API v3 to download video metadata and playlists.",
            },
            {
                "id": "p2",
                "name": "web-dashboard",
                "readme_snippet": "# web-dashboard\nAdmin dashboard for system monitoring.",
                "summary": "Admin dashboard built with React and Next.js for monitoring system metrics.",
            },
        ]

    def test_returns_citations(self):
        projects = self._make_projects()
        citations = extract_citations("YouTube API video", projects)
        assert len(citations) > 0
        assert all(isinstance(c, Citation) for c in citations)

    def test_relevance_sorted_descending(self):
        projects = self._make_projects()
        citations = extract_citations("YouTube API video data", projects)
        for i in range(len(citations) - 1):
            assert citations[i].relevance >= citations[i + 1].relevance

    def test_max_citations_respected(self):
        projects = self._make_projects()
        citations = extract_citations("YouTube API video", projects, max_citations=2)
        assert len(citations) <= 2

    def test_no_match_returns_empty(self):
        projects = self._make_projects()
        citations = extract_citations("quantum physics entanglement", projects)
        assert citations == []

    def test_empty_projects_returns_empty(self):
        citations = extract_citations("test query", [])
        assert citations == []

    def test_empty_query_returns_empty(self):
        projects = self._make_projects()
        citations = extract_citations("", projects)
        assert citations == []

    def test_stop_word_only_query_returns_empty(self):
        projects = self._make_projects()
        citations = extract_citations("the and is", projects)
        assert citations == []

    def test_projects_without_text_skipped(self):
        projects = [
            {"id": "p1", "name": "empty-proj", "readme_snippet": None, "summary": None},
        ]
        citations = extract_citations("anything", projects)
        assert citations == []

    def test_citation_fields(self):
        projects = self._make_projects()
        citations = extract_citations("YouTube API", projects)
        assert len(citations) > 0
        cit = citations[0]
        assert cit.project_id in ("p1", "p2")
        assert isinstance(cit.chunk, str)
        assert 0.0 < cit.relevance <= 1.0
        assert isinstance(cit.source_file, str)

    def test_relevance_prefers_better_match(self):
        projects = self._make_projects()
        citations = extract_citations("YouTube video data API", projects)
        # The youtube project should produce the top citation
        top = citations[0]
        assert top.project_id == "p1"
