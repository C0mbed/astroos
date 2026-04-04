"""Pipeline runner — orchestrates stage execution for each frame."""

from __future__ import annotations

import logging
from dataclasses import dataclass, field
from typing import Protocol

from .frame import FrameContext, FrameState

log = logging.getLogger(__name__)


class PipelineRejection(Exception):
    """Raised by a stage to reject a frame with a reason code."""
    def __init__(self, reason: str):
        self.reason = reason
        super().__init__(reason)


class PipelineStage(Protocol):
    """Protocol that all pipeline stages must implement."""
    def process(self, frame: FrameContext) -> FrameContext: ...


@dataclass
class PipelineConfig:
    calibration_enabled: bool = True
    plate_solve_enabled: bool = False
    live_stack_enabled: bool = True
    quality_gate: dict = field(default_factory=dict)
    archive_root: str = "archive"
    quarantine_root: str = "quarantine"


class PipelineRunner:
    """
    Processes a single frame through all configured stages.

    Instantiate once per session. Call process() for each captured frame.
    """

    def __init__(self, config: PipelineConfig, event_bus=None):
        self._config = config
        self._bus = event_bus
        self._stages = self._build_stages()

    def _build_stages(self) -> list[PipelineStage]:
        from .stages.ingest import IngestStage
        from .stages.calibrate import CalibrateStage
        from .stages.analyse import AnalyseStage
        from .stages.gate import QualityGateStage
        from .stages.archive import ArchiveStage

        stages: list[PipelineStage] = [
            IngestStage(),
            CalibrateStage(enabled=self._config.calibration_enabled),
            AnalyseStage(),
            QualityGateStage(thresholds=self._config.quality_gate),
        ]

        if self._config.plate_solve_enabled:
            from .stages.solve import PlateSolveStage
            stages.append(PlateSolveStage())

        stages.append(
            ArchiveStage(
                archive_root=self._config.archive_root,
                quarantine_root=self._config.quarantine_root,
            )
        )

        return stages

    def process(self, frame: FrameContext) -> FrameContext:
        """Run frame through all stages. Returns frame with final state set."""
        log.info("Pipeline: processing frame %s (%s)", frame.id, frame.raw_path)

        for stage in self._stages:
            if frame.state == FrameState.FAILED:
                break
            try:
                frame = stage.process(frame)
            except PipelineRejection as exc:
                frame.reject(exc.reason)
                log.info(
                    "Pipeline: frame %s rejected at %s — %s",
                    frame.id, stage.__class__.__name__, exc.reason,
                )
            except Exception:
                frame.reject("STAGE_ERROR")
                log.exception(
                    "Pipeline: unexpected error in stage %s for frame %s",
                    stage.__class__.__name__, frame.id,
                )

        self._emit(frame)
        return frame

    def _emit(self, frame: FrameContext) -> None:
        if self._bus is None:
            return
        event_type = (
            "pipeline.frame.passed"
            if frame.state == FrameState.PASSED
            else "pipeline.frame.failed"
        )
        self._bus.emit(event_type, source="pipeline", payload=frame.to_dict())
