# Autofocus

## Overview

Autofocus runs a V-curve or hyperbolic curve fit to find the focus position that minimises star FWHM. It is triggered:
- At session start
- After a filter change (if temperature compensation is off or offset not calibrated)
- On a temperature delta trigger (configurable °C threshold)
- On a time trigger (configurable minutes interval)
- On manual request via UI / sequence step

## Methods

### 1. V-Curve (Linear)

Move focuser through N positions, measure FWHM at each. Fit two linear segments to find the vertex (focus point).

- Fast, works with most cameras and seeing conditions
- Less robust when seeing is highly variable during the run

### 2. Hyperbolic Fit

Same data collection, but fits a hyperbola to the full curve. More robust for fast optics (f/4 and below).

### 3. Bahtinov Mask (Manual / Semi-Auto)

User inserts Bahtinov mask. System detects spike pattern and reports defocus direction and magnitude. Not fully automated — intended for initial focus before an automated session.

## Temperature Compensation

The focuser can be driven in temperature-compensation mode between autofocus runs:

```yaml
autofocus:
  temperature_compensation:
    enabled: true
    coefficient_steps_per_degree_c: 48   # positive = extend on cooling
    source: focuser_temp_probe           # or: ambient, dew_heater_probe
```

## Autofocus Run Config

```yaml
autofocus:
  method: hyperbolic
  step_size: 50           # focuser steps between samples
  num_steps: 9            # total positions sampled (step_size × num_steps = range)
  binning: 2x2            # coarser binning for speed
  exposure_s: 5
  filter: L               # always focus on L (apply offsets for other filters)
  filter_offsets:         # pre-calibrated steps from L focus position
    R: -12
    G: -8
    B: +15
    Ha: +42
    OIII: +38
  min_star_count: 5       # abort if fewer stars detected
  max_fwhm_threshold: 8   # arcsec — reject sample if seeing spike detected
  retry_on_failure: true
  max_retries: 2
```

## Output

Each autofocus run appends to the session log:

```json
{
  "timestamp": "2026-04-03T22:14:33Z",
  "filter": "L",
  "position_steps": 24850,
  "fwhm_arcsec": 2.31,
  "temperature_c": 8.4,
  "method": "hyperbolic",
  "r_squared": 0.9934,
  "samples": [...]
}
```
