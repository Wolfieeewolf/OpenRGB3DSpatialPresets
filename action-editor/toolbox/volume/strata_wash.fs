name: Strata wash
description: Cool height-banded wash across floor, mid, and sky
duration_ms: 10000
loop: while_active
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 90
slider: size 0 100 70
swatch: 60 140 220

void volumeMain(out vec4 out_color, in vec3 p01) {
    float inten = vfxSaturate(u_params[1]);
    float band = clamp(u_params[2], 0.05, 1.0);
    float t = u_time * 0.35;
    // Layers bend gently, like haze bands over a horizon
    float bend = (vfxFbm3(vec3(p01.x * 2.0, t * 0.5, p01.z * 2.0), 3.0) - 0.5) * 0.16;
    float y = p01.y + bend;
    float drift = vfxFbm3(vec3(p01.x * 3.2, y * 6.0, p01.z * 2.6) + vec3(t, 0.0, 0.0), 3.0);
    vec3 col = vfxPalette3(y, vec3(0.06, 0.14, 0.32), vec3(0.16, 0.52, 0.95), vec3(0.72, 0.9, 1.0));
    col = mix(col, vec3(0.4, 0.75, 1.0), drift * 0.25);
    float horizon = vfxSoftBand(y - 0.34, 0.22 + 0.16 * band);
    // Floor + ceiling catch so desks/ceiling strips stay alive
    float floors = 0.2 * (1.0 - smoothstep(0.0, 0.18, p01.y));
    float sky = 0.15 * smoothstep(0.78, 1.0, p01.y);
    float body = clamp(0.66 + 0.28 * horizon + 0.14 * drift + floors + sky, 0.5, 1.0);
    float a = body * inten;
    out_color = vec4(col * a, a);
}
