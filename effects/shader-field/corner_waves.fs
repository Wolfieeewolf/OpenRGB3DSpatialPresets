name: Corner Waves
class: shader_field_corner_waves
category: Shader Field
description: Four-corner rings (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: length
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: length 0 100 45 pct | Wavelength | Ripple wavelength (0 = off)

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
    vec2 p = (uv - 0.5) * (zoom * dens_mul) + 0.5;
    float vel = 1.6 + 2.2 * detail;
    float len = mix(18.0, 8.0, detail) / mix(0.45, 2.2, tweak);
    float t = (u_time * 2.613) * vel;
    float aspect = u_resolution.x / max(u_resolution.y, 1.0);
    vec2 q = (p - 0.5) * vec2(aspect, 1.0) + 0.5;
    float d0 = length((q - vec2(0.0, 0.0)) * u_resolution.xy / max(u_resolution.y, 1.0));
    float d1 = length((q - vec2(1.0, 0.0)) * u_resolution.xy / max(u_resolution.y, 1.0));
    float d2 = length((q - vec2(0.0, 1.0)) * u_resolution.xy / max(u_resolution.y, 1.0));
    float d3 = length((q - vec2(1.0, 1.0)) * u_resolution.xy / max(u_resolution.y, 1.0));
    float w = sin(d0 / len - t) + sin(d1 / len - t) + sin(d2 / len - t) + sin(d3 / len - t);
    float v = pow(clamp(0.5 + 0.25 * w, 0.0, 1.0), contrast);
    float h = fract(hue + 0.08 * w + u_time * 0.05225);
    vec3 crest = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    crest = crest * crest * (3.0 - 2.0 * crest);
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), crest, v), 1.0);
}
