# Component Contract: Device Connection Dashboard
**Version:** 1.1.0 (final)  
**Date:** 2026-04-04  
**Author:** Canvas (CombeCrew Design)  
**Status:** Ready for Build  
**Replaces:** Device Dashboard Contract v1.0.0 (draft)  

---

## Overview

The Device Connection Dashboard is a persistent 280px panel, docked on the right side of the App Shell. It surfaces every hardware device known to Dark Sky with live health state, connection quality, and last-command trace. Clicking any device row opens a Device Detail Panel — a contextual drawer that exposes device-specific controls and calibration actions.

This is both a diagnostic surface (is my equipment healthy?) and a control surface (connect, calibrate, adjust). The distinction from the Sequencer is important: the Dashboard controls hardware state and readiness. The Sequencer controls what the hardware does during a session.

---

## Panel Anatomy

```
┌─────────────────────────────────────────────────────┐
│ DEVICES                   [Reconnect All]            │  ← Panel Header: 40px
├─────────────────────────────────────────────────────┤
│ MOUNTS                                               │  ← Category Header: 24px
│ ┌───────────────────────────────────────────────┐   │
│ │ ●  EQ6-R Pro          NOMINAL  indi_mount     │   │  ← Device Row: 56px
│ │    RA 05h34m  +22°01′  │ 12ms  │ v2.1.4       │   │
│ └───────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────┤
│ CAMERAS                                              │
│ ┌───────────────────────────────────────────────┐   │
│ │ ●  ZWO ASI2600MM      NOMINAL  zwo_asi        │   │
│ │    −15.0° / −15.0°     │ 8ms   │ v3.2.0       │   │
│ └───────────────────────────────────────────────┘   │
│ ┌───────────────────────────────────────────────┐   │
│ │ ◌  Guide Camera       DISCONNECTED  —         │   │
│ │    —                   │ —     │ —             │   │
│ └───────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────┤
│ [more category sections]                             │
├─────────────────────────────────────────────────────┤
│ LAST COMMAND                              [▾ Expand] │  ← Log Section Header: 24px
│ 23:14:02  mount.slew_to_ra_dec  OK                  │
│ 23:13:58  camera.expose(300s)   OK                  │
│ 23:13:55  filter_wheel.set(Ha)  OK                  │
└─────────────────────────────────────────────────────┘
```

---

## Panel Header

**Height:** 40px  
**Background:** `color-bg-surface`  
**Border-bottom:** 1px solid `color-border-subtle`  
**Padding:** `0 space-4`

**Elements:**
- "DEVICES" label: `text-label-lg` (12px Space Grotesk 500 uppercase 0.04em), `color-text-muted`
- "Reconnect All" button:
  - Height: 28px, `padding: 0 space-2`
  - `border: 1px solid color-border-default`, `border-radius: radius-sm`
  - `text-label-sm` (11px 500), `color-text-secondary`
  - Background: transparent
  - Hover: `background: color-bg-elevated`, `border-color: color-border-strong`
  - Disabled (all devices connected): `opacity: 38%`, `pointer-events: none`
  - `aria-label="Reconnect all disconnected devices"`, `aria-disabled` when all connected

---

## Category Headers

**Height:** 24px  
**Background:** `color-bg-surface`  
**Border-bottom:** 1px solid `color-border-subtle`  
**Padding:** `0 space-4`  
**Text:** `text-label-lg` (12px 500 uppercase 0.04em), `color-text-muted`

Categories, in order:

1. MOUNTS
2. CAMERAS
3. FOCUSERS
4. FILTER WHEELS
5. ROTATORS
6. GUIDERS
7. DOME / ROOF
8. WEATHER
9. POWER / UPS

Categories with no devices configured are hidden entirely. Order is fixed regardless of device count.

---

## Device Row

**Height:** 56px fixed  
**Width:** 100% of panel  
**Background:** `color-bg-surface`  
**Border-bottom:** 1px solid `color-border-subtle`  
**Padding:** `0 space-4`  
**Interactive:** Yes — click/Enter → opens Device Detail Panel  
**Cursor:** `pointer`

