name: Lightning Veins
class: shader_field_lightning_veins
category: Shader Field
description: Sparse jagged bolts that flash across the plane (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #AACCFF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: branches
slider: wash 0 100 45 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: branches 0 100 40 pct | Branches | Side-fork amount

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
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (2.0 * zoom);
    float t = u_time * 2.1;
    float flash = pow(0.5 + 0.5 * sin(t * 3.7 + floor(t * 1.3) * 2.1), 8.0);
    float x = p.x * dens_mul;
    float y = p.y;
    float jag = 0.0;
    float amp = 0.18 + 0.22 * tweak;
    for(int i = 0; i < 4; i++)
    {
        float f = pow(2.0, float(i));
        jag += sin(y * (3.0 + 4.0 * detail) * f + t * (1.2 + 0.4 * float(i)) + float(i) * 1.7) * (amp / f);
    }
    float bolt = exp(-abs(x - jag) * mix(14.0, 36.0, contrast / 2.5));
    float fork = exp(-abs(x - jag * 0.55 - 0.12 * sin(y * 8.0 + t)) * 22.0) * tweak;
    float v = (bolt + fork * 0.65) * (0.35 + 0.65 * flash);
    float h = fract(hue + 0.55 + flash * 0.05);
    vec3 rgb = mix(vec3(0.55, 0.75, 1.0), vfxHsv2rgb(vec3(h, 0.35, 1.0)), 0.45);
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }
    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
