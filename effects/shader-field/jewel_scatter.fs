name: Jewel Scatter
class: shader_field_jewel_scatter
category: Shader Field
description: Soft sparkle dots (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: amount
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: amount 0 100 55 pct | Amount | Jewel amount

float jsHash(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}
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
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (3.0 * zoom * dens_mul * (0.75 + detail));
    p += vec2(u_time * 0.3135, -u_time * 0.209);
    vec2 g = floor(p);
    vec2 f = fract(p) - 0.5;
    float h0 = jsHash(g);
    float tw = 0.45 + 0.55 * sin((u_time * 2.613) * (3.0 + 5.0 * h0) + h0 * 30.0);
    float jewel = exp(-dot(f, f) * mix(14.0, 32.0, contrast / 2.5)) * step(mix(0.95, 0.45, tweak), h0) * tw * tweak;
    float trail = exp(-abs(f.y) * 10.0) * exp(-abs(f.x) * 4.0) * step(0.78, h0) * 0.45;
    float v = mix(clamp(jewel, 0.0, 1.0), clamp(jewel + trail, 0.0, 1.0), wash);
    float hh = fract(hue + h0 * 0.55 + u_time * 0.06532);
    vec3 rgb = vfxHsv2rgb(vec3(hh, 0.85, 1.0));
    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
