name: Portal vortex
description: Spinning energy portal disc with pull toward centre
duration_ms: 6000
loop: while_active
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 100
slider: size 0 100 65
swatch: 180 80 255

void volumeMain(out vec4 out_color, in vec3 p01) {
    float inten = clamp(u_params[1], 0.0, 1.0);
    float scale = clamp(u_params[2], 0.05, 1.0);
    float t = u_time * (1.05 + scale);
    vec3 o = vec3(0.5, 0.48, 0.5);
    vec3 d = p01 - o;
    float r = length(d.xz);
    float ang = atan(d.z, d.x) + t * 2.35 - d.y * 2.8;
    float spiral = 0.5 + 0.5 * sin(ang * 5.5 + r * 16.0 - t * 4.2);
    float ring_r = 0.16 + 0.24 * scale;
    float ring = 1.0 - smoothstep(0.0, 0.1 + 0.08 * scale, abs(r - ring_r));
    ring *= 0.55 + 0.45 * spiral;
    float disc = (1.0 - smoothstep(0.0, 0.38 * scale + 0.22, r)) * (0.25 + 0.2 * spiral);
    float core = exp(-r * 9.0) * (0.65 + 0.35 * sin(t * 5.5));
    // Vertical thickness — portal disc, not a flat floor stamp
    float yfall = 1.0 - smoothstep(0.18, 0.55, abs(d.y));
    // Orbiting spark beads on the ring (sparse-LED friendly)
    float beads = 0.0;
    for(int i = 0; i < 6; ++i) {
        float fi = float(i);
        float ba = ang + fi * 1.047 + t * 0.4;
        vec3 bp = o + vec3(cos(ba) * ring_r, 0.02 * sin(t * 3.0 + fi), sin(ba) * ring_r);
        beads += exp(-dot(p01 - bp, p01 - bp) / (0.004 + 0.006 * scale));
    }
    float dens = (max(ring, disc) * yfall + core + beads * 0.55);
    dens = clamp(dens, 0.0, 1.0);
    if(dens < 0.03) { out_color = vec4(0.0); return; }
    vec3 cold = vec3(0.15, 0.35, 1.0);
    vec3 hot = vec3(0.95, 0.3, 1.0);
    vec3 white = vec3(1.0, 0.95, 1.0);
    vec3 col = mix(cold, hot, spiral);
    col = mix(col, white, clamp(core + beads * 0.35, 0.0, 1.0));
    float a = dens * inten;
    out_color = vec4(col * a, a);
}
