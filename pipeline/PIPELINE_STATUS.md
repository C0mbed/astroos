# Pipeline Status — Dark Sky
**Last updated:** 2026-04-12
**Updated by:** Canvas (v2_1 session)

---

## Active Pipeline

| Component | Stage | Status | Gate | Blocking |
|-----------|-------|--------|------|----------|
| Status Bar v1.1.0 | Stage 4 → 5 | **Approved with conditions** — 2 items for Forge before Stage 5 | Canvas ✅ (conditional) | Forge: top separator + zone borders |
| Dark Sky Icon v1.0.0 | Stage 2 | **In progress** — master SVG produced this session | Canvas | Forge: technical review + ICO packaging |
| Device Dashboard v1.1.0 | Stage 3 | Awaiting build | Forge | — |
| Safety Panel v1.0.0 | Stage 3 | Awaiting build | Forge | — |
| Device Connect v1.0.0 Surface A | Stage 3 | Awaiting build | Forge | — |
| Device Connect v1.0.0 Surface B | Stage 3 | Awaiting build (unblocked) | Forge | — |
| Settings → Safety v1.1.0 | Stage 2 → 3 | Canvas ✅ Matt ✅ — awaiting Forge sign-off | Forge | — |
| Status Bar Amendment v1.2.0 | Sign-off | Canvas ✅ — awaiting Forge + Matt sign-off | Forge + Matt | Blocks Stage 5 |
| Device Setup v1.0.0 | Stage 2 → 3 | Canvas ✅ — awaiting Forge + Matt sign-off | Forge + Matt | — |

---

## Conditions on Status Bar Stage 4 → Stage 5 Transition

Before Crucible (Stage 5) begins:

1. **Forge:** Add `1px solid {color-border-subtle}` top border to Status Bar container
2. **Forge:** Implement Zone A right border: `1px solid {color-border-subtle}`
3. **Forge:** Implement Zone B right border: `1px solid {color-border-subtle}` (per Amendment §4.1)
4. **Forge:** Sign off on Amendment v1.2.0

When all four items are committed, ping Canvas for a 10-minute conditional check,
then Stage 5 is cleared.

---

## Sign-off Queue (Forge)

These contracts are ready and waiting for Forge review and sign-off:

| Contract | Canvas Status | Matt Status | Forge Status |
|----------|--------------|-------------|--------------|
| Status Bar Amendment v1.2.0 | ✅ Signed | ⬜ Pending | ⬜ Pending |
| Device Setup v1.0.0 | ✅ Signed | ⬜ Pending | ⬜ Pending |
| Settings → Safety v1.1.0 | ✅ Signed | ✅ Signed | ⬜ Pending |

All three are current and valid. No changes pending.

---

## Icon Pipeline Note

Dark Sky Icon v1.0.0 is at Stage 2. Canvas has produced:
- Master SVG (accretion disc, 1024×1024)
- Wordmark SVG (horizontal lockup)

Before Stage 3 (Forge implementation):
- Canvas to deliver: PNG exports at 256, 48, 32, 16px
- Canvas to deliver: ICO spec (16/32/48 combined)
- Forge to confirm: SVG renders correctly at all required sizes
- Forge to confirm: ICO build process for WinUI 3 packaging

---

*Pipeline Status — Dark Sky | Canvas*
