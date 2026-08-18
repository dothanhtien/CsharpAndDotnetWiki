const THEME_STORAGE_KEY = "cs-dotnet-wiki-theme";
const themeToggle = document.getElementById("theme-toggle");
const hljsLightLink = document.getElementById("hljs-light");
const hljsDarkLink = document.getElementById("hljs-dark");

function systemPrefersDark() {
  return window.matchMedia("(prefers-color-scheme: dark)").matches;
}

function effectiveTheme() {
  const stored = localStorage.getItem(THEME_STORAGE_KEY);
  return stored || (systemPrefersDark() ? "dark" : "light");
}

function applyTheme(theme) {
  document.documentElement.dataset.theme = theme;
  hljsLightLink.disabled = theme === "dark";
  hljsDarkLink.disabled = theme !== "dark";
  mermaid.initialize({
    startOnLoad: false,
    theme: theme === "dark" ? "dark" : "default",
  });
}

// Apply immediately (before content loads) to avoid a flash of the wrong theme.
applyTheme(effectiveTheme());

themeToggle.addEventListener("click", () => {
  const next = effectiveTheme() === "dark" ? "light" : "dark";
  localStorage.setItem(THEME_STORAGE_KEY, next);
  applyTheme(next);
  loadContent({ resetScroll: false });
});

window
  .matchMedia("(prefers-color-scheme: dark)")
  .addEventListener("change", () => {
    if (localStorage.getItem(THEME_STORAGE_KEY)) return;
    applyTheme(effectiveTheme());
    loadContent({ resetScroll: false });
  });

const menuBtn = document.getElementById("menu-btn");
const sidebar = document.getElementById("sidebar");
menuBtn.addEventListener("click", () => sidebar.classList.toggle("open"));
document.addEventListener("click", (e) => {
  if (
    window.innerWidth <= 900 &&
    sidebar.classList.contains("open") &&
    !sidebar.contains(e.target) &&
    e.target !== menuBtn
  ) {
    sidebar.classList.remove("open");
  }
});

const homeLink = document.getElementById("home-link");
const lectureNavEl = document.getElementById("lecture-nav");
homeLink.addEventListener("click", () => {
  if (window.innerWidth <= 900) sidebar.classList.remove("open");
});

function slugify(text) {
  return text
    .toString()
    .trim()
    .toLowerCase()
    .replace(/[^\p{L}\p{N}\s-]/gu, "")
    .replace(/\s+/g, "-");
}

const LANG_STORAGE_KEY = "cs-dotnet-wiki-lang";
const langToggle = document.getElementById("lang-toggle");
const tocTitle = document.getElementById("toc-title");
const UI_TEXT = {
  en: {
    toc: "Contents",
    loading: "Loading lecture content…",
    loadError: "Failed to load",
    mermaidError: "Mermaid diagram error: ",
    genericError: "Error loading content: ",
    copy: "Copy",
    copied: "Copied!",
    notFound: "Lecture not found.",
    home: "Home",
    homeTitle: "Lectures",
    homeIntro: "Pick a lecture below or from the sidebar to get started.",
  },
  vi: {
    toc: "Mục lục",
    loading: "Đang tải nội dung bài giảng…",
    loadError: "Không tải được",
    mermaidError: "Lỗi sơ đồ Mermaid: ",
    genericError: "Lỗi khi tải nội dung: ",
    copy: "Sao chép",
    copied: "Đã sao chép!",
    notFound: "Không tìm thấy bài giảng.",
    home: "Trang chủ",
    homeTitle: "Danh sách bài giảng",
    homeIntro: "Chọn một bài giảng bên dưới hoặc từ sidebar để bắt đầu.",
  },
};

const COPY_ICON =
  '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.4"><rect x="5.5" y="5.5" width="8.5" height="8.5" rx="1.5"/><path d="M2.5 10.5v-7A1.5 1.5 0 0 1 4 2h7"/></svg>';
const CHECK_ICON =
  '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.6"><path d="M3 8.5 6.2 12 13 4" stroke-linecap="round" stroke-linejoin="round"/></svg>';

