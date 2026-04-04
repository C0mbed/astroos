# Component Contract: Status Bar
**Version:** 1.1.0 (final)  
**Date:** 2026-04-04  
**Author:** Canvas (CombeCrew Design)  
**Status:** Ready for Build  
**Replaces:** Status Bar Contract v1.0.0 (draft)  

---

## Overview

The Status Bar is a fixed 36px strip (32px Travel) anchored immediately below the Titlebar. It is always visible — it never scrolls, collapses, or hides. It is the user's primary glanceable status surface: everything needed to confirm a session is running correctly, readable at arm's length in the dark.

Two zones are interactive (Safety and Astrospheric). All others are display-only.

---

## Anatomy

```
┌────────────────────────────────────────────────────────────────────────────────────────────┐
│ [A: SAFETY] │ [B: TARGET + FILTER] │ [C: MOUNT] │ [D: CAMERA] │ [E: RMS] │ [F: MER] │ [G: DAWN] │ [H: ASTRO] │
└────────────────────────────────────────────────────────────────────────────────────────────┘

Zone widths:
  A  Safety           120px  fixed
  B  Session Context  flex   min-content 180px, flex-grow 1
  C  Mount            196px  fixed
  D  Camera           196px  fixed
  E  Guide RMS        108px  fixed    Priority 2 — hide < 1200px
  F  Meridian         96px   fixed    Priority 3 — hide < 1100px
  G  Dawn             80px   fixed    Priority 4 — hide < 1000px
  H  Astrospheric     56px   fixed    Priority 5 — hide < 900px
```

All zones: `display: flex`, `align-items: center`, `height: 100%`  
Zone separators: `1px solid color-border-subtle`, `height: 20px` (not full bar height), vertically centred  
Container: `background: color-bg-app`, `border-bottom: 1px solid color-border-subtle`

---

## Zone A — Safety Indicator

**Width:** 120px fixed  
**Interactive:** Yes — click/Enter → opens Safety Detail Panel (overlay)

```
  [dot]  [LABEL]  [⚠]
   8px    11px    14px (conditional)
   ← space-4 left padding →
```

| State | Dot colour | Label | Label colour | ⚠ icon | Background tint |
|---|---|---|---|---|---|
| Safe | `color-status-safe` | SAFE | `color-status-safe` | Hidden | None |
| Caution | `color-status-caution` | CAUTION | `color-status-caution` | Hidden | `color-status-degraded-bg` |
| Unsafe | `color-status-unsafe` | UNSAFE | `color-status-unsafe` | Visible | `color-status-error-bg` |

**Elements:**
- Dot: 8px CSS circle (`border-radius: radius-full`), `color-status-safe/caution/unsafe` background
- Label: `text-label-sm` (11px Space Grotesk 500 uppercase 0.05em tracking), matches dot colour
- ⚠ icon: Phosphor `Warning` Bold 14px, `color-status-unsafe`, `margin-left: space-1`, only rendered when Unsafe
- Background tint: applied to Zone A container only, not the full bar

**Interaction:**
- `cursor: pointer`
- Hover: `color-bg-elevated` tint behind zone content
- Focus: 2px inset `color-border-strong` ring
- `role="button"`, `tabindex="0"`
- `aria-label="Safety status: [SAFE/CAUTION/UNSAFE]. Press Enter for details."`
- `aria-live="assertive"` on the label — announces state changes immediately

**Unsafe animation:**
- Background tint: fades in `duration-normal` `ease-standard` on state change to Unsafe
- Reduce Motion: instant

---

## Zone B — Session Context (Target + Filter)

**Width:** flex, `min-width: 180px`, `flex-grow: 1`  
**Interactive:** No  
**Padding:** `0 space-4`

```
  [Target Name]  [Filter Pill]
```

**Target name:**
- `text-heading-sm` (15px Space Grotesk 600)
- `color-text-primary`
- Max 16 characters, `text-overflow: ellipsis`, `overflow: hidden`, `white-space: nowrap`
- No target active: `— —` in `color-text-muted`

**Filter pill:**
- `margin-left: space-2`
- Height: 18px, `padding: 0 space-2` (0 8px)
- `border: 1px solid color-border-default`
- `border-radius: radius-full`
- Text: `text-label-sm` (11px 500 uppercase 0.05em), `color-text-secondary`
- `font-family: JetBrains Mono` — filter names are data
- Max 6 chars (Ha, OIII, SII, Lum, Red, Grn, Blu)
- No filter active: pill hidden entirely

---

## Zone C — Mount State

**Width:** 196px fixed  
**Interactive:** No  
**Padding:** `0 space-4`

```
  [STATE]  [coordinate string]
```

**State label:**
- `text-label-sm` (11px 500 uppercase 0.05em)
- Colour by state (see table below)
- `margin-right: space-2`

**Coordinate string:**
- `text-data-md` (13px JetBrains Mono 400)
- `color-text-data`
- `white-space: nowrap`

