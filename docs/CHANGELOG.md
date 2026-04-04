# Changelog

All notable changes to Dark Sky are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).
Versioning follows [Semantic Versioning](https://semver.org/).

---

## [Unreleased]

*(Next: App Shell Layout, Status Bar, Device Dashboard)*

---

## [0.1.0] — 2026-04-04

### Added
- Brand System v1.0.0 (`/docs/BRAND_SYSTEM.md`) — Canvas, CombeCrew Design
- CSS token layer — primitives, semantic (dark + light), Night Mode (`src/styles/`)
- Typography system — Space Grotesk + JetBrains Mono, type scale utilities, `.font-telemetry` class
- Theme module (`src/theme.ts`) — Night Mode toggle, `N` keyboard shortcut, `☾` button hook, localStorage persistence
- Global CSS entry point with reset, Night Mode transition class, focus ring base, custom scrollbars
- Pipeline folder structure (`/pipeline/contracts/`, `/pipeline/reviews/`, `/pipeline/status/`)
- Architecture Decision Records 001–005 (`/docs/DECISIONS.md`)
