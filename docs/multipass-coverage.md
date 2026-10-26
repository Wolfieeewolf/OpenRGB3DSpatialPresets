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
| `effects/**` | pending | | FolderVolume / library looks |
| `controllers/**` | pending | | |
| VLC / movie-sync helpers | pending | | |
