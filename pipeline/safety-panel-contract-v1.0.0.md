# Component Contract: Safety Panel

**Version:** 1.0.0
**Date:** 2026-04-09
**Status:** Ready for Review — Pending Forge Sign-off
**Author:** Canvas
**Pipeline Stage:** 2 — Design & Component Contract

---

## 0. Contract Notes & Design Calls

The following design calls were made by Canvas where the brief left
explicit latitude or where a question was not answered before contract
deadline. Each is flagged — Forge and Matt may challenge any of them
before sign-off.

**DC-001 — Drawer width: fixed 520px, not percentage-based.**
Forge's brief specified ~40% screen width. Canvas is diverging to a
fixed 520px. Rationale: percentage-based widths produce inconsistent
content layout — at 1280px a 40% drawer is 512px, at 2560px it is
1024px. The Safety Panel contains a fixed-diameter horizon plot and a
structured list of safety conditions; both benefit from predictable
column widths. 520px gives comfortable content layout at 1280px
(the minimum supported resolution) while remaining proportionate at
1920px. If Forge has a strong implementation reason to retain
percentage-based width, escalate before build begins.

**DC-002 — Override controls: visible-locked in default state.**
The question of whether override controls should be hidden until
Settings-unlocked or visible in a locked state was not answered before
contract deadline. Canvas is specifying visible-locked: a single
subdued row reading "Safety overrides — enable in Settings →" with a
settings link icon. Rationale: power users should discover this
capability on day one without a support query. The locked affordance
is deliberately low-contrast and non-intrusive — it doesn't compete
with active safety content. If Matt prefers fully hidden until
unlocked, this is a one-state removal, not a redesign.

**DC-003 — Horizon mask: read-only polar plot in panel.**
The Safety Panel shows the horizon mask as a read-only status
visualisation — not an editor. The full polar-plot editor lives in
Observatory settings. Size: 240×240px at standard density, 200×200px
at compact density. If this proves too small for meaningful
interpretation, the horizon plot row should include a "View full
editor →" link rather than expanding the panel.

---

## 1. Purpose & Scope

The Safety Panel is a right-side drawer overlay providing at-a-glance
safety system status and quick access to safety limit configuration
during active or standby sessions. It does NOT contain sequence
controls, device controls, or full horizon mask editing. It is accessed
from Status Bar Zone A and is the single surface for safety monitoring
in the application.

The Safety Panel is an overlay — it sits above the main application
content. The application behind it remains visible and interactive
through the scrim. Escape key or scrim click dismisses.

---

## 2. Anatomy

```
+--[SCRIM: full screen behind drawer]----------------------------+
|                                                                 |
|  +--[DRAWER: right edge, 520px wide, full viewport height]---+ |
|  |                                                            | |
|  |  [DRAWER HEADER]                                          | |
|  |  +---------+---------------------------+----------------+ | |
|  |  | ← [X]  |  Safety                   | [Night Mode ●] | | |
|  |  +---------+---------------------------+----------------+ | |
|  |                                                            | |
|  |  [SYSTEM STATUS BANNER]                                   | |
|  |  +------------------------------------------------------+ | |
|  |  | ● All Systems Safe          [last checked 0:12 ago]  | | |
|  |  +------------------------------------------------------+ | |
|  |                                                            | |
|  |  [SECTION: SAFETY SOURCES]                                | |
|  |  Label: "Monitoring Sources"                              | |
|  |  +------------------------------------------------------+ | |
|  |  | ● ASCOM ObservingConditions   Connected    [config>] | | |
|  |  | ○ Safe-file                   Off                    | | |
|  |  | ○ HTTP Endpoint               Off                    | | |
|  |  | ○ INDI Weather                Off                    | | |
|  |  | ○ Weather Forecast (fallback) Off                    | | |
|  |  +------------------------------------------------------+ | |
|  |                                                            | |
|  |  [SECTION: SAFETY LIMITS]                                 | |
|  |  Label: "Limits"                                          | |
|  |  +------------------------------------------------------+ | |
|  |  | Minimum Altitude     [——●——————]  25°                | | |
|  |  | Hour Angle Limit     [————————●]  5.5h               | | |
|  |  | Pier Flip Margin     [——●——————]  10°                | | |
|  |  +------------------------------------------------------+ | |
|  |                                                            | |
|  |  [SECTION: HORIZON MASK]                                  | |
|  |  Label: "Horizon Mask"                                    | |
|  |  +------------------------------------------------------+ | |
|  |  |  [POLAR PLOT 240×240px — read only]                  | | |
|  |  |  Active profile: Home Observatory    [Edit full →]   | | |
|  |  +------------------------------------------------------+ | |
|  |                                                            | |
|  |  [SECTION: OVERRIDES]                                     | |
|  |  Label: "Safety Overrides"                                | |
|  |  +------------------------------------------------------+ | |
|  |  | 🔒 Safety overrides — enable in Settings →           | | |  <- Override Locked state
|  |  +------------------------------------------------------+ | |
|  |                                                            | |
|  |  [SECTION: GOODNIGHT]                                     | |
|  |  Label: "Goodnight Sequence"                              | |
|  |  +------------------------------------------------------+ | |
|  |  | Configured: Park → Cool Down → Close Roof            | | |
|  |  | [Run Goodnight Now]                                   | | |
|  |  +------------------------------------------------------+ | |
|  |                                                            | |
|  +------------------------------------------------------------+ |
+----------------------------------------------------------------+
```

