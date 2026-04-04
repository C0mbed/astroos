# Hardware Support

## Device Categories

| Category | Doc |
|----------|-----|
| Mounts | [mount.md](mount.md) |
| Cameras | [camera.md](camera.md) |
| Focusers | [focuser.md](focuser.md) |
| Filter Wheels | [filter-wheel.md](filter-wheel.md) |
| Rotators | [rotator.md](rotator.md) |
| Domes & Roll-Off Roofs | [dome.md](dome.md) |
| Weather Stations | [weather.md](weather.md) |
| Guiders | [guider.md](guider.md) |
| Flat Panels | [flat-panel.md](flat-panel.md) |

## Driver Model

See [driver-model.md](driver-model.md) for the plugin-based hardware abstraction.

## Protocol Bridges

AstroOS communicates with hardware through two primary protocol layers:

### INDI (Linux / macOS)
INDI (Instrument-Neutral Distributed Interface) is the standard open-source protocol for astronomy hardware on Linux. AstroOS ships with an INDI client that discovers and connects to any INDI server.

### ASCOM (Windows)
ASCOM is the Windows standard. AstroOS supports ASCOM via the ASCOM Remote REST server, making ASCOM devices accessible cross-platform.

### Direct Drivers
For hardware not covered by INDI or ASCOM, AstroOS supports direct serial/USB drivers as plugins. These implement the same abstract device interface.
