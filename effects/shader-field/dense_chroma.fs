name: Dense Chroma
class: shader_field_dense_chroma
category: Shader Field
description: Packed color plasma (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
slider: wash 0 100 60 pct | Wash: | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density: | Feature spacing / count (0 = sparse / nearly off, 100 = dense)

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
    vec2 p = (uv - 0.5) * (2.4 * zoom);
    float t = u_time * 2.482;
    float dens = (3.5 + 8.0 * detail) * dens_mul;
    float v = sin(p.x * dens + t) + sin(p.y * dens * 1.1 - t)
            + sin((p.x + p.y) * dens * 0.55 + t * 1.3)
            + cos(length(p) * dens * 0.9 - t * 0.8)
            + sin(dot(p, vec2(0.7, -0.5)) * dens + t * 0.6);
    v = pow(clamp(v * 0.18 + 0.5, 0.0, 1.0), mix(1.6, 0.7, contrast / 2.5));
    float h = fract(v * 1.4 + hue + t * 0.04);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(rgb * v, 1.0);
}
