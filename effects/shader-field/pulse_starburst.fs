name: Pulse Starburst
class: shader_field_pulse_starburst
category: Shader Field
description: Radial starburst rays with a breathing pulse (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #FFCC66 #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: rays
slider: wash 0 100 55 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: rays 0 100 50 pct | Rays | Number of starburst spokes

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
    float r = length(p);
    float a = atan(p.y, p.x);
    float t = u_time * 1.25;
    float n = 4.0 + floor(mix(4.0, 16.0, tweak) * dens_mul * 0.5);
    float spokes = pow(abs(sin(a * n * 0.5 + t * 0.4)), mix(1.2, 4.0, contrast / 2.5));
    float pulse = 0.55 + 0.45 * sin(t * 2.2 + r * (6.0 + 4.0 * detail));
    float v = spokes * exp(-r * 1.05) * pulse;
    float h = fract(hue + 0.08 + a * 0.05 + t * 0.02);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.75, 1.0));
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }
    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
