# Component Contract: Device Connect

**Version:** 1.0.0
**Date:** 2026-04-09
**Status:** Canvas ✅ — Forge + Matt sign-off needed
**Author:** Canvas
**Pipeline Stage:** 2 — Design & Component Contract

---

## 0. Contract Scope

This contract covers two related but distinct surfaces:

**Surface A — Device Connect Screen**
A full-screen, top-level navigation mode. The primary surface for connecting, disconnecting, and inspecting all hardware. Accessed via the navigation rail and the Status Bar quick-action button.

**Surface B — Status Bar Connection Widget**
A permanent Status Bar resident (Zone B). Always visible regardless of active mode. Provides at-a-glance connection status and a Connect All shortcut without requiring navigation to the full screen.

---

## 0.1 Design Principles

**No popups.** Every action, status, and configuration lives on the screen.

**The full picture, always.** All devices are visible simultaneously.

**Per-device granularity with global convenience.** Connect All and Disconnect All are first-class actions.

**Troubleshooting is a real use case.** The screen must work as a diagnostic surface, not just a pre-session checkbox.

---

## 0.2 Contract Notes & Design Calls

**DC-001 — Detail expansion is inline, not a side panel.**
When a user expands a device for configuration or diagnostics, the detail expands inline below the device row. Rationale: side panels create a nested drawer pattern that competes with the Safety Panel overlay language. Inline expansion keeps the device list visible at all times.

**DC-002 — Device rows are fixed height in collapsed state.**
72px per row, regardless of device type. Specialisation happens in the expanded detail only.

**DC-003 — Connecting state uses per-device progress, not a global spinner.**
Each device shows its own connection progress independently.

---

## 1. Surface A — Device Connect Screen

### 1.1 Layout

```
+--[SCREEN: full viewport]---------------------------------------+
|  [NAVIGATION RAIL — left, per app shell spec]                 |
|                                                                 |
|  +--[CONTENT AREA: full width minus nav rail]----------------+ |
|  |                                                            | |
|  |  [PAGE HEADER]                                            | |
|  |  +------------------------------------------------------+ | |
|  |  | Devices                        [Connect All]         | | |
|  |  |                                [Disconnect All]      | | |
|  |  +------------------------------------------------------+ | |
|  |                                                            | |
|  |  [CONNECTION SUMMARY BAR]                                 | |
|  |  +------------------------------------------------------+ | |
|  |  | ● 6 of 8 devices connected   [1 error]  [1 offline] | | |
|  |  +------------------------------------------------------+ | |
|  |                                                            | |
|  |  [DEVICE LIST]                                            | |
|  |  | ● Mount      10Micron GM2000    Connected [Disconnect]| | |
|  |  | ● Camera     ZWO ASI2600MM     Connected  [Disconnect]| | |
|  |  | ● Guide Cam  ZWO ASI290MM      Connected  [Disconnect]| | |
|  |  |   +--[INLINE DETAIL]-------------------------------+  | | |
|  |  |   | Driver:   ASCOM.ZWO.Camera                    |  | | |
|  |  |   | Temp:     -10.2°C    Gain: 100 (unity)        |  | | |
|  |  |   | [Full Settings →]                             |  | | |
|  |  |   +-----------------------------------------------+  | | |
|  |  | ○ Focuser    ZWO EAF           Disconnected [Connect] | | |
|  |  | ⚠ Rotator    Pegasus FlatMaster  Error     [Retry]    | | |
|  |  | ○ Flat Panel  —               Not Configured [Setup →]| | |
|  |  | ○ Weather    ASCOM ObsConditions Disconnected [Connect]| | |
|  |  +------------------------------------------------------+ | |
|  |  | + Add Device                                          | | |
|  +------------------------------------------------------------+ |
+----------------------------------------------------------------+
```

---

### 1.2 Page Header

| Property                       | Value                                    |
|--------------------------------|------------------------------------------|
| Height                         | 72px                                     |
| Horizontal padding             | space-8 (32px)                           |
| Bottom border                  | 1px color-border-default                 |
| Page title                     | "Devices" — text-display-sm (36px / 700) |
| Connect All button             | Primary button style                     |
| Disconnect All button          | Secondary/ghost button style             |
| Connect All — disabled when    | All devices already connected            |
| Disconnect All — disabled when | All devices already disconnected         |

---

### 1.3 Connection Summary Bar

| Property           | Value                      |
|--------------------|----------------------------|
| Height             | 40px                       |
| Horizontal padding | space-8 (32px)             |
| Background         | color-background-secondary |
| Border bottom      | 1px color-border-subtle    |
| Typography         | text-body-sm (14px / 400)  |

