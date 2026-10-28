# Name
name: Pulse Ring
class: PulseRing
category: Volume
description: Soft expanding shells that stay visible on sparse LED rooms
global: speed brightness frequency detail size scale fps color surface position strip bands thickness
pattern_label: Style:
pattern_key: ring_style
pattern: Pulse
pattern: Radial Rainbow
combo: pulse_shape 3 | Pulse shape:
option: Circle (flat ring)
option: Sphere
option: Hexagon
option: Triangle
option: Square
slider: hole_size 0 75 8 unit pct | Hole size: | Inner dead zone
slider: pulse_amplitude 20 200 100 unit pct | Pulse amplitude: | Shell brightness
slider: direction_deg 0 360 0 | Phase: | Ring phase offset
param: anim_time
param: unit hole_size
param: gthickness
param: unit pulse_amplitude
param: ndetail
param: pattern
param: turn direction_deg
param: combo pulse_shape
param: size_ring
param: hue_scroll_s
finish: hex

# Effect
float hexMetric(vec2 p)
{
    vec2 a = abs(p);
    return max(dot(a, vec2(0.5, 0.8660254)), a.x);
}
float polyMetric(vec2 p, float n)
{
    float an = 6.2831853 / n;
    float a = atan(p.y, p.x);
    float r = length(p);
    return cos(floor(0.5 + a / an) * an - a) * r;
}
float footprintXZ(vec2 p, int shape)
{
    if(shape == 2)
        return hexMetric(p) / 0.8660254;
    if(shape == 3)
        return max(polyMetric(p, 3.0) / 0.5, 0.0);
    if(shape == 4)
        return max(abs(p.x), abs(p.y));
    return length(p);
}
float softBand(float dist_to_edge, float half_w)
{
    float s = max(half_w, 0.028);
    float core = exp(-(dist_to_edge * dist_to_edge) / (s * s * 0.40));
    float halo = exp(-(dist_to_edge * dist_to_edge) / (s * s * 1.70)) * 0.40;
    return clamp(core + halo, 0.0, 1.0);
}
void volumeMain(out vec4 out_color, in vec3 p01)
{
    float time_sec = u_params[0];
    float hole_r = clamp(u_params[1], 0.0, 0.7);
    float sigma = max(u_params[2], 0.018);
    float amp = clamp(u_params[3], 0.2, 2.0);
    float detail = clamp(u_params[4], 0.05, 1.0);
    int style = int(u_params[5] + 0.5);
    float phase_offset = u_params[6];
    int shape = int(clamp(u_params[7], 0.0, 4.0) + 0.5);
    float size_scale = clamp(u_params[8], 0.25, 2.5);
    float hue_scroll = fract(u_params[9]);
    vec3 p = p01 * 2.0 - 1.0;
    float inv_size = 1.0 / max(size_scale, 0.25);
    float extent = (shape == 1) ? 1.55 : 1.12;
    float usable = max(0.12, extent - hole_r);

    float half_w = max(0.028, sigma * mix(0.70, 0.26, detail));
    float y_half = mix(0.40, 0.14, detail);

    float d = 0.0;
    float height_mul = 1.0;
    if(shape == 1)
    {
        d = length(p) * inv_size;
        height_mul = 1.0;
    }
    else
    {
        d = footprintXZ(p.xz, shape) * inv_size;
        height_mul = 1.0 - smoothstep(y_half, y_half + 0.045, abs(p.y));
    }

    float intensity = 0.0;
    float color_drv = 0.0;
    float az01 = fract(atan(p.z, p.x) / 6.2831853 + 0.5 + hue_scroll);

    if(style == 1)
    {
        float fill = smoothstep(hole_r - 0.03, hole_r + 0.02, d);
        float outer_end = (shape == 1) ? 1.65 : 1.22;
        float outer = 1.0 - smoothstep(extent * 0.90, outer_end, d);
        intensity = fill * outer * height_mul;
        color_drv = az01;
    }
    else
    {
        /* Continuous expand from anim_time - soft dual crest avoids wrap flash. */
        float expand = fract(time_sec * 0.28 + phase_offset);
        float center = hole_r + expand * usable;
        float prev = hole_r + (expand - 1.0) * usable;
        float ring0 = softBand(d - center, half_w);
        float ring1 = softBand(d - prev, half_w) * (1.0 - expand);
        intensity = max(ring0, ring1) * height_mul;
        intensity *= smoothstep(hole_r - 0.04, hole_r + 0.02, d);
        intensity *= amp;
        color_drv = fract(clamp((d - hole_r) / usable, 0.0, 1.0) * 0.85 + hue_scroll);
    }

    intensity = clamp(intensity, 0.0, 1.0);
    out_color = vec4(intensity, color_drv, 0.0, 1.0);
}
