# Code Explore - Design Document

**Date**: 2026-03-09
**Status**: Approved

## Problem

Developer with 500+ projects scattered across local directories and GitHub accounts.
No way to search past work, recall experiments, or reuse existing solutions.

## Solution

**Code Explore** - a personal developer knowledge base that:
- Scans and indexes all projects (local + remote)
- Analyzes each project: languages, frameworks, dependencies, patterns, quality
- Generates AI summaries via local Ollama
- Enables fulltext + semantic search across everything
- Provides CLI, Web UI, and Desktop interfaces

## Architecture

```
code_explore/
├── models.py          # Pydantic data models
├── database.py        # SQLite + FTS5 storage
├── scanner/
│   ├── local.py       # Filesystem repo discovery
│   └── git_info.py    # Git metadata extraction
├── analyzer/
│   ├── language.py    # Language detection
│   ├── metrics.py     # Quality metrics
│   ├── dependencies.py # Dependency parsing
│   └── patterns.py    # Pattern/concept detection
├── indexer/
│   └── embeddings.py  # Ollama embeddings + LanceDB
├── search/
│   ├── fulltext.py    # SQLite FTS5
│   ├── semantic.py    # LanceDB vectors
│   └── hybrid.py      # Combined search + RRF ranking
├── summarizer/
│   └── ollama.py      # LLM project summaries
├── cli/
│   └── main.py        # Typer CLI
└── api/
    └── main.py        # FastAPI REST API
```

## Tech Stack

| Component | Technology | Rationale |
|-----------|-----------|-----------|
| Language | Python 3.11+ | Best ML/embedding ecosystem |
| CLI | Typer + Rich | Developer-friendly, great output |
| API | FastAPI | Async, fast, auto-docs |
| DB | SQLite + FTS5 | Zero config, embedded, fast fulltext |
| Vectors | LanceDB | Local, embedded, no server needed |
| Embeddings | Ollama (nomic-embed-text) | Local, private, free |
| Summaries | Ollama (llama3.2) | Local LLM, no API costs |
| Web UI | Next.js (Phase 2) | React-based, SSR |
| Desktop | Tauri (Phase 3) | Lightweight, Rust backend |

## Data Model

- **Project**: Core entity with all metadata, analysis, and AI-generated content
- **LanguageInfo**: Per-language file/line counts
- **DependencyInfo**: Parsed from package manifests
- **PatternInfo**: Detected concepts with confidence scores
- **QualityMetrics**: Code quality indicators
- **GitInfo**: Repository metadata

## Search Strategy

1. **Fulltext (FTS5)**: Exact keyword matching on name, summary, tags
2. **Semantic (LanceDB)**: Vector similarity on project embeddings
3. **Hybrid**: Both searches combined with Reciprocal Rank Fusion

## CLI Commands

```bash
code-explore scan ~/Repositories          # Scan local repos
code-explore scan --github <user>          # Scan GitHub (Phase 2)
code-explore search "youtube api videos"   # Hybrid search
code-explore search --exact "OAuth2"       # Fulltext only
code-explore show <project>                # Project detail
code-explore index                         # Generate embeddings
code-explore stats                         # Overview dashboard
code-explore serve                         # Start API server
```

## Phases

### Phase 1 (MVP - current)
- CLI with scan, search, show, index, stats, serve
- Local directory scanning
- Full code analysis pipeline
- SQLite FTS5 + LanceDB semantic search
- Ollama embeddings + summaries
- FastAPI backend

### Phase 2
- GitHub/GitLab remote scanning
- Next.js Web UI dashboard
- Incremental re-indexing
- Quality/maturity scoring

### Phase 3
- Tauri desktop app
- Business features (pricing model)
- Team/sharing features

## Business Model (future)

- Open source core (MIT)
- Target: Individual developers
- Potential: freemium SaaS for teams
- Revenue: Premium features (team search, hosted instance, CI integration)
