name: Kaleido Fold
class: shader_field_kaleido_fold
category: Shader Field
description: Mirrored kaleidoscope wedges (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #FF66CC #40E0FF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: folds
slider: wash 0 100 60 pct | Wash: | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density: | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: folds 0 100 50 pct | Folds | Mirror slice count

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
    vec2 p = (uv - 0.5) * (2.0 * zoom * dens_mul);
    float t = u_time * 0.95;
    float n = 3.0 + floor(mix(2.0, 8.0, tweak));
    float a = atan(p.y, p.x);
    float r = length(p);
    float seg = 6.2831853 / n;
    a = abs(mod(a, seg) - seg * 0.5);
    vec2 q = vec2(cos(a), sin(a)) * r;
    q.x += 0.25 * sin(q.y * (3.0 + 4.0 * detail) + t);
    float v = 0.5 + 0.5 * sin(q.x * (5.0 + 6.0 * detail) + t) * cos(q.y * (4.0 + 5.0 * detail) - t);
    v = pow(clamp(v, 0.0, 1.0), mix(1.8, 0.8, contrast / 2.5));
    /* Soft edge past UV corners (√2); keep folds filling the room box. */
    v *= 1.0 - smoothstep(1.35, 1.85, r);
    float h = fract(hue + a * 0.15 + r * 0.08 + t * 0.03);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.88, 1.0));
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }
    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
