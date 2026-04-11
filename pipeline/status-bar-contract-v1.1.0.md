# Component Contract: Status Bar

**Version:** 1.1.0
**Date:** 2026-04-09
**Status:** Signed off — Ready for Stage 3 Build
**Author:** Forge (reconstructed from session log v1_2 / v1_3)
**Pipeline Stage:** 3 — Build to Contract

---

## 0. Overview

The Status Bar is a permanent 40px horizontal strip fixed at the bottom of the application chrome. It is always visible in every mode and every screen. It is divided into four fixed zones — A, B, C, D — each owned by a distinct subsystem.

The Status Bar is the application's ambient intelligence layer: it communicates safety, connection, session, and mode state at all times without requiring the user to navigate anywhere.

---

## 1. Zone Layout

```
+--[STATUS BAR: full viewport width, 40px height]-------------------------------------------+
|                                                                                             |
|  [ZONE A: 40px]  [ZONE B: 160–220px flex]  [ZONE C: fill]  [ZONE D: 40–80px]              |
|  Safety trigger  Connection Widget          Sequencer        Night Mode + Settings          |
|                                                                                             |
+---------------------------------------------------------------------------------------------+
```

### Zone Dimensions

| Zone   | Width              | Content                          | Right border           |
|--------|--------------------|----------------------------------|------------------------|
| Zone A | 40px fixed         | Safety Panel trigger button      | 1px color-border-subtle |
| Zone B | min 160px, max 220px flex | Connection Widget           | 1px color-border-subtle |
| Zone C | fills remaining    | Sequencer status / sequence name | none                   |
| Zone D | 40–80px            | Night Mode toggle + Settings icon | none                   |

**Layout:** Horizontal flex row, all zones vertically centred.
**Total height:** 40px fixed.
**Background:** color-background-primary.
**Top border:** 1px color-border-default (separates from main content area above).

---

## 2. Zone A — Safety Panel Trigger

### Purpose

Single icon button. Opens / closes the Safety Panel drawer overlay. Acts as ambient safety status indicator via icon state and colour.

### Anatomy

```
+--[ZONE A: 40×40px]--+
|    [SHIELD ICON]     |
+----------------------+
```

### Spec

| Property          | Value                                            |
|-------------------|--------------------------------------------------|
| Width             | 40px fixed                                       |
| Height            | 40px (full status bar height)                    |
| Icon              | Phosphor Shield (Duotone), 20×20px               |
| Touch target      | 40×40px (entire zone)                            |
| Right border      | 1px color-border-subtle                          |
| Background hover  | color-background-secondary                       |
| Background active | color-background-tertiary                        |
| Transition        | Background 100ms ease-standard                   |

### Icon States

| Safety State     | Icon Color               | Icon Style     |
|------------------|--------------------------|----------------|
| All Clear        | color-status-success     | Duotone filled |
| Warning          | color-status-warning     | Duotone filled |
| Unsafe           | color-status-error       | Duotone filled |
| Disconnected     | color-text-tertiary      | Outline        |
| Initialising     | color-text-secondary     | Outline        |
| Panel Open       | color-interactive-primary | Duotone filled (active state) |

### Accessibility

- `AutomationProperties.Name`: "Safety Panel" + current safety status (e.g. "Safety Panel — All Systems Safe")
- `AutomationProperties.Role`: Button
- Keyboard: Space / Enter toggles panel
- Focus indicator: 2px color-interactive-primary, 2px offset

---

## 3. Zone B — Connection Widget

Zone B is specified in full in the Device Connect Contract v1.0.0 (Section 2) and amended by Status Bar Amendment v1.2.0. This section provides the Zone B integration spec only.

### Integration Spec

| Property            | Value                                                         |
|---------------------|---------------------------------------------------------------|
| Zone width          | min 160px, max 220px (content-driven within these bounds)     |
| Zone height         | 40px                                                          |
| Right border        | 1px color-border-subtle                                       |
| Internal padding    | None — Connection Widget component manages its own spacing    |
| Overflow behaviour  | Label truncates with ellipsis at max-width                    |
| Component           | `ConnectionWidget.xaml` — self-contained UserControl          |

The Connection Widget is a self-sizing component. Zone B clips it to the min/max bounds above. See Device Connect Contract v1.0.0 §2 and Status Bar Amendment v1.2.0 §3 for the full widget spec including all states.

---

## 4. Zone C — Sequencer Status

### Purpose

Displays active sequence name and session state. Provides a shortcut to the Sequencer. This zone fills all remaining horizontal space between Zone B and Zone D.

### States

| Sequencer State  | Display                                             |
|------------------|-----------------------------------------------------|
| Idle             | "No sequence loaded" — color-text-tertiary          |
| Ready            | "[Sequence name]" — color-text-secondary            |
| Running          | "▶ [Sequence name] — [progress]" — color-text-primary |
| Paused           | "⏸ [Sequence name] — Paused" — color-status-warning |
| Stopped          | "[Sequence name] — Stopped" — color-text-secondary  |
| Error            | "⚠ [Error summary]" — color-status-error            |

