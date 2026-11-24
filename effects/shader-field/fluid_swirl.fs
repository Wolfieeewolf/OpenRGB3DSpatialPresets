name: Fluid Swirl
class: shader_field_fluid_swirl
category: Shader Field
description: Soft current (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
slider: wash 0 100 60 pct | Wash: | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density: | Feature spacing / count (0 = sparse / nearly off, 100 = dense)

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
    /* Ideas only (swirl / domain-warp shaders): nested current + soft arms. Clean-room. */
    vec2 p = (uv - 0.5) * (2.0 * (zoom * dens_mul));
    float t = u_time * 1.306;
    vec2 q = p;
    q.x += 0.42 * sin(q.y * (2.0 + 3.0 * detail) + t);
    q.y += 0.42 * cos(q.x * (2.0 + 3.0 * detail) - t * 0.9);
    q.x += 0.18 * sin(q.y * (5.0 + 4.0 * detail) - t * 1.35);
    q.y += 0.18 * cos(q.x * (4.5 + 3.5 * detail) + t * 1.15);
    float rad = length(q);
    float ang = atan(q.y, q.x);
    float arms = 0.55 + 0.45 * sin(ang * (3.0 + 2.0 * detail) + rad * 2.4 - t * 0.85);
    float v = sin(q.x * (3.0 + 5.0 * detail) + t) * cos(q.y * (3.0 + 4.0 * detail) - t);
    v = v * 0.72 + 0.28 * sin(rad * (4.0 + 3.0 * detail) - t * 1.1) * arms;
    v = pow(clamp(v * 0.5 + 0.5, 0.0, 1.0), mix(1.7, 0.8, contrast / 2.5));
    /* Soft edge past UV corners (√2); keep currents filling the room box. */
    v *= 1.0 - smoothstep(1.35, 1.85, rad);
    float h = fract(hue + rad * 0.08 + ang * 0.02 + t * 0.02);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    rgb = mix(vec3(1.0), rgb, 0.65);
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
