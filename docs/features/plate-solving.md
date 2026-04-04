# Plate Solving

## Overview

Plate solving identifies the exact pointing position of the telescope by matching star patterns in a captured frame against a star catalog. AstroOS uses plate solving for:

- **Centering** — accurate GoTo after a slew (sync mount to solved position)
- **Blind solving** — establish pointing model from scratch
- **Frame annotation** — tag archive frames with accurate WCS headers
- **Drift detection** — catch mount tracking errors during long sequences

## Solver Backends (Plugin-Based)

| Backend | Type | Notes |
|---------|------|-------|
| `astrometry_net_local` | Local | Highest accuracy, requires index files (~20–40 GB) |
| `astap` | Local | Fast, compact indexes, recommended default |
| `astrometry_net_online` | Remote | Fallback, requires internet, slower |

## Solve Flow

```
capture short exposure (configurable, default 5s)
    → extract star list (SEP or similar)
        → submit to solver backend
            → receive WCS solution
                → compute pointing error (RA/Dec delta)
                    → if error > threshold: sync mount + re-slew
                        → re-solve and verify
                            → pass: proceed with sequence
                            → fail after N retries: abort
```

## Configuration

```yaml
plate_solving:
  backend: astap
  astap:
    executable: /usr/local/bin/astap
    index_path: /data/astap/indexes
    search_radius_deg: 30
  solve_exposure_s: 5
  solve_binning: 2x2
  max_pointing_error_arcmin: 1.0
  max_retries: 3
  annotate_archive_frames: true   # write WCS to all archived FITS
```

## WCS Output

Solved frames get these FITS headers added:

```
WCSAXES = 2
CTYPE1  = 'RA---TAN'
CTYPE2  = 'DEC--TAN'
CRVAL1  = 83.82...
CRVAL2  = -5.39...
CRPIX1  = 2048.0
CRPIX2  = 1536.0
CD1_1   = -0.000122
CD1_2   = 0.0
CD2_1   = 0.0
CD2_2   = 0.000122
PLTSOLVD= T
PLTSOLV = 'ASTAP'
```
