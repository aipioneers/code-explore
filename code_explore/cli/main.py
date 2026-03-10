"""Typer CLI for Code Explore."""

import asyncio
import hashlib
from collections import Counter
from pathlib import Path

import typer
from rich.console import Console
from rich.panel import Panel
from rich.progress import Progress, SpinnerColumn, TextColumn, BarColumn, MofNCompleteColumn
from rich.table import Table
from rich.tree import Tree

from code_explore.database import init_db, save_project, get_project, get_all_projects, get_project_count
from code_explore.models import Project, ProjectSource, ProjectStatus
from code_explore.cli.config_cmd import config_app

app = typer.Typer(
    name="code-explore",
    help="Personal developer knowledge base - index, analyze and search all your projects.",
    no_args_is_help=True,
)
app.add_typer(config_app, name="config")
console = Console()


def _project_id(path: Path) -> str:
    return hashlib.md5(str(path).encode()).hexdigest()[:12]


@app.command()
def scan(
    path: str = typer.Argument(..., help="Root directory to scan for repositories"),
    depth: int = typer.Option(4, "--depth", "-d", help="Maximum directory depth"),
    force: bool = typer.Option(False, "--force", "-f", help="Re-scan existing projects"),
    no_ai: bool = typer.Option(False, "--no-ai", help="Skip AI summaries and tags (requires Ollama)"),
    no_embed: bool = typer.Option(False, "--no-embed", help="Skip vector embeddings"),
    model: str | None = typer.Option(None, "--model", "-m", help="Ollama model name"),
) -> None:
    """Scan repositories, analyze, summarize, and index — all in one step."""
    from code_explore.scanner.local import scan_local_repos
    from code_explore.scanner.git_info import extract_git_info, get_git_head
    from code_explore.scanner.readme import read_readme, list_key_files
    from code_explore.analyzer.language import detect_languages
    from code_explore.analyzer.metrics import calculate_metrics
    from code_explore.analyzer.dependencies import detect_dependencies
    from code_explore.analyzer.patterns import detect_patterns

    init_db()
    root = Path(path).expanduser().resolve()

    if not root.is_dir():
        console.print(f"[red]Error:[/red] Path does not exist: {root}")
        raise typer.Exit(1)

    steps = ["scan", "analyze"]
    if not no_ai:
        steps.append("summarize + tag")
    if not no_embed:
        steps.append("embed")
    console.print(Panel(
        f"Scanning [bold cyan]{root}[/bold cyan] (depth={depth})\n"
        f"Steps: {' → '.join(steps)}",
        title="Code Explore",
    ))

    repos = asyncio.run(scan_local_repos(root, max_depth=depth))

    if not repos:
        console.print("[yellow]No repositories found.[/yellow]")
        raise typer.Exit(0)

    results: list[Project] = []
    new_or_changed: list[Project] = []

    with Progress(
        SpinnerColumn(),
        TextColumn("[progress.description]{task.description}"),
        BarColumn(),
        MofNCompleteColumn(),
        console=console,
    ) as progress:
        # Phase 1: Scan & analyze
        task = progress.add_task("Analyzing repositories...", total=len(repos))

        for repo_path in repos:
            pid = _project_id(repo_path)

            if not force:
                existing = get_project(pid)
                if existing:
                    current_head = get_git_head(repo_path)
                    if current_head and existing.git_head == current_head:
                        progress.update(task, advance=1, description=f"[dim]Skipping {repo_path.name}[/dim]")
                        results.append(existing)
                        continue

            progress.update(task, advance=0, description=f"Analyzing [cyan]{repo_path.name}[/cyan]")

            from datetime import datetime

            languages, primary_language = detect_languages(repo_path)
            quality = calculate_metrics(repo_path)
            dependencies = detect_dependencies(repo_path)
            patterns, frameworks = detect_patterns(repo_path)
            git_info = extract_git_info(repo_path)
            head_sha = get_git_head(repo_path)
            readme_snippet = read_readme(repo_path)
            key_file_list = list_key_files(repo_path)

            project = Project(
                id=pid,
                name=repo_path.name,
                path=str(repo_path),
                source=ProjectSource.LOCAL,
                status=ProjectStatus.ANALYZED,
                languages=languages,
                primary_language=primary_language,
                frameworks=frameworks,
                dependencies=dependencies,
                patterns=patterns,
                quality=quality,
                git=git_info,
                git_head=head_sha,
                readme_snippet=readme_snippet,
                key_files=key_file_list,
                scanned_at=datetime.now(),
                analyzed_at=datetime.now(),
            )

            save_project(project)
            results.append(project)
            new_or_changed.append(project)
            progress.update(task, advance=1)

        # Phase 2: AI summaries & tags (all projects that need them)
        if not no_ai:
            from code_explore.summarizer.ollama import summarize_project

            ai_candidates = [p for p in results if not p.summary or not p.ai_tags]
            if ai_candidates:
                ai_task = progress.add_task("AI summaries & tags...", total=len(ai_candidates))
                summarized = 0
                tagged = 0

                for project in ai_candidates:
                    progress.update(ai_task, description=f"Summarizing [cyan]{project.name}[/cyan]")
                    needs_summary = not project.summary
                    needs_tags = not project.ai_tags

                    if model:
                        summary, tags, concepts, ai_tags = summarize_project(project, model=model)
                    else:
                        summary, tags, concepts, ai_tags = summarize_project(project)

                    if summary and needs_summary:
                        project.summary = summary
                        project.tags = tags
                        project.concepts = concepts
                        summarized += 1
                    if ai_tags and needs_tags:
                        project.ai_tags = ai_tags
                        tagged += 1
                    if summary or ai_tags:
                        save_project(project)
                    progress.update(ai_task, advance=1)

        # Phase 3: Embeddings (all projects)
        if not no_embed:
            from code_explore.indexer.embeddings import index_project as embed_project
            from datetime import datetime

            embed_task = progress.add_task("Generating embeddings...", total=len(results))
            for project in results:
                progress.update(embed_task, description=f"Embedding [cyan]{project.name}[/cyan]")
                embed_project(project)
                project.status = ProjectStatus.INDEXED
                project.indexed_at = datetime.now()
                save_project(project)
                progress.update(embed_task, advance=1)

    table = Table(title=f"Scanned {len(results)} Projects")
    table.add_column("Name", style="cyan", no_wrap=True)
    table.add_column("Language", style="green")
    table.add_column("Files", justify="right", style="yellow")
    table.add_column("Lines", justify="right")
    table.add_column("Frameworks", style="magenta")
    table.add_column("Status", style="blue")

    for p in sorted(results, key=lambda x: x.name.lower()):
        table.add_row(
            p.name,
            p.primary_language or "-",
            str(p.quality.total_files),
            str(p.quality.total_lines),
            ", ".join(p.frameworks[:3]) if p.frameworks else "-",
            p.status.value,
        )

    console.print(table)


