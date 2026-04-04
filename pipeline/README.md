# Pipeline

Source code for the AstroOS imaging pipeline. Each stage is a standalone processor that receives a frame context and returns it (possibly modified) or raises a rejection.

## Structure

```
pipeline/
├── __init__.py
├── runner.py          # Orchestrates stage execution for each frame
├── frame.py           # Frame context dataclass (state, paths, metrics)
├── stages/
│   ├── __init__.py
│   ├── ingest.py      # Validate FITS header, populate frame context
│   ├── calibrate.py   # Apply bias/dark/flat calibration
│   ├── analyse.py     # Measure FWHM, eccentricity, SNR, star count
│   ├── gate.py        # Quality gate (accept/reject)
│   ├── solve.py       # Plate solve and write WCS headers (optional)
│   └── archive.py     # Write to FITS store, record metadata
├── calibration/
│   ├── __init__.py
│   ├── library.py     # Master calibration frame selector
│   ├── builder.py     # Master frame creator (stack raws → master)
│   └── matcher.py     # Match light frame to best available master
├── stack/
│   ├── __init__.py
│   ├── aligner.py     # Star-pattern frame alignment
│   └── stacker.py     # Live stack accumulator (mean/median/sigma-clip)
└── storage/
    ├── __init__.py
    ├── archive.py     # FITS file writer, directory management
    ├── database.py    # SQLite metadata DB interface
    └── integrity.py   # Checksum computation and verification
```

## Running the Pipeline

The pipeline runner is instantiated per session and processes frames as they arrive:

```python
from pipeline.runner import PipelineRunner
from pipeline.frame import FrameContext

runner = PipelineRunner(config=session_config)

# Called by the sequencer after each exposure
frame = FrameContext(raw_path="/tmp/capture/frame_001.fits")
result = runner.process(frame)

if result.state == FrameState.PASSED:
    print(f"Frame archived: {result.archive_path}")
else:
    print(f"Frame rejected: {result.rejection_reason}")
```

## Adding a Pipeline Stage

1. Create a new file in `stages/`
2. Implement the `PipelineStage` protocol:

```python
class MyStage:
    def process(self, frame: FrameContext) -> FrameContext:
        # Modify frame context
        # Raise PipelineRejection(reason) to reject the frame
        return frame
```

3. Register it in `runner.py`'s stage list.
