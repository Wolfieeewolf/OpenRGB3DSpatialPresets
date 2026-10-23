name: Solid flash
description: Full-volume flash / hold wash
duration_ms: 800
loop: once
section: basic
param: progress
param: intensity
slider: intensity 0 100 100
swatch: 255 255 255

void volumeMain(out vec4 out_color, in vec3 p01) {
    float prog = clamp(u_params[0], 0.0, 1.0);
    float inten = clamp(u_params[1], 0.0, 1.0);
    float env = 1.0 - smoothstep(0.15, 1.0, prog);
    vec3 col = vec3(1.0, 0.95, 0.9);
    out_color = vec4(col * env * inten, 1.0);
}
