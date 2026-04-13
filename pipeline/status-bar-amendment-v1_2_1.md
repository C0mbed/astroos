# Contract Amendment: Status Bar v1.2.1

**Date:** 2026-04-12
**Author:** Canvas
**Status:** Ready for Forge + Matt Sign-Off
**Patches:** Amendment v1.2.0 (Sections 2.6, 3.1, 4.2)
**Amendment type:** Corrective — retires one state, formalises one state.

---

## Amendment Scope

Two corrections to Amendment v1.2.0, both arising from Stage 4 visual QA
and Matt's direct confirmation on 2026-04-12.

| Change | Sections affected | Authority |
|--------|-------------------|-----------|
| Retire AllConnected (not Ready) as a distinct state | 2.6, 3.1, 4.2 | Matt confirmed 2026-04-12 |
| Formalise Error Present as a standalone state (not additive) | 2.6, 4.2 | Canvas accepted Forge's implementation, 2026-04-12 |

No other content from v1.2.0 is changed. All other sections remain in force.

---

## Change 1 — Retire AllConnected (not Ready) State

### Background

Amendment v1.2.0 Section 2.6 defines "All Connected (not Ready)" as a distinct
state — all devices connected but Session Ready conditions not fully met.

**Matt confirmed on 2026-04-12: this state does not exist.**

Session Ready is the fully connected state. There is no intermediate between
Partially Connected and Session Ready. The distinction was a spec artefact from
when the state model was still being worked out.

The live build correctly implements 5 states with no AllConnected intermediate.
The build is correct. This amendment brings the contract into alignment with it.

### Section 2.6 — Updated Summary Bar States Table (full replacement)

Replaces the table in Amendment v1.2.0 Section 2.6.

**Remove:** "All Connected (not Ready)" row.
**Amend:** "Error present" row — see Change 2 below.
**Result:** 5-state model, matching the live build.

| State | Text | Indicator | CTA |
|-------|------|-----------|-----|
| All Disconnected | "No devices connected" | ○ color-border-default (hollow) | "Connect All" chip |
| Connecting | "Connecting… [N] of [N]" | Spinner (animated) | — |
| Partially Connected | "[N] of [N] devices connected" | ● color-status-warning | "Connect All" chip |
| Error Present *(amended — see Change 2)* | "[N] Errors" | ● color-status-error | "Retry" chip |
| **Session Ready** | **"All [N] devices ready"** | **● color-status-success** | **"Start Session →"** |

**Note on AllConnected vs Session Ready (replaces note in v1.2.0 §2.6):**
Session Ready is the fully connected state. When all devices are connected and all
Session Ready conditions are met, the widget shows Session Ready directly. There
is no intermediate "all connected, not ready" state in the Status Bar widget.

If future requirements introduce a meaningful distinction (e.g. camera still cooling
after all devices connect), this can be revisited in a subsequent amendment. For
V1, Session Ready is the terminal connected state.

### Section 3.1 — Status Bar Connection Widget (editorial correction)

In Amendment v1.2.0 Section 3.1, the insertion note reads:
> "Insert as a new state after 'All Connected.'"

**Replace with:**
> "Session Ready is the fifth and final widget state, following Partially Connected.
> No AllConnected (not Ready) state exists in the widget."

All other content in Section 3.1 is unchanged.

---

## Change 2 — Formalise Error Present as a Standalone State

### Background

Amendment v1.2.0 Section 2.6 describes Error Present as an **additive modifier**:
> "Appends ' · [N] error' in color-status-error"

This means Error was designed to overlay other states — e.g. "3/5 Connected · 2 Errors."

**Forge implemented Error as a standalone state:** red dot, "[N] Errors" label,
"Retry" chip — replacing rather than appending to the current state.

**Canvas accepted this implementation during Stage 4 review as superior UX.**
When errors are present, the count and the remediation action ("Retry") are the
primary information. Displaying "3/5 Connected · 2 Errors" makes the user parse
two pieces of information to understand the situation. A clean "2 Errors / Retry"
is unambiguous.

