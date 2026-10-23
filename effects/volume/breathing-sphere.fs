# Name
name: Breathing Sphere
class: BreathingSphere
category: Volume
description: Breathing shell or whole-room inhale wave
global: speed brightness frequency detail size scale fps color surface position strip bands
resolution: 28
rainbow: true
combo: breathing_shape | Shape:
option: Sphere | Round 3D ball shell.
option: Square | Axis-aligned cube silhouette.
option: Rectangle | Rectangular box matching room width vs depth.
option: Triangle | Triangular prism (flat sides).
option: Pentagon | Pentagonal prism (flat sides).
option: Whole room (inhale wave) | No shell — inhale/exhale color wave from the effect origin.
combo: edge_profile 1 | Edge:
option: Soft | Gentle boundary falloff.
option: Crisp | Hard silhouette. Breath pulse keeps the chosen shape.
slider: breath_pulse_pct 0 100 55 pct | Breath pulse: | How far the shell grows/shrinks each breath. 0 = static.
slider: center_hole_pct 0 100 0 pct | Center hole: | 0 = solid; higher carves an empty core. Ignored in whole-room mode.
param: progress_tau
param: ndetail
param: combo edge_profile
param: unit center_hole_pct
param: combo breathing_shape
param: atlas_ax
param: atlas_az
param: unit breath_pulse_pct
param: size
param: atlas_sx
param: atlas_sy
param: atlas_sz
finish: hex

