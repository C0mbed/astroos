# Component Contract: Settings → Safety

**Version:** 1.1.0
**Date:** 2026-04-09
**Author:** Canvas
**Status:** Canvas ✅ Matt ✅ — Forge sign-off needed

> **Version note:** This contract is v1.1.0 in the safety design sequence.
> Safety Panel Contract v1.0.0 (OI-001) identified Settings → Safety as a
> required companion deliverable before override states 6.7 and 6.8 could be
> built. This contract resolves that open item.

---

## Pipeline References

- **Resolves:** Safety Panel v1.0.0 — OI-001 (Settings → Safety screen contract)
- **Blocks:** Safety Panel v1.0.0 — override states 6.7 (Override Active) and 6.8 (Override Requested)
- **Relates to:** Safety Panel Contract v1.0.0 (signed), Brand System v1.0.0
- **Forge build target:** `DarkSky.App / Views / Settings / SafetySettingsView.xaml`

---

## 1. Screen Purpose & Context

Settings → Safety is a **settings content screen** — not a modal, not an overlay. It occupies the main content area of the Settings shell when the user navigates to the Safety category in the left-rail navigation.

This screen does two distinct jobs:

1. **Configure** all safety parameters: pier flip limits, altitude limits, horizon mask, safety monitoring sources, and Goodnight sequence steps.
2. **Gate** override capability — the one-time acknowledgment that unlocks the override controls in the Safety Panel. This is the only place in the app where override capability can be enabled. The Safety Panel's override states (6.7, 6.8) are inaccessible until this screen's toggle has been confirmed.

**Design principle:** This screen must be calm and trustworthy. It deals with real equipment risk. No decorative noise. Every element earns its place.

---

## 2. Settings IA — Navigation Context

Settings uses a **left-rail category navigation + content area** layout.

Left-rail categories (canonical order):

```
General
Site & Observatory
Safety           ← active (this screen)
Equipment
Sequencer
AI Advisor
Appearance
About
```

---

## 3. Screen Anatomy

```
┌─────────────────────────────────────────────────────────────────────┐
│  Settings                                                           │
├───────────────┬─────────────────────────────────────────────────────┤
│               │  Safety                                             │
│  General      │  ─────────────────────────────────────────────────  │
│  Site &       │                                                     │
│  Observatory  │  ┌─ SECTION: Safety Limits ──────────────────────┐  │
│               │  │  Pier Flip Management                         │  │
│  ▶ Safety     │  │  Altitude Limits                              │  │
│               │  │  Horizon Mask                                 │  │
│  Equipment    │  └───────────────────────────────────────────────┘  │
│  Sequencer    │                                                     │
│  AI Advisor   │  ┌─ SECTION: Safety Monitoring ──────────────────┐  │
│  Appearance   │  │  Source type selector                         │  │
│  About        │  │  Source configuration (contextual)            │  │
│               │  │  Unsafe threshold + auto-response             │  │
│               │  └───────────────────────────────────────────────┘  │
│               │                                                     │
│               │  ┌─ SECTION: Goodnight Sequence ─────────────────┐  │
│               │  │  Ordered step list (enable/disable per step)  │  │
│               │  │  Post-sequence script (optional)              │  │
│               │  └───────────────────────────────────────────────┘  │
│               │                                                     │
│               │  ┌─ SECTION: Override Capability ────────────────┐  │
│               │  │  Toggle + description + acknowledgment gate   │  │
│               │  └───────────────────────────────────────────────┘  │
│               │                                                     │
└───────────────┴─────────────────────────────────────────────────────┘
```

---

## 4. Layout Dimensions

| Element                        | Spec                                                           |
|--------------------------------|----------------------------------------------------------------|
| Left-rail navigation width     | 220px (fixed) — Settings Shell contract                        |
| Content area left padding      | space-8 (32px)                                                 |
| Content area right padding     | space-8 (32px)                                                 |
| Content area top padding       | space-6 (24px) below screen title                              |
| Content area max width         | 720px (content does not stretch beyond this at wide viewports) |
| Screen title bottom margin     | space-6 (24px)                                                 |
| Section gap (between sections) | space-8 (32px)                                                 |
| Section header bottom margin   | space-4 (16px)                                                 |
| Section internal row gap       | space-3 (12px)                                                 |

---

## 5. Section Specs

### 5.1 Section: Safety Limits

**Section header:** "Safety Limits" — text-heading-sm / color-text-primary

#### 5.1.1 Pier Flip Management

