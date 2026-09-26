# Name
name: Slow Waves — soft blue bands

# Effect
void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    vec2 uv = frag_coord / u_resolution;
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    vec2 p = (uv - 0.5) * zoom;
    float dens = 4.0 + 14.0 * detail;
    float wave = sin(p.x * dens + u_time) * 0.5 + sin(p.y * dens * 0.75 - u_time * 0.85) * 0.5;
    wave = pow(clamp(wave * 0.5 + 0.5, 0.0, 1.0), contrast);
    float ang = hue * 6.2831853;
    vec3 deep = vec3(0.04, 0.06, 0.18);
    vec3 crest = vec3(0.20 + 0.35 * cos(ang), 0.45 + 0.25 * sin(ang), 0.90);
    out_color = vec4(mix(deep, crest, smoothstep(0.28, 0.72, wave)), 1.0);
}