---

## 3. Dimensions

### Drawer Container

| Property | Value                   | Notes                                            |
|----------|-------------------------|--------------------------------------------------|
| Width    | 520px                   | Fixed — see DC-001                               |
| Height   | 100vh                   | Full viewport height                             |
| Position | Fixed, right: 0, top: 0 |                                                  |
| Z-index  | 900                     | Above all content, below modal dialogs (z: 1000) |

### Scrim

| Property   | Value                                         |
|------------|-----------------------------------------------|
| Position   | Fixed, full viewport                          |
| Z-index    | 899                                           |
| Background | rgba(0, 0, 0, 0.45) light mode                |
| Background | rgba(0, 0, 0, 0.60) dark mode                 |
| Background | rgba(120, 0, 0, 0.35) Night Mode (red-tinted) |

### Drawer Header

| Property           | Value                          |
|--------------------|--------------------------------|
| Height             | 56px fixed                     |
| Horizontal padding | space-5 (20px)                 |
| Border bottom      | 1px color-border-default       |
| Close button       | 32×32px hit target, icon 20×20 |

### Section Headers

| Property           | Value                             |
|--------------------|-----------------------------------|
| Height             | 32px                              |
| Top margin         | space-6 (24px) above each section |
| Horizontal padding | space-5 (20px)                    |

### System Status Banner

| Property           | Value                       |
|--------------------|-----------------------------|
| Height             | 48px                        |
| Horizontal padding | space-5 (20px)              |
| Margin top         | space-4 (16px) below header |
| Border radius      | radius-md (8px)             |

### Content Area

| Property           | Value                                  |
|--------------------|----------------------------------------|
| Horizontal padding | space-5 (20px) left and right          |
| Bottom padding     | space-8 (32px)                         |
| Overflow           | scroll (vertical), hidden (horizontal) |
| Scroll behaviour   | smooth                                 |

### Limit Sliders

| Property            | Value                     |
|---------------------|---------------------------|
| Row height          | 48px                      |
| Track height        | 4px                       |
| Thumb diameter      | 16px                      |
| Label width         | 160px fixed               |
| Value readout width | 48px fixed, right-aligned |

### Horizon Plot

| Property                          | Value                        |
|-----------------------------------|------------------------------|
| Size (standard)                   | 240×240px                    |
| Size (compact — observatory mode) | 200×200px                    |
| Margin                            | auto (horizontally centred)  |
| Bottom margin                     | space-3 (12px) above caption |

### Override Row (Locked state)

| Property      | Value                          |
|---------------|--------------------------------|
| Height        | 40px                           |
| Border radius | radius-md (8px)                |
| Border        | 1px dashed color-border-default |

---

## 4. Typography

