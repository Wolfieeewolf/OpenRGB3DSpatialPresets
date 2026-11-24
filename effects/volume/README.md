# Volume

Soft 3-D field effects. Each `.fs` file implements `volumeMain(out vec4 out_color, in vec3 p01)` where `p01` is the LED position normalized to `[0,1]³` within the occupancy atlas.

## Atlas / VFX

- Prefer `resolution: 40`–`48` (engine max **64**; default assist is 32). Denser atlas = cleaner shells on the room grid overlay without changing LED point-eval.
- The host injects **`SpatialVfxLib410`** (`vfx*`). Prefer `vfxCoreRim` / `vfxShell` / `vfxSoftBand` / `vfxFbm3` / `vfxHash*` for soft edges and noise instead of one-off helpers.
- Keep look math; do not paste Shadertoy / CC BY-NC-SA sources (see repo CREDITS and plugin shader-conversion.md). Ideas-only `# Source:` lines are fine.

## Output

- `out_color.r` = intensity (brightness gate)
- `out_color.g` = gradient / palette driver (meaning varies by `finish:`)
- `out_color.b` = gloss / highlight driver for **Style: Rich** (Flat ignores it and stays colour × intensity)
- `finish:` selects the host coloring pipeline (`hex`, `atlas`, `hsv`, `depth`, `surface`, `spiral`, `rgb`, `audio`)

## Colors (Source × Style)

Effect Colors panel:

| Axis | Options | Notes |
|------|---------|--------|
| **Source** | Look / Custom / Rainbow | Look = authored `colors:` ramp; Custom = your stops; Rainbow = spectrum |
| **Style** | Flat / Rich | Flat = simple fills (Basic); Rich = shade + highlights. Both stay available. |

Default for volume looks is **Look + Rich**. Flat and Rainbow Basic remain one click away. Hue/Sat tint Look and Custom; Reset to Look restores authored stops.

Globals (Speed / Size / Scale / Detail / Thickness / …) plus each look’s `slider:` / `param:` cover tweaks. Add a look slider when globals cannot express a distinct knob.
