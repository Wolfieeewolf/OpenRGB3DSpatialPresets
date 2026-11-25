name: Electric storm
description: Crackling lightning forks jumping through the room
duration_ms: 4000
loop: while_active
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 100
slider: size 0 100 60
swatch: 160 200 255

void volumeMain(out vec4 out_color, in vec3 p01) {
    float inten = vfxSaturate(u_params[1]);
    float scale = clamp(u_params[2], 0.05, 1.0);
    float t = u_time * (2.2 + scale * 1.8);
    float dens = 0.0;
    float lightning = 0.0;
    // Forking bolts - overlap two seed windows so reshuffles don't hard-cut
    for(int b = 0; b < 5; ++b)
    {
        float fb = float(b);
        float tick = t * 0.55 + fb * 0.17;
        float f0 = floor(tick);
        float blend = smoothstep(0.0, 1.0, fract(tick));
        for(int layer = 0; layer < 2; ++layer)
        {
            float fl = f0 + float(layer);
            float wlayer = (layer == 0) ? (1.0 - blend) : blend;
            if(wlayer < 0.02) continue;
            float seed = vfxHash11(fb * 19.7 + fl);
            float ax = 0.12 + 0.76 * vfxHash11(seed * 3.1);
            float az = 0.12 + 0.76 * vfxHash11(seed * 7.3);
            float ay0 = 0.04 + 0.18 * vfxHash11(seed * 11.0);
            float ay1 = 0.72 + 0.22 * vfxHash11(seed * 13.0);
            float best = 1e9;
            for(int s = 0; s < 10; ++s)
            {
                float u0 = float(s) / 10.0;
                float u1 = float(s + 1) / 10.0;
                vec3 j0 = (vec3(vfxHash11(seed + u0 * 17.0), 0.5, vfxHash11(seed + u0 * 29.0)) - 0.5) * 0.24 * scale;
                vec3 j1 = (vec3(vfxHash11(seed + u1 * 17.0), 0.5, vfxHash11(seed + u1 * 29.0)) - 0.5) * 0.24 * scale;
                vec3 a = vec3(ax, mix(ay0, ay1, u0), az) + j0;
                vec3 c = vec3(ax, mix(ay0, ay1, u1), az) + j1;
                best = min(best, vfxSdCapsule(p01, a, c, 0.0));
            }
            float flash = 0.5 + 0.5 * sin(t * 8.0 + fb * 2.2 + fl);
            // White-hot core with a violet halo, fat enough for sparse LED layouts
            float bolt = vfxCoreRim(best, 0.11 + 0.06 * scale);
            float glow = vfxSoftBand(best, 0.11 + 0.05 * scale);
            dens += wlayer * flash * (bolt + 0.35 * glow);
            lightning += wlayer * flash * 0.2;
        }
    }
    // Storm cloud along the ceiling, lit from inside by the strikes
    float cloud = vfxFbm3(vec3(p01.x * 3.0, p01.y * 1.5 + t * 0.05, p01.z * 3.0) + vec3(t * 0.08, 0.0, 0.0), 4.0);
    cloud *= smoothstep(0.55, 1.0, p01.y);
    float haze = 0.04 + 0.03 * vfxNoise3(p01 * 4.0 + vec3(t * 0.3));
    dens += cloud * (0.12 + 0.6 * lightning) + haze * (0.5 + 0.5 * scale);
    dens = vfxSaturate(dens);
    if(dens < 0.03) { out_color = vec4(0.0); return; }
    vec3 col = mix(vec3(0.35, 0.4, 1.0), vec3(1.0), vfxSaturate(dens * 1.15));
    col = mix(col, vec3(0.55, 0.45, 0.9), cloud * (1.0 - lightning) * 0.5);
    float a = dens * inten;
    out_color = vec4(col * a, a);
}
