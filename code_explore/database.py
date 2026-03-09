"""SQLite database with FTS5 for Code Explore."""

import sqlite3
from datetime import datetime
from pathlib import Path

from code_explore.models import Project

DEFAULT_DB_PATH = Path.home() / ".code-explore" / "code-explore.db"


def get_db_path() -> Path:
    path = DEFAULT_DB_PATH
    path.parent.mkdir(parents=True, exist_ok=True)
    return path


def get_connection(db_path: Path | None = None) -> sqlite3.Connection:
    path = db_path or get_db_path()
    conn = sqlite3.connect(str(path))
    conn.row_factory = sqlite3.Row
    conn.execute("PRAGMA journal_mode=WAL")
    conn.execute("PRAGMA foreign_keys=ON")
    return conn


def init_db(db_path: Path | None = None) -> None:
    conn = get_connection(db_path)
    conn.executescript("""
        CREATE TABLE IF NOT EXISTS projects (
            id TEXT PRIMARY KEY,
            name TEXT NOT NULL,
            path TEXT,
            remote_url TEXT,
            source TEXT DEFAULT 'local',
            status TEXT DEFAULT 'pending',
            data JSON NOT NULL,
            summary TEXT,
            tags TEXT,
            scanned_at TEXT,
            analyzed_at TEXT,
            indexed_at TEXT,
            created_at TEXT DEFAULT (datetime('now')),
            updated_at TEXT DEFAULT (datetime('now'))
        );

        CREATE INDEX IF NOT EXISTS idx_projects_name ON projects(name);
        CREATE INDEX IF NOT EXISTS idx_projects_source ON projects(source);
        CREATE INDEX IF NOT EXISTS idx_projects_status ON projects(status);

        CREATE VIRTUAL TABLE IF NOT EXISTS projects_fts USING fts5(
            name,
            summary,
            tags,
            content='projects',
            content_rowid='rowid',
            tokenize='porter unicode61'
        );

        CREATE TRIGGER IF NOT EXISTS projects_ai AFTER INSERT ON projects BEGIN
            INSERT INTO projects_fts(rowid, name, summary, tags)
            VALUES (new.rowid, new.name, new.summary, new.tags);
        END;

        CREATE TRIGGER IF NOT EXISTS projects_ad AFTER DELETE ON projects BEGIN
            INSERT INTO projects_fts(projects_fts, rowid, name, summary, tags)
            VALUES ('delete', old.rowid, old.name, old.summary, old.tags);
        END;

        CREATE TRIGGER IF NOT EXISTS projects_au AFTER UPDATE ON projects BEGIN
            INSERT INTO projects_fts(projects_fts, rowid, name, summary, tags)
            VALUES ('delete', old.rowid, old.name, old.summary, old.tags);
            INSERT INTO projects_fts(rowid, name, summary, tags)
            VALUES (new.rowid, new.name, new.summary, new.tags);
        END;
    """)
    conn.commit()
    conn.close()


def save_project(project: Project, db_path: Path | None = None) -> None:
    conn = get_connection(db_path)
    now = datetime.now().isoformat()
    tags_str = ", ".join(project.tags) if project.tags else ""

    conn.execute(
        """
        INSERT OR REPLACE INTO projects
        (id, name, path, remote_url, source, status, data, summary, tags,
         scanned_at, analyzed_at, indexed_at, updated_at)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """,
        (
            project.id, project.name, project.path, project.remote_url,
            project.source.value, project.status.value,
            project.model_dump_json(),
            project.summary, tags_str,
            project.scanned_at.isoformat() if project.scanned_at else None,
            project.analyzed_at.isoformat() if project.analyzed_at else None,
            project.indexed_at.isoformat() if project.indexed_at else None,
            now,
        ),
    )
    conn.commit()
    conn.close()


def get_project(project_id: str, db_path: Path | None = None) -> Project | None:
    conn = get_connection(db_path)
    row = conn.execute("SELECT data FROM projects WHERE id = ?", (project_id,)).fetchone()
    conn.close()
    if row:
        return Project.model_validate_json(row["data"])
    return None


def get_all_projects(db_path: Path | None = None) -> list[Project]:
    conn = get_connection(db_path)
    rows = conn.execute("SELECT data FROM projects ORDER BY name").fetchall()
    conn.close()
    return [Project.model_validate_json(row["data"]) for row in rows]


def search_fulltext(
    query: str, limit: int = 20, db_path: Path | None = None
) -> list[tuple[str, float]]:
    conn = get_connection(db_path)
    rows = conn.execute(
        """
        SELECT p.id, fts.rank
        FROM projects_fts fts
        JOIN projects p ON p.rowid = fts.rowid
        WHERE projects_fts MATCH ?
        ORDER BY fts.rank
        LIMIT ?
        """,
        (query, limit),
    ).fetchall()
    conn.close()
    return [(row["id"], row["rank"]) for row in rows]


def delete_project(project_id: str, db_path: Path | None = None) -> None:
    conn = get_connection(db_path)
    conn.execute("DELETE FROM projects WHERE id = ?", (project_id,))
    conn.commit()
    conn.close()


def get_project_count(db_path: Path | None = None) -> int:
    conn = get_connection(db_path)
    count = conn.execute("SELECT COUNT(*) as cnt FROM projects").fetchone()["cnt"]
    conn.close()
    return count
