# Name
name: Harmonic Pulse
class: HarmonicPulse
category: Volume
description: Room-wide harmonic pulse
global: speed brightness frequency detail size scale fps color surface position strip bands
pattern_index: 1
pattern_label: Color pulse:
pattern_key: harmonic_color_mode
pattern: One color (brightness pulse)
pattern: Two colors (snap A↔B)
pattern: Two colors (soft A↔B)
pattern: Multi color (chase)
slider: harmonic_spatial_amount 0 100 85 unit pct | Spatial waves:
slider: harmonic_flow_amount 40 250 115 unit pct | Pulse energy:
slider: harmonic_pulse_contrast 35 200 85 unit pct | Pulse contrast:
slider: harmonic_zoom_wobble 0 300 55 unit | Zoom wobble:
param: harmonic_motion
param: harmonic_freq
param: unit harmonic_zoom_wobble
param: unit harmonic_pulse_contrast
param: harmonic_density
param: unit harmonic_spatial_amount
finish: hex

# Effect
void volumeMain(out vec4 out_color, in vec3 p01)
{
    float motion = max(u_params[0], 0.0);
    float spatial_freq = max(u_params[1], 0.5);
    float wobble = clamp(u_params[2], 0.0, 3.0);
    float contrast = clamp(u_params[3], 0.35, 2.5);
    float size_density = max(u_params[4], 0.2);
    float pulse_mix = clamp(u_params[5], 0.0, 1.0);
    const float TWO_PI = 6.2831853;

    float beat = 0.5 + 0.5 * sin(u_time * motion * TWO_PI);
    float beat2 = 0.5 + 0.5 * sin(u_time * motion * 1.618 * TWO_PI + 1.2);
    float master = clamp(0.65 * beat + 0.35 * beat2, 0.0, 1.0);

    float zw = 0.5 + 0.5 * sin(u_time * motion * 0.55 * TWO_PI);
    float zoom = 1.0 + zw * wobble * 0.35;

    float xf = (p01.x - 0.5) * spatial_freq * zoom * size_density;
    float yf = (p01.y - 0.5) * spatial_freq * zoom * size_density;
    float zf = (p01.z - 0.5) * spatial_freq * zoom * size_density;

    float t1 = u_time * motion * TWO_PI;
    float t2 = u_time * motion * 0.73 * TWO_PI;
    float field = 0.5 + 0.5 * (
        0.45 * sin(xf * TWO_PI + t1) +
        0.30 * cos(yf * TWO_PI + t2) +
        0.25 * sin(zf * TWO_PI + t1 - t2));
    field = clamp(field, 0.0, 1.0);

    float val = mix(master, field * (0.35 + 0.65 * master), pulse_mix);
    val = pow(clamp(val, 0.0, 1.0), contrast);
    val = clamp(0.18 + 0.82 * val, 0.0, 1.0);

    /* Hue follows spatial cycles tightly (≈ spatial_freq bands across the room). */
    float phase01 = fract(xf + yf * 0.71 + zf * 0.43 + master * 0.08 + u_time * motion * 0.12);
    out_color = vec4(val, phase01, master, 1.0);
}
