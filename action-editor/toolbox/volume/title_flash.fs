name: Title flash
description: Room-filling white blast that pops then dies
duration_ms: 900
loop: once
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 100
slider: size 0 100 80
swatch: 255 255 255

void volumeMain(out vec4 out_color, in vec3 p01) {
    float prog = vfxSaturate(u_params[0]);
    float inten = vfxSaturate(u_params[1]);
    float scale = clamp(u_params[2], 0.05, 1.0);
    float attack = smoothstep(0.0, 0.05, prog);
    float decay = pow(max(0.0, 1.0 - prog * 1.45), 1.8);
    float flash = attack * decay;
    // Hard gate so the last frames are fully black before the pack stops.
    float gate = 1.0 - smoothstep(0.82, 0.98, prog);
    vec3 d = p01 - vec3(0.5);
    float r = length(d);
    float cover = mix(0.55, 1.0, scale);
    float bloom = flash * (cover + (1.0 - cover) * (1.0 - r));
    // Light shell racing out from the hit, plus a wide lens-flare streak across the room
    float shock = vfxShell(r - prog * 1.1, 0.10 + 0.08 * scale) * (1.0 - smoothstep(0.1, 0.6, prog));
    float streak = vfxSoftBand(length(d.yz), 0.10 + 0.10 * scale) * flash;
    float a = vfxSaturate(bloom + shock * 0.6 + streak * 0.5) * inten * gate;
    if(a < 1e-4) { out_color = vec4(0.0); return; }
    vec3 col = mix(vec3(0.7, 0.8, 1.0), vec3(1.0), vfxSaturate(flash * 1.3));
    col = mix(col, vec3(0.55, 0.75, 1.0), vfxSaturate(streak - bloom));
    out_color = vec4(col * a, a);
}
