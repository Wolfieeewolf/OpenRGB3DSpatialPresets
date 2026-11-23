# Name
name: Soft Blobs
description: Neon metaballs.

# Effect
vec2 sbCenter(float i, float t)
{
    return 0.7 * vec2(sin(t * (0.6 + 0.13 * i) + i * 1.7), cos(t * (0.5 + 0.11 * i) + i * 2.3));
}

void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    vec2 uv = frag_coord / u_resolution;
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (1.7 * zoom);
    float t = u_time * 0.6532;

    /* Metaballs: each blob adds r^2 / d^2; the surface sits where the sum reaches 1. */
    float field = 0.0;
    float r2 = 0.03 + 0.05 * detail;
    for(int i = 0; i < 5; i++)
    {
        vec2 d = p - sbCenter(float(i), t);
        field += r2 / max(dot(d, d), 1e-4);
    }
    float rim = exp(-abs(field - 1.0) * mix(3.0, 9.0, contrast / 2.5));
    float fill = clamp((field - 1.0) * 0.25, 0.0, 0.6);
    float v = clamp(rim + fill, 0.0, 1.0);

    /* Field saturated so blob cores stay one colour instead of cycling. */
    float h = fract(hue + 0.85 + 0.12 * sin(min(field, 2.0) * 1.2 + t));
    vec3 rgb = clamp(abs(mod(h * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    out_color = vec4(mix(vec3(0.02, 0.0, 0.04), rgb, v), 1.0);
}
