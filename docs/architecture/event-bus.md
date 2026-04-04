# Event Bus Specification

## Purpose

The event bus is the nervous system of AstroOS. It decouples producers from consumers, enabling a modular architecture where components can be added, removed, or replaced without modifying anything else.

## Design

- **Synchronous by default within a process.** Events are dispatched synchronously to allow predictable ordering within a session.
- **Async bridge for cross-process / network consumers.** The API layer subscribes to the bus and forwards events over WebSocket.
- **No wildcards in subscriptions at the handler level.** Each handler declares the exact event types it handles.

## Event Schema

Every event is a typed dataclass:

```python
@dataclass
class Event:
    type: str          # e.g. "pipeline.frame.passed"
    source: str        # component that emitted it
    timestamp: float   # UTC unix timestamp
    payload: dict      # event-specific data
    session_id: str    # ties event to a session
```

## Event Type Registry

### hardware.*
| Event | Payload |
|-------|---------|
| `hardware.connected` | `{device_type, device_id, driver}` |
| `hardware.disconnected` | `{device_type, device_id, reason}` |
| `hardware.error` | `{device_type, device_id, error, recoverable}` |
| `hardware.state_changed` | `{device_type, device_id, old_state, new_state}` |

### sequence.*
| Event | Payload |
|-------|---------|
| `sequence.started` | `{sequence_id, target, plan}` |
| `sequence.step.started` | `{step_type, step_index, params}` |
| `sequence.step.completed` | `{step_type, step_index, duration_s}` |
| `sequence.completed` | `{sequence_id, frames_acquired, frames_passed}` |
| `sequence.aborted` | `{sequence_id, reason, last_step}` |

### pipeline.*
| Event | Payload |
|-------|---------|
| `pipeline.frame.ingested` | `{frame_id, path, exposure_s, filter}` |
| `pipeline.frame.passed` | `{frame_id, fwhm, eccentricity, snr, archive_path}` |
| `pipeline.frame.failed` | `{frame_id, reason, quarantine_path}` |
| `pipeline.calibration.applied` | `{frame_id, bias, dark, flat}` |

### scheduler.*
| Event | Payload |
|-------|---------|
| `scheduler.target.selected` | `{target_id, priority, altitude, moon_sep}` |
| `scheduler.session.planned` | `{date, targets, total_time_h}` |
| `scheduler.weather.hold` | `{reason, resume_at}` |

### storage.*
| Event | Payload |
|-------|---------|
| `storage.write.confirmed` | `{frame_id, path, checksum}` |
| `storage.integrity.failed` | `{path, expected_checksum, actual_checksum}` |
