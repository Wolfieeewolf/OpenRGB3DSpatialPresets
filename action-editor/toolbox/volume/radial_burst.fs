name: Radial burst
description: Expanding shell from the room centre
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
    float prog = vfxSaturate(u_params[0]);
    float inten = vfxSaturate(u_params[1]);
    float size = clamp(u_params[2], 0.05, 1.0);
    float t = u_time;
    float front = prog * 1.4;
    // Turbulent shock front
    vec3 q = vfxDomainWarp(p * 1.6, t * 1.2, 0.18) / 1.6;
    float r = length(q);
    float shock = vfxShell(r - front, 0.08 + 0.18 * size);
    // Fireball: boiling core that grows, then burns out
    vec3 outward = p / max(length(p), 1e-3);
    float turb = vfxFbm3(p * 3.0 - outward * t * 1.5, 4.0);
    float fireball = (1.0 - smoothstep(0.0, 0.55 + 0.6 * front, r)) * (1.0 - prog);
    fireball *= 0.55 + 0.75 * turb;
    // Debris: glowing embers flung out ahead of the blast, with short tails
    float debris = 0.0;
    for(int i = 0; i < 10; ++i) {
        float fi = float(i);
        vec3 dir = normalize(vec3(vfxHash11(fi * 7.31), vfxHash11(fi * 3.17 + 1.3), vfxHash11(fi * 5.77 + 2.1)) * 2.0 - 1.0 + 1e-3);
        vec3 pos = dir * front * (0.9 + 0.25 * vfxHash11(fi * 9.1));
        debris += vfxStreak(p - pos, dir, 0.22 + 0.12 * size, 0.10 + 0.06 * size);
    }
    debris *= 1.0 - smoothstep(0.55, 0.9, prog);
    // Initial flash lights the whole room for a beat before the blast takes shape
    float flash = exp(-r * 2.2) * (1.0 - smoothstep(0.0, 0.3, prog));
    float heat = shock * 0.85 + fireball + debris * 0.7 + flash;
    heat *= 1.0 - smoothstep(0.88, 1.0, prog);
    float a = vfxSaturate(heat) * inten;
    if(a < 1e-4) { out_color = vec4(0.0); return; }
    vec3 col = vfxHeatRgb(0.35 + 0.65 * vfxSaturate(heat * 0.8 + (1.0 - prog) * 0.25));
    out_color = vec4(col * a, a);
}
