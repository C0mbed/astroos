# Component Contract: Safety Panel
**Version:** 1.0.0  
**Date:** 2026-04-04  
**Author:** Canvas (CombeCrew Design)  
**Status:** Ready for Build  

---

## Overview

The Safety Panel is a full-pane view within the Equipment section of the Nav Rail. It is the command centre for all three safety subsystems: hardware limits, Goodnight configuration, and site monitoring. It is also accessible as a modal overlay triggered by clicking Status Bar Zone A.

---

## Panel Layout

```
┌─────────────────────────────────────────────────────────────┐
│  Safety                                                      │  ← Pane header
├─────────────────────────────────────────────────────────────┤
│  ┌──────────────────────────────────────────────────────┐   │
│  │  CURRENT STATUS                                      │   │  ← Status card: always top
│  │  ● SAFE    Boltwood (ASCOM)  ·  Last update: 00:42   │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                              │
│  ▾ HARDWARE LIMITS          ─────────────────────────────   │  ← Collapsible sections
│  ▾ GOODNIGHT SEQUENCE       ─────────────────────────────   │
│  ▾ SITE MONITOR             ─────────────────────────────   │
│  ▾ MANUAL CONTROLS          ─────────────────────────────   │
└─────────────────────────────────────────────────────────────┘
```

Pane scrolls vertically. All four sections are expanded by default on first use, then remember their collapsed state per user preference.

---

## Section 1: Current Status Card

**Always expanded — cannot be collapsed.**  
**Background:** Colour-coded by state — `color-status-[state]-bg` at full opacity as a tinted card  
**Border:** 1px `color-status-[state]` at 30% opacity  
**Border-radius:** `radius-lg`  
**Padding:** `space-4`

```
┌─────────────────────────────────────────────────────────────┐
│  ●  SAFE                                                     │
│                                                              │
│  Source  Boltwood Cloud Sensor (ASCOM)                       │
│  Updated  2 minutes ago  ·  Poll: 60s                        │
│                                                              │
│  Cloud cover: Clear  ·  Wind: 8 km/h  ·  Humidity: 54%      │
└─────────────────────────────────────────────────────────────┘
```

**When OVERRIDE is active:**
```
┌─────────────────────────────────────────────────────────────┐
│  ⚠  OVERRIDE ACTIVE                                         │
│                                                              │
│  You have overridden: West pier HA limit (3.5h)              │
│  Acknowledged: 23:14:02  ·  Expires: end of sequence        │
│                                                              │
│                                  [End Override]              │
└─────────────────────────────────────────────────────────────┘
```

**When weather fallback is active (no hardware source):**
- Source line shows: `~ Weather forecast (Astrospheric)` with a `~` prefix in `color-status-degraded`
- A callout below: "No safety device configured. Decisions based on weather forecast — less reliable than a real-time sensor."
- Callout background: `color-status-degraded-bg`, border: `color-status-degraded` at 20%

Elements:

| Element | Spec |
|---|---|
| Status dot | 12px, `color-status-[state]` |
| Status label | `text-display-sm` (24px Space Grotesk 600), `color-status-[state]` |
| Source label | `text-body-sm` (13px), `color-text-secondary` |
| Timestamp | `text-caption` (11px), `color-text-muted` |
| Readings strip | `text-data-sm` (11px JetBrains Mono), `color-text-data` |
| "End Override" button | Ghost button, 28px compact, `color-status-error` border + text |

---

## Section 2: Hardware Limits

**Section header:** collapsible, `text-label-lg` uppercase, Phosphor `CaretDown/Up` 14px

### 2a. Pier Flip Limits

```
┌─────────────────────────────────────────────────────────────┐
│  Pier Flip Limits                                            │
│                                                              │
│  East limit    ┌──────────┐  hours    [From driver: 3.0h]   │
│                │  3.0     │                                  │
│                └──────────┘                                  │
│  West limit    ┌──────────┐  hours    [From driver: 3.5h]   │
│                │  3.5     │                                  │
│                └──────────┘                                  │
│                                                              │
│  ⚠ Overriding pier limits may cause scope-to-pier           │
│    collision. Override at your own risk.                     │
└─────────────────────────────────────────────────────────────┘
```

