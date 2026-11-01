name: Scan sweep
class: shader_field_scan_sweep
category: Shader Field
description: Hard scanner beam sweeping across the projection plane
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #33FF88 #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: width
param: color_mix
slider: width 0 100 28 pct | Beam width | Soft edge of the scan line
slider: color_mix 0 100 70 pct | Color mix | Host Color / Rainbow / Pattern blend

void spatialMain(out vec4 out_color, in vec2 frag_coord) {
    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float t = fract(u_time * 0.28);
    float w = mix(0.02, 0.18, u_params[4]);
    float d = abs(uv.x - t);
    d = min(d, abs(uv.x - (t - 1.0)));
    float beam = exp(-pow(d / max(w, 0.008), 2.0) * 5.0);
    float grid = step(0.92, fract(uv.y * 24.0 + u_time)) * 0.25;
    vec3 col = vec3(0.05, 0.12, 0.08) + vec3(0.2, 1.0, 0.45) * beam;
    col += vec3(0.8) * beam * grid;
    float hue = u_params[2];
    col = mix(col, col.gbr, hue);
    float c = pow(clamp(beam + grid * beam, 0.0, 1.0), mix(1.1, 0.5, u_params[1] - 0.35));
    out_color = vec4(col * max(c, 0.05), 1.0);
}