@app.command()
def search(
    query: str = typer.Argument(..., help="Search query"),
    mode: str = typer.Option("hybrid", "--mode", "-m", help="Search mode: fulltext, semantic, or hybrid"),
    limit: int = typer.Option(None, "--limit", "-l", help="Maximum results"),
    language: str = typer.Option(None, "--language", "-L", help="Filter by primary language"),
    framework: str = typer.Option(None, "--framework", "-F", help="Filter by framework"),
    pattern: str = typer.Option(None, "--pattern", help="Filter by architectural pattern"),
    tag: str = typer.Option(None, "--tag", "-t", help="Filter by AI-generated tag"),
) -> None:
    """Search across all indexed projects."""
    from code_explore.config import get_config
    from code_explore.search.filters import apply_filters

    if limit is None:
        limit = get_config().result_limit
    init_db()

    if mode == "fulltext":
        from code_explore.search.fulltext import search as fulltext_search
        results = fulltext_search(query, limit=limit)
    elif mode == "semantic":
        from code_explore.search.semantic import search as semantic_search
        results = semantic_search(query, limit=limit)
    else:
        from code_explore.search.hybrid import search as hybrid_search
        results = hybrid_search(query, limit=limit)

    # Apply post-filters
    results = apply_filters(results, language=language, framework=framework, pattern=pattern, tag=tag)

    # Show active filters
    active_filters = []
    if language:
        active_filters.append(f"language={language}")
    if framework:
        active_filters.append(f"framework={framework}")
    if pattern:
        active_filters.append(f"pattern={pattern}")
    if tag:
        active_filters.append(f"tag={tag}")

    if active_filters:
        console.print(f"[dim]Filters: {', '.join(active_filters)}[/dim]")

    if not results:
        console.print(f"[yellow]No results found for:[/yellow] {query}")
        raise typer.Exit(0)

    table = Table(title=f"Search Results for '{query}' ({mode})")
    table.add_column("Name", style="cyan", no_wrap=True)
    table.add_column("Language", style="green")
    table.add_column("Summary", style="white", max_width=50)
    table.add_column("Score", justify="right", style="yellow")

    for r in results:
        summary_snippet = (r.project.summary or "")[:80]
        if r.project.summary and len(r.project.summary) > 80:
            summary_snippet += "..."

        table.add_row(
            r.project.name,
            r.project.primary_language or "-",
            summary_snippet or "-",
            f"{r.score:.3f}",
        )

    console.print(table)
    console.print(f"[dim]{len(results)} result(s) found[/dim]")