function addCopyButtons() {
  const t = UI_TEXT[currentLang];
  document.querySelectorAll("#content pre > code").forEach((code) => {
    const pre = code.parentElement;
    if (
      pre.closest(".mermaid-mount") ||
      pre.parentElement.classList.contains("code-block-wrap")
    )
      return;

    const wrap = document.createElement("div");
    wrap.className = "code-block-wrap";
    pre.replaceWith(wrap);
    wrap.appendChild(pre);

    const btn = document.createElement("button");
    btn.type = "button";
    btn.className = "copy-btn";
    btn.innerHTML =
      COPY_ICON + '<span class="copy-btn-label">' + t.copy + "</span>";
    btn.addEventListener("click", async () => {
      try {
        await navigator.clipboard.writeText(code.innerText);
      } catch (err) {
        const ta = document.createElement("textarea");
        ta.value = code.innerText;
        ta.style.position = "fixed";
        ta.style.opacity = "0";
        document.body.appendChild(ta);
        ta.select();
        try {
          document.execCommand("copy");
        } catch (e2) {}
        ta.remove();
      }
      btn.classList.add("copied");
      btn.innerHTML =
        CHECK_ICON + '<span class="copy-btn-label">' + t.copied + "</span>";
      clearTimeout(btn._resetTimer);
      btn._resetTimer = setTimeout(() => {
        btn.classList.remove("copied");
        btn.innerHTML =
          COPY_ICON + '<span class="copy-btn-label">' + t.copy + "</span>";
      }, 1800);
    });
    wrap.appendChild(btn);
  });
}

let LECTURES = []; // loaded from lectures.json
let currentLectureId = null; // id of the lecture currently shown
let currentLang = localStorage.getItem(LANG_STORAGE_KEY) || "en";
let renderToken = 0;

function currentLecture() {
  return LECTURES.find((l) => l.id === currentLectureId) || null;
}

function setLangUI(lang) {
  langToggle.dataset.active = lang;
  tocTitle.textContent = UI_TEXT[lang].toc;
  document.documentElement.lang = lang;
}

function updateHomeLink() {
  homeLink.textContent = "🏠 " + UI_TEXT[currentLang].home;
  homeLink.classList.toggle("active", currentLectureId === null);
}

function renderLectureNav() {
  updateHomeLink();

  // The lecture list only shows on the home page — inside a lecture the
  // sidebar is just Home + that lecture's own Contents (TOC) below it.
  if (currentLectureId !== null) {
    lectureNavEl.innerHTML = "";
    return;
  }

  const frag = document.createDocumentFragment();
  LECTURES.forEach((lec) => {
    const a = document.createElement("a");
    a.href = "#" + lec.id;
    a.dataset.id = lec.id;
    a.textContent =
      currentLang === "vi" && lec.titleVi ? lec.titleVi : lec.title;
    if (lec.id === currentLectureId) a.classList.add("active");
    a.addEventListener("click", () => {
      if (window.innerWidth <= 900) sidebar.classList.remove("open");
    });
    frag.appendChild(a);
  });
  lectureNavEl.innerHTML = "";
  lectureNavEl.appendChild(frag);
}

function renderHome() {
  const t = UI_TEXT[currentLang];
  const items = LECTURES.map((lec) => {
    const title = currentLang === "vi" && lec.titleVi ? lec.titleVi : lec.title;
    const summary =
      currentLang === "vi" && lec.summaryVi ? lec.summaryVi : lec.summary || "";
    return `<li><a class="lecture-card" href="#${lec.id}">
      <span class="lecture-card-title">${title}</span>
      <p class="lecture-card-summary">${summary}</p>
    </a></li>`;
  }).join("");
  contentEl.innerHTML = `<div id="home-view">
    <h1>${t.homeTitle}</h1>
    <p class="home-intro">${t.homeIntro}</p>
    <ul class="lecture-cards">${items}</ul>
  </div>`;
  contentEl.classList.remove("content-loading");
  tocTitle.style.display = "none";
  document.getElementById("toc").innerHTML = "";
}

langToggle.addEventListener("click", () => {
  currentLang = currentLang === "vi" ? "en" : "vi";
  localStorage.setItem(LANG_STORAGE_KEY, currentLang);
  renderLectureNav();
  loadContent();
});

