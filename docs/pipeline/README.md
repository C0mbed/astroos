# Imaging Pipeline Design

## Overview

The AstroOS pipeline processes every captured light frame through a series of stages. The pipeline is **per-frame** and **synchronous within a session** — each frame completes the pipeline before the next exposure begins (or runs in a background worker if the next exposure is underway).

## Pipeline Stages

```
[1] Ingest       — validate FITS header, record metadata
[2] Calibrate    — subtract bias/dark, divide flat
[3] Analyse      — measure FWHM, eccentricity, background, SNR
[4] Quality Gate — accept or reject frame based on metrics
[5] Plate Solve  — add WCS headers (optional, configurable per sequence)
[6] Archive      — write to FITS store with checksum
[7] Stack        — add to live stack (optional)
```

## Stage Docs

| Stage | Doc |
|-------|-----|
| Calibration | [calibration.md](calibration.md) |
| Analysis | [analysis.md](analysis.md) |
| Quality Gate | [quality-gate.md](quality-gate.md) |
| Archive | [archive.md](archive.md) |
| Live Stacking | [live-stacking.md](live-stacking.md) |

## Frame States

```
CAPTURED → CALIBRATED → ANALYSED → PASSED → ARCHIVED
                                  ↘ FAILED → QUARANTINED
```

## Failure Handling

- A failed frame is moved to a quarantine directory, never deleted.
- The failure reason, measured metrics, and threshold values are logged.
- If the failure rate exceeds a configurable threshold (e.g., > 30% of frames in a row), the sequencer is notified and may abort or notify the user.

## Calibration Frame Management

The pipeline maintains a **master calibration library**:

```
calibration/
├── masters/
│   ├── bias/
│   │   └── {camera}_{temp}_{binning}_{gain}.fits
│   ├── dark/
│   │   └── {camera}_{temp}_{binning}_{gain}_{exposure}.fits
│   └── flat/
│       └── {camera}_{filter}_{binning}_{date}.fits
└── raws/
    ├── bias/
    ├── dark/
    └── flat/
```

Masters are selected automatically by matching camera, temperature (within ±2°C for darks), binning, gain, and exposure.