**Driver badge:** `text-label-sm`, `color-text-muted`, `background: color-bg-elevated`, `border: 1px solid color-border-subtle`, `border-radius: radius-sm`, `padding: 2px space-2`  
- Shows when a driver value is available
- Label: "From driver: [value]h"
- If user's input matches driver value exactly: badge shows in `color-status-nominal` — they match
- If user's input differs from driver value: badge shows in `color-status-degraded` + text "Driver: [value]h — you have overridden this"

**Input fields:** standard number input, `step: 0.1`, range `0.1–12.0`  
**Unit label:** `text-body-sm`, `color-text-muted`, `margin-left: space-2`  
**Warning callout:** `color-status-degraded-bg` tinted box, `border-left: 2px solid color-status-degraded`, `border-radius: 0 radius-sm radius-sm 0`, Phosphor `Warning` 14px Bold, `text-body-sm`

### 2b. Altitude Limits

```
Minimum altitude    [  15  ]  °    ☑ Enforce
Maximum altitude    [  89  ]  °    ☐ Enforce (zenith blind spot)
```

**Toggle:** standard checkbox-style toggle  
**Input:** number input, `step: 1`, min/max validated  

### 2c. Horizon Mask

```
Horizon mask    ☑ Enabled    [Edit Mask ↗]    [Import .hrz / .csv ↗]

Preview: [miniature polar plot — 120×60px read-only thumbnail]
         [N] [90°]
              |
   [W]────────+────────[E]
              |
             [S]
Points: 24  ·  Min alt at current Az: 18°
```

**Edit Mask button:** opens Horizon Mask Editor modal (see below)  
**Import button:** opens file picker, accepts `.hrz` and `.csv`  
**Mini polar plot:** SVG, read-only, shows the horizon profile as a filled area below the mask line. `color-status-error-bg` fill for the "blocked" zone below the mask line. `color-interactive-primary` stroke for the mask line itself.

---

## Horizon Mask Editor Modal

**Size:** 720×560px modal, `elevation-4`  
**Title:** "Horizon Mask Editor"

```
┌─────────────────────────────────────────────────────────────────┐
│  Horizon Mask Editor                               [✕ Close]    │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│   90° ─────────────────────────────────────────────────────     │
│        |                                                         │
│   60° ─┤                                                         │
│        |                                                         │
│   30° ─┤         ·─────·                ·───·                   │  ← mask line
│        |    ·───·       ·──────────────·     ·────·             │
│   15° ─┤···                                       ·············  │  ← default
│        |                                                         │
│    0° ─────────────────────────────────────────────────────     │
│        N    45°   E    135°   S    225°   W    315°    N        │
│                                                                  │
│  Click to add points  ·  Drag points to move  ·  Right-click to remove  │
│                                                                  │
│  [Use flat horizon: 15°]     Points: 12          [Clear all]     │
├─────────────────────────────────────────────────────────────────┤
│  [Cancel]                                          [Save Mask]   │
└─────────────────────────────────────────────────────────────────┘
```

**Chart area:** SVG, interactive  
- Click empty area: add point at that azimuth/altitude
- Click + drag existing point: move it
- Right-click existing point: remove it
- Line: `color-interactive-primary`, 2px stroke, linear interpolation between points
- Blocked zone (below mask): `color-status-error` at 8% opacity fill
- Safe zone (above mask): `color-status-nominal` at 4% opacity fill
- Grid lines: `color-border-subtle`, labeled
- Current scope position overlay: if mount is connected, a crosshair marker shows the scope's current Az/Alt position in real time

**"Use flat horizon" button:** sets two anchor points at the specified altitude across all azimuths. Label dynamically shows the current minimum altitude setting.

**Point tooltip (hover):** shows `Az: [value]°  Alt: [value]°` in a small tooltip above the point

