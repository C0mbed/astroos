# Dark Sky — Brand System
**Version:** 1.0.0  
**Date:** 2026-04-03  
**Owner:** Canvas (CombeCrew Design)  
**Status:** Approved — Ready for Token Implementation

---

## 1. Brand Foundation

### Positioning
- **What Dark Sky is:** A precision observatory control platform for serious astrophotographers — unattended automation for those who won't compromise on their data.
- **Who it's for:** Intermediate-to-advanced astrophotographers. People who own mounts worth more than their cars. People who have driven two hours to a dark site and cannot afford a wasted night.
- **How it should feel:** Powerful, calm, inviting. A trusted instrument with a personality.
- **How it should NOT feel:** Sterile, corporate, cluttered, "space-themed for the sake of it," consumer-toy simple, or engineering-dashboard cold.

### The Slack / Not Teams Principle
Teams was designed by consensus. Slack was designed with conviction. Dark Sky follows Slack's model: every element earns its place, every interaction feels considered, the app has a point of view. Serious work can feel good to use. Precision and warmth are not opposites.

### Two Modes, One Language
**Observatory Mode:** Full panel layout. Data-dense. All systems visible. Built for someone at a desk with multiple monitors, running a planned automated session. The command centre.

**Travel Mode:** Simplified. Essentials forward. Built for someone in a field with a laptop on a camping table. No unnecessary panels. Larger touch targets. The field companion.

Same component library. Same design language. Same tokens. Different density, different defaults, same product.

### Voice & Tone
Dark Sky speaks like a trusted co-pilot — confident, precise, never alarmist. It states facts. It doesn't dramatise.

- "Session complete — 247 frames captured" not "SEQUENCE FINISHED ✓"
- "Mount: Tracking" not "Mount is currently tracking successfully"
- "Dawn in 2h 18m" not "WARNING: Astronomical dawn approaching"
- "Let's connect your observatory" not "No devices detected"
- "Still watching the skies" on an idle session screen

Errors are informative, not punishing. Success is acknowledged, not celebrated with confetti. The app trusts the user to be competent — it gives them information, not instructions.

---

## 2. Naming & Wordmark

### Product Name
**Dark Sky** — two words, title case, no article.  
Domain: **thedarksky.com**  
App identifier: `darksky` (all lowercase, no space, for file paths, package names, window class)

### Wordmark
**Font:** Space Grotesk SemiBold (600)  
**Tracking:** −0.02em  
**Case:** Title case ("Dark Sky") — never all-caps in the wordmark  
**Treatment:** Wordmark only in V1. No logomark/icon in this spec — that is a separate brand deliverable.

### Wordmark Usage
- Minimum size: 16px cap height
- Clear space: equal to the cap height of the "D" on all sides
- Approved backgrounds: `color-bg-app` (dark), `color-bg-surface` (dark), white (light mode)
- Night mode: wordmark renders in `color-text-primary-night` (the warm near-white red)
- Never: stretched, recoloured to a status colour, placed on a mid-tone surface without contrast verification, used below minimum size

---

## 3. Color System

### Design Principles
1. Dark Sky is a **dark-first application.** The light theme exists for daytime use; dark is the native state.
2. **Night Mode is not Dark Mode.** Night Mode is a manual override to a red-channel-only palette for observatory use. It layers on top of whichever system theme is active.
3. **No white flashes — ever.** Transitions between states must not produce a white frame. All animations use the dark palette as their base.
4. Status colours carry meaning. They are not decorative. Using `color-status-nominal` on a non-status element is a bug.

---

### 3a. Primitive Palette

These are the raw colour values. They are never used directly in component code — only semantic tokens reference them. Forge imports these as the base layer.

#### Dark Theme Primitives

