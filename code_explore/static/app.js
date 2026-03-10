/* Code Explore Dashboard — v0.4.0 */

const API = {
    async get(path) {
        const resp = await fetch(`/api${path}`);
        if (!resp.ok) throw new Error(`API error: ${resp.status}`);
        return resp.json();
    },
    stats() { return this.get('/stats'); },
    projects(params) { return this.get('/projects' + qs(params)); },
    project(id) { return this.get(`/projects/${id}`); },
    search(params) { return this.get('/search' + qs(params)); },
    facets(params) { return this.get('/facets' + qs(params)); },
    tags(params) { return this.get('/tags' + qs(params)); },
};

function qs(params) {
    if (!params) return '';
    const p = new URLSearchParams();
    for (const [k, v] of Object.entries(params)) {
        if (v) p.set(k, v);
    }
    const s = p.toString();
    return s ? '?' + s : '';
}

function el(tag, attrs, ...children) {
    const e = document.createElement(tag);
    if (attrs) for (const [k, v] of Object.entries(attrs)) {
        if (k === 'class') e.className = v;
        else if (k.startsWith('on')) e.addEventListener(k.slice(2), v);
        else if (k === 'html') e.innerHTML = v;
        else e.setAttribute(k, v);
    }
    for (const c of children) {
        if (typeof c === 'string') e.appendChild(document.createTextNode(c));
        else if (c) e.appendChild(c);
    }
    return e;
}

/* ---- State ---- */
let searchTimeout = null;
let lastHash = '';

/* ---- Routing ---- */
function route() {
    const hash = location.hash || '#/';
    lastHash = hash;

    document.querySelectorAll('.view').forEach(v => v.classList.add('hidden'));
    document.querySelectorAll('.nav-link').forEach(l => l.classList.remove('active'));

    if (hash.startsWith('#/project/')) {
        const id = hash.slice(10);
        showView('detail');
        loadProjectDetail(id);
    } else if (hash.startsWith('#/search')) {
        showView('search');
        document.querySelector('[data-view="search"]').classList.add('active');
        restoreSearchFromHash();
    } else {
        showView('overview');
        document.querySelector('[data-view="overview"]').classList.add('active');
        loadOverview();
    }
}

function showView(name) {
    document.querySelectorAll('.view').forEach(v => v.classList.add('hidden'));
    document.getElementById('view-' + name).classList.remove('hidden');
}

/* ---- Overview ---- */
async function loadOverview() {
    try {
        const [stats, tagsData] = await Promise.all([API.stats(), API.tags()]);
        renderStats(stats);
        renderTagCloud(tagsData.tags);
        const projects = await API.projects();
        renderProjectList('project-list', projects);
    } catch (err) {
        document.getElementById('stats-grid').innerHTML =
            '<div class="empty-state"><div class="empty-icon">!</div><p>Could not load data. Is the server running?</p></div>';
    }
}

function renderStats(stats) {
    const grid = document.getElementById('stats-grid');
    grid.innerHTML = '';

    grid.appendChild(statCard(stats.total_projects, 'Projects', null));
    grid.appendChild(statCard(stats.total_files.toLocaleString(), 'Files', null));
    grid.appendChild(statCard(stats.total_lines.toLocaleString(), 'Lines of Code', null));

    const topLangs = Object.entries(stats.languages).slice(0, 5);
    if (topLangs.length) {
        grid.appendChild(statCard(topLangs.length, 'Languages', topLangs));
    }

    const topFw = Object.entries(stats.frameworks).slice(0, 5);
    if (topFw.length) {
        grid.appendChild(statCard(topFw.length, 'Frameworks', topFw));
    }
}

function statCard(value, label, items) {
    const card = el('div', { class: 'stat-card' },
        el('div', { class: 'stat-value' }, String(value)),
        el('div', { class: 'stat-label' }, label),
    );
    if (items) {
        const itemsDiv = el('div', { class: 'stat-items' });
        for (const [name, count] of items) {
            itemsDiv.appendChild(el('span', null,
                el('strong', null, String(count)), ' ' + name
            ));
        }
        card.appendChild(itemsDiv);
    }
    return card;
}

