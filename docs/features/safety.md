# Safety System

## Overview

The safety system is the highest-priority component in AstroOS. It runs on its own polling loop, independent of the sequencer, and can trigger an emergency abort at any time.

## Safety Checks

| Check | Source | Unsafe Condition |
|-------|--------|-----------------|
| Cloud cover | Weather station / all-sky camera | Clouds detected |
| Wind speed | Anemometer | > configurable threshold (e.g., 40 km/h) |
| Rain / humidity | Hygrometer / rain sensor | Rain detected or humidity > 85% |
| Dew point delta | Temp + humidity | Mirror dew risk (delta < 3°C) |
| Hardware error | Device error events | Unrecoverable mount/camera fault |
| Power | UPS monitor | Battery < threshold, mains lost |
| User-defined | Script / webhook | Any custom condition |

## Abort Levels

### Level 1 — Soft Abort
Pause current sequence. Keep dome open. Re-evaluate in 10 minutes.

Triggered by: temporary cloud cover, wind gust spike.

### Level 2 — Hard Abort
Stop imaging. Park mount. Close dome/roof. Notify user.

Triggered by: sustained bad weather, rain, humidity limit, power event.

### Level 3 — Emergency Abort
Immediate park, close dome. Kill all hardware connections. Notify user urgently.

Triggered by: hardware fault mid-slew, UPS battery critical, loss of communication with mount.

## Configuration

```yaml
safety:
  poll_interval_s: 30
  weather_source: boltwood_cloud_sensor   # plugin name
  wind_max_kmh: 40
  humidity_max_pct: 85
  dew_delta_min_c: 3
  resume_after_soft_abort_min: 10
  notifications:
    - channel: pushover
      on_levels: [2, 3]
    - channel: email
      on_levels: [3]
```