**States** (see also Status Bar Amendment v1.2.0 for Session Ready state):

| State               | Text                                             | Indicator              |
|---------------------|--------------------------------------------------|------------------------|
| All Connected       | "All [N] devices connected"                      | ● color-status-success |
| Partially Connected | "[N] of [N] devices connected"                   | ● color-status-warning |
| All Disconnected    | "No devices connected"                           | ○ color-text-tertiary  |
| Connecting          | "Connecting… [N] of [N]"                         | Spinner (animated)     |
| Error present       | Appends " · [N] error" in color-status-error     | —                      |
| Offline present     | Appends " · [N] offline" in color-text-secondary | —                      |

---

### 1.4 Device Row — Collapsed

```
+--[ROW CONTAINER: 72px height, full content width]------------+
|                                                               |
|  [STATUS DOT 10px]  [DEVICE TYPE ICON 24px]                  |
|                     [DEVICE NAME — text-body-lg / 600]       |
|                     [DRIVER NAME — text-body-sm / 400]       |
|                                                 [STATUS TEXT] |
|                                              [PRIMARY ACTION] |
|                                              [EXPAND CHEVRON] |
+---------------------------------------------------------------+
```

| Property              | Value                                                |
|-----------------------|------------------------------------------------------|
| Row height            | 72px fixed                                           |
| Horizontal padding    | space-8 (32px) left, space-6 (24px) right            |
| Status dot            | 10px diameter, 12px left of icon                     |
| Device type icon      | 24×24px, Phosphor Duotone                            |
| Primary action button | 32px height, ghost/secondary style, right-aligned    |
| Expand chevron        | 20×20px, rightmost element                           |
| Min touch target      | 44×44px on all interactive elements                  |
| Row separator         | 1px color-border-subtle                              |

**Device type icons (Phosphor Duotone):**

| Device Type     | Icon              |
|-----------------|-------------------|
| Mount           | Compass           |
| Imaging Camera  | Camera            |
| Guide Camera    | Aperture          |
| Focuser         | SlidersHorizontal |
| Filter Wheel    | CirclesFour       |
| Rotator         | ArrowsClockwise   |
| Flat Panel      | Rectangle         |
| Dew Controller  | Drop              |
| Weather Station | CloudSun          |
| Aux / Generic   | Cpu               |

---

### 1.5 Device Row — States

#### Connected
| Element        | Value                            |
|----------------|----------------------------------|
| Status dot     | color-status-success, solid      |
| Status text    | "Connected" color-status-success |
| Primary action | "Disconnect" ghost button        |

#### Disconnected
| Element        | Value                                         |
|----------------|-----------------------------------------------|
| Status dot     | color-border-default, hollow ring (2px stroke) |
| Status text    | "Disconnected" color-text-tertiary            |
| Primary action | "Connect" primary button (subdued)            |

#### Connecting
| Element        | Value                                               |
|----------------|-----------------------------------------------------|
| Status dot     | Replaced by 10px spinner, color-interactive-primary |
| Status text    | "Connecting…" color-text-secondary                  |
| Primary action | Disabled, "Connecting…" label                       |

#### Error
| Element        | Value                                        |
|----------------|----------------------------------------------|
| Status dot     | color-status-error, solid                    |
| Status text    | "Error" color-status-error                   |
| Primary action | "Retry" ghost button, color-status-error      |
| Row background | color-status-error-subtle                    |

#### Not Configured
| Element        | Value                                                     |
|----------------|-----------------------------------------------------------|
| Status dot     | color-border-default, hollow ring, 50% opacity            |
| Device name    | color-text-secondary                                      |
| Driver name    | "No driver selected" color-text-tertiary                  |
| Status text    | "Not Configured" color-text-tertiary                      |
| Primary action | "Setup →" — routes to Settings → Devices → [Type]        |
| Expand chevron | Hidden                                                    |

#### Offline / Unavailable
| Element        | Value                                |
|----------------|--------------------------------------|
| Status dot     | color-text-tertiary, hollow ring      |
| Status text    | "Offline" color-text-tertiary         |
| Primary action | "Connect" (reconnect attempt)         |

---

### 1.6 Device Row — Expanded (Inline Detail)

