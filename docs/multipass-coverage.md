# Multipass coverage (presets)

**Rule:** every tracked content file is listed. Folder-level `keep` is forbidden.
Status: `pending` | `hygiene` | `fixed` | `parked-claude` | `keep` (keep only after a real judgment pass).

## Parked for Claude reset (~2026-11-23)

Hybrid campaign 2026-10-27: current model does **CONTRIBUTING hygiene only**. Full every-file keep/kill (role, seams, once→black, sibling headers, sparse-LED polish, GLSL structure) waits for Claude.

**Resume order after reset:**
1. `action-editor/toolbox/volume` (every `.fx`/`.fs`)
2. `action-editor/toolbox/{basic,pixel,media}`
3. `effects/{volume,audio,media,patterns,shader-field}`
4. `triggers/effects/**` + `movie-sync/**`
5. `controllers/**` + docs/scripts/root

Skill: plugin `.cursor/skills/openrgb3d-multipass-review/SKILL.md`. Verify with commits, not row status alone.

After hygiene in this campaign: touched files → `hygiene`/`fixed`; untouched → stay `pending` (not `keep`).

**Hygiene scan (no edits):** `effects/patterns/**`, `controllers/**`, `template.controller.json`, most toolbox `.fx` without fancy dashes - still `pending` for Claude judgment (not `keep`).

## Per-file inventory
### action-editor

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `action-editor/README.md` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `action-editor/toolbox/basic/alternating.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/blink.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/candle.fx` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `action-editor/toolbox/basic/chase.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/colorwash.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/cycle.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/dissolve.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/fade.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/plane_sweep.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/plane_sweep.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/pulse.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/solid.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/solid_flash.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/solid_flash.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/spin.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/strobe.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/twinkle.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/wave.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/basic/wipe.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/media/media_gif.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/media/media_image.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/media/media_text.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/pixel/balls.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/pixel/bars.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/pixel/fire.fx` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `action-editor/toolbox/pixel/plasma.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/pixel/scanner.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/pixel/snow.fx` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `action-editor/toolbox/pixel/sparse_spark.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/pixel/sparse_spark.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/aurora_curtain.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `action-editor/toolbox/volume/burst.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/comet.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/confetti.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/dual_vortex.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/electric_storm.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `action-editor/toolbox/volume/field_aurora.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/field_burst.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/field_dual_vortex.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/field_electric.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/field_flash.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/field_portal.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/field_ripple.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/field_room_glow.fx` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `action-editor/toolbox/volume/field_strata.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/fill.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/helix.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/laser_sweep.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/meteor.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/neon_ribbon.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/noise3d.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/ocean_horizon.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/orbit.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/particle_embers.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/particle_fireflies.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/particle_meteors.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/particle_rain.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/portal_vortex.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `action-editor/toolbox/volume/radial_burst.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/ribbon_helix.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/ripple.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/ripple_ring.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/room_glow.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `action-editor/toolbox/volume/spark_burst.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/spherewipe.fx` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/strata_wash.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `action-editor/toolbox/volume/title_flash.fs` | pending | | Judgment → Claude ~2026-11-23 |

### controllers

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `controllers/cpu-coolers/amd/amd_wraith_prism.json` | pending | | Judgment → Claude ~2026-11-23 |
| `controllers/fans/corsair/corsair_sp120_rgb_pro.json` | pending | | Judgment → Claude ~2026-11-23 |
| `controllers/fans/corsair/icue-220t/corsair_icue_220t_sp120_front_bottom.json` | pending | | Judgment → Claude ~2026-11-23 |
| `controllers/fans/corsair/icue-220t/corsair_icue_220t_sp120_front_middle.json` | pending | | Judgment → Claude ~2026-11-23 |
| `controllers/fans/corsair/icue-220t/corsair_icue_220t_sp120_front_top.json` | pending | | Judgment → Claude ~2026-11-23 |
| `controllers/graphics-cards/intel/intel_arc_a770.json` | pending | | Judgment → Claude ~2026-11-23 |
| `controllers/keyboards/razer/razer_blackwidow_x_chroma.json` | pending | | Judgment → Claude ~2026-11-23 |
| `controllers/keyboards/razer/razer_tartarus_v2.json` | pending | | Judgment → Claude ~2026-11-23 |
| `controllers/mice/razer/razer_deathadder_elite.json` | pending | | Judgment → Claude ~2026-11-23 |
| `controllers/motherboards/msi/msi_mpg_x570_gaming_edge_wifi_onboard_led.json` | pending | | Judgment → Claude ~2026-11-23 |
| `controllers/speakers/logitech/g560/logitech_g560_lightsync_speaker_left.json` | pending | | Judgment → Claude ~2026-11-23 |
| `controllers/speakers/logitech/g560/logitech_g560_lightsync_speaker_right.json` | pending | | Judgment → Claude ~2026-11-23 |

### docs (other)

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `docs/device_notes/corsair_icue_220t_sp120.md` | pending | | Judgment → Claude ~2026-11-23 |
| `docs/PRESET_CATALOG.md` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `docs/PRESET_FORMAT.md` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |

### effects/audio

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `effects/audio/audio-level.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/audio/audio-paint-brush.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/audio/audio-pulse.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/audio/audio-strip-visualizer.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/audio/bass-punch.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/audio/note-sparkle.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/audio/README.md` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/audio/spectrum-bars.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |

### effects/media

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `effects/media/omni-shape-texture.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/media/README.md` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/media/texture-projection.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |

