# Shader Field

2-D projected field effects. Each `.fs` file implements `spatialMain(out vec4 out_color, in vec2 frag_coord)`.

## Looks + third party

Looks are **original / license-clean** ports. Do **not** paste Shadertoy or other CC BY-NC-SA sources into this pack (incompatible with GPL-2.0-or-later). Using an **idea** is fine; if code is copied from a permissive licence, add `# Source: …` (see repo [CREDITS.md](../../CREDITS.md) and plugin [shader-conversion.md](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/blob/main/Documentation/shader-conversion.md)).

The plugin injects **`SpatialVfxLib410`** (`vfx*` helpers) ahead of every body. Prefer shared colour / rim helpers where they do not fight the look.

## Controls

**Globals** (Speed, Brightness, Size, Scale, Detail, Frequency, …) cover motion and occupancy.

**Per-look sliders** (header `param:` / `slider:` → `u_params[4..7]`) cover what globals cannot:

| Slider | Default | Role |
| --- | --- | --- |
| **Wash** | 60 | Soft fill (0 = **off** — crests only on black, 100 = full soft look) |
| **Density** | 50 | Feature spacing / count (0 = **off**, 100 = dense) |
| Look-specific (optional) | varies | Thickness, bands, rings, petals, cells, spread, … — drive the **named** spatial feature (not animation speed). **0 = off** |

Every look must **read and apply** these via `u_params[4..]`. Declaring a slider without using it is a bug.

Add another `param:` + `slider:` when a look has a distinct knob globals do not express.

## Atlas / UV

**Overlay atlas** is engine-fixed at **1024×576**. LED colours use GPU UV point-eval. Prefer `(frag_coord * 2.0 - u_resolution.xy) / u_resolution.y` or the classic `(uv - 0.5) * aspect * zoom` form already used by restored looks. Size (`u_params[0]`) scales feature density on that plane.

`_example_plasma.fs` is the header template for new looks.

Library target is **50** `.fs` looks (including the example). Newer gap-fillers: Soft Tunnel, Kaleido Fold, Lightning Veins, Caustic Pools, Hypno Disc, Particle Drift, Pulse Starburst, Marble Flow, Ribbon Curtain, Warp Portal, Zigzag Bands.
