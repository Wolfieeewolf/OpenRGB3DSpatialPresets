name: Plane sweep
description: Room-plane wipe through Spatial Anchor volume
duration_ms: 1200
loop: once
section: basic
param: progress
param: intensity
param: size
slider: intensity 0 100 85
slider: size 0 100 25
swatch: 80 180 255

void volumeMain(out vec4 out_color, in vec3 p01) {
    float prog = clamp(u_params[0], 0.0, 1.0);
    float inten = clamp(u_params[1], 0.0, 1.0);
    float w = 0.04 + 0.2 * clamp(u_params[2], 0.05, 1.0);
    float d = abs(p01.x - prog);
    float cover = 1.0 - smoothstep(0.0, w, d);
    cover *= 1.0 - smoothstep(0.85, 1.0, prog);
    float a = cover * inten;
    if(a < 1e-4) { out_color = vec4(0.0); return; }
    vec3 col = mix(vec3(0.2, 0.5, 1.0), vec3(1.0, 1.0, 1.0), cover);
    out_color = vec4(col * a, a);
}
