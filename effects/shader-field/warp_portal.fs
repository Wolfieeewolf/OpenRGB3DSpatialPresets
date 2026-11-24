name: Warp Portal
class: shader_field_warp_portal
category: Shader Field
description: Warped portal disc with swirling rim (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #8050FF #40E0FF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: rim
slider: wash 0 100 55 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: rim 0 100 50 pct | Rim | Portal edge thickness

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
    float r = length(p);
    float a = atan(p.y, p.x);
    float t = u_time * 1.35;
    float radius = 0.35 + 0.12 * sin(t * 0.8);
    float thick = mix(0.02, 0.18, tweak);
    float rim = exp(-pow(abs(r - radius) / max(thick, 0.01), 2.0) * 5.0);
    float swirl = 0.5 + 0.5 * sin(a * (3.0 + 4.0 * detail) * dens_mul + r * 8.0 - t * 3.0);
    float core = exp(-r * r * 5.5) * 0.35 * wash;
    float v = rim * (0.45 + 0.55 * swirl) + core;
    v = pow(clamp(v, 0.0, 1.0), mix(1.6, 0.75, contrast / 2.5));
    float h = fract(hue + a * 0.08 + t * 0.03);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }
    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
