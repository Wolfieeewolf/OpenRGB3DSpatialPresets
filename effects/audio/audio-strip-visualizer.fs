name: Audio Strip Visualizer
class: AudioStripVisualizer
category: Audio
description: Strip-first spectrum bars or scrolling spectrogram driven by Listen Role / Hz band
drive: audio
audio_preset: spectrum
audio_media: spectrogram
global: audio speed brightness frequency size scale color strip path
resolution: 22
user_colors: 2
needs_frequency: true
combo: display_mode | Display:
option: Spectrum bars
option: Spectrogram scroll
combo: mirror_bars | Mirror:
option: Off
option: On
slider: scroll_speed 0 200 50 unit pct | Scroll speed: | Spectrogram scroll rate (bar mode ignores this).
param: combo display_mode
param: path_axis
param: combo mirror_bars
param: size_audio
param: audio_bar_edge
param: audio_strip_scroll
param: speed_mul
finish: audio
# Effect
void volumeMain(out vec4 out_color, in vec3 p01)
{
    int display_mode = int(floor(u_params[0] + 0.5));
    int path_axis    = int(floor(u_params[1] + 0.5));
    float mirror_b   = step(0.5, u_params[2]);
    float size_m     = max(u_params[3], 0.2);
    float bar_edge   = max(u_params[4], 0.02);
    float scroll_off = fract(u_params[5]);
    float speed_mul  = max(u_params[6], 0.15);

    /* Frequency / column count baked in — changing these requires a host rebuild. */
    const float col_count = 64.0;
    const float row_count = 72.0;

    float ax = p01.x, ay = p01.y, az = p01.z;
    float path01 = ax, disp01 = ay;
    if(path_axis == 1) { path01 = ay; disp01 = ax; }
    else if(path_axis == 2) { path01 = az; disp01 = ay; }

    if(mirror_b > 0.5 && display_mode == 0)
        path01 = abs(path01 * 2.0 - 1.0);
    path01 = clamp(path01, 0.0, 1.0);
    disp01 = clamp(disp01, 0.0, 1.0);

    float energy = 0.0;
    if(display_mode == 1)
    {
        /* Spectrogram: newer rows at low disp01 values. */
        float age01 = clamp(1.0 - disp01 + scroll_off * 0.15, 0.0, 1.0);
        float u = (floor(path01 * (col_count - 1.0) + 0.5) + 0.5) / col_count;
        float v = (floor(age01 * (row_count - 1.0) + 0.5) + 0.5) / row_count;
        float band = texture2D(u_media, vec2(u, v)).r;
        /* Contrast gate: min ~0.08 before lighting — no wash on ambient noise. */
        float gate_t = clamp((band - 0.08) / 0.10, 0.0, 1.0);
        gate_t = gate_t * gate_t * (3.0 - 2.0 * gate_t);
        energy = band * gate_t;
    }
    else
    {
        float u = (floor(path01 * (col_count - 1.0) + 0.5) + 0.5) / col_count;
        float level = texture2D(u_media, vec2(u, 0.5)).r;
        /* Softstep gate before bar mask — quiet columns stay dark. */
        float gate_t = clamp((level - 0.06) / 0.10, 0.0, 1.0);
        gate_t = gate_t * gate_t * (3.0 - 2.0 * gate_t);
        level = level * gate_t;
        float edge = bar_edge / max(0.2, size_m);
        float bt = clamp((level - disp01 + edge * 0.5) / max(edge, 0.01), 0.0, 1.0);
        float bar = bt * bt * (3.0 - 2.0 * bt);
        energy = level * bar * (0.85 + 0.15 * speed_mul);
    }

    energy = clamp(energy, 0.0, 1.0);
    out_color = vec4(energy, path01, 0.0, 1.0);
}
