# Brand System Amendment — v1.1.0

**Date:** 2026-04-09
**Author:** Canvas
**Reason:** WCAG AA compliance for caution-action button (Settings → Safety Contract v1.1.0 — SS-002)
**Previous version:** Brand System v1.0.0
**Status:** Approved by Matt 2026-04-09

---

## Change: New Semantic Color Token

### Problem

`color-status-warning` (#D97706 light) used as a button background with white (#FFFFFF) label text achieves a contrast ratio of approximately 3.0:1 — below the WCAG AA minimum of 4.5:1 for normal text.

This token is used as the background for the "I understand — Enable Overrides" confirmation button in Settings → Safety v1.1.0. It is also the candidate token for any future caution-level destructive action buttons across the app.

### Solution

Add a dedicated accessible variant token for use specifically as a **button background** where white or near-white text will be placed on top.

---

## Token Addition

### Primitive Color (new)

| Token             | Value     | Notes                                                                        |
|-------------------|-----------|------------------------------------------------------------------------------|
| `color-amber-700` | `#B45309` | Darker amber — derived from Tailwind amber-700. Added to primitive palette.  |

### Semantic Color (new)

| Token                              | Light Mode | Dark Mode | Contrast (white text)          | WCAG      | Usage                                                  |
|------------------------------------|------------|-----------|--------------------------------|-----------|--------------------------------------------------------|
| `color-status-warning-accessible`  | `#B45309`  | `#F59E0B` | Light: 4.7:1 ✅ / Dark: 3.0:1 ⚠ | AA (light) | Button backgrounds requiring white/light text on amber |

**Dark mode note:** In dark mode, `color-status-warning` (#F59E0B) on a dark label (#0A0A0A) achieves approximately 10.5:1 — AAA. The existing dark-mode warning token is already accessible. `color-status-warning-accessible` in dark mode maps to the same value as `color-status-warning` (#F59E0B) because the dark mode problem doesn't exist.

---

## Existing Token — Unchanged

| Token                  | Light Mode | Dark Mode | Usage                                                                                                                    |
|------------------------|------------|-----------|--------------------------------------------------------------------------------------------------------------------------|
| `color-status-warning` | `#D97706`  | `#F59E0B` | Warning indicators, borders, badges, icons, text-on-light backgrounds — **not** for button backgrounds with white text  |

---

## Usage Rule (added to Brand System)

> `color-status-warning` is for **indicators** — borders, badge backgrounds at opacity, icon fills, text.
>
> `color-status-warning-accessible` is for **interactive surfaces** — button backgrounds where white or near-white label text sits directly on top.
>
> Never use `color-status-warning` as a filled button background with a light label. Always use `color-status-warning-accessible` in that context.

---

## Affected Components

| Component                                               | Version | Change                                                          |
|---------------------------------------------------------|---------|-----------------------------------------------------------------|
| Settings → Safety — Acknowledgment modal confirm button | v1.1.0  | Button bg: `color-status-warning-accessible` / Label: `#FFFFFF` |
| Any future caution-action button                        | —       | Use `color-status-warning-accessible` as the standard token     |

---

## Contrast Ratios (full audit)

| Foreground | Background (Light)                            | Ratio | WCAG   |
|------------|-----------------------------------------------|-------|--------|
| `#FFFFFF`  | `color-status-warning` (#D97706)              | 3.0:1 | Fail ❌ |
| `#FFFFFF`  | `color-status-warning-accessible` (#B45309)   | 4.7:1 | AA ✅   |
| `#0A0A0A`  | `color-status-warning` (#D97706)              | 7.0:1 | AAA ✅  |

| Foreground | Background (Dark)                             | Ratio  | WCAG  |
|------------|-----------------------------------------------|--------|-------|
| `#0A0A0A`  | `color-status-warning-accessible` (#F59E0B)   | 10.5:1 | AAA ✅ |

---

## Forge Implementation Note

Add to the existing design token file:

```
// Primitive
color-amber-700: #B45309

// Semantic
color-status-warning-accessible:
  light: #B45309   (color-amber-700)
  dark:  #F59E0B   (color-amber-500 — same as color-status-warning dark)
```

Night Mode overlay: both tokens must be verified against the red overlay stack. Flag at Stage 4 visual QA.
