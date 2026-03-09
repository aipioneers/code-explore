# Feature Specification: Project Naming Decision

**Feature**: Comprehensive naming analysis and final name selection for the developer knowledge base tool
**Status**: Draft
**Created**: 2026-03-09

## Problem Statement

The project currently uses the working name `code-explore`. Before publishing to PyPI, Homebrew, and other registries, a thorough naming analysis is needed to ensure the chosen name is:
- Available across all distribution channels
- Not conflicting with existing projects or trademarks
- Short and memorable as a CLI command
- Discoverable via search engines
- Compliant with naming guidelines of all target registries

## User Scenarios & Acceptance Criteria

### Scenario 1: Developer installs via pip
**Given** a developer who heard about the tool
**When** they type `pip install <name>`
**Then** they get the correct package without confusion with other projects

### Scenario 2: Developer uses the CLI daily
**Given** a developer who uses the tool regularly
**When** they type the command name from memory
**Then** the command is short enough to type quickly (< 10 characters) and easy to remember

### Scenario 3: Developer searches for the tool
**Given** a developer searching Google/GitHub for the tool
**When** they search the project name
**Then** the tool appears in top results (not drowned out by generic terms)

### Scenario 4: Developer recommends the tool
**Given** a developer telling a colleague about the tool
**When** they say the name aloud
**Then** the colleague can spell it correctly and find it

---

## Naming Best Practices (Research Summary)

