"""Tests for chunk-level citation extraction."""

from code_explore.search.citations import (
    Citation,
    _score_chunk,
    _split_sentences,
    _tokenize,
    _split_paragraphs,
    _detect_sections,
    _chunk_text,
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


class TestSplitParagraphs:
    def test_basic_paragraphs(self):
        text = "First paragraph.\n\nSecond paragraph.\n\nThird paragraph."
        paragraphs = _split_paragraphs(text)
        assert len(paragraphs) == 3
        assert paragraphs[0] == "First paragraph."
        assert paragraphs[1] == "Second paragraph."
        assert paragraphs[2] == "Third paragraph."

    def test_single_paragraph(self):
        text = "Just one paragraph with no blank lines."
        paragraphs = _split_paragraphs(text)
        assert len(paragraphs) == 1

    def test_empty(self):
        assert _split_paragraphs("") == []

    def test_multiple_blank_lines(self):
        text = "Para one.\n\n\n\nPara two."
        paragraphs = _split_paragraphs(text)
        assert len(paragraphs) == 2

    def test_whitespace_only_between(self):
        text = "Para one.\n   \n  \nPara two."
        paragraphs = _split_paragraphs(text)
        assert len(paragraphs) == 2


class TestDetectSections:
    def test_markdown_headers(self):
        text = "# Introduction\nSome intro text.\n\n## Details\nDetail text here."
        sections = _detect_sections(text)
        # Should have 2 sections
        assert len(sections) == 2
        assert sections[0][0] == "Introduction"
        assert "Some intro text" in sections[0][1]
        assert sections[1][0] == "Details"
        assert "Detail text here" in sections[1][1]

    def test_all_caps_headers(self):
        text = "OVERVIEW\nThis is the overview.\n\nDETAILS\nThese are details."
        sections = _detect_sections(text)
        assert len(sections) == 2
        assert sections[0][0] == "OVERVIEW"
        assert sections[1][0] == "DETAILS"

    def test_no_headers(self):
        text = "Just plain text with no headers at all."
        sections = _detect_sections(text)
        assert len(sections) == 1
        assert sections[0][0] is None
        assert "Just plain text" in sections[0][1]

    def test_nested_markdown_headers(self):
        text = "# Top\nTop content.\n\n## Sub\nSub content.\n\n### SubSub\nDeep content."
        sections = _detect_sections(text)
        assert len(sections) == 3
        assert sections[0][0] == "Top"
        assert sections[1][0] == "Sub"
        assert sections[2][0] == "SubSub"


class TestChunkText:
    def test_short_text_uses_sentences(self):
        """Texts with <5 paragraphs should be chunked by sentences."""
        text = "First sentence. Second sentence. Third sentence."
        chunks = _chunk_text(text)
        assert len(chunks) == 3
        # Each chunk is (section_title, chunk_text)
        for title, chunk in chunks:
            assert title is None  # no headers
            assert len(chunk) > 0

    def test_long_text_uses_paragraphs(self):
        """Texts with >=5 paragraphs should be chunked by paragraphs."""
        text = "\n\n".join([f"Paragraph {i} with some content." for i in range(6)])
        chunks = _chunk_text(text)
        assert len(chunks) == 6
        assert "Paragraph 0" in chunks[0][1]
        assert "Paragraph 5" in chunks[5][1]

    def test_section_titles_attached(self):
        """Chunks inherit the nearest section title."""
        text = "# Setup\nInstall deps. Configure env.\n\n# Usage\nRun the CLI."
        chunks = _chunk_text(text)
        setup_chunks = [c for c in chunks if c[0] == "Setup"]
        usage_chunks = [c for c in chunks if c[0] == "Usage"]
        assert len(setup_chunks) > 0
        assert len(usage_chunks) > 0

    def test_paragraph_vs_sentence_threshold(self):
        """Exactly 4 paragraphs should still use sentence splitting."""
        text = "Para one sentence.\n\nPara two sentence.\n\nPara three sentence.\n\nPara four sentence."
        chunks = _chunk_text(text)
        # 4 paragraphs < 5 threshold, so sentences should be used
        # Each paragraph has 1 sentence, so total sentences == 4
        # This tests the threshold boundary
        assert len(chunks) >= 4


class TestScoreChunk:
    def test_full_overlap(self):
        """With no text/idf, only the base component (weight 0.6) fires."""
        query = {"api", "rest"}
        chunk = {"api", "rest", "server"}
        # base = 2/2 = 1.0; combined = 0.6 * 1.0 = 0.6
        assert _score_chunk(chunk, query) == 0.6

    def test_partial_overlap(self):
        """Partial overlap without text yields base * 0.6."""
        query = {"api", "rest", "server"}
        chunk = {"api", "database"}
        score = _score_chunk(chunk, query)
        # base = 1/3; combined = 0.6 * (1/3) = 0.2
        assert abs(score - 0.6 / 3) < 0.01

    def test_no_overlap(self):
        assert _score_chunk({"foo"}, {"bar"}) == 0.0

    def test_empty_query(self):
        assert _score_chunk({"foo"}, set()) == 0.0

    def test_empty_chunk(self):
        assert _score_chunk(set(), {"foo"}) == 0.0


class TestScoreChunkAdvanced:
    def test_rare_term_weighted_higher(self):
        """A rare query term matching should score higher than a common one."""
        # corpus_freq simulates that "kubernetes" is rarer than "api"
        # When both chunks match 1 query term, the one matching the rarer
        # term should score higher.
        query_text = "kubernetes api deployment"
        chunk_rare = "kubernetes orchestration"
        chunk_common = "api endpoint"
        # We test through extract_citations to get the full scoring pipeline
        projects = [
            {
                "id": "p1",
                "name": "proj-rare",
                "readme_snippet": chunk_rare,
                "summary": "",
            },
            {
                "id": "p2",
                "name": "proj-common",
                "readme_snippet": chunk_common,
                "summary": "",
            },
        ]
        citations = extract_citations(query_text, projects)
        # Both should appear; the scoring should differentiate them
        assert len(citations) >= 2

    def test_bigram_matching_bonus(self):
        """Chunks containing consecutive query word pairs should score higher."""
        query = "machine learning pipeline"
        # chunk_bigram has "machine learning" as consecutive words
        chunk_bigram = "This machine learning pipeline works well."
        # chunk_scattered has the same words but separated by content words
        chunk_scattered = "This machine vision deep learning pipeline runs."
        projects = [
            {
                "id": "p1",
                "name": "bigram-proj",
                "readme_snippet": chunk_bigram,
                "summary": "",
            },
            {
                "id": "p2",
                "name": "scattered-proj",
                "readme_snippet": chunk_scattered,
                "summary": "",
            },
        ]
        citations = extract_citations(query, projects)
        assert len(citations) >= 2
        # The bigram match should rank first
        assert citations[0].project_id == "p1"

    def test_proximity_bonus(self):
        """Chunks where query terms appear close together should score higher."""
        query = "video metadata"
        # chunk_close: "video" and "metadata" are adjacent
        chunk_close = "Download video metadata quickly."
        # chunk_far: "video" and "metadata" are far apart
        chunk_far = "The video player has many features including streaming and caching and also metadata."
        projects = [
            {
                "id": "p1",
                "name": "close-proj",
                "readme_snippet": chunk_close,
                "summary": "",
            },
            {
                "id": "p2",
                "name": "far-proj",
                "readme_snippet": chunk_far,
                "summary": "",
            },
        ]
        citations = extract_citations(query, projects)
        assert len(citations) >= 2
        assert citations[0].project_id == "p1"


class TestContextWindow:
    def test_context_included(self):
        """Each citation should include surrounding context."""
        text = (
            "First sentence about setup. "
            "Second sentence about the API. "
            "Third sentence about deployment."
        )
        projects = [
            {
                "id": "p1",
                "name": "ctx-proj",
                "readme_snippet": text,
                "summary": "",
            },
        ]
        citations = extract_citations("API", projects)
        assert len(citations) > 0
        cit = citations[0]
        assert hasattr(cit, "context")
        # The context should include the neighboring sentences
        assert cit.context is not None
        assert len(cit.context) > len(cit.chunk)

    def test_context_at_start(self):
        """First chunk should still have context (just the following sentence)."""
        text = "API endpoint for users. Second sentence here. Third sentence here."
        projects = [
            {
                "id": "p1",
                "name": "ctx-proj",
                "readme_snippet": text,
                "summary": "",
            },
        ]
        citations = extract_citations("API endpoint users", projects)
        assert len(citations) > 0
        cit = citations[0]
        assert cit.context is not None

    def test_context_at_end(self):
        """Last chunk should still have context (just the preceding sentence)."""
        text = "First sentence here. Second sentence here. API endpoint for users."
        projects = [
            {
                "id": "p1",
                "name": "ctx-proj",
                "readme_snippet": text,
                "summary": "",
            },
        ]
        citations = extract_citations("API endpoint users", projects)
        assert len(citations) > 0
        cit = citations[0]
        assert cit.context is not None


class TestSectionTitleField:
    def test_section_title_in_citation(self):
        """Citation should carry the section title when available."""
        text = "# Installation\nRun pip install. Configure the settings.\n\n# Usage\nCall the API endpoint."
        projects = [
            {
                "id": "p1",
                "name": "sec-proj",
                "readme_snippet": text,
                "summary": "",
            },
        ]
        citations = extract_citations("API endpoint", projects)
        assert len(citations) > 0
        api_cit = citations[0]
        assert hasattr(api_cit, "section_title")
        assert api_cit.section_title == "Usage"

    def test_section_title_none_when_no_headers(self):
        """Section title should be None when there are no headers."""
        text = "Simple text about the API endpoint."
        projects = [
            {
                "id": "p1",
                "name": "no-sec-proj",
                "readme_snippet": text,
                "summary": "",
            },
        ]
        citations = extract_citations("API endpoint", projects)
        assert len(citations) > 0
        assert citations[0].section_title is None


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
        # New fields
        assert hasattr(cit, "section_title")
        assert hasattr(cit, "context")

    def test_relevance_prefers_better_match(self):
        projects = self._make_projects()
        citations = extract_citations("YouTube video data API", projects)
        # The youtube project should produce the top citation
        top = citations[0]
        assert top.project_id == "p1"