| State | Label | Colour |
|---|---|---|
| Tracking | TRACKING | `color-status-nominal` |
| Slewing | SLEWING | `color-status-degraded` |
| Parked | PARKED | `color-text-secondary` |
| Idle | IDLE | `color-text-muted` |
| Fault | FAULT | `color-status-error` |
| Disconnected | — | `color-status-disconnected` |

**Coordinate format (user-configurable, default RA/Dec):**
- RA/Dec: `05h34m  +22°01′` (abbreviated to fit — drop arcseconds at zone width)
- Alt/Az: `+42.3°  178°` 
- If disconnected: `—` centred in zone

---

## Zone D — Camera State

**Width:** 196px fixed  
**Interactive:** No  
**Padding:** `0 space-4`

```
  [STATE]  [countdown]  [temperature]
```

**State label:** `text-label-sm`, same colour convention as Mount zone

| State | Label | Colour |
|---|---|---|
| Exposing | EXPOSING | `color-status-nominal` |
| Downloading | DWNLD | `color-status-degraded` |
| Cooling | COOLING | `color-text-secondary` |
| Idle | IDLE | `color-text-muted` |
| Fault | FAULT | `color-status-error` |
| Disconnected | — | `color-status-disconnected` |

**Countdown (EXPOSING state only):**
- `text-data-md` (13px JetBrains Mono 400), `color-text-data`
- Format: `MM:SS` counting down
- `margin-left: space-2`
- Hidden when not exposing

**Temperature:**
- `text-data-sm` (11px JetBrains Mono 400)
- `margin-left: space-2`
- Format: `−15.0°` (en-dash, 1 decimal, degree symbol, no C suffix — space is tight)
- At target temp (±0.5°): `color-status-nominal`
- 0.5°–2° from target: `color-text-secondary`
- > 2° from target: `color-status-degraded`
- Disconnected: hidden

---

## Zone E — Guide RMS

**Width:** 108px fixed  
**Interactive:** No  
**Padding:** `0 space-4`  
**Hide below:** 1200px viewport width

```
  [RMS]  [value″]
```

**Label:**
- `text-label-sm` uppercase, `color-text-muted`
- `margin-right: space-1`

**Value:**
- `text-data-lg` (16px JetBrains Mono 500)
- Colour by threshold:
  - < 0.80″ → `color-status-nominal`
  - 0.80–1.50″ → `color-status-degraded`
  - > 1.50″ → `color-status-error`
- Unit `″` rendered as `text-data-sm` inline immediately after value (no space), `color-text-muted`
- Guider not running / disconnected: `—` in `color-text-muted`, `text-data-md` size

---

## Zone F — Time to Meridian

**Width:** 96px fixed  
**Interactive:** No  
**Padding:** `0 space-3`  
**Hide below:** 1100px viewport width

```
  [MER]  [HH:MM]
```

**Label:** `text-label-sm` uppercase, `color-text-muted`  
**Value:** `text-data-md` (13px JetBrains Mono 400)

| Condition | Value colour |
|---|---|
| > 30 min | `color-text-data` |
| 10–30 min | `color-status-degraded` |
| < 10 min | `color-status-error` |
| Flip due / overdue | `FLIP` label (not time), `color-status-error` |
| Unknown | `—` `color-text-muted` |

**FLIP animation:**
- When value = "FLIP": opacity pulses 1→0.35→1, 1000ms linear, infinite
- Reduce Motion: static, full opacity, no pulse

---

## Zone G — Time to Dawn

**Width:** 80px fixed  
**Interactive:** No  
**Padding:** `0 space-3`  
**Hide below:** 1000px viewport width

```
  [DAWN]  [HH:MM]
```

**Label:** `text-label-sm` uppercase, `color-text-muted`  
**Value:** `text-data-md` (13px JetBrains Mono 400)

| Condition | Value colour |
|---|---|
| > 60 min | `color-text-data` |
| 30–60 min | `color-status-degraded` |
| < 30 min | `color-status-error` |
| Unknown | `—` `color-text-muted` |

---

## Zone H — Astrospheric Score

**Width:** 56px fixed  
**Interactive:** Yes — click/Enter → opens Weather Detail Panel (overlay)  
**Padding:** `0 space-2`  
**Hide below:** 900px viewport width

```
  [☁]  [score]
```

**Icon:** Phosphor `Cloud` 14px, colour matches score tier (see below)  
**Score:** `text-data-lg` (16px JetBrains Mono 500), colour matches tier

| Score range | Colour | Meaning |
|---|---|---|
| 80–100 | `color-status-nominal` | Good to excellent conditions |
| 50–79 | `color-status-degraded` | Marginal conditions |
| 0–49 | `color-status-error` | Poor conditions |
| Not authenticated | `—` (dash) | `color-text-muted`, icon `color-text-muted` |
| Authenticated, fetching | Spinner 12px | — |
| Authenticated, no data | `?` | `color-text-muted` |

