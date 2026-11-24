name: Psyche Grid
class: shader_field_psyche_grid
category: Shader Field
description: Soft 60s lattice (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: cells
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: cells 0 100 50 pct | Cells | Grid cell density (0 = off)

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
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (2.3 * zoom);
    float t = u_time * 0.9145;
    float cells = (3.0 + 7.0 * detail) * dens_mul * mix(0.45, 2.2, tweak);
    vec2 g = abs(fract(p * cells + vec2(t * 0.2, -t * 0.15)) - 0.5);
    float grid = 1.0 - smoothstep(0.38, 0.48, max(g.x, g.y));
    float swirl = 0.5 + 0.5 * sin((p.x + p.y) * cells * 0.5 + t * 2.0);
    float v = pow(clamp(mix(swirl * 0.55, 1.0, grid), 0.0, 1.0), mix(1.5, 0.75, contrast / 2.5));
    float h = fract(hue + floor(p.x * cells) * 0.08 + floor(p.y * cells) * 0.05 + t * 0.03);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
