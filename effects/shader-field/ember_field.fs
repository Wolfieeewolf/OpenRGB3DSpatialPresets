# Name
name: Ripple Ember — fire rings

# Effect
void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    vec2 uv = frag_coord / u_resolution;
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    vec2 p = (uv - 0.5) * (2.0 * zoom);
    float r = length(p);
    float a = atan(p.y, p.x);
    float rings = 2.0 + 5.0 * detail;
    float ring = pow(1.0 - abs(sin(r * rings * 3.14159 - u_time * 1.6)), mix(1.2, 4.0, contrast * 0.35));
    float swirl = 0.5 + 0.5 * sin(a * (3.0 + 4.0 * detail) + u_time * 0.9 + r * 4.0);
    float ember = pow(clamp(ring * (0.35 + 0.65 * swirl) * (1.15 - r), 0.0, 1.0), contrast);
    float h = fract(hue + ember * 0.12 + u_time * 0.02);
    vec3 hot = vec3(1.0, 0.28 + 0.25 * cos(h * 6.2831853), 0.04);
    out_color = vec4(mix(vec3(0.02, 0.0, 0.03), hot, ember), 1.0);
}
