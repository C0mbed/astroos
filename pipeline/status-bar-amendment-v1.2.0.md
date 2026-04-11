# Contract Amendment: Status Bar v1.2.0

**Date:** 2026-04-09
**Author:** Canvas
**Status:** Ready for Forge Review
**Amendment type:** Additive — new states only. No existing spec revised.

---

## Amendment Scope

This amendment patches two surfaces simultaneously. Both are spec gaps
that are blocking Forge's Device Connect Surface B build.

| Surface                      | Document                                     | Gap resolved                                                                      |
|------------------------------|----------------------------------------------|-----------------------------------------------------------------------------------|
| Connection Summary Bar       | Device Connect Contract v1.0.0 — Section 1.3 | Adds Session Ready as a 6th state                                                 |
| Status Bar Connection Widget | Device Connect Contract v1.0.0 — Section 2.2 | Adds Session Ready widget state + provisional Zone B dimensions (resolves OI-001) |

No existing states, dimensions, or tokens are changed. All additions
are backward-compatible with the v1.0.0 spec.

---

## 1. Session Ready — Definition

Session Ready is a **computed positive state**, not merely "all devices
connected." It requires all of the following conditions simultaneously:

| Condition                                      | Logic                                                               |
|------------------------------------------------|---------------------------------------------------------------------|
| All configured devices connected               | 0 disconnected, 0 offline, 0 error                                  |
| No active safety warnings or unsafe conditions | Safety status = All Clear or No Source (source absent is not unsafe) |
| Site profile exists                            | Settings → Site & Observatory has a valid site configured            |
| No sequence currently running                  | Sequencer is in Idle or Stopped state                               |

**V1 scope note:** Camera cooler-at-target is NOT a Session Ready
condition in V1. A camera that is still cooling but connected reads as
Session Ready — the user chooses when to start. This can be revisited
in v1.3.0 if user testing shows the CTA fires too early.

Session Ready clears immediately if any condition above is no longer
met. The Connection Summary Bar and Status Bar Widget revert to their
previous states without animation (instant update, not a graceful
transition — losing readiness is informational, not a dramatic event).

---

## 2. Connection Summary Bar — Session Ready State

**Location:** Device Connect Contract v1.0.0 — Section 1.3,
Summary Bar states table. Insert as a 6th row.

---

### 2.1 State Anatomy

```
┌──────────────────────────────────────────────────────────────────┐
│  ● All [N] devices ready                    [Start Session  →]   │
└──────────────────────────────────────────────────────────────────┘
```

This state **replaces** the "All Connected" state visually when all
Session Ready conditions are met. It is not shown in addition to it.

---

### 2.2 Layout Spec

| Element                | Spec                                                              |
|------------------------|-------------------------------------------------------------------|
| Bar height             | 40px — unchanged from v1.0.0                                      |
| Status dot             | 10px, color-status-success                                        |
| Status text            | "All [N] devices ready" — text-body-sm / 400 / color-text-primary |
| CTA button             | Right-aligned pill, 32px height                                   |
| CTA label              | "Start Session →" — text-label / 500                              |
| CTA horizontal padding | space-3 (12px) left and right                                     |
| CTA border radius      | radius-full (pill)                                                |
| CTA to bar right edge  | space-6 (24px)                                                    |
| Text to CTA gap        | space-4 (16px) minimum                                            |

### 2.3 CTA Color Spec

| Element                | Light Token                               | Dark Token                |
|------------------------|-------------------------------------------|---------------------------|
| CTA background         | color-interactive-primary                 | color-interactive-primary |
| CTA background (hover) | color-interactive-pressed                 | color-interactive-pressed |
| CTA label              | #FFFFFF                                   | #FFFFFF                   |
| CTA focus ring         | 2px color-interactive-primary, 2px offset | same                      |

### 2.4 CTA Action

"Start Session →" navigates to **Sequencer mode**.

| Sequencer state on click       | Behaviour                                                           |
|--------------------------------|---------------------------------------------------------------------|
| Sequence loaded (idle/stopped) | Navigate to Sequencer — sequence is ready to run                    |
| No sequence loaded             | Navigate to Sequencer — New Sequence flow opens automatically       |
| Sequence running               | This state is not reachable — Session Ready requires Sequencer idle |

The CTA does not start a sequence automatically. It navigates. The
user initiates the run from the Sequencer. This is intentional —
"ready to image" and "start imaging" are two distinct decisions.

### 2.5 Disconnect All confirmation (Session Ready state)

When the user triggers Disconnect All from Session Ready state, the
inline confirmation in the summary bar adds a context line:

```
Your rig is ready to image. Disconnect all devices?
[Disconnect All]   [Cancel]
```

Same inline treatment as v1.0.0 — no modal.

### 2.6 Updated Summary Bar States Table (full replacement)

