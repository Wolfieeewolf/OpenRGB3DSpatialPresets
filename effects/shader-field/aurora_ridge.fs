name: Aurora Ridge
class: shader_field_aurora_ridge
category: Shader Field
description: Horizon curtain (Wash / Density / look sliders; vfx colour)
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

float arRidge(float x, float t)
{
    return 0.5 + 0.10 * sin(x * 3.1 + t * 0.9) + 0.06 * sin(x * 7.3 - t * 1.3) + 0.03 * sin(x * 13.7 + t * 2.1);
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
    vec2 p = (uv - 0.5) * zoom + 0.5;
    float t = u_time * 0.8;

    float dy = p.y - arRidge(p.x * (0.6 + 1.4 * detail), t);
    float line = exp(-abs(dy) * mix(6.0, 18.0, contrast / 2.5));
    float rays = 0.6 + 0.4 * sin(p.x * (40.0 + 60.0 * detail) * dens_mul + 3.0 * sin(p.x * 5.0 + t));
    float curtain = (dy > 0.0) ? exp(-dy * 4.0) * rays : 0.0;
    float glow = clamp(line + 0.6 * curtain * wash, 0.0, 1.0);
    float crest = pow(clamp(line, 0.0, 1.0), 2.2);
    float lit = mix(crest, glow, wash);
    float h = fract(hue + lit * 0.08 + u_time * 0.03919);
    vec3 tint = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    vec3 base = mix(vec3(0.10, 0.80, 0.45), vec3(0.55, 0.25, 0.90), clamp(dy * 3.0 + 0.5, 0.0, 1.0));
    out_color = vec4(mix(base, tint, 0.25) * lit, 1.0);
}
