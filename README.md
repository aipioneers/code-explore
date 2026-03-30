# code-explore

Your codebase, searchable in seconds.

[![PyPI](https://img.shields.io/pypi/v/code-explore)](https://pypi.org/project/code-explore/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Python 3.11+](https://img.shields.io/badge/python-3.11+-blue.svg)](https://python.org)

Scan hundreds of git repositories, analyze languages and patterns, generate AI summaries with local Ollama, and find anything with hybrid search. Fulltext for exact matches, semantic for meaning — fused with Reciprocal Rank Fusion into one ranked result.

## Quick Start

```bash
pip install code-explore
```

```bash
# Scan your projects directory
cex scan ~/Projects

# Search across everything
cex search "authentication middleware"

# View a specific project
cex show my-project

# Stats overview
cex stats
```

## How It Works

```
Discover → Analyze → Summarize → Index → Search
```

1. **Discover** — finds git repositories recursively, reads READMEs, extracts git metadata
2. **Analyze** — detects 70+ languages, maps dependencies, identifies architectural patterns, calculates code metrics
3. **Summarize** — generates descriptions and tags using local Ollama models
4. **Index** — builds SQLite FTS5 tables and LanceDB vector store for hybrid search
5. **Search** — combines fulltext, semantic, and pattern-based results with Reciprocal Rank Fusion

## Features

- **One-command scan** — discovers repos, detects 70+ languages, maps dependencies and patterns
- **AI summaries** — local Ollama generates descriptions and tags for every project
- **Hybrid search** — SQLite FTS5 + LanceDB vectors + pattern matching, fused with RRF
- **Confidence scoring** — every result tells you how reliable it is
- **Incremental indexing** — only re-processes projects that changed since last scan
- **REST API + dashboard** — optional FastAPI server with web UI at `/dashboard`
- **Plugin system** — extend scanning and analysis with custom plugins
- **Works offline** — AI features use local models, degrades gracefully without Ollama

## Search Modes

```bash
# Fulltext — exact keyword matches via SQLite FTS5
cex search "fastapi middleware" --mode fulltext

# Semantic — meaning-based via LanceDB vector search
cex search "tools for processing images" --mode semantic

# Hybrid (default) — both engines, fused with RRF
cex search "authentication"
```

## Configuration

Three-layer priority: defaults → config file → environment variables.

```bash
# Config file location (respects XDG_CONFIG_HOME)
~/.config/code-explore/config.toml

# Or via environment variables
export CEX_OLLAMA_MODEL="llama3.2"
export CEX_DB_PATH="~/.local/share/code-explore/projects.db"
```

## Development

```bash
git clone https://github.com/aipioneers/code-explore.git
cd code-explore
pip install -e ".[dev]"
pytest tests/ -v
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for development setup and guidelines.

## License

[MIT](LICENSE)

---

Part of the [AI Pioneers](https://pioneers.ai) ecosystem · [code-adapt](https://github.com/aipioneers/code-adapt) · [spec-intelligence](https://github.com/aipioneers/spec-intelligence)
