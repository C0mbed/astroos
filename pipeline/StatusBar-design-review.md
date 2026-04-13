# Design Review — Status Bar v1.1.0
**Review Version:** 1.1.0 (updated with live screenshot review)
**Date:** 2026-04-12
**Reviewer:** Canvas
**References Contract:** Status Bar Contract v1.1.0 + Amendment v1.2.0
**References Log:** v2_1

---

## Screenshot Review Notes

Six screenshots were provided from `C:\Users\c0mbe\astroos\screenshots\`.
Three yielded clean visual review. Three were unusable due to status bar being
cropped out of frame.

**Usable:**
- Image 1 — Partially Connected (clear)
- Image 2 — Session Ready (clear) — filename mislabelled, see below
- Image 4 — Disconnected (clear) — filename mislabelled, see below

**Unusable:**
- Image 3 — Bar cropped out of frame
- Image 5 — Bar cropped out of frame
- Image 6 — Bar mostly cropped (partial chip visible only)

**Filename anomalies — flag for Forge:**
- `state_2-Connecting.png` actually shows **Session Ready**
- `state_4-AllConnected.png` actually shows **Disconnected**

The mock timer is cycling faster than the screenshot tool captures. Forge should
regenerate a clean screenshot set with the mock paused at each state, or slowed
significantly, before the Stage 5 sign-off record is finalised. This does not
affect the build verdict — it is an admin/record-keeping issue.

**States not captured cleanly:** Connecting, Error Present.
These were reviewed against Forge's verbal descriptions only.

---

## State-by-State Review

| # | State | Contract Spec | Visual Review | Verdict | Severity |
|---|-------|---------------|--------------|---------|----------|
| 1 | Disconnected | Hollow ring indicator, "Disconnected" label, "Connect All" chip | Hollow grey circle, "Disconnected" label, blue "Connect All" pill — **Canvas-verified** | **Pass** | — |
| 2 | Connecting | Animated spinner, "Connecting…" label, no chip | Not captured — Forge-confirmed description accepted | **Pass (unverified)** | — |
| 3 | Partially Connected | Filled amber dot (color-status-warning), "3/5 Connected" label, "Connect All" chip | Amber dot, "3/5 Connected", blue "Connect All" pill — **Canvas-verified** | **Pass** | — |
| 4 | Error Present | Standalone state: red dot, "N Errors" label, "Retry" chip | Not captured — Forge-confirmed description accepted | **Pass (unverified)** | — |
| 5 | Session Ready | Green dot (color-status-success), label in color-status-success, "Start →" chip | Green dot, "Session Ready" in green text, blue "Start →" pill — **Canvas-verified** | **Pass** | — |

**Additional confirmed observation — Session Ready label colour:**
The "Session Ready" label text is rendered in color-status-success (green). This
is correct per Amendment §3.1 and is a nice touch — the entire widget reads
green when the system is ready. Confirmed pass.

**Additional confirmed observation — Disconnected ring:**
The hollow ring renders as a mid-grey circle outline, consistent with
color-border-default on the dark background. Confirmed pass.

---

## Two Open Questions — Canvas Decisions

### Question 1 — Top Separator

**Visual confirmation:** The flush treatment is confirmed in the live build.
Status bar and content area share a contiguous dark surface with no divider.

**Canvas decision: OPTION B — Add the separator. Confirmed.**

The screenshot makes this decision easy to validate. The content area and status
bar are visually the same layer. As content populates above the bar, this will
create a smeared boundary. The separator must be present before any content
surfaces are built.

**Specification:**
- Property: top border on Status Bar outer container
- Value: 1px solid {color-border-subtle}
- Always visible, all states

**Forge must implement before Stage 5.**

---

### Question 2 — Zone Borders

**Visual confirmation:** Zone borders confirmed absent in the live build.
Zone A and Zone B are visually undivided.

**Canvas decision: CONFIRM zone borders per Amendment v1.2.0 §4.1. Confirmed.**

**Specification:** Per Amendment v1.2.0 §4.1 — unchanged.
- Zone A: 1px solid {color-border-subtle} on right edge
- Zone B: 1px solid {color-border-subtle} on right edge

**Forge must implement before Stage 5.**

---

## Accessibility Review

| Check | Expected | Status |
|-------|----------|--------|
| Colour not sole indicator | All states have label text | Pass — confirmed in screenshots |
| Session Ready label contrast | Green text on dark bg — must meet AA | Pass — color-status-success on #0B1220 meets AA |
| Disconnected ring contrast | Grey hollow ring on dark bg | Pass — legible, sufficient contrast |
| Animated spinner reduced-motion | Respects prefers-reduced-motion | Pending — flag for Crucible Stage 5 |
| Chip button accessibility | Keyboard focusable, labelled | Forge-confirmed — not Canvas-verified |

---

## Summary Finding Table

| Finding | Severity | Required Before Stage 5 | Action Owner |
|---------|----------|------------------------|-------------|
| Top separator absent — now specified | Minor | **YES — implement** | Forge |
| Zone A + B right borders absent | **Major** | **YES — implement** | Forge |
| Error state: standalone vs appended | Minor | No build change | Canvas: amendment v1.2.1 |
| AllConnected (not Ready) state absent | Minor | No build change | Canvas: amendment v1.2.1 |
| Screenshot set mislabelled/incomplete | Admin | Regenerate before Stage 5 record | Forge |
| Spinner reduced-motion compliance | Minor | Verify in Stage 5 | Crucible |
| Forge sign-off on Amendment v1.2.0 | Admin | **YES** | Forge |

---

## Verdict

**APPROVED WITH CONDITIONS**

Visual inspection confirms all three captured states are correct. Forge-confirmed
descriptions accepted for Connecting and Error Present.

Two implementation items required before Stage 5:
1. ✅ 1px top separator on Status Bar container
2. ✅ Zone A and Zone B right borders per Amendment §4.1

Additionally, Forge should regenerate the screenshot sign-off set with the mock
paused at each state before the Stage 5 record is finalised.

---

## Additional Commission — Branded Empty State (Matt decision: 2026-04-12)

The centered "Dark Sky" TextBlock placeholder in the main content area should be
replaced with the branded wordmark lockup. This commission was confirmed by Matt
on review of the live screenshots.

### Spec: Content Area Empty State

**Element:** Horizontal wordmark lockup — mark left, "DARK SKY" text right.

**Position:**
- Horizontal: centred in content area
- Vertical: 42% from top of content area
  (optical centre — slightly above true vertical centre)

**Dimensions:**

| Element | Value |
|---------|-------|
| Mark (dark-sky-icon.svg) | 48×48px, rx=5.2px |
| Gap (mark to text) | 16px (space-4) |
| Text: "DARK SKY" | Space Grotesk 700, 24px, all caps |
| Letter-spacing | 3px |
| Text colour | {color-text-primary} (≈ #EEF2F6 dark mode) |
| Opacity — entire lockup | 28% |

**Opacity rationale:**
This is an empty state, not a splash screen or loading screen. At 28% the mark
is present and felt — it establishes identity without competing with content once
content arrives. At full opacity it becomes a statement the app hasn't earned yet
at this stage of the session.

**Asset reference:** `dark-sky-icon.svg` — produced this session, Stage 2.
Use the SVG directly via WinUI Image control, or substitute the 48px PNG export
once Forge confirms the export pass.

**Implementation note for Forge:** This replaces the TextBlock with alignment
`HorizontalAlignment="Center" VerticalAlignment="Center"` (default template content).
The replacement element is a StackPanel (Horizontal orientation) containing:
1. Image (48×48, Source: dark-sky-icon.svg or 48px PNG)
2. TextBlock ("DARK SKY", Space Grotesk 700, 24px, letter-spacing 3px)
The StackPanel has `Opacity="0.28"` applied at the container level.

---

## Part 3 — Contract Status Confirmations (for Forge)

### Status Bar Amendment v1.2.0
**CURRENT AND VALID.** Sign-off required before Stage 5.
Canvas will issue v1.2.1 to formalise Error standalone treatment and
AllConnected retirement.

### Device Setup v1.0.0
**CURRENT AND VALID.** Ready for Forge review and sign-off.

### Settings → Safety v1.1.0
**CURRENT AND VALID.** Ready for Forge review and sign-off.

---

*Status Bar Design Review v1.1.0 — Dark Sky*
*Canvas / CombeCrew Design*
*Stage 4 Gate: Approved with conditions*