```
+--[ROW HEADER: 72px — unchanged]------------------------------+
|  ● Mount   10Micron GM2000    Connected   [Disconnect]   ∧   |
+---------------------------------------------------------------+
|  +--[INLINE DETAIL: variable height]----------------------+  |
|  |  DRIVER        ASCOM.10Micron.Telescope                |  |
|  |  RA            14h 29m 43.0s                           |  |
|  |  DEC           +20° 05' 42"                            |  |
|  |  ALT           52.4°      AZ    184.2°                 |  |
|  |  TRACKING      Sidereal                                |  |
|  |                                                         |  |
|  |  [Full Settings →]     [ASCOM Setup →]                 |  |
|  +----------------------------------------------------------+  |
```

| Property           | Value                                  |
|--------------------|----------------------------------------|
| Horizontal padding | space-8 (32px) left, space-6 (24px) right |
| Top padding        | space-4 (16px)                         |
| Bottom padding     | space-5 (20px)                         |
| Background         | color-background-secondary             |
| Property label     | text-label (12px / 500 / ALL CAPS) color-text-tertiary |
| Property value     | text-mono (13px / 400) color-text-primary |

**Detail properties by device type:**

| Device Type    | Properties                                           |
|----------------|------------------------------------------------------|
| Mount          | Driver, RA, Dec, Alt, Az, Tracking Rate, Pier Side   |
| Imaging Camera | Driver, Sensor Temp, Cooler Power, Gain, Offset, Bin |
| Guide Camera   | Driver, Sensor Temp, Gain                            |
| Focuser        | Driver, Position, Temperature, Backlash              |
| Filter Wheel   | Driver, Current Filter Slot, Slot Count              |
| Rotator        | Driver, Position Angle, Mechanical Angle             |
| Weather        | Driver, Temp, Humidity, Cloud Cover, Sky Quality     |

---

### 1.7 Add Device Row

| Property           | Value                                                     |
|--------------------|-----------------------------------------------------------|
| Height             | 48px                                                      |
| Typography         | text-body-sm (14px / 400) color-text-secondary            |
| Icon               | Phosphor Plus 16px, color-text-secondary                  |
| Action             | Routes to Settings → Devices → Add Device                 |
| Separator above    | 1px color-border-subtle                                   |

---

### 1.8 Empty State

```
+--[CONTENT AREA]--------------+
|                               |
|    [Icon: Cpu 48px]           |
|    No devices configured      |
|    Add your equipment to      |
|    start connecting.          |
|    [Add Your First Device]    |
|                               |
+-------------------------------+
```

---

### 1.9 Connect All / Disconnect All Behaviour

**Connect All:** Initiates connection for all Disconnected and Offline devices simultaneously. Not Configured devices are skipped. Button changes to "Connecting…" while any device is in Connecting state.

**Disconnect All:** Triggers inline confirmation within the Connection Summary Bar — NOT a modal:

```
Disconnect all devices? Sessions in progress will be interrupted.
[Disconnect All]  [Cancel]
```

---

### 1.10 Screen-Level Colors

| Element                   | Light Token                | Dark Token                 |
|---------------------------|----------------------------|----------------------------|
| Page background           | color-background-primary   | color-background-primary   |
| Summary bar background    | color-background-secondary | color-background-secondary |
| Row background (default)  | color-background-primary   | color-background-primary   |
| Row background (hover)    | color-background-secondary | color-background-secondary |
| Row background (expanded) | color-background-secondary | color-background-secondary |
| Row background (error)    | color-status-error-subtle  | color-status-error-subtle  |
| Row separator             | color-border-subtle        | color-border-subtle        |

---

## 2. Surface B — Status Bar Connection Widget

See also: Status Bar Amendment v1.2.0 (adds Session Ready state + Zone B dimensions).

### 2.1 Widget Anatomy (Collapsed — default)

| Property             | Value                    |
|----------------------|--------------------------|
| Widget height        | 40px (full Status Bar)   |
| Status dot           | 8px diameter             |
| Label                | text-label (12px / 500)  |
| Action chip          | Pill shape, 28px height  |
| Min touch target     | 44×44px                  |

### 2.2 Widget States

#### All Connected
| Element        | Value                     |
|----------------|---------------------------|
| Status dot     | color-status-success      |
| Label          | "6/6 Connected"           |
| Action chip    | Hidden                    |

#### Partially Connected
| Element          | Value                                        |
|------------------|----------------------------------------------|
| Status dot       | color-status-warning                         |
| Label            | "4/6 Connected"                              |
| Action chip      | "Connect All" — primary chip style           |
| Action chip click | Triggers Connect All directly               |

