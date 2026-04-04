"""Stage 1 — Ingest: validate FITS header and populate frame context."""

from __future__ import annotations

import logging

from astropy.io import fits

from ..frame import FrameContext, FrameType
from ..runner import PipelineRejection

log = logging.getLogger(__name__)

REQUIRED_HEADERS = ["EXPTIME", "DATE-OBS", "INSTRUME"]


class IngestStage:
    def process(self, frame: FrameContext) -> FrameContext:
        try:
            header = fits.getheader(frame.raw_path)
        except Exception as exc:
            raise PipelineRejection("INVALID_FITS") from exc

        missing = [k for k in REQUIRED_HEADERS if k not in header]
        if missing:
            log.warning("Frame %s missing headers: %s", frame.id, missing)

        frame.exposure_s = header.get("EXPTIME")
        frame.date_obs = header.get("DATE-OBS")
        frame.camera_id = header.get("INSTRUME")
        frame.gain = header.get("GAIN")
        frame.filter = header.get("FILTER")
        frame.target = header.get("OBJECT")
        frame.temp_sensor_c = header.get("CCD-TEMP")

        bx = header.get("XBINNING", 1)
        by = header.get("YBINNING", 1)
        frame.binning = f"{bx}x{by}"

        frame_type_str = header.get("FRAME", "LIGHT").upper()
        try:
            frame.frame_type = FrameType(frame_type_str)
        except ValueError:
            frame.frame_type = FrameType.LIGHT

        log.debug(
            "Ingested frame %s: target=%s filter=%s exp=%.1fs",
            frame.id, frame.target, frame.filter, frame.exposure_s or 0,
        )
        return frame
