"""Extract git metadata from a local repository."""

from pathlib import Path

from git import InvalidGitRepositoryError, Repo

from code_explore.models import GitInfo


def extract_git_info(repo_path: str | Path) -> GitInfo:
    """Read git metadata from the repository at repo_path and return a GitInfo model."""
    repo_path = Path(repo_path).expanduser().resolve()

    try:
        repo = Repo(repo_path)
    except InvalidGitRepositoryError:
        return GitInfo()

    remote_url = _get_remote_url(repo)
    default_branch = _get_default_branch(repo)
    total_commits, last_commit_date, last_commit_message, contributors = _get_commit_info(repo)

    return GitInfo(
        remote_url=remote_url,
        default_branch=default_branch,
        total_commits=total_commits,
        last_commit_date=last_commit_date,
        last_commit_message=last_commit_message,
        contributors=contributors,
    )


def _get_remote_url(repo: Repo) -> str | None:
    if not repo.remotes:
        return None
    try:
        return repo.remotes.origin.url
    except AttributeError:
        return repo.remotes[0].url


def _get_default_branch(repo: Repo) -> str | None:
    if repo.bare:
        try:
            return repo.head.reference.name
        except TypeError:
            return None

    for name in ("main", "master", "develop"):
        if name in repo.heads:
            return name

    try:
        return repo.active_branch.name
    except TypeError:
        return None


def _get_commit_info(
    repo: Repo,
) -> tuple[int, "datetime | None", str | None, list[str]]:
    from datetime import datetime, timezone

    try:
        head_commit = repo.head.commit
    except ValueError:
        return 0, None, None, []

    total_commits = head_commit.count()

    last_commit_date = datetime.fromtimestamp(
        head_commit.committed_date, tz=timezone.utc
    )
    last_commit_message = head_commit.message.strip()

    seen: set[str] = set()
    contributors: list[str] = []
    for commit in repo.iter_commits(max_count=500):
        name = commit.author.name
        if name and name not in seen:
            seen.add(name)
            contributors.append(name)

    return total_commits, last_commit_date, last_commit_message, contributors
