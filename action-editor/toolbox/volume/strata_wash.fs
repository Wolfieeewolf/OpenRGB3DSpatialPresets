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
    float inten = clamp(u_params[1], 0.0, 1.0);
    float band = clamp(u_params[2], 0.05, 1.0);
    float y = p01.y;
    float t = u_time * 0.35;
    float drift = 0.5 + 0.5 * sin(p01.x * 3.2 + p01.z * 2.6 + t);
    float drift2 = 0.5 + 0.5 * sin(p01.z * 4.0 - t * 0.7 + p01.x);
    vec3 lowc = vec3(0.06, 0.14, 0.32);
    vec3 midc = vec3(0.16, 0.52, 0.95);
    vec3 hic = vec3(0.72, 0.9, 1.0);
    vec3 col = mix(lowc, midc, smoothstep(0.0, 0.4, y));
    col = mix(col, hic, smoothstep(0.45, 1.0, y));
    col = mix(col, vec3(0.4, 0.75, 1.0), drift * 0.2);
    float horizon = 1.0 - smoothstep(0.2 - band * 0.1, 0.52 + band * 0.15, abs(y - 0.34));
    // Floor + ceiling catch so desks/ceiling strips stay alive
    float floors = 0.2 * (1.0 - smoothstep(0.0, 0.18, y));
    float sky = 0.15 * smoothstep(0.78, 1.0, y);
    float body = 0.68 + 0.28 * horizon + 0.1 * drift + 0.06 * drift2 + floors + sky;
    body = clamp(body, 0.5, 1.0);
    float a = body * inten;
    out_color = vec4(col * a, a);
}
