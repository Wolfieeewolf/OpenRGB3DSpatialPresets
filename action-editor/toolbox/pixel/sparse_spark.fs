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
    float prog = vfxSaturate(u_params[0]);
    float inten = vfxSaturate(u_params[1]);
    float dens = clamp(u_params[2], 0.05, 1.0);
    // One possible spark per cell, at a random spot inside it, glowing softly
    vec3 g = p01 * mix(6.0, 18.0, dens);
    vec3 id = floor(g);
    vec3 f = fract(g);
    float n = vfxHash31(id);
    vec3 c = 0.25 + 0.5 * vec3(vfxHash31(id + 1.3), vfxHash31(id + 2.7), vfxHash31(id + 4.1));
    float glow = vfxSoftBand(length(f - c), 0.6);
    float twinkle = vfxPulse01(u_time * (10.0 + 8.0 * n) + vfxHash31(id + 7.7) * 6.283, 2.0);
    float life = 1.0 - smoothstep(0.0, 1.0, prog);
    life *= 1.0 - smoothstep(0.88, 1.0, prog);
    float a = step(0.82, n) * glow * twinkle * life * inten;
    if(a < 1e-4) { out_color = vec4(0.0); return; }
    // Hot white when it flares, cooling to orange as it fades
    vec3 col = vfxHeatRgb(0.55 + 0.45 * twinkle * life);
    out_color = vec4(col * a, a);
}
