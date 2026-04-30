# labs-responsive-shell

A messaging app shell in Uno. Responsive, themed, motion-aware.

Live: https://yottaverseltd.github.io/labs-responsive-shell/

## What it is

A small single-project Uno Platform sample targeting WebAssembly and Skia Desktop. The shell reframes as a three-tab messaging app: a Home dashboard, a Chat surface that swaps between a two-pane rail and a single-pane drill-in at 900 px, and a grouped Settings page. All content is mock. No network, no telemetry, no auth. This ships as a starting point, not a tutorial.

## Run it

```
dotnet restore
dotnet build -c Release
dotnet run --project LabsResponsiveShell/LabsResponsiveShell.csproj -f net9.0-desktop
```

## Architecture notes

- Color, spacing, radius, and type live in `Styles/Tokens.xaml` with Dark (default) and Light theme dictionaries. No hardcoded brushes in pages.
- Motion duration and easing live in `Styles/Motion.xaml`. `Motion/ReducedMotion.cs` clamps durations to zero when the OS prefers reduced motion, via `prefers-reduced-motion` on WASM and `UISettings.AnimationsEnabled` on Skia.
- Shell rail collapses to a tab bar at 768 px. Chat swaps between two-pane and single-pane drill-in at 900 px. `AdaptiveTrigger` drives both transitions.
- `AppSettings` is the single observable source for theme, flow direction, reduced motion, sound preset, last-seen audience, and read receipts. Every surface that exposes these controls binds to the same instance.
- The only radial glow sits behind the Home hero. A 200 ms skeleton overlay covers tab switches. Reduced motion holds the overlay flat for 120 ms.
- `ToastRelay.Show(message)` routes into a single host mounted in `AppShell`, which slides a pill up from the bottom for two seconds.
- Warnings are errors. Nullable is enabled. Analyzer level is latest.

## License

MIT. See `LICENSE`.
