name: Melt Petals
class: shader_field_melt_petals
category: Shader Field
description: Soft floral wash (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: petals
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: petals 0 100 45 pct | Petals | Petal count (0 = off)

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
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (1.8 * (zoom * dens_mul));
    float t = u_time * 0.9145;
    float r = length(p);
    float a = atan(p.y, p.x);
    float petals = mix(3.0, 12.0, tweak) + floor(3.0 * detail + 0.5);
    float flower = 0.5 + 0.5 * cos(a * petals + sin(r * 4.0 - t) * 1.2);
    float melt = sin(r * (5.0 + 6.0 * detail) - t * 1.4 + flower * 2.0);
    float v = pow(clamp(0.5 + 0.45 * melt * flower, 0.0, 1.0), mix(1.8, 0.8, contrast / 2.5));
    v *= exp(-r * 0.55);
    float h = fract(hue + flower * 0.12 + t * 0.02);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    rgb = mix(vec3(1.0), rgb, 0.65);
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