### Row Layout

```
┌─────────────────────────────────────────────────────────┐   56px
│  ●  [Device Name]              [STATUS]  [Driver]       │   Row 1: 30px from top
│     [Telemetry]     │  [Latency]  │  [Version]          │   Row 2: 12px from Row 1 baseline
└─────────────────────────────────────────────────────────┘
 ↑
 Status dot: 10px, top-aligned to Row 1 text baseline, margin-right: space-2
```

### Row 1 Elements

**Status dot:**
- 10px CSS circle, `border-radius: radius-full`
- Colours: see Status table below
- DISCONNECTED state: circle is stroke-only (`border: 2px solid color-status-disconnected`, `background: transparent`)
- CONNECTING state: see Animation section

**Device name:**
- `text-heading-sm` (15px Space Grotesk 600), `color-text-primary`
- Max 20 characters, `text-overflow: ellipsis`
- DISCONNECTED: `color-text-secondary`

**Status badge:**
- `margin-left: auto` (right-aligned within row 1 flex container)
- `text-label-sm` (11px 500 uppercase 0.05em), colour matches dot
- No background, no border — text only

**Driver name:**
- `text-data-sm` (11px JetBrains Mono 400), `color-text-muted`
- `margin-left: space-2`
- DISCONNECTED or unknown: `—`
- Native driver: show driver identifier (e.g., `zwo_asi`, `native`)
- ASCOM driver: prefix with `ascom:` (e.g., `ascom:ASICamera2`)
- INDI driver: prefix with `indi:` (e.g., `indi:indi_eqmod`)

### Row 2 Elements (Telemetry Strip)

All Row 2 text: `text-data-sm` (11px JetBrains Mono 400)  
Separator between items: ` │ ` in `color-border-subtle`  
Disconnected state: all items `—`

**Per-device telemetry:**

| Device type | Row 2 content |
|---|---|
| Mount | `RA 05h34m  +22°01′` (RA + Dec abbreviated) |
| Camera (imaging) | `−15.0° / −15.0°` (current temp / target temp) |
| Camera (guide) | `uncooled` or `−5.0° / −5.0°` if cooled |
| Focuser | `Pos: 12450  Temp: 8.3°` |
| Filter Wheel | `Ha  [3/7]` (current filter name + slot position/total) |
| Rotator | `124.5°` (current angle) |
| Dome / Roof | `Az: 182°  OPEN` (azimuth + shutter state) |
| Weather Station | `Wind: 12km/h  Hum: 62%` |
| UPS / Power | `Mains: OK  Batt: 94%` |
| Guider | `RMS: 0.48″  GUIDING` |

**Latency:** `color-text-muted`
- Format: `12ms`
- < 50ms: `color-text-muted`
- 50–200ms: `color-status-degraded`
- > 200ms: `color-status-error`
- Unknown: `—`

**Driver version:** `color-text-muted`
- Format: `v2.1.4`
- Native driver: from plugin metadata
- ASCOM: from ASCOM driver info block
- INDI: from INDI driver info block
- Unavailable: `—`

### Status Colour Table

| Status | Dot fill | Badge colour | Row tint |
|---|---|---|---|
| NOMINAL | `color-status-nominal` | `color-status-nominal` | None |
| DEGRADED | `color-status-degraded` | `color-status-degraded` | `color-status-degraded-bg` (5% opacity) |
| DISCONNECTED | Stroke only `color-status-disconnected` | `color-status-disconnected` | None — row at 65% opacity |
| ERROR | `color-status-error` | `color-status-error` | `color-status-error-bg` (8% opacity) |
| CONNECTING | Animated (see below) | `color-status-degraded` text "CONNECTING" | None |

### Row States