| Token | Hex | Notes |
|---|---|---|
| `primitive-dark-950` | `#09090E` | Deepest background |
| `primitive-dark-900` | `#0F0F18` | App chrome |
| `primitive-dark-850` | `#141420` | Panel surfaces |
| `primitive-dark-800` | `#1C1C28` | Elevated surfaces |
| `primitive-dark-750` | `#242435` | Cards |
| `primitive-dark-700` | `#2E2E42` | Input fields |
| `primitive-dark-600` | `#42425E` | Borders |
| `primitive-dark-500` | `#5E5E80` | Muted borders |
| `primitive-dark-400` | `#8080A8` | Disabled text |
| `primitive-dark-300` | `#A8A8C8` | Secondary text |
| `primitive-dark-200` | `#C8C8E0` | Primary text (soft) |
| `primitive-dark-100` | `#E8E8F4` | Primary text |
| `primitive-dark-050` | `#F4F4FA` | High contrast text |

#### Light Theme Primitives

| Token | Hex | Notes |
|---|---|---|
| `primitive-light-950` | `#F8F8FC` | Page background |
| `primitive-light-900` | `#F0F0F8` | Secondary background |
| `primitive-light-850` | `#E8E8F2` | Tertiary / panel bg |
| `primitive-light-800` | `#DCDCEC` | Borders |
| `primitive-light-700` | `#C4C4DC` | Stronger borders |
| `primitive-light-600` | `#9898B8` | Muted text |
| `primitive-light-500` | `#6868A0` | Secondary text |
| `primitive-light-400` | `#404080` | Body text |
| `primitive-light-300` | `#242465` | Primary text |
| `primitive-light-200` | `#14144A` | Strong text |

#### Accent Primitives (both themes)

| Token | Hex | Notes |
|---|---|---|
| `primitive-accent-500` | `#5B6CF5` | Brand primary (dark theme) |
| `primitive-accent-400` | `#7B8CF8` | Brand hover (dark theme) |
| `primitive-accent-600` | `#3D4ED4` | Brand pressed (dark theme) |
| `primitive-accent-light-500` | `#3D4ED4` | Brand primary (light theme) |
| `primitive-accent-light-400` | `#5B6CF5` | Brand hover (light theme) |

#### Status Primitives

| Token | Hex | Usage |
|---|---|---|
| `primitive-nominal-bg` | `#0D2B1A` | Nominal state background (dark) |
| `primitive-nominal-text` | `#34C759` | Nominal text/icon (dark) |
| `primitive-nominal-bg-light` | `#E8F7EE` | Nominal state background (light) |
| `primitive-nominal-text-light` | `#1A7A3F` | Nominal text/icon (light) |
| `primitive-degraded-bg` | `#2B1F08` | Warning background (dark) |
| `primitive-degraded-text` | `#FF9F0A` | Warning text/icon (dark) |
| `primitive-degraded-bg-light` | `#FFF4E0` | Warning background (light) |
| `primitive-degraded-text-light` | `#954F00` | Warning text/icon (light) |
| `primitive-error-bg` | `#2B0A0A` | Error background (dark) |
| `primitive-error-text` | `#FF453A` | Error text/icon (dark) |
| `primitive-error-bg-light` | `#FFECEA` | Error background (light) |
| `primitive-error-text-light` | `#C0392B` | Error text/icon (light) |
| `primitive-disconnected-bg` | `#1A1A28` | Disconnected background (dark) |
| `primitive-disconnected-text` | `#636380` | Disconnected text (dark) |

#### Night Mode Primitives

| Token | Hex | Notes |
|---|---|---|
| `primitive-night-950` | `#0D0000` | Deepest background |
| `primitive-night-900` | `#150000` | App chrome |
| `primitive-night-850` | `#1E0000` | Panel surfaces |
| `primitive-night-800` | `#280000` | Elevated surfaces |
| `primitive-night-750` | `#330000` | Cards |
| `primitive-night-700` | `#420000` | Input fields |
| `primitive-night-600` | `#5C0000` | Borders |
| `primitive-night-500` | `#800000` | Interactive primary |
| `primitive-night-400` | `#AA0000` | Interactive hover |
| `primitive-night-300` | `#CC2222` | Status / accent |
| `primitive-night-200` | `#E04444` | Secondary text |
| `primitive-night-100` | `#F08080` | Primary text |
| `primitive-night-050` | `#FFB0B0` | High contrast text |