@app.command()
def show(
    name_or_id: str = typer.Argument(..., help="Project name or ID"),
) -> None:
    """Show detailed information about a project."""
    init_db()

    project = get_project(name_or_id)
    if not project:
        projects = get_all_projects()
        matches = [p for p in projects if p.name.lower() == name_or_id.lower()]
        if not matches:
            matches = [p for p in projects if name_or_id.lower() in p.name.lower()]
        if matches:
            project = matches[0]

    if not project:
        console.print(f"[red]Project not found:[/red] {name_or_id}")
        raise typer.Exit(1)

    tree = Tree(f"[bold cyan]{project.name}[/bold cyan]")

    info_branch = tree.add("[bold]General[/bold]")
    info_branch.add(f"ID: {project.id}")
    info_branch.add(f"Path: {project.path or 'N/A'}")
    info_branch.add(f"Source: {project.source.value}")
    info_branch.add(f"Status: {project.status.value}")
    if project.git.remote_url:
        info_branch.add(f"Remote: {project.git.remote_url}")

    if project.languages:
        lang_branch = tree.add("[bold]Languages[/bold]")
        for lang in project.languages:
            lang_branch.add(f"{lang.name}: {lang.files} files, {lang.lines} lines ({lang.percentage}%)")

    if project.frameworks:
        fw_branch = tree.add("[bold]Frameworks[/bold]")
        for fw in project.frameworks:
            fw_branch.add(fw)

    if project.dependencies:
        dep_branch = tree.add(f"[bold]Dependencies[/bold] ({len(project.dependencies)})")
        for dep in project.dependencies[:20]:
            version = f" ({dep.version})" if dep.version else ""
            dev_tag = " [dim][dev][/dim]" if dep.dev else ""
            dep_branch.add(f"{dep.name}{version}{dev_tag}")
        if len(project.dependencies) > 20:
            dep_branch.add(f"[dim]... and {len(project.dependencies) - 20} more[/dim]")

    if project.patterns:
        pat_branch = tree.add("[bold]Patterns[/bold]")
        for pat in project.patterns:
            pat_branch.add(f"{pat.name} ({pat.category}) - confidence: {pat.confidence:.0%}")

    quality = project.quality
    q_branch = tree.add("[bold]Quality Metrics[/bold]")
    q_branch.add(f"Total files: {quality.total_files}")
    q_branch.add(f"Total lines: {quality.total_lines}")
    q_branch.add(f"Avg file size: {quality.avg_file_size:.0f} lines")
    checks = []
    if quality.has_tests:
        checks.append("[green]tests[/green]")
    if quality.has_ci:
        checks.append("[green]CI[/green]")
    if quality.has_docs:
        checks.append("[green]docs[/green]")
    if quality.has_readme:
        checks.append("[green]README[/green]")
    if quality.has_license:
        checks.append("[green]license[/green]")
    if quality.has_gitignore:
        checks.append("[green].gitignore[/green]")
    if checks:
        q_branch.add(f"Has: {', '.join(checks)}")

    if project.git.total_commits:
        git_branch = tree.add("[bold]Git[/bold]")
        git_branch.add(f"Branch: {project.git.default_branch or 'N/A'}")
        git_branch.add(f"Commits: {project.git.total_commits}")
        if project.git.last_commit_date:
            git_branch.add(f"Last commit: {project.git.last_commit_date:%Y-%m-%d}")
        if project.git.last_commit_message:
            git_branch.add(f"Message: {project.git.last_commit_message[:80]}")
        if project.git.contributors:
            git_branch.add(f"Contributors: {', '.join(project.git.contributors[:10])}")

    if project.summary:
        tree.add(f"[bold]Summary[/bold]\n{project.summary}")
    if project.tags:
        tree.add(f"[bold]Tags:[/bold] {', '.join(project.tags)}")
    if project.concepts:
        tree.add(f"[bold]Concepts:[/bold] {', '.join(project.concepts)}")

    # AI Tags grouped by category
    if project.ai_tags:
        ai_branch = tree.add("[bold]AI Tags[/bold]")
        by_category: dict[str, list[str]] = {}
        for t in project.ai_tags:
            cat = t.category.value if hasattr(t.category, "value") else str(t.category)
            by_category.setdefault(cat, []).append(t.value)
        for cat, values in sorted(by_category.items()):
            ai_branch.add(f"[magenta]{cat}:[/magenta] {', '.join(values)}")

    console.print(Panel(tree, title=f"Project: {project.name}", border_style="cyan"))


