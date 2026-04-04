"""Stage 6 — Archive: write frame to FITS store with checksum."""

from __future__ import annotations

import hashlib
import logging
import shutil
from datetime import datetime, timezone
from pathlib import Path

from ..frame import FrameContext, FrameState

log = logging.getLogger(__name__)


def _sha256(path: Path) -> str:
    h = hashlib.sha256()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(65536), b""):
            h.update(chunk)
    return h.hexdigest()


def _archive_path(root: Path, frame: FrameContext, passed: bool) -> Path:
    date = frame.date_obs or datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")
    try:
        dt = datetime.fromisoformat(date.replace("Z", "+00:00"))
    except ValueError:
        dt = datetime.now(timezone.utc)

    year = dt.strftime("%Y")
    month = dt.strftime("%m")
    day = dt.strftime("%d")
    target = (frame.target or "unknown").replace(" ", "_")
    filter_ = frame.filter or "none"

    camera = (frame.camera_id or "cam").replace(" ", "_")
    exp = f"{frame.exposure_s:.0f}s" if frame.exposure_s else "Xs"
    ts = dt.strftime("%Y%m%dT%H%M%SZ")

    filename = f"{camera}_{target}_{filter_}_{exp}_{ts}.fits"

    if not passed:
        reason = frame.rejection_reason or "REJECTED"
        filename = filename.replace(".fits", f"_{reason}.fits")

    return root / year / month / day / target / filter_ / filename


class ArchiveStage:
    def __init__(self, archive_root: str = "archive", quarantine_root: str = "quarantine"):
        self._archive = Path(archive_root)
        self._quarantine = Path(quarantine_root)

    def process(self, frame: FrameContext) -> FrameContext:
        passed = frame.state == FrameState.PASSED
        root = self._archive if passed else self._quarantine
        dest = _archive_path(root, frame, passed)
        dest.parent.mkdir(parents=True, exist_ok=True)

        source = frame.calibrated_path or frame.raw_path
        shutil.copy2(source, dest)

        frame.checksum = _sha256(dest)

        if passed:
            frame.archive_path = dest
        else:
            frame.state = FrameState.QUARANTINED

        log.info(
            "Frame %s %s → %s [%s]",
            frame.id,
            "archived" if passed else "quarantined",
            dest,
            frame.checksum[:8],
        )
        return frame