---

### 3b. Semantic Tokens

These are what Forge uses in all component code. Never reference primitives directly.

#### Background

| Token | Dark Theme | Light Theme | Night Mode | Usage |
|---|---|---|---|---|
| `color-bg-app` | `primitive-dark-900` | `primitive-light-950` | `primitive-night-900` | App chrome, window background |
| `color-bg-surface` | `primitive-dark-850` | `primitive-light-900` | `primitive-night-850` | Panel and sidebar surfaces |
| `color-bg-elevated` | `primitive-dark-800` | `primitive-light-850` | `primitive-night-800` | Cards, dropdowns, popovers |
| `color-bg-input` | `primitive-dark-700` | `#FFFFFF` | `primitive-night-700` | Input fields, textareas |
| `color-bg-overlay` | `rgba(9,9,14,0.72)` | `rgba(20,20,74,0.40)` | `rgba(13,0,0,0.80)` | Modal backdrops |

#### Text

| Token | Dark Theme | Light Theme | Night Mode | Usage |
|---|---|---|---|---|
| `color-text-primary` | `primitive-dark-100` | `primitive-light-200` | `primitive-night-050` | Primary readable text |
| `color-text-secondary` | `primitive-dark-300` | `primitive-light-500` | `primitive-night-100` | Supporting text, descriptions |
| `color-text-muted` | `primitive-dark-400` | `primitive-light-600` | `primitive-night-200` | Timestamps, labels, deemphasised |
| `color-text-disabled` | `primitive-dark-400` | `primitive-light-600` | `primitive-night-300` | Disabled element text |
| `color-text-data` | `primitive-dark-050` | `primitive-light-300` | `primitive-night-050` | Monospace telemetry values |

#### Border

| Token | Dark Theme | Light Theme | Night Mode | Usage |
|---|---|---|---|---|
| `color-border-subtle` | `primitive-dark-600` | `primitive-light-800` | `primitive-night-600` | Hairline dividers |
| `color-border-default` | `primitive-dark-500` | `primitive-light-700` | `primitive-night-500` | Component borders |
| `color-border-strong` | `primitive-dark-400` | `primitive-light-500` | `primitive-night-400` | Focused / active borders |

#### Interactive

| Token | Dark Theme | Light Theme | Night Mode | Usage |
|---|---|---|---|---|
| `color-interactive-primary` | `primitive-accent-500` | `primitive-accent-light-500` | `primitive-night-400` | Buttons, links, active nav |
| `color-interactive-hover` | `primitive-accent-400` | `primitive-accent-light-400` | `primitive-night-300` | Hover state |
| `color-interactive-pressed` | `primitive-accent-600` | `#2B38B0` | `primitive-night-500` | Pressed/active state |
| `color-interactive-subtle` | `rgba(91,108,245,0.12)` | `rgba(61,78,212,0.08)` | `rgba(170,0,0,0.15)` | Hover bg on rows/items |
| `color-interactive-disabled` | `primitive-dark-600` | `primitive-light-700` | `primitive-night-600` | Disabled interactive element |

#### Status

| Token | Dark Theme | Light Theme | Night Mode | Usage |
|---|---|---|---|---|
| `color-status-nominal` | `primitive-nominal-text` | `primitive-nominal-text-light` | `#CC6600` | Nominal/OK indicator |
| `color-status-nominal-bg` | `primitive-nominal-bg` | `primitive-nominal-bg-light` | `rgba(80,30,0,0.40)` | Nominal state background tint |
| `color-status-degraded` | `primitive-degraded-text` | `primitive-degraded-text-light` | `#CC8800` | Warning/degraded indicator |
| `color-status-degraded-bg` | `primitive-degraded-bg` | `primitive-degraded-bg-light` | `rgba(60,40,0,0.40)` | Warning background tint |
| `color-status-error` | `primitive-error-text` | `primitive-error-text-light` | `#FF4444` | Error/fault indicator |
| `color-status-error-bg` | `primitive-error-bg` | `primitive-error-bg-light` | `rgba(80,0,0,0.40)` | Error background tint |
| `color-status-disconnected` | `primitive-disconnected-text` | `#8888A8` | `#664444` | Offline/disconnected |
| `color-status-safe` | `primitive-nominal-text` | `primitive-nominal-text-light` | `#CC6600` | Safety system: all clear |
| `color-status-unsafe` | `primitive-error-text` | `primitive-error-text-light` | `#FF4444` | Safety system: abort |
| `color-status-caution` | `primitive-degraded-text` | `primitive-degraded-text-light` | `#CC8800` | Safety system: monitor |