#### All Disconnected
| Element          | Value                                        |
|------------------|----------------------------------------------|
| Status dot       | color-border-default, hollow ring            |
| Label            | "Disconnected"                               |
| Action chip      | "Connect All" — primary chip style           |

#### Connecting
| Element    | Value                                   |
|------------|-----------------------------------------|
| Status dot | Spinner, 8px, color-interactive-primary |
| Label      | "Connecting…"                           |
| Action chip | Hidden while connecting                |

#### Error
| Element    | Value                           |
|------------|---------------------------------|
| Status dot | color-status-error              |
| Label      | "[N] Error" or "1 Error"        |
| Action chip | "Retry" chip                   |

---

### 2.3 Widget Popover

Opens on click of the widget. 320px wide, anchored below widget. No scrim. Dismissed by clicking outside or pressing Escape.

```
+--[POPOVER: 320px wide]------------------------+
|                                                |
|  Devices                    [Manage Devices →] |
|  ────────────────────────────────────────────  |
|  ● Mount          10Micron GM2000  Connected   |
|  ● Camera         ZWO ASI2600MM   Connected    |
|  ○ Focuser        ZWO EAF         Disconnected |
|  ⚠ Rotator        Pegasus         Error        |
|  ────────────────────────────────────────────  |
|  [Connect All]             [Disconnect All]    |
|                                                |
+------------------------------------------------+
```

| Property      | Value                                  |
|---------------|----------------------------------------|
| Width         | 320px fixed                            |
| Max height    | 400px with internal scroll             |
| Border radius | radius-lg (12px)                       |
| Z-index       | 800 (below Safety Panel drawer at 900) |
| Padding       | space-4 (16px)                         |

---

## 3. Animation

### Screen — Row Expand / Collapse

| Property         | Value                                                |
|------------------|------------------------------------------------------|
| Detail panel     | Height 0 → content height                            |
| Duration         | 200ms                                                |
| Easing           | ease-decelerate (expand), ease-accelerate (collapse) |
| Chevron rotation | 0° → 180°, 200ms ease-standard                       |

### Status Bar Widget

| Property           | Value                                                |
|--------------------|------------------------------------------------------|
| State changes      | Label cross-fade, 200ms ease-standard                |
| Action chip appear | Width expand + opacity, 200ms ease-decelerate        |
| Action chip dismiss | Width collapse + opacity, 150ms ease-accelerate     |

---

## 4. Accessibility

### Screen Reader Annotations

| Element                 | Role      | Label                               |
|-------------------------|-----------|-------------------------------------|
| Page                    | Page      | "Device Connect"                    |
| Summary bar             | StatusBar | "Connection summary"                |
| Device list             | List      | "Devices"                           |
| Device row (collapsed)  | ListItem  | "[Device name], [driver]"           |
| Connect button (row)    | Button    | "Connect [device name]"             |
| Disconnect button (row) | Button    | "Disconnect [device name]"          |
| Connect All             | Button    | "Connect all devices"               |
| Status Bar widget       | Button    | "Device connections"                |
| Popover                 | Dialog    | "Device connections"                |

---

## 5. Platform Notes (Windows / WinUI 3)

- Device list: WinUI ListView with custom ItemTemplate. VisualStateManager for hover, selected, expanded states.
- Row expansion: RowDefinition Height animation or VisualStateManager. Do not use Visibility.Collapsed — use height 0 to enable animation.
- Status Bar widget: UserControl resident in Status Bar. Popover = WinUI Flyout, FlyoutPlacementMode = Bottom.
- Connect All: async, cancellation-token aware. UI must not freeze during connection sequence.
- ASCOM driver connections: may be slow (2–10 seconds per device). Each row's Connecting state must be independently cancellable.

---

## 6. Open Items

| ID     | Item                                                                               | Blocking? |
|--------|------------------------------------------------------------------------------------|-----------|
| OI-001 | App Shell contract — Zone B dimensions (**Resolved by Status Bar Amendment v1.2.0**) | No       |
| OI-002 | Maximum supported device count — scroll or paginate?                               | No        |
| OI-003 | ASCOM Setup → behaviour — Dark Sky native UI or driver's own dialog?               | No        |

---

## 7. Sign-off

| Role                | Name   | Status  | Date       |
|---------------------|--------|---------|------------|
| Design (Canvas)     | Canvas | Approved | 2026-04-09 |
| Engineering (Forge) | Forge  | Pending  | —          |
| Product (Matt)      | Matt   | Pending  | —          |

---

*Component Contract v1.0.0 — Dark Sky*
*Canvas / CombeCrew Design*