Sources: [The Poetics of CLI Command Names](https://smallstep.com/blog/the-poetics-of-cli-command-names/), [CLI Guidelines](https://clig.dev/), [PEP 423](https://peps.python.org/pep-0423/), [Choosing project names](https://opensource.com/article/18/2/choosing-project-names-four-key-considerations), community discussions.

**Successful short CLI names follow patterns:**
- `rg` (ripgrep), `fzf`, `bat`, `jq`, `exa` — 2-5 chars, daily typing
- `tmux`, `htop`, `curl` — real words or abbreviations, memorable
- Single words over compounds (no hyphens in command names = less typing friction)

**Key principles:**
1. Don't claim generic words ("explore", "scan", "code") — drowns in search
2. Made-up or uncommon real words work best for SEO uniqueness
3. The "phone test" — can someone find the tool after hearing the name once?
4. Short for daily use (under 8 chars ideal, 10 max)
5. Command name can differ from package name via entry points

---

## Availability Analysis

### Current Name: `code-explore`

| Channel | Status | Issue |
|---------|--------|-------|
| PyPI | Available | No package `code-explore` exists |
| GitHub org | **OK** | Published under `github.com/aipioneers/code-explore` |
| npm | **CONFLICT** | `npm explore` is a built-in npm command |
| Homebrew | Available | No formula exists |
| CLI length | 3 chars (`cex`) | Short alias via entry point, full name also available |
| SEO | **WEAK** | "code explore" returns generic GitHub/IDE results |

**Verdict**: Accepted — published under AI Pioneers org, short CLI alias `cex` solves typing friction.

---

## Eliminated Names (40+ researched)

| Name | Reason for elimination |
|------|----------------------|
| `codex` | OpenAI Codex (massive brand) |
| `glean` | Mozilla Glean SDK + Glean.com (VC-backed enterprise search) + Facebook Glean |
| `trove` | OpenStack Trove + PyPI "trove classifiers" core concept |
| `codehive` | `codehive.blog` exists (active tech blog) |
| `reposcout` | Rust TUI tool, featured on Hacker News |
| `code-atlas` | Multiple CodeAtlas projects (GitHub, VS Code plugin, codeatlas.dev) |
| `delve` | PyPI package exists + Microsoft Delve (Office 365) |
| `codesage` | Amazon Science CodeSage (ICLR 2024 paper) |
| `codescope` | PyPI package exists |
| `codeowl` | PyPI package exists |
| `kodex` | PyPI package exists |
| `sourcery` | Major AI code review product (sourcery.ai) |
| `devvault` | GitHub org DevVault (341 repos) |
| `codefind` | PyPI package exists |
| `repomap` | Associated with Aider's repomap concept |
| `coda` | PyPI package + Panic's IDE |
| `sieve` | PyPI package exists |
| `folio` | PyPI package exists |
| `stash` | PyPI package exists |
| `codescan` | `codescanai` on PyPI, generic |
| `codebase-indexer` | Already exists on PyPI |
| `devtrove` | GitHub user exists |
| `codetrove` | GitHub org exists |

---

## Final Candidates

### Tier 1: Top Recommendations

#### 1. `recce` — Reconnaissance for your codebase

| Aspect | Details |
|--------|---------|
| Meaning | British military term for reconnaissance/scouting |
| CLI | `recce search "YouTube API"` — **5 chars**, fastest to type |
| PyPI | **Available** — no package found |
| GitHub | Available as repo name |
| SEO | **Excellent** — unique term, not a common tech product |
| Phonetic | "REK-ee" — easy to say, spell, and remember |
| Metaphor | Perfect: "reconnaissance" = surveying/exploring territory = what the tool does |
| Domain | `recce.dev` needs manual check |
| Risk | Some may not know the word (niche British English) |

#### 2. `repowise` — Wisdom about your repositories

| Aspect | Details |
|--------|---------|
| Meaning | "Wise about repositories" — self-descriptive |
| CLI | `repowise search "YouTube API"` — **8 chars** |
| PyPI | **Available** — no package found |
| GitHub | **Available** — no org or prominent repo |
| SEO | **Good** — unique compound word, no competing products |
| Phonetic | "REP-oh-wize" — easy to say and spell |
| Metaphor | Clear: wisdom/knowledge about your repo collection |
| Domain | `repowise.dev` needs manual check |
| Risk | Slightly long (8 chars) but still comfortable |

#### 3. `verso` — Turn the pages of your code

| Aspect | Details |
|--------|---------|
| Meaning | Italian/Latin for "verse, page, turn" (also "the back side of a page") |
| CLI | `verso search "YouTube API"` — **5 chars** |
| PyPI | **Available** — no package found |
| GitHub | Needs check (Rust `verso` browser project exists) |
| SEO | **Good** — uncommon English word |
| Phonetic | "VER-so" — elegant, easy, international |
| Metaphor | Poetic: turning/browsing through your code collection |
| Domain | `verso.dev` needs manual check |
| Risk | Meaning not immediately obvious without context |

#### 4. `repozen` — Zen mastery over your repositories

| Aspect | Details |
|--------|---------|
| Meaning | "Repository" + "Zen" = calm mastery |
| CLI | `repozen search "YouTube API"` — **7 chars** |
| PyPI | **Available** — no package found |
| GitHub | **Available** — nothing found |
| SEO | **Excellent** — completely unique made-up word |
| Phonetic | "REP-oh-zen" — catchy, easy to say |
| Metaphor | Good: achieving zen/calm/overview over your code chaos |
| Domain | `repozen.dev` needs manual check |
| Risk | Made-up word, less immediately descriptive |

### Tier 2: Solid Alternatives

| Name | Chars | Available | Metaphor | Notes |
|------|-------|-----------|----------|-------|
| `projsight` | 9 | PyPI + GitHub | "Project insight" | Professional, descriptive, slightly long |
| `devmap` | 6 | PyPI + GitHub | "Developer map" | Short but could be confused with "dev roadmap" |
| `almanac` | 7 | PyPI (likely) | "Reference book" | Great metaphor, classic feel |

---

## Naming Guidelines Compliance

### PyPI (PEP 423 / PEP 508)
- Names must match: `[A-Za-z0-9]([A-Za-z0-9._-]*[A-Za-z0-9])?`
- Hyphens, underscores, dots are normalized to the same package
- Must not be "confusingly similar" to existing packages
- All Tier 1 candidates comply

### Homebrew
- Formula names: lowercase with hyphens
- Must not conflict with existing formulae
- All Tier 1 candidates comply

### CLI Command
- Under 10 characters ideal for daily use
- No hyphens (typing friction)
- Single word preferred
- Must not conflict with common Unix commands (`ls`, `grep`, `find`, `top`, etc.)
- All Tier 1 candidates pass (none conflict with Unix commands)

---

## Functional Requirements

### FR-1: Name must be available on PyPI
Both hyphenated and underscored variants must be free.

### FR-2: Name must be available as GitHub repository
Usable as `github.com/{owner}/{name}` without confusion.

### FR-3: CLI command must be under 10 characters
The daily-use command should be short. Package name can differ from CLI command.

### FR-4: Name must be unique in search results
Google search for the exact name should not return a dominant existing project.

### FR-5: Name must be pronounceable and spellable
Pass the "phone test" — can be communicated verbally.

### FR-6: Name should work as a domain
At least one common TLD (.dev, .io, .app) should be available.

---

## Success Criteria

1. Selected name is registerable on PyPI without conflicts
2. GitHub repository can be created without confusion
3. CLI command is 10 characters or fewer
4. Google search returns no dominant competing project
5. At least one relevant domain TLD is available
6. Passes the "phone test"

---

## Recommendation

**Decision: `code-explore`** (package name) + **`cex`** (short CLI alias)

- **PyPI package**: `code-explore` (available, verified 2026-03-09)
- **GitHub repository**: `github.com/aipioneers/code-explore`
- **GitHub organization**: AI Pioneers (`github.com/aipioneers`)
- **CLI commands**: Both `cex` (3 chars) and `code-explore` (12 chars) via entry points
- **Homebrew formula**: `code-explore`

```bash
pip install code-explore

# Short alias for daily use (3 chars):
cex search "YouTube API alle Videos"
cex show data-youtube
cex stats

# Full name also works:
code-explore search "YouTube API alle Videos"
```

**Entry point configuration in `pyproject.toml`:**
```toml
[project.scripts]
cex = "code_explore.cli:main"
code-explore = "code_explore.cli:main"
```

---

## Assumptions

1. PyPI is the primary distribution channel
2. GitHub repository under AI Pioneers organization (`github.com/aipioneers`)
3. Domain registration desired but not blocking for launch
4. CLI command name can differ from PyPI package name
5. No trademark registration planned initially
6. Target audience: developers comfortable with English CLI names

## Dependencies

- PyPI account setup
- GitHub repository creation (or rename)
- Optional: Domain registration

## Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| Name squatting on PyPI | High | Register immediately after decision |
| Trademark conflict | Medium | Search USPTO/EUIPO before finalizing |
| SEO competition | Low | Build community/stars to improve ranking |
| Name change after publication | High | Decide before first PyPI release |
