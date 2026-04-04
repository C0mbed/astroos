# Tech Spec: App Shell
**Component:** App Shell — the root layout container for all Dark Sky UI  
**Version:** 1.0.0  
**Date:** 2026-04-04  
**Author:** Canvas (CombeCrew Design)  
**Status:** Ready for Canvas Contract  
**Depends on:** Brand System v1.0.0, Token Layer  

---

## Overview

The App Shell is the persistent root layout. It is not a "screen" — it is the fixed structural container that every screen, pane, and panel lives inside. It owns:

- The Titlebar (48px)
- The Status Bar (36px)
- The Nav Rail (56px collapsed / 200px expanded)
- The Active Pane (flex-fill)
- The Device Panel (280px, toggleable)
- The Log Strip (72px collapsed / up to 50vh expanded)

The App Shell renders once on launch and never unmounts. All navigation is handled by swapping the Active Pane content. No full-page transitions. No reloads.

---

## Layout Model

### Observatory Mode — Full Layout

```
┌─────────────────────────────────────────────────────────────────┐
│  TITLEBAR                                                  48px  │
├─────────────────────────────────────────────────────────────────┤
│  STATUS BAR                                                36px  │
├──────────┬──────────────────────────────────────┬───────────────┤
│          │                                      │               │
│   NAV    │   ACTIVE PANE                        │  DEVICE PANEL │
│   RAIL   │                                      │               │
│   56px   │   flex: 1                            │   280px       │
│          │   min-width: 0                       │   toggleable  │
│          │                                      │               │
│          │                                      │               │
│          ├──────────────────────────────────────┤               │
│          │  LOG STRIP              72px / 50vh  │               │
└──────────┴──────────────────────────────────────┴───────────────┘
```

### Travel Mode — Simplified Layout

Identical DOM structure. Changes are CSS-driven via `data-mode="travel"` on the root element:

- Status Bar: 32px (down from 36px)
- Nav Rail: 48px collapsed width (down from 56px), labels never auto-show on hover
- Device Panel: `display: none` by default, toggled via nav Equipment item
- Log Strip: `display: none` by default, toggled via `L` key or nav Log item
- All type sizes: step up by one scale token (applied via `--mode-type-scale: 1` root custom property that components read)

### Night Mode

Night Mode is a colour override only. It does not change layout dimensions, component structure, or DOM. It is applied via `data-theme="night"` on the root `<html>` element. All token values resolve to their night-mode equivalents via CSS custom property overrides scoped to `[data-theme="night"]`.

---

## Component Breakdown

### 1. Titlebar

**Height:** 48px fixed  
**Width:** 100vw  
**Background:** `color-bg-app`  
**Border-bottom:** 1px solid `color-border-subtle`  
**Z-index:** 100 (above all pane content, below modals)

**Layout (left to right):**

```
[  Dark Sky wordmark  ]  [  ·  ·  ·  ]  [  OBS ⟷ TRAVEL toggle  ]  [  ☾  ]  [  ···  ]
 ← 16px left padding →                                                ← 16px right padding →
```

**Elements:**

| Element | Spec |
|---|---|
| Wordmark | Space Grotesk 600, 16px, `color-text-primary`, left-aligned, `padding-left: space-4` |
| Spacer | `flex: 1` — pushes right-side controls to the right |
| Mode Toggle | Segmented control — see Section below |
| Night Mode button | Icon-only button, 32×32px, Phosphor `Moon` (outline = day, fill = night), `color-text-secondary`, toggles `data-theme="night"` on `<html>` |
| Overflow menu `···` | Icon-only button, 32×32px, Phosphor `DotsThree`, opens settings/about menu |
| Right padding | `padding-right: space-4` |

**Mode Toggle (OBS ⟷ TRAVEL):**

```
┌─────────────────────────┐
│  ● Observatory │ Travel  │   ← active segment has filled bg
└─────────────────────────┘
```

- Container: `border: 1px solid color-border-default`, `border-radius: radius-sm`, height 28px
- Each segment: `padding: 0 space-3`, `text-label-sm` uppercase, min-width 80px
- Active segment: `background: color-interactive-primary`, `color: white` (or `#FFFFFF` — this is the one place white text appears, on the coloured interactive background)
- Inactive segment: `background: transparent`, `color: color-text-secondary`
- Transition: `duration-fast`, `ease-standard`
- Sets `data-mode="observatory"` or `data-mode="travel"` on root `<html>`
- Persists to user preferences on change

---

### 2. Status Bar

**Height:** 36px Observatory / 32px Travel  
**Width:** 100% of viewport  
**Background:** `color-bg-app`  
**Border-bottom:** 1px solid `color-border-subtle`  
**Z-index:** 99 (below Titlebar, above pane content)  
**Overflow:** hidden — nothing wraps, nothing scrolls

Full specification: see `/pipeline/contracts/status-bar-contract.md`

---

### 3. Nav Rail

**Width:** 56px collapsed / 200px expanded (Observatory) | 48px collapsed / never expanded (Travel)  
**Height:** `calc(100vh - 84px)` — full remaining height below Titlebar + Status Bar  
**Background:** `color-bg-surface`  
**Border-right:** 1px solid `color-border-subtle`  
**Z-index:** 50  
**Transition:** width `duration-normal` `ease-standard`

