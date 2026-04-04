# Goodnight Moon — ASCII Art Spec
**Component:** Goodnight Sequence Overlay — Session Complete State  
**Version:** 1.0.0  
**Date:** 2026-04-04  

---

## Concept

When the Goodnight sequence completes successfully, the overlay transitions to a "Session Complete" screen. The centrepiece is an ASCII art illustration — a woman asleep in bed, the moon visible through the window above — a playful nod to *Goodnight Moon* by Margaret Wise Brown. It is whimsical, warm, and completely unexpected in a precision observatory tool. That is the point.

This is Dark Sky's personality in a single screen. It earns trust all night through precision. Then at the end it winks.

---

## The Art

```
                          *    .  *       .        *
              .    *           .       *      .
        *          .    *                  .       *    .
                                  *
                              .        .
                        *                    *

                              ( (
                           (       )
                         (    . .    )
                        (   .  O  .   )
                         (   . - .   )
                           (       )
                              ) )
                          ___________
                         /           \
                        /   * * * *   \
                       |    * * * *    |
                        \   * * * *   /
                         \___________/


            .   *      .        *          .       *      .

                    .       .          *        .
          *                                               *
              .          *        .          *       .


         ____________________________________________________
        |                                                    |
        |    .    *                              *    .      |
        |                                                    |
        |   _____              ,---.                         |
        |  |     |   zZz      /     \     *          .      |
        |  | [=] |           ( -   - )                      |
        |  |  _  |    .       \  u  /           *           |
        |  |_____|             \___/       .                 |
        |     |         ___   /     \                        |
        |     |________|   | |  ___  |    .                  |
        |                  |_||_| |_||                       |
        |                   |  | | |  |          *     .     |
        |   ________________|__|_|_|__|___________           |
        |  /                                       \         |
        | /      Good night, Dark Sky.              \        |
        |/                                           \       |
        |   247 frames captured  ·  Session complete  |      |
        |  ___________________________________________|      |
        |___________________________________________________ |
```

---

## Rendering Spec

**Font:** JetBrains Mono, 11px, `line-height: 1.3`  
**Colour:** `color-text-muted` base — faint, not bright. The art is atmospheric, not loud.  
**Stars** (the `*` and `.` characters scattered above): slightly brighter — `color-text-secondary`  
**Moon** (the circle shape): `color-status-degraded` (warm amber) — the moon glows  
**"zZz"**: `color-text-muted` with a gentle opacity pulse (1→0.4→1, 3000ms ease-in-out) — she is asleep  
**"Good night, Dark Sky." text inside the art:** `color-text-primary`, slightly larger — 13px  
**Frame count line:** replaced dynamically with actual session stats: `[N] frames captured · [H]h [M]m session`  
**Reduce Motion:** "zZz" pulse static at 50% opacity. No other animations in the art.

**Night Mode:** The entire overlay is already in the night palette. The moon shifts to `#CC6600` (the night-mode nominal amber). Stars remain faint warm-red. The art reads beautifully in red — it was designed for it.

**Alignment:** Centred horizontally and vertically in the overlay. On small windows, scales down by reducing font to 10px before any truncation.

**Dynamic substitution:** The bottom two lines inside the bed frame are replaced with live session data:
- Line 1: "Good night, [target name]." — if a target was active, use its name. "Good night, Dark Sky." if no target.
- Line 2: "[N] frames captured  ·  [duration] session" — actual stats from completed sequence

---

## Animation — Entrance

On transition from sequence-running state to session-complete state:

1. Step list fades out: `duration-normal`, `ease-exit`
2. ASCII art fades in from bottom: `translateY(20px → 0)` + opacity 0→1, `duration-slow`, `ease-enter`, 200ms delay
3. Stars twinkle once on entrance: opacity 0.3→1→0.6, staggered across the star characters, `duration-deliberate` (600ms) — then settle at their final opacity
4. "zZz" begins its sleep pulse loop

Reduce Motion: all transitions instant, no star twinkle, no zZz pulse.

---

## File Location

The ASCII art string is stored in `/src/ui/assets/goodnight-moon.txt` — loaded at runtime, not hardcoded in the component. This allows it to be updated without a code change, and allows for future seasonal or special-event variants (solstice edition, eclipse night, etc.).