| State                     | Text                                         | Indicator                       | CTA                   |
|---------------------------|----------------------------------------------|---------------------------------|-----------------------|
| All Disconnected          | "No devices connected"                       | ○ color-border-default (hollow) | "Connect All" chip    |
| Connecting                | "Connecting… [N] of [N]"                     | Spinner (animated)              | —                     |
| Partially Connected       | "[N] of [N] devices connected"               | ● color-status-warning          | "Connect All" chip    |
| All Connected (not Ready) | "All [N] devices connected"                  | ● color-status-success          | —                     |
| Error present             | Appends " · [N] error" in color-status-error | —                               | —                     |
| **Session Ready** *(new)* | **"All [N] devices ready"**                  | **● color-status-success**      | **"Start Session →"** |

**All Connected vs Session Ready:** These are distinct states. "All
Connected" shows when devices are connected but Session Ready conditions
are not all met (e.g. no site profile configured, safety warning
active). Session Ready only shows when every condition is satisfied.
The difference communicates meaningfully to the user — connected is
not the same as ready.

---

## 3. Status Bar Connection Widget — Session Ready State

**Location:** Device Connect Contract v1.0.0 — Section 2.2,
Widget States. Insert as a new state after "All Connected."

---

### 3.1 Session Ready Widget State

```
┌─────────────────────────────────────────────┐
│  ●  Session Ready              [  Start →  ] │
└─────────────────────────────────────────────┘
```

| Element                     | Spec                                                       |
|-----------------------------|------------------------------------------------------------|
| Status dot                  | 8px, color-status-success                                  |
| Label                       | "Session Ready" — text-label / 500 / color-status-success  |
| Action chip                 | "Start →" — primary chip style, 28px height, pill          |
| Action chip click           | Navigates to Sequencer mode (same logic as Section 2.4)    |
| Widget click (outside chip) | Opens connection popover (same as All Connected)           |

**Label colour note:** In Session Ready, the label text uses
`color-status-success` rather than the default `color-text-primary`.
This is the one state where the label carries positive colour — it is
a reward signal, not just status text. All other widget states use
`color-text-primary` or `color-text-secondary` for the label.

### 3.2 Popover in Session Ready State

The connection popover (Section 2.3 of Device Connect v1.0.0) gains
one addition in Session Ready state — a header strip above the device list:

```
┌─────────────────────────────────────────────────────┐
│  ● Your rig is ready to image.                      │
│  ─────────────────────────────────────────────────  │
│  ● Mount          10Micron GM2000  Connected        │
│  ● Camera         ZWO ASI2600MM   Connected         │
│  ...                                                │
│  ─────────────────────────────────────────────────  │
│  [Start Session]              [Disconnect All]      │
└─────────────────────────────────────────────────────┘
```

**Header strip spec:**

- Height: 32px
- Background: color-status-success at 10% opacity
- Text: "● Your rig is ready to image." — text-body-sm / 500 / color-status-success
- Horizontal padding: space-4 (16px)
- Bottom border: 1px color-border-subtle

**Footer in Session Ready:** "Start Session" replaces "Connect All"
as the primary footer action. "Disconnect All" remains as secondary.
"Start Session" button: full popover width secondary/ghost style —
not primary fill, because the popover is informational, not a launch
pad. The navigation CTA in the summary bar is the primary call.

---

## 4. Zone B Provisional Dimensions (resolves Device Connect OI-001)

**Note:** These are provisional dimensions pending the App Shell
contract. They are sufficient for Forge to begin Surface B build.
The App Shell contract will confirm or amend these values. If amended,
a Zone B layout revision will be issued as v1.2.1.

### 4.1 Status Bar Dimensions (provisional)

| Property                | Value                                                   | Notes                                                    |
|-------------------------|---------------------------------------------------------|----------------------------------------------------------|
| Status Bar total height | 40px                                                    | Fixed — matches Connection Summary Bar height convention |
| Zone A width            | 40px                                                    | Safety Panel trigger — icon button only                  |
| Zone B width            | flex: min 160px, max 220px                              | Connection Widget — content-driven within bounds         |
| Zone C width            | flex: fill remaining                                    | Sequencer status / sequence name — out of scope here     |
| Zone D width            | 40px–80px                                               | Night Mode + settings icons — out of scope here          |
| Zone layout             | Horizontal flex row, vertically centred                 | Zones A, B, C, D left to right                           |
| Zone B internal padding | 0 — no internal padding; widget manages its own spacing |                                                          |
| Zone B right border     | 1px color-border-subtle                                 | Visual separation from Zone C                            |
| Zone A right border     | 1px color-border-subtle                                 | Visual separation from Zone B                            |

### 4.2 Zone B Widget Sizing

