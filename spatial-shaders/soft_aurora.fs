# Name
name: Soft Aurora — curtain bands

# Effect
void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    vec2 uv = frag_coord / u_resolution;
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (1.6 * zoom);
    float t = u_time * 0.35;
    float v = 0.0;
    float band0 = 0.55 + 0.45 * sin(p.x * (1.2) + t * 0.6 + sin(p.y * 2.2 + t * 0.4));
    float curtain0 = exp(-abs(p.y + 0.15 * sin(p.x * 1.5 + t) + 0.36) * (3.0 + detail * 2.0));
    v += band0 * curtain0 * 0.45;
    float band1 = 0.55 + 0.45 * sin(p.x * (1.2 + 0.35 * detail) + t * 0.75 + sin(p.y * 2.2 + t * 0.4 + 1.0));
    float curtain1 = exp(-abs(p.y + 0.15 * sin(p.x * 1.5 + t + 1.0) + 0.18) * (3.0 + detail * 2.0));
    v += band1 * curtain1 * 0.33;
    float band2 = 0.55 + 0.45 * sin(p.x * (1.2 + 0.70 * detail) + t * 0.90 + sin(p.y * 2.2 + t * 0.4 + 2.0));
    float curtain2 = exp(-abs(p.y + 0.15 * sin(p.x * 1.5 + t + 2.0)) * (3.0 + detail * 2.0));
    v += band2 * curtain2 * 0.26;
    float band3 = 0.55 + 0.45 * sin(p.x * (1.2 + 1.05 * detail) + t * 1.05 + sin(p.y * 2.2 + t * 0.4 + 3.0));
    float curtain3 = exp(-abs(p.y + 0.15 * sin(p.x * 1.5 + t + 3.0) - 0.18) * (3.0 + detail * 2.0));
    v += band3 * curtain3 * 0.21;
    float band4 = 0.55 + 0.45 * sin(p.x * (1.2 + 1.40 * detail) + t * 1.20 + sin(p.y * 2.2 + t * 0.4 + 4.0));
    float curtain4 = exp(-abs(p.y + 0.15 * sin(p.x * 1.5 + t + 4.0) - 0.36) * (3.0 + detail * 2.0));
    v += band4 * curtain4 * 0.18;
    v = pow(clamp(v, 0.0, 1.0), mix(1.4, 0.7, contrast / 2.5));
    float h = fract(0.55 + hue + v * 0.12 + t * 0.02);
    vec3 rgb = clamp(abs(mod(h * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    rgb = rgb * rgb * (3.0 - 2.0 * rgb);
    out_color = vec4(mix(vec3(1.0), rgb, 0.55 + 0.35 * v) * (0.20 + 0.80 * v), 1.0);
}
