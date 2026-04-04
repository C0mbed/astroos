# Frame Analysis

## Overview

The analysis stage measures frame quality metrics from the calibrated image. These metrics feed into the quality gate and are stored permanently in the frame metadata regardless of whether the frame passes or fails.

## Star Detection

Stars are extracted using SEP (Source Extractor as a Python library) or a compatible backend:

1. Estimate and subtract background using a mesh-based estimator
2. Detect sources above `detection_threshold` × background RMS
3. Filter: remove sources touching frame edge, saturated sources, extended objects
4. Compute shape parameters for each star: FWHM, ellipticity, position angle

## FWHM Measurement

FWHM is computed per-star using a Gaussian fit to the PSF, then the **median** of the top-N brightest non-saturated stars is used as the frame FWHM.

Conversion from pixels to arcseconds requires the image scale:

```
arcsec_per_pixel = (pixel_size_micron / focal_length_mm) × 206.265
```

Camera pixel size and focal length are read from the equipment profile config.

## Eccentricity

Eccentricity is derived from the semi-major (a) and semi-minor (b) axes of the star PSF ellipse:

```
eccentricity = sqrt(1 - (b/a)²)
```

Frame eccentricity = median across detected stars. Values close to 0 indicate round stars; values approaching 1 indicate severe elongation (tracking error, wind, or miscollimation).

## SNR

SNR is estimated per-star using the aperture photometry model:

```
SNR = signal / sqrt(signal + n_pix × (sky_rms² + read_noise²))
```

The reported frame SNR is the SNR of the faintest star used in the analysis (configurable percentile).

## Background Statistics

- Background level (ADU)
- Background RMS
- Background gradient (ratio of brightest to darkest background tile)

## Metadata Output

All metrics are written to:
1. The frame's FITS header (as custom keywords)
2. The metadata database entry for the frame

```
ASTR_FW  = 2.31       / FWHM arcsec
ASTR_ECC = 0.12       / Eccentricity
ASTR_SNR = 42.3       / Frame SNR
ASTR_NSTR= 87         / Star count
ASTR_BKGD= 312.4      / Background ADU
ASTR_BGRD= 1.04       / Background gradient ratio
ASTR_CLIP= 0.0002     / Clipping fraction
```
