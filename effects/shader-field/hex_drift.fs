name: Hex Drift
class: shader_field_hex_drift
category: Shader Field
description: Lattice wash (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: cells
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: cells 0 100 50 pct | Cells | Hex cell density (0 = off)

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
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (3.2 * zoom * dens_mul * mix(0.45, 2.2, tweak) * (0.7 + detail));
    float t = u_time * 1.045;
    p += vec2(sin(t * 0.7), cos(t * 0.55)) * 0.15;
    vec2 r = vec2(1.0, 0.8660254);
    vec2 h1 = r * 0.5;
    vec2 a = mod(p, r) - h1;
    vec2 b = mod(p - h1, r) - h1;
    vec2 gv = (dot(a, a) < dot(b, b)) ? a : b;
    float d = max(abs(gv.x) * 0.866 + abs(gv.y) * 0.5, abs(gv.y));
    float edge = 1.0 - smoothstep(0.28, 0.42, d);
    float pulse = 0.5 + 0.5 * sin(t * 2.0 + (p.x + p.y) * 1.5);
    float v = pow(clamp(edge * (0.55 + 0.45 * pulse), 0.0, 1.0), mix(1.6, 0.85, contrast / 2.5));
    float hh = fract(hue + d * 0.35 + t * 0.05);
    vec3 rgb = vfxHsv2rgb(vec3(hh, 0.85, 1.0));
    rgb = rgb * rgb * (3.0 - 2.0 * rgb);
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(rgb * v, 1.0);
}
