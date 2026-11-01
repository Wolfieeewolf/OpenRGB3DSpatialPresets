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
    float prog = clamp(u_params[0], 0.0, 1.0);
    float inten = clamp(u_params[1], 0.0, 1.0);
    float scale = clamp(u_params[2], 0.05, 1.0);
    float attack = smoothstep(0.0, 0.05, prog);
    float decay = pow(max(0.0, 1.0 - prog * 1.45), 1.8);
    float flash = attack * decay;
    // Hard gate so the last frames are fully black before the pack stops.
    flash *= 1.0 - smoothstep(0.82, 0.98, prog);
    // Pulse widens coverage; low pulse keeps a soft centre falloff.
    float cover = mix(0.55, 1.0, scale);
    float a = flash * inten * (cover + (1.0 - cover) * (1.0 - length(p01 - vec3(0.5))));
    if(a < 1e-4) { out_color = vec4(0.0); return; }
    vec3 col = mix(vec3(0.7, 0.8, 1.0), vec3(1.0), clamp(flash * 1.3, 0.0, 1.0));
    out_color = vec4(col * a, a);
}
