name: Aurora curtain
description: Vertical aurora ribbons drifting through the room
duration_ms: 10000
loop: while_active
section: volume
param: progress
param: intensity
param: size
slider: intensity 0 100 95
slider: size 0 100 70
swatch: 40 255 160

void volumeMain(out vec4 out_color, in vec3 p01) {
    float inten = clamp(u_params[1], 0.0, 1.0);
    float scale = clamp(u_params[2], 0.05, 1.0);
    float t = u_time * (0.32 + 0.38 * scale);
    float x = p01.x;
    float y = p01.y;
    float z = p01.z;
    // Layered curtains with shear + slow lateral drift (LED-cube aurora language)
    float shear = y * (2.2 + 1.4 * scale);
    float c1 = sin((x * 6.5 + z * 2.8) + t + shear);
    float c2 = sin((x * 10.5 - z * 4.5) - t * 1.25 + y * 3.6);
    float c3 = sin((x * 3.8 + z * 7.5) + t * 0.65 + y * 1.2);
    float ribbon = 0.5 + 0.3 * c1 + 0.24 * c2 + 0.16 * c3;
    ribbon = pow(clamp(ribbon, 0.0, 1.0), 1.25);
    // Soft vertical band — bright mid, never a hard cutoff that leaves floors dead
    float height = smoothstep(0.02, 0.28, y) * (1.0 - smoothstep(0.82, 1.0, y));
    height = max(height, 0.22);
    float dens = ribbon * (0.4 + 0.6 * height);
    // Occasional bright filament so the look has character, not flat wash
    float filament = pow(max(0.0, c1 * c2), 3.0) * 0.55;
    dens = clamp(dens + filament, 0.0, 1.0);
    if(dens < 0.035) { out_color = vec4(0.0); return; }
    vec3 g = vec3(0.1, 0.95, 0.42);
    vec3 c = vec3(0.2, 0.5, 1.0);
    vec3 m = vec3(0.9, 0.3, 1.0);
    vec3 col = mix(g, c, clamp(0.5 + 0.5 * c2, 0.0, 1.0));
    col = mix(col, m, clamp(0.3 + 0.4 * c3, 0.0, 1.0) * (1.0 - y));
    col = mix(col, vec3(0.85, 1.0, 0.95), filament);
    float a = dens * inten;
    out_color = vec4(col * a, a);
}
