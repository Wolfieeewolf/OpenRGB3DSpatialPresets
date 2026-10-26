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
| `controllers/mice/razer/razer_deathadder_elite.json` | fixed | 2026-10-27 | `category` typo `mouses` → `mice` |
| `controllers/motherboards/msi/msi_mpg_x570_gaming_edge_wifi_onboard_led.json` | fixed | 2026-10-27 | `category` singular `motherboard` → `motherboards` |
| `controllers/` remaining (10 files) | keep | 2026-10-27 | JSON valid; required fields present; dimensions/arrays consistent; mapping bounds clean |
| `movie-sync/vlc/openrgb_movie_sync_intf.lua` | fixed | 2026-10-27 | Duplicate `.movie_sync.json / .movie_sync.json` in log message |
| `movie-sync/vlc/openrgb_movie_lights.lua` | fixed | 2026-10-27 | File-picker only filtered `.movie_cues.json`; now also accepts `.movie_sync.json`; descriptor updated; documented in vlc/README |
| `movie-sync/vlc/openrgb_movie_sync_author.lua` | keep | 2026-10-27 | Clean |
| `movie-sync/vlc/README.md` | fixed | 2026-10-27 | Missing `openrgb_movie_lights.lua` row in script/install tables; added with role + install path |
| `movie-sync/docs/spatial-audit.md` | fixed | 2026-10-27 | Broken link `movie-cue-authoring.md` → `movie-sync-authoring.md` |
| `movie-sync/docs/movie-sync-authoring.md` | keep | 2026-10-27 | Clean |
| `movie-sync/README.md` | keep | 2026-10-27 | Clean |
| `movie-sync/catalog.json` | keep | 2026-10-27 | All 10 effect IDs resolve to present action files |
| `movie-sync/_example_sidecar.movie_sync.json` | keep | 2026-10-27 | Valid; all effect_ids in catalog; format fields correct |
