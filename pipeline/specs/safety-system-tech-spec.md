# Tech Spec: Safety System
**Component:** Safety System — Hardware Limits, Goodnight Sequence, Site Monitor  
**Version:** 1.0.0  
**Date:** 2026-04-04  
**Author:** Canvas (CombeCrew Design)  
**Status:** Ready for Canvas Contract  
**Depends on:** Brand System v1.0.0, App Shell, hardware.driver-model, features/safety.md  

---

## Overview

The Safety System is three distinct but interconnected subsystems:

1. **Hardware Safety Limits** — mechanical constraints that prevent equipment collision and damage. Enforced by the platform regardless of user intent.
2. **Session Safety & Goodnight** — automated shutdown sequence triggered by session conditions (target altitude, dawn, weather). Each step is individually controllable.
3. **Site Safety Monitor** — live monitoring of the remote site's safety state, either from a configured hardware source or weather-based fallback.

All three subsystems feed into the Safety Indicator in the Status Bar (Zone A). All three are configured and monitored from the Safety Panel, accessible from the Equipment section of the Nav Rail or by clicking the Status Bar Safety zone.

---

## Subsystem 1: Hardware Safety Limits

### 1a. Pier Flip / Hour Angle Limits

**Purpose:** Prevent the optical tube from colliding with the pier during tracking or slewing.

**Data model:**

```
pier_limits:
  source: driver | user_override    # which value is active
  driver_east_ha: float             # hours, from mount driver (nullable)
  driver_west_ha: float             # hours, from mount driver (nullable)
  user_east_ha: float               # hours, user-configured
  user_west_ha: float               # hours, user-configured
  active_east_ha: float             # computed: driver unless overridden
  active_west_ha: float             # computed: driver unless overridden
  override_acknowledged: bool
  override_acknowledged_at: datetime
```

**Enforcement:**

- Before any slew: platform checks whether target RA/Dec at current LST falls within active HA limits
- If outside limits: slew is blocked. User is shown a clear message: which limit is violated, what the current value is, and what the override path is.
- During tracking: if tracking would carry the mount outside limits, the sequencer inserts a meridian flip or stops tracking with a Goodnight trigger
- Override path: user must complete the Override Acknowledgement flow (see Section 4) before the limit is bypassed for that session

**Driver behaviour:**

- On device connect: platform reads `driver_east_ha` and `driver_west_ha` from mount driver if available
- If driver exposes limits: `source` defaults to `driver`. Active limits shown with "From driver" badge.
- If driver does not expose limits: `source` defaults to `user_override`. Platform prompts user to configure.
- User-configured values are always persisted even when driver limits are active — they become the active values immediately when override is engaged

---

### 1b. Altitude Limits

**Purpose:** Prevent slewing below the horizon or into fixed obstructions; prevent tracking into the zenith blind spot if applicable.

**Data model:**

```
altitude_limits:
  minimum_alt: float        # degrees, default 15°
  maximum_alt: float        # degrees, default 89° (zenith blind spot)
  enforce_minimum: bool     # default true
  enforce_maximum: bool     # default false (most mounts handle zenith fine)
  horizon_mask_enabled: bool
  horizon_mask: HorizonMask  # see 1c
```

**Enforcement:** Same pattern as pier limits — slew blocked with clear error message, override path available.

---

### 1c. Horizon Mask (Full Polar Plot, V1)

**Purpose:** Per-azimuth altitude constraint map. Accounts for walls, trees, neighbouring buildings, and any fixed obstruction that creates an irregular horizon profile.

**Data model:**

```
HorizonMask:
  points: Array[{azimuth: float, altitude: float}]
  # azimuth 0–360°, altitude 0–90°
  # minimum 2 points, maximum 360 points (1° resolution)
  # platform interpolates linearly between points
  # if horizon_mask_enabled: minimum_alt is ignored (mask takes precedence)
```

**UI — Horizon Mask Editor:**

The editor is a polar plot displayed as a rectangular chart with azimuth on the X axis (0°–360°) and altitude on the Y axis (0°–90°). The user places points by clicking on the chart. The platform draws the interpolated line between points.

