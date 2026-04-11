# Contract: Device Setup & First-Run Flow

**Version:** 1.0.0
**Date:** 2026-04-09
**Author:** Canvas
**Status:** Canvas ✅ — Forge + Matt sign-off needed

---

## Pipeline References

- **Informs:** Settings → Equipment (future contract)
- **Introduces:** In-App Driver Picker component (reused in Settings → Equipment)
- **Introduces:** Equipment Database (sensor specs, driver manifest)
- **Relates to:** Device Connect Contract v1.0.0, Status Bar Amendment v1.2.0
- **Forge build targets:**
  - `DarkSky.App / Views / Setup / FirstRunWizard.xaml`
  - `DarkSky.App / Controls / DriverPicker.xaml`
  - `DarkSky.App / Services / DeviceDetectionService.cs`
  - `DarkSky.App / Data / EquipmentDatabase.json`
  - `DarkSky.App / Services / DriverManifestService.cs`

---

## Core Design Principle

> Device setup should feel like Apple designed it. Not Ubuntu.
>
> The user plugs in their camera. Dark Sky knows what it is. It has the
> right driver ready. It knows the recommended settings. The user
> confirms, connects, and images. That is the entire flow.

**No ASCOM Chooser dialog. No manual ProgID entry. No hunting for drivers. No leaving the app.** Dark Sky owns the entire experience.

---

## 1. System Architecture Overview

| Service                  | What it does                                                                        | When it runs                             |
|--------------------------|-------------------------------------------------------------------------------------|------------------------------------------|
| `DeviceDetectionService` | USB VID/PID scan via WMI/WinRT — identifies physically connected hardware           | On app launch, on USB plug/unplug events |
| `AscomProfileService`    | Enumerates installed ASCOM drivers via `Profile.GetDrivers()`                       | On app launch, after any driver install  |
| `AlpacaDiscoveryService` | UDP broadcast on port 32227 — finds network Alpaca devices                          | On app launch, on network change         |
| `DriverManifestService`  | Fetches and caches the Dark Sky driver manifest JSON — maps models to download URLs | On app launch (cached; refresh daily)    |

The Equipment Database is a bundled JSON file (`EquipmentDatabase.json`) shipped with the app. It maps hardware models to sensor specs and recommended settings. Always available offline.

---

## 2. Flow Map

### 2.1 First-Run (no equipment profile exists)

```
App launch
  └── No equipment profile in SQLite
        └── [WIZARD OPENS — full screen overlay]
              │
              ├── Screen 1: Welcome & Scan
              │     Running: USB scan + ASCOM enum + Alpaca discovery (~2–3s)
              │
              ├── Screen 2: Devices Found
              │     Lists detected devices by type. Driver status per device.
              │
              ├── [For each device with driver NOT installed]
              │     ├── Screen 3a: Driver Download (parallel progress)
              │     └── Screen 3b: Driver Install (user runs installer)
              │
              ├── Screen 4: Equipment Profile
              │     Name the profile, confirm gear list, auto-fill sensor specs
              │
              ├── Screen 5: Connect & Confirm
              │     Connect All — per-device animated progress + live telemetry
              │
              └── Screen 6: Setup Complete
                    "Your rig is ready" — navigate to Sequencer or Device Connect
```

### 2.2 Subsequent-Use: New Device Detected

```
USB plug-in event detected during normal use
  └── Not in active profile
        └── Status Bar notification strip appears:
              "New device detected: ZWO ASI2600MM Pro — Add to profile?"
              [Add →]  [Dismiss]
                    └── Opens Driver Picker (modal) → adds to profile → does NOT auto-connect
```

### 2.3 Add Device Manually

```
Settings → Equipment → "Add Device"
  └── Opens Device Type selector
        └── Opens In-App Driver Picker (modal) for that type
```

---

## 3. Screen 1 — Welcome & Scan

### 3.1 Layout

