name: Potential Rings
class: shader_field_potential_rings
category: Shader Field
description: Soft electromagnetic-style rings from a field potential (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: rings
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: rings 0 100 50 pct | Rings | Isoline / ring density (0 = off)

void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    float wash = clamp(u_params[4], 0.0, 1.0);
    float density = clamp(u_params[5], 0.0, 1.0);
    float tweak = clamp(u_params[6], 0.0, 1.0);
    float dens_mul = mix(0.12, 2.50, density);

    if(density < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    if(tweak < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (2.1 * zoom);
    float t = u_time * 1.698;
    vec2 c0 = vec2(sin(t * 0.4), cos(t * 0.35)) * 0.35;
    vec2 c1 = vec2(cos(t * 0.3), sin(t * 0.45)) * 0.4;
    float pot = 1.0 / max(length(p - c0), 0.08) - 1.0 / max(length(p - c1), 0.08);
    float rings = sin(pot * (2.5 + 5.0 * detail) * mix(0.45, 2.2, tweak) - t) * dens_mul;
    float v = pow(clamp(0.5 + 0.5 * rings, 0.0, 1.0), mix(1.9, 0.85, contrast / 2.5));
    v *= exp(-length(p) * 0.4);
    float h = fract(hue + pot * 0.05 + t * 0.02);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