**Expansion trigger:**
- Observatory: hover over rail expands to 200px, shows text labels. Mouse leave collapses back.
- Travel: never expands. Labels are tooltips on hover only (Phosphor tooltip pattern).
- Keyboard: `Tab` focuses nav items. `Enter`/`Space` activates. Rail does not expand on keyboard focus alone.

**Nav Item anatomy (collapsed):**

```
┌────────┐
│        │
│  icon  │  22px icon, centred in 56px width
│        │
└────────┘
   48px height per item
```

**Nav Item anatomy (expanded):**

```
┌──────────────────────┐
│  icon   Label        │  22px icon + 12px gap + text-body-sm label
└──────────────────────┘
   48px height, padding-left: space-4
```

**Nav Items — ordered top to bottom:**

| Icon (Phosphor) | Label | Pane target | Keyboard shortcut |
|---|---|---|---|
| `Aperture` (Bold) | Sequence | SequencerPane | `S` |
| `ImageSquare` | Live | LivePreviewPane | `V` |
| `Crosshair` | Focus | FocusPane | `F` |
| `ArrowsOutCardinal` | Guide | GuidePane | `G` |
| `Compass` | Framing | FramingPane | `A` (for Astrometry) |
| `Terminal` | Log | LogPane | `L` |
| — (divider) | — | — | — |
| `HardDrives` | Equipment | EquipmentPane | `E` |

**Divider:** 1px horizontal rule, `color-border-subtle`, `margin: space-2 space-3`

**Power Menu trigger — bottom of rail:**

```
┌────────────────────────────────┐
│  ⚡  Quick Actions              │  ← pinned to bottom of rail
└────────────────────────────────┘
```

- Position: `position: sticky; bottom: 0` within the rail scroll container
- Background: `color-bg-surface` (matches rail, covers scrolled items)
- Border-top: 1px solid `color-border-subtle`
- Height: 48px
- Icon: Phosphor `Lightning` Bold, `color-interactive-primary`
- Label (expanded): "Quick Actions", `text-body-sm`, `color-text-secondary`
- Keyboard: `P` or `Cmd/Ctrl+K` (command palette convention)
- Opens: Power Menu overlay (separate component spec)

**Active state:**

- Icon switches to Fill variant
- Icon + label: `color-interactive-primary`
- Left accent bar: 2px × 24px, `color-interactive-primary`, `border-radius: 0 radius-sm radius-sm 0`, `position: absolute; left: 0`
- Background: `color-interactive-subtle` (12% opacity indigo tint)

**Hover state (inactive items):**

- Background: `color-bg-elevated`
- No icon change

---

### 4. Active Pane

**Width:** `flex: 1`, `min-width: 0`  
**Height:** `calc(100vh - 84px - log-strip-height)`  
**Background:** `color-bg-app`  
**Overflow:** `hidden` on the container — each pane manages its own internal scroll

The Active Pane is a single mounting point. Only one pane is mounted at a time. Pane switching is:

1. Current pane exit: opacity 1→0, `duration-fast` `ease-exit`
2. New pane enter: opacity 0→1, `duration-fast` `ease-enter`
3. Reduce Motion: instant swap, no opacity transition

