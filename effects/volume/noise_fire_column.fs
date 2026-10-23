name: Noise fire column
class: volume_noise_fire_column
category: Volume
description: Height-driven noise fire column (kit) — fuel in XZ, heat along Y, not a soft blob
global: color speed brightness size scale detail
user_colors: 1
soft_catch: 0.045
resolution: 32
colors: #FF6A1A
slider: fuel 0 100 78 pct | Fuel | Base burn strength
slider: flicker 0 100 62 pct | Flicker | Noise liveliness
slider: column 0 100 48 pct | Column | How tight the column is

void volumeMain(out vec4 out_color, in vec3 p01) {
    float fuel = clamp(u_params[0], 0.05, 1.0);
    float flick = clamp(u_params[1], 0.05, 1.0);
    float col_w = mix(0.42, 0.1, clamp(u_params[2], 0.0, 1.0));
    float h = kit_height01(p01);
    vec2 xz = kit_roomUv(p01) - vec2(0.5);
    float radial = length(xz);
    float mask = exp(-pow(radial / max(col_w, 0.04), 2.2));
    float base = fuel * (1.0 - smoothstep(0.12, 0.92, h));
    vec3 np = vec3(xz * 4.0, h * 2.4 - u_time * (1.4 + flick));
    float n = kit_noiseStack(np, 3, 0.52);
    float tip = kit_snoise(vec3(xz * 7.0, h * 4.5 - u_time * 2.8));
    float dens = mask * base * (0.3 + 0.7 * n) * (0.65 + 0.35 * tip);
    dens *= smoothstep(0.02, 0.1, h);
    // Hot core near axis, cooler tips
    float core = exp(-pow(radial / max(col_w * 0.45, 0.02), 2.0));
    vec3 hot = kit_hsv2rgb(vec3(0.08 + 0.06 * (1.0 - h), 0.9, 0.7 + 0.3 * dens));
    vec3 cool = kit_hsv2rgb(vec3(0.02 + 0.04 * n, 0.75, 0.35 + 0.4 * dens));
    vec3 col = mix(cool, hot, core);
    out_color = vec4(col * dens * 1.65, 1.0);
}
