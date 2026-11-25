name: Plane sweep
description: A bright plane wiping through the room
duration_ms: 1200
loop: once
section: basic
param: progress
param: intensity
param: size
slider: intensity 0 100 85
slider: size 0 100 25
swatch: 80 180 255

void volumeMain(out vec4 out_color, in vec3 p01) {
    float prog = vfxSaturate(u_params[0]);
    float inten = vfxSaturate(u_params[1]);
    float w = 0.04 + 0.2 * clamp(u_params[2], 0.05, 1.0);
    float d = p01.x - prog;
    // Bright core with a soft rim, and an afterglow left behind the plane
    float core = vfxCoreRim(d, w);
    float trail = exp(min(d, 0.0) / (w * 1.5)) * step(d, 0.0) * 0.3;
    float shimmer = 0.85 + 0.15 * vfxNoise3(vec3(p01.yz * 6.0, u_time * 2.0));
    float cover = max(core, trail) * shimmer;
    cover *= 1.0 - smoothstep(0.85, 1.0, prog);
    float a = cover * inten;
    if(a < 1e-4) { out_color = vec4(0.0); return; }
    vec3 col = mix(vec3(0.2, 0.5, 1.0), vec3(1.0), core);
    out_color = vec4(col * a, a);
}