The build is correct. This amendment formalises it.

### Error Present — Full Standalone Spec

**Widget layout:**

```
┌─────────────────────────────────────────────┐
│  ●  2 Errors                  [  Retry  ]   │
└─────────────────────────────────────────────┘
```

| Element | Spec |
|---------|------|
| Status dot | 8px, color-status-error |
| Label | "[N] Errors" — text-label / 500 / color-text-primary |
| Label format | Integer + "Errors" — e.g. "1 Error", "2 Errors", "5 Errors" |
| Label singular | "1 Error" (not "1 Errors") |
| Action chip | "Retry" — primary chip style, 28px height, pill |
| Action chip click | Triggers reconnect attempt on all errored devices |
| Widget click (outside chip) | Opens connection popover |

**State precedence:**
Error Present takes priority over Partially Connected when one or more devices
are in an error state. If 3 devices are connected and 2 are erroring (not merely
disconnected), the widget shows Error Present, not Partially Connected.

"Error" is defined as a device that has attempted connection and actively failed —
not a device that is simply disconnected. A device that has never connected shows
as part of the Disconnected or Partially Connected count. A device that attempted
and errored triggers Error Present.

**Summary Bar Note:**
The Summary Bar (Device Connect Contract v1.0.0) was specified with the same
additive approach. For consistency, the same standalone treatment applies there.
This amendment covers both surfaces.

### Section 4.2 — Zone B Widget Sizing Table (full replacement)

Replaces the table in Amendment v1.2.0 Section 4.2.

**Remove:** "All Connected (no chip)" row.
**Amend:** "Error" row to reflect standalone treatment.

| State | Approximate widget width |
|-------|--------------------------|
| All Disconnected + "Connect All" chip | ~200px |
| Connecting (no chip) | ~130px |
| Partially Connected + "Connect All" chip | ~200px |
| Error Present + "Retry" chip | ~160px |
| Session Ready + "Start →" chip | ~190px |

These are approximate. Final widths determined by font rendering at
text-label (12px / 500). Forge allows the widget to size naturally
within the 160–220px Zone B bounds.

---

## State Model Summary (definitive — supersedes all previous versions)

Five states, in order of typical session progression:

| # | State | Trigger | CTA |
|---|-------|---------|-----|
| 1 | Disconnected | No devices connected | Connect All |
| 2 | Connecting | Connection in progress | — |
| 3 | Partially Connected | Some connected, some not, none erroring | Connect All |
| 4 | Error Present | One or more devices in error state | Retry |
| 5 | Session Ready | All connected, all conditions met | Start → |

States 3 and 4 can coexist sequentially but not simultaneously — the widget
shows one state at a time. Error Present takes precedence over Partially
Connected when errors are present.

---

## No Other Changes

All other content in Amendment v1.2.0 remains in force:
- Session Ready definition (Section 1)
- Connection Summary Bar Session Ready spec (Sections 2.1–2.5)
- Status Bar Connection Widget Session Ready spec (Sections 3.1–3.2)
- Zone B provisional dimensions (Section 4.1)
- Animation specs (Section 5)
- Accessibility additions (Section 6)
- Night Mode note (Section 7)
- Sketch symbol updates (Section 8)

---

## Sign-Off

| Role | Status | Date |
|------|--------|------|
| Canvas | ✅ Approved | 2026-04-12 |
| Forge | ⬜ Pending | — |
| Matt | ⬜ Pending | — |

Both Forge and Matt sign-off required before Amendment v1.2.1 is in force.
Amendment v1.2.0 remains the operative document until v1.2.1 is signed.

---

*Status Bar Amendment v1.2.1 — Dark Sky*
*Patches: Amendment v1.2.0 (Sections 2.6, 3.1, 4.2)*
*Canvas / CombeCrew Design*
