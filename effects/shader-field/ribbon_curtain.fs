name: Ribbon Curtain
class: shader_field_ribbon_curtain
category: Shader Field
description: Vertical flowing ribbons like a light curtain (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #60FFD0 #FF80E0
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: strands
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: strands 0 100 50 pct | Strands | Ribbon count

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
    float t = u_time * 1.05;
    float n = mix(3.0, 14.0, tweak) * dens_mul;
    float x = p.x * n;
    float wave = 0.15 * sin(p.y * (4.0 + 5.0 * detail) + t) + 0.08 * sin(p.y * 9.0 - t * 1.3);
    float band = abs(fract(x + wave) - 0.5) * 2.0;
    float v = 1.0 - smoothstep(0.0, mix(0.18, 0.55, wash), band);
    v = pow(clamp(v, 0.0, 1.0), mix(1.5, 0.7, contrast / 2.5));
    float h = fract(hue + p.x * 0.12 + p.y * 0.05 + t * 0.025);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.8, 1.0));
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }
    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
