name: Slow Waves
class: shader_field_slow_waves
category: Shader Field
description: Soft blue bands (Wash / Density / look sliders; vfx colour)
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
    vec2 p = (uv - 0.5) * zoom;
    float dens = (4.0 + 14.0 * detail) * dens_mul;
    float wave = sin(p.x * dens + (u_time * 2.613)) * 0.5 + sin(p.y * dens * 0.75 - u_time * 2.221) * 0.5;
    wave = pow(clamp(wave * 0.5 + 0.5, 0.0, 1.0), contrast);
    float ang = hue * 6.2831853;
    vec3 crest = vec3(0.20 + 0.35 * cos(ang), 0.45 + 0.25 * sin(ang), 0.90);
    float crest_m = pow(smoothstep(0.42, 0.78, wave), 1.6);
    float body_m = smoothstep(0.18, 0.72, wave);
    float lit = mix(crest_m, body_m, wash);
    out_color = vec4(crest * lit, 1.0);
}
