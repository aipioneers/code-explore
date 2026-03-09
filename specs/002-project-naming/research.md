# Research: Project Naming Decision

**Feature**: 002-project-naming
**Date**: 2026-03-09

## R1: PyPI Package Name Availability

**Decision**: `code-explore`
**Rationale**: Verified available via PyPI JSON API (404). PEP 508 normalization means `code-explore`, `code_explore`, and `code.explore` all resolve to the same package — none are registered.
**Alternatives considered**: 95+ names researched (see spec.md Eliminated Names). All common `code*` prefix names are taken on PyPI due to widespread name squatting.

## R2: GitHub Repository Location

**Decision**: `github.com/aipioneers/code-explore`
**Rationale**: User owns the `aipioneers` GitHub organization. Repository name `code-explore` is available under this org (verified via API, 404). Publishing under an org provides better branding and allows team collaboration.
**Alternatives considered**: Personal account (`tobiasoberrauch/code-explore`) — viable but less professional for a published tool.

## R3: CLI Command Alias

**Decision**: `cex` (3 chars) as primary short alias, `code-explore` (12 chars) as full command
**Rationale**: `cex` is not a standard Unix command (verified via `which`). The `cex` PyPI package exists but is an inactive Cex.io crypto API library (v1.0, no console scripts) — no CLI command conflict. Entry points in `pyproject.toml` allow both commands from a single package install.
**Alternatives considered**: `ce` (2 chars, too short/ambiguous), `codex` (taken by OpenAI), `cx` (common abbreviation in other contexts).

## R4: Homebrew Distribution

**Decision**: `code-explore` as formula name
**Rationale**: No existing Homebrew formula named `code-explore`. Formula naming convention (lowercase with hyphens) matches the package name.
**Alternatives considered**: N/A — standard convention.

## R5: SEO & Discoverability

**Decision**: Accept weak SEO for "code explore" (generic terms), mitigate via org branding
**Rationale**: "code explore" returns generic results, but `aipioneers/code-explore` is unique. The `cex` alias is highly unique in search. Building GitHub stars and PyPI downloads will improve organic ranking over time.
**Alternatives considered**: Unique made-up names (better SEO) were considered but user preferred the descriptive working title.

## R6: `cex` Entry Point Conflict Analysis

**Decision**: Safe to use `cex` as CLI entry point
**Rationale**:
- Not a standard Unix/macOS command (`which cex` → not found)
- PyPI `cex` package (Cex.io API) does not register console scripts — no CLI conflict
- No Homebrew formula named `cex`
- Unique enough for searchability
**Risk**: If a user has `pip install cex` AND our package, no command collision (different packages, `cex` library has no entry points).
