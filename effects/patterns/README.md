# Patterns

1-D strip / kernel patterns (with a small 2D/3D-feel pilot set). Each `.kernel` file defines a short GLSL fragment that sets `k` in `[-1, 1]`.

Recent pilots (ideas only from Pixelblaze-style libraries — clean-room GLSL): `fire_wind`, `swirlpool`, `fireworks_nova`, `honeycomb`, `wavy_bands`.

Available globals: `s01`, `r`, `ph`, `u_phase`, `tsec`, `TWO_PI`, `hash11()`, `smstep()`, `fractf()`, plus cross-axis pilots `t01` / `u01`. The strip composer also injects **`SpatialVfxLib410`** (`vfx*`).

Patterns are a motion/texture layer on the effect shape. **Color from** (in the plugin UI) chooses:

- **Pattern** — this kernel’s authored palette
- **Effect** — Look / Custom / Rainbow with Hue / Sat and Flat / Rich; the kernel supplies layout and shade

Index numbers must be unique across all kernels (used for cross-referencing in presets and shell-pattern).
