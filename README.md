# AstroOS

**Observatory-first astrophotography acquisition platform.**

AstroOS is a modular, hardware-agnostic platform for automated astronomical imaging. It targets serious amateur and semi-professional observatories where reliability, repeatability, and unattended operation are non-negotiable.

---

## Core Pillars

| Pillar | Description |
|--------|-------------|
| **Acquisition** | Sequence-driven, multi-target imaging sessions with full hardware orchestration |
| **Pipeline** | Automated calibration, stacking, and quality-gating per light frame |
| **Hardware** | Unified driver model for mounts, cameras, focusers, filter wheels, rotators, domes |
| **Scheduling** | Constraint-based target scheduling with weather and sky-quality awareness |
| **Storage** | Structured FITS archive with metadata indexing and integrity verification |

---

## Repository Layout

```
astroos/
├── docs/               # Wiki — architecture, features, hardware, API
├── pipeline/           # Imaging pipeline stages (capture → calibration → stack → output)
├── src/                # Platform source code
│   ├── core/           # Event bus, config, logging, plugin registry
│   ├── hardware/       # Device driver abstractions (INDI/ASCOM bridges)
│   ├── sequencer/      # Session planner and sequence executor
│   ├── scheduler/      # Multi-night constraint-based target scheduler
│   ├── storage/        # FITS archive, metadata DB, integrity checks
│   ├── api/            # REST + WebSocket API layer
│   └── ui/             # Web-based observatory control panel
├── tests/              # Unit, integration, and hardware-in-the-loop tests
├── config/             # Default and example configuration files
└── scripts/            # Setup, deployment, and maintenance scripts
```

## Quick Links

- [Architecture Overview](docs/architecture/overview.md)
- [Feature Spec](docs/features/README.md)
- [Pipeline Design](docs/pipeline/README.md)
- [Hardware Support](docs/hardware/README.md)
- [API Reference](docs/api/README.md)
- [Roadmap](docs/roadmap.md)
