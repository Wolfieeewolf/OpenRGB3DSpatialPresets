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

float hash11(float n) { return fract(sin(n) * 43758.5453123); }

void volumeMain(out vec4 out_color, in vec3 p01) {
    float inten = clamp(u_params[1], 0.0, 1.0);
    float scale = clamp(u_params[2], 0.05, 1.0);
    float t = u_time * (2.2 + scale * 1.8);
    float dens = 0.0;
    // Soft ion haze so the room never goes fully black between bolts
    float haze = 0.04 + 0.03 * sin(p01.x * 4.0 + t * 0.5) * sin(p01.z * 3.0 - t * 0.4);
    dens += haze * (0.5 + 0.5 * scale);
    // Forking bolts — overlap two seed windows so reshuffles don't hard-cut
    for(int b = 0; b < 5; ++b)
    {
        float fb = float(b);
        float tick = t * 0.55 + fb * 0.17;
        float f0 = floor(tick);
        float f1 = f0 + 1.0;
        float blend = smoothstep(0.0, 1.0, fract(tick));
        for(int layer = 0; layer < 2; ++layer)
        {
            float fl = (layer == 0) ? f0 : f1;
            float wlayer = (layer == 0) ? (1.0 - blend) : blend;
            if(wlayer < 0.02) continue;
            float seed = hash11(fb * 19.7 + fl);
            float ax = 0.12 + 0.76 * hash11(seed * 3.1);
            float az = 0.12 + 0.76 * hash11(seed * 7.3);
            float ay0 = 0.04 + 0.18 * hash11(seed * 11.0);
            float ay1 = 0.72 + 0.22 * hash11(seed * 13.0);
            float best = 1e9;
            for(int s = 0; s < 10; ++s)
            {
                float u0 = float(s) / 10.0;
                float u1 = float(s + 1) / 10.0;
                float jx = (hash11(seed + u0 * 17.0) - 0.5) * 0.24 * scale;
                float jz = (hash11(seed + u0 * 29.0) - 0.5) * 0.24 * scale;
                float jx1 = (hash11(seed + u1 * 17.0) - 0.5) * 0.24 * scale;
                float jz1 = (hash11(seed + u1 * 29.0) - 0.5) * 0.24 * scale;
                vec3 a = vec3(ax + jx, mix(ay0, ay1, u0), az + jz);
                vec3 b = vec3(ax + jx1, mix(ay0, ay1, u1), az + jz1);
                vec3 ab = b - a;
                float ab2 = max(dot(ab, ab), 1e-6);
                float tt = clamp(dot(p01 - a, ab) / ab2, 0.0, 1.0);
                float d2 = dot(p01 - (a + ab * tt), p01 - (a + ab * tt));
                best = min(best, d2);
            }
            float flash = 0.5 + 0.5 * sin(t * 8.0 + fb * 2.2 + fl);
            // Fat kernel so sparse device grids still catch the bolt
            float bolt = exp(-best / (0.0035 + 0.0045 * scale));
            float glow = exp(-best / (0.012 + 0.014 * scale));
            dens += wlayer * flash * (bolt + 0.35 * glow);
        }
    }
    dens = clamp(dens, 0.0, 1.0);
    if(dens < 0.03) { out_color = vec4(0.0); return; }
    vec3 col = mix(vec3(0.3, 0.5, 1.0), vec3(1.0), clamp(dens * 1.15, 0.0, 1.0));
    float a = dens * inten;
    out_color = vec4(col * a, a);
}