| State | Visual |
|---|---|
| Default | As spec'd above |
| Hover | `background: color-interactive-subtle` |
| Focused | `outline: 2px solid color-border-strong; outline-offset: -2px` (inset) |
| Pressed | `background: color-bg-elevated`, `duration-instant` |
| Active (detail open) | Left accent bar: 2px × 32px `color-interactive-primary`, `border-radius: 0 radius-sm radius-sm 0`, `position: absolute; left: 0` |
| Recovery flash | Row bg → `color-status-nominal-bg` for 2s, then fade out `duration-slow` |

**Accessibility per row:**
- `role="button"`, `tabindex="0"`
- `aria-label="[Device name], [status]. Press Enter to view device controls."`
- `aria-expanded="true/false"` when device detail is open
- `aria-live="polite"` on status badge — announces status changes

---

## Last Command Log Section

**Header:** 24px, same as Category Header style, label "LAST COMMAND"  
**Expand button:** Phosphor `CaretDown` / `CaretUp` 14px, right-aligned, `color-text-muted`  
**Collapsed (default):** Shows last 3 commands  
**Expanded:** Shows last 20 commands, panel section scrollable

**Command row:**
- Height: 22px
- Format: `[HH:MM:SS]  [command.method(args)]  [result]`
- All: `text-data-sm` (11px JetBrains Mono), `padding: 0 space-4`
- Timestamp: `color-text-muted`
- Command: `color-text-secondary`
- Result OK: `color-status-nominal`
- Result ERR: `color-status-error`; entire row gets `color-status-error-bg` tint

---

## Device Detail Panel

Clicking a device row opens the Device Detail Panel. This is a contextual drawer that slides in from the right over the Device Dashboard, or replaces it on narrow viewports.

**Width:** 280px (same as parent panel — replaces it)  
**Animation:** slide in from right `translateX(280px → 0)`, `duration-normal`, `ease-enter`  
**Back button:** Phosphor `ArrowLeft` 18px + "Devices" label, top of panel, `color-text-secondary`

### Detail Panel Header

```
┌─────────────────────────────────────┐
│ ←  [Device Name]                    │  ← 48px
│     [STATUS]  [Driver]  [Latency]   │  ← 24px
├─────────────────────────────────────┤
│ ● NOMINAL  │  indi_mount  │  12ms   │
└─────────────────────────────────────┘
```

**Connect / Disconnect button:** right-aligned in header, 28px compact button  
- When disconnected: "Connect" — `color-interactive-primary` border and text  
- When connected: "Disconnect" — `color-text-muted` border, ghost style

### Control Sections (per device type)

