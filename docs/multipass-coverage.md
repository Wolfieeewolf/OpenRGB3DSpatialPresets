# Multipass coverage (presets)

**Rule:** anything not yet multipassed is **stale**. Status: `pending` | `pass` | `keep` | `fixed` | `parked`.

## Toolbox — volume

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `volume/confetti.fx` | fixed | 2026-10-27 | Dead `min_intensity` → density |
| `volume/helix.fx` | fixed | 2026-10-27 | Y-axis Gram-Schmidt frame (was flat) |
| `volume/ocean_horizon.fx` | fixed | 2026-10-27 | Dead `pulse` → horizon_band |
| `volume/` remaining `.fx`/`.fs` | keep | 2026-10-27 | Full folder multipass |

## Toolbox — basic / pixel / media

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `basic/alternating.fx` | fixed | 2026-10-27 | Removed dead `direction` knob |
| `basic/strobe.fx` | fixed | 2026-10-27 | `paint_stop(0)` — hard flash role |
| `basic/` other | keep | 2026-10-27 | |
| `pixel/` | keep | 2026-10-27 | |
| `media/` | keep | 2026-10-27 | |

## Packs

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `triggers/effects/roomscape/` | keep | 2026-10-27 | All 8 packs |
| `movie-sync/.../roomscape_aurora` | fixed | 2026-10-27 | speed → match trigger twin |
| `movie-sync/.../roomscape_portal` | fixed | 2026-10-27 | portal speed → match twin |
| `movie-sync/effects/**` other | keep | 2026-10-27 | |

## Other shipped content

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `effects/audio/**` | fixed | 2026-10-27 | 6 keep · 1 fixed: "Phrase" → "Phase" typo in audio-paint-brush.fs |
| `effects/media/**` | fixed | 2026-10-27 | 1 keep · 1 fixed: mojibake em-dash in omni-shape-texture.fs comment |
| `effects/patterns/**` | fixed | 2026-10-27 | 45 keep · 1 fixed: `step` var shadowing GLSL built-in in steps.kernel |
| `effects/shader-field/**` | keep | 2026-10-27 | 41 files clean; stub/full header split documented in README |
| `effects/volume/**` | fixed | 2026-10-27 | 19 keep · 2 fixed: mojibake em-dashes in shell-pattern.fs (×2) and surface-ambient.fs (×9) |
| `controllers/**` | pending | | |
| VLC / movie-sync helpers | pending | | |