**Pane components (to be spec'd individually):**

| Pane | Component | Status |
|---|---|---|
| SequencerPane | Sequence editor + target queue | Pending spec |
| LivePreviewPane | Frame preview + histogram + metadata | Pending spec |
| FocusPane | Focus graph + V-curve + manual controls | Pending spec |
| GuidePane | Guide graph + corrections + PHD2/EKOS status | Pending spec |
| FramingPane | Sky chart + FOV overlay + target search | Pending spec |
| LogPane | Full-width log table | Pending spec |
| EquipmentPane | Device list + device detail (replaces Device Panel on small widths) | Pending spec |

---

### 5. Device Panel

**Width:** 280px  
**Height:** `calc(100vh - 84px)`  
**Background:** `color-bg-surface`  
**Border-left:** 1px solid `color-border-subtle`  
**Z-index:** 50  
**Default state:** Visible (Observatory), hidden (Travel)

**Toggle behaviour:**
- Toggle button: Phosphor `SidebarSimple` icon in the Titlebar right area (between Night Mode and overflow menu)
- Collapsed: panel slides out right (`transform: translateX(280px)`), transition `duration-normal` `ease-standard`
- When collapsed, the Active Pane expands to fill the freed space via flex
- State persists to user preferences

Full specification: see `/pipeline/contracts/device-dashboard-contract.md`

---

### 6. Log Strip

**Height collapsed:** 72px (shows 3 log lines)  
**Height expanded:** up to `50vh`, user-draggable  
**Width:** fills Active Pane width (not full viewport — does not extend under Nav Rail or Device Panel)  
**Background:** `color-bg-surface`  
**Border-top:** 1px solid `color-border-subtle`  
**Z-index:** 40

**Collapsed anatomy:**

```
┌─────────────────────────────────────────────────────────────┐  ← drag handle: 4px
│ LOG                                              [↑ Expand] │  ← header: 24px
│ 23:14:02  mount.slew_to_ra_dec(05h34m, +22°00)  OK         │  ← line 1
│ 23:13:58  camera.expose(300.0s, Ha, 1x1, g100)  OK         │  ← line 2
│ 23:13:55  filter_wheel.set_position(3)          OK         │  ← line 3
└─────────────────────────────────────────────────────────────┘
```

**Drag handle:** 4px strip at top, `cursor: ns-resize`, `background: color-border-subtle` on hover  
**Expand button:** Phosphor `ArrowsOutSimple` 14px, `text-label-sm` "Expand", `color-text-muted`, right-aligned  
**Log lines:** `text-data-sm` (11px JetBrains Mono), `color-text-secondary`  
**Timestamp:** `color-text-muted`  
**Command:** `color-text-primary`  
**Result OK:** `color-status-nominal`  
**Result ERR:** `color-status-error` — full line gets `color-status-error-bg` tint  
**Auto-scroll:** newest entry at bottom, strip auto-scrolls. User scrolling up pauses auto-scroll. A "Jump to latest" pill appears when paused.

**Expanded state:**

- Strip fills up to 50vh (user-draggable by drag handle)
- Full log table with timestamp, level, source, message, result columns
- Filter bar appears at top: filter by level (INFO/WARN/ERROR), by source (mount/camera/etc), text search
- Keyboard `L` or `Escape` collapses back

**Travel Mode:** Log Strip hidden by default. `L` key makes it appear as an overlay panel (same content, positioned as a bottom sheet with `elevation-4` shadow).

---

## Responsive Breakpoints

| Viewport width | Behaviour |
|---|---|
| ≥ 1280px | Full layout, all panels default open |
| 1024–1279px | Device Panel closed by default |
| 900–1023px | Status Bar: Priority 2 zones hidden (Guide RMS) |
| 800–899px | Status Bar: Priority 3 zones hidden (+ Meridian) |
| < 800px | Warning banner: "Window too narrow for observatory use" |

---

## State Management Requirements

The App Shell owns and persists the following state (to user preferences / localStorage):

| State key | Type | Default (Observatory) | Default (Travel) |
|---|---|---|---|
| `mode` | `'observatory' \| 'travel'` | `'observatory'` | `'travel'` |
| `theme` | `'system' \| 'night'` | `'system'` | `'system'` |
| `navActivePane` | string (pane name) | `'SequencerPane'` | `'LivePreviewPane'` |
| `devicePanelOpen` | boolean | `true` | `false` |
| `logStripHeight` | number (px) | `72` | `72` |
| `logStripVisible` | boolean | `true` | `false` |
| `navRailExpanded` | boolean | `false` (hover-driven) | `false` |

---

## Keyboard Shortcuts (App Shell level)

| Key | Action |
|---|---|
| `N` | Toggle Night Mode |
| `S` | Navigate to Sequencer |
| `V` | Navigate to Live Preview |
| `F` | Navigate to Focus |
| `G` | Navigate to Guide |
| `A` | Navigate to Framing |
| `L` | Toggle Log Strip (expand/collapse or show/hide in Travel) |
| `E` | Navigate to Equipment |
| `P` or `Ctrl+K` | Open Power Menu |
| `Escape` | Close Power Menu / collapse Log Strip / close any open overlay |
| `[` | Toggle Device Panel |

Shortcuts are disabled when focus is inside an input, textarea, or select element.

---

## Accessibility

- Root landmark: `<div role="application" aria-label="Dark Sky Observatory Control">`
- Titlebar: `<header role="banner">`
- Status Bar: `<div role="status" aria-label="Observatory Status" aria-live="polite">`
- Nav Rail: `<nav role="navigation" aria-label="Main navigation">`
- Active Pane: `<main role="main" id="active-pane">`
- Device Panel: `<aside role="complementary" aria-label="Device Status">`
- Log Strip: `<section aria-label="Session Log" aria-live="polite">`
- Mode toggle: `role="radiogroup"`, each option `role="radio"` with `aria-checked`
- Night Mode button: `aria-label="Night mode: off"` / `"Night mode: on"`, `aria-pressed`
- All keyboard shortcuts: documented in a help modal (`?` key), `aria-keyshortcuts` on relevant elements

---

## Open Questions for Forge

1. **State persistence:** localStorage is the simplest approach for user preferences. Confirm this is acceptable, or if you prefer a user preferences API endpoint (relevant if multi-machine sync is on the roadmap).
2. **Pane mounting strategy:** Should inactive panes be unmounted (simpler, no background state) or hidden (`display: none`, preserves scroll position and state)? Canvas recommends hidden for Sequencer and Log (preserve state), unmounted for others.
3. **Log Strip drag handle:** Needs a `mousedown` + `mousemove` resize implementation. Confirm Forge is handling this or if a library is preferred.
4. **Power Menu:** This spec stubs the trigger point. The Power Menu itself is a separate component spec coming from Canvas shortly. Forge should not begin building the Power Menu internals until that contract is delivered.

