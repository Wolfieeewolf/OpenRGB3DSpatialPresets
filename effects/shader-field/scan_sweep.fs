name: Scan sweep
class: shader_field_scan_sweep
category: Shader Field
description: Hard scanner beam sweeping across the projection plane (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #33FF88 #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: width
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: width 0 100 28 pct | Width | Scan beam width

void spatialMain(out vec4 out_color, in vec2 frag_coord) {
    float wash = clamp(u_params[4], 0.0, 1.0);
    float density = clamp(u_params[5], 0.0, 1.0);
    float tweak = clamp(u_params[6], 0.0, 1.0);
    float dens_mul = mix(0.12, 2.50, density);

    if(density < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float t = fract(u_time * 0.7316);
    float w = mix(0.0, 0.22, tweak); 
    if(w < 1e-4) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }
    float d = abs(uv.x - t);
    d = min(d, abs(uv.x - (t - 1.0)));
    float beam = exp(-pow(d / max(w, 0.008), 2.0) * 5.0);
    float grid = step(0.92, fract(uv.y * (12.0 + 28.0 * dens_mul) + (u_time * 2.613))) * 0.25 * wash;
    vec3 col = vec3(0.2, 1.0, 0.45) * beam;
    col += vec3(0.8) * beam * grid;
    float hue = u_params[2];
    col = mix(col, col.gbr, hue);
    float c = pow(clamp(beam + grid * beam, 0.0, 1.0), mix(1.1, 0.5, u_params[1] - 0.35));
    out_color = vec4(col * c, 1.0);
}
