name: Strata wash
description: Height-banded room wash (horizon / ceiling / floor)
duration_ms: 10000
loop: while_active
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 70
slider: size 0 100 40
swatch: 255 140 60

void volumeMain(out vec4 out_color, in vec3 p01) {
    float inten = clamp(u_params[1], 0.0, 1.0);
    float band = clamp(u_params[2], 0.05, 1.0);
    float y = p01.y;
    float horizon = 1.0 - smoothstep(0.35 - band * 0.2, 0.55 + band * 0.25, abs(y - 0.42));
    float warm = mix(0.35, 1.0, horizon);
    vec3 lowc = vec3(0.15, 0.05, 0.08);
    vec3 midc = vec3(1.0, 0.45, 0.15);
    vec3 hic = vec3(0.35, 0.45, 0.9);
    vec3 col = mix(lowc, midc, smoothstep(0.0, 0.45, y));
    col = mix(col, hic, smoothstep(0.55, 1.0, y));
    float breathe = 0.85 + 0.15 * sin(u_time * 0.7);
    out_color = vec4(col * warm * inten * breathe, 1.0);
}
