# Code Explore

**Deine persoenliche Developer Knowledge Base** -- Indexiere, analysiere und durchsuche alle deine Projekte mit KI-gestuetzter semantischer Suche.

Code Explore scannt deine lokalen Repositories, analysiert jedes Projekt automatisch (Sprachen, Dependencies, Patterns, Qualitaet) und macht alles ueber Volltext- und semantische Suche auffindbar. Laeuft komplett lokal auf deinem Rechner -- keine Cloud, keine API-Kosten.

---

## Inhaltsverzeichnis

- [Features](#features)
- [Voraussetzungen](#voraussetzungen)
- [Installation](#installation)
- [Quickstart](#quickstart)
- [CLI-Referenz](#cli-referenz)
- [REST API](#rest-api)
- [Architektur](#architektur)
- [Datenmodell](#datenmodell)
- [Konfiguration](#konfiguration)
- [Entwicklung](#entwicklung)
- [Roadmap](#roadmap)
- [Lizenz](#lizenz)

---

## Features

### Scanning & Analyse

- Automatisches Erkennen von Git-Repositories in beliebigen Verzeichnissen
- Spracherkennung mit Dateien-/Zeilen-Verteilung (100+ Sprachen)
- Dependency-Parsing: `package.json`, `requirements.txt`, `pyproject.toml`, `Cargo.toml`, `go.mod`, `pom.xml`, `Gemfile`, `composer.json`
- Pattern-Erkennung: REST API, GraphQL, OAuth, JWT, Docker, Kubernetes, ML/AI, WebSocket, und 30+ weitere
- Framework-Erkennung: React, Next.js, Django, FastAPI, Express, Vue, Angular, Flask, Spring, etc.
- Quality Metrics: Tests, CI, Docs, README, License, .gitignore
- Git-Metadaten: Commits, Contributors, Branches, Remote-URLs

### KI-gestuetzte Features (via Ollama)

- Automatische Projekt-Zusammenfassungen (2-3 Saetze pro Projekt)
- Tag-Generierung (5-10 relevante Tags)
- Concept-Extraktion (3-5 architektonische Konzepte)
- Embedding-Generierung fuer semantische Suche

### Suche

- **Volltext-Suche**: SQLite FTS5 mit Porter-Stemming ueber Name, Summary und Tags
- **Semantische Suche**: Vektor-Aehnlichkeit via LanceDB und Ollama-Embeddings
- **Hybrid-Suche**: Kombiniert beide Ansaetze mit Reciprocal Rank Fusion (RRF)

### Interfaces

- **CLI**: Rich-formatierte Terminal-Ausgabe mit Progress-Bars, Tabellen und Baum-Ansichten
- **REST API**: FastAPI mit automatischer OpenAPI-Dokumentation
- **Web UI**: Geplant (Phase 2)
- **Desktop App**: Geplant (Phase 3, Tauri)

---

## Voraussetzungen

| Komponente | Version | Zweck |
|-----------|---------|-------|
| Python | >= 3.11 | Runtime |
| pip | aktuell | Paketmanager |
| Git | aktuell | Repository-Erkennung |
| [Ollama](https://ollama.com) | aktuell | Lokale KI (Summaries + Embeddings) |

### Ollama einrichten

```bash
# Ollama installieren (macOS)
brew install ollama

# Ollama starten
ollama serve

# Benoetigte Modelle herunterladen
ollama pull nomic-embed-text    # Embeddings (768 Dimensionen)
ollama pull llama3.2:3b         # Summaries
```

> Ollama ist optional. Ohne Ollama funktionieren Scan, Analyse, Volltext-Suche und die API -- nur semantische Suche und KI-Summaries sind dann nicht verfuegbar.

---

## Installation

```bash
# Repository klonen
git clone https://github.com/tobiasoberrauch/code-explore.git
cd code-explore

# Als editierbares Paket installieren
pip install -e .

# Mit Dev-Dependencies (fuer Tests und Linting)
pip install -e ".[dev]"

# Pruefen ob es funktioniert
code-explore --help
```

---

## Quickstart

```bash
# 1. Repositories scannen und analysieren
code-explore scan ~/Repositories --depth 3

# 2. KI-Summaries und Embeddings generieren (braucht Ollama)
code-explore index

# 3. Suchen!
code-explore search "YouTube API Videos abrufen"
code-explore search "OAuth login" --mode semantic
code-explore search "REST API" --mode fulltext

# 4. Projekt-Details anzeigen
code-explore show mein-projekt

# 5. Statistiken
code-explore stats
```

---

## CLI-Referenz

### `code-explore scan <path>`

Scannt ein Verzeichnis rekursiv nach Git-Repositories und analysiert jedes gefundene Projekt.

```bash
code-explore scan ~/Repositories           # Standard (Tiefe 4)
code-explore scan ~/Projects --depth 2     # Nur 2 Ebenen tief
code-explore scan ~/Code --force           # Bereits gescannte Projekte erneut analysieren
```

**Optionen:**

| Option | Kurz | Default | Beschreibung |
|--------|------|---------|-------------|
| `--depth` | `-d` | `4` | Maximale Verzeichnistiefe |
| `--force` | `-f` | `false` | Bereits gescannte Projekte erneut analysieren |

**Was wird analysiert:**

- Sprachen und Zeilen-Verteilung
- Dependencies aus Paketmanager-Dateien
- Architektur-Patterns und Frameworks
- Quality Metrics (Tests, CI, Docs, etc.)
- Git-Metadaten (Commits, Contributors, Remotes)

**Uebersprungene Verzeichnisse:** `node_modules`, `venv`, `.venv`, `__pycache__`, `.cache`, `vendor`, `dist`, `build`, `target`

---

### `code-explore search <query>`

Durchsucht alle indexierten Projekte.

```bash
# Hybrid-Suche (Standard) -- kombiniert Volltext und Semantik
code-explore search "machine learning model training"

# Nur semantische Suche -- findet konzeptuell aehnliche Projekte
code-explore search "Video-Generierung mit KI" --mode semantic

# Nur Volltext -- exakte Keyword-Matches
code-explore search "FastAPI" --mode fulltext

# Ergebnisse limitieren
code-explore search "React Dashboard" --limit 5
```

**Optionen:**

| Option | Kurz | Default | Beschreibung |
|--------|------|---------|-------------|
| `--mode` | `-m` | `hybrid` | Suchmodus: `fulltext`, `semantic`, `hybrid` |
| `--limit` | `-l` | `20` | Maximale Anzahl Ergebnisse |

**Suchmodi im Detail:**

| Modus | Engine | Staerke | Braucht Ollama |
|-------|--------|---------|----------------|
| `fulltext` | SQLite FTS5 | Exakte Keywords, schnell | Nein |
| `semantic` | LanceDB + Ollama | Konzeptuelle Aehnlichkeit, natuerliche Sprache | Ja |
| `hybrid` | Beide + RRF-Ranking | Beste Ergebnisse, kombiniert beide Staerken | Ja (Fallback auf Fulltext) |

---

### `code-explore show <name-oder-id>`

Zeigt detaillierte Informationen zu einem Projekt.

```bash
code-explore show mein-projekt     # Nach Name suchen
code-explore show a1b2c3d4e5f6     # Nach ID suchen
code-explore show crawler          # Auch Teilnamen funktionieren
```

**Angezeigte Informationen:**

- Allgemein: ID, Pfad, Source, Status, Remote-URL
- Sprachen: Auflistung mit Dateien, Zeilen und Prozentanteil
- Frameworks und Dependencies
- Erkannte Patterns mit Confidence-Score
- Quality Metrics mit Checks (Tests, CI, Docs, etc.)
- Git-Info: Branch, Commits, letzter Commit, Contributors
- KI-Summary, Tags und Concepts

---

### `code-explore index`

Generiert KI-Summaries und Embeddings fuer alle gescannten Projekte.

```bash
code-explore index
```

- Generiert Summaries nur fuer Projekte, die noch keine haben
- Generiert Embeddings fuer alle Projekte (immer aktualisiert)
- Braucht laufendes Ollama mit `llama3.2:3b` und `nomic-embed-text`
- Bei 100+ Projekten kann das einige Minuten dauern

---

### `code-explore stats`

Zeigt eine Uebersicht aller indexierten Projekte.

```bash
code-explore stats
```

**Ausgabe:**

- Gesamtzahl Projekte, Dateien und Zeilen
- Sprachen-Verteilung
- Top Frameworks
- Top Patterns
- Projekt-Status-Verteilung

---

### `code-explore serve`

Startet den FastAPI REST API Server.

```bash
code-explore serve                          # Standard: 0.0.0.0:8000
code-explore serve --port 3000              # Anderer Port
code-explore serve --host 127.0.0.1         # Nur lokal erreichbar
```

**Optionen:**

| Option | Kurz | Default | Beschreibung |
|--------|------|---------|-------------|
| `--host` | `-h` | `0.0.0.0` | Bind-Adresse |
| `--port` | `-p` | `8000` | Port |

API-Docs: `http://localhost:8000/docs` (Swagger UI)

---

## REST API

Die API wird mit `code-explore serve` gestartet und bietet folgende Endpoints:

### Endpoints

#### `GET /api/projects`

Listet alle Projekte mit optionalen Filtern.

```bash
# Alle Projekte
curl http://localhost:8000/api/projects

# Nach Sprache filtern
curl "http://localhost:8000/api/projects?language=Python"

# Nach Framework filtern
curl "http://localhost:8000/api/projects?framework=React"

# Nach Source filtern
curl "http://localhost:8000/api/projects?source=local"
```

#### `GET /api/projects/{id}`

Gibt ein einzelnes Projekt zurueck.

```bash
curl http://localhost:8000/api/projects/a1b2c3d4e5f6
```

#### `GET /api/search`

Durchsucht alle Projekte.

```bash
# Hybrid-Suche (Standard)
curl "http://localhost:8000/api/search?q=machine+learning"

# Semantische Suche
curl "http://localhost:8000/api/search?q=video+generation&mode=semantic"

# Volltext mit Limit
curl "http://localhost:8000/api/search?q=React&mode=fulltext&limit=5"
```

**Query-Parameter:**

| Parameter | Default | Beschreibung |
|-----------|---------|-------------|
| `q` | erforderlich | Suchbegriff |
| `mode` | `hybrid` | `fulltext`, `semantic`, `hybrid` |
| `limit` | `20` | Max. Ergebnisse (1-100) |

#### `GET /api/stats`

Gibt aggregierte Statistiken zurueck.

```bash
curl http://localhost:8000/api/stats
```

#### `POST /api/scan`

Startet einen Scan ueber die API.

```bash
curl -X POST http://localhost:8000/api/scan \
  -H "Content-Type: application/json" \
  -d '{"path": "~/Repositories", "depth": 3}'
```

---

## Architektur

```
code_explore/
├── models.py              # Pydantic-Datenmodelle
├── database.py            # SQLite + FTS5 (Storage & Volltext)
├── scanner/
│   ├── local.py           # Filesystem-Scanner (findet Git-Repos)
│   └── git_info.py        # Git-Metadaten-Extraktion (GitPython)
├── analyzer/
│   ├── language.py        # Spracherkennung (100+ Extensions)
│   ├── metrics.py         # Quality Metrics (Tests, CI, Docs)
│   ├── dependencies.py    # Dependency-Parsing (11 Formate)
│   └── patterns.py        # Pattern-Erkennung (30+ Patterns)
├── indexer/
│   └── embeddings.py      # Ollama-Embeddings + LanceDB-Storage
├── search/
│   ├── fulltext.py        # SQLite FTS5 Suche
│   ├── semantic.py        # LanceDB Vektor-Suche
│   └── hybrid.py          # Kombinierte Suche mit RRF
├── summarizer/
│   └── ollama.py          # LLM-Summaries via Ollama
├── cli/
│   └── main.py            # Typer CLI (6 Commands)
└── api/
    └── main.py            # FastAPI REST API (6 Endpoints)
```

### Tech-Stack

| Komponente | Technologie | Warum |
|-----------|-------------|-------|
| Sprache | Python 3.11+ | Bestes ML/Embedding-Oekosystem |
| CLI | Typer + Rich | Entwicklerfreundlich, schoene Ausgabe |
| API | FastAPI | Async, schnell, Auto-Docs |
| Datenbank | SQLite + FTS5 | Zero Config, eingebettet, schnelle Volltextsuche |
| Vektoren | LanceDB | Lokal, eingebettet, kein Server noetig |
| Embeddings | Ollama (nomic-embed-text) | Lokal, privat, kostenlos |
| Summaries | Ollama (llama3.2:3b) | Lokales LLM, keine API-Kosten |

### Datenfluss

```
Verzeichnis
    |
    v
[Scanner] --> findet Git-Repos
    |
    v
[Analyzer] --> Sprachen, Deps, Patterns, Metrics, Git-Info
    |
    v
[Database] --> SQLite + FTS5 (persistent in ~/.code-explore/)
    |
    v
[Summarizer] --> Ollama LLM generiert Summary, Tags, Concepts
    |
    v
[Indexer] --> Ollama Embeddings --> LanceDB Vektoren
    |
    v
[Search] --> Fulltext (FTS5) + Semantic (LanceDB) --> Hybrid (RRF)
    |
    v
[CLI / API] --> Ergebnisse anzeigen
```

---

## Datenmodell

### Project

Das zentrale Datenmodell, das alle Informationen zu einem Projekt buendelt:

```python
class Project:
    id: str                          # MD5-Hash des Pfads (12 Zeichen)
    name: str                        # Verzeichnisname
    path: str | None                 # Lokaler Pfad
    remote_url: str | None           # Git-Remote-URL
    source: "local" | "github" | "gitlab"
    status: "pending" | "scanning" | "analyzed" | "indexed" | "error"

    # Analyse-Ergebnisse
    languages: list[LanguageInfo]    # Sprachen mit Dateien/Zeilen/Prozent
    primary_language: str | None     # Hauptsprache (meiste Zeilen)
    frameworks: list[str]            # Erkannte Frameworks
    dependencies: list[DependencyInfo]  # Geparste Dependencies
    patterns: list[PatternInfo]      # Erkannte Patterns mit Confidence
    quality: QualityMetrics          # Tests, CI, Docs, etc.
    git: GitInfo                     # Commits, Contributors, Branch

    # KI-generiert
    summary: str | None              # 2-3 Satz Zusammenfassung
    tags: list[str]                  # 5-10 relevante Tags
    concepts: list[str]              # 3-5 architektonische Konzepte
```

### Erkannte Patterns (Auswahl)

| Kategorie | Patterns |
|-----------|----------|
| API | REST API, GraphQL, gRPC, WebSocket |
| Auth | OAuth, JWT, Auth0, Firebase Auth |
| Database | PostgreSQL, MongoDB, Redis, SQLite, Prisma, TypeORM |
| Framework | React, Vue, Angular, Next.js, FastAPI, Django, Express, Spring |
| Cloud | AWS, GCP, Azure, Docker, Kubernetes |
| Concept | WebScraping, ML/AI, CLI Tool, Browser Extension, Mobile App, Microservices |

---

## Konfiguration

### Speicherorte

| Datei | Pfad | Beschreibung |
|-------|------|-------------|
| SQLite-Datenbank | `~/.code-explore/code-explore.db` | Alle Projekt-Daten + FTS5-Index |
| LanceDB-Vektoren | `~/.code-explore/vectors/` | Embedding-Vektoren fuer semantische Suche |

### Ollama-Modelle

Die verwendeten Modelle koennen in der Konfiguration geaendert werden:

| Modell | Verwendung | Dimensionen |
|--------|-----------|-------------|
| `nomic-embed-text` | Embeddings | 768 |
| `llama3.2:3b` | Summaries | - |

Alternative Modelle koennen durch Aendern der Konstanten in `code_explore/summarizer/ollama.py` und `code_explore/indexer/embeddings.py` konfiguriert werden.

### Datenbank zuruecksetzen

```bash
# Kompletten Index loeschen und neu starten
rm -rf ~/.code-explore/
code-explore scan ~/Repositories --depth 3
code-explore index
```

---

## Entwicklung

### Setup

```bash
# Repository klonen
git clone https://github.com/tobiasoberrauch/code-explore.git
cd code-explore

# Dev-Installation
pip install -e ".[dev]"
```

### Projektstruktur

```
code-explore/
├── code_explore/          # Python-Paket (23 Dateien, ~2.900 LoC)
├── docs/
│   └── plans/             # Design-Dokumente
├── src/                   # Sample-Codebase fuer Tests
├── specs/                 # Feature-Spezifikationen
├── pyproject.toml         # Projekt-Konfiguration
└── README.md              # Diese Datei
```

### Tests ausfuehren

```bash
pytest
```

### Linting

```bash
ruff check .
ruff format .
```

### Neuen Analyzer hinzufuegen

1. Datei in `code_explore/analyzer/` erstellen
2. Funktion schreiben, die einen `Path` nimmt und ein Model zurueckgibt
3. In `code_explore/analyzer/__init__.py` exportieren
4. In `code_explore/cli/main.py` im `scan`-Command aufrufen
5. Ergebnis im `Project`-Model speichern

### Neues Pattern hinzufuegen

In `code_explore/analyzer/patterns.py` ein neues `_PatternRule` hinzufuegen:

```python
_PatternRule(
    name="Mein Pattern",
    category="concept",  # api, auth, database, framework, cloud, concept
    file_patterns=["*muster*", "*.xyz"],          # Dateinamen-Matching
    dir_patterns=["muster-verzeichnis"],           # Verzeichnis-Matching
    content_patterns=[r"import muster_lib"],        # Regex im Datei-Inhalt
    confidence_file=0.6,                            # Confidence bei Datei-Match
    confidence_dir=0.8,                             # Confidence bei Dir-Match
    confidence_content=0.9,                         # Confidence bei Content-Match
)
```

---

## Roadmap

### Phase 1 -- MVP (aktuell)

- [x] CLI mit scan, search, show, index, stats, serve
- [x] Lokales Directory-Scanning
- [x] Vollstaendige Code-Analyse-Pipeline
- [x] SQLite FTS5 Volltext-Suche
- [x] LanceDB semantische Suche
- [x] Ollama Embeddings + Summaries
- [x] FastAPI REST API

### Phase 2 -- Erweiterungen

- [ ] GitHub/GitLab Remote-Scanning via API
- [ ] Next.js Web-UI Dashboard
- [ ] Inkrementelles Re-Indexing (nur geaenderte Projekte)
- [ ] Quality/Maturity Scoring (0-100)
- [ ] Projekt-Vergleich
- [ ] Export (JSON, CSV)

### Phase 3 -- Produkt

- [ ] Tauri Desktop App
- [ ] Team-Features (geteilte Suche)
- [ ] Hosted SaaS-Version
- [ ] CI/CD-Integration
- [ ] IDE-Plugin (VS Code)

---

## Lizenz

MIT

---

*Built with Python, SQLite, LanceDB, Ollama, and a lot of side projects that needed to be found again.*
