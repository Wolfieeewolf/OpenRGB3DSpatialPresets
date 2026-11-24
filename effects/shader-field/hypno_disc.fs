name: Hypno Disc
class: shader_field_hypno_disc
category: Shader Field
description: Concentric hypnotic pulse rings (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #FF40A0 #40E0FF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: spin
slider: wash 0 100 55 pct | Wash: | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density: | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: spin 0 100 40 pct | Spin: | Angular twist of the rings

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
    vec2 p = (uv - 0.5) * (2.0 * zoom);
    float r = length(p);
    float a = atan(p.y, p.x);
    float t = u_time * 1.55;
    float twist = a * mix(0.0, 2.5, tweak);
    float rings = dens_mul * (5.0 + 8.0 * detail);
    float v = pow(abs(sin(r * rings - t * 2.8 + twist)), mix(1.4, 3.2, contrast / 2.5));
    /* Soft edge past UV corners (√2); keep the disc filling the room box. */
    v *= 1.0 - smoothstep(1.35, 1.85, r);
    float h = fract(hue + r * 0.12 + a * 0.04 + t * 0.03);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.9, 1.0));
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }
    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
