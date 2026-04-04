"""Stage 3 — Analyse: measure FWHM, eccentricity, SNR, star count."""

from __future__ import annotations

import logging
from pathlib import Path

import numpy as np
from astropy.io import fits

from ..frame import FrameContext, FrameMetrics
from ..runner import PipelineRejection

log = logging.getLogger(__name__)

# arcsec per pixel — should come from equipment profile; placeholder here
DEFAULT_PLATE_SCALE = 1.0  # arcsec/px


class AnalyseStage:
    def __init__(self, plate_scale_arcsec_per_px: float = DEFAULT_PLATE_SCALE):
        self._plate_scale = plate_scale_arcsec_per_px

    def process(self, frame: FrameContext) -> FrameContext:
        source_path = frame.calibrated_path or frame.raw_path
        try:
            data = fits.getdata(source_path).astype(np.float32)
        except Exception as exc:
            raise PipelineRejection("ANALYSIS_READ_ERROR") from exc

        try:
            import sep  # type: ignore
        except ImportError:
            log.warning("SEP not installed — skipping analysis")
            return frame

        # Background estimation and subtraction
        data_c = np.ascontiguousarray(data)
        bkg = sep.Background(data_c)
        data_sub = data_c - bkg

        # Source extraction
        objects = sep.extract(data_sub, thresh=3.0, err=bkg.globalrms)

        if len(objects) == 0:
            raise PipelineRejection("STAR_COUNT_TOO_LOW")

        # Compute FWHM for each star (Gaussian approximation: FWHM ≈ 2√(2ln2) × σ)
        fwhm_px = 2.355 * np.sqrt(0.5 * (objects["a"] ** 2 + objects["b"] ** 2))
        eccentricities = np.sqrt(1 - (objects["b"] / np.maximum(objects["a"], 1e-6)) ** 2)

        # Use brightest non-saturated stars (top 50 by flux)
        sort_idx = np.argsort(objects["flux"])[::-1][:50]
        fwhm_arcsec = float(np.median(fwhm_px[sort_idx])) * self._plate_scale
        eccentricity = float(np.median(eccentricities[sort_idx]))

        # Background gradient
        bkg_mesh = bkg.back()
        bg_min = float(bkg_mesh.min())
        bg_max = float(bkg_mesh.max())
        bg_gradient = bg_max / max(bg_min, 1.0)

        # Clipping fraction
        saturation = np.iinfo(np.uint16).max if data.max() < 70000 else 65535
        clipping_fraction = float(np.sum(data >= saturation * 0.98) / data.size)

        # SNR of the faintest star in the analysis set (25th percentile)
        snr_values = objects["flux"][sort_idx] / np.maximum(objects["fluxerr"][sort_idx], 1e-6)
        snr = float(np.percentile(snr_values, 25))

        frame.metrics = FrameMetrics(
            fwhm_arcsec=fwhm_arcsec,
            eccentricity=eccentricity,
            snr=snr,
            star_count=len(objects),
            background_adu=float(bkg.globalback),
            background_gradient=bg_gradient,
            clipping_fraction=clipping_fraction,
        )

        log.debug(
            "Frame %s: FWHM=%.2f\" ecc=%.2f SNR=%.1f stars=%d",
            frame.id, fwhm_arcsec, eccentricity, snr, len(objects),
        )
        return frame
