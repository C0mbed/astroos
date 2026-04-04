# Sequencer

## Overview

The sequencer is the core execution engine of an imaging session. It takes a **sequence** — an ordered list of steps — and executes them against connected hardware, handling errors, retries, and aborts.

## Sequence Definition

A sequence is defined in YAML:

```yaml
sequence:
  id: m42-lrgb-night1
  target:
    name: M42
    ra: "05h35m17.3s"
    dec: "-05d23m28s"
  steps:
    - type: slew
      params:
        settle_s: 10
    - type: autofocus
      params:
        filter: L
        step_size: 50
        steps: 9
    - type: loop
      count: 30
      steps:
        - type: expose
          params:
            filter: L
            duration_s: 300
            binning: 1x1
            gain: 100
        - type: dither
          params:
            pixels: 15
            settle_s: 8
    - type: loop
      count: 10
      steps:
        - type: expose
          params:
            filter: R
            duration_s: 300
            binning: 1x1
```

## Step Types

| Step | Description |
|------|-------------|
| `slew` | Slew mount to target coordinates |
| `sync` | Sync mount to plate-solved position |
| `expose` | Take a single exposure (camera + filter wheel) |
| `autofocus` | Run autofocus routine |
| `dither` | Command guider to dither |
| `loop` | Repeat child steps N times (or until time limit) |
| `wait` | Wait for condition (time, altitude, keypress) |
| `guide_start` | Start guiding |
| `guide_stop` | Stop guiding |
| `flat` | Automated flat frame capture (with flat panel or sky) |
| `park` | Park mount |

## Error Handling

Each step can declare a retry policy:

```yaml
- type: autofocus
  retry:
    max_attempts: 3
    on_failure: abort   # or: skip, retry, notify
```

Top-level session abort policy is declared per-sequence and defines what to do when an unrecoverable error occurs (e.g., `park_and_close_dome`).

## Abort Safety

An abort can be triggered by:
- User via UI / API
- Safety system (weather, hardware error)
- Pipeline quality gate (too many failed frames)

On abort, the sequencer runs the cleanup chain: stop guiding → stop imaging → park mount → notify.