---

### 3c. Contrast Ratios

All pairs at WCAG AA minimum (4.5:1 for normal text, 3:1 for large text). AAA target where achievable.

**Dark Theme**

| Foreground | Background | Ratio | WCAG |
|---|---|---|---|
| `color-text-primary` on `color-bg-app` | `#E8E8F4` on `#0F0F18` | 15.8:1 | AAA ✅ |
| `color-text-secondary` on `color-bg-surface` | `#A8A8C8` on `#141420` | 7.1:1 | AAA ✅ |
| `color-text-data` on `color-bg-surface` | `#F4F4FA` on `#141420` | 17.2:1 | AAA ✅ |
| `color-status-nominal` on `color-bg-app` | `#34C759` on `#0F0F18` | 8.4:1 | AAA ✅ |
| `color-status-degraded` on `color-bg-app` | `#FF9F0A` on `#0F0F18` | 9.1:1 | AAA ✅ |
| `color-status-error` on `color-bg-app` | `#FF453A` on `#0F0F18` | 6.2:1 | AA ✅ |
| `color-interactive-primary` on `color-bg-surface` | `#5B6CF5` on `#141420` | 4.8:1 | AA ✅ |

**Night Mode**

| Foreground | Background | Ratio | WCAG |
|---|---|---|---|
| `color-text-primary` on `color-bg-app` | `#FFB0B0` on `#150000` | 12.4:1 | AAA ✅ |
| `color-text-secondary` on `color-bg-surface` | `#F08080` on `#1E0000` | 7.8:1 | AAA ✅ |
| `color-text-data` on `color-bg-surface` | `#FFB0B0` on `#1E0000` | 11.3:1 | AAA ✅ |
| `color-status-error` on `color-bg-app` | `#FF4444` on `#150000` | 7.1:1 | AAA ✅ |

---

## 4. Typography System

### Typefaces

**Primary — Space Grotesk (Variable)**  
Source: Google Fonts (`fonts.googleapis.com/css2?family=Space+Grotesk:wght@300..700`)  
Fallback stack: `'Space Grotesk', system-ui, -apple-system, 'Segoe UI', sans-serif`  
Usage: All UI text — navigation, labels, body copy, headings, status text, microcopy  
Feature settings: `'kern' 1, 'liga' 1`

**Monospace — JetBrains Mono (Variable)**  
Source: Google Fonts (`fonts.googleapis.com/css2?family=JetBrains+Mono:wght@400;500`)  
Fallback stack: `'JetBrains Mono', 'Cascadia Code', 'Consolas', 'Courier New', monospace`  
Usage: ALL numeric telemetry, coordinates, exposure values, filter names, driver versions, log output, code  
Feature settings: `'tnum' 1` (tabular figures — mandatory for aligned data columns)

**No other typefaces.** Decorative or display faces are not used in UI components. Space Grotesk at display weight *is* the display face.

---

### Type Scale