const contentEl = document.getElementById("content");
let isFirstLoad = true;

async function loadContent({ resetScroll = true } = {}) {
  const myToken = ++renderToken;
  const wasFirstLoad = isFirstLoad;
  const t = UI_TEXT[currentLang];
  setLangUI(currentLang);

  if (currentLectureId === null) {
    renderHome();
    isFirstLoad = false;
    if (!wasFirstLoad && resetScroll) window.scrollTo(0, 0);
    return;
  }

  const lec = currentLecture();
  if (!lec) {
    contentEl.innerHTML = '<p style="color:red">' + t.notFound + "</p>";
    tocTitle.style.display = "none";
    document.getElementById("toc").innerHTML = "";
    return;
  }

  if (isFirstLoad) {
    contentEl.innerHTML = '<div id="loading">' + t.loading + "</div>";
  } else {
    contentEl.classList.add("content-loading");
  }

  const enFile = lec.dir + "/README.md";
  const viFile = lec.dir + "/README.vi.md";
  let lang = currentLang;
  let file = lang === "vi" ? viFile : enFile;

  let res;
  try {
    res = await fetch(file + "?v=" + Date.now(), { cache: "no-store" });
  } catch (err) {
    if (myToken !== renderToken) return;
    contentEl.classList.remove("content-loading");
    contentEl.innerHTML =
      '<p style="color:red">' + t.genericError + err.message + "</p>";
    return;
  }
  if (myToken !== renderToken) return;

  if (!res.ok) {
    // Fall back to English (README.md) if the Vietnamese translation isn't published for this lecture.
    if (lang === "vi") {
      lang = "en";
      file = enFile;
      try {
        res = await fetch(file + "?v=" + Date.now(), {
          cache: "no-store",
        });
      } catch (err) {
        if (myToken !== renderToken) return;
        contentEl.classList.remove("content-loading");
        contentEl.innerHTML =
          '<p style="color:red">' + t.genericError + err.message + "</p>";
        return;
      }
      if (myToken !== renderToken) return;
    }
    if (!res.ok) {
      contentEl.classList.remove("content-loading");
      contentEl.innerHTML =
        '<p style="color:red">' +
        t.loadError +
        " " +
        file +
        " (status " +
        res.status +
        ")</p>";
      return;
    }
  }
  const md = await res.text();
  if (myToken !== renderToken) return;

  // Pull out ```mermaid fenced blocks before handing markdown to the parser.
  const mermaidBlocks = [];
  const mdWithPlaceholders = md.replace(
    /```mermaid\r?\n([\s\S]*?)```/g,
    (m, code) => {
      const idx = mermaidBlocks.length;
      mermaidBlocks.push(code.replace(/\n$/, ""));
      return `@@MERMAID_PLACEHOLDER_${idx}@@`;
    },
  );

  const renderer = new marked.Renderer();
  const slugCounts = {};
  renderer.heading = function (token) {
    const level = token.depth;
    const html = this.parser.parseInline(token.tokens);
    let base = slugify(token.text);
    slugCounts[base] = (slugCounts[base] || 0) + 1;
    const slug =
      slugCounts[base] > 1 ? `${base}-${slugCounts[base] - 1}` : base;
    return `<h${level} id="${slug}">${html}</h${level}>\n`;
  };
  // Resolve relative links/images against the lecture's own folder, so a
  // README can link to files (e.g. ./src/Program.cs, ./diagram.png) sitting
  // next to it without knowing the wiki's root-relative path.
  const baseHref = lec.dir + "/";
  renderer.link = function (token) {
    const href =
      /^([a-z]+:)?\/\//i.test(token.href) ||
      token.href.startsWith("#") ||
      token.href.startsWith("/")
        ? token.href
        : baseHref + token.href;
    const html = this.parser.parseInline(token.tokens);
    const title = token.title ? ` title="${token.title}"` : "";
    return `<a href="${href}"${title}>${html}</a>`;
  };
  renderer.image = function (token) {
    const src =
      /^([a-z]+:)?\/\//i.test(token.href) || token.href.startsWith("/")
        ? token.href
        : baseHref + token.href;
    const title = token.title ? ` title="${token.title}"` : "";
    return `<img src="${src}" alt="${token.text || ""}"${title}>`;
  };

  marked.setOptions({ renderer, breaks: false, gfm: true });

  let html = marked.parse(mdWithPlaceholders);

  const mermaidHtmlBlocks = [];
  for (let i = 0; i < mermaidBlocks.length; i++) {
    if (myToken !== renderToken) return;
    try {
      const { svg } = await mermaid.render(
        "mermaid-svg-" + myToken + "-" + i,
        mermaidBlocks[i],
      );
      mermaidHtmlBlocks.push(`<div class="mermaid-mount">${svg}</div>`);
    } catch (err) {
      mermaidHtmlBlocks.push(
        '<p style="color:red">' + t.mermaidError + err.message + "</p>",
      );
      console.error("Mermaid render error for block", i, err);
    }
  }
  if (myToken !== renderToken) return;

  html = html.replace(
    /<p>\s*@@MERMAID_PLACEHOLDER_(\d+)@@\s*<\/p>/g,
    (m, i) => mermaidHtmlBlocks[Number(i)],
  );
  html = html.replace(
    /@@MERMAID_PLACEHOLDER_(\d+)@@/g,
    (m, i) => mermaidHtmlBlocks[Number(i)],
  );

  contentEl.innerHTML = html;
  contentEl.classList.remove("content-loading");
  isFirstLoad = false;

  document.querySelectorAll("#content pre code").forEach((block) => {
    try {
      hljs.highlightElement(block);
    } catch (e) {}
  });

  addCopyButtons();

  tocTitle.style.display = "";
  const headings = document.querySelectorAll(
    "#content h1, #content h2, #content h3",
  );
  const tocEl = document.getElementById("toc");
  const frag = document.createDocumentFragment();
  headings.forEach((h) => {
    if (!h.id) return;
    const a = document.createElement("a");
    a.href = "#" + currentLectureId + "?h=" + h.id; // kept out of the router; smooth-scroll handled below
    a.textContent = h.textContent;
    a.className = "toc-" + h.tagName.toLowerCase();
    a.addEventListener("click", (e) => {
      e.preventDefault();
      h.scrollIntoView({ behavior: "smooth", block: "start" });
      if (window.innerWidth <= 900) sidebar.classList.remove("open");
    });
    frag.appendChild(a);
  });
  tocEl.innerHTML = "";
  tocEl.appendChild(frag);

  if (!wasFirstLoad && resetScroll) window.scrollTo(0, 0);
}