| Element                  | Token               | Size / Weight / Line Height                    |
|--------------------------|---------------------|------------------------------------------------|
| Drawer title "Safety"    | text-heading-lg     | 24px / 600 / 1.3                               |
| Section labels           | text-label          | 12px / 500 / 1.4 / +0.06em tracking — ALL CAPS |
| System status text       | text-body-lg        | 16px / 600 / 1.5                               |
| Source list item name    | text-body-sm        | 14px / 400 / 1.5                               |
| Source list item status  | text-label          | 12px / 500 / 1.4                               |
| Limit label              | text-body-sm        | 14px / 400 / 1.5                               |
| Limit value readout      | text-mono           | 13px / 400 / 1.5                               |
| Horizon mask caption     | text-caption        | 11px / 400 / 1.4                               |
| Override locked row      | text-body-sm        | 14px / 400 / 1.5                               |
| Override locked row      | color-text-tertiary | Subdued — not primary                          |
| Goodnight config summary | text-body-sm        | 14px / 400 / 1.5                               |
| Last checked timestamp   | text-caption        | 11px / 400 / 1.4                               |

---

## 5. Color Tokens

### Drawer Shell

| Element                   | Light Token              | Dark Token               | Night Mode                                   |
|---------------------------|--------------------------|--------------------------|----------------------------------------------|
| Drawer background         | color-background-primary | color-background-primary | color-background-primary + red overlay at 8% |
| Drawer border (left edge) | color-border-default     | color-border-default     | color-border-night                           |
| Scrim                     | rgba(0,0,0,0.45)         | rgba(0,0,0,0.60)         | rgba(120,0,0,0.35)                           |
| Section label             | color-text-tertiary      | color-text-tertiary      | color-text-tertiary-night                    |
| Divider between sections  | color-border-subtle      | color-border-subtle      | color-border-subtle-night                    |

### System Status Banner

| State        | Background                  | Text                 | Icon                 |
|--------------|-----------------------------|----------------------|----------------------|
| All Clear    | color-status-success-subtle | color-status-success | color-status-success |
| Warning      | color-status-warning-subtle | color-status-warning | color-status-warning |
| Unsafe       | color-status-error-subtle   | color-status-error   | color-status-error   |
| Disconnected | color-background-secondary  | color-text-secondary | color-text-secondary |
| Initialising | color-background-secondary  | color-text-secondary | —                    |

### Safety Source Rows

| Element     | Connected            | Off/Inactive         | Error              |
|-------------|----------------------|----------------------|--------------------|
| Status dot  | color-status-success | color-border-default | color-status-error |
| Label text  | color-text-primary   | color-text-secondary | color-text-primary |
| Status text | color-status-success | color-text-tertiary  | color-status-error |

### Limit Sliders

| Element                  | Light Token               | Dark Token                |
|--------------------------|---------------------------|---------------------------|
| Track (filled)           | color-interactive-primary | color-interactive-primary |
| Track (unfilled)         | color-border-default      | color-border-default      |
| Thumb                    | color-interactive-primary | color-interactive-primary |
| Warning threshold marker | color-status-warning      | color-status-warning      |
| Value readout text       | color-text-primary        | color-text-primary        |

### Override Section

| State            | Background                  | Border                        | Text                 | Icon                 |
|------------------|-----------------------------|-------------------------------|----------------------|----------------------|
| Locked           | transparent                 | color-border-default (dashed) | color-text-tertiary  | color-text-tertiary  |
| Unlocked / Ready | transparent                 | none                          | color-text-secondary | color-text-secondary |
| Active           | color-status-warning-subtle | color-status-warning          | color-status-warning | color-status-warning |

### Goodnight Section

| State    | Element            | Token                       |
|----------|--------------------|-----------------------------|
| Idle     | Run button         | color-interactive-primary   |
| Active   | Step progress text | color-text-primary          |
| Active   | Progress bar fill  | color-interactive-primary   |
| Complete | Background         | color-status-success-subtle |

---

## 6. States

### 6.1 Closed (default)

Drawer is fully off-screen to the right. Scrim is not rendered.
No layout impact on the application content behind it.

### 6.2 Opening

Drawer slides in from right. Scrim fades up simultaneously.
See Section 8 — Animation for timing specs.

### 6.3 Open — All Clear

