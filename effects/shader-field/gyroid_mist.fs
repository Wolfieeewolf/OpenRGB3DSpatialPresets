name: Gyroid Mist
class: shader_field_gyroid_mist
category: Shader Field
description: Soft SDF wash (Wash / Density / look sliders; vfx colour)
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
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (2.4 * (zoom * dens_mul));
    float t = u_time * 1.045;
    float s = 2.2 + 4.0 * detail;
    float g = sin(p.x * s + t) * cos(p.y * s - t * 0.8)
            + sin((p.x + p.y) * s * 0.7 - t)
            + cos(length(p) * s * 0.9 + t * 1.1);
    float v = pow(clamp(0.5 + 0.5 * g * 0.55, 0.0, 1.0), mix(1.7, 0.8, contrast / 2.5));
    float mist = exp(-length(p) * 0.35) * v;
    float h = fract(hue + v * 0.15 + t * 0.02);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    rgb = rgb * rgb * (3.0 - 2.0 * rgb);
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(mist, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        mist = mix(_crest, _raw, wash);
    }
    out_color = vec4(mix(vec3(0.0), rgb, mist), 1.0);
}
