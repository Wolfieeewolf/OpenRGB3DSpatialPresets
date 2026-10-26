# Volume

Soft 3-D field effects. Each `.fs` file implements `volumeMain(out vec4 out_color, in vec3 p01)` where `p01` is the LED position normalized to `[0,1]³` within the occupancy atlas.

Output conventions:
- `out_color.r` = intensity (brightness gate)
- `out_color.g` = gradient / palette driver (meaning varies by `finish:`)
- `out_color.b` = auxiliary (hotness, pulse index, etc. — documented per-file when used)
- `finish:` tag selects the host coloring pipeline (`hex`, `atlas`, `hsv`, `depth`, `surface`, `spiral`, `rgb`)
