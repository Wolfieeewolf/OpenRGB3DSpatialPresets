name: Dual vortex
description: Twin counter-rotating swirls filling the room
duration_ms: 8000
loop: while_active
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 95
slider: size 0 100 70
swatch: 255 90 40

float dvVortex(vec3 d, float dir, float t, float scale) {
    float r = length(d.xz);
    // Twisting arms that wind tighter near the eye
    float ang = atan(d.z, d.x) * 3.0 + dir * (t * 2.3 + d.y * 3.6) - dir * r * 14.0;
    float n = vfxFbm3(vec3(cos(ang), sin(ang), r * 4.0 - t * dir) * 1.5, 3.0);
    float arms = vfxSaturate(0.5 + 0.5 * sin(ang) + (n - 0.5) * 0.9);
    float fall = exp(-r * (1.9 / max(0.2, scale)));
    float tube = 0.55 + 0.45 * (1.0 - smoothstep(0.0, 0.55, abs(d.y)));
    float eye = vfxCoreRim(r, 0.06 + 0.06 * scale) * tube;
    return arms * arms * fall * tube + eye * 0.8;
}
void volumeMain(out vec4 out_color, in vec3 p01) {
    float inten = vfxSaturate(u_params[1]);
    float scale = clamp(u_params[2], 0.05, 1.0);
    float t = u_time * (0.85 + scale);
    float v0 = dvVortex(p01 - vec3(0.3, 0.5, 0.42), 1.0, t, scale);
    float v1 = dvVortex(p01 - vec3(0.7, 0.5, 0.58), -1.0, t * 0.9, scale);
    // Where the two storms meet, energy crackles white-hot
    float clash = vfxSaturate(v0 * v1 * 4.0) * (0.6 + 0.4 * vfxNoise3(p01 * 12.0 + t * 3.0));
    float fill = 0.12 + 0.1 * vfxFbm3(p01 * 2.5 + vec3(t * 0.3), 2.0);
    float dens = vfxSaturate(v0 + v1 + fill + clash);
    if(dens < 0.035) { out_color = vec4(0.0); return; }
    vec3 hot = vec3(1.0, 0.32, 0.08);
    vec3 cool = vec3(0.15, 0.5, 1.0);
    vec3 col = mix(cool, hot, vfxSaturate(v0 / max(v0 + v1, 1e-3)));
    col = mix(col, vec3(1.0, 0.9, 0.75), vfxSaturate(max(v0, v1) * 0.45 + clash));
    float a = dens * inten;
    out_color = vec4(col * a, a);
}