**Label:** "Pier Flip Management"
**Sublabel:** "Controls when your mount flips across the meridian."

**Default mode: Driver Managed**

A radio-group of two options:

```
(●) Driver managed       Let your mount driver control flip timing.
( ) Custom limits        Set your own hour angle limits.
```

When **Driver Managed** is selected: no additional fields shown. Informational note (text-caption / color-text-secondary): "Dark Sky reads flip limits from your mount driver. Change them in your mount's ASCOM driver settings."

When **Custom Limits** is selected, the following fields appear (animated expand, duration-normal / ease-decelerate):

**Field: Flip past meridian**

- Label: "Flip past meridian (hours)"
- Input type: Numeric spinner — JetBrains Mono, 14px
- Range: 0.0 – 3.0 h, step 0.1 / Default: 0.5 / Width: 120px fixed
- Trailing unit label: "h" — text-label / color-text-secondary

**Field: Safety buffer before flip**

- Label: "Safety buffer before flip (minutes)"
- Input type: Numeric spinner / Range: 0–60 min, step 5 / Default: 10 / Width: 120px fixed

**Field: Enforce limits as hard stop**

- Label: "Enforce limits as hard stop" / Input type: Toggle (right-aligned) / Default: ON
- Help text: "When on, Dark Sky halts the sequence if the flip limit is reached. When off, it warns only."

#### 5.1.2 Altitude Limits

**Field: Minimum altitude**

- Label: "Minimum altitude" / Range: 0–90°, step 1 / Default: 15 / Width: 100px / Trailing unit: "°"

**Field: Pre-sequence altitude check**

- Label: "Check target altitude before sequence starts" / Toggle / Default: ON

#### 5.1.3 Horizon Mask

**State A — No mask defined:**

```
┌─────────────────────────────────────────────────────┐
│  No horizon mask defined                            │
│  Without a mask, the minimum altitude limit         │
│  applies uniformly in all directions.               │
│                                                     │
│  [  Open Mask Editor  ]                             │
└─────────────────────────────────────────────────────┘
```

**State B — Mask defined:**

```
┌─────────────────────────────────────────────────────┐
│  [Horizon mask thumbnail — 80×40px polar preview]  │
│  Observatory East Ridge              [Edit Mask →]  │
│  Last updated: 2026-04-09                           │
└─────────────────────────────────────────────────────┘
```

---

### 5.2 Section: Safety Monitoring

**Section header:** "Safety Monitoring"

#### 5.2.1 Monitoring Source

Source type selector — segmented control (5 options):

```
[ ASCOM ] [ Safe File ] [ HTTP ] [ INDI ] [ Forecast ]
```

**Source: ASCOM ObservingConditions** (default) — Driver selector + Test Connection button

**Source: Safe File** — File path input + Browse button + Safe value field + Poll interval

**Source: HTTP Endpoint** — URL + Safe value + Poll interval + Test Connection

**Source: INDI Weather** — Server host + Port (default 7624) + Driver name + Test Connection

**Source: Weather Forecast Fallback** — Informational note + Unsafe above cloud cover % spinner

#### 5.2.2 Unsafe Response

Radio group:

```
(●) Pause sequence and alert
    The sequence pauses. Dark Sky alerts you in the Log Strip and
    Safety Panel. Resume manually when conditions improve.

( ) Trigger Goodnight
    Immediately begins the Goodnight sequence.

( ) Alert only — do not interrupt
    Dark Sky logs the condition and shows an alert. The sequence
    continues running. Use with caution.
```

**Field: Monitoring active during (checkbox group)**

- [ x ] Sequence running
- [ x ] Sequence paused
- [   ] App open, no sequence running

---

### 5.3 Section: Goodnight Sequence

**Section header:** "Goodnight Sequence"

#### 5.3.1 Step List

Fixed-order vertical list of toggleable rows:

```
┌─────────────────────────────────────────────────────┐
│  Step  Description                          Enabled │
├─────────────────────────────────────────────────────┤
│  1     Stop active sequence                   [ ON] │
│  2     Save session log                       [ ON] │
│  3     Warm camera to ambient (cool off)      [ ON] │
│  4     Park mount                             [ ON] │
│  5     Close dome / roll-off roof             [OFF] │
│  6     Close rotator to 0°                    [OFF] │
│  7     Disconnect all devices                 [ ON] │
└─────────────────────────────────────────────────────┘
```

