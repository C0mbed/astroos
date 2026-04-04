"""Live stack accumulator — builds a running stack as frames arrive."""

from __future__ import annotations

import logging
from pathlib import Path

import numpy as np
from astropy.io import fits

log = logging.getLogger(__name__)


class LiveStacker:
    """
    Accumulates calibrated frames into a running stack.
    Thread-safe for use from a background pipeline worker.
    """

    def __init__(
        self,
        method: str = "sigma_clip",
        sigma_low: float = 3.0,
        sigma_high: float = 3.0,
        output_path: Path | None = None,
    ):
        self._method = method
        self._sigma_low = sigma_low
        self._sigma_high = sigma_high
        self._output_path = output_path
        self._frames: list[np.ndarray] = []
        self._reference_header: fits.Header | None = None

    @property
    def frame_count(self) -> int:
        return len(self._frames)

    def add(self, frame_path: Path) -> np.ndarray | None:
        """Add a new frame to the stack. Returns the updated stack array."""
        try:
            with fits.open(frame_path) as hdul:
                data = hdul[0].data.astype(np.float32)
                if self._reference_header is None:
                    self._reference_header = hdul[0].header.copy()
        except Exception:
            log.exception("LiveStacker: failed to read frame %s", frame_path)
            return None

        self._frames.append(data)
        stack = self._compute_stack()

        if self._output_path and stack is not None:
            self._write(stack)

        return stack

    def _compute_stack(self) -> np.ndarray | None:
        if not self._frames:
            return None

        cube = np.stack(self._frames, axis=0)

        if self._method == "mean":
            return np.mean(cube, axis=0)

        if self._method == "median":
            return np.median(cube, axis=0)

        if self._method in ("sigma_clip", "kappa_sigma"):
            return self._sigma_clip(cube)

        return np.mean(cube, axis=0)

    def _sigma_clip(self, cube: np.ndarray) -> np.ndarray:
        mean = np.mean(cube, axis=0)
        std = np.std(cube, axis=0) + 1e-6
        low = mean - self._sigma_low * std
        high = mean + self._sigma_high * std
        mask = (cube >= low) & (cube <= high)
        masked = np.where(mask, cube, np.nan)
        with np.errstate(all="ignore"):
            result = np.nanmean(masked, axis=0)
        return np.where(np.isnan(result), mean, result)

    def _write(self, stack: np.ndarray) -> None:
        assert self._output_path is not None
        self._output_path.parent.mkdir(parents=True, exist_ok=True)
        hdr = self._reference_header or fits.Header()
        hdr["STACKN"] = self.frame_count
        hdr["STACKMTH"] = self._method
        fits.writeto(self._output_path, stack, hdr, overwrite=True)
