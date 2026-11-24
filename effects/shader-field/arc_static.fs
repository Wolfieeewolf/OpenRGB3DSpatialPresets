name: Arc Static
class: shader_field_arc_static
category: Shader Field
description: Soft electric wash (Wash / Density / look sliders; vfx colour)
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

float asHash(vec2 p){return fract(sin(dot(p,vec2(12.9898,78.233)))*43758.5453);}
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
    vec2 p = (uv - 0.5) * (2.2 * (zoom * dens_mul));
    float t = u_time * 2.874;
    float bolts = 0.0;
    for(int i = 0; i < 5; i++)
    {
        float fi = float(i);
        float ang = fi * 1.2566 + t * 0.15;
        vec2 dir = vec2(cos(ang), sin(ang));
        float along = dot(p, dir);
        float across = abs(dot(p, vec2(-dir.y, dir.x)));
        float jag = asHash(vec2(floor(along * (4.0 + 6.0 * detail) + t * 3.0), fi));
        float w = exp(-across * mix(10.0, 22.0, contrast / 2.5) / (0.35 + jag));
        bolts += w * (0.35 + 0.65 * jag);
    }
    float v = clamp(bolts * 0.55, 0.0, 1.0);
    float h = fract(0.55 + hue * 0.25 + v * 0.1 + t * 0.03);
    vec3 rgb = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    rgb = mix(vec3(1.0), rgb, 0.35);
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(v, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        v = mix(_crest, _raw, wash);
    }

    out_color = vec4(mix(vec3(0.0), rgb, v), 1.0);
}