- Status banner: green, "All Systems Safe", last-checked timestamp
- All active safety sources show green dot
- Configured limits displayed in sliders — interactive, adjustable
- Horizon mask polar plot rendered read-only
- Override section: Locked (DC-002) or Unlocked/Ready depending on Settings
- Goodnight section: idle state with "Run Goodnight Now" button

### 6.4 Open — Warning

- Status banner: amber, "[Source name] approaching limit" or "[N] warnings active"
- Specific source row highlighted with amber dot and status text
- If altitude limit: slider thumb at or near warning threshold marker
- All other sections unchanged
- No blocking behaviour — session continues

### 6.5 Open — Unsafe / Halted

- Status banner: red, prominent, "[Reason]: sequence halted" or "[Reason]: sequence paused"
- Specific source row: red dot, error status text
- If triggered by altitude: slider shows current value in red
- A "View Log →" link in the banner routes to the Log Strip entry for the halt event
- No blocking behaviour in the panel itself — the session state is managed by the Sequencer; the Safety Panel reports it

### 6.6 Open — Override Locked (default, Settings not enabled)

Override section shows the locked row from anatomy above:

```
🔒 Safety overrides — enable in Settings →
```

Row is 40px, dashed border, all elements at color-text-tertiary.
The "→" is a settings-link icon (Phosphor ArrowSquareOut 16px).
Tapping/clicking navigates to Settings → Safety.
This row does not appear alarming. It is informational and subdued.

### 6.7 Open — Override Unlocked / Ready

Override section changes. Locked row is replaced by:

- Per-limit override toggles, one row per configured limit
- Each toggle row: limit name + current value + toggle
- All toggles are OFF by default at session start
- No acknowledgment prompt — that was completed in Settings
- Small caption below the section: "Overrides enabled in Settings" at text-caption / color-text-tertiary

### 6.8 Open — Override Active

One or more override toggles are ON.

The System Status Banner shifts:

```
⚠ Override Active — [limit name] bypassed
```

Banner background: color-status-warning-subtle
Banner text: color-status-warning

The specific override row shows the toggle in ON state.

AI advisor surfaces a contextual note (if AI enabled and advisor not suppressed by user). The note appears in the advisor's configured personality voice. Examples:

- Mission Control: "Hour angle override active. Monitor manually."
- Sagan: "You've asked the universe to wait. It may not."
- HAL: "I would recommend against this. I am noting it regardless."
- Laplace: "The limit exists for a reason I have already calculated."
- Tycho: "Bold. Historically, this ends one of two ways."

These are illustrative — final copy to be written by Canvas in the Advisor Copy document (separate deliverable, post-contract).

The override note is NOT modal, NOT blocking, NOT dismissable via a button. It auto-clears when the override is deactivated.

### 6.9 Goodnight Sequence — Active

Goodnight section expands to show:

- Step list with current step highlighted
- Progress indicator (step N of N)
- Cannot be dismissed by scrim click while Goodnight is running — the close button remains available but triggers a confirmation: "Goodnight sequence is running. Close panel? The sequence will continue." Confirmation is inline in the header, not a modal.

### 6.10 Goodnight Sequence — Complete

Goodnight section shows completion state.
"Goodnight Moon" ASCII art illustration surfaces here — centred in the Goodnight section, replacing the step list.
See Goodnight Complete spec (separate deliverable) for ASCII art dimensions and animation.
Status banner updates to: "Session complete — all systems parked" (or equivalent based on configured steps).

### 6.11 Safety Source — Disconnected

Shown when no safety source is connected at all.
This is distinct from Unsafe — the safety system cannot evaluate conditions, not that it has evaluated and found danger.

Status banner:

```
○ No Safety Source Connected
```

Background: color-background-secondary
Text: color-text-secondary

All source rows show inactive state.
A subdued inline message below the sources list:
"Safety monitoring requires at least one active source. Connect a source in Settings → Safety."

### 6.12 Safety Source — Error

One or more sources were connected but have stopped responding or returned an error. Distinct from intentionally disconnected.

Affected source row: red dot, "Connection error" status text, a retry icon (Phosphor ArrowClockwise 16px) as a trailing action.

