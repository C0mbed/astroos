# Roadmap

## Milestone 0 — Foundation
*Target: initial working scaffold*

- [ ] Project structure and build system
- [ ] Core event bus implementation
- [ ] Plugin registry and loader
- [ ] Configuration system (YAML, env override)
- [ ] Logging framework
- [ ] Basic CLI entrypoint

## Milestone 1 — Hardware Layer
*First hardware connection*

- [ ] Abstract device interfaces (Mount, Camera, Focuser, FilterWheel)
- [ ] INDI client driver bridge
- [ ] ASCOM Remote driver bridge
- [ ] ZWO ASI direct driver
- [ ] Equipment profile config
- [ ] Device connection manager with reconnect logic

## Milestone 2 — Acquisition Core
*First unattended sequence*

- [ ] Sequence YAML parser and validator
- [ ] Sequence executor (step runner)
- [ ] Expose step (camera + filter wheel)
- [ ] Slew step (mount)
- [ ] Autofocus routine (V-curve)
- [ ] Meridian flip detection and handling
- [ ] Basic guiding integration (PHD2 via socket)
- [ ] Dithering

## Milestone 3 — Pipeline
*First processed frame in archive*

- [ ] FITS ingest and header validation
- [ ] Master calibration frame creator
- [ ] Calibration applicator (bias/dark/flat)
- [ ] Star detection (SEP)
- [ ] FWHM / eccentricity measurement
- [ ] Quality gate (configurable thresholds)
- [ ] FITS archive writer with checksum
- [ ] Metadata database (SQLite)
- [ ] Quarantine handling

## Milestone 4 — Scheduler & Safety
*First multi-target, multi-night session*

- [ ] Constraint-based target scheduler
- [ ] Ephemeris integration (altitude, moon, twilight)
- [ ] Weather station plugin (Boltwood, MQTT)
- [ ] Safety system (multi-level abort)
- [ ] Dome / roll-off roof control
- [ ] Session plan output

## Milestone 5 — API & UI
*Remote control and monitoring*

- [ ] REST API (FastAPI)
- [ ] WebSocket event bridge
- [ ] Web control panel (SvelteKit)
- [ ] Live stack preview
- [ ] Session dashboard (frame count, FWHM graph, stack preview)
- [ ] Notification system (Pushover, email)

## Milestone 6 — Polish & Robustness
*Production-ready for observatory use*

- [ ] Plate solving integration (ASTAP)
- [ ] WCS header annotation for all archived frames
- [ ] Autofocus: hyperbolic fit, filter offsets, temperature compensation
- [ ] Live stacking with alignment and sigma-clipping
- [ ] Adaptive quality gate
- [ ] Archive integrity scanner (scheduled)
- [ ] Full test suite (unit + integration + hardware-in-the-loop)
- [ ] Installation and deployment scripts

## Backlog

- Sky quality monitoring (SQM integration)
- All-sky camera cloud detection
- Meridian flip model calibration wizard
- Multi-camera support (simultaneous imaging)
- Cloud sync / backup integration
- Mobile companion app
- Pointing model (N-star alignment)
- Spectroscopy acquisition mode
