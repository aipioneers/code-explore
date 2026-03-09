# code-explore

Developer knowledge base CLI — scan, index, and search your programming projects.

## Install

```bash
pip install code-explore
```

## Usage

```bash
# Short alias (3 chars):
cex scan ~/Projects
cex index
cex search "YouTube API alle Videos"
cex show data-youtube
cex stats

# Full command also works:
code-explore scan ~/Projects
```

## Features

- Scan local project directories and extract metadata (languages, dependencies, patterns)
- Generate AI summaries using local Ollama models
- Create vector embeddings for semantic search (multilingual)
- Hybrid search combining fulltext + semantic ranking
- Incremental indexing with change detection
