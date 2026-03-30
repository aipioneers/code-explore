# Contributing to code-explore

Contributions are welcome. Whether it's a bug report, feature suggestion, or pull request — thank you for helping improve code-explore.

## Reporting Bugs

Open an issue using the **Bug Report** template. Include steps to reproduce, expected behavior, and your environment.

## Suggesting Features

Open an issue using the **Feature Request** template. Describe the use case and why it matters.

## Development Setup

```bash
git clone https://github.com/aipioneers/code-explore.git
cd code-explore
pip install -e ".[dev]"
pytest tests/ -v
```

Requires Python 3.11+. AI features (summaries, semantic search) require [Ollama](https://ollama.ai) running locally — everything works without it via fulltext fallback.

## Pull Requests

1. Fork the repo and create a branch from `main`
2. Write tests for new functionality
3. Run `pytest tests/ -v` and make sure everything passes
4. Open a PR with a clear description of what and why

## Code Style

- **Linter**: ruff (line length 100, target Python 3.11)
- **Types**: Pydantic v2 models for all data structures
- **Tests**: pytest with fixtures in `conftest.py`

## Code of Conduct

This project follows the [Contributor Covenant](CODE_OF_CONDUCT.md). Be kind.
