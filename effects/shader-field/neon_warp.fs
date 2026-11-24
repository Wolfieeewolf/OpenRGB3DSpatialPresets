name: Neon Warp
class: shader_field_neon_warp
category: Shader Field
description: Tunnel glow (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: rings
slider: wash 0 100 60 pct | Wash: | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density: | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: rings 0 100 55 pct | Rings | Tunnel ring density

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
    vec2 p = (uv - 0.5) * (1.8 * zoom);
    float t = u_time * 0.7838;

    float r = max(length(p), 0.02);
    float a = atan(p.y, p.x);
    float depth = 0.35 / r + t;
    float rings = abs(sin(depth * (mix(1.5, 5.0, tweak) + 3.0 * detail) * dens_mul + a * 2.0 + sin(depth * 0.7) * 1.5));
    float glow = clamp(pow(0.06 / max(rings, 0.02), mix(0.8, 1.3, contrast / 2.5)), 0.0, 2.0);
    /* Inner hole + soft fade past UV corners (√2 ≈ 1.41). */
    glow *= smoothstep(0.0, 0.25, r) * clamp(1.85 - r * 0.45, 0.0, 1.0);
    float fog = clamp(0.12 / max(r, 0.05), 0.0, 0.55);
    float crest = pow(clamp(glow * 0.55, 0.0, 1.0), 2.2);
    float lit = mix(crest, clamp(glow * 0.55 + fog, 0.0, 1.0), wash);
    if(tweak < 0.01) lit = 0.0;
    float h = fract(hue + 0.9 + depth * 0.05 + 0.1 * sin(a));
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    out_color = vec4(clamp(rgb * lit * 1.6, 0.0, 1.0), 1.0);
}
