name: Sparse spark
description: Sparse sparkles flickering through the room
duration_ms: 1600
loop: once
section: pixel
param: progress
param: intensity
param: size
slider: intensity 0 100 80
slider: size 0 100 35
swatch: 255 220 120

void volumeMain(out vec4 out_color, in vec3 p01) {
    float prog = clamp(u_params[0], 0.0, 1.0);
    float inten = clamp(u_params[1], 0.0, 1.0);
    float dens = clamp(u_params[2], 0.05, 1.0);
    vec3 g = floor(p01 * mix(8.0, 28.0, dens));
    float n = fract(sin(dot(g, vec3(12.9898, 78.233, 45.164))) * 43758.5453);
    float life = 1.0 - smoothstep(0.0, 1.0, prog);
    life *= 1.0 - smoothstep(0.88, 1.0, prog);
    float spark = step(0.92, n) * life;
    float flicker = step(0.5, fract(n * 17.0 + u_time * 9.0));
    float a = spark * flicker * inten;
    if(a < 1e-4) { out_color = vec4(0.0); return; }
    vec3 col = mix(vec3(1.0, 0.6, 0.2), vec3(1.0, 1.0, 0.85), n);
    out_color = vec4(col * a, a);
}
