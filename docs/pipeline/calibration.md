# Frame Calibration

## Overview

Calibration removes systematic sensor artifacts from raw light frames before analysis or stacking.

## Calibration Equation

```
calibrated = (raw - master_bias - master_dark) / master_flat
```

Where:
- `master_bias` — median stack of bias frames (zero-duration, same gain, same temp)
- `master_dark` — median stack of dark frames (same duration, gain, temp ±2°C)
- `master_flat` — normalised median stack of flat frames (same filter, binning)

If a master dark includes bias signal (as is common), the bias step is skipped to avoid double-subtraction:

```
calibrated = (raw - master_dark) / master_flat
```

## Master Frame Selection

Masters are matched against the light frame on these fields (in priority order):

| Field | Match Type |
|-------|-----------|
| Camera ID | Exact |
| Binning | Exact |
| Gain | Exact |
| Temperature | ±2°C (darks only) |
| Exposure duration | Exact (darks) / N/A (bias, flat) |
| Filter | Exact (flats only) |
| Date | Most recent valid master |

If no matching master exists, the frame is flagged `CALIBRATION_MISSING` and proceeds uncalibrated (with a warning). This is configurable — some setups prefer to abort instead.

## Master Creation

Masters are created from raw calibration frames via the calibration manager:

```
POST /api/v1/calibration/create-master
{
  "type": "dark",
  "frames": ["path/to/raw1.fits", "path/to/raw2.fits", ...],
  "stacking_method": "median",   // mean, median, sigma-clip
  "sigma_low": 3.0,
  "sigma_high": 3.0
}
```

Masters are automatically recreated when enough new raw calibration frames are available (configurable threshold).

## Pedestal / Offset Handling

Some cameras output negative values after bias subtraction. AstroOS adds a configurable pedestal to prevent negative pixel values that break downstream tools:

```yaml
calibration:
  pedestal_adu: 100
```
