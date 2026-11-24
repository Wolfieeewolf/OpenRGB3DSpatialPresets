name: Ripple Ember
class: shader_field_ember_field
category: Shader Field
description: Expanding fire rings with warm ember falloff (Wash / Density / look sliders; vfx colour)
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
slider: rings 0 100 50 pct | Rings | Fire ring count (0 = off)

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
    vec2 p = (uv - 0.5) * (2.0 * zoom);
    float r = length(p);
    float a = atan(p.y, p.x);
    float rings = (2.0 + 5.0 * detail) * dens_mul * mix(0.35, 2.2, tweak);
    float ring = pow(1.0 - abs(sin(r * rings * 3.14159 - u_time * 4.18)), mix(1.2, 4.0, contrast * 0.35));
    float swirl = 0.5 + 0.5 * sin(a * (3.0 + 4.0 * detail) + u_time * 2.351 + r * 4.0);
    float ember = pow(clamp(ring * (0.35 + 0.65 * swirl) * (1.15 - r), 0.0, 1.0), contrast);
    float h = fract(hue + ember * 0.12 + u_time * 0.05225);
    vec3 hot = vec3(1.0, 0.28 + 0.25 * cos(h * 6.2831853), 0.04);
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(ember, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        ember = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), hot, ember), 1.0);
}
