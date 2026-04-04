# Architecture Decision Records
*Dark Sky — /docs/DECISIONS.md*

---

## ADR-001 — CSS Custom Properties as the Token Implementation Format

**Date:** 2026-04-04  
**Status:** Accepted  
**Author:** Forge

### Context
The Brand System defines a three-layer token architecture (primitives → semantic → Night Mode). This needs a concrete implementation format that components reference at runtime.

### Options Considered
1. **CSS Custom Properties** — native browser support, no build step, runtime-swappable via `[data-theme]` attribute
2. **SCSS Variables** — compile-time only, no runtime swapping without a full stylesheet swap
3. **JSON Token File** — useful for cross-platform sync, but requires a build pipeline to transform to CSS; not directly consumable by components

### Decision
CSS Custom Properties (option 1).

### Rationale
- Night Mode requires runtime token swapping — SCSS variables are compile-time only, making this impossible without regenerating stylesheets
- No build dependency; tokens work in any context (web, Electron/Tauri, WebView in future native app)
- JSON export can be layered on top later for iOS/Android/Tauri cross-platform token sync — the CSS layer remains unchanged
- The `[data-theme]` attribute contract is simple and debuggable in browser devtools

### Consequences
- Token values are visible in browser devtools (acceptable — no secrets in tokens)
- SCSS/PostCSS build tools can still consume the CSS files without transformation
- If a design token tool (Style Dictionary, Tokens Studio) is adopted later, it will export to CSS custom properties — direct compatibility

---

## ADR-002 — Night Mode as a Separate Token Override File

**Date:** 2026-04-04  
**Status:** Accepted  
**Author:** Forge

### Context
Night Mode is not Dark Mode. It is a red-channel-only palette that layers on top of whichever theme is active. It needs to override only semantic colour tokens — not typography, spacing, or motion tokens.

### Options Considered
1. **Separate file** (`tokens.night.css`) with `[data-theme="night"]` overrides only
2. **Inline in** `tokens.semantic.css` — Night Mode section at the bottom
3. **JavaScript-driven** — set CSS custom property values directly via JS

### Decision
Separate file (option 1).

### Rationale
- Separation of concerns: the Night Mode file is the canonical place for Night Mode values, making it easy for Canvas to audit
- The import order in `index.css` makes the cascade relationship explicit and intentional
- Option 2 makes `tokens.semantic.css` harder to navigate as the token count grows
- Option 3 loses the declarative CSS advantage; harder to inspect and override in devtools

### Consequences
- Three CSS files to maintain instead of two — acceptable complexity trade-off
- Night Mode values are easily auditable and modifiable by Canvas without touching other layers

---

## ADR-003 — Theme State on `<html>`, Not `<body>`

**Date:** 2026-04-04  
**Status:** Accepted  
**Author:** Forge

### Context
The `[data-theme]` attribute must be set somewhere in the DOM for CSS selectors to cascade.

### Options Considered
1. `<html>` element (document root)
2. `<body>` element
3. A dedicated `<div id="app-root">` wrapper

### Decision
`<html>` element (option 1).

### Rationale
- CSS custom properties cascade from `<html>` into both `<head>` and `<body>` content, including any dynamically injected stylesheets
- On initial page load, reading localStorage and setting the attribute on `<html>` before first paint prevents Flash of Unstyled Content (FOUC) — particularly important for a dark-first app to avoid a white flash
- Matches the pattern used by Tailwind, shadcn/ui, and other reference implementations — well-understood convention
- Option 3 creates a specificity edge case if any global styles target `:root`

### Consequences
- `ThemeManager.init()` must be called as early as possible in the app lifecycle, before any rendering
- In SSR contexts (if ever adopted), the theme attribute must be set on the server-side `<html>` using the stored preference cookie/header to avoid FOUC

---

## ADR-004 — Phosphor Icons via npm Package

**Date:** 2026-04-04  
**Status:** Accepted  
**Author:** Forge

### Context
The Brand System specifies Phosphor Icons. The integration approach affects how Canvas writes icon references in component contracts.

### Options Considered
1. **npm package** (`@phosphor-icons/web`) with tree-shaking
2. **SVG sprite** — subset exported from Phosphor, bundled as a single file
3. **Web component** — Phosphor's `<ph-icon>` web component

### Decision
npm package with tree-shaking (option 1).

### Rationale
- Sprite maintenance is manual — every new icon Canvas uses requires a sprite rebuild and re-export. This creates friction in the Canvas → Forge pipeline
- Web component has Flash of Unstyled Icons (FOUI) risk in frameworks with SSR; also adds a custom element upgrade step
- npm package tree-shakes to only the icons actually used — bundle size grows only when new icons are added to components
- Standard import syntax works naturally with any bundler (Vite, webpack, Rollup)

### Consequences
- Canvas writes icon references using Phosphor icon names from phosphoricons.com
- Icon weight is specified per-use (Regular, Bold, Fill) per the Brand System convention
- No manual sprite maintenance required

### Open Question
Canvas to confirm: is the Regular/Bold/Fill weight convention complete, or are there contexts that need Duotone weight?

---

## ADR-005 — `N` Keyboard Shortcut with Input Focus Guard

**Date:** 2026-04-04  
**Status:** Accepted  
**Author:** Forge

### Context
Night Mode is toggled via the `N` keyboard shortcut. The app contains text inputs (sequence names, profile names, filter names). Without a guard, pressing `N` while naming a sequence would accidentally toggle Night Mode.

### Decision
The `N` shortcut is intercepted at `document` level with a guard that checks `document.activeElement`. If the focused element is a text-like input (`<input type="text/search/email/url/password/tel">`, `<textarea>`, or `contenteditable`), the event is ignored.

Number and range inputs are explicitly allowed through — a user adjusting a numeric value should still be able to toggle Night Mode with `N`.

### Consequences
- The guard must be kept in sync if new input types are introduced
- This is implemented in `ThemeManager.init()` in `src/theme.ts`

---
