name: Radial burst
description: Expanding shell from Spatial Anchor (p01 0.5)
duration_ms: 1400
loop: once
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 90
slider: size 0 100 50
swatch: 255 80 20

void volumeMain(out vec4 out_color, in vec3 p01) {
    vec3 p = p01 * 2.0 - 1.0;
    float r = length(p);
    float prog = clamp(u_params[0], 0.0, 1.0);
    float inten = clamp(u_params[1], 0.0, 1.0);
    float band = 0.08 + 0.18 * clamp(u_params[2], 0.05, 1.0);
    float front = prog * 1.4;
    float d = abs(r - front);
    float cover = 1.0 - smoothstep(0.0, band, d);
    cover *= 1.0 - smoothstep(0.7, 1.0, prog);
    float core = exp(-r * 2.2) * (1.0 - prog);
    vec3 col = mix(vec3(1.0, 0.2, 0.05), vec3(1.0, 0.9, 0.4), clamp(1.0 - r, 0.0, 1.0));
    out_color = vec4(col * (cover + core) * inten, 1.0);
}
