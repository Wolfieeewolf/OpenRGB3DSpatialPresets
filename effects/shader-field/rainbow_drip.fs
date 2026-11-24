name: Rainbow Drip
class: shader_field_rainbow_drip
category: Shader Field
description: Falling color streaks (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: columns
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: columns 0 100 50 pct | Columns | Drip column count (0 = off)

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
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (2.0 * zoom);
    float t = u_time * 2.09;
    float cols = (4.0 + 10.0 * detail) * dens_mul * mix(0.35, 2.4, tweak);
    float x = p.x * cols;
    float id = floor(x);
    float f = fract(x) - 0.5;
    float drip = fract(p.y * (1.2 + 0.8 * detail) - t * (0.6 + 0.5 * fract(sin(id * 12.9898) * 43758.5)) + id * 0.17);
    float streak = exp(-abs(f) * mix(8.0, 18.0, contrast / 2.5)) * pow(1.0 - abs(drip * 2.0 - 1.0), 2.2);
    float v = clamp(streak, 0.0, 1.0);
    float h = fract(hue + id * 0.07 + drip * 0.1);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
