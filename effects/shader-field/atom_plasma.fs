name: Atom Plasma
class: shader_field_atom_plasma
category: Shader Field
description: Compact plasma lobes that stay readable on sparse LED grids (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: lobes
slider: wash 0 100 60 pct | Wash: | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density: | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: lobes 0 100 55 pct | Lobes | Lobe separation (0 = off)

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
    vec2 p = (uv - 0.5) * (2.0 * zoom);
    float t = u_time * 2.221;
    float dens = (1.4 + 2.8 * detail) * dens_mul;
    float v = 0.0;
    for(int i = 0; i < 4; i++)
    {
        float fi = float(i);
        vec2 c = vec2(cos(t * 0.4 + fi * 1.7), sin(t * 0.35 + fi * 2.1)) * ((0.25 + 0.12 * fi) * mix(0.35, 1.85, tweak));
        float d = length(p - c);
        v += sin(d * dens * 6.2831853 - t * (1.2 + 0.2 * fi)) * exp(-d * (2.2 + detail));
    }
    v = pow(clamp(v * 0.55 + 0.5, 0.0, 1.0), mix(1.8, 0.75, contrast / 2.5));
    float h = fract(v * 0.7 + hue + t * 0.03);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    rgb = rgb * rgb * (3.0 - 2.0 * rgb);
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(rgb * v, 1.0);
}
