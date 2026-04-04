"""Stage 5 — Plate Solve: add WCS headers to calibrated frame."""

from __future__ import annotations

import logging
import subprocess
from pathlib import Path

from astropy.io import fits

from ..frame import FrameContext

log = logging.getLogger(__name__)


class PlateSolveStage:
    """
    Runs ASTAP plate solver on the calibrated frame and writes WCS headers.
    Solve failures are logged but do not reject the frame.
    """

    def __init__(
        self,
        executable: str = "astap",
        index_path: str | None = None,
        search_radius_deg: float = 30.0,
        exposure_s: float = 5.0,
    ):
        self._executable = executable
        self._index_path = index_path
        self._search_radius = search_radius_deg

    def process(self, frame: FrameContext) -> FrameContext:
        source = frame.calibrated_path or frame.raw_path
        wcs_path = source.with_suffix(".wcs")

        cmd = [
            self._executable,
            "-f", str(source),
            "-r", str(self._search_radius),
            "-o", str(source.with_suffix("")),
        ]
        if self._index_path:
            cmd += ["-d", self._index_path]

        try:
            result = subprocess.run(
                cmd, capture_output=True, text=True, timeout=120
            )
            if result.returncode != 0:
                log.warning("Plate solve failed for frame %s: %s", frame.id, result.stderr)
                return frame
        except subprocess.TimeoutExpired:
            log.warning("Plate solve timed out for frame %s", frame.id)
            return frame
        except FileNotFoundError:
            log.warning("ASTAP executable not found: %s", self._executable)
            return frame

        if not wcs_path.exists():
            log.warning("ASTAP did not produce WCS file for frame %s", frame.id)
            return frame

        with fits.open(source, mode="update") as hdul:
            wcs_header = fits.getheader(wcs_path)
            for key in ["WCSAXES", "CTYPE1", "CTYPE2", "CRVAL1", "CRVAL2",
                        "CRPIX1", "CRPIX2", "CD1_1", "CD1_2", "CD2_1", "CD2_2"]:
                if key in wcs_header:
                    hdul[0].header[key] = wcs_header[key]
            hdul[0].header["PLTSOLVD"] = True
            hdul[0].header["PLTSOLV"] = "ASTAP"
            hdul.flush()

        frame.wcs_solved = True
        if "CRVAL1" in fits.getheader(source):
            frame.ra_center_deg = fits.getheader(source).get("CRVAL1")
            frame.dec_center_deg = fits.getheader(source).get("CRVAL2")

        log.info(
            "Plate solved frame %s: RA=%.4f Dec=%.4f",
            frame.id, frame.ra_center_deg or 0, frame.dec_center_deg or 0,
        )
        return frame