The Connection Widget is a self-sizing component within Zone B.
It grows from minimum (dot + label only, no chip) to maximum (dot +
label + chip) based on state. Zone B clips to max-width 220px — if
the widget content would exceed this, the label truncates with ellipsis
at the midpoint.

| State                                    | Approximate widget width |
|------------------------------------------|--------------------------|
| All Disconnected + "Connect All" chip    | ~200px                   |
| Connecting (no chip)                     | ~130px                   |
| Partially Connected + "Connect All" chip | ~200px                   |
| All Connected (no chip)                  | ~140px                   |
| Session Ready + "Start →" chip           | ~190px                   |
| Error + "Retry" chip                     | ~160px                   |

These are approximate. Final widths will be determined by font
rendering at text-label (12px / 500). Forge should allow the widget
to size naturally within the 160–220px bounds.

---

## 5. Animation — Session Ready Transitions

These supplement the animation spec in Device Connect v1.0.0 Section 3.

### 5.1 Entering Session Ready

Triggered when the last Session Ready condition is satisfied (typically
the last device connects successfully).

| Element            | Animation                                               | Duration | Easing          |
|--------------------|---------------------------------------------------------|----------|-----------------|
| Summary bar text   | Cross-fade from "All [N] connected" to "All [N] ready"  | 300ms    | ease-standard   |
| Start Session chip | Width expand from 0 + opacity 0→1                       | 300ms    | ease-decelerate |
| Widget label       | Cross-fade + colour shift to color-status-success       | 250ms    | ease-standard   |
| Widget chip        | Width expand from 0 + opacity 0→1                       | 250ms    | ease-decelerate |

### 5.2 Leaving Session Ready

Triggered when any condition is no longer met.

| Element            | Animation                                       | Duration | Easing          |
|--------------------|-------------------------------------------------|----------|-----------------|
| Start Session chip | Width collapse to 0 + opacity 1→0               | 150ms    | ease-accelerate |
| Summary bar text   | Cross-fade back to appropriate state            | 200ms    | ease-standard   |
| Widget label       | Cross-fade + colour shift to color-text-primary | 200ms    | ease-standard   |

### 5.3 Reduce Motion

All width expand/collapse → instant. Cross-fades at duration-instant
(80ms). Colour shifts instant.

---

## 6. Accessibility Additions

These supplement the accessibility spec in Device Connect v1.0.0 Section 4.

| Element                         | Role      | Label                               | State                   |
|---------------------------------|-----------|-------------------------------------|-------------------------|
| Summary bar (Session Ready)     | StatusBar | "Connection summary"                | "All [N] devices ready" |
| Start Session CTA (summary bar) | Button    | "Start imaging session"             | —                       |
| Widget (Session Ready)          | Button    | "Device connections, session ready" | —                       |
| Widget chip "Start →"           | Button    | "Start imaging session"             | —                       |
| Popover header strip            | Text      | "Your rig is ready to image"        | —                       |
| Popover Start Session footer    | Button    | "Start imaging session"             | —                       |

**Live region:** The Connection Summary Bar should be marked as
`aria-live="polite"` so that state changes (including the transition
to Session Ready) are announced to screen reader users without
interrupting ongoing narration.

---

## 7. Night Mode

Session Ready states under Night Mode inherit the standard Night Mode
overlay. No specific overrides required. `color-status-success` under
the red Night Mode overlay reads as a warm amber — this is acceptable
for the positive ready state and does not conflict with warning amber
(`color-status-warning`) because the label text provides the semantic
meaning. Canvas will include Session Ready in the Night Mode contrast
audit (Safety Panel OI-002 / safety design system v1.2.0).

---

## 8. Sketch Symbol Updates

| Symbol                      | Change                                          |
|-----------------------------|-------------------------------------------------|
| DeviceConnect/SummaryBar    | Add variant: `SessionReady`                     |
| StatusBar/ConnectionWidget  | Add variant: `SessionReady`                     |
| StatusBar/ConnectionPopover | Add variant: `SessionReady` (with header strip) |

---

## 9. Resolved Open Items

| ID     | Contract              | Resolution                                                                                        |
|--------|-----------------------|---------------------------------------------------------------------------------------------------|
| OI-001 | Device Connect v1.0.0 | Zone B provisional dimensions specified in Section 4. Confirmed by App Shell contract when issued. |
| —      | — (session decision)  | Session Ready state: Connection Summary Bar. Confirmed by Matt 2026-04-09.                        |

---

## 10. Sign-Off

| Role   | Status     | Date       |
|--------|------------|------------|
| Canvas | ✅ Approved | 2026-04-09 |
| Forge  | ⬜ Pending  | —          |
| Matt   | ⬜ Pending  | —          |

---

*Status Bar Amendment v1.2.0 — Dark Sky*
*Patches: Device Connect Contract v1.0.0 (Sections 1.3, 2.2, 2.3)*
*Canvas / CombeCrew Design*
