name: Aurora wash
class: shader_field_aurora_wash
category: Shader Field
description: Soft vertical aurora curtains - atmosphere wash for walls / ceiling projection (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40FF99 #59A0FF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: bands
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: bands 0 100 55 pct | Bands | Curtain fold count

void spatialMain(out vec4 out_color, in vec2 frag_coord) {
    float wash = clamp(u_params[4], 0.0, 1.0);
    float density = clamp(u_params[5], 0.0, 1.0);
    float tweak = clamp(u_params[6], 0.0, 1.0);
    float dens_mul = mix(0.12, 2.50, density);

    if(density < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float t = u_time * 0.5748;
    float b = mix(0.5, 9.0, tweak) * dens_mul;
    if(tweak < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }
    float w = sin(uv.x * b + t) * 0.5 + 0.5;
    w *= sin(uv.x * (b * 0.55) - t * 1.3) * 0.5 + 0.5;
    float curtain = pow(w, mix(1.6, 0.7, u_params[1] - 0.35));
    curtain *= smoothstep(0.0, 0.25, uv.y) * smoothstep(1.0, 0.55, uv.y);
    float shim = 0.5 + 0.5 * sin(uv.y * 18.0 + t * 4.0 + uv.x * 6.0);
    float crest = pow(clamp(curtain, 0.0, 1.0), 2.4);
    float body = clamp(curtain, 0.0, 1.0);
    float lit = mix(crest, body, wash);
    vec3 col = mix(vec3(0.0), vec3(0.2, 0.95, 0.55), lit);
    col = mix(col, vec3(0.3, 0.55, 1.0), lit * shim * 0.55 * wash);
    float hue = u_params[2];
    col = mix(col, col.brg, hue * 0.65);
    out_color = vec4(col, 1.0);
}
