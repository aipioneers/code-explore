# Quickstart: Publishing code-explore

## 1. Configure entry points

Add to `pyproject.toml`:

```toml
[project.scripts]
cex = "code_explore.cli:main"
code-explore = "code_explore.cli:main"
```

## 2. Create GitHub repo

```bash
gh repo create aipioneers/code-explore \
  --public \
  --description "Developer knowledge base CLI — scan, index, and search your programming projects" \
  --source . \
  --push
```

## 3. Build and test locally

```bash
pip install -e .
cex --help
code-explore --help
```

## 4. Publish to PyPI

```bash
# Install build tools
pip install build twine

# Build
python -m build

# Upload to TestPyPI first
twine upload --repository testpypi dist/*

# Then to real PyPI
twine upload dist/*
```

## 5. Verify

```bash
pip install code-explore
cex search "YouTube API alle Videos"
```
