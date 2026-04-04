"""Stage 2 — Calibrate: subtract bias/dark, divide flat."""

from __future__ import annotations

import logging
import shutil
import tempfile
from pathlib import Path

import numpy as np
from astropy.io import fits

from ..frame import FrameContext, FrameState

log = logging.getLogger(__name__)


class CalibrateStage:
    def __init__(self, enabled: bool = True, library=None):
        self._enabled = enabled
        self._library = library  # CalibrationLibrary instance, injected at runtime

    def process(self, frame: FrameContext) -> FrameContext:
        if not self._enabled:
            frame.calibrated_path = frame.raw_path
            return frame

        if self._library is None:
            log.warning("No calibration library configured — skipping calibration")
            frame.calibrated_path = frame.raw_path
            return frame

        refs = self._library.find_masters(frame)
        frame.calibration_refs = refs

        with fits.open(frame.raw_path) as hdul:
            data = hdul[0].data.astype(np.float32)
            header = hdul[0].header.copy()

        if refs.dark_path:
            dark = fits.getdata(refs.dark_path).astype(np.float32)
            data = data - dark
            header["CALDRK"] = str(refs.dark_path)
        elif refs.bias_path:
            bias = fits.getdata(refs.bias_path).astype(np.float32)
            data = data - bias
            header["CALBIAS"] = str(refs.bias_path)

        if refs.flat_path:
            flat = fits.getdata(refs.flat_path).astype(np.float32)
            flat_norm = flat / np.median(flat)
            flat_norm = np.where(flat_norm < 0.1, 1.0, flat_norm)  # avoid div-by-zero
            data = data / flat_norm
            header["CALFLAT"] = str(refs.flat_path)

        header["CALSTAT"] = "BDF" if refs.flat_path else ("BD" if refs.dark_path else "B")

        cal_path = Path(tempfile.mkdtemp()) / f"{frame.id}_cal.fits"
        fits.writeto(cal_path, data, header)
        frame.calibrated_path = cal_path

        log.debug("Calibrated frame %s → %s", frame.id, cal_path)
        return frame
