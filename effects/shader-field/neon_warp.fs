# Name
name: Neon Warp
description: Tunnel glow.

# Effect
void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    vec2 uv = frag_coord / u_resolution;
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (1.8 * zoom);
    float t = u_time * 0.7838;

    /* Polar tunnel: depth grows as 1 / radius, rings scroll towards the viewer and twist. */
    float r = max(length(p), 0.02);
    float a = atan(p.y, p.x);
    float depth = 0.35 / r + t;
    float rings = abs(sin(depth * (2.0 + 4.0 * detail) + a * 2.0 + sin(depth * 0.7) * 1.5));
    float glow = clamp(pow(0.06 / max(rings, 0.02), mix(0.8, 1.3, contrast / 2.5)), 0.0, 2.0);
    glow *= smoothstep(0.0, 0.25, r) * clamp(1.4 - r * 0.6, 0.0, 1.0);

    float h = fract(hue + 0.9 + depth * 0.05 + 0.1 * sin(a));
    vec3 rgb = clamp(abs(mod(h * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    out_color = vec4(clamp(rgb * glow, 0.0, 1.0), 1.0);
}
