# Tasks: Project Naming Decision

**Feature**: 002-project-naming
**Branch**: `002-project-naming`
**Generated**: 2026-03-09
**Total Tasks**: 16
**Spec**: [spec.md](./spec.md)
**Plan**: [impl-plan.md](./impl-plan.md)

---

## User Story Mapping

| Story | Spec Scenario | Priority | Tasks |
|-------|---------------|----------|-------|
| US1 | Developer installs via pip | P1 | T003–T006, T010–T013 |
| US2 | Developer uses the CLI daily | P1 | T004 (entry points) |
| US3 | Developer searches for the tool | P2 | T007–T009 |
| US4 | Developer recommends the tool | P3 | T014–T016 |

---

## Phase 1: Setup

- [ ] T001 Create `pyproject.toml` with package metadata in project root (`name = "code-explore"`, description, author, license, urls, classifiers, requires-python, keywords)
- [ ] T002 Add `[build-system]` section to `pyproject.toml` with `hatchling` or `setuptools` backend

## Phase 2: Foundational (blocking for all stories)

- [ ] T003 Add `[project.dependencies]` to `pyproject.toml` listing all runtime dependencies from `code_explore/` imports (lancedb, ollama, etc.)
- [ ] T004 Add `[project.scripts]` entry points to `pyproject.toml`: `cex = "code_explore.cli.main:main"` and `code-explore = "code_explore.cli.main:main"`
- [ ] T005 Verify local editable install works: `pip install -e .` and both `cex --help` and `code-explore --help` produce correct output
- [ ] T006 Verify `python -m build` produces valid sdist and wheel in `dist/`

## Phase 3: US1 — Developer installs via pip (P1)

**Goal**: `pip install code-explore` works from PyPI and installs the tool without conflicts.

**Independent test criteria**: Run `pip install code-explore` in a fresh venv, verify `cex --help` outputs CLI usage.

- [ ] T010 [US1] Upload package to TestPyPI via `twine upload --repository testpypi dist/*`
- [ ] T011 [US1] Test install from TestPyPI: `pip install -i https://test.pypi.org/simple/ code-explore` in a clean virtualenv
- [ ] T012 [US1] Upload package to production PyPI via `twine upload dist/*`
- [ ] T013 [US1] Verify production install: `pip install code-explore` in a clean virtualenv, confirm `cex --help` and `code-explore --help` work

## Phase 4: US3 — Developer searches for the tool (P2)

**Goal**: `github.com/aipioneers/code-explore` is accessible and discoverable.

**Independent test criteria**: Navigate to repo URL, verify description and topics are visible.

- [ ] T007 [P] [US3] Create GitHub repo: `gh repo create aipioneers/code-explore --public --description "Developer knowledge base CLI — scan, index, and search your programming projects" --source . --push`
- [ ] T008 [P] [US3] Add GitHub topics to repo: `cli`, `developer-tools`, `code-search`, `knowledge-base`, `python` via `gh repo edit aipioneers/code-explore --add-topic`
- [ ] T009 [US3] Verify `github.com/aipioneers/code-explore` is accessible and shows correct description

## Phase 5: US4 — Developer recommends the tool (P3, optional)

**Goal**: Domain and branding support discoverability when recommended verbally.

**Independent test criteria**: Preferred domain TLD is registered or documented as unavailable.

- [ ] T014 [P] [US4] Check domain availability for `code-explore.dev`, `codeexplore.dev`, `cex.dev`
- [ ] T015 [P] [US4] Search USPTO/EUIPO for "code-explore" and "cex" in software categories, document findings in `specs/002-project-naming/trademark-search.md`
- [ ] T016 [US4] Register preferred domain if available (non-blocking for launch)

---

## Dependencies

```
T001 → T002 → T003, T004 (parallel)
T003, T004 → T005 → T006
T006 → T010 → T011 → T012 → T013
T007, T008 (parallel, independent of T001–T006)
T014, T015 (parallel, independent of all others)
```

## Parallel Execution Opportunities

| Parallel Group | Tasks | Reason |
|---------------|-------|--------|
| Package config | T003, T004 | Different sections of same file, no dependency |
| GitHub setup | T007, T008 | Independent repo operations |
| Branding research | T014, T015 | Independent research tasks |
| GitHub + Package | T007–T009 alongside T001–T006 | Completely independent tracks |

## Implementation Strategy

**MVP scope**: Phase 1 + Phase 2 + Phase 4 (US3)
- Create `pyproject.toml`, configure entry points, create GitHub repo
- This gives a working installable package and public repository
- PyPI publishing (Phase 3) follows once local build is verified

**Incremental delivery**:
1. First: `pyproject.toml` + local install verification (T001–T006)
2. Second: GitHub repo creation (T007–T009) — can run in parallel with step 1
3. Third: PyPI registration (T010–T013) — requires step 1 complete
4. Optional: Domain/trademark (T014–T016) — non-blocking, any time
