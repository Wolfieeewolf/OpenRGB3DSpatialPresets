name: Vortex Swirl
class: shader_field_vortex_swirl
category: Shader Field
description: Spiral field (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: arms
slider: wash 0 100 60 pct | Wash: | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density: | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: arms 0 100 50 pct | Arms | Spiral arm emphasis

void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    float wash = clamp(u_params[4], 0.0, 1.0);
    float density = clamp(u_params[5], 0.0, 1.0);
    float tweak = clamp(u_params[6], 0.0, 1.0);
    float dens_mul = mix(0.12, 2.50, density);

    if(density < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    vec2 p = (uv - 0.5) * (2.0 * zoom);
    float r = length(p);
    float a = atan(p.y, p.x);
    float arms = 2.0 + floor(mix(2.0, 6.0, tweak) * detail + 0.5);
    float spiral = sin(a * arms + r * (6.0 + 8.0 * detail) - u_time * 4.18);
    float v = pow(clamp(0.5 + 0.5 * spiral, 0.0, 1.0), mix(2.0, 0.85, contrast / 2.5));
    v *= exp(-r * 0.32);
    float rings = (0.5 + 0.5 * sin(r * (10.0 + 12.0 * detail) - u_time * 5.225)) * dens_mul;
    v = max(v, rings * exp(-r * 0.55) * 0.55);
    float h = fract(hue + a * 0.08 + r * 0.12 + u_time * 0.07838);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
