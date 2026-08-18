# Deploying to GitHub Pages

This wiki is a static site (plain HTML/CSS/JS, no build step), deployed via GitHub Actions using the workflow at [.github/workflows/deploy-pages.yml](.github/workflows/deploy-pages.yml).

## How the deployment works

On every push to `main` (or a manual run), the workflow:

1. Checks out the repo.
2. Runs `actions/configure-pages` to prepare the Pages environment.
3. Uploads the entire repo root (`path: '.'`) as the Pages artifact via `actions/upload-pages-artifact`.
4. Publishes it with `actions/deploy-pages`.

No build/compile step runs - the uploaded artifact is the repo as-is. Only the static files (`index.html`, `styles.css`, `app.js`, `lectures.json`, the `README.md`/`README.vi.md` files under `lectures/`) are ever fetched by the site at runtime; the `.NET` sample projects under `lectures/*/src/` are included in the upload but are never built - they exist for readers to clone and run locally with `dotnet run`.

`.nojekyll` is present at the repo root so GitHub Pages serves files as-is instead of running its default Jekyll processing (which would otherwise ignore folders/files starting with `_` and can mangle Markdown).

## One-time setup

1. **Push this repo to GitHub** (create the repo first if it doesn't exist yet):

   ```bash
   git init
   git add .
   git commit -m "Initial commit"
   git branch -M main
   git remote add origin https://github.com/<username>/<repo>.git
   git push -u origin main
   ```

2. **Enable GitHub Actions as the Pages source:**
   - Go to the repo on GitHub → **Settings → Pages**.
   - Under **Build and deployment → Source**, select **GitHub Actions**.

3. **Trigger the deploy:**
   - Push to `main` (the workflow runs automatically), or
   - Go to **Actions → Deploy wiki to GitHub Pages → Run workflow** to trigger it manually (the workflow also listens for `workflow_dispatch`).

4. **Get the URL:**
   - Once the workflow finishes, the site is live at `https://<username>.github.io/<repo>/`.
   - The exact URL is also printed in the workflow run summary and shown under **Settings → Pages**.

## Updating the site

Every push to `main` redeploys automatically - there's nothing else to run. Typical update flow when adding a new lecture:

1. Add the lecture folder under `lectures/` (see the root [README.md](README.md#how-to-add-a-new-lecture) for the folder layout).
2. Add the matching entry to [lectures.json](lectures.json).
3. Commit and push to `main`.
4. Watch the **Actions** tab for the deploy to finish, then check the live URL.

## Verifying before you push

Since there's no build step to catch mistakes, check locally first:

```bash
python3 -m http.server 8000
# open http://localhost:8000/ and click through the lectures
```

`fetch()` of `lectures.json` and the README files requires an actual HTTP server - opening `index.html` directly via `file://` won't load lecture content.

## Troubleshooting

- **404 on the Pages URL** - confirm **Settings → Pages → Source** is set to **GitHub Actions**, not "Deploy from a branch". If it was just changed, push again (or re-run the workflow) to trigger a fresh deploy.
- **Blank sidebar / lecture list, or content fails to load** - open the browser console. A failed fetch of `lectures.json` or a lecture's `README.md`/`README.vi.md` usually means a typo in `dir`/`id` in `lectures.json`, or a path mismatch with the actual folder name.
- **Workflow fails on `actions/deploy-pages`** - check that the repo's Pages feature is actually enabled (step 2 above); this action fails if the environment hasn't been set up yet.
- **Changes pushed but the live site still shows old content** - check the **Actions** tab to confirm the workflow run succeeded; also try a hard refresh, since `index.html` fetches lecture content with a cache-busting query string but the HTML/JS itself can still be cached by the browser or a CDN in front of Pages.
