# C# & .NET Wiki

A wiki collecting C# / .NET lectures. Each lecture is its own folder under [lectures/](lectures/), containing:

- `README.md` - the lecture content in **English** (Markdown, supports code blocks, tables, blockquotes, Mermaid diagrams) - this is the default, required file.
- `README.vi.md` - the **Vietnamese** translation (optional; if missing, the wiki falls back to `README.md`).
- `src/` - the accompanying sample project (console app or ASP.NET Core Web API).

The [index.html](index.html) page reads `lectures.json` to build the lecture list in the sidebar, then fetches and renders the selected lecture's `README.md` (Markdown → HTML, syntax highlighting, Mermaid, code copy button, VI/EN toggle, light/dark theme).

## How to add a new lecture

1. Copy one of the two sample folders:
   - [lectures/00-console-app-template](lectures/00-console-app-template) - console app project.
   - [lectures/00-web-api-template](lectures/00-web-api-template) - ASP.NET Core Web API project.
2. Rename the folder following the `NN-lecture-slug` convention (two-digit number + slug).
3. Write the lecture content in the new folder's `README.md` (English, default) and `README.vi.md` if you want a Vietnamese version.
4. Update the sample code in `src/`.
5. Add a matching entry to [lectures.json](lectures.json) at the repo root:

   ```json
   {
     "id": "02-lecture-slug",
     "dir": "lectures/02-lecture-slug",
     "title": "02. Lecture title",
     "titleVi": "02. Tên bài giảng",
     "summary": "Short English summary.",
     "summaryVi": "Mô tả ngắn tiếng Việt."
   }
   ```

   `id`/`dir` must match the actual folder name. `titleVi`/`summaryVi` can be omitted if not yet translated (the sidebar/home page will fall back to the English `title`/`summary`).

6. Open `index.html` in a browser (or run `python3 -m http.server` at the repo root) to check it.

## Running a lecture's sample project

```bash
cd lectures/00-console-app-template/src
dotnet run
```

```bash
cd lectures/00-web-api-template/src
dotnet run
```

## Deploying to GitHub Pages

The repo already has a workflow at [.github/workflows/deploy-pages.yml](.github/workflows/deploy-pages.yml) that auto-deploys on every push to `main`. See [DEPLOYMENT.md](DEPLOYMENT.md) for the full setup and troubleshooting guide; short version:

1. Push this repo to GitHub.
2. Go to the repo's **Settings → Pages**, and set **Source: GitHub Actions**.
3. Push (or re-run the workflow) - the site will be available at `https://<username>.github.io/<repo>/`.

The site is plain static HTML/JS (no build step), so only the Markdown files and `lectures.json` are read directly by the wiki; the projects in `src/` are only for reading/reference and running locally with `dotnet run` - they are never built or deployed to Pages.

## Directory structure

```
CsharpAndDotnetWiki/
├── index.html                            # Wiki page markup (shell)
├── styles.css                            # All styling
├── app.js                                # All render/routing logic
├── lectures.json                         # Lecture list (id, dir, title, titleVi, summary, summaryVi)
├── lectures/
│   ├── 00-console-app-template/
│   │   ├── README.md
│   │   └── src/
│   └── 00-web-api-template/
│       ├── README.md
│       └── src/
├── .github/workflows/deploy-pages.yml
├── .nojekyll
└── .gitignore
```