**Save / Cancel:** Save overwrites the stored mask, triggers confirmation if mask has points below 10° (low-altitude imaging is genuinely risky)

---

## Section 3: Goodnight Configuration

### Trigger Conditions

```
  ☑  Target altitude drops below    [  20  ] °
  ☑  Dawn approaching within        [  30  ] min
  ☑  Safety system signals UNSAFE
  ☑  Site monitor goes silent        [   2  ] min grace period
  ☐  Sequence completes (always do Goodnight after sequence)
```

Each row: `text-body-sm`, checkbox toggle, conditional number input, unit label

### Goodnight Sequence Steps

```
  GOODNIGHT SEQUENCE

  ┌──────────────────────────────────────────────┐
  │  1  Finish or abort exposure        [always] │  ← locked
  │  2  Warm camera            ☑  Rate: [2] °/min│
  │  3  Park mount             ☑               │
  │  4  Close flat panel       ☐  (not configured)│  ← greyed
  │  5  Close dome / roof      ☐  (not configured)│  ← greyed
  │  6  Disconnect devices     ☑               │
  │  7  Rest state             ☑  [Stay running ▾]│
  │  8  Custom script          ☐  [Browse...]   │
  └──────────────────────────────────────────────┘

  [Test Goodnight]          [Run Goodnight Now]
```

**Step rows:**
- Height: 44px
- Step number: `text-label-sm`, `color-text-muted`, 24px wide
- Step name: `text-body-sm`, `color-text-primary`
- Toggle: standard toggle switch, right-aligned
- Inline config: appears when step is enabled and configurable (e.g., warm rate input, rest state select)
- Locked steps: no toggle, `[always]` badge in `color-text-muted`
- Greyed steps (device not configured): 38% opacity, `color-text-muted` text, no toggle, explanation in parentheses

**Step 7 — Rest state select:**
Options: "Stay running (session complete screen)" / "Return to idle" / "Shut down PC after [10] min"  
PC shutdown option: shows a yellow callout warning when selected: "PC will shut down after the delay. Ensure all data is saved."

**"Test Goodnight" button:** ghost button, runs dry-run simulation, opens a read-only version of the Goodnight Overlay showing estimated timings without executing any commands

**"Run Goodnight Now" button:** primary destructive button (error style border), full width, opens a confirmation modal before executing

---

## Section 4: Site Monitor

```
  Safety source    [Boltwood Cloud Sensor (ASCOM) ▾]

  ASCOM driver     [Boltwood.CloudSensor          ]
  Device index     [0                             ]
  Poll interval    [60] seconds

  [Test Connection]

  Status
  ● Connected  ·  Last response: 2 min ago  ·  Reported: SAFE
  Consecutive failures: 0

  ─────────────────────────────────────────
  Fallback    ☑  Use weather forecast if source unavailable
              Threshold: Cloud < [40]  Wind < [40] km/h
                         Humidity < [85] %  Precip < [20] %
```

**Source selector options:** None / Safe-file / ASCOM ObservingConditions / HTTP Endpoint / INDI weather  
**Conditional fields:** shown/hidden based on source selection  
**Test Connection button:** fires a single poll immediately, shows result inline  
**Status block:** live polling state in `text-data-sm` mono  

---

## Section 5: Manual Controls

One "Run now" button per Goodnight step, listed vertically. Device connection state shown inline.

```
  MANUAL CONTROLS

  Warm camera          ● Connected    [Warm camera now]
  Park mount           ● Connected    [Park now]
  Close flat panel     ○ Not configured  —
  Close dome / roof    ○ Not configured  —
  Disconnect all       —              [Disconnect all]
```

Row height: 40px  
Device state dot: 8px, `color-status-nominal` / `color-status-disconnected`  
Button: compact 28px ghost button  
Not configured: `color-text-muted`, button hidden  

Each "Run now" button sends the command directly to the device — it does not go through the Goodnight sequence engine. If the device is not connected, button is disabled with tooltip "Device not connected."

---

## Override Acknowledgement Modal

Reusable pattern — used anywhere in the app a safety limit is bypassed.