### effects/patterns

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `effects/patterns/blackhole.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/bounce.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/bpm.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/candle.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/chase.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/color_blend.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/color_wave.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/comet.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/confetti.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/cylon.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/distortion_waves.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/dot_bounce.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/fire_layered.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/fire_simple.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/frequency_saws.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/glitter_burst.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/gradient.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/halloween.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/heartbeat.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/hue_bounce.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/interference.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/juggle.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/larson.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/lissajous.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/matrix_rain.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/meteor.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/noise_octaves.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/ocean.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/plasma_sin.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/pulse.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/random_drift.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/README.md` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/ripple.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/running_light.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/saw.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/sin3.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/sine.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/smooth_noise.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/sparkle.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/sparkle_dark.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/spectrum_waves.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/square.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/steps.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/theater_chase.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/tri_fade.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/triangle.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/tricolor.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/twinkle_sparse.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/twinkle_up.kernel` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/patterns/wipe.kernel` | pending | | Judgment → Claude ~2026-11-23 |

### effects/shader-field

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `effects/shader-field/_example_plasma.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/shader-field/arc_static.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/atom_plasma.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/aurora_ridge.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/aurora_wash.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/shader-field/blau_waves.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/cell_bloom.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/corner_waves.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/dense_chroma.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/ember_field.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/energy_ring.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/shader-field/fluid_swirl.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/fog_drift.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/gyroid_mist.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/hex_drift.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/jewel_scatter.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/lobe_plasma.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/melt_petals.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/neon_space.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/neon_warp.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/noise_contour.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/oil_slick.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/oozy_flow.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/petal_spin.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/plasmic_ribbons.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/potential_rings.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/psyche_grid.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/rainbow_drip.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/README.md` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/shader-field/room_plasma.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/scan_sweep.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/shader-field/slow_waves.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/soft_aurora.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/soft_blobs.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/soft_ripples.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/spectrum_glow.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/star_sparkle.fs` | pending | | Judgment → Claude ~2026-11-23 |
| `effects/shader-field/voronoi_wash.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/vortex_swirl.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/shader-field/wave_mesh.fs` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |

### effects/volume

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `effects/volume/bouncing-ball.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/breathing-sphere.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/bubbles.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/color-wheel.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/depth-tone.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/dna-helix.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/grid-kit.fs` | fixed | 2026-10-28 | Moving Boxes 4→7; globals via motion_clock/freq_n; Thickness/Path/Size live |
| `effects/volume/harmonic-pulse.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/hex-lattice.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/noise_fire_column.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/particle-field.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/plasma.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/pulse-ring.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/README.md` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `effects/volume/rotating-cone.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/shell-pattern.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/spiral.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/starfield.fs` | fixed | 2026-10-28 | Full multipass: continuous-time fly-through (no progress wrap flash); fatter sparse-LED kernels; stratified star fill; clearer knobs/description |
| `effects/volume/surface-ambient.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/traveling-light.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |
| `effects/volume/wave.fs` | fixed | 2026-10-28 | Full multipass: continuous-time motion (no progress wrap flash); fatter sparse-LED kernels; clearer knobs/description |

### movie-sync

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `movie-sync/_example_sidecar.movie_sync.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/catalog.json` | pending | | Judgment → Claude ~2026-11-23 |
| `movie-sync/docs/movie-sync-authoring.md` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/docs/spatial-audit.md` | pending | | Judgment → Claude ~2026-11-23 |
| `movie-sync/effects/_template.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/effects/explosions/explosion.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/effects/explosions/roomscape_meteor_storm.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/effects/pulses/title_flash.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/effects/washes/ocean_horizon.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/effects/washes/room_wash.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/effects/washes/roomscape_aurora.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/effects/washes/roomscape_embers.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/effects/washes/roomscape_helix_rain.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/effects/washes/roomscape_portal.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/effects/wraps/laser_sweep.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/README.md` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/vlc/openrgb_movie_lights.lua` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/vlc/openrgb_movie_sync_author.lua` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/vlc/openrgb_movie_sync_intf.lua` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `movie-sync/vlc/README.md` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |

### scripts + root

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `.gitignore` | pending | | Judgment → Claude ~2026-11-23 |
| `CONTRIBUTING.md` | pending | | Judgment → Claude ~2026-11-23 |
| `README.md` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `scripts/generate_preset_catalog.py` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `scripts/smoke_presets.py` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `scripts/write_effect_pack_fx.py` | pending | | Judgment → Claude ~2026-11-23 |
| `template.controller.json` | pending | | Judgment → Claude ~2026-11-23 |

### triggers

| Path | Status | Date | Notes |
|------|--------|------|-------|
| `triggers/effects/_template.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `triggers/effects/chase_flash.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `triggers/effects/desk_ripple.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `triggers/effects/rainbow_wash.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `triggers/effects/roomscape/roomscape_aurora.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `triggers/effects/roomscape/roomscape_embers.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `triggers/effects/roomscape/roomscape_field_ripple.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `triggers/effects/roomscape/roomscape_glow_fireflies.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `triggers/effects/roomscape/roomscape_meteor_storm.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `triggers/effects/roomscape/roomscape_neon_ribbon.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `triggers/effects/roomscape/roomscape_portal.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `triggers/effects/roomscape/roomscape_spark_burst.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `triggers/effects/sequence_wipe.action.json` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |
| `triggers/README.md` | hygiene | 2026-10-27 | ASCII dash/quote/ellipsis hygiene (hybrid pass) |

