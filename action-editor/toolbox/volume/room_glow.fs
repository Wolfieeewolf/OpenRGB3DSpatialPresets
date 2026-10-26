name: Room glow
description: Full-room wash — every LED stays lit with slow colour drift
duration_ms: 10000
loop: while_active
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 90
slider: size 0 100 80
swatch: 80 160 255

void volumeMain(out vec4 out_color, in vec3 p01) {
    float inten = clamp(u_params[1], 0.0, 1.0);
    float scale = clamp(u_params[2], 0.05, 1.0);
    float t = u_time * (0.22 + 0.32 * scale);
    // Soft volumetric fill — never drops near zero (Atmos room light)
    float wave = 0.5 + 0.5 * sin(p01.x * 3.0 + p01.z * 2.3 + t);
    float wave2 = 0.5 + 0.5 * sin(p01.y * 3.6 - t * 0.75 + p01.x * 1.4);
    float wave3 = 0.5 + 0.5 * sin(p01.z * 2.8 + p01.y * 1.5 + t * 0.55);
    float body = 0.58 + 0.24 * wave + 0.16 * wave2 + 0.1 * wave3;
    body = clamp(body, 0.48, 1.0);
    vec3 lowc = vec3(0.04, 0.1, 0.24);
    vec3 midc = vec3(0.14, 0.42, 0.82);
    vec3 hic = vec3(0.55, 0.85, 1.0);
    vec3 col = mix(lowc, midc, smoothstep(0.0, 0.5, p01.y));
    col = mix(col, hic, smoothstep(0.45, 1.0, p01.y));
    col = mix(col, vec3(0.35, 0.72, 1.0), wave * 0.3);
    col = mix(col, vec3(0.55, 0.4, 0.95), wave3 * 0.12 * (1.0 - p01.y));
    float a = body * inten;
    out_color = vec4(col * a, a);
}
