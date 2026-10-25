name: Dual vortex
description: Twin counter-rotating swirls filling the room with colour
duration_ms: 8000
loop: while_active
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 95
slider: size 0 100 70
swatch: 255 90 40

void volumeMain(out vec4 out_color, in vec3 p01) {
    float inten = clamp(u_params[1], 0.0, 1.0);
    float scale = clamp(u_params[2], 0.05, 1.0);
    float t = u_time * (0.85 + scale);
    vec3 c0 = vec3(0.3, 0.5, 0.42);
    vec3 c1 = vec3(0.7, 0.5, 0.58);
    vec3 d0 = p01 - c0;
    vec3 d1 = p01 - c1;
    float a0 = atan(d0.z, d0.x) + t * 2.35 + d0.y * 3.8;
    float a1 = atan(d1.z, d1.x) - t * 2.05 + d1.y * 3.4;
    float r0 = length(d0.xz);
    float r1 = length(d1.xz);
    float s0 = 0.5 + 0.5 * sin(a0 * 4.0 + r0 * 13.0 - t);
    float s1 = 0.5 + 0.5 * sin(a1 * 4.0 + r1 * 13.0 + t * 0.8);
    float fall0 = 0.55 + 0.45 * (1.0 - smoothstep(0.0, 0.55, abs(d0.y)));
    float fall1 = 0.55 + 0.45 * (1.0 - smoothstep(0.0, 0.55, abs(d1.y)));
    float v0 = s0 * exp(-r0 * (1.9 / max(0.2, scale))) * fall0;
    float v1 = s1 * exp(-r1 * (1.9 / max(0.2, scale))) * fall1;
    // Soft room fill so corners still breathe
    float fill = 0.12 + 0.1 * sin(p01.x * 2.5 + p01.z * 2.0 + t * 0.5);
    float dens = clamp(v0 + v1 + fill, 0.0, 1.0);
    if(dens < 0.035) { out_color = vec4(0.0); return; }
    vec3 hot = vec3(1.0, 0.32, 0.08);
    vec3 cool = vec3(0.15, 0.5, 1.0);
    vec3 white = vec3(1.0, 0.9, 0.75);
    float mixv = clamp(v0 / max(v0 + v1, 1e-3), 0.0, 1.0);
    vec3 col = mix(cool, hot, mixv);
    col = mix(col, white, clamp(max(v0, v1) * 0.45, 0.0, 1.0));
    float a = dens * inten;
    out_color = vec4(col * a, a);
}