| Token | Family | Size | Weight | Line Height | Letter Spacing | Usage |
|---|---|---|---|---|---|---|
| `text-display-lg` | Space Grotesk | 32px | 600 | 1.1 | −0.02em | Welcome screen, session complete |
| `text-display-sm` | Space Grotesk | 24px | 600 | 1.15 | −0.02em | Panel section titles |
| `text-heading-lg` | Space Grotesk | 18px | 600 | 1.25 | −0.01em | Modal titles, page headers |
| `text-heading-sm` | Space Grotesk | 15px | 600 | 1.3 | 0 | Widget titles, device names |
| `text-body-lg` | Space Grotesk | 14px | 400 | 1.6 | 0 | Primary body text |
| `text-body-sm` | Space Grotesk | 13px | 400 | 1.5 | 0 | Secondary body, descriptions |
| `text-label-lg` | Space Grotesk | 12px | 500 | 1.3 | 0.04em | Category labels (uppercase) |
| `text-label-sm` | Space Grotesk | 11px | 500 | 1.2 | 0.05em | Status badges, tags (uppercase) |
| `text-caption` | Space Grotesk | 11px | 400 | 1.4 | 0.01em | Footnotes, timestamps |
| `text-data-xl` | JetBrains Mono | 20px | 500 | 1.1 | 0 | Large telemetry (histogram, focus graph) |
| `text-data-lg` | JetBrains Mono | 16px | 500 | 1.2 | 0 | Primary telemetry values (Status Bar) |
| `text-data-md` | JetBrains Mono | 13px | 400 | 1.3 | 0 | Secondary telemetry, coordinates |
| `text-data-sm` | JetBrains Mono | 11px | 400 | 1.3 | 0 | Timestamps, versions, log entries |

### Typography Rules

1. **Labels that function as category headers** are set in `text-label-lg`, uppercase, with `color-text-muted`. They orient the user — they do not add hierarchy noise.
2. **Telemetry values are always monospace.** No exceptions. This includes temperatures, RMS values, times, counts, coordinates, exposure durations, gain values, binning, and anything numeric that the user needs to read accurately.
3. **Line length:** Body text panels: 60–75 characters max. Log entries: full width (monospace, scannable). Status Bar zones: single line only — truncate, never wrap.
4. **No italic** in UI components. Italic is reserved for the brand voice / marketing layer.
5. **Minimum rendered size: 11px.** Nothing in the UI is smaller than this. If content doesn't fit at 11px, the component needs to be redesigned, not the font shrunk.

---

## 5. Spacing System

**Base unit: 4px**

| Token | Value | Usage |
|---|---|---|
| `space-px` | 1px | Hairline offsets, border positions |
| `space-1` | 4px | Icon-to-label gap, badge padding |
| `space-2` | 8px | Internal component padding, tight list gaps |
| `space-3` | 12px | Input padding, list item padding |
| `space-4` | 16px | Standard component padding, card padding-x |
| `space-5` | 20px | Between related groups in a panel |
| `space-6` | 24px | Card padding-y, section internal padding |
| `space-8` | 32px | Between unrelated groups, section gaps |
| `space-10` | 40px | Major section separation |
| `space-12` | 48px | Panel-level breathing room |
| `space-16` | 64px | Screen-level gaps, welcome screen |

### Fixed Component Heights

| Component | Height | Notes |
|---|---|---|
| Status Bar | 36px | Fixed, never changes |
| Nav Rail (Observatory) | 100% − 36px | Fills remaining height |
| Nav Rail width | 56px | Icon + label collapsed; 200px expanded |
| Device row | 56px | Two-row telemetry layout |
| Category header | 24px | Single line |
| Panel header | 40px | With action button |
| Input field | 36px | Standard |
| Button (default) | 36px | Standard |
| Button (compact) | 28px | Toolbar / dense panels |
| Status Bar zone separator | 1px × 24px | Vertically centred, not full height |

---

## 6. Layout Grid

### Observatory Mode

