name: Zigzag Bands
class: shader_field_zigzag_bands
category: Shader Field
description: Diagonal zigzag bands marching across the plane (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #FF9060 #60D0FF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: zig
slider: wash 0 100 55 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: zig 0 100 50 pct | Zigzag | How sharp the zig folds are

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
    float t = u_time * 1.1;
    float zig = mix(0.05, 0.45, tweak) * sin(p.y * (6.0 + 8.0 * detail) + t);
    float band = fract((p.x + p.y * 0.35 + zig) * dens_mul * (2.5 + 2.0 * detail) + t * 0.25);
    float v = 1.0 - smoothstep(0.0, mix(0.12, 0.4, wash), abs(band - 0.5) * 2.0);
    v = pow(clamp(v, 0.0, 1.0), mix(1.6, 0.75, contrast / 2.5));
    float h = fract(hue + p.x * 0.08 + t * 0.02);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.82, 1.0));
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }
    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
