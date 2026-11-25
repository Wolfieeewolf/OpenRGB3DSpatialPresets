name: Portal vortex
description: Spinning energy portal with pull toward the centre
duration_ms: 6000
loop: while_active
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 100
slider: size 0 100 65
swatch: 180 80 255

void volumeMain(out vec4 out_color, in vec3 p01) {
    float inten = vfxSaturate(u_params[1]);
    float scale = clamp(u_params[2], 0.05, 1.0);
    float t = u_time * (1.05 + scale);
    vec3 o = vec3(0.5, 0.48, 0.5);
    vec3 d = p01 - o;
    float r = length(d.xz);
    float ang = atan(d.z, d.x) + t * 2.35 - d.y * 2.8;
    float swirl = vfxFbm3(vec3(cos(ang * 2.0), sin(ang * 2.0), r * 6.0 - t * 2.0), 3.0);
    float spiral = vfxSaturate(0.5 + 0.5 * sin(ang * 5.5 + r * 16.0 - t * 4.2) + (swirl - 0.5) * 0.6);
    // Event horizon: a 3D ring whose edge shimmers
    float ring_r = 0.16 + 0.24 * scale;
    vec3 dw = vfxDomainWarp(d * 3.0, t * 0.7, 0.12) / 3.0;
    float ring = vfxShell(vfxSdTorus(dw, vec2(ring_r, 0.0)), 0.14 + 0.10 * scale);
    ring *= 0.55 + 0.45 * spiral;
    float disc = (1.0 - smoothstep(0.0, 0.38 * scale + 0.22, r)) * (0.3 + 0.35 * swirl);
    float core = vfxCoreRim(length(d * vec3(1.0, 0.5, 1.0)), 0.14) * (0.65 + 0.35 * sin(t * 5.5));
    float yfall = 1.0 - smoothstep(0.18, 0.55, abs(d.y));
    // Sparks riding the ring, trailing light behind them
    float beads = 0.0;
    for(int i = 0; i < 6; ++i) {
        float fi = float(i);
        float ba = fi * 1.047 - t * 2.35;
        vec3 bp = o + vec3(cos(ba) * ring_r, 0.02 * sin(t * 3.0 + fi), sin(ba) * ring_r);
        vec3 vel = vec3(sin(ba), 0.0, -cos(ba));
        beads += vfxStreak(p01 - bp, vel, 0.08 + 0.05 * scale, 0.05 + 0.04 * scale);
    }
    float dens = vfxSaturate(max(ring, disc) * yfall + core + beads * 0.6);
    if(dens < 0.03) { out_color = vec4(0.0); return; }
    vec3 col = mix(vec3(0.15, 0.35, 1.0), vec3(0.95, 0.3, 1.0), spiral);
    col = mix(col, vec3(1.0, 0.95, 1.0), vfxSaturate(core + beads * 0.35));
    float a = dens * inten;
    out_color = vec4(col * a, a);
}