function renderTagCloud(tags) {
    const cloud = document.getElementById('tag-cloud');
    cloud.innerHTML = '';
    if (!tags || !tags.length) {
        cloud.innerHTML = '<span class="empty-state">No AI tags yet. Run <code>cex index</code> to generate.</span>';
        return;
    }
    for (const t of tags.slice(0, 30)) {
        const pill = el('a', {
            class: 'tag-pill',
            href: `#/search?tag=${encodeURIComponent(t.value)}`,
            'data-category': t.category,
        },
            t.value,
            el('span', { class: 'tag-count' }, String(t.count)),
        );
        cloud.appendChild(pill);
    }
}

/* ---- Project Cards ---- */
function renderProjectList(containerId, projects, showScore) {
    const container = document.getElementById(containerId);
    container.innerHTML = '';
    if (!projects.length) {
        container.innerHTML = '<div class="empty-state"><div class="empty-icon">&#128269;</div><p>No projects found</p></div>';
        return;
    }
    for (const p of projects) {
        container.appendChild(projectCard(p, showScore));
    }
}

function projectCard(project, showScore) {
    const p = project.project || project;
    const score = project.score;

    const tags = [];
    if (p.ai_tags) {
        for (const t of p.ai_tags.slice(0, 4)) {
            tags.push(el('span', { class: 'card-tag' }, t.value));
        }
    }
    if (p.frameworks) {
        for (const fw of p.frameworks.slice(0, 2)) {
            tags.push(el('span', { class: 'card-tag' }, fw));
        }
    }

    const statusClass = 'status-badge status-' + (p.status || 'pending');

    const card = el('div', {
        class: 'project-card',
        onclick: () => { location.hash = '#/project/' + p.id; },
    },
        el('div', { class: 'card-header' },
            el('span', { class: 'card-name' }, p.name),
            el('span', { class: 'card-lang' }, p.primary_language || '?'),
        ),
        el('div', { class: 'card-summary' }, p.summary || 'No summary available'),
        el('div', { class: 'card-tags' }, ...tags),
    );

    if (showScore && score !== undefined) {
        card.appendChild(el('div', { class: 'card-score' }, 'Score: ' + score.toFixed(3)));
    }

    return card;
}

/* ---- Search ---- */
function initSearch() {
    const input = document.getElementById('search-input');
    const mode = document.getElementById('mode-select');
    const filters = ['filter-language', 'filter-framework', 'filter-pattern', 'filter-tag'];

    input.addEventListener('input', () => debounceSearch());
    mode.addEventListener('change', () => doSearch());
    for (const fId of filters) {
        document.getElementById(fId).addEventListener('change', () => doSearch());
    }

    loadFacets();
}

function debounceSearch() {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => doSearch(), 300);
}

function getSearchParams() {
    return {
        q: document.getElementById('search-input').value.trim(),
        mode: document.getElementById('mode-select').value,
        language: document.getElementById('filter-language').value,
        framework: document.getElementById('filter-framework').value,
        pattern: document.getElementById('filter-pattern').value,
        tag: document.getElementById('filter-tag').value,
    };
}

async function doSearch() {
    const params = getSearchParams();
    updateSearchHash(params);

    if (!params.q) {
        document.getElementById('search-results').innerHTML =
            '<div class="empty-state"><div class="empty-icon">&#128269;</div><p>Type a query to search</p></div>';
        document.getElementById('search-status').textContent = '';
        return;
    }

    document.getElementById('search-status').textContent = 'Searching...';

    try {
        const results = await API.search(params);
        document.getElementById('search-status').textContent =
            `${results.length} result(s) for "${params.q}"` +
            (params.language ? ` · language: ${params.language}` : '') +
            (params.framework ? ` · framework: ${params.framework}` : '') +
            (params.pattern ? ` · pattern: ${params.pattern}` : '') +
            (params.tag ? ` · tag: ${params.tag}` : '');

        const container = document.getElementById('search-results');
        container.innerHTML = '';
        if (!results.length) {
            container.innerHTML = '<div class="empty-state"><div class="empty-icon">&#128269;</div><p>No results found</p></div>';
            return;
        }
        for (const r of results) {
            container.appendChild(projectCard(r, true));
        }

        loadFacets(params);
    } catch (err) {
        document.getElementById('search-status').textContent = 'Search failed: ' + err.message;
    }
}