@app.command()
def index(
    model: str | None = typer.Option(None, "--model", "-m", help="Ollama model name for summarization"),
) -> None:
    """Re-generate AI summaries and embeddings for existing projects."""
    from code_explore.indexer.embeddings import index_project as embed_project, index_all_projects
    from code_explore.summarizer.ollama import summarize_project

    init_db()
    projects = get_all_projects()

    if not projects:
        console.print("[yellow]No projects found. Run 'scan' first.[/yellow]")
        raise typer.Exit(0)

    console.print(Panel(f"Indexing [bold]{len(projects)}[/bold] projects", title="Code Explore"))

    with Progress(
        SpinnerColumn(),
        TextColumn("[progress.description]{task.description}"),
        BarColumn(),
        MofNCompleteColumn(),
        console=console,
    ) as progress:
        summary_task = progress.add_task("Generating summaries & AI tags...", total=len(projects))
        summarized = 0
        tagged = 0

        for project in projects:
            needs_summary = not project.summary
            needs_tags = not project.ai_tags

            if needs_summary or needs_tags:
                progress.update(summary_task, description=f"Summarizing [cyan]{project.name}[/cyan]")
                if model:
                    summary, tags, concepts, ai_tags = summarize_project(project, model=model)
                else:
                    summary, tags, concepts, ai_tags = summarize_project(project)
                if summary and needs_summary:
                    project.summary = summary
                    project.tags = tags
                    project.concepts = concepts
                    summarized += 1
                if ai_tags:
                    project.ai_tags = ai_tags
                    tagged += 1
                if summary or ai_tags:
                    save_project(project)
            progress.update(summary_task, advance=1)

        embed_task = progress.add_task("Generating embeddings...", total=len(projects))
        indexed = 0

        for project in projects:
            progress.update(embed_task, description=f"Embedding [cyan]{project.name}[/cyan]")
            from datetime import datetime
            embed_project(project)
            project.status = ProjectStatus.INDEXED
            project.indexed_at = datetime.now()
            save_project(project)
            indexed += 1
            progress.update(embed_task, advance=1)

    console.print(f"[green]Summarized {summarized}, AI-tagged {tagged}, indexed {indexed} projects.[/green]")


@app.command()
def tags(
    category: str = typer.Option(None, "--category", "-c", help="Filter by category: domain, technology-role, maturity"),
) -> None:
    """List all unique AI tags across projects with counts."""
    init_db()
    projects = get_all_projects()

    if not projects:
        console.print("[yellow]No projects found. Run 'scan' first.[/yellow]")
        raise typer.Exit(0)

    # Collect all AI tags with counts
    tag_counts: Counter[str] = Counter()
    tag_categories: dict[str, str] = {}

    for p in projects:
        for t in p.ai_tags:
            cat = t.category.value if hasattr(t.category, "value") else str(t.category)
            if category and cat != category:
                continue
            tag_counts[t.value] += 1
            tag_categories[t.value] = cat

    if not tag_counts:
        if category:
            console.print(f"[yellow]No AI tags found for category '{category}'.[/yellow]")
        else:
            console.print("[yellow]No AI tags found. Run 'cex index' to generate tags.[/yellow]")
        raise typer.Exit(0)

    # Group by category
    by_category: dict[str, list[tuple[str, int]]] = {}
    for tag_value, count in tag_counts.most_common():
        cat = tag_categories[tag_value]
        by_category.setdefault(cat, []).append((tag_value, count))

    total_tags = len(tag_counts)
    total_projects = len(projects)
    console.print(Panel(
        f"[bold]{total_tags}[/bold] unique AI tags across [bold]{total_projects}[/bold] projects",
        title="AI Tags",
        border_style="magenta",
    ))

    for cat in sorted(by_category.keys()):
        items = by_category[cat]
        table = Table(title=f"{cat} tags")
        table.add_column("Tag", style="magenta")
        table.add_column("Projects", justify="right", style="yellow")
        for tag_value, count in items:
            table.add_row(tag_value, str(count))
        console.print(table)