Status banner: amber (warning-level, not unsafe-level) — "[Source name] connection error — monitoring degraded"

### 6.13 Initialising

Shown at application startup before the first evaluation pass.

Status banner:

```
⟳ Safety system initialising…
```

Skeleton loaders replace source rows (shimmer, see skeleton spec below).
Limit sliders render with last-known values from saved profile (greyed out, non-interactive until initialisation completes).

### 6.14 No Site Profile

Safety limits cannot be evaluated because no site or equipment profile exists.

Status banner:

```
○ No site profile configured
```

Limits section replaced by:

```
Safety limits require a site profile.
[Configure Site Profile →]
```

CTA button: secondary style, routes to Settings → Site Profile.

### 6.15 Night Mode Variants

All states above have Night Mode variants. Night Mode is a red overlay applied at the ThemeService level — it is not a separate theme, it is a composited layer.

Additional Night Mode considerations specific to Safety Panel:

- Status banner colours must remain distinguishable under the red overlay. Canvas will audit all banner/overlay colour combinations against the Night Mode red before handoff of final colour values.
- Status dots (green/amber/red) must remain functionally distinguishable. Under a red overlay, green dots may read as yellow/amber — this is an accessibility concern. Night Mode status dot variants use brightness differentiation in addition to hue: Connected = full brightness, Warning = medium, Error = pulsing. See Night Mode Accessibility Note below.

**Night Mode Accessibility Note:**
Under Night Mode overlay, hue-only status differentiation fails. Dark Sky's Night Mode status indicators use a dual-signal system: hue + brightness (All Clear = bright, Warning = medium, Error = low-brightness pulsing). This applies only in Night Mode — standard light/dark modes retain conventional traffic-light colours.

### 6.16 Closing

Reverse of Opening. See Section 8 — Animation.

---

## 7. Skeleton Spec (Initialising State)

| Property              | Value                                                              |
|-----------------------|--------------------------------------------------------------------|
| Source row skeleton   | 14px height, 180px width (name) + 60px (status)                   |
| Skeleton colour light | #E5E7EB → #F3F4F6                                                  |
| Skeleton colour dark  | #374151 → #4B5563                                                  |
| Animation             | Shimmer left-to-right, 400ms, linear, infinite                     |
| Reduce Motion         | Static skeleton, no shimmer                                        |
| Rows shown            | 3 skeleton rows (represents loading state, not exact source count) |

---

## 8. Animation

### Opening Sequence

| Step | Element          | Animation                | Duration                 | Easing          |
|------|------------------|--------------------------|--------------------------|-----------------|
| 1    | Scrim            | Opacity 0 → 0.45         | 200ms                    | ease-decelerate |
| 1    | Drawer           | TranslateX +520px → 0    | 250ms                    | ease-decelerate |
| 2    | Content sections | Opacity 0 → 1, staggered | 150ms each, 30ms stagger | ease-decelerate |

Steps 1 runs simultaneously. Step 2 begins after Step 1 completes.

### Closing Sequence

| Step | Element | Animation             | Duration | Easing          |
|------|---------|-----------------------|----------|-----------------|
| 1    | Drawer  | TranslateX 0 → +520px | 200ms    | ease-accelerate |
| 1    | Scrim   | Opacity → 0           | 200ms    | ease-accelerate |

### State Transitions (within open drawer)

| Transition            | Element        | Animation               | Duration | Easing          |
|-----------------------|----------------|-------------------------|----------|-----------------|
| All Clear → Warning   | Status banner  | Background cross-fade   | 250ms    | ease-standard   |
| Warning → Unsafe      | Status banner  | Background cross-fade   | 150ms    | ease-standard   |
| Any → Override Active | Override row   | Background fade in      | 250ms    | ease-standard   |
| Override deactivated  | Override row   | Background fade out     | 250ms    | ease-standard   |
| Source connects       | Status dot     | Opacity + scale 0.8→1.0 | 150ms    | ease-spring     |
| Section expand        | Section content | Height + opacity        | 200ms    | ease-decelerate |

### Error State Pulse (Safety Source Error + Override Active)

