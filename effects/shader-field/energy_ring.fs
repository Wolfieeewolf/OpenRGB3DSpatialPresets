name: Energy Ring
class: shader_field_energy_ring
category: Shader Field
description: Expanding sci-fi ring / portal disc projected onto the room (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #8C59FF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: thickness
slider: wash 0 100 60 pct | Wash: | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density: | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: thickness 0 100 35 pct | Thickness | Ring shell width

void spatialMain(out vec4 out_color, in vec2 frag_coord) {
    float wash = clamp(u_params[4], 0.0, 1.0);
    float density = clamp(u_params[5], 0.0, 1.0);
    float tweak = clamp(u_params[6], 0.0, 1.0);
    float dens_mul = mix(0.12, 2.50, density);

    if(density < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    vec2 p = uv * 2.0 - 1.0;
    float t = u_time * 1.437;
    float thick = mix(0.0, 0.28, tweak); 
    if(thick < 1e-4) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }
    float radius = 0.25 + fract(t * 0.35 * dens_mul) * 0.85;
    float d = abs(length(p) - radius);
    float ring = exp(-pow(d / max(thick, 0.01), 2.0) * 4.0);
    float core = exp(-length(p) * length(p) * 6.0) * 0.15;
    float pulse = 0.65 + 0.35 * sin(t * 6.28318 + length(p) * 8.0);
    float sharp = ring * pulse;
    float softv = clamp(ring * pulse + core, 0.0, 1.0);
    float v = mix(sharp, softv, wash);
    vec3 col = mix(vec3(0.0), vec3(0.25, 0.85, 1.0), v);
    col += vec3(0.55, 0.3, 1.0) * core * wash;
    float hue = u_params[2];
    col = mix(col, col.gbr, hue);
    float c = pow(clamp(v, 0.0, 1.0), mix(1.2, 0.55, u_params[1] - 0.35));
    out_color = vec4(col * c, 1.0);
}
