# labs-responsive-shell

Mock messaging shell for Uno Platform: responsive layout, shared tokens, motion that respects reduced motion.

## Try it on GitHub Pages

**Live:** https://yottaverseltd.github.io/labs-responsive-shell/

**Prerequisites:** none for WASM. **Desktop zip** and **Android APK** appear on the **`continuous`** prerelease after a green **`ci`** run on **`main`**: https://github.com/yottaverseltd/labs-responsive-shell/releases/tag/continuous

**Troubleshooting:** If the live URL returns 404, open **Settings → Pages → Build and deployment** and set **Source** to **GitHub Actions**, then re-run the latest **deploy-pages** workflow.

**Desktop (Windows):** https://github.com/yottaverseltd/labs-responsive-shell/releases/download/continuous/labs-responsive-shell-net9.0-desktop.zip — direct download (`labs-responsive-shell-net9.0-desktop.zip`, refreshed on every successful `main` build). Release hub: https://github.com/yottaverseltd/labs-responsive-shell/releases/tag/continuous

**Android:** https://github.com/yottaverseltd/labs-responsive-shell/releases/download/continuous/labs-responsive-shell-net9.0-android.apk — direct download (`labs-responsive-shell-net9.0-android.apk`). Sideload only; Android may show an “unknown publisher” warning because the CI build uses an ephemeral debug-style signing key.

**Source:** https://github.com/yottaverseltd/labs-responsive-shell

Version-tagged [Releases](https://github.com/yottaverseltd/labs-responsive-shell/releases) (`v*`) still get desktop zips from [`release-desktop.yml`](.github/workflows/release-desktop.yml) if you prefer a fixed tag.

## What it is

A small single-project sample targeting **WebAssembly**, **Skia desktop**, and **Android**. Three tabs: Home, adaptive Chat (two-pane vs drill-in at 900 px), grouped Settings. Mock content only: no network, telemetry, or auth.

## Run it

```powershell
dotnet workload install wasm-tools android   # once per machine
dotnet restore
dotnet build -c Release
dotnet run --project LabsResponsiveShell/LabsResponsiveShell.csproj -f net9.0-desktop
```

## Architecture notes

- Color, spacing, radius, and type live in `Styles/Tokens.xaml` with Dark (default) and Light theme dictionaries. No hardcoded brushes in pages.
- Motion duration and easing live in `Styles/Motion.xaml`. `Motion/ReducedMotion.cs` clamps durations when the OS prefers reduced motion (`prefers-reduced-motion` on WASM, `UISettings.AnimationsEnabled` on Skia).
- Shell rail collapses to a tab bar at 768 px. Chat swaps between two-pane and single-pane drill-in at 900 px. `AdaptiveTrigger` drives both transitions.
- `AppSettings` is the single observable source for theme, flow direction, reduced motion, sound preset, last-seen audience, and read receipts.
- Warnings are errors. Nullable is enabled. Analyzer level is latest.

## CI

- [`ci.yml`](.github/workflows/ci.yml) — On PRs and non-`main` branches: **wasm** + **desktop** only (fast). On **`main`** (push or manual **workflow dispatch**): adds **Android** publish, then upserts the **`continuous`** prerelease asset URLs above and uploads **desktop-windows** / **android-apk** artifacts.
- [`deploy-pages.yml`](.github/workflows/deploy-pages.yml) — WASM to GitHub Pages.
- [`release-desktop.yml`](.github/workflows/release-desktop.yml) — Optional desktop zip when you push a `v*` tag.

## License

MIT. See `LICENSE`.
