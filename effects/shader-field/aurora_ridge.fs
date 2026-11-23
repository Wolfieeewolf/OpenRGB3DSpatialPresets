# Name
name: Aurora Ridge
description: Horizon curtain.

# Effect
float arRidge(float x, float t)
{
    return 0.5 + 0.10 * sin(x * 3.1 + t * 0.9) + 0.06 * sin(x * 7.3 - t * 1.3) + 0.03 * sin(x * 13.7 + t * 2.1);
}

void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    vec2 uv = frag_coord / u_resolution;
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    vec2 p = (uv - 0.5) * zoom + 0.5;
    float t = u_time * 0.8;

    /* A glowing ridge line, with curtains of light rising above it. */
    float dy = p.y - arRidge(p.x * (0.6 + 1.4 * detail), t);
    float line = exp(-abs(dy) * mix(6.0, 18.0, contrast / 2.5));
    float rays = 0.6 + 0.4 * sin(p.x * (40.0 + 60.0 * detail) + 3.0 * sin(p.x * 5.0 + t));
    float curtain = (dy > 0.0) ? exp(-dy * 4.0) * rays : 0.0;
    float glow = clamp(line + 0.6 * curtain, 0.0, 1.0);

    vec3 base = mix(vec3(0.10, 0.80, 0.45), vec3(0.55, 0.25, 0.90), clamp(dy * 3.0 + 0.5, 0.0, 1.0));
    float h = fract(hue + glow * 0.08 + u_time * 0.03919);
    vec3 tint = clamp(abs(mod(h * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    out_color = vec4(mix(base, tint, 0.25) * (0.06 + 0.94 * glow), 1.0);
}
