"""Frame context — the single object that flows through all pipeline stages."""

from __future__ import annotations

import uuid
from dataclasses import dataclass, field
from enum import Enum
from pathlib import Path


class FrameState(str, Enum):
    CAPTURED = "CAPTURED"
    CALIBRATED = "CALIBRATED"
    ANALYSED = "ANALYSED"
    PASSED = "PASSED"
    FAILED = "FAILED"
    QUARANTINED = "QUARANTINED"


class FrameType(str, Enum):
    LIGHT = "LIGHT"
    BIAS = "BIAS"
    DARK = "DARK"
    FLAT = "FLAT"


@dataclass
class FrameMetrics:
    """Quality metrics measured during the analysis stage."""
    fwhm_arcsec: float | None = None
    eccentricity: float | None = None
    snr: float | None = None
    star_count: int | None = None
    background_adu: float | None = None
    background_gradient: float | None = None
    clipping_fraction: float | None = None


@dataclass
class CalibrationRefs:
    """Paths to master calibration frames applied to this frame."""
    bias_path: Path | None = None
    dark_path: Path | None = None
    flat_path: Path | None = None


@dataclass
class FrameContext:
    """
    Carries all state about a single frame through the pipeline.

    Stages read from and write to this object. No stage modifies the raw file
    — each stage either operates on a working copy or writes metadata.
    """
    raw_path: Path
    id: str = field(default_factory=lambda: str(uuid.uuid4()))
    frame_type: FrameType = FrameType.LIGHT
    state: FrameState = FrameState.CAPTURED

    # Populated from FITS header during ingest
    target: str | None = None
    filter: str | None = None
    exposure_s: float | None = None
    gain: int | None = None
    binning: str | None = None
    camera_id: str | None = None
    date_obs: str | None = None          # ISO 8601 UTC
    temp_sensor_c: float | None = None

    # Set during calibration
    calibrated_path: Path | None = None
    calibration_refs: CalibrationRefs = field(default_factory=CalibrationRefs)

    # Set during analysis
    metrics: FrameMetrics = field(default_factory=FrameMetrics)

    # Set after quality gate
    rejection_reason: str | None = None

    # Set after plate solve
    ra_center_deg: float | None = None
    dec_center_deg: float | None = None
    wcs_solved: bool = False

    # Set after archive
    archive_path: Path | None = None
    checksum: str | None = None

    def reject(self, reason: str) -> None:
        self.state = FrameState.FAILED
        self.rejection_reason = reason

    def to_dict(self) -> dict:
        return {
            "id": self.id,
            "raw_path": str(self.raw_path),
            "state": self.state,
            "target": self.target,
            "filter": self.filter,
            "exposure_s": self.exposure_s,
            "gain": self.gain,
            "fwhm": self.metrics.fwhm_arcsec,
            "eccentricity": self.metrics.eccentricity,
            "snr": self.metrics.snr,
            "star_count": self.metrics.star_count,
            "rejection_reason": self.rejection_reason,
            "archive_path": str(self.archive_path) if self.archive_path else None,
            "checksum": self.checksum,
        }