**Size:** 480px wide, variable height, centred, `elevation-4`  
**Cannot be dismissed:** no click-outside dismissal, no `Esc` key dismissal  
**Backdrop:** `color-bg-overlay`

```
┌───────────────────────────────────────────────────────┐
│                                                       │
│  ⚠  Safety Limit Override                            │
│                                                       │
│  You are about to override a safety limit.            │
│                                                       │
│  Limit       West pier hour angle                     │
│  Configured  3.5 hours                                │
│  Current     3.8 hours (0.3h beyond limit)            │
│  Risk        Scope-to-pier collision is possible      │
│                                                       │
│  ───────────────────────────────────────────────────  │
│                                                       │
│  Overriding safety limits is done entirely at your    │
│  own risk. Dark Sky accepts no responsibility for     │
│  equipment damage resulting from safety overrides.    │
│                                                       │
│  [Cancel]              [I understand — override ▶]   │
│                         (available in 2s)             │
│                                                       │
└───────────────────────────────────────────────────────┘
```

**Title:** `text-heading-lg` (18px 600), `color-text-primary`  
**⚠ icon:** Phosphor `Warning` Bold 24px, `color-status-error`, `margin-bottom: space-3`  
**Limit / Current / Risk rows:** two-column layout, label `text-body-sm color-text-muted`, value `text-body-sm color-text-primary`  
**Risk value:** `color-status-error`  
**Divider:** 1px `color-border-subtle`  
**Disclaimer text:** `text-body-sm` (13px), `color-text-secondary`, italic  
**"Cancel" button:** ghost, left-aligned  
**"I understand — override" button:**
- Initial state (0–2s after modal opens): disabled, 38% opacity, label shows "(available in 2s)"
- After 2s: enabled, `border: 1px solid color-status-error`, `color: color-status-error`, font-weight 600
- The 2-second delay is intentional and non-bypassable — it prevents accidental click-through
- On click: modal closes, override is logged, platform proceeds

**Reduce Motion:** 2-second delay still applies (it is not an animation — it is an intentional friction mechanism)

---

## Goodnight Sequence Overlay

Triggered automatically or manually when Goodnight begins. Full-screen overlay, cannot be dismissed during execution (can be cancelled with confirmation).

**Size:** Full viewport overlay  
**Background:** `color-bg-overlay` (deep)  
**Z-index:** maximum — above everything including modals

