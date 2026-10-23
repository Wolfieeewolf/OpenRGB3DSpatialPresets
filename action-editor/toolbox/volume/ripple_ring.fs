name: Ripple ring
description: Soft radial ripple from anchor
duration_ms: 1800
loop: once
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 80
slider: size 0 100 40
swatch: 100 200 255

void volumeMain(out vec4 out_color, in vec3 p01) {
    vec3 p = p01 * 2.0 - 1.0;
    float r = length(p);
    float prog = clamp(u_params[0], 0.0, 1.0);
    float inten = clamp(u_params[1], 0.0, 1.0);
    float band = 0.06 + 0.14 * clamp(u_params[2], 0.05, 1.0);
    float front = prog * 1.5;
    float ring = 1.0 - smoothstep(0.0, band, abs(r - front));
    ring *= 1.0 - smoothstep(0.75, 1.0, prog);
    vec3 col = mix(vec3(0.2, 0.6, 1.0), vec3(0.9, 0.95, 1.0), ring);
    out_color = vec4(col * ring * inten, 1.0);
}