function updateSearchHash(params) {
    const p = new URLSearchParams();
    if (params.q) p.set('q', params.q);
    if (params.mode && params.mode !== 'hybrid') p.set('mode', params.mode);
    if (params.language) p.set('language', params.language);
    if (params.framework) p.set('framework', params.framework);
    if (params.pattern) p.set('pattern', params.pattern);
    if (params.tag) p.set('tag', params.tag);
    const s = p.toString();
    const newHash = '#/search' + (s ? '?' + s : '');
    if (location.hash !== newHash) {
        history.replaceState(null, '', newHash);
    }
}

function restoreSearchFromHash() {
    const hash = location.hash;
    const qIdx = hash.indexOf('?');
    if (qIdx === -1) {
        loadFacets();
        return;
    }
    const params = new URLSearchParams(hash.slice(qIdx + 1));
    document.getElementById('search-input').value = params.get('q') || '';
    document.getElementById('mode-select').value = params.get('mode') || 'hybrid';

    // Restore filters after facets are loaded
    loadFacets().then(() => {
        if (params.get('language')) document.getElementById('filter-language').value = params.get('language');
        if (params.get('framework')) document.getElementById('filter-framework').value = params.get('framework');
        if (params.get('pattern')) document.getElementById('filter-pattern').value = params.get('pattern');
        if (params.get('tag')) document.getElementById('filter-tag').value = params.get('tag');
        if (params.get('q')) doSearch();
    });
}

async function loadFacets(params) {
    try {
        const facets = await API.facets(params || {});
        populateFilter('filter-language', facets.languages, 'All Languages');
        populateFilter('filter-framework', facets.frameworks, 'All Frameworks');
        populateFilter('filter-pattern', facets.patterns, 'All Patterns');
        populateFilter('filter-tag', facets.tags, 'All Tags');
    } catch (err) { /* ignore */ }
}

function populateFilter(selectId, items, defaultLabel) {
    const select = document.getElementById(selectId);
    const current = select.value;
    select.innerHTML = '';
    select.appendChild(el('option', { value: '' }, defaultLabel));
    for (const [name, count] of Object.entries(items || {})) {
        select.appendChild(el('option', { value: name }, `${name} (${count})`));
    }
    if (current) select.value = current;
}

