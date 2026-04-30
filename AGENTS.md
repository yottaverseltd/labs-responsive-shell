# AGENTS

Rules for humans and agents touching this repo.

## Prose

- No em-dashes. Use period, comma, colon, parens, or a line break.
- No AI-smell phrases: "Let's", "We will", "Here we", "Simply", "Essentially", "Imagine", "In essence", "At its core", "This function", "Now we".
- No tutorial tone. Senior voice. Short paragraphs.
- No emojis.

## Code

- Nullable enabled. Warnings-as-errors. Analyzer level latest.
- Comments explain why, not what. Only add a `// why` note for non-obvious invariants, constraints, or trade-offs.
- Colors resolve through tokens in `Styles/Tokens.xaml`. Page XAML never hardcodes a brush.
- Motion durations resolve through resources in `Styles/Motion.xaml`. Animations clamp to zero when reduced motion is on.
- File-scoped namespaces. `sealed` by default for leaf classes.

## Commits

- Imperative, lowercase subject.
- No trailing period on the subject.
- Body optional, wrap at 72.

## Forbidden

- Telemetry, analytics, third-party trackers.
- Network calls, auth, SignalR. Future labs carry those.
- New files outside the documented layout.
- Brush, color, or duration literals in page XAML.