All control sections follow the same pattern:
- Section label: `text-label-lg` uppercase, `color-text-muted`, `border-bottom: 1px solid color-border-subtle`
- Controls: standard form components (input, select, button, toggle) from the form component library (to be spec'd separately)
- Action buttons: full-width, 36px height, `border-radius: radius-md`

---

**MOUNT controls:**

| Control | Type | Notes |
|---|---|---|
| Slew to RA/Dec | Two inputs + button | RA (HH:MM:SS), Dec (±DD:MM:SS), [Slew] button |
| Slew to Alt/Az | Two inputs + button | Alt (±DD.d°), Az (DDD.d°), [Slew] button |
| Park | Button | Phosphor `MapPin` icon, confirms current park position |
| Unpark | Button | Only shown when parked |
| Sync | Button (destructive) | Syncs to current plate-solve result |
| Tracking rate | Select | Sidereal / Lunar / Solar / Custom |
| Abort | Button (error style) | Immediately halts all mount motion |

---

**CAMERA (imaging) controls:**

| Control | Type | Notes |
|---|---|---|
| Target temperature | Number input + [Set] | Degrees C, min −40, max +20 |
| Cooler on/off | Toggle | Labelled "Cooler" |
| Warm up camera | Button | Initiates gradual warm-up sequence |
| Gain | Number input | Range from driver capabilities |
| Offset | Number input | Range from driver capabilities |
| Binning | Segmented control | 1×1 / 2×2 / 3×3 / 4×4 |
| Full frame / subframe | Toggle + region inputs | Subframe: X, Y, W, H inputs |

---

**CAMERA (guide) controls:**

| Control | Type | Notes |
|---|---|---|
| Gain | Number input | |
| Binning | Segmented control | 1×1 / 2×2 |
| Exposure (guiding) | Number input | Seconds, used by guider |

---

**FOCUSER controls:**

| Control | Type | Notes |
|---|---|---|
| Current position | Display + manual input | Type a position and press Enter or [Go] |
| Step in / Step out | +/− buttons | Configurable step size |
| Step size | Number input | Steps per click |
| Halt | Button | Immediately stops focuser motion |
| Run autofocus | Button (primary) | Launches the autofocus routine from Focus pane |
| Temperature display | Read-only | If focuser reports temperature |

---

**FILTER WHEEL controls:**

| Control | Type | Notes |
|---|---|---|
| Filter slot selector | Grid of slot buttons | Shows slot number + filter name; active slot highlighted |
| Filter names | Inline editable labels | Click to rename each slot |
| Focus offset per filter | Number inputs per slot | Focus position delta in steps |
| Focus filter | Select | "Use for autofocus: [filter name]" — or "Current filter" |
| Calibrate wheel | Button | Runs wheel calibration routine |
| Slot count | Read-only | Reported by driver (5, 7, or custom) |

**Filter slot grid layout:**
```
┌─────┬─────┬─────┬─────┐
│  1  │  2  │  3  │  4  │
│  L  │  R  │  G  │  B  │
├─────┼─────┼─────┼─────┤
│  5  │  6  │  7  │     │
│ Ha  │OIII │ SII │  —  │
└─────┴─────┴─────┴─────┘
```
- Each slot: 48×40px button, `border-radius: radius-sm`
- Active slot: `background: color-interactive-primary`, white text
- Inactive slot: `background: color-bg-elevated`, `color-text-primary`
- Empty slot (< max): `background: color-bg-surface`, `border: 1px dashed color-border-subtle`, `color-text-muted`

---

**ROTATOR controls:**

| Control | Type | Notes |
|---|---|---|
| Current angle | Display | Degrees, 1 decimal |
| Target angle | Number input + [Rotate] | 0–360°, wraps |
| Reverse direction | Toggle | Inverts rotation direction |
| Sync angle | Button | Sets current position as a known angle |
| PA from plate solve | Button | Sets rotator to match current plate-solve PA |

---

**DOME / ROOF controls:**

| Control | Type | Notes |
|---|---|---|
| Shutter | Open / Close buttons | Confirmation required for Close |
| Azimuth | Number input + [Slew] | Degrees |
| Sync with mount | Toggle | Auto-tracks mount azimuth |
| Park dome | Button | |
| Slave mode | Toggle | Dome follows mount automatically |

---

**WEATHER STATION controls:**

| Control | Type | Notes |
|---|---|---|
| Live readings | Read-only grid | Wind, humidity, temp, dew point, rain, sky temp |
| Safety thresholds | Editable inputs | Mirrors safety config — links to Safety settings page |
| Data source | Read-only | Plugin name |
| Last updated | Timestamp | |

---

**UPS / POWER controls:**

| Control | Type | Notes |
|---|---|---|
| Mains status | Display | OK / On Battery / Fault |
| Battery level | Progress bar + percentage | |
| Estimated runtime | Display | HH:MM |
| Alarm threshold | Number input | % battery at which to trigger alert |

---

**GUIDER controls:**

| Control | Type | Notes |
|---|---|---|
| Guider state | Display | Idle / Calibrating / Guiding / Paused / Error |
| Start guiding | Button (primary) | |
| Stop guiding | Button | |
| Dither | Button | Manual dither trigger |
| Calibrate | Button | Runs guider calibration |
| RMS X / Y / Total | Read-only | Arcseconds |
| Guide algorithm | Select | PHD2/EKOS pass-through |

---

## Panel Scroll Behaviour

The panel body scrolls when device list + command log exceed panel height. The Panel Header and Command Log Header are sticky within the scroll container. Individual device rows and category headers scroll with the content.

---

## Animation Summary

| Element | Animation | Duration | Easing | Reduce Motion |
|---|---|---|---|---|
| Detail panel open | `translateX(280→0)` | `duration-normal` | `ease-enter` | Instant |
| Detail panel close | `translateX(0→280)` | `duration-normal` | `ease-exit` | Instant |
| Device panel toggle (App Shell) | `translateX(0↔280)` | `duration-normal` | `ease-standard` | Instant |
| CONNECTING dot pulse | Opacity 1→0.4→1 | 800ms linear ∞ | — | Static at 60% opacity |
| Recovery flash | Bg `color-status-nominal-bg` → fade out | 2000ms total, 250ms fade | `ease-standard` | No flash, instant |
| Row hover | Bg tint | `duration-fast` | `ease-standard` | Instant |

---

## Accessibility

**Panel:**
- `role="complementary"`, `aria-label="Device Status Panel"`
- Panel toggle button (in Titlebar): `aria-label="Device panel: open/closed"`, `aria-expanded`

**Device lists:**
- Each category: `role="group"` with `aria-labelledby` pointing to category header id
- Category header: `id="device-group-[category]"`, `role="heading"` `aria-level="3"`
- Device rows: `role="button"`, `tabindex="0"`, `aria-label="[Name], [status]. Press Enter for controls."`, `aria-expanded`
- Status changes: `aria-live="polite"` on status badge
- ERROR status changes: `aria-live="assertive"`

**Detail Panel:**
- Back button: `aria-label="Back to all devices"`
- On open: focus moves to panel header
- On close: focus returns to the device row that opened it
- All controls: standard `<label>` associations, no placeholder-only labels
- Destructive actions (Abort, Disconnect, Close dome): require confirmation — a 32px confirmation bar appears below the button: "Are you sure? [Confirm] [Cancel]"

**Filter slot grid:**
- `role="radiogroup"`, `aria-label="Filter slot selection"`
- Each slot: `role="radio"`, `aria-checked`, `aria-label="Slot [N]: [filter name]"`

---

## Sketch Symbol References

```
DeviceDashboard/Panel/Root
DeviceDashboard/PanelHeader/Default
DeviceDashboard/CategoryHeader/Default
DeviceDashboard/DeviceRow/Nominal
DeviceDashboard/DeviceRow/Degraded
DeviceDashboard/DeviceRow/Disconnected
DeviceDashboard/DeviceRow/Error
DeviceDashboard/DeviceRow/Connecting
DeviceDashboard/DeviceRow/ActiveDetail
DeviceDashboard/CommandLog/Collapsed
DeviceDashboard/CommandLog/Expanded
DeviceDashboard/DetailPanel/Header/Nominal
DeviceDashboard/DetailPanel/Header/Error
DeviceDashboard/DetailPanel/Header/Disconnected
DeviceDashboard/DetailPanel/Controls/Mount
DeviceDashboard/DetailPanel/Controls/Camera
DeviceDashboard/DetailPanel/Controls/Focuser
DeviceDashboard/DetailPanel/Controls/FilterWheel
DeviceDashboard/DetailPanel/Controls/Rotator
DeviceDashboard/DetailPanel/Controls/Dome
DeviceDashboard/DetailPanel/Controls/Weather
DeviceDashboard/DetailPanel/Controls/Power
DeviceDashboard/DetailPanel/Controls/Guider
DeviceDashboard/FilterSlotGrid/Default
DeviceDashboard/FilterSlotGrid/Slot/Active
DeviceDashboard/FilterSlotGrid/Slot/Inactive
DeviceDashboard/FilterSlotGrid/Slot/Empty
```

Sketch page: `Components → Section: Dashboard`

---

*Canvas sign-off required before Forge builds. Forge sign-off required before build starts.*  
*Canvas: ✅ Ready | Forge: ⬜ Pending*
