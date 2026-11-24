name: Neon Space
class: shader_field_neon_space
category: Shader Field
description: Soft star haze (Wash / Density / look sliders; vfx colour)
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
slider: amount 0 100 50 pct | Amount | Star amount (0 = off)

float nsHash(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}
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
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (2.6 * (zoom * dens_mul) * (0.8 + detail));
    p += 0.1 * vec2(sin(u_time * 0.5225), cos(u_time * 0.4442));
    vec2 g = floor(p);
    vec2 f = fract(p) - 0.5;
    float h0 = nsHash(g);
    float tw = 0.5 + 0.5 * sin((u_time * 2.613) * (2.0 + 4.0 * h0) + h0 * 20.0);
    float star = exp(-dot(f, f) * mix(18.0, 40.0, contrast / 2.5)) * step(mix(0.97, 0.40, tweak), h0) * tw;
    float haze = 0.15 + 0.25 * sin(length(p) * 2.0 - u_time * 1.045);
    float v = clamp(star + haze * 0.35, 0.0, 1.0);
    float hh = fract(hue + h0 * 0.3 + u_time * 0.05225);
    vec3 rgb = vfxHsv2rgb(vec3(hh, 0.85, 1.0));
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