Quick setup path (for users who don't have a custom horizon):
- "Use flat horizon" button: sets two points (0°, [value]) and (360°, [value]) using the minimum altitude setting
- This is equivalent to the simple minimum altitude — it just stores it in the mask format

Import path (for users with existing horizon data):
- Import from file: accepts the Cartes du Ciel horizon format (.hrz) and plain CSV (az,alt)
- This is a V1 feature — horizon data is commonly shared in the astrophotography community

**Enforcement:** Before any slew, platform checks whether target alt at target azimuth is above the interpolated mask value at that azimuth. If below: blocked, same override path.

---

## Subsystem 2: Session Safety & Goodnight

### 2a. Goodnight Trigger Conditions

Any of these triggers initiates the Goodnight sequence:

| Trigger | Condition | Level |
|---|---|---|
| Target below minimum altitude | Target alt < `session_min_alt` (default 20°) | Graceful |
| Astronomical dawn | Time to dawn < configured lead time (default 30 min) | Graceful |
| Safety system: Level 2 abort | Cloud, wind, humidity limit | Graceful |
| Safety system: Level 3 abort | Hardware fault, UPS critical, rain | Immediate |
| Manual trigger | User initiates from Safety Panel or Power Menu | Graceful |
| Site monitor: UNSAFE | Safety broadcast or weather fallback signals unsafe | Graceful or Immediate (configurable) |

**Graceful vs. Immediate:**
- Graceful: completes current exposure, then proceeds through sequence in order
- Immediate: aborts current exposure immediately, proceeds through sequence at maximum speed

### 2b. Goodnight Sequence

Fixed order. Each step individually enable/disable-able. One optional post-sequence script hook.

```
Step 1: FINISH_OR_ABORT_EXPOSURE
  - Graceful: wait for current exposure to complete (max: remaining exposure time)
  - Immediate: send abort_exposure() to camera driver immediately
  - Always enabled — cannot be disabled
  - Cannot be overridden

Step 2: WARM_CAMERA
  - Enabled by default
  - Rate: +2°C per minute (configurable, min 0.5°C/min, max 5°C/min)
  - Target: ambient temperature or +10°C above current, whichever is lower
  - Platform polls camera.get_temperature() until within 2° of target or timeout (60 min)
  - Disable if: uncooled camera, or user preference

Step 3: PARK_MOUNT
  - Enabled by default
  - Sends mount.park() — mount goes to configured park position
  - Platform polls mount.is_parked() until true or timeout (10 min)
  - Disable if: user wants mount to remain at last position (remote access use case)

Step 4: CLOSE_FLAT_PANEL
  - Disabled by default (only enabled if flat panel / flip-flat is configured)
  - Sends flat_panel.close()
  - Platform polls flat_panel.is_closed() until true or timeout (2 min)
  - Only shown in UI if a flat panel device is connected

Step 5: CLOSE_DOME_OR_ROOF
  - Disabled by default (only enabled if dome / roof controller is configured)
  - Sends dome.close_shutter() and dome.park() if dome parking is configured
  - Platform polls dome.shutter_status == CLOSED until true or timeout (10 min)
  - Only shown in UI if a dome/roof device is connected
  - This step has its own safety interlock: if mount is not parked (Step 3 failed),
    platform will not close dome without explicit user confirmation
    Reason: closing dome on an unparked mount risks scope/dome collision

Step 6: DISCONNECT_DEVICES
  - Enabled by default
  - Disconnects devices in safe order: guider → camera → filter wheel → focuser → rotator → mount → dome
  - Mount is disconnected after dome to allow dome to complete any final movement
  - Disable if: user wants devices to remain connected (for remote access)

Step 7: REST_STATE
  - Always executes
  - Options (user-configured):
    a) App remains running, session complete screen shown
    b) App remains running, returns to idle state (default)
    c) App initiates PC shutdown after N minutes (user sets delay, default 10 min)
  - PC shutdown option requires explicit enable in settings — it is opt-in, never default

Step 8: CUSTOM_SCRIPT (optional)
  - Disabled by default
  - User provides a script path or command
  - Runs after all other steps complete (or after Step 7 if Step 7 = rest, before if shutdown)
  - 60 second timeout, then continues regardless
  - stdout/stderr captured to session log
```

### 2c. Individual Step Manual Triggers

Every Goodnight step can be triggered individually from:
- The Safety Panel (a "Run" button next to each step)
- The Power Menu (quick action: "Warm camera", "Park mount", "Close dome", etc.)
- The Device Detail Panel for the relevant device (e.g., "Warm up" in camera controls, "Park" in mount controls)

Individual triggers do NOT use the Goodnight sequence engine — they send the command directly to the device driver. This distinction matters: if the user manually warms the camera while a sequence is running, the sequence is not affected. Only the Goodnight sequence engine coordinates the full shutdown.

---

## Subsystem 3: Site Safety Monitor

### 3a. Configured Safety Broadcast

**Supported protocols (V1):**

| Protocol | Description | Config required |
|---|---|---|
| Safe-file (text) | A file on disk, updated by a weather station. Content: "safe" or "unsafe" (or 1/0). | File path, poll interval |
| ASCOM ObservingConditions | Standard ASCOM interface for weather/safety devices. Covers Boltwood, AAG, and many others via their ASCOM drivers. | ASCOM driver name, device index |
| HTTP endpoint | Platform polls a URL. Response must contain a JSON field `safe: true/false` or plain text "safe"/"unsafe". | URL, poll interval, auth (optional) |
| INDI weather driver | INDI `WEATHER_STATUS` property, standard value `OK`/`WARNING`/`ALERT`. | INDI server address, driver name |

**Polling:** Platform polls the configured source every `poll_interval_s` seconds (configurable per source, default 60s, minimum 10s).

**Signal loss handling:** If the safety source stops responding (timeout, connection error, file not updated within 3× poll interval), platform treats this as **UNSAFE** after a configurable grace period (default 2 minutes). The logic: a silent safety system is a failed safety system.

**Status:** Source health is displayed in the Safety Panel and the Site Monitor widget. Users see exactly which source is configured, when it last responded, and what it reported.

### 3b. Weather-Based Fallback

Active when: no safety broadcast source is configured, OR configured source is unavailable.

**Source:** The weather integration (Astrospheric or configured equivalent).

**Decision logic:**

```
UNSAFE if any of:
  - Cloud cover score < configured threshold (default: 40)
  - Wind speed > configured threshold (default: 40 km/h)
  - Precipitation probability > configured threshold (default: 20%)
  - Humidity > configured threshold (default: 85%)

CAUTION if any of:
  - Cloud cover score between threshold and threshold+20
  - Wind speed between threshold×0.7 and threshold
  - Humidity between threshold-10 and threshold

SAFE: none of the above
```

**Important UX note:** When weather fallback is active (vs. a configured safety device), the Safety Indicator in the Status Bar shows a distinct visual treatment — a `~` prefix or a small "forecast" icon — to communicate that the safety state is based on a weather forecast, not a real-time sensor. This distinction is critical. A Boltwood sensor at the observatory is ground truth. Astrospheric is a forecast. Users must never confuse the two.

### 3c. Safety State Machine

```
STATES:
  SAFE      — all conditions nominal, session may proceed
  CAUTION   — marginal conditions, monitoring closely, sequence continues
             — if CAUTION persists > configured duration (default 10 min), escalates to UNSAFE
  UNSAFE    — conditions exceeded threshold, Goodnight triggered
  OVERRIDE  — user has acknowledged and overridden an UNSAFE condition
             — session continues at user's explicit risk
             — override expires after configured duration (default: end of current sequence)

TRANSITIONS:
  SAFE → CAUTION: any condition enters marginal range
  CAUTION → SAFE: all conditions return to nominal (immediate)
  CAUTION → UNSAFE: CAUTION persists beyond duration, or any condition crosses hard threshold
  UNSAFE → GOODNIGHT: automatic (default) or manual (if user has disabled auto-Goodnight)
  UNSAFE → OVERRIDE: user completes Override Acknowledgement flow
  OVERRIDE → SAFE: conditions return to nominal, override automatically clears
  OVERRIDE → UNSAFE: conditions worsen beyond override threshold (configurable)
             — second override required, with stronger acknowledgement language
```

---

## Safety Panel — UI Requirements

The Safety Panel is a full-pane view accessible from the Equipment nav section (sub-navigation) and from clicking Status Bar Zone A.

### Panel Sections

**1. Current Status** (top, always visible)
- Large status indicator: SAFE / CAUTION / UNSAFE / OVERRIDE
- Source: "Boltwood Cloud Sensor via ASCOM" or "Weather forecast (Astrospheric)" with last-updated timestamp
- Current readings from safety source (whatever is available: cloud cover, wind, humidity, etc.)
- Active override indicator if OVERRIDE state, with time remaining and "End Override" button

**2. Hardware Limits**
- Pier flip limits: east HA / west HA, source badge ("From driver" or "User configured"), edit controls
- Altitude minimum: number input, degrees
- Altitude maximum: toggle + number input
- Horizon mask: toggle + "Edit Mask" button → opens Horizon Mask Editor (modal)

**3. Goodnight Configuration**
- Trigger conditions: checkboxes + value inputs for each trigger
- Sequence steps: ordered list with enable/disable toggle per step + per-step config (e.g., warm-up rate)
- "Test Goodnight" button: runs a dry-run simulation without sending any commands, shows estimated sequence duration
- "Run Goodnight Now" button (primary, destructive): triggers Goodnight with confirmation

**4. Site Monitor**
- Source selector: None / Safe-file / ASCOM ObservingConditions / HTTP Endpoint / INDI weather
- Source configuration: conditional fields based on selection
- Test button: validates the configured source right now
- Status: last poll time, last value, consecutive failures count
- Fallback behaviour: toggle "Use weather forecast as fallback if source unavailable"

**5. Manual Step Controls**
- Each Goodnight step listed individually with a "Run now" button
- Steps that require a device show the device connection state
- Steps not applicable (e.g., close flat panel when no flat panel configured) are greyed with explanation

---

## Override Acknowledgement Pattern

Used whenever a user bypasses a safety limit. Reusable across the entire app.

**Trigger:** User attempts an action that violates a configured safety limit.

**Flow:**

```
1. Action is BLOCKED
   Platform does not perform the action.
   A modal appears — cannot be dismissed by clicking outside.

2. Modal content:
   ┌─────────────────────────────────────────────┐
   │ ⚠  Safety Limit Override                    │
   │                                             │
   │ You are about to override a safety limit.   │
   │                                             │
   │ Limit:    West pier hour angle (3.5h)       │
   │ Current:  Your mount is at 3.8h west        │
   │ Risk:     Scope-to-pier collision possible  │
   │                                             │
   │ Overriding safety limits is done entirely   │
   │ at your own risk. Dark Sky accepts no       │
   │ responsibility for equipment damage         │
   │ resulting from limit overrides.             │
   │                                             │
   │ [Cancel]          [I understand, override]  │
   └─────────────────────────────────────────────┘

3. "I understand, override" button:
   - Requires a deliberate interaction — not a single click
   - Implementation: button is disabled for 2 seconds after modal opens
     (prevents accidental immediate click-through)
   - After 2s: button becomes active, label changes to bold, colour shifts to error style

4. On confirm:
   - Override is logged to session log with timestamp and user action
   - Platform proceeds with the overridden action
   - Status Bar Zone A shows OVERRIDE indicator for the duration
   - If override is for a positional limit: override is per-slew only (must re-acknowledge for next slew)
   - If override is for weather/safety: override persists for configured duration or until manually ended
```

**Override log entry format:**
```
[HH:MM:SS]  SAFETY OVERRIDE  [limit name]  [value exceeded]  User acknowledged
```

---

## Data Events (for Forge — Event Bus)

Safety system emits on `safety.*` namespace:

```
safety.state_changed         {from, to, reason, source}
safety.limit_violated        {limit_type, limit_value, current_value, action_blocked}
safety.override_acknowledged {limit_type, user_id, expires_at}
safety.override_expired      {limit_type}
safety.goodnight_triggered   {trigger, level}
safety.goodnight_step        {step, status: 'starting'|'complete'|'failed', detail}
safety.goodnight_complete    {duration_s, steps_completed, steps_failed}
safety.monitor_source_lost   {source_type, last_seen_at}
safety.monitor_source_restored {source_type}
```

---

## Open Questions for Forge

1. **Goodnight sequence configurability (pending Matt's answer):** Fixed order + enable/disable per step + optional post-sequence script. Confirm this is the agreed model before building the sequence engine.

2. **Rest state / PC shutdown (pending Matt's answer):** App stays running vs. optional PC shutdown after session. This affects Step 7 of the Goodnight sequence engine.

3. **ASCOM ObservingConditions:** Confirm the ASCOM bridge can expose the `IObservingConditions` interface. If the INDI bridge is the primary path for Linux/Mac (future), confirm INDI `WEATHER_STATUS` property is accessible via the existing INDI driver abstraction.

4. **Horizon mask interpolation:** Linear interpolation between user-placed points is the spec. Confirm Forge's preference for the interpolation implementation — this runs in the sequencer's pre-slew check, not in the UI.

5. **Override persistence:** Per-slew overrides for positional limits, session-duration overrides for weather limits. Confirm this is stored in session state (not persisted to user preferences — overrides should not survive an app restart).