- Steps 1 and 7 are **locked** — their toggles render ON and are non-interactive.
- Locked step tooltip on hover: "This step is required and cannot be skipped."
- Row height: 44px / Toggle: 40×20px WinUI 3 ToggleSwitch

#### 5.3.2 Post-Sequence Script

Text input + "Browse…" button (filters .bat .ps1 .exe .py)

Warning note (shown when a path is set):

```
⚠  Dark Sky runs this script with your user permissions.
   Verify the script is safe before use.
```

#### 5.3.3 PC Shutdown

**Locked decision:** The app stays running after Goodnight completes. PC shutdown is user-initiated only. There is no PC shutdown option in this screen.

---

### 5.4 Section: Override Capability

**Section header:** "Override Capability"

This section is visually distinct — 1px top border in color-border-default with space-8 (32px) above.

#### 5.4.1 Override Toggle Row

```
┌─────────────────────────────────────────────────────────┐
│  Allow safety overrides in Safety Panel          [OFF]  │
│                                                         │
│  When enabled, override controls are always             │
│  visible in the Safety Panel. They remain locked        │
│  until an active session is in progress.                │
└─────────────────────────────────────────────────────────┘
```

- Container: color-background-secondary / border 1px color-border-default / radius-md / padding space-6 (24px)
- Default state: OFF

#### 5.4.2 Toggle Interaction — First Enable (Acknowledgment Gate)

When the user toggles ON for the **first time**:

1. Toggle moves to ON — **immediately**
2. Acknowledgment modal appears (150ms delay)
3. Cancel → toggle snaps back to OFF
4. Confirm → acknowledgment stored in SQLite → toggle stays ON

**Subsequent enables:** Toggle flips ON directly. No modal.

**Disabling:** Toggle flips OFF directly. No modal. `overrideCapabilityAcknowledged` remains true.

#### 5.4.3 Acknowledgment Modal Spec

**Modal title:** "Enable Safety Overrides" / **Width:** 480px fixed

**Body copy:**

> This lets you override pier flip, altitude, and horizon limits from the Safety Panel during a session.
>
> Safety limits protect your equipment. Overriding them is sometimes the right call — meridian delays, unusual target geometry, time-critical sequences — but the risk transfers to you.
>
> You won't be asked again.

**Buttons:**

