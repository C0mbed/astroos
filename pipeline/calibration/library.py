"""Calibration library — selects master frames for a given light frame."""

from __future__ import annotations

import logging
from pathlib import Path

from ..frame import CalibrationRefs, FrameContext

log = logging.getLogger(__name__)

DARK_TEMP_TOLERANCE_C = 2.0


class CalibrationLibrary:
    """
    Locates master calibration frames that match a light frame's parameters.

    Masters are stored under a root directory:
        masters/bias/{camera}_{binning}_{gain}.fits
        masters/dark/{camera}_{binning}_{gain}_{exposure}s_{temp}C.fits
        masters/flat/{camera}_{filter}_{binning}_{date}.fits
    """

    def __init__(self, root: Path):
        self._root = Path(root)

    def find_masters(self, frame: FrameContext) -> CalibrationRefs:
        refs = CalibrationRefs()
        refs.dark_path = self._find_dark(frame)
        if refs.dark_path is None:
            refs.bias_path = self._find_bias(frame)
        refs.flat_path = self._find_flat(frame)
        return refs

    def _find_dark(self, frame: FrameContext) -> Path | None:
        dark_dir = self._root / "masters" / "dark"
        if not dark_dir.exists():
            return None
        candidates = list(dark_dir.glob(
            f"{_safe(frame.camera_id)}_{frame.binning}_{frame.gain}_"
            f"{frame.exposure_s:.0f}s_*.fits"
        ))
        if not candidates:
            return None
        if frame.temp_sensor_c is not None:
            candidates = _filter_by_temp(candidates, frame.temp_sensor_c)
        return candidates[0] if candidates else None

    def _find_bias(self, frame: FrameContext) -> Path | None:
        bias_dir = self._root / "masters" / "bias"
        if not bias_dir.exists():
            return None
        candidates = list(bias_dir.glob(
            f"{_safe(frame.camera_id)}_{frame.binning}_{frame.gain}.fits"
        ))
        return candidates[0] if candidates else None

    def _find_flat(self, frame: FrameContext) -> Path | None:
        flat_dir = self._root / "masters" / "flat"
        if not flat_dir.exists():
            return None
        candidates = sorted(
            flat_dir.glob(f"{_safe(frame.camera_id)}_{frame.filter}_{frame.binning}_*.fits"),
            reverse=True,  # most recent first
        )
        return candidates[0] if candidates else None


def _safe(s: str | None) -> str:
    return (s or "unknown").replace(" ", "_")


def _filter_by_temp(candidates: list[Path], target_temp: float) -> list[Path]:
    def temp_from_path(p: Path) -> float | None:
        for part in p.stem.split("_"):
            if part.endswith("C"):
                try:
                    return float(part[:-1])
                except ValueError:
                    pass
        return None

    return [
        c for c in candidates
        if (t := temp_from_path(c)) is None or abs(t - target_temp) <= DARK_TEMP_TOLERANCE_C
    ]
