# labs-responsive-shell

A responsive Uno Platform shell. Clone, run, ship.

## What it is

A small single-project Uno Platform sample targeting WebAssembly and Skia Desktop. The shell adapts at 768 px between a vertical nav rail and a bottom tab bar. Three toggles wire a token-driven theme, a flow-direction flip, and an OS-aware reduced-motion mode. No network, no telemetry, no auth. This ships as a starting point, not a tutorial.

## Run it

```
dotnet restore
dotnet build -c Release
dotnet run --project LabsResponsiveShell/LabsResponsiveShell.csproj -f net9.0-desktop
```

## Architecture notes

- Color, spacing, radius, and type live in `Styles/Tokens.xaml` with Dark (default) and Light theme dictionaries. No hardcoded brushes in pages.
- Motion duration and easing live in `Styles/Motion.xaml`. `Motion/ReducedMotion.cs` clamps durations to zero when the OS prefers reduced motion, via `prefers-reduced-motion` on WASM and `UISettings.AnimationsEnabled` on Skia.
- Breakpoint is 768 px. `AdaptiveTrigger` swaps the shell between rail and tab bar. Chat swaps between two-pane and single-pane with forward navigation.
- `AppSettings` is the single observable source for theme, direction, and motion. Every surface that exposes these controls binds to the same instance.
- Warnings are errors. Nullable is enabled. Analyzer level is latest.

## License

MIT. See `LICENSE`.
