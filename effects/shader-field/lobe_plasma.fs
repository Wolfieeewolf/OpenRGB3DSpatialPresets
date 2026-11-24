name: Lobe Plasma
class: shader_field_lobe_plasma
category: Shader Field
description: Multi-center swirl (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)

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
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (1.35 * (zoom * dens_mul));
    float t = (u_time * 2.613);
    float n_its = 3.0 + floor(2.0 * detail + 0.5);
    float n_d_prod = 1.0;
    float i0 = 0.0;
    n_d_prod += sin(length(p - vec2(sin(6.2831 * i0), cos(6.2831 * i0)) * sin(t * 0.2 + i0)) * (8.0 + 4.0 * detail));
    i0 = 1.0 / n_its;
    n_d_prod += sin(length(p - vec2(sin(6.2831 * i0), cos(6.2831 * i0)) * sin(t * 0.2 + i0)) * (8.0 + 4.0 * detail));
    i0 = 2.0 / n_its;
    n_d_prod += sin(length(p - vec2(sin(6.2831 * i0), cos(6.2831 * i0)) * sin(t * 0.2 + i0)) * (8.0 + 4.0 * detail));
    i0 = 3.0 / n_its;
    n_d_prod += sin(length(p - vec2(sin(6.2831 * i0), cos(6.2831 * i0)) * sin(t * 0.2 + i0)) * (8.0 + 4.0 * detail));
    i0 = 4.0 / n_its;
    n_d_prod += sin(length(p - vec2(sin(6.2831 * i0), cos(6.2831 * i0)) * sin(t * 0.2 + i0)) * (8.0 + 4.0 * detail));
    float n1 = 0.5 + 0.5 * sin(n_d_prod * 4.0 + t * 2.2);
    float n2 = 0.5 + 0.5 * sin(n_d_prod * 2.0 + t * 2.2);
    float n3 = 0.5 + 0.5 * sin(n_d_prod * 1.0 + t * 2.2);
    float v = pow(clamp((n1 + n2 + n3) / 3.0, 0.0, 1.0), contrast);
    float h = fract(hue + n1 * 0.2 + t * 0.03);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    rgb = rgb * rgb * (3.0 - 2.0 * rgb);
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }
    out_color = vec4(clamp(rgb * v, 0.0, 1.0), 1.0);
}