- Secondary: "Cancel"
- Primary destructive: "I understand — Enable Overrides" — `color-status-warning-accessible` (#B45309 light / #F59E0B dark) background, #FFFFFF label light / #0A0A0A label dark

**Dismiss:** Click outside or Escape = Cancel (toggle snaps OFF). Modal cannot be dismissed without an explicit choice.

#### 5.4.4 Override Enabled State

```
┌─────────────────────────────────────────────────────────────┐
│  ⚠  Override capability active                              │
│  ─────────────────────────────────────────────────────────  │
│  Allow safety overrides in Safety Panel              [ON]   │
└─────────────────────────────────────────────────────────────┘
```

Container border shifts to 1px color-status-warning. "⚠ Override capability active" badge appears with color-status-warning at 15% opacity background.

---

## 6. Full State Inventory

| State                             | Description                                                                          |
|-----------------------------------|--------------------------------------------------------------------------------------|
| Default — unconfigured            | All sections at defaults. No source configured. No mask. Override OFF.               |
| Pier flip: Driver Managed         | Default, no custom fields                                                            |
| Pier flip: Custom Limits          | Fields expanded                                                                      |
| Horizon Mask: None                | Empty state with CTA                                                                 |
| Horizon Mask: Defined             | Preview thumbnail + Edit link                                                        |
| Monitoring source: ASCOM/SafeFile/HTTP/INDI/Forecast | Respective fields shown                                           |
| Test Connection: Testing/Connected/Failed | Respective button state                                                      |
| Goodnight: step enabled/disabled  | Toggle ON/OFF                                                                        |
| Override: OFF                     | Container default                                                                    |
| Override: Acknowledgment modal    | Modal over screen, scrim behind                                                      |
| Override: ON                      | Container amber border, badge shown                                                  |

---

## 7. Typography

| Element                      | Token           | Size | Weight |
|------------------------------|-----------------|------|--------|
| Screen title "Safety"        | text-heading-lg | 24px | 600    |
| Section headers              | text-heading-sm | 18px | 600    |
| Field labels                 | text-body-sm    | 14px | 600    |
| Field help text              | text-caption    | 11px | 400    |
| Field values                 | text-body-sm    | 14px | 400    |
| Numeric/path inputs          | text-mono       | 13px | 400    |
| Step descriptions            | text-body-sm    | 14px | 400    |
| Modal title                  | text-heading-sm | 18px | 600    |
| Modal body                   | text-body-sm    | 14px | 400    |

---

## 8. Color Tokens

| Element                       | Light Mode                 | Dark Mode                  |
|-------------------------------|----------------------------|----------------------------|
| Screen background             | color-background-primary   | color-background-primary   |
| Input background              | color-background-secondary | color-background-secondary |
| Input border (default)        | color-border-default       | color-border-default       |
| Input border (focused)        | color-interactive-primary  | color-interactive-primary  |
| Override section border (OFF) | color-border-default       | color-border-default       |
| Override section border (ON)  | color-status-warning       | color-status-warning       |
| Override badge bg             | color-status-warning @ 15% | color-status-warning @ 15% |
| Override badge text           | color-status-warning       | color-status-warning       |
| Confirm button bg             | color-status-warning-accessible (#B45309) | color-status-warning-accessible (#F59E0B) |
| Confirm button text           | #FFFFFF                    | #0A0A0A                    |

---

## 9. Accessibility

| Requirement                  | Implementation                                                                                                        |
|------------------------------|-----------------------------------------------------------------------------------------------------------------------|
| Screen region                | `AutomationProperties.Name="Safety Settings"`                                                                         |
| Acknowledgment modal         | Focus traps inside modal on open. First focus: "Cancel". Escape = Cancel.                                             |
| Locked step toggles          | `IsEnabled="False"`. `AutomationProperties.HelpText` = "This step is required and cannot be skipped."                |
| Confirmation button          | Name: "Enable safety overrides, I understand"                                                                         |
| Contrast — confirm button    | White (#FFFFFF) on #B45309: 4.7:1 ✅ AA                                                                               |

---

## 10. Motion

| Interaction            | Spec                                                                               |
|------------------------|------------------------------------------------------------------------------------|
| Custom limits expand   | height 0→auto + opacity 0→1, 250ms, ease-decelerate                                |
| Custom limits collapse | height auto→0 + opacity 1→0, 150ms, ease-accelerate                                |
| Modal entrance         | scale(0.95)→scale(1.0) + opacity 0→1, 250ms, ease-decelerate                       |
| Modal exit (cancel)    | scale(1.0)→scale(0.95) + opacity 1→0, 150ms, ease-accelerate                       |
| Modal exit (confirm)   | opacity 1→0 only, 150ms, ease-standard                                              |
| Override border        | color transition color-border-default → color-status-warning, 250ms                |
| Reduce Motion          | All transitions → instant. Fades only at duration-instant (80ms).                  |

---

## 11. Open Items for Matt

| ID     | Item                                                                                                    | Status                                |
|--------|---------------------------------------------------------------------------------------------------------|---------------------------------------|
| SS-001 | Goodnight step order and configurability: fixed order + per-step enable/disable + optional script hook. | ✅ Resolved — Matt approved 2026-04-09 |
| SS-002 | `color-status-warning-accessible` = #B45309 light / #F59E0B dark. Brand System amendment v1.1.0.      | ✅ Resolved — Matt approved 2026-04-09 |

---

## 12. Forge Notes

- **SQLite fields required:** `settings.overrideCapabilityEnabled` (bool, default false), `settings.overrideCapabilityAcknowledged` (bool, default false), `settings.pierFlipMode` (enum), `settings.pierFlipHours` (float), `settings.pierFlipBufferMinutes` (int), `settings.pierFlipHardStop` (bool), `settings.altitudeMinimum` (int), `settings.preSequenceAltitudeCheck` (bool), `settings.horizonMaskPath` (string nullable), `settings.monitoringSource` (enum), `settings.monitoringUnsafeResponse` (enum), Goodnight step enable flags, `settings.postSequenceScriptPath` (string nullable).
- Safety Panel states 6.7 and 6.8 check `settings.overrideCapabilityEnabled = true` before rendering override controls.
- Acknowledgment modal should be a WinUI 3 `ContentDialog`.
- Elevation-3 assumption for Safety Panel drawer shell (flagged by Forge in v1_5) is confirmed by Canvas.

---

## 13. Sign-Off

| Role   | Status                                  | Date       |
|--------|-----------------------------------------|------------|
| Canvas | ✅ Approved — v1.1.0                     | 2026-04-09 |
| Forge  | ⬜ Pending                               | —          |
| Matt   | ✅ Approved — SS-001 and SS-002 resolved | 2026-04-09 |
