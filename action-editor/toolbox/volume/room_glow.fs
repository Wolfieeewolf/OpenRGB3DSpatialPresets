name: Room glow
description: Full-room wash - every LED stays lit with slow colour drift
duration_ms: 10000
loop: while_active
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 90
slider: size 0 100 80
swatch: 80 160 255

void volumeMain(out vec4 out_color, in vec3 p01) {
    float inten = vfxSaturate(u_params[1]);
    float scale = clamp(u_params[2], 0.05, 1.0);
    float t = u_time * (0.22 + 0.32 * scale);
    // Slow rolling clouds of light - never drops near zero (Atmos room light)
    vec3 w = vfxDomainWarp(p01 * 1.8, t, 0.25);
    float cloud = vfxFbm3(w * 1.6 + vec3(0.0, t * 0.3, 0.0), 3.0);
    float shimmer = vfxNoise3(w * 4.0 - vec3(t * 0.6));
    float body = clamp(0.55 + 0.38 * cloud + 0.1 * shimmer, 0.48, 1.0);
    vec3 col = vfxPalette3(p01.y, vec3(0.04, 0.1, 0.24), vec3(0.14, 0.42, 0.82), vec3(0.55, 0.85, 1.0));
    col = mix(col, vec3(0.35, 0.72, 1.0), cloud * 0.3);
    col = mix(col, vec3(0.55, 0.4, 0.95), smoothstep(0.55, 0.9, shimmer) * 0.25 * (1.0 - p01.y));
    float a = body * inten;
    out_color = vec4(col * a, a);
}
