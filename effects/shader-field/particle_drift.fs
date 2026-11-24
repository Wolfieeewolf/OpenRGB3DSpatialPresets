name: Particle Drift
class: shader_field_particle_drift
category: Shader Field
description: Dense drifting particle field (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #D0E8FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: size_pts
slider: wash 0 100 50 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 55 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: size_pts 0 100 40 pct | Point size | Soft halo around each particle

void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    float wash = clamp(u_params[4], 0.0, 1.0);
    float density = clamp(u_params[5], 0.0, 1.0);
    float tweak = clamp(u_params[6], 0.0, 1.0);
    float dens_mul = mix(0.12, 2.50, density);

    if(density < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    float t = u_time * 0.85;
    float dens = (mix(8.0, 42.0, detail)) * dens_mul / zoom;
    vec2 drift = vec2(t * 0.08, t * 0.05);
    vec2 cell = floor((uv + drift) * dens);
    vec2 f = fract((uv + drift) * dens) - 0.5;
    float h = fract(sin(dot(cell, vec2(127.1, 311.7))) * 43758.5453);
    vec2 off = (vec2(h, fract(h * 9.3)) - 0.5) * 0.55;
    float d = length(f - off);
    float soft = mix(90.0, 28.0, tweak);
    float star = exp(-d * d * soft * mix(1.0, 1.6, contrast / 2.5)) * step(0.35, h);
    float halo = exp(-d * d * soft * 0.28) * step(0.35, h) * tweak;
    float v = mix(star, clamp(star + halo, 0.0, 1.0), wash);
    float tw = 0.55 + 0.45 * sin(t * (2.0 + h * 4.0) + h * 6.28);
    v *= tw;
    float hh = fract(hue + h * 0.2 + t * 0.02);
    vec3 rgb = mix(vec3(0.85, 0.9, 1.0), vfxHsv2rgb(vec3(hh, 0.45, 1.0)), 0.55);
    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
