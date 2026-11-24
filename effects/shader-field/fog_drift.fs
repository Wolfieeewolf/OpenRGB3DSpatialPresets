name: Fog Drift
class: shader_field_fog_drift
category: Shader Field
description: Soft rolling haze (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
slider: wash 0 100 60 pct | Wash: | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density: | Feature spacing / count (0 = sparse / nearly off, 100 = dense)

float fdHash(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}
float fdNoise(vec2 p)
{
    vec2 i = floor(p);
    vec2 f = fract(p);
    float a = fdHash(i);
    float b = fdHash(i + vec2(1.0, 0.0));
    float c = fdHash(i + vec2(0.0, 1.0));
    float d = fdHash(i + vec2(1.0, 1.0));
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}
void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    float wash = clamp(u_params[4], 0.0, 1.0);
    float density = clamp(u_params[5], 0.0, 1.0);
    float dens_mul = mix(0.12, 2.50, density);

    if(density < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    vec2 p = (uv - 0.5) * (1.7 * (zoom * dens_mul));
    float t = u_time * 0.6532;
    float n = 0.0;
    float amp = 0.55;
    vec2 q = p * (1.5 + 2.5 * detail) + vec2(t, -t * 0.6);
    for(int i = 0; i < 4; i++)
    {
        n += fdNoise(q) * amp;
        q *= 2.05;
        amp *= 0.5;
    }
    float v = pow(clamp(n, 0.0, 1.0), mix(1.5, 0.7, contrast / 2.5));
    float h = fract(0.62 + hue * 0.2 + n * 0.1 + t * 0.02);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    rgb = mix(vec3(1.0), rgb, 0.4);
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
