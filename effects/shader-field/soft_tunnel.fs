name: Soft Tunnel
class: shader_field_soft_tunnel
category: Shader Field
description: Soft dive tunnel with traveling rings (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: depth
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: depth 0 100 55 pct | Depth | How far the tunnel recedes

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
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (2.0 * zoom);
    float r = length(p);
    float a = atan(p.y, p.x);
    float t = u_time * 1.15;
    float z = mix(0.35, 1.45, tweak);
    float rings = dens_mul * (4.0 + 6.0 * detail);
    float tunnel = 1.0 / max(r * z + 0.18, 0.08);
    float stripes = 0.5 + 0.5 * sin(tunnel * rings - t * 3.2 + a * (2.0 + 2.0 * detail));
    float v = pow(clamp(stripes * exp(-r * 0.55), 0.0, 1.0), mix(2.0, 0.85, contrast / 2.5));
    float h = fract(hue + tunnel * 0.04 + a * 0.03 + t * 0.02);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.82, 1.0));
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }
    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
