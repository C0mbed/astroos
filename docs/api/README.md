# API Reference

## Overview

AstroOS exposes a REST API and a WebSocket API for remote control and monitoring.

- **REST** — request/response for commands and queries
- **WebSocket** — real-time event stream (event bus bridge)

Base URL: `http://{host}:8080/api/v1`

## Authentication

API keys are configured in `config/api.yaml`. Pass the key as a header:

```
X-AstroOS-Key: your-api-key
```

## REST Endpoints

### Session

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/session` | Current session status |
| `POST` | `/session/start` | Start a session with a sequence |
| `POST` | `/session/abort` | Abort the current session |
| `POST` | `/session/pause` | Pause after current step |
| `POST` | `/session/resume` | Resume a paused session |

### Sequences

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/sequences` | List all sequences |
| `GET` | `/sequences/{id}` | Get sequence definition |
| `POST` | `/sequences` | Create a new sequence |
| `PUT` | `/sequences/{id}` | Update a sequence |
| `DELETE` | `/sequences/{id}` | Delete a sequence |

### Hardware

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/hardware` | List all configured devices |
| `GET` | `/hardware/{type}` | Get device state |
| `POST` | `/hardware/{type}/connect` | Connect device |
| `POST` | `/hardware/{type}/disconnect` | Disconnect device |
| `POST` | `/hardware/mount/slew` | Slew mount to coordinates |
| `POST` | `/hardware/mount/park` | Park mount |
| `POST` | `/hardware/camera/expose` | Take a single exposure |
| `POST` | `/hardware/focuser/move` | Move focuser |

### Pipeline

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/pipeline/frames` | Query archived frames |
| `GET` | `/pipeline/frames/{id}` | Get frame metadata |
| `GET` | `/pipeline/stack` | Get current live stack info |
| `POST` | `/calibration/create-master` | Create a master calibration frame |

### Scheduler

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/scheduler/targets` | List all targets |
| `POST` | `/scheduler/targets` | Add a target |
| `GET` | `/scheduler/plan` | Get tonight's plan |
| `POST` | `/scheduler/plan/generate` | Regenerate plan |

## WebSocket Events

Connect to `ws://{host}:8080/api/v1/events` to receive real-time events.

Events are JSON objects matching the [event bus schema](../architecture/event-bus.md).

### Subscribing

```json
{ "action": "subscribe", "types": ["pipeline.*", "sequence.*"] }
```

Send `{"action": "subscribe", "types": ["*"]}` to receive all events.

### Example Event

```json
{
  "type": "pipeline.frame.passed",
  "source": "pipeline",
  "timestamp": 1743724800.0,
  "session_id": "sess_abc123",
  "payload": {
    "frame_id": "frame_001",
    "fwhm": 2.31,
    "eccentricity": 0.12,
    "snr": 42.3,
    "archive_path": "archive/2026/04/03/M42/L/..."
  }
}
```
