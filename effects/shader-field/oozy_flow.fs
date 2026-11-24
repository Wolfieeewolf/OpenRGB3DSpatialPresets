name: Oozy Flow
class: shader_field_oozy_flow
category: Shader Field
description: Soft molten wash (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)

void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    float wash = clamp(u_params[4], 0.0, 1.0);
    float density = clamp(u_params[5], 0.0, 1.0);
    float dens_mul = mix(0.12, 2.50, density);

    if(density < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (1.9 * zoom);
    float t = u_time * 1.176;
    float dens = (2.0 + 4.5 * detail) * dens_mul;
    float v = 0.0;
    for(int i = 0; i < 5; i++)
    {
        float fi = float(i);
        vec2 q = p + vec2(sin(t * 0.7 + fi * 1.3), cos(t * 0.55 + fi * 0.9)) * (0.15 + 0.05 * fi);
        v += sin(q.x * dens + t + fi) * cos(q.y * dens * 0.85 - t * 0.8 + fi);
    }
    v = pow(clamp(v * 0.28 + 0.5, 0.0, 1.0), mix(1.7, 0.75, contrast / 2.5));
    float h = fract(hue + v * 0.18 + t * 0.02);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    rgb = mix(vec3(1.0), rgb, 0.55);
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