function idFromHash() {
  // Empty hash (or bare "#") means the start page - the list of lectures.
  const h = decodeURIComponent(location.hash.replace(/^#/, ""));
  return h || null;
}

function switchLecture(id) {
  // id === null routes to the start page; otherwise it must be a known lecture.
  if (id !== null && !LECTURES.some((l) => l.id === id)) return;
  currentLectureId = id;
  renderLectureNav();
  loadContent();
}

window.addEventListener("hashchange", () => {
  const id = idFromHash();
  if (id !== currentLectureId) switchLecture(id);
});

async function init() {
  try {
    const res = await fetch("lectures.json?v=" + Date.now(), {
      cache: "no-store",
    });
    if (!res.ok) throw new Error("status " + res.status);
    const allLectures = await res.json();
    // Entries flagged "hidden": true stay in lectures.json (e.g. to keep
    // config without deleting it) but are excluded from the rendered wiki.
    LECTURES = allLectures.filter((l) => !l.hidden);
  } catch (err) {
    contentEl.innerHTML =
      '<p style="color:red">' +
      UI_TEXT[currentLang].genericError +
      "lectures.json: " +
      err.message +
      "</p>";
    console.error(err);
    return;
  }

  // No hash (first visit, or user landed on the bare URL) -> start page
  // listing all lectures, instead of jumping straight into lecture #1.
  const hashId = idFromHash();
  currentLectureId =
    hashId && LECTURES.some((l) => l.id === hashId) ? hashId : null;

  setLangUI(currentLang);
  renderLectureNav();
  loadContent().catch((err) => {
    contentEl.innerHTML =
      '<p style="color:red">' +
      UI_TEXT[currentLang].genericError +
      err.message +
      "</p>";
    console.error(err);
  });
}

init();
