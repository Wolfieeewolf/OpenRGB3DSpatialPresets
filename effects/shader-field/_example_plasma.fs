name: Example plasma
class: shader_field_example_plasma
category: Shader Field
description: Example Shader Field with header-driven Colors and sliders (spatialMain) (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40A0FF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: amount
slider: wash 0 100 60 pct | Wash | Soft fill amount (0 = sharper crests, 100 = full soft look)
slider: density 0 100 50 pct | Density | Pattern spacing / feature count
slider: amount 0 100 55 pct | Amount | Pattern thickness

void spatialMain(out vec4 out_color, in vec2 frag_coord) {
    float wash = clamp(u_params[4], 0.0, 1.0);
    float density = clamp(u_params[5], 0.0, 1.0);
    float tweak = clamp(u_params[6], 0.0, 1.0);
    float dens_mul = mix(0.12, 2.50, density);

    if(density < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float t = u_time * 0.9145;
    float d = tweak;
    float n = 0.5 + 0.5 * sin((uv.x * 8.0 * dens_mul + t) * (1.0 + d))
                    * sin((uv.y * 6.0 * dens_mul - t * 0.7) * (1.0 + d * 0.5));
    n = pow(clamp(n, 0.0, 1.0), mix(1.4, 0.6, u_params[1] - 0.35));
    float hue = u_params[2];
    vec3 col = vfxHsv2rgb(vec3(hue, 0.85, 1.0));
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(n, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        n = mix(_crest, _raw, wash);
    }
    out_color = vec4(mix(vec3(0.0), col, n), 1.0);
}