| Property      | Value                                      |
|---------------|--------------------------------------------|
| Animation     | Opacity 1.0 → 0.6 → 1.0                    |
| Duration      | 2000ms                                     |
| Easing        | ease-standard                              |
| Repeat        | Infinite while error/override state active |
| Reduce Motion | No pulse — static indicator only           |

### Reduce Motion

All translate animations → instant swap or simple fade (150ms max).
All stagger sequences → single simultaneous fade.
All pulsing → removed; static indicator only.
Respect prefers-reduced-motion system setting.

---

## 9. Interaction Behaviour

### Dismiss

- **Escape key:** Closes drawer. If Goodnight Active, triggers inline confirmation in header (see State 6.9).
- **Scrim click:** Closes drawer. Same Goodnight caveat.
- **Close button (×):** Closes drawer. Same Goodnight caveat.

### Limit Sliders

- Interactive when: Open — All Clear, Open — Warning, Override Unlocked, Override Active.
- Non-interactive when: Initialising (last known values shown, greyed out), No Site Profile (section replaced by CTA).
- Value updates: immediate visual update on drag, committed to settings on pointer-up / key-up.
- Keyboard: Arrow keys adjust by 1° or 0.1h increment. Shift+Arrow adjusts by 5° or 0.5h.
- Constraint: minimum altitude slider cannot be set below 0° or above 45°. Hour angle cannot exceed 6h.

### Safety Source Config Link

"config →" trailing link on connected source row. Routes to Settings → Safety → [Source Name]. Does not close the drawer.

### Override Toggle (Unlocked state only)

- Toggle ON: immediately applies override. No second confirmation. AI advisor note surfaces (if enabled). Override Active state begins.
- Toggle OFF: immediately deactivates override. All Clear state resumes (if no other issues). AI advisor note clears.

### Settings Link (Override Locked row)

Tapping "enable in Settings →" navigates to Settings → Safety. Drawer closes on navigation. On return, drawer state reflects updated Settings.

### "Run Goodnight Now" Button

- Triggers Goodnight sequence confirmation.
- Confirmation is inline within the Goodnight section — NOT a separate modal.
- Confirmation text: "Start Goodnight sequence? This will [summary of configured steps]."
- Two inline actions: "Start" (primary) and "Cancel" (ghost).
- On confirm: Goodnight Active state begins.

---

## 10. Accessibility

### Minimum Touch Targets

All interactive elements: 44×44px minimum.
Slider thumbs: 44×44px touch target around 16px visual thumb.

### Keyboard Navigation

- Drawer opens: focus moves to close button.
- Tab order: Close button → Status banner (read-only) → Sources list → Limits section → Horizon mask (skip if non-interactive) → Override section → Goodnight section.
- Escape: closes drawer (per Section 9).
- Slider keyboard interaction: per Section 9.

### Screen Reader Annotations (WinUI AutomationPeer)

| Element             | Role         | Label                                          | Value/State                   |
|---------------------|--------------|------------------------------------------------|-------------------------------|
| Drawer              | Pane         | "Safety Panel"                                 | —                             |
| Close button        | Button       | "Close Safety Panel"                           | —                             |
| Status banner       | StatusBar    | "Safety status"                                | Current status text           |
| Source row          | ListItem     | "[Source name]"                                | "Connected" / "Off" / "Error" |
| Limit slider        | Slider       | "[Limit name]"                                 | Current value + unit          |
| Override toggle     | ToggleButton | "[Limit name] override"                        | "On" / "Off"                  |
| Override locked row | Text         | "Safety overrides locked. Enable in Settings." | —                             |
| Goodnight button    | Button       | "Run Goodnight Sequence"                       | —                             |

### Contrast Ratios

All text/background pairings must meet:

- Body text on panel background: minimum 7:1 (AAA target)
- Status text on banner background: minimum 4.5:1 (AA minimum)
- Slider value readout: minimum 4.5:1
- Override locked row text: minimum 3:1 (large text exception, non-critical information)

Night Mode contrast audit: Canvas will complete before Stage 4.

### Focus Indicator

2px solid color-interactive-primary, 2px offset. Visible in all states and all modes including Night Mode.

---

## 11. Platform Notes (Windows / WinUI 3)

