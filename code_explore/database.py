"""SQLite database with FTS5 for Code Explore."""

import sqlite3
from datetime import datetime
from pathlib import Path

from code_explore.models import Project


def get_db_path() -> Path:
    from code_explore.config import get_config

    path = get_config().db_path
    path.parent.mkdir(parents=True, exist_ok=True)
    return path


def get_connection(db_path: Path | None = None) -> sqlite3.Connection:
    path = db_path or get_db_path()
    conn = sqlite3.connect(str(path))
    conn.row_factory = sqlite3.Row
    conn.execute("PRAGMA journal_mode=WAL")
    conn.execute("PRAGMA foreign_keys=ON")
    return conn


def _migrate_db(conn: sqlite3.Connection) -> None:
    """Add columns/tables that may not exist in older databases."""
    cursor = conn.execute("PRAGMA table_info(projects)")
    columns = {row["name"] for row in cursor.fetchall()}

    if not columns:
        # Table doesn't exist yet, nothing to migrate
        return

    if "readme_snippet" not in columns:
        conn.execute("ALTER TABLE projects ADD COLUMN readme_snippet TEXT")
    if "git_head" not in columns:
        conn.execute("ALTER TABLE projects ADD COLUMN git_head TEXT")

    # Rebuild FTS table to include readme_snippet if needed
    try:
        conn.execute("SELECT readme_snippet FROM projects_fts LIMIT 1")
    except sqlite3.OperationalError:
        conn.execute("DROP TABLE IF EXISTS projects_fts")
        conn.execute("DROP TRIGGER IF EXISTS projects_ai")
        conn.execute("DROP TRIGGER IF EXISTS projects_ad")
        conn.execute("DROP TRIGGER IF EXISTS projects_au")
    conn.commit()


def init_db(db_path: Path | None = None) -> None:
    conn = get_connection(db_path)
    _migrate_db(conn)
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
            readme_snippet TEXT,
            git_head TEXT,
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
            readme_snippet,
            content='projects',
            content_rowid='rowid',
            tokenize='porter unicode61'
        );

        CREATE TRIGGER IF NOT EXISTS projects_ai AFTER INSERT ON projects BEGIN
            INSERT INTO projects_fts(rowid, name, summary, tags, readme_snippet)
            VALUES (new.rowid, new.name, new.summary, new.tags, new.readme_snippet);
        END;

        CREATE TRIGGER IF NOT EXISTS projects_ad AFTER DELETE ON projects BEGIN
            INSERT INTO projects_fts(projects_fts, rowid, name, summary, tags, readme_snippet)
            VALUES ('delete', old.rowid, old.name, old.summary, old.tags, old.readme_snippet);
        END;

        CREATE TRIGGER IF NOT EXISTS projects_au AFTER UPDATE ON projects BEGIN
            INSERT INTO projects_fts(projects_fts, rowid, name, summary, tags, readme_snippet)
            VALUES ('delete', old.rowid, old.name, old.summary, old.tags, old.readme_snippet);
            INSERT INTO projects_fts(rowid, name, summary, tags, readme_snippet)
            VALUES (new.rowid, new.name, new.summary, new.tags, new.readme_snippet);
        END;
    """)
    conn.commit()
    conn.close()


def save_project(project: Project, db_path: Path | None = None) -> None:
    conn = get_connection(db_path)
    now = datetime.now().isoformat()
    tag_parts = list(project.tags) if project.tags else []
    if project.ai_tags:
        tag_parts.extend(t.value for t in project.ai_tags)
    tags_str = ", ".join(tag_parts)

    readme_str = (project.readme_snippet or "")[:2000]

    conn.execute(
        """
        INSERT OR REPLACE INTO projects
        (id, name, path, remote_url, source, status, data, summary, tags,
         readme_snippet, git_head, scanned_at, analyzed_at, indexed_at, updated_at)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """,
        (
            project.id, project.name, project.path, project.remote_url,
            project.source.value, project.status.value,
            project.model_dump_json(),
            project.summary, tags_str,
            readme_str, project.git_head,
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
    try:
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
    except sqlite3.OperationalError:
        # Query may contain special characters that break FTS5 syntax.
        # Escape by wrapping each token in double quotes.
        tokens = query.split()
        escaped = " ".join(f'"{t}"' for t in tokens if t)
        try:
            rows = conn.execute(
                """
                SELECT p.id, fts.rank
                FROM projects_fts fts
                JOIN projects p ON p.rowid = fts.rowid
                WHERE projects_fts MATCH ?
                ORDER BY fts.rank
                LIMIT ?
                """,
                (escaped, limit),
            ).fetchall()
        except sqlite3.OperationalError:
            conn.close()
            return []
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
