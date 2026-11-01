name: Energy ring
class: shader_field_energy_ring
category: Shader Field
description: Expanding sci-fi ring / portal disc projected onto the room
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #8C59FF
param: zoom
param: contrast
param: hue
param: detail
param: thickness
param: color_mix
slider: thickness 0 100 35 pct | Ring thickness | Soft shell width
slider: color_mix 0 100 55 pct | Color mix | Host Color / Rainbow / Pattern blend

void spatialMain(out vec4 out_color, in vec2 frag_coord) {
    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    vec2 p = uv * 2.0 - 1.0;
    float t = u_time * 0.55;
    float thick = mix(0.04, 0.22, u_params[4]);
    float radius = 0.25 + fract(t * 0.35) * 0.85;
    float d = abs(length(p) - radius);
    float ring = exp(-pow(d / max(thick, 0.01), 2.0) * 4.0);
    float core = exp(-length(p) * length(p) * 6.0) * 0.15;
    float pulse = 0.65 + 0.35 * sin(t * 6.28318 + length(p) * 8.0);
    vec3 col = mix(vec3(0.05, 0.12, 0.22), vec3(0.25, 0.85, 1.0), ring * pulse);
    col += vec3(0.55, 0.3, 1.0) * core;
    float hue = u_params[2];
    col = mix(col, col.gbr, hue);
    float c = pow(clamp(ring + core, 0.0, 1.0), mix(1.2, 0.55, u_params[1] - 0.35));
    out_color = vec4(col * c, 1.0);
}
