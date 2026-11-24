name: Soft Aurora
class: shader_field_soft_aurora
category: Shader Field
description: Curtain bands (Wash / Density / look sliders; vfx colour)
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
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (1.6 * (zoom * dens_mul));
    float t = u_time * 0.9145;
    float v = 0.0;
    float band0 = 0.55 + 0.45 * sin(p.x * (1.2) + t * 0.6 + sin(p.y * 2.2 + t * 0.4));
    float curtain0 = exp(-abs(p.y + 0.15 * sin(p.x * 1.5 + t) + 0.36) * (3.0 + detail * 2.0));
    v += band0 * curtain0 * 0.45;
    float band1 = 0.55 + 0.45 * sin(p.x * (1.2 + 0.35 * detail) + t * 0.75 + sin(p.y * 2.2 + t * 0.4 + 1.0));
    float curtain1 = exp(-abs(p.y + 0.15 * sin(p.x * 1.5 + t + 1.0) + 0.18) * (3.0 + detail * 2.0));
    v += band1 * curtain1 * 0.33;
    float band2 = 0.55 + 0.45 * sin(p.x * (1.2 + 0.70 * detail) + t * 0.90 + sin(p.y * 2.2 + t * 0.4 + 2.0));
    float curtain2 = exp(-abs(p.y + 0.15 * sin(p.x * 1.5 + t + 2.0)) * (3.0 + detail * 2.0));
    v += band2 * curtain2 * 0.26;
    float band3 = 0.55 + 0.45 * sin(p.x * (1.2 + 1.05 * detail) + t * 1.05 + sin(p.y * 2.2 + t * 0.4 + 3.0));
    float curtain3 = exp(-abs(p.y + 0.15 * sin(p.x * 1.5 + t + 3.0) - 0.18) * (3.0 + detail * 2.0));
    v += band3 * curtain3 * 0.21;
    float band4 = 0.55 + 0.45 * sin(p.x * (1.2 + 1.40 * detail) + t * 1.20 + sin(p.y * 2.2 + t * 0.4 + 4.0));
    float curtain4 = exp(-abs(p.y + 0.15 * sin(p.x * 1.5 + t + 4.0) - 0.36) * (3.0 + detail * 2.0));
    v += band4 * curtain4 * 0.18;
    v = pow(clamp(v, 0.0, 1.0), mix(1.4, 0.7, contrast / 2.5));
    float h = fract(0.55 + hue + v * 0.12 + t * 0.02);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    rgb = rgb * rgb * (3.0 - 2.0 * rgb);
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(rgb * v, 1.0);
}
