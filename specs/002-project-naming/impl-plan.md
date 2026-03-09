# Implementation Plan: Project Naming Decision

**Feature**: 002-project-naming
**Branch**: `002-project-naming`
**Spec**: [spec.md](./spec.md)
**Research**: [research.md](./research.md)
**Date**: 2026-03-09

---

## Technical Context

| Aspect | Value |
|--------|-------|
| Package name | `code-explore` |
| Python module | `code_explore` |
| CLI commands | `cex` (primary), `code-explore` (full) |
| GitHub repo | `github.com/aipioneers/code-explore` |
| PyPI | Available (verified) |
| Homebrew | Available (verified) |
| `cex` conflicts | None (verified) |

---

## Phase 1: GitHub Repository Setup

### Task 1.1: Create repository under aipioneers

- Create `code-explore` repo under `github.com/aipioneers`
- Set description: "Developer knowledge base CLI — scan, index, and search 500+ programming projects"
- Initialize with existing codebase from current local repo
- Set default branch to `main`

### Task 1.2: Update repository metadata

- Add topics: `cli`, `developer-tools`, `code-search`, `knowledge-base`, `python`
- Add license (if not already present)
- Configure repository settings (issues enabled, wiki disabled, discussions optional)

---

## Phase 2: Package Configuration

### Task 2.1: Configure entry points in `pyproject.toml`

Add CLI entry points so both `cex` and `code-explore` commands are available after `pip install`:

```toml
[project.scripts]
cex = "code_explore.cli:main"
code-explore = "code_explore.cli:main"
```

### Task 2.2: Update package metadata

Ensure `pyproject.toml` has correct metadata for PyPI publication:

- `name = "code-explore"`
- `description` — concise one-liner
- `authors` — maintainer info
- `urls` — point to `github.com/aipioneers/code-explore`
- `classifiers` — appropriate trove classifiers
- `keywords` — searchable terms
- `requires-python` — minimum Python version

### Task 2.3: Verify package builds locally

- Run `python -m build` to create sdist + wheel
- Verify `cex` and `code-explore` commands work after `pip install -e .`

---

## Phase 3: PyPI Registration

### Task 3.1: Register on TestPyPI first

- Upload to `test.pypi.org` to verify package name and metadata
- Test installation: `pip install -i https://test.pypi.org/simple/ code-explore`
- Verify both `cex` and `code-explore` commands work

### Task 3.2: Publish to PyPI

- Upload to `pypi.org` to secure the package name
- Verify: `pip install code-explore` works
- Verify: `cex --help` and `code-explore --help` produce correct output

---

## Phase 4: Homebrew Formula (Optional, post-launch)

### Task 4.1: Create Homebrew formula

- Create formula for `homebrew-tap` or submit to `homebrew-core`
- Formula should install `cex` and `code-explore` commands
- Test: `brew install code-explore`

---

## Phase 5: Domain & Branding (Optional, non-blocking)

### Task 5.1: Domain registration

- Check availability of `code-explore.dev`, `codeexplore.dev`, `cex.dev`
- Register preferred domain if available
- Not blocking for launch

### Task 5.2: Basic trademark search

- Search USPTO/EUIPO for "code-explore" and "cex" in software categories
- Document findings — informational only, no registration planned initially

---

## Success Verification Checklist

- [ ] `pip install code-explore` installs successfully from PyPI
- [ ] `cex --help` shows correct CLI help
- [ ] `code-explore --help` shows correct CLI help
- [ ] `github.com/aipioneers/code-explore` is accessible
- [ ] Google search for "code-explore aipioneers" returns the GitHub repo
- [ ] Package metadata on PyPI shows correct description, URLs, and classifiers

---

## Dependencies

| Dependency | Status | Blocking |
|------------|--------|----------|
| PyPI account | Needed | Yes (Phase 3) |
| aipioneers GitHub org | Exists | No |
| `pyproject.toml` entry points | To configure | Yes (Phase 2) |
| Domain registration | Optional | No |

---

## Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Someone registers `code-explore` on PyPI first | High | Execute Phase 3 promptly |
| `cex` command conflicts on user systems | Low | Both commands work; document alias |
| SEO weakness for generic "code explore" | Low | Use org-qualified searches; build stars |
| Name change needed post-publication | High | This plan finalizes the name — no change after PyPI publish |