```
┌─────────────────────────────────────────────────────────────────┐
│                                                                 │
│                  [Dark Sky logotype — 48px]                     │
│                                                                 │
│              Set up your equipment                              │
│                                                                 │
│          We're scanning for connected devices.                  │
│                                                                 │
│         ┌──────────────────────────────────────┐               │
│         │  ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░  │  ← scan bar   │
│         └──────────────────────────────────────┘               │
│                                                                 │
│         USB Devices  ···      ASCOM Drivers  ✓                  │
│         Alpaca Network  ···                                     │
│                                                                 │
│                    [Skip setup — I'll do this later]            │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### 3.2 Spec

| Element              | Spec                                                                              |
|----------------------|-----------------------------------------------------------------------------------|
| Heading              | "Set up your equipment" — text-display-sm (36px / 700)                            |
| Subheading           | "We're scanning for connected devices." — text-body-lg / color-text-secondary     |
| Scan bar             | 360px wide, 4px height, radius-full. Shimmer left-to-right, 1200ms infinite.      |
| Status icon: pending | "···" — animated ellipsis, dot-by-dot, 600ms cycle                                |
| Status icon: complete | "✓" — color-status-success                                                       |
| Status icon: failed  | "✕" — color-status-error                                                          |
| Skip link            | "Skip setup — I'll do this later" — text-label / color-text-secondary             |

Auto-advances to Screen 2 after all services return or timeout at 5s.

---

## 4. Screen 2 — Devices Found

### 4.1 Layout

```
┌─────────────────────────────────────────────────────────────────┐
│  ←  Back                                          1 of 5  ●●○○○ │
│                                                                 │
│  Here's what we found                                           │
│                                                                 │
│  ┌─ CAMERAS ───────────────────────────────────────────────┐    │
│  │  [ZWO logo]  ZWO ASI2600MM Pro         ✓  Driver ready  │    │
│  │              ASCOM.ASICamera2.Camera                    │    │
│  └─────────────────────────────────────────────────────────┘    │
│  [+ Add camera]                                                 │
│                                                                 │
│  ┌─ FOCUSERS ──────────────────────────────────────────────┐    │
│  │  [ZWO logo]  ZWO EAF                   ⬇  Get driver    │    │
│  │              Driver not installed                       │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                 │
│                                          [Continue  →]          │
└─────────────────────────────────────────────────────────────────┘
```

### 4.2 Device Group Spec

| Element                   | Spec                                                              |
|---------------------------|-------------------------------------------------------------------|
| Device row height         | 56px                                                              |
| Manufacturer logo         | 24×24px — bundled asset. Fallback: device type icon.             |
| Driver status — ready     | "✓ Driver ready" — color-status-success / text-label             |
| Driver status — not installed | "⬇ Get driver" — color-interactive-primary / text-label      |
| Driver status — unknown   | "? Driver unknown" — color-text-secondary / text-label           |

### 4.3 Auto-Detection Match Confidence

| Match Type                               | Display                                                  |
|------------------------------------------|----------------------------------------------------------|
| High — USB VID/PID exact match           | Device name shown. Driver pre-selected.                  |
| Medium — ASCOM driver installed, no USB  | Driver name shown. "Connected via ASCOM" tag.            |
| Low — Alpaca network device              | Device name from Alpaca response. "Network device" tag.  |
| None                                     | "Not detected" with "Add manually" option                |

### 4.4 Continue Button Logic

| Situation                        | Button label                | Enabled? |
|----------------------------------|-----------------------------|----------|
| All drivers installed or unknown | "Continue →"                | Yes      |
| 1+ drivers need downloading      | "Get Drivers & Continue →"  | Yes      |
| No devices added at all          | "Continue without devices →" | Yes      |

---

## 5. In-App Driver Picker Component

Replaces the ASCOM Chooser dialog entirely. A **modal overlay** used in three contexts: First-Run Wizard, Settings → Equipment, and New Device Detected flow.

### 5.1 Component Anatomy

```
┌───────────────────────────────────────────────────────────────┐
│  Select Camera Driver                                    [✕]  │
│  ─────────────────────────────────────────────────────────    │
│  [🔍  Search drivers…                                     ]    │
│                                                               │
│  INSTALLED                                                    │
│  ──────────────────────────────────────────────────────────   │
│  ● [ZWO]  ZWO ASI Camera (Universal)      ASCOM.ASICamera2…   │
│  ● [QHY]  QHYCCD Unified Driver           ASCOM.QHYCCD…       │
│                                                               │
│  AVAILABLE TO DOWNLOAD                                        │
│  ──────────────────────────────────────────────────────────   │
│  ○ [P1A]  Player One Astronomy Camera     ASCOM.POA…  [⬇]    │
│  ○ [ATK]  Atik Cameras Driver             ASCOM.Atik… [⬇]    │
│                                                               │
│  OTHER ASCOM DRIVERS                                          │
│  ──────────────────────────────────────────────────────────   │
│  ○ [ASCOM] Generic ASCOM Camera           Any camera…         │
│                                                               │
│  ─────────────────────────────────────────────────────────    │
│  [Cancel]                                   [Select Driver]   │
└───────────────────────────────────────────────────────────────┘
```

### 5.2 Dimensions

| Property            | Value                          |
|---------------------|--------------------------------|
| Modal width         | 560px fixed                    |
| Modal max height    | 600px (scrollable list within) |
| Modal elevation     | elevation-4                    |
| Header height       | 52px                           |
| Search bar height   | 40px                           |
| Driver row height   | 48px                           |
| Group header height | 28px                           |
| Footer height       | 56px                           |

### 5.3 Driver Row Spec

| Element           | Spec                                                                        |
|-------------------|-----------------------------------------------------------------------------|
| Radio indicator   | 16px circle. Selected: color-interactive-primary fill.                       |
| Manufacturer logo | 24×24px asset. Fallback: device type glyph.                                  |
| Driver name       | text-body-sm / 600 / color-text-primary                                      |
| ProgID            | text-caption / color-text-secondary / JetBrains Mono, truncated at 28 chars  |
| Download chip     | "⬇" icon only, 28×28px. Downloading does not select — selection = radio.    |
| Recommended badge | Shown when Dark Sky detected this hardware via USB and this is matched driver |
| Row selected      | color-interactive-primary at 8% fill + radio filled                          |

### 5.4 Pre-Selected State (Auto-Detection)

When opened from a high-confidence USB detection match, the matched driver is pre-selected, scrolled into view, and the "Recommended" badge is shown. User can confirm immediately.

**This is the Apple moment:** you connected a ZWO ASI2600MM Pro, we opened the picker, the right driver is already highlighted. Press confirm and you're done.

### 5.5 Download Chip Interaction

1. Chip → spinner (downloading to `%TEMP%\DarkSky\Drivers\`)
2. Complete → chip changes to "⬆ Install"
3. User clicks Install → Windows UAC prompt for installer
4. Dark Sky polls ASCOM Profile Store at 3s intervals for up to 60s
5. Driver detected → row animates up to "Installed" section (ease-spring 400ms)

### 5.6 Accessibility

| Requirement      | Spec                                                                  |
|------------------|-----------------------------------------------------------------------|
| Role             | `dialog` — `AutomationProperties.Name` = "Select [DeviceType] Driver" |
| Focus trap       | Focus cycles within modal on Tab                                      |
| Initial focus    | Search field on open                                                  |
| Escape           | Dismisses (same as Cancel)                                            |
| List navigation  | Arrow keys to move between rows. Space/Enter to select.               |

---

## 6. Driver Download & Install Flow (Screens 3a / 3b)

### Screen 3a — Downloading

All drivers download in parallel. Progress bars per driver. Auto-advances to Screen 3b when complete.

### Screen 3b — Installing

```
┌─────────────────────────────────────────────────────────────────┐
│  Install your drivers                                           │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  [ZWO logo]  ZWO ASI Camera Driver v2.1.4.0             │    │
│  │                              [Install — 18.2 MB  ↗ ]   │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### Install Row States

