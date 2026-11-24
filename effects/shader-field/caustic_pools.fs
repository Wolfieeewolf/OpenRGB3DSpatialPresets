name: Caustic Pools
class: shader_field_caustic_pools
category: Shader Field
description: Slow underwater light pools (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #20C8FF #E8FFF8
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: calm
slider: wash 0 100 70 pct | Wash: | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density: | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: calm 0 100 45 pct | Calm | Softer vs sharper caustic edges

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
    vec2 p = (uv - 0.5) * (2.0 * zoom * dens_mul);
    float t = u_time * mix(0.55, 1.35, 1.0 - tweak);
    vec2 q = p;
    q.x += 0.35 * sin(q.y * (2.5 + 3.0 * detail) + t);
    q.y += 0.35 * cos(q.x * (2.2 + 2.8 * detail) - t * 0.85);
    float a = sin(q.x * (3.5 + 4.0 * detail) + t);
    float b = cos(q.y * (3.2 + 3.5 * detail) - t * 1.1);
    float c = sin((q.x + q.y) * (2.4 + 2.0 * detail) + t * 0.7);
    float v = pow(clamp(a * b * 0.55 + c * 0.45, 0.0, 1.0), mix(1.6, 0.75, contrast / 2.5));
    float h = fract(hue + 0.48 + length(q) * 0.04 + t * 0.015);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.55, 1.0));
    rgb = mix(vec3(0.75, 0.95, 1.0), rgb, 0.7);
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }
    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
