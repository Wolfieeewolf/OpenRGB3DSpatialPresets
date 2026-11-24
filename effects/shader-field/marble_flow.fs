name: Marble Flow
class: shader_field_marble_flow
category: Shader Field
description: Soft marble veins with slow domain warp (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #70A0FF #FFE0F0
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: veins
slider: wash 0 100 65 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: veins 0 100 55 pct | Veins | Vein contrast / count

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
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (2.0 * zoom * dens_mul);
    float t = u_time * 0.55;
    vec2 q = p;
    q += 0.45 * vec2(sin(q.y * 2.2 + t), cos(q.x * 2.0 - t * 0.9));
    q += 0.22 * vec2(sin(q.y * (4.0 + 3.0 * detail) - t * 1.2), cos(q.x * (3.5 + 2.5 * detail) + t));
    float n = 0.0;
    float amp = 1.0;
    for(int i = 0; i < 4; i++)
    {
        n += amp * sin(q.x * (2.5 + float(i)) + q.y * (1.8 + 0.4 * float(i)) + t * (0.4 + 0.1 * float(i)));
        q *= 1.9;
        amp *= 0.5;
    }
    float v = abs(sin(n * mix(1.2, 3.2, tweak)));
    v = pow(clamp(v, 0.0, 1.0), mix(1.7, 0.75, contrast / 2.5));
    float h = fract(hue + n * 0.03 + t * 0.01);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.55, 1.0));
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }
    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
