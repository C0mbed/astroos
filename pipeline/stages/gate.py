"""Stage 4 — Quality Gate: accept or reject frame based on measured metrics."""

from __future__ import annotations

import logging

from ..frame import FrameContext, FrameState
from ..runner import PipelineRejection

log = logging.getLogger(__name__)

DEFAULTS = {
    "fwhm_max_arcsec": 4.5,
    "eccentricity_max": 0.5,
    "snr_min": 20.0,
    "star_count_min": 10,
    "background_gradient_max": 1.3,
    "clipping_fraction_max": 0.01,
}


class QualityGateStage:
    def __init__(self, thresholds: dict | None = None):
        self._t = {**DEFAULTS, **(thresholds or {})}

    def process(self, frame: FrameContext) -> FrameContext:
        m = frame.metrics

        checks = [
            (m.fwhm_arcsec, "fwhm_max_arcsec", "FWHM_TOO_HIGH",
             lambda v, t: v > t),
            (m.eccentricity, "eccentricity_max", "ECCENTRICITY_TOO_HIGH",
             lambda v, t: v > t),
            (m.snr, "snr_min", "SNR_TOO_LOW",
             lambda v, t: v < t),
            (m.star_count, "star_count_min", "STAR_COUNT_TOO_LOW",
             lambda v, t: v < t),
            (m.background_gradient, "background_gradient_max", "BACKGROUND_GRADIENT",
             lambda v, t: v > t),
            (m.clipping_fraction, "clipping_fraction_max", "CLIPPING",
             lambda v, t: v > t),
        ]

        for value, threshold_key, reason, is_bad in checks:
            if value is None:
                continue
            if is_bad(value, self._t[threshold_key]):
                raise PipelineRejection(reason)

        frame.state = FrameState.PASSED
        log.debug("Frame %s passed quality gate", frame.id)
        return frame
