# Pipeline Status
*Last updated: 2026-04-04 by Forge*

## Framework Decision
**WinUI 3 / Windows App SDK 1.6.x — LOCKED**
Windows 11 minimum (10.0.22000.0). x64 only. Self-contained deployment.

## Foundation Complete

| Item | Status | Notes |
|---|---|---|
| Brand System v1.0.0 | ✅ Committed | docs/BRAND_SYSTEM.md |
| CSS token layer | ✅ Committed | docs/design/tokens/ |
| Theme module reference | ✅ Committed | docs/design/theme/theme.ts |
| C# solution scaffold | ✅ Committed | AstroOS.sln, 8 projects |
| XAML token stub | ✅ Committed | src/AstroOS.UI/Styles/Tokens.xaml |
| ADR-001 to ADR-006 | ✅ Committed | docs/DECISIONS.md |

## Active Pipeline

| Component | Stage | Status | Blocking |
|---|---|---|---|
| Status Bar | 3 — Build | 🔴 Blocked | XAML token layer not yet translated |
| Device Dashboard | 3 — Build | 🔴 Blocked | XAML token layer not yet translated |
| App Shell | 2 — Contract | ⏳ Awaiting Canvas contract | — |
| Safety Panel | 2 — Contract | ⏳ Awaiting Canvas contract | — |
| Goodnight Moon overlay | 3 — Ready to build | ✅ Contract approved | Waiting on App Shell mount point |

## Next Actions

| # | Action | Owner |
|---|---|---|
| 1 | Translate CSS token layer → XAML ResourceDictionary | Forge |
| 2 | Deliver App Shell component contract | Canvas |
| 3 | Deliver Safety Panel component contract | Canvas |
| 4 | Build Status Bar to contract | Forge (after token layer) |
| 5 | Build Device Dashboard to contract | Forge (after token layer) |

## Open Questions

| # | Question | Owner | Status |
|---|---|---|---|
| 1 | PC shutdown after Goodnight sequence — app stays running or optional OS shutdown? | Matt | ⏳ Pending |
