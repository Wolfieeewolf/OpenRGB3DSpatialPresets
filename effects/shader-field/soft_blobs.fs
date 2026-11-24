name: Soft Blobs
class: shader_field_soft_blobs
category: Shader Field
description: Neon metaballs (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: spread
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: spread 0 100 45 pct | Spread | How far blobs roam from centre

vec2 sbCenter(float i, float t, float spread)
{
    return mix(0.0, 0.95, spread) * vec2(sin(t * (0.6 + 0.13 * i) + i * 1.7), cos(t * (0.5 + 0.11 * i) + i * 2.3));
}

void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    float wash = clamp(u_params[4], 0.0, 1.0);
    float density = clamp(u_params[5], 0.0, 1.0);
    float tweak = clamp(u_params[6], 0.0, 1.0);
    float dens_mul = mix(0.12, 2.50, density);
    if(tweak < 0.01 || density < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (1.7 * zoom);
    float t = u_time * 0.6532;

    /* Metaballs: each blob adds r^2 / d^2; the surface sits where the sum reaches 1. */
    float field = 0.0;
    float r2 = (0.03 + 0.05 * detail) * dens_mul;
    for(int i = 0; i < 5; i++)
    {
        vec2 d = p - sbCenter(float(i), t, tweak);
        field += r2 / max(dot(d, d), 1e-4);
    }
    float rim = exp(-abs(field - 1.0) * mix(3.0, 9.0, contrast / 2.5));
    float fill = clamp((field - 1.0) * 0.45, 0.0, 0.85);
    float v = mix(clamp(rim, 0.0, 1.0), clamp(rim + fill, 0.0, 1.0), wash);

    /* Field saturated so blob cores stay one colour instead of cycling. */
    float h = fract(hue + 0.85 + 0.12 * sin(min(field, 2.0) * 1.2 + t));
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