```
┌─────────────────────────────────────────────────────────────────┐
│                                                                  │
│                      Good night.                                 │  ← 32px Space Grotesk 600
│              Session complete — 247 frames captured              │  ← 14px secondary
│                                                                  │
│  ──────────────────────────────────────────────────────────────  │
│                                                                  │
│   ✓  Exposure finished          00:47 remaining → completed      │
│   ●  Warming camera             −4.2° → target −0.5° or above   │  ← active step
│   ○  Park mount                 Waiting                          │
│   ○  Close flat panel           Waiting                          │
│   ○  Disconnect devices         Waiting                          │
│   ○  Rest                       Waiting                          │
│                                                                  │
│  ──────────────────────────────────────────────────────────────  │
│                                                                  │
│  Triggered by: Target below minimum altitude (18° < 20°)        │
│  Estimated time remaining: ~12 minutes                           │
│                                                                  │
│                       [Cancel Goodnight]                         │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

**"Good night." heading:** `text-display-lg` (32px 600), `color-text-primary`, centred  
**Subtitle:** `text-body-lg` (14px), `color-text-secondary`, centred — shows frame count if available, "Shutting down" if no session data  
**Step list:**  
- Completed step: Phosphor `CheckCircle` Fill 16px `color-status-nominal`, `text-body-sm color-text-secondary`
- Active step: Phosphor `Circle` Bold animated pulse 16px `color-interactive-primary`, `text-body-sm color-text-primary`, progress detail to the right
- Pending step: Phosphor `Circle` Regular 16px `color-text-muted`, `text-body-sm color-text-muted`
- Failed step: Phosphor `XCircle` Fill 16px `color-status-error`, `text-body-sm color-status-error`

**Active step progress detail:** `text-data-sm` mono, `color-text-muted`, right-aligned — shows live telemetry relevant to the active step (camera temp ticking up, mount slewing to park, etc.)

**Trigger explanation:** `text-body-sm`, `color-text-muted`, centred  
**Estimated time:** `text-body-sm`, `color-text-muted`, centred  

**Cancel button:**
- Ghost button, centred, `color-text-muted` border + text
- Hover: `color-status-error` border + text
- On click: confirmation modal — "Cancel Goodnight? The mount and dome may be left in an unsafe state. [Keep going] [Cancel sequence]"
- Cancel is unavailable once Step 5 (Close dome/roof) is in progress — shown as disabled with tooltip "Dome closing — cannot cancel"

**On completion (all steps done):**
- Step list remains visible with all checkmarks
- Heading changes to "Session complete." (or remains "Good night." if no session was running)
- Subtitle shows final stats if available: frames captured, session duration
- Cancel button replaced with "Close" button
- If rest state = PC shutdown: a countdown appears — "Shutting down in [N]:00 [Cancel shutdown]"
- Night Mode: overlay uses night-mode palette — deep red background, warm text — gives the end-of-night moment atmosphere appropriate to the experience

**Reduce Motion:** Step icon pulse is static at 60% opacity. No other animations in this overlay.

---

## Accessibility

**Safety Panel:**
- `role="main"` within the pane structure
- Section headers: `role="heading"` `aria-level="2"`, with `aria-expanded` for collapsible sections
- Toggle controls: `role="switch"`, `aria-checked`, `aria-label="[step name]: enabled/disabled"`
- Hardware limit inputs: `<label>` associations, `aria-describedby` for warning callouts
- Status card: `aria-live="polite"`, announces safety state changes

**Override Acknowledgement Modal:**
- `role="alertdialog"`, `aria-modal="true"`, `aria-labelledby` → title
- On open: focus moves to modal, trapped inside until dismissed
- "I understand" button: `aria-disabled="true"` for first 2s, then `aria-disabled="false"` — screen reader announces "Override available" when button activates
- Cancel: always focusable from modal open

**Goodnight Overlay:**
- `role="dialog"`, `aria-modal="true"`, `aria-label="Goodnight sequence in progress"`
- `aria-live="polite"` on step list — announces step state changes
- Active step: `aria-label="[step name]: in progress, [detail]"`
- Completed: `aria-label="[step name]: complete"`
- Failed: `aria-live="assertive"` — failure is announced immediately
- Cancel button: `aria-label="Cancel Goodnight sequence"`

---

## Sketch Symbol References

```
SafetyPanel/StatusCard/Safe
SafetyPanel/StatusCard/Caution
SafetyPanel/StatusCard/Unsafe
SafetyPanel/StatusCard/Override
SafetyPanel/StatusCard/WeatherFallback
SafetyPanel/HardwareLimits/PierFlip
SafetyPanel/HardwareLimits/Altitude
SafetyPanel/HardwareLimits/HorizonMask
SafetyPanel/HorizonMaskEditor/Default
SafetyPanel/GoodnightConfig/StepRow/Enabled
SafetyPanel/GoodnightConfig/StepRow/Disabled
SafetyPanel/GoodnightConfig/StepRow/Locked
SafetyPanel/GoodnightConfig/StepRow/NotConfigured
SafetyPanel/SiteMonitor/Default
SafetyPanel/ManualControls/Default
OverrideAcknowledgement/Locked
OverrideAcknowledgement/Available
GoodnightOverlay/Active
GoodnightOverlay/Complete
GoodnightOverlay/Failed
GoodnightOverlay/NightMode
```

Sketch page: `Components → Section: Safety`

---

*Canvas: ✅ Ready | Forge: ⬜ Pending sign-off*  
*Pending Matt's answers: Goodnight sequence configurability (Q3), rest state / PC shutdown (Q5)*  
*These affect Step 7 and Step 8 of the sequence spec only — all other sections are final.*