| State                   | Appearance                                                          |
|-------------------------|---------------------------------------------------------------------|
| Ready to install        | "Install — [size] ↗" button                                         |
| Installing (waiting)    | Spinner + "Waiting for installation to complete…"                   |
| Installed — detected    | "✓ Installed — [ProgID] detected" — color-status-success            |
| Skipped                 | "Skipped — you can change this in Settings → Equipment"             |
| Failed (timeout)        | "✕ Not detected" — color-status-error + "Try again" link            |

---

## 7. Screen 4 — Equipment Profile

Profile name field (default: "My Observatory") + device cards with auto-populated sensor specs from Equipment Database.

Each device card shows:
- Sensor data (if database match found)
- Driver ProgID in JetBrains Mono
- [Edit] link to open Driver Picker

---

## 8. Screen 5 — Connect & Confirm

The "wow" moment. Every device connects, live telemetry appears, and the user sees real numbers from their hardware for the first time.

### Device Row — Connection States

| State           | Progress bar             | Status text                     | Color                    |
|-----------------|--------------------------|---------------------------------|--------------------------|
| Connecting      | Animated shimmer         | "Connecting…"                   | color-interactive-primary |
| Connected       | Full bar, static         | "Connected ✓"                   | color-status-success     |
| Connected + telemetry | Full bar + expand  | "Connected ✓"                   | color-status-success     |
| Failed          | No bar                   | "Connection failed — [error]"   | color-status-error       |

### Telemetry Expand Animation

Height expands + content fades in. Numeric values count up from 0 to live value over 400ms — this is the wow moment. Reduce Motion: instant expand, values at final state immediately.

### Telemetry per Device Type

- **Camera:** Sensor temp (°C), cooling target, gain, offset, binning
- **Mount:** RA/Dec (HMS/DMS), tracking mode, pier side
- **Focuser:** Step position, temperature
- **Filter Wheel:** Current filter name/position
- **Rotator:** Current angle (°)

---

## 9. Screen 6 — Setup Complete