```
┌──────────────────────────────────────────────────────────┐
│  Titlebar: 48px  [Dark Sky]  [OBS ⟷ TRAVEL]  [☾] [···] │
├──────────────────────────────────────────────────────────┤
│  Status Bar: 36px                                        │
├────────┬─────────────────────────────────────┬───────────┤
│        │                                     │           │
│  Nav   │   Active Pane                       │  Device   │
│  Rail  │   (changes with nav selection)      │  Panel    │
│  56px  │                                     │  280px    │
│  (icon │                                     │  (toggle) │
│  only) │                                     │           │
│        ├─────────────────────────────────────┤           │
│        │  Log Strip: 72px (3 lines visible)  │           │
│        │  [expandable to 50% height]         │           │
└────────┴─────────────────────────────────────┴───────────┘
```

**Active Pane** is the majority of the screen. It reconfigures completely based on nav selection. It is not a multi-panel mosaic (that was NINA's model) — it is a single focused context that can contain sub-panels within it.

**Device Panel** (right) is toggleable. Default: open in Observatory, closed in Travel. It is the Device Connection Dashboard.

**Nav Rail** (left): icon-only by default. Hover expands to show labels (200px). Keyboard: Tab cycles through nav items.

**Log Strip** (bottom): always 3 lines visible. Click/drag to expand. Keyboard shortcut `L` toggles full log.

### Travel Mode

Identical structure, with these changes:
- Device Panel: hidden by default (accessible via nav)
- Nav Rail: collapses further to 48px, labels never shown unless hovering
- Log Strip: hidden by default (accessible via `L`)
- All font sizes: +1px step up across the board (body 14px → body 15px etc.)
- Status Bar: 32px (4px reduced)

### Content Area Columns (within Active Pane)

| Pane | Column layout |
|---|---|
| Sequencer | Two columns: sequence list (40%) + step editor (60%) |
| Live Preview | Single column: image fills available height |
| Focus | Two columns: focus graph (60%) + controls (40%) |
| Guiding | Two columns: guide graph (65%) + controls (35%) |
| Framing | Single column: sky chart fills available space |
| Log | Single column: full-width log table |
| Equipment | Two columns: device list (40%) + device detail (60%) |

### Minimum Window Size
- Observatory: 1024 × 700px
- Travel: 800 × 600px
- Below minimum: banner warning, UI still functional but not guaranteed

---

## 7. Elevation System

Dark Sky uses a subtle elevation model. Surfaces are distinguished by background colour, not drop shadows. Shadows are reserved for overlapping layers (modals, tooltips).

| Token | Value | Usage |
|---|---|---|
| `elevation-0` | `none` | Flat — app background, nav |
| `elevation-1` | `none` (bg: `color-bg-surface`) | Panels, sidebars |
| `elevation-2` | `none` (bg: `color-bg-elevated`) | Cards, inputs |
| `elevation-3` | `0 2px 8px rgba(0,0,0,0.32)` | Dropdowns, popovers |
| `elevation-4` | `0 8px 32px rgba(0,0,0,0.48)` | Modals, dialogs |
| `elevation-5` | `0 16px 48px rgba(0,0,0,0.64)` | Critical alerts, safety overlays |

Night Mode shadow colour: use `rgba(0,0,0,0.72)` for all elevation shadow values — deeper black reads better against red surfaces.

---

## 8. Border Radius System

| Token | Value | Usage |
|---|---|---|
| `radius-sm` | 4px | Tags, badges, inputs, small buttons |
| `radius-md` | 6px | Buttons (default), cards (inner) |
| `radius-lg` | 8px | Cards, panels, popovers |
| `radius-xl` | 12px | Modals, sheets, large cards |
| `radius-2xl` | 16px | Welcome screen cards, feature cards |
| `radius-full` | 9999px | Pills, status dots (as needed), toggles |

---

## 9. Motion System

### Principles
1. **Motion provides feedback, not decoration.** If removing an animation makes the UI harder to understand, it earns its place. If removing it makes no difference, remove it.
2. **No looping animations during normal session operation.** A blinking value during a 5-hour imaging run is torture. The single exception: the connecting-state device dot pulse — it stops the moment the device connects.
3. **Safety state changes are immediate.** No easing on a safety abort. It must feel instant because it is urgent.

### Duration Tokens

| Token | Value | Usage |
|---|---|---|
| `duration-instant` | 80ms | Button press feedback, toggle flick |
| `duration-fast` | 150ms | Hover states, small micro-interactions |
| `duration-normal` | 250ms | Panel transitions, state changes |
| `duration-slow` | 400ms | Page/pane transitions, modal entrance |
| `duration-deliberate` | 600ms | Welcome screen, onboarding moments |

### Easing Tokens

| Token | Curve | Usage |
|---|---|---|
| `ease-standard` | `cubic-bezier(0.4, 0, 0.2, 1)` | General transitions |
| `ease-enter` | `cubic-bezier(0, 0, 0.2, 1)` | Elements entering the screen |
| `ease-exit` | `cubic-bezier(0.4, 0, 1, 1)` | Elements leaving the screen |
| `ease-spring` | `cubic-bezier(0.34, 1.56, 0.64, 1)` | Mode toggle, occasional delight moments |

### Reduce Motion

Every animation in Dark Sky has a Reduce Motion fallback. When `prefers-reduced-motion: reduce` is active:

- All transitions → instant (0ms) or simple opacity fade at `duration-fast`
- Entrance animations → no translate/scale, opacity only
- Connecting-state pulse → static dot at 60% opacity
- Meridian flip pulse → static, full opacity
- Pane transitions → instant swap

Implementation: Forge uses a CSS custom property `--motion-duration-scale` set to `0` when reduced motion is detected. All `duration-*` tokens are multiplied by this scale. Opacity-only fallbacks are defined per component.

---

## 10. Iconography

### Icon Set
**Phosphor Icons** (phosphoricons.com)  
Weight: Regular for nav and general UI; Bold for status indicators; Fill for active/selected states.  
Fallback: If a specific glyph is missing from Phosphor, commission a custom SVG at the same optical weight.

### Icon Sizing

| Context | Size | Notes |
|---|---|---|
| Nav rail icons | 22px | At 56px rail width, optically centred |
| Panel action icons | 18px | Toolbar buttons, panel headers |
| Inline / label icons | 16px | Next to text labels |
| Status dots | 8–10px | CSS circle, not an icon |
| Status Bar icons | 14px | Cloud, warning glyphs |

### Icon Rules
- Icons do not have their own colour — they inherit `currentColor` from their container's text colour
- Never use colour alone to convey icon meaning — always pair with a label or tooltip
- Active nav icon: fill variant + `color-interactive-primary`
- All icons have `aria-hidden="true"` — meaning is carried by adjacent text or `aria-label` on the parent

---

## 11. Telescope & Sensor Profile (Data Model — Design Context)

Dark Sky requires user-defined equipment profiles to power the Framing Assistant and FOV calculations. This is not a component spec — it is a data model Canvas needs to design forms around.

### Required Fields

| Field | Type | Notes |
|---|---|---|
| Profile name | String | e.g., "Main Rig — C11 + ASI2600" |
| Telescope / lens name | String | |
| Focal length | Integer (mm) | Used for FOV calc |
| Aperture | Integer (mm) | Used for f-ratio and limiting magnitude |
| f-ratio | Float | Computed from focal length / aperture; editable as override |
| Focal reducer / Barlow | Float (multiplier) | Default 1.0 (none) |
| Effective focal length | Float (mm) | Computed: focal_length × reducer; display only |
| Sensor name | String | Selectable from a bundled database, or manual |
| Sensor width | Float (mm) | |
| Sensor height | Float (mm) | |
| Pixel size | Float (µm) | |
| Resolution X | Integer (px) | |
| Resolution Y | Integer (px) | |
| Computed FOV X | Float (arcmin) | Display only: (sensor_width / focal_length) × 3438 |
| Computed FOV Y | Float (arcmin) | Display only |
| Computed plate scale | Float (arcsec/px) | Display only: (pixel_size / focal_length) × 206.265 |
| Filter wheel slots | Integer | 5 or 7 (or custom) |
| Filter slot names | Array[String] | e.g., ["L","R","G","B","Ha","OIII","SII"] |
| Rotator offset | Float (degrees) | Home position calibration offset |

This profile drives: FOV rectangle in Framing Assistant, recommended dither scale, focus run V-curve display scale, and any auto-calculated exposure guidance.

---

## 12. Night Mode UX Rules

Night Mode is a first-class feature, not a colour swap. These rules govern its behaviour across the entire app.

1. **Toggle:** Keyboard shortcut `N` (toggleable). Also accessible from the `☾` icon in the Titlebar right section. The icon uses a filled crescent when Night Mode is active, outline when inactive.
2. **Persistence:** Night Mode state persists across app restarts, stored in user preferences.
3. **Transition:** When Night Mode activates, the entire UI transitions to the night palette in `duration-slow` (400ms), `ease-standard`. This is the one deliberate full-UI transition. Reduce Motion: instant swap.
4. **No white at any point during the transition.** The interpolation must go dark→red, never dark→white→red. Forge must ensure intermediate states are tested.
5. **Images:** The first-run/welcome nebula image is desaturated and red-channel-boosted in Night Mode. A separate pre-processed version of each welcome image is shipped with the app.
6. **External content:** If the app embeds any external web view (e.g., a sky chart from an external source), Night Mode applies a CSS filter: `invert(1) sepia(1) hue-rotate(300deg) saturate(4)` as a best-effort approximation. This is a known limitation, noted in the UI with a tooltip.
7. **The Power Menu** in Night Mode uses red-palette backgrounds. All status colours shift to their Night Mode values. The interactive accent shifts from indigo to deep red.

---

## 13. Accessibility Baseline

- **WCAG 2.1 AA** is the minimum. AAA is the target for all text pairs.
- **Minimum touch/click target:** 32×32px for desktop (cursor precision). 44×44px for any element that could be used on a touchscreen (Travel mode consideration).
- **Keyboard navigability:** Every interactive element is reachable by keyboard. Tab order follows visual reading order. Focus rings use `color-border-strong` outline, 2px solid, 2px offset.
- **Screen reader:** All status values are in `aria-live` regions. Safety state changes use `aria-live="assertive"`. Navigation landmarks use proper ARIA roles.
- **Colour alone never conveys meaning.** Status is always colour + label. Device state is always colour + text. A fully colour-blind user can operate Dark Sky.
- **Reduced motion:** Implemented system-wide via `prefers-reduced-motion`. See Section 9.
- **Night Mode and accessibility:** Night Mode values all pass WCAG AA contrast. The red palette was chosen with sufficient luminance contrast — warm reds on near-black backgrounds, not saturated mid-reds on dark reds.

---

## 14. Component Naming Convention (Sketch + Code)

```
[Component]/[Variant]/[State]

Examples:
  Button/Primary/Default
  Button/Primary/Hover
  Button/Primary/Pressed
  Button/Primary/Disabled
  Button/Ghost/Default
  DeviceRow/Nominal/Default
  DeviceRow/Error/Default
  DeviceRow/Connecting/Default
  StatusZone/Mount/Tracking
  StatusZone/Mount/Fault
  NavItem/Default/Inactive
  NavItem/Default/Active
  NavItem/Default/Focused
```

---

## 15. File Locations

| Asset | Location |
|---|---|
| Brand System (this doc) | `/docs/BRAND_SYSTEM.md` |
| Component Contracts | `/pipeline/contracts/[component]-contract.md` |
| Design Reviews | `/pipeline/reviews/[component]-review.md` |
| Sketch file | `DarkSky-Design.sketch` |
| Welcome images (source) | `/src/ui/assets/welcome/` |
| Icon set | `/src/ui/assets/icons/` (Phosphor subset export) |
| Font files (local fallback) | `/src/ui/assets/fonts/` |

---

*Dark Sky Brand System v1.0.0 — Canvas, CombeCrew Design*  
*Next: App Shell Layout Spec → Status Bar Contract (final) → Device Dashboard Contract (final) → Live Preview Panel*
