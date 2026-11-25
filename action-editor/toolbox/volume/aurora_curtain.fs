name: Aurora curtain
description: Vertical aurora ribbons drifting through the room
duration_ms: 10000
loop: while_active
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 95
slider: size 0 100 70
swatch: 40 255 160

void volumeMain(out vec4 out_color, in vec3 p01) {
    float inten = vfxSaturate(u_params[1]);
    float scale = clamp(u_params[2], 0.05, 1.0);
    float t = u_time * (0.25 + 0.3 * scale);
    float y = p01.y;
    // Curtains are folded sheets: warp the floor plan so the sheets bend and drift
    vec3 w = vfxDomainWarp(vec3(p01.x * 2.0, y * 0.6, p01.z * 2.0), t, 0.35);
    float fold1 = sin(w.x * 4.0 + w.z * 2.5 + t + y * (1.8 + scale));
    float fold2 = sin(w.x * 2.6 - w.z * 3.8 - t * 1.2 + y * 1.1);
    float soft = 0.22 + 0.14 * scale;
    float sheet = max(vfxCoreRim(fold1 * 0.35, soft), vfxCoreRim(fold2 * 0.35, soft) * 0.8);
    // Vertical rays streaming up the curtain
    float rays = 0.55 + 0.45 * vfxNoise3(vec3(w.x * 9.0, y * 1.2 - t * 0.8, w.z * 9.0));
    float height = smoothstep(0.0, 0.3, y) * (1.0 - smoothstep(0.85, 1.0, y));
    height = max(height, 0.25);
    float haze = 0.08 * vfxFbm3(vec3(p01.x * 3.0, y * 2.0, p01.z * 3.0) + t * 0.3, 3.0);
    float dens = vfxSaturate(sheet * rays * (0.4 + 0.6 * height) + haze);
    if(dens < 0.035) { out_color = vec4(0.0); return; }
    // Green base, cyan middle, magenta crown - like a real aurora
    vec3 col = vfxPalette3(y, vec3(0.1, 1.0, 0.45), vec3(0.15, 0.75, 0.9), vec3(0.8, 0.25, 1.0));
    col = mix(col, vec3(0.85, 1.0, 0.95), vfxSaturate(sheet * rays - 0.75) * 2.0);
    float a = dens * inten;
    out_color = vec4(col * a, a);
}