### Typography

- Sequence name: text-label (12px / 500)
- Status prefix (▶ ⏸ ⚠): 12px glyph, left of name, 4px gap
- Truncation: sequence name truncates with ellipsis when Zone C width is insufficient

### Interaction

- Click anywhere in Zone C: navigates to Sequencer mode
- Cursor: pointer
- Hover: color-background-secondary fill (full zone width)

### Accessibility

- `AutomationProperties.Name`: "Sequencer — [current status]"
- `AutomationProperties.Role`: Button

---

## 5. Zone D — Night Mode + Settings

### Purpose

Night Mode quick-toggle and Settings navigation shortcut.

### Anatomy

```
+--[ZONE D: 40–80px]----------------+
|  [MOON ICON]   [GEAR ICON]        |
+------------------------------------+
```

### Spec

| Property         | Value                                          |
|------------------|------------------------------------------------|
| Width            | 40px (Moon icon only) to 80px (both icons)     |
| Icon size        | Phosphor Duotone, 20×20px each                 |
| Icon gap         | 0px — icons are adjacent                       |
| Touch target     | 40×40px per icon                               |
| Moon icon        | Phosphor Moon — toggles Night Mode on/off      |
| Gear icon        | Phosphor GearSix — navigates to Settings       |
| Background hover | color-background-secondary (per icon, not zone) |

### Night Mode Icon States

| Night Mode State | Moon Icon Color       | Tooltip              |
|------------------|-----------------------|----------------------|
| Off              | color-text-secondary  | "Enable Night Mode"  |
| On               | color-status-warning  | "Disable Night Mode" |

When Night Mode is active, the entire Status Bar receives the Night Mode red overlay — it does not change its own token colours independently. The overlay is applied by ThemeService at the root level.

---

## 6. Status Bar — Full Color Spec

| Element                   | Light Token                | Dark Token                 |
|---------------------------|----------------------------|----------------------------|
| Background                | color-background-primary   | color-background-primary   |
| Top border                | color-border-default       | color-border-default       |
| Zone A/B divider borders  | color-border-subtle        | color-border-subtle        |
| Icon (default)            | color-text-secondary       | color-text-secondary       |
| Icon (hover)              | color-text-primary         | color-text-primary         |
| Zone hover background     | color-background-secondary | color-background-secondary |
| Night Mode overlay        | ThemeService — not a token | ThemeService — not a token |

---

## 7. Typography

All text in the Status Bar uses text-label (12px / 500 / 1.4). No exceptions. The Status Bar is a compact 40px strip — no larger type is appropriate.

---

## 8. Animation

| Transition                | Spec                                                         |
|---------------------------|--------------------------------------------------------------|
| Safety icon state change  | Color cross-fade, 200ms ease-standard                        |
| Zone C text update        | Opacity fade 1→0→1, 150ms total, ease-standard               |
| Night Mode toggle         | ThemeService-level — Status Bar inherits, no independent spec |
| Reduce Motion             | All transitions instant                                      |

---

## 9. Platform Notes (Windows / WinUI 3)

- Status Bar is a UserControl fixed at the bottom of the main Window layout, outside the NavigationView frame.
- Height is fixed at 40px — not adjustable by the user.
- Zone layout implemented as a horizontal StackPanel or Grid with ColumnDefinition widths: Auto (40px), MinWidth/MaxWidth bounds (Zone B), Star fill (Zone C), Auto (Zone D).
- Zone B hosts the `ConnectionWidget` UserControl.
- Night Mode overlay applied at the Window or ContentRoot level by ThemeService — the Status Bar does not apply it independently.
- Escape key on Zone A (Safety Panel trigger): if panel is open, close it. If closed, open it.

---

## 10. Out of Scope (this contract)

- Connection Widget full spec → Device Connect Contract v1.0.0 §2 + Status Bar Amendment v1.2.0
- Session Ready state → Status Bar Amendment v1.2.0
- Full Sequencer status spec → Sequencer contract (future)
- Log Strip (sits above Status Bar) → separate contract
- Night Mode overlay implementation → ThemeService contract (future)

---

## 11. Sign-off

| Role                | Name   | Status   | Date       |
|---------------------|--------|----------|------------|
| Design (Canvas)     | Canvas | Approved | 2026-04-09 |
| Engineering (Forge) | Forge  | Approved | 2026-04-09 |
| Product (Matt)      | Matt   | Approved | 2026-04-09 |

---

*Component Contract v1.1.0 — Dark Sky*
*Forge / CombeCrew Engineering*
*Reconstructed from session log v1_2 / v1_3 — 2026-04-09*
