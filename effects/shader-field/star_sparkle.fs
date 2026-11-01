name: Star sparkle
class: shader_field_star_sparkle
category: Shader Field
description: Twinkling star field on the projection plane - pairs with Particles starfield
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #D9E6FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: density
param: color_mix
slider: density 0 100 50 pct | Density | How many twinkles
slider: color_mix 0 100 40 pct | Color mix | Host Color / Rainbow / Pattern blend

void spatialMain(out vec4 out_color, in vec2 frag_coord) {
    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float t = u_time * 0.4;
    float dens = mix(8.0, 28.0, u_params[4]);
    vec2 cell = floor(uv * dens);
    vec2 f = fract(uv * dens) - 0.5;
    float h = fract(sin(dot(cell, vec2(127.1, 311.7))) * 43758.5453);
    float tw = 0.5 + 0.5 * sin(t * (2.0 + h * 5.0) + h * 6.28318);
    float d = length(f - (vec2(h, fract(h * 7.1)) - 0.5) * 0.35);
    float star = exp(-d * d * mix(80.0, 220.0, tw)) * step(0.55, h);
    star *= tw;
    vec3 col = mix(vec3(0.02, 0.03, 0.08), vec3(0.85, 0.9, 1.0), star);
    float hue = u_params[2];
    col = mix(col, col.brg, hue * 0.4);
    out_color = vec4(col, 1.0);
}