@app.command()
def update(
    force: bool = typer.Option(False, "--force", "-f", help="Re-analyze even if git HEAD unchanged"),
    reindex: bool = typer.Option(False, "--reindex", help="Regenerate embeddings for updated projects"),
    resummarize: bool = typer.Option(False, "--resummarize", help="Regenerate AI summaries for updated projects"),
    retag: bool = typer.Option(False, "--retag", help="Regenerate AI tags for updated projects"),
) -> None:
    """Update existing projects by re-analyzing changed repositories."""
    from datetime import datetime

    from code_explore.scanner.git_info import extract_git_info, get_git_head
    from code_explore.scanner.readme import read_readme, list_key_files
    from code_explore.analyzer.language import detect_languages
    from code_explore.analyzer.metrics import calculate_metrics
    from code_explore.analyzer.dependencies import detect_dependencies
    from code_explore.analyzer.patterns import detect_patterns

    init_db()
    projects = get_all_projects()

    if not projects:
        console.print("[yellow]No projects found. Run 'scan' first.[/yellow]")
        raise typer.Exit(0)

    console.print(Panel(f"Updating [bold]{len(projects)}[/bold] projects", title="Code Explore Update"))

    checked = 0
    unchanged = 0
    updated = 0
    errors = 0
    updated_projects: list[Project] = []

    with Progress(
        SpinnerColumn(),
        TextColumn("[progress.description]{task.description}"),
        BarColumn(),
        MofNCompleteColumn(),
        console=console,
    ) as progress:
        task = progress.add_task("Checking projects...", total=len(projects))

        for project in projects:
            checked += 1

            if not project.path:
                progress.update(task, advance=1, description=f"[dim]No path for {project.name}[/dim]")
                continue

            repo_path = Path(project.path)

            if not repo_path.is_dir():
                progress.update(task, advance=1, description=f"[red]Missing: {project.name}[/red]")
                errors += 1
                continue

            if not force:
                current_head = get_git_head(repo_path)
                if current_head and project.git_head == current_head:
                    progress.update(task, advance=1, description=f"[dim]Unchanged: {project.name}[/dim]")
                    unchanged += 1
                    continue

            progress.update(task, advance=0, description=f"Analyzing [cyan]{project.name}[/cyan]")

            languages, primary_language = detect_languages(repo_path)
            quality = calculate_metrics(repo_path)
            dependencies = detect_dependencies(repo_path)
            patterns, frameworks = detect_patterns(repo_path)
            git_info = extract_git_info(repo_path)
            head_sha = get_git_head(repo_path)
            readme_snippet = read_readme(repo_path)
            key_file_list = list_key_files(repo_path)

            project.languages = languages
            project.primary_language = primary_language
            project.frameworks = frameworks
            project.dependencies = dependencies
            project.patterns = patterns
            project.quality = quality
            project.git = git_info
            project.git_head = head_sha
            project.readme_snippet = readme_snippet
            project.key_files = key_file_list
            project.status = ProjectStatus.ANALYZED
            project.analyzed_at = datetime.now()

            save_project(project)
            updated_projects.append(project)
            updated += 1
            progress.update(task, advance=1)

        if (resummarize or retag) and updated_projects:
            from code_explore.summarizer.ollama import summarize_project

            summary_task = progress.add_task("Resummarizing...", total=len(updated_projects))
            for project in updated_projects:
                progress.update(summary_task, description=f"Summarizing [cyan]{project.name}[/cyan]")
                if resummarize:
                    project.summary = None
                    project.tags = []
                    project.concepts = []
                if retag:
                    project.ai_tags = []
                summary, tags, concepts, ai_tags = summarize_project(project)
                if summary and resummarize:
                    project.summary = summary
                    project.tags = tags
                    project.concepts = concepts
                if ai_tags and retag:
                    project.ai_tags = ai_tags
                save_project(project)
                progress.update(summary_task, advance=1)

        if reindex and updated_projects:
            from code_explore.indexer.embeddings import index_project as embed_project

            embed_task = progress.add_task("Re-indexing...", total=len(updated_projects))
            for project in updated_projects:
                progress.update(embed_task, description=f"Embedding [cyan]{project.name}[/cyan]")
                embed_project(project)
                project.status = ProjectStatus.INDEXED
                project.indexed_at = datetime.now()
                save_project(project)
                progress.update(embed_task, advance=1)

    console.print()
    console.print(Panel(
        f"[bold]Checked:[/bold] {checked}\n"
        f"[bold]Unchanged (skipped):[/bold] {unchanged}\n"
        f"[bold]Updated:[/bold] {updated}\n"
        f"[bold]Errors (missing path):[/bold] {errors}",
        title="Update Summary",
        border_style="green" if errors == 0 else "yellow",
    ))


