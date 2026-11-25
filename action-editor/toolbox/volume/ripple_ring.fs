name: Ripple ring
description: Thick radial ripple that floods the room then fades
duration_ms: 2400
loop: once
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 90
slider: size 0 100 70
swatch: 100 200 255

void volumeMain(out vec4 out_color, in vec3 p01) {
    vec3 p = p01 * 2.0 - 1.0;
    float prog = vfxSaturate(u_params[0]);
    float inten = vfxSaturate(u_params[1]);
    float size = clamp(u_params[2], 0.05, 1.0);
    // Shockwave: warp the sample point so the front ripples instead of being a perfect sphere
    vec3 q = vfxDomainWarp(p * 1.4, u_time * 0.6, 0.10 + 0.08 * size) / 1.4;
    float r = length(q);
    float front = prog * 1.65;
    float band = 0.16 + 0.30 * size;
    float ring = vfxShell(r - front, band);
    // Wake: churning light left behind the front, so the room floods rather than blinks
    float churn = vfxFbm3(p * 2.2 + vec3(0.0, u_time * 0.4, 0.0), 3.0);
    float wake = 1.0 - smoothstep(front - band, front + band * 0.5, r);
    wake *= (0.35 + 0.65 * churn) * (1.0 - prog * 0.6);
    float cover = max(ring, wake * 0.6);
    cover *= 1.0 - smoothstep(0.78, 1.0, prog);
    float a = vfxSaturate(cover) * inten;
    if(a < 1e-4) { out_color = vec4(0.0); return; }
    vec3 col = mix(vec3(0.08, 0.28, 0.85), vec3(0.35, 0.75, 1.0), churn);
    col = mix(col, vec3(0.9, 0.97, 1.0), vfxSaturate(ring * 1.2 - 0.2));
    out_color = vec4(col * a, a);
}
