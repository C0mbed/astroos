# Constraint-Based Scheduler

## Overview

The scheduler plans multi-target, multi-night sessions. Given a target list and a set of constraints, it produces an ordered execution plan that maximises imaging time while respecting all constraints.

## Scheduling Model

Each target carries a set of **constraints** and a **priority**:

```yaml
target:
  id: ngc1499
  name: NGC 1499 California Nebula
  ra: "04h03m14s"
  dec: "+36d22m00s"
  priority: 2
  quota:
    L: 180m
    Ha: 300m
  constraints:
    min_altitude_deg: 30
    max_moon_separation_deg: 45
    max_moon_illumination_pct: 60
    meridian_window_min: 60     # must have at least 60 min before meridian flip
    time_window:
      start: "21:00"
      end: "04:00"
```

## Scoring

For each candidate target, the scheduler computes a score:

```
score = priority_weight
      × altitude_score(current_alt)
      × moon_score(moon_sep, moon_illum)
      × urgency_score(remaining_quota)
      × window_score(time_in_optimal_window)
```

The highest-scoring target that passes all hard constraints is selected.

## Constraint Types

| Constraint | Type | Description |
|------------|------|-------------|
| `min_altitude_deg` | Hard | Target below this altitude is never selected |
| `max_moon_separation_deg` | Hard | Target too close to moon is rejected |
| `max_moon_illumination_pct` | Hard | Session halted if moon too bright |
| `meridian_window_min` | Hard | Skip target if flip would occur too soon |
| `time_window` | Hard | Target only valid in this local time range |
| `seeing_max_arcsec` | Soft | Penalise poor seeing, don't hard-block |
| `sky_quality_min_mpsas` | Soft | Penalise light-polluted sky |

## Weather Hold

When the weather station or sky quality monitor reports unsafe conditions, the scheduler enters a **weather hold**:
1. Pause current sequence.
2. Close dome / park mount.
3. Poll weather every N minutes.
4. Resume automatically when safe window > resume_threshold (default 30 min).

## Output: Session Plan

```json
{
  "date": "2026-04-03",
  "sunset": "20:14",
  "civil_dark": "21:02",
  "astronomical_dark": "21:58",
  "dawn": "05:12",
  "targets": [
    { "id": "ngc1499", "start": "21:58", "end": "01:30", "filters": ["L","Ha"] },
    { "id": "m45",     "start": "01:32", "end": "03:45", "filters": ["L","R","G","B"] }
  ],
  "total_imaging_time_h": 5.75
}
```
