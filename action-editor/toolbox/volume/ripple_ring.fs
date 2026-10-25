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
    float r = length(p);
    float prog = clamp(u_params[0], 0.0, 1.0);
    float inten = clamp(u_params[1], 0.0, 1.0);
    float band = 0.18 + 0.35 * clamp(u_params[2], 0.05, 1.0);
    float front = prog * 1.65;
    float ring = 1.0 - smoothstep(0.0, band, abs(r - front));
    // Fill inside the wave so the room lights up, not just a thin shell
    float fill = (1.0 - smoothstep(0.0, front + band * 0.5, r)) * (1.0 - prog * 0.55);
    float cover = max(ring, fill * 0.55);
    cover *= 1.0 - smoothstep(0.78, 1.0, prog);
    float a = cover * inten;
    if(a < 1e-4) { out_color = vec4(0.0); return; }
    vec3 col = mix(vec3(0.1, 0.35, 0.85), vec3(0.85, 0.95, 1.0), clamp(cover, 0.0, 1.0));
    out_color = vec4(col * a, a);
}
