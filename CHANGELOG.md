# Changelog

All notable changes to AstroOS (Dark Sky) are recorded here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [Unreleased]

### Added

- **Status Bar v1.1.0** — `StatusBar.xaml` / `StatusBar.xaml.cs`
  - Zones A and B implemented; Zones C and D marked as stubs (out of scope v1.1.0)
  - Zone A: Safety Panel trigger button (Segoe Fluent Icons shield glyph placeholder;
    Phosphor Duotone integration deferred to Safety Panel contract build)
  - Zone A: `SafetyStatusAutomationLabel` computed from current safety state
  - Zone B: `ConnectionWidget` UserControl hosted within min 160px / max 220px column
  - Top border and Zone A/B divider borders via `color-border-default` / `color-border-subtle` tokens
  - Status Bar Contract v1.1.0

- **Connection Widget v1.1.0** — `ConnectionWidget.xaml` / `ConnectionWidget.xaml.cs`
  - Six visual states via WinUI 3 VisualStateManager: `AllDisconnected`, `Connecting`,
    `PartiallyConnected`, `AllConnected`, `SessionReady`, `ErrorPresent`
  - Animated chip appear/dismiss via MaxWidth + Opacity `DoubleAnimation` on `VisualTransition`
  - `GeneratedDuration="0:0:0.2"` default; `0:0:0.3` entering SessionReady; `0:0:0.15` leaving
  - Reduce Motion respected: `UISettings.AnimationsEnabled` read at control load;
    `GoToState(..., useTransitions: false)` skips all transitions and generated interpolation
  - Action chip pill shape (28px height, `CornerRadius=14`, 8px horizontal padding)
  - Device Connect Contract v1.0.0 §2 + Status Bar Amendment v1.2.0 §3

- **Session Ready state** — Amendment v1.2.0
  - `IsSessionReady` computed property on `StatusBarViewModel`: all four conditions (devices
    connected without errors, no safety warnings, valid site profile, sequencer idle/stopped)
  - `"Session Ready"` label in `color-status-success` (unique: only state with coloured label text)
  - `"Start →"` action chip navigating to Sequencer mode
  - Status Bar Amendment v1.2.0

- **`color-status-warning-accessible` token** — Brand System Amendment v1.1.0
  - Light: `#B45309` (4.7:1 WCAG AA with white text)
  - Dark: `#F59E0B` (10.5:1 AAA — same as `color-status-warning` dark mode)
  - Full `Tokens.xaml` token layer populated with Light / Dark / Default ThemeDictionaries
  - All 15 semantic colour tokens defined (background, border, text, interactive, status)

- **Service interface stubs** (no implementations — for future build stages)
  - `IConnectionService` — device connection state, Connect/Disconnect All
  - `ISafetyService` — aggregate safety status
  - `ISequencerService` — sequence engine state
  - `ISiteProfileService` — site profile validity

- **Model enums**
  - `ConnectionState` (6 values including `SessionReady`)
  - `SafetyStatus` (AllClear, NoSource, WarningActive, Unsafe)
  - `SequencerState` (Idle, Running, Stopped, Paused)

- **`StatusBarViewModelTests` stubs** — `tests/AstroOS.UI.Tests/`
  - 8 test method stubs for Crucible to implement
  - xUnit + Moq project targeting `net9.0-windows10.0.22000.0`
  - `AstroOS.UI.Tests` registered in `AstroOS.sln`

---

*AstroOS — Dark Sky*
*CombeCrew Engineering*