- Drawer implemented as WinUI 3 overlay panel or equivalent SplitView with DisplayMode="Overlay", OpenPanelLength=520.
- Scrim: a semi-transparent Rectangle covering ContentRoot, rendered below the drawer z-order.
- Sliders: WinUI Slider control, custom-styled via ControlTemplate to match token spec. Do not use default WinUI Slider visual — the thumb and track dimensions in this spec differ from defaults.
- Night Mode overlay: applied by ThemeService at the root layer. The Safety Panel does not implement Night Mode independently — it inherits from the ThemeService compositing layer.
- Escape key handling: register KeyboardAccelerator on the drawer Panel with Key=Escape.
- Reduce Motion: check SystemInformation.AnimationsEnabled and respect UISettings.AnimationsEnabled.

---

## 12. Out of Scope

The following are referenced in this contract but are separate deliverables with their own contracts:

- **Settings → Safety screen** — override enable toggle, one-time liability acknowledgment flow, source configuration. This is a dependency for Override Locked/Unlocked states. Must be contracted before or simultaneously with Safety Panel build begins.
- **Goodnight Complete state** — "Goodnight Moon" ASCII art illustration, dimensions, and animation. Referenced in State 6.10.
- **AI Advisor copy** — personality-tier microcopy for all Override Active advisor notes. Referenced in State 6.8.
- **Night Mode colour audit** — Night Mode status dot brightness differentiation values. Referenced in State 6.15.
- **Full Horizon Mask Editor** — lives in Observatory settings, not in this panel.

---

## 13. Open Items

| ID     | Item                                                        | Owner  | Blocking?                                                              |
|--------|-------------------------------------------------------------|--------|------------------------------------------------------------------------|
| OI-001 | Settings → Safety screen contract                           | Canvas | Yes — Override states cannot be fully built without it                 |
| OI-002 | Night Mode colour audit for status indicators               | Canvas | Yes — Night Mode variants cannot be finalised                          |
| OI-003 | Goodnight Complete ASCII art spec                           | Canvas | No — can ship without it, add in subsequent pass                       |
| OI-004 | AI Advisor copy (all 5 personality tiers)                   | Canvas | No — placeholder copy acceptable for initial build                     |
| OI-005 | Matt to confirm: visible-locked vs hidden override (DC-002) | Matt   | No — visible-locked is the current spec; hidden is a one-state removal |

---

## 14. Sketch Symbol Reference

| Symbol                               | Variants                                              |
|--------------------------------------|-------------------------------------------------------|
| SafetyPanel/Drawer/AllClear          | Default                                               |
| SafetyPanel/Drawer/Warning           | Default                                               |
| SafetyPanel/Drawer/Unsafe            | Default                                               |
| SafetyPanel/Drawer/OverrideLocked    | Default                                               |
| SafetyPanel/Drawer/OverrideActive    | Default                                               |
| SafetyPanel/Drawer/GoodnightActive   | Default                                               |
| SafetyPanel/Drawer/GoodnightComplete | Default                                               |
| SafetyPanel/Drawer/Disconnected      | Default                                               |
| SafetyPanel/Drawer/SourceError       | Default                                               |
| SafetyPanel/Drawer/Initialising      | Default                                               |
| SafetyPanel/Drawer/NoSiteProfile     | Default                                               |
| SafetyPanel/StatusBanner             | AllClear, Warning, Unsafe, Disconnected, Initialising |
| SafetyPanel/SourceRow                | Connected, Off, Error                                 |
| SafetyPanel/LimitSlider              | Default, Warning, Disabled                            |
| SafetyPanel/OverrideRow              | Locked, UnlockedOff, UnlockedOn                       |
| SafetyPanel/GoodnightSection         | Idle, Active, Complete                                |

Sketch page: Components → Section: Safety
Artboard naming: SafetyPanel — [State]

---

## Sign-off

| Role                | Name   | Status  | Date       |
|---------------------|--------|---------|------------|
| Design (Canvas)     | Canvas | Approved | 2026-04-09 |
| Engineering (Forge) | Forge  | Pending  | —          |
| Product (Matt)      | Matt   | Pending  | —          |

---

*Component Contract v1.0.0 — Dark Sky*
*Canvas / CombeCrew Design*
