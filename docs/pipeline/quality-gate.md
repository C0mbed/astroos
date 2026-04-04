# Quality Gate

## Overview

The quality gate accepts or rejects each calibrated frame based on configurable metrics. Rejected frames are quarantined, never deleted.

## Metrics

| Metric | Description | Unit |
|--------|-------------|------|
| `fwhm` | Full Width at Half Maximum of average star PSF | arcsec |
| `eccentricity` | Average star elongation (0=round, 1=fully elongated) | dimensionless |
| `snr` | Signal-to-noise ratio of faintest accepted star | dimensionless |
| `star_count` | Number of stars detected in frame | count |
| `background_gradient` | Max/min background ratio (detects vignetting or light leak) | ratio |
| `clipping_fraction` | Fraction of pixels at saturation | 0–1 |

## Default Thresholds

```yaml
quality_gate:
  fwhm_max_arcsec: 4.5
  eccentricity_max: 0.5
  snr_min: 20
  star_count_min: 10
  background_gradient_max: 1.3
  clipping_fraction_max: 0.01
```

## Rejection Reasons

Each rejected frame is tagged with the primary rejection reason:

- `FWHM_TOO_HIGH` — poor seeing or focus drift
- `ECCENTRICITY_TOO_HIGH` — tracking error, wind shake, or miscollimation
- `SNR_TOO_LOW` — clouds or very short exposure
- `STAR_COUNT_TOO_LOW` — clouds obscuring field
- `BACKGROUND_GRADIENT` — light leak or strong gradient
- `CLIPPING` — overexposed (flat panel too bright, or target is too bright)
- `CALIBRATION_FAILURE` — upstream calibration step failed

## Adaptive Mode

When `adaptive: true`, the gate adjusts thresholds based on the session median:

```yaml
quality_gate:
  adaptive: true
  adaptive_fwhm_sigma: 1.5   # reject if > median + 1.5σ
```

This handles nights where seeing is variable — on a good night the gate is tighter, on a mediocre night it's looser, always relative to what's achievable in current conditions.

## Per-Sequence Override

Individual sequences can override global quality gate thresholds:

```yaml
sequence:
  quality_gate:
    fwhm_max_arcsec: 3.0     # tighter for high-res planetary imaging
    eccentricity_max: 0.3
```
