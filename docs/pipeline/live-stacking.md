# Live Stacking

## Overview

Live stacking integrates passing frames into a running stack during the session, providing a real-time preview of the accumulating image.

## Stack Types

| Mode | Description | Use Case |
|------|-------------|---------|
| `mean` | Simple mean of aligned frames | High SNR objects, fast |
| `median` | Median per pixel | Satellite trail rejection |
| `sigma_clip` | Iterative sigma-clipping | Best rejection, slower |
| `kappa_sigma` | Single-pass sigma clip | Compromise speed/quality |

## Frame Alignment

Before stacking, each new frame is aligned to the reference frame (the first accepted frame of the session):

1. Detect stars in new frame
2. Match star pattern to reference using triangle matching
3. Compute affine transform (translation + rotation, no scale change assumed)
4. Apply transform with sub-pixel interpolation (Lanczos-3)

Alignment failures are logged but do not fail the frame — the frame is archived normally, just not added to the live stack.

## Output

The live stack is updated after each new frame is added and written to a well-known path:

```
/tmp/astroos/live_stack_{session_id}_{filter}.fits
```

The UI polls (or subscribes via WebSocket) for stack updates and renders a stretched preview.

## Configuration

```yaml
live_stacking:
  enabled: true
  method: sigma_clip
  sigma_low: 3.0
  sigma_high: 3.0
  alignment: true
  alignment_min_stars: 8
  output_dir: /tmp/astroos
  stretch:
    method: asinh
    black_point: 0.001
    white_point: 0.999
```
