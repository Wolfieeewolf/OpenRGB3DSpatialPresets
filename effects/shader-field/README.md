# Shader Field

2-D projected field effects. Each `.fs` file implements `spatialMain(out vec4 out_color, in vec2 frag_coord)`.

Two header formats coexist:

- **Stub format** (legacy majority): minimal `# Name` / `name:` header; params are auto-wired as `u_params[0]` = zoom, `[1]` = contrast, `[2]` = hue, `[3]` = detail.
- **Full format** (newer additions, e.g. `aurora_wash.fs`, `energy_ring.fs`, `scan_sweep.fs`, `star_sparkle.fs`): structured header with `class:`, `category:`, `global:`, `user_colors:`, explicit sliders, and `finish:`.

The `_example_plasma.fs` file is the reference template for new-format shaders.
