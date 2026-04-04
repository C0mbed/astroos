# Architecture Overview

## Design Philosophy

AstroOS is built around three constraints that observatories impose:

1. **Unattended operation** — sessions run without a human present. Every failure mode must be handled or safely aborted.
2. **Hardware heterogeneity** — observatories mix vendors, generations, and protocols. The platform must not couple business logic to any specific device.
3. **Data integrity** — a corrupted or mis-calibrated frame is worse than no frame. The pipeline must be conservative.

---

## Component Model

```
┌─────────────────────────────────────────────────────────────┐
│                        UI / API                             │
│          (Web Control Panel + REST/WebSocket API)           │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                     Sequencer                               │
│   (Session Executor → Target Queue → Sequence Runner)       │
└────┬─────────────────┬──────────────────┬───────────────────┘
     │                 │                  │
┌────▼────┐    ┌───────▼──────┐   ┌──────▼──────┐
│Hardware │    │   Pipeline   │   │  Scheduler  │
│ Layer   │    │  (per-frame) │   │ (multi-night│
│         │    │              │   │  planning)  │
└────┬────┘    └───────┬──────┘   └──────┬──────┘
     │                 │                  │
┌────▼─────────────────▼──────────────────▼──────┐
│                    Core                         │
│      (Event Bus · Config · Plugin Registry)     │
└────────────────────┬────────────────────────────┘
                     │
              ┌──────▼──────┐
              │   Storage   │
              │ (FITS Archive│
              │  + Metadata)│
              └─────────────┘
```

---

## Data Flow: Single Imaging Frame

```
Mount tracks target
    → Camera exposes
        → Frame lands in capture buffer
            → Pipeline: validate header
                → Subtract master bias/dark
                    → Divide master flat
                        → Plate-solve (optional, per sequence)
                            → Quality gate (FWHM, eccentricity, SNR)
                                → Pass: archive to FITS store
                                → Fail: quarantine + log reason
```

---

## Event Bus

All inter-component communication flows through a central event bus (pub/sub). No component holds a direct reference to another. This makes it possible to:

- Replace any component without touching others
- Record and replay sessions for debugging
- Run components in separate processes or on separate machines

Key event namespaces:
- `hardware.*` — device state changes, connection events
- `sequence.*` — sequence start/step/complete/abort
- `pipeline.*` — frame ingested, passed, failed, archived
- `scheduler.*` — target selected, session planned, weather hold
- `storage.*` — write confirmed, integrity check result

---

## Plugin Registry

Hardware drivers, pipeline processors, and scheduler constraints are all loaded as plugins. The registry discovers plugins at startup via entry points (or a config-declared plugin list). This means adding a new camera driver or a new pipeline filter never requires modifying platform code.

---

## Related Docs

- [Hardware Driver Model](../hardware/driver-model.md)
- [Pipeline Design](../pipeline/README.md)
- [Sequencer](../features/sequencer.md)
- [Event Bus Spec](event-bus.md)