/* ---- Project Detail ---- */
async function loadProjectDetail(id) {
    const container = document.getElementById('project-detail');
    container.innerHTML = '<div class="empty-state">Loading...</div>';

    try {
        const p = await API.project(id);
        container.innerHTML = '';

        // Header
        const header = el('div', { class: 'detail-header' },
            el('h1', null, p.name),
            el('div', { class: 'detail-meta' },
                (p.primary_language || '') + ' · ' + (p.status || '') +
                (p.path ? ' · ' + p.path : '')
            ),
        );
        container.appendChild(header);

        // Summary
        if (p.summary) {
            container.appendChild(detailSection('Summary', el('div', { class: 'detail-summary' }, p.summary)));
        }

        // Languages
        if (p.languages && p.languages.length) {
            const ul = el('ul');
            for (const l of p.languages) {
                ul.appendChild(el('li', null, `${l.name}: ${l.files} files, ${l.lines} lines (${l.percentage}%)`));
            }
            container.appendChild(detailSection('Languages', ul));
        }

        // Frameworks
        if (p.frameworks && p.frameworks.length) {
            const ul = el('ul');
            for (const fw of p.frameworks) ul.appendChild(el('li', null, fw));
            container.appendChild(detailSection('Frameworks', ul));
        }

        // Dependencies
        if (p.dependencies && p.dependencies.length) {
            const ul = el('ul');
            for (const d of p.dependencies.slice(0, 25)) {
                ul.appendChild(el('li', null, d.name + (d.version ? ' (' + d.version + ')' : '') + (d.dev ? ' [dev]' : '')));
            }
            if (p.dependencies.length > 25) {
                ul.appendChild(el('li', null, `... and ${p.dependencies.length - 25} more`));
            }
            container.appendChild(detailSection(`Dependencies (${p.dependencies.length})`, ul));
        }

        // Patterns
        if (p.patterns && p.patterns.length) {
            const ul = el('ul');
            for (const pt of p.patterns) {
                ul.appendChild(el('li', null, `${pt.name} (${pt.category}) — ${Math.round(pt.confidence * 100)}%`));
            }
            container.appendChild(detailSection('Patterns', ul));
        }

        // Quality Metrics
        const q = p.quality;
        if (q) {
            const div = el('div');
            div.appendChild(detailRow('Total files', q.total_files));
            div.appendChild(detailRow('Total lines', q.total_lines));
            div.appendChild(detailRow('Avg file size', Math.round(q.avg_file_size) + ' lines'));
            const checks = [];
            if (q.has_tests) checks.push('tests');
            if (q.has_ci) checks.push('CI');
            if (q.has_docs) checks.push('docs');
            if (q.has_readme) checks.push('README');
            if (q.has_license) checks.push('license');
            if (checks.length) div.appendChild(detailRow('Has', checks.join(', ')));
            container.appendChild(detailSection('Quality Metrics', div));
        }

        // Git
        if (p.git && p.git.total_commits) {
            const div = el('div');
            if (p.git.default_branch) div.appendChild(detailRow('Branch', p.git.default_branch));
            div.appendChild(detailRow('Commits', p.git.total_commits));
            if (p.git.last_commit_date) div.appendChild(detailRow('Last commit', p.git.last_commit_date.slice(0, 10)));
            if (p.git.last_commit_message) div.appendChild(detailRow('Message', p.git.last_commit_message.slice(0, 80)));
            if (p.git.remote_url) div.appendChild(detailRow('Remote', p.git.remote_url));
            container.appendChild(detailSection('Git', div));
        }

        // Tags
        if (p.tags && p.tags.length) {
            const div = el('div', { class: 'tag-cloud' });
            for (const t of p.tags) {
                div.appendChild(el('span', { class: 'tag-pill' }, t));
            }
            container.appendChild(detailSection('Tags', div));
        }

        // AI Tags
        if (p.ai_tags && p.ai_tags.length) {
            const div = el('div');
            const byCategory = {};
            for (const t of p.ai_tags) {
                const cat = t.category || 'domain';
                if (!byCategory[cat]) byCategory[cat] = [];
                byCategory[cat].push(t);
            }
            for (const cat of Object.keys(byCategory).sort()) {
                const catDiv = el('div', { style: 'margin-bottom: 0.5rem;' },
                    el('strong', null, cat + ': '),
                );
                const tagDiv = el('span', { class: 'tag-cloud', style: 'display: inline-flex;' });
                for (const t of byCategory[cat]) {
                    tagDiv.appendChild(el('a', {
                        class: 'tag-pill',
                        href: `#/search?tag=${encodeURIComponent(t.value)}`,
                        'data-category': cat,
                    }, t.value));
                }
                catDiv.appendChild(tagDiv);
                div.appendChild(catDiv);
            }
            container.appendChild(detailSection('AI Tags', div));
        }

        // Concepts
        if (p.concepts && p.concepts.length) {
            const div = el('div', { class: 'tag-cloud' });
            for (const c of p.concepts) {
                div.appendChild(el('span', { class: 'tag-pill' }, c));
            }
            container.appendChild(detailSection('Concepts', div));
        }

    } catch (err) {
        container.innerHTML = '<div class="empty-state"><div class="empty-icon">!</div><p>Could not load project</p></div>';
    }
}

function detailSection(title, content) {
    const section = el('div', { class: 'detail-section' },
        el('h3', null, title),
    );
    section.appendChild(content);
    return section;
}

function detailRow(label, value) {
    return el('div', { class: 'detail-row' },
        el('span', { class: 'label' }, label),
        el('span', { class: 'value' }, String(value)),
    );
}

/* ---- Back Button ---- */
document.getElementById('back-btn').addEventListener('click', () => {
    history.back();
});

/* ---- Init ---- */
window.addEventListener('hashchange', route);
initSearch();
route();