# Effect
float smstep(float e0, float e1, float x)
{
    float t = clamp((x - e0) / max(e1 - e0, 1e-5), 0.0, 1.0);
    return t * t * (3.0 - 2.0 * t);
}
float polyRadialXZ(vec2 p, float n)
{
    float an = 6.2831853 / max(n, 3.0);
    float a = atan(p.y, p.x);
    float r = length(p);
    return cos(floor(0.5 + a / an) * an - a) * r / cos(3.14159265 / n);
}
float shapeMetric(vec3 l, int shape, float ax, float az)
{
    if(shape == 1)
        return max(max(abs(l.x), abs(l.y)), abs(l.z));
    if(shape == 2)
        return max(max(abs(l.x) / max(ax, 1e-4), abs(l.y)), abs(l.z) / max(az, 1e-4));
    if(shape == 3 || shape == 4)
    {
        float n = (shape == 3) ? 3.0 : 5.0;
        return max(polyRadialXZ(l.xz, n), abs(l.y));
    }
    return length(l);
}
float shellCover(int shape, float sx, float sy, float sz, float ax, float az)
{
    float hx = 0.5 * sx;
    float hy = 0.5 * sy;
    float hz = 0.5 * sz;
    if(shape == 0 || shape == 5)
        return 0.5 * length(vec3(sx, sy, sz));
    if(shape == 3 || shape == 4)
    {
        float n = (shape == 3) ? 3.0 : 5.0;
        float max_poly = 0.0;
        max_poly = max(max_poly, polyRadialXZ(vec2(-hx, -hz), n));
        max_poly = max(max_poly, polyRadialXZ(vec2(-hx, hz), n));
        max_poly = max(max_poly, polyRadialXZ(vec2(hx, -hz), n));
        max_poly = max(max_poly, polyRadialXZ(vec2(hx, hz), n));
        return max(max_poly, hy);
    }
    if(shape == 2)
        return max(max(hx / max(ax, 1e-4), hy), hz / max(az, 1e-4));
    return max(max(hx, hy), hz);
}
void volumeMain(out vec4 out_color, in vec3 p01)
{
    float breath_phase = u_params[0];
    float detail = max(u_params[1], 0.05);
    int edge = int(clamp(u_params[2], 0.0, 1.0) + 0.5);
    float hole_frac = clamp(u_params[3], 0.0, 0.95);
    int shape = int(clamp(u_params[4], 0.0, 5.0) + 0.5);
    float ax = clamp(u_params[5], 0.15, 1.0);
    float az = clamp(u_params[6], 0.15, 1.0);
    float pulse_strength = clamp(u_params[7], 0.0, 1.0);
    float size_m = max(u_params[8], 0.08);
    vec3 s = max(vec3(u_params[9], u_params[10], u_params[11]), vec3(0.25));

    float breath_amp = pulse_strength * 0.92;
    float cover = max(shellCover(shape, s.x, s.y, s.z, ax, az), 1.0);
    float R = max(cover * size_m * (1.0 + breath_amp * sin(breath_phase)), 0.02);

    vec3 l = (p01 - vec3(0.5)) * s;

    if(shape == 5)
    {
        float dist_norm = clamp(length(l) * 0.5, 0.0, 1.5);
        float inhale = sin(breath_phase) * pulse_strength;
        float exhale = sin(breath_phase + 1.2) * pulse_strength;
        float spat = 9.0 + 5.0 * detail;
        float wave = sin(inhale * 3.14159265 * 1.15 - dist_norm * spat) * pulse_strength;
        float ripple = sin(breath_phase * 2.1 - dist_norm * 6.2831853 * 2.2 + l.y * 0.02 * detail) * pulse_strength;
        float rush = sin(exhale * 1.7 + (l.x + l.z) * 0.015 * detail) * 0.4 * pulse_strength;
        float air = 0.78 + 0.22 * (0.5 + 0.5 * sin(breath_phase * 1.05)) * (0.55 + 0.45 * pulse_strength);
        if(pulse_strength < 0.001)
            air = 0.85;
        float hue01 = clamp(dist_norm, 0.0, 1.0);
        out_color = vec4(clamp(air, 0.0, 1.0), hue01, 0.0, 1.0);
        return;
    }

    float distance = shapeMetric(l, shape, ax, az);
    float sphere_intensity = 0.0;
    float norm_in_shell = 0.0;

    float band = (edge == 1) ? 0.018 : 0.16;
    band = max(band * (0.7 + 0.3 / max(detail, 0.2)), (edge == 1) ? 0.012 : 0.02);

    if(hole_frac <= 0.001)
    {
        if(edge == 1)
        {
            float inside = 1.0 - smstep(R - band * 0.12, R + band * 0.55, distance);
            float surface = 1.0 - smstep(0.0, band * 0.55, abs(distance - R));
            inside = max(inside, surface * 0.4 * step(distance, R + band));
            if(distance > R + band)
                inside = 0.0;
            sphere_intensity = clamp(inside, 0.0, 1.0);
        }
        else
        {
            float inside = 1.0 - smstep(R - band * 0.35, R + band, distance);
            float soft_out = 1.0 - smstep(R, R + band * 2.2, distance);
            inside = max(inside, soft_out * 0.35);
            sphere_intensity = clamp(inside, 0.0, 1.0);
        }
        norm_in_shell = clamp(distance / (R + 1e-5), 0.0, 1.2);
    }
    else
    {
        float r_in = hole_frac * R * 0.9;
        float iw = (edge == 1) ? band * 0.4 : band * 1.1;
        float ow = (edge == 1) ? band * 0.5 : band * 1.6;
        float inner_open = smstep(r_in - iw, r_in + iw, distance);
        float outer_open = 1.0 - smstep(R - ow * 0.2, R + ow, distance);
        float span_eff = max(R - r_in, R * 0.08);
        float u = clamp((distance - r_in) / span_eff, 0.0, 1.0);
        float bell = sin(u * 3.14159265);
        if(edge == 1)
        {
            float b2 = bell * bell;
            bell = b2 * b2;
            if(distance > R + ow)
                outer_open = 0.0;
        }
        sphere_intensity = clamp(inner_open * outer_open * (0.15 + 0.85 * bell), 0.0, 1.0);
        norm_in_shell = clamp((distance - r_in) / max(R - r_in, 1e-4), 0.0, 1.2);
    }

    out_color = vec4(clamp(sphere_intensity, 0.0, 1.0), clamp(norm_in_shell, 0.0, 1.0), 0.0, 1.0);
}
