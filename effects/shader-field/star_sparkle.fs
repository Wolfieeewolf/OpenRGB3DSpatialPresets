name: Star Sparkle
class: shader_field_star_sparkle
category: Shader Field
description: Twinkling star field on the projection plane - pairs with Particles starfield (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #D9E6FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: count
slider: wash 0 100 60 pct | Wash: | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density: | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: count 0 100 50 pct | Count | Twinkle density

void spatialMain(out vec4 out_color, in vec2 frag_coord) {
    float wash = clamp(u_params[4], 0.0, 1.0);
    float density = clamp(u_params[5], 0.0, 1.0);
    float tweak = clamp(u_params[6], 0.0, 1.0);
    float dens_mul = mix(0.12, 2.50, density);

    if(density < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float t = u_time * 1.045;
    if(tweak < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }
    float dens = (mix(4.0, 36.0, tweak)) * dens_mul;
    vec2 cell = floor(uv * dens);
    vec2 f = fract(uv * dens) - 0.5;
    float h = fract(sin(dot(cell, vec2(127.1, 311.7))) * 43758.5453);
    float tw = 0.5 + 0.5 * sin(t * (2.0 + h * 5.0) + h * 6.28318);
    float d = length(f - (vec2(h, fract(h * 7.1)) - 0.5) * 0.35);
    float star = exp(-d * d * mix(80.0, 220.0, tw)) * step(0.55, h);
    float halo = exp(-d * d * mix(18.0, 40.0, tw)) * step(0.55, h) * 0.35;
    star = mix(star, clamp(star + halo, 0.0, 1.0), wash);
    star *= tw;
    vec3 col = mix(vec3(0.0), vec3(0.85, 0.9, 1.0), star);
    float hue = u_params[2];
    col = mix(col, col.brg, hue * 0.4);
    out_color = vec4(col, 1.0);
}
