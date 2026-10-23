name: Example plasma
class: shader_field_example_plasma
category: Shader Field
description: Example Shader Field with header-driven Colors and sliders (spatialMain)
global: color speed brightness size scale detail frequency position
user_colors: 1
colors: #40A0FF
param: zoom
param: contrast
param: hue
param: detail
param: density
param: color_mix
slider: density 0 100 55 pct | Density | Pattern thickness
slider: color_mix 0 100 60 pct | Color mix | 0 = shader colors only, 100 = host Color / Rainbow / Pattern

void spatialMain(out vec4 out_color, in vec2 frag_coord) {
    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float t = u_time * 0.35;
    float d = u_params[4];
    float n = 0.5 + 0.5 * sin((uv.x * 8.0 + t) * (1.0 + d))
                    * sin((uv.y * 6.0 - t * 0.7) * (1.0 + d * 0.5));
    n = pow(clamp(n, 0.0, 1.0), mix(1.4, 0.6, u_params[1] - 0.35));
    vec3 col = mix(vec3(0.05, 0.1, 0.25), vec3(0.3, 0.7, 1.0), n);
    float hue = u_params[2];
    col = mix(col, col.gbr, hue);
    out_color = vec4(col, 1.0);
}
