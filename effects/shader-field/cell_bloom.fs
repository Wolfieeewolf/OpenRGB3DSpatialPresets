name: Cell Bloom
class: shader_field_cell_bloom
category: Shader Field
description: Organic cells (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: scale
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: scale 0 100 50 pct | Scale | Cell size (0 = off)

float cbHash(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}
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
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (3.0 * zoom * dens_mul * mix(0.45, 2.2, tweak) * (0.7 + detail));
    p += vec2(u_time * 0.209, -u_time * 0.1306);
    vec2 g = floor(p);
    vec2 f = fract(p);
    float md = 8.0;
    float mid = 8.0;
    for(int j = -1; j <= 1; j++)
    for(int i = -1; i <= 1; i++)
    {
        vec2 o = vec2(float(i), float(j));
        vec2 r = o + vec2(cbHash(g + o), cbHash(g + o + 19.0)) - f;
        float d = dot(r, r);
        if(d < md){mid = md; md = d;}
        else if(d < mid) mid = d;
    }
    float edge = clamp(sqrt(mid) - sqrt(md), 0.0, 1.0);
    float v = pow(clamp(1.0 - md * 1.8, 0.0, 1.0), mix(1.6, 0.7, contrast / 2.5));
    v *= 0.55 + 0.45 * smoothstep(0.02, 0.12, edge);
    float h = fract(hue + md * 0.35 + u_time * 0.05225);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
