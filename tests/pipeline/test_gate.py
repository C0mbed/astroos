"""Tests for the quality gate stage."""

import pytest

from pipeline.frame import FrameContext, FrameMetrics, FrameState
from pipeline.runner import PipelineRejection
from pipeline.stages.gate import QualityGateStage
from pathlib import Path


def _frame_with_metrics(**metrics_kwargs) -> FrameContext:
    frame = FrameContext(raw_path=Path("/dev/null"))
    frame.metrics = FrameMetrics(**metrics_kwargs)
    return frame


def test_passes_good_frame():
    gate = QualityGateStage()
    frame = _frame_with_metrics(
        fwhm_arcsec=2.5,
        eccentricity=0.1,
        snr=80.0,
        star_count=50,
        background_gradient=1.05,
        clipping_fraction=0.0001,
    )
    result = gate.process(frame)
    assert result.state == FrameState.PASSED


def test_rejects_bad_fwhm():
    gate = QualityGateStage()
    frame = _frame_with_metrics(
        fwhm_arcsec=8.0,
        eccentricity=0.1,
        snr=80.0,
        star_count=50,
    )
    with pytest.raises(PipelineRejection) as exc:
        gate.process(frame)
    assert exc.value.reason == "FWHM_TOO_HIGH"


def test_rejects_high_eccentricity():
    gate = QualityGateStage()
    frame = _frame_with_metrics(
        fwhm_arcsec=2.5,
        eccentricity=0.9,
        snr=80.0,
        star_count=50,
    )
    with pytest.raises(PipelineRejection) as exc:
        gate.process(frame)
    assert exc.value.reason == "ECCENTRICITY_TOO_HIGH"


def test_rejects_low_snr():
    gate = QualityGateStage()
    frame = _frame_with_metrics(fwhm_arcsec=2.5, eccentricity=0.1, snr=5.0, star_count=50)
    with pytest.raises(PipelineRejection) as exc:
        gate.process(frame)
    assert exc.value.reason == "SNR_TOO_LOW"


def test_custom_threshold_override():
    gate = QualityGateStage(thresholds={"fwhm_max_arcsec": 10.0})
    frame = _frame_with_metrics(
        fwhm_arcsec=8.0,
        eccentricity=0.1,
        snr=80.0,
        star_count=50,
    )
    result = gate.process(frame)
    assert result.state == FrameState.PASSED


def test_skips_none_metrics():
    gate = QualityGateStage()
    frame = _frame_with_metrics(fwhm_arcsec=2.5)  # most metrics are None
    result = gate.process(frame)
    assert result.state == FrameState.PASSED