**Interaction:**
- `cursor: pointer`
- Hover: `color-bg-elevated` tint
- Focus: 2px inset `color-border-strong` ring
- `role="button"`, `tabindex="0"`
- `aria-label="Astrospheric score: [value]. Press Enter for weather detail."` or `"Astrospheric: not available"` when unauthenticated

---

## Responsive Zone Priority

When viewport narrows, zones hide in this order (each zone disappears fully — no partial display):

```
< 1200px → hide Zone E (Guide RMS)
< 1100px → hide Zone F (Meridian)
< 1000px → hide Zone G (Dawn)
< 900px  → hide Zone H (Astrospheric)
< 800px  → show warning banner (App Shell responsibility)
```

Zone B (Session Context) absorbs the freed space via `flex-grow: 1`.

---

## Full Bar States

| State | Description |
|---|---|
| Session running, all nominal | All zones populated, nominal colours |
| Session idle, devices connected | Zones show device states; B zone shows `— —` |
| Partial connection | Disconnected zones show `—` and `color-status-disconnected` |
| Safety abort (Level 2+) | Zone A: UNSAFE, tinted background; other zones continue updating |
| All devices disconnected | All data zones show `—`; full bar opacity reduced to 70% except Zone A |
| Night Mode | All colours resolve to night-mode token values |
| Travel Mode | Bar height 32px; all zones otherwise identical |

---

## Animation Summary

| Element | Animation | Duration | Easing | Reduce Motion |
|---|---|---|---|---|
| Zone A tint on unsafe | Fade in | `duration-normal` | `ease-standard` | Instant |
| Zone F FLIP pulse | Opacity 1→0.35→1 | 1000ms linear | — | None (static) |
| Zone H spinner | Rotate 360° | 800ms linear infinite | — | None (static spinner) |
| All value updates | Instant swap | — | — | — |
| Night Mode transition | All colours cross-fade | `duration-slow` | `ease-standard` | Instant |

**Rule:** No numeric value in the Status Bar animates during normal operation. Numbers replace numbers instantly. The user is reading this bar across a session — animation on data is noise, not signal.

---

## Accessibility

- Container: `role="banner"` is owned by Titlebar. Status Bar: `<div role="status" aria-label="Observatory Status">` within `<header>`
- All live values: wrapped in `aria-live="polite"` regions — updates are announced non-interruptively
- Safety state change to Unsafe: `aria-live="assertive"` — interrupts current screen reader output
- Zone A: `role="button" tabindex="0" aria-label="Safety status: [state]. Press Enter for details."`
- Zone H: `role="button" tabindex="0" aria-label="Astrospheric score: [value/unavailable]. Press Enter for weather detail."`
- Zones C, D, E, F, G: `aria-label` on their container with full human-readable value, e.g., `aria-label="Mount: Tracking, RA 5h 34m, Dec +22 degrees"`
- Focus rings: `outline: 2px solid var(--color-border-strong); outline-offset: -2px` (inset, so it doesn't overflow the 36px bar)
- All contrast pairs meet WCAG AA minimum. See Brand System Section 3c for documented ratios.

---

## Sketch Symbol References

```
StatusBar/Root
StatusBar/Zone/Safety/Safe
StatusBar/Zone/Safety/Caution
StatusBar/Zone/Safety/Unsafe
StatusBar/Zone/Session/Active
StatusBar/Zone/Session/Idle
StatusBar/Zone/Mount/Tracking
StatusBar/Zone/Mount/Slewing
StatusBar/Zone/Mount/Parked
StatusBar/Zone/Mount/Fault
StatusBar/Zone/Mount/Disconnected
StatusBar/Zone/Camera/Exposing
StatusBar/Zone/Camera/Downloading
StatusBar/Zone/Camera/Idle
StatusBar/Zone/Camera/Fault
StatusBar/Zone/Camera/Disconnected
StatusBar/Zone/GuideRMS/Nominal
StatusBar/Zone/GuideRMS/Degraded
StatusBar/Zone/GuideRMS/Error
StatusBar/Zone/GuideRMS/Inactive
StatusBar/Zone/Meridian/Normal
StatusBar/Zone/Meridian/Warning
StatusBar/Zone/Meridian/Critical
StatusBar/Zone/Meridian/Flip
StatusBar/Zone/Dawn/Normal
StatusBar/Zone/Dawn/Warning
StatusBar/Zone/Dawn/Critical
StatusBar/Zone/Astrospheric/Good
StatusBar/Zone/Astrospheric/Marginal
StatusBar/Zone/Astrospheric/Poor
StatusBar/Zone/Astrospheric/Unavailable
```

Sketch page: `Components → Section: Chrome → Status Bar`

---

*Canvas sign-off required before Forge builds. Forge sign-off required before build starts.*  
*Canvas: ✅ Ready | Forge: ⬜ Pending*
