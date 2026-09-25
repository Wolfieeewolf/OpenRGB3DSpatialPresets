# Name
name: Checker Drift — moving lattice

# Effect
void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    vec2 uv = frag_coord / u_resolution;
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    float cells = 3.0 + 10.0 * detail;
    vec2 p = (uv - 0.5) * zoom * cells + vec2(u_time * 0.35, -u_time * 0.22);
    vec2 g = abs(fract(p) - 0.5);
    float checker = step(0.5, mod(floor(p.x) + floor(p.y), 2.0));
    float grid = 1.0 - smoothstep(0.42, 0.48, max(g.x, g.y));
    float v = pow(clamp(mix(checker * 0.55, 1.0, grid), 0.0, 1.0), contrast);
    float h = fract(hue + 0.08 * floor(p.x) + 0.05 * floor(p.y));
    vec3 rgb = clamp(abs(mod(h * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    out_color = vec4(mix(vec3(0.02, 0.02, 0.05), rgb, v), 1.0);
}
