name: Aurora wash
class: shader_field_aurora_wash
category: Shader Field
description: Soft vertical aurora curtains - atmosphere wash for walls / ceiling projection
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40FF99 #59A0FF
param: zoom
param: contrast
param: hue
param: detail
param: bands
param: color_mix
slider: bands 0 100 55 pct | Bands | How many curtain folds
slider: color_mix 0 100 45 pct | Color mix | Host Color / Rainbow / Pattern blend

void spatialMain(out vec4 out_color, in vec2 frag_coord) {
    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float t = u_time * 0.22;
    float b = mix(2.0, 7.0, u_params[4]);
    float w = sin(uv.x * b + t) * 0.5 + 0.5;
    w *= sin(uv.x * (b * 0.55) - t * 1.3) * 0.5 + 0.5;
    float curtain = pow(w, mix(1.6, 0.7, u_params[1] - 0.35));
    curtain *= smoothstep(0.0, 0.25, uv.y) * smoothstep(1.0, 0.55, uv.y);
    float shim = 0.5 + 0.5 * sin(uv.y * 18.0 + t * 4.0 + uv.x * 6.0);
    vec3 col = mix(vec3(0.02, 0.08, 0.12), vec3(0.2, 0.95, 0.55), curtain);
    col = mix(col, vec3(0.3, 0.55, 1.0), curtain * shim * 0.55);
    float hue = u_params[2];
    col = mix(col, col.brg, hue * 0.65);
    out_color = vec4(col, 1.0);
}