@app.command()
def stats() -> None:
    """Show overview statistics of all indexed projects."""
    init_db()
    projects = get_all_projects()

    if not projects:
        console.print("[yellow]No projects found. Run 'scan' first.[/yellow]")
        raise typer.Exit(0)

    lang_counter: Counter[str] = Counter()
    framework_counter: Counter[str] = Counter()
    pattern_counter: Counter[str] = Counter()
    total_files = 0
    total_lines = 0
    status_counter: Counter[str] = Counter()

    for p in projects:
        if p.primary_language:
            lang_counter[p.primary_language] += 1
        for fw in p.frameworks:
            framework_counter[fw] += 1
        for pat in p.patterns:
            pattern_counter[pat.name] += 1
        total_files += p.quality.total_files
        total_lines += p.quality.total_lines
        status_counter[p.status.value] += 1

    console.print(Panel(
        f"[bold]Total projects:[/bold] {len(projects)}\n"
        f"[bold]Total files:[/bold] {total_files:,}\n"
        f"[bold]Total lines:[/bold] {total_lines:,}",
        title="Code Explore Stats",
        border_style="cyan",
    ))

    if lang_counter:
        lang_table = Table(title="Languages")
        lang_table.add_column("Language", style="green")
        lang_table.add_column("Projects", justify="right", style="yellow")
        for lang, count in lang_counter.most_common(15):
            lang_table.add_row(lang, str(count))
        console.print(lang_table)

    if framework_counter:
        fw_table = Table(title="Top Frameworks")
        fw_table.add_column("Framework", style="magenta")
        fw_table.add_column("Projects", justify="right", style="yellow")
        for fw, count in framework_counter.most_common(15):
            fw_table.add_row(fw, str(count))
        console.print(fw_table)

    if pattern_counter:
        pat_table = Table(title="Top Patterns")
        pat_table.add_column("Pattern", style="blue")
        pat_table.add_column("Projects", justify="right", style="yellow")
        for pat, count in pattern_counter.most_common(10):
            pat_table.add_row(pat, str(count))
        console.print(pat_table)

    if status_counter:
        status_table = Table(title="Project Status")
        status_table.add_column("Status", style="cyan")
        status_table.add_column("Count", justify="right", style="yellow")
        for status, count in status_counter.most_common():
            status_table.add_row(status, str(count))
        console.print(status_table)


@app.command()
def serve(
    host: str = typer.Option("0.0.0.0", "--host", "-h", help="Bind host"),
    port: int = typer.Option(8000, "--port", "-p", help="Bind port"),
) -> None:
    """Start the FastAPI server with web dashboard."""
    import uvicorn

    init_db()
    console.print(Panel(
        f"Starting server on [bold cyan]http://{host}:{port}[/bold cyan]\n"
        f"Dashboard at [bold cyan]http://{host}:{port}[/bold cyan]\n"
        f"API docs at [bold cyan]http://{host}:{port}/docs[/bold cyan]",
        title="Code Explore API",
    ))
    uvicorn.run("code_explore.api.main:app", host=host, port=port, reload=False)


if __name__ == "__main__":
    app()
