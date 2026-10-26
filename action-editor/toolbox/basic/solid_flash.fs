name: Solid flash
description: Room-filling flash that pops then dies
duration_ms: 800
loop: once
section: basic
param: progress
param: intensity
param: size
slider: intensity 0 100 100
slider: size 0 100 80
swatch: 255 255 255

void volumeMain(out vec4 out_color, in vec3 p01) {
    float prog = clamp(u_params[0], 0.0, 1.0);
    float inten = clamp(u_params[1], 0.0, 1.0);
    float scale = clamp(u_params[2], 0.05, 1.0);
    float env = 1.0 - smoothstep(0.12, 0.95, prog);
    // Hard gate so the last frames are fully black before the pack stops.
    env *= 1.0 - smoothstep(0.82, 0.98, prog);
    // Pulse widens coverage; low pulse keeps a soft centre falloff.
    float cover = mix(0.55, 1.0, scale);
    float a = env * inten * (cover + (1.0 - cover) * (1.0 - length(p01 - vec3(0.5))));
    if(a < 1e-4) { out_color = vec4(0.0); return; }
    vec3 col = vec3(1.0, 0.95, 0.9);
    out_color = vec4(col * a, a);
}