```
┌─────────────────────────────────────────────────────────────────┐
│                                                                 │
│                   ✦                                             │
│                                                                 │
│              Your rig is ready.                                 │
│                                                                 │
│            3 devices connected and ready to image.             │
│                                                                 │
│         ┌──────────────────────────────────────────┐           │
│         │         Open Sequencer  →                │           │
│         └──────────────────────────────────────────┘           │
│                                                                 │
│              Or go to  Device Connect                           │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

- ✦ sparkle: 32px, color-interactive-primary. Entrance: scale 0 + rotate 45° over 600ms ease-spring.
- Heading: "Your rig is ready." — text-display-sm (36px / 700)
- No back button — the wizard is complete.

---

## 10. Subsequent-Use: New Device Detected Notification

```
┌───────────────────────────────────────────────────────────┐
│  🔌  New device detected: ZWO ASI2600MM Pro    [Add →]  ✕  │
└───────────────────────────────────────────────────────────┘
```

| Element      | Spec                                                               |
|--------------|--------------------------------------------------------------------|
| Strip height | 36px                                                               |
| Background   | color-background-secondary                                         |
| Auto-dismiss | After 30 seconds if no interaction                                 |
| [Add →]      | Opens Driver Picker for detected device type, driver pre-selected  |

Adding to profile does NOT auto-connect. The user connects manually in Device Connect.

---

## 11. Equipment Database Spec

### 11.1 Schema

```json
{
  "version": "1.0.0",
  "cameras": [
    {
      "id": "zwo-asi2600mm-pro",
      "manufacturer": "ZWO",
      "model": "ASI2600MM Pro",
      "sensor": "Sony IMX571",
      "pixelSizeMicrons": 3.76,
      "resolutionX": 6248,
      "resolutionY": 4176,
      "sensorWidthMm": 23.5,
      "sensorHeightMm": 15.7,
      "adcBits": 16,
      "recommendedGain": 100,
      "recommendedOffset": 50,
      "hcgGain": 100,
      "coolingDeltaC": 35,
      "bayerPattern": null,
      "usbVid": "0x03C3",
      "usbPid": "0x571F",
      "ascomProgId": "ASCOM.ASICamera2.Camera",
      "nativeSdkSupported": true
    }
  ],
  "driverManifest": {
    "version": "1.0.0",
    "lastUpdated": "2026-04-09",
    "drivers": [
      {
        "id": "zwo-asicamera2",
        "manufacturer": "ZWO",
        "friendlyName": "ZWO ASI Camera (Universal)",
        "ascomProgId": "ASCOM.ASICamera2.Camera",
        "deviceType": "Camera",
        "currentVersion": "2.1.4.0",
        "downloadUrl": "https://[manifest-cdn]/drivers/zwo-asicamera2-2.1.4.0.exe",
        "fileSizeBytes": 19087360,
        "sha256": "[hash]",
        "supportedModels": ["ASI2600MM Pro", "ASI2600MC Pro", "ASI6200MM Pro"]
      }
    ]
  }
}
```

### 11.2 Driver Manifest Update Strategy

- `EquipmentDatabase.json` sensor data: bundled only, always offline ✅
- `driverManifest` section: fetched from Dark Sky CDN, cached daily ✅

---

## 12. Accessibility

| Requirement          | Spec                                                                                          |
|----------------------|-----------------------------------------------------------------------------------------------|
| Wizard overlay       | `role="dialog"` `aria-modal="true"` `AutomationProperties.Name="Dark Sky Setup"`              |
| Scan status          | `aria-live="polite"` — announces completions                                                  |
| Connection progress  | `aria-live="polite"` — announces each device connection                                       |
| Progress bars        | `role="progressbar"` with `aria-valuenow` / `aria-valuemin` / `aria-valuemax`                 |
| Notification strip   | `aria-live="assertive"` — new device detection is user-relevant                               |
| Driver picker        | Full spec in Section 5.6                                                                      |

---

## 13. Open Items

| ID     | Item                                                                                               | Blocking?                          |
|--------|----------------------------------------------------------------------------------------------------|------------------------------------|
| DS-001 | Equipment Database seed data — Canvas to deliver (ZWO, QHY, Player One, Moravian, Atik, SX, FLI) | Not blocking schema / picker build |
| DS-002 | Manufacturer logo asset library — 24×24px logos. Canvas to deliver as asset pack.                 | Not blocking flow build            |
| DS-003 | Driver manifest CDN endpoint — Forge to spec and host.                                             | Blocks download flow testing only  |
| DS-004 | "Help →" in connection error rows — deferred to future contract. Button renders but does nothing.  | No                                 |

---

## 14. Sign-Off

| Role   | Status     | Date       |
|--------|------------|------------|
| Canvas | ✅ Approved | 2026-04-09 |
| Forge  | ⬜ Pending  | —          |
| Matt   | ⬜ Pending  | —          |

---

*Device Setup & First-Run Flow Contract v1.0.0 — Dark Sky*
*Canvas / CombeCrew Design*
