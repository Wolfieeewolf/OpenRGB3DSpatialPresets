name: Checker Drift
class: shader_field_spectrum_glow
category: Shader Field
description: Moving checker lattice with drifting glow (Wash / Density / look sliders; vfx colour)
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
slider: cells 0 100 50 pct | Cells | Checker density (0 = off)

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
    float cells = (3.0 + 10.0 * detail) * dens_mul;
    vec2 p = (uv - 0.5) * zoom * cells + vec2(u_time * 0.9145, -u_time * 0.5748);
    vec2 g = abs(fract(p * mix(0.45, 2.4, tweak)) - 0.5);
    float checker = step(0.5, mod(floor(p.x) + floor(p.y), 2.0));
    float grid = 1.0 - smoothstep(0.42, 0.48, max(g.x, g.y));
    float v = pow(clamp(mix(checker * 0.55, 1.0, grid), 0.0, 1.0), contrast);
    float h = fract(hue + 0.08 * floor(p.x) + 0.05 * floor(p.y));
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
