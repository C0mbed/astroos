# FITS Archive

## Overview

Every frame that passes the quality gate is written to the FITS archive. The archive is the authoritative data store — it is append-only and content-addressed.

## Directory Layout

```
archive/
└── {year}/
    └── {month}/
        └── {date}/
            └── {target}/
                └── {filter}/
                    └── {camera}_{target}_{filter}_{exposure}s_{datetime}_{index}.fits
```

Example:
```
archive/2026/04/03/M42/L/ASI2600MM_M42_L_300s_20260403T220145Z_001.fits
```

## Frame Metadata Database

In addition to the filesystem, all frame metadata is indexed in a SQLite database (or PostgreSQL for larger installations). This enables fast queries like:

- "All L frames of M42 taken in April 2026 with FWHM < 3 arcsec"
- "Total integration time on NGC 1499 in Hα"
- "Frames with temperature drift > 2°C during exposure"

### Schema (simplified)

```sql
CREATE TABLE frames (
    id          TEXT PRIMARY KEY,   -- UUID
    path        TEXT NOT NULL,
    checksum    TEXT NOT NULL,      -- SHA-256
    target      TEXT,
    filter      TEXT,
    exposure_s  REAL,
    gain        INTEGER,
    binning     TEXT,
    camera_id   TEXT,
    date_obs    TEXT,               -- ISO 8601 UTC
    ra          REAL,               -- degrees, from WCS or mount
    dec         REAL,
    fwhm        REAL,               -- arcsec
    eccentricity REAL,
    snr         REAL,
    star_count  INTEGER,
    background  REAL,
    temp_c      REAL,               -- sensor temp at exposure start
    state       TEXT,               -- PASSED, FAILED, QUARANTINED
    rejection_reason TEXT
);
```

## Integrity Verification

Every archived frame is checksummed (SHA-256) at write time. The archive runs a scheduled integrity scan that re-checksums all files and flags any that don't match the stored hash.

```yaml
archive:
  integrity_scan:
    schedule: "0 6 * * *"   # daily at 6am
    on_failure: alert        # alert | quarantine | delete
```

## Quarantine

Frames that fail the quality gate are not deleted — they are moved to a quarantine directory with the same structure as the archive:

```
quarantine/2026/04/03/M42/L/ASI2600MM_M42_L_300s_..._rejection_FWHM_TOO_HIGH.fits
```

The rejection reason is appended to the filename and stored in the metadata DB. Quarantined frames can be re-evaluated if thresholds are later adjusted.
