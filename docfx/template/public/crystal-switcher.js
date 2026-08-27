(function () {
  const versionSelect = document.getElementById('crystal-version');
  const langSelect = document.getElementById('crystal-lang');
  if (!versionSelect || !langSelect) return;

  const rel = document.querySelector('meta[name="docfx:rel"]')?.content ?? '';
  const path = window.location.pathname;

  /** @type {{ default: string, defaultPage: string, versions: { id: string, label: string }[], pages: string[], pagesByVersion?: Record<string, string[]> }} */
  let config = {
    default: 'v3.0',
    defaultPage: 'introduction',
    versions: [
      { id: 'v3.0', label: '3.0.0 (Current)' },
      { id: 'v2.0', label: '2.0.1 (Legacy)' },
      { id: 'v1.2', label: 'v1.2 (Legacy)' },
    ],
    pages: [
      'introduction',
      'getting-started',
      'architecture',
      'aot-compatibility',
      'upgrade',
      'upgrade-from-2.0',
      'upgrade-from-2.0.0',
      'upgrade-from-1.2',
      'tutorials/create-first-app',
      'tutorials/mvvm-pattern',
      'tutorials/navigation',
      'tutorials/module-development',
      'tutorials/dependency-injection',
      'tutorials/migrate-from-avalonia',
    ],
  };

  function loadConfig() {
    return fetch(rel + 'public/crystal-versions.json')
      .then((r) => (r.ok ? r.json() : config))
      .then((data) => {
        config = data;
        populateVersionSelect();
      })
      .catch(() => populateVersionSelect());
  }

  function populateVersionSelect() {
    versionSelect.replaceChildren();
    for (const v of config.versions) {
      const opt = document.createElement('option');
      opt.value = v.id;
      opt.textContent = v.label;
      versionSelect.appendChild(opt);
    }
  }

  function currentLang() {
    return path.includes('/zh-CN/') ? 'zh-CN' : 'en';
  }

  function currentVersion() {
    const match = path.match(/\/docs\/(v[\d.]+)\//);
    if (match) return match[1];
    return config.default;
  }

  function currentPage() {
    const match = path.match(/\/docs\/v[\d.]+\/(?:zh-CN\/)?(.+?)\.html/i);
    if (match && config.pages.includes(match[1])) return match[1];
    return config.defaultPage || 'introduction';
  }

  function resolvePage(version, page) {
    const allowed = config.pagesByVersion?.[version] ?? config.pages;
    if (allowed.includes(page)) return page;
    return config.defaultPage || 'introduction';
  }

  function buildDocUrl(version, lang, page) {
    const resolved = resolvePage(version, page);
    const prefix =
      lang === 'zh-CN' ? `docs/${version}/zh-CN/` : `docs/${version}/`;
    return rel + prefix + resolved + '.html';
  }

  function syncControls() {
    versionSelect.value = currentVersion();
    langSelect.value = currentLang();
  }

  loadConfig().then(() => {
    syncControls();

    versionSelect.addEventListener('change', () => {
      window.location.href = buildDocUrl(
        versionSelect.value,
        langSelect.value,
        currentPage()
      );
    });

    langSelect.addEventListener('change', () => {
      window.location.href = buildDocUrl(
        versionSelect.value,
        langSelect.value,
        currentPage()
      );
    });
  });
})();
