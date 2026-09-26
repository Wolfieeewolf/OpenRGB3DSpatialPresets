name: Spectrum Bars
class: SpectrumBars
category: Audio
description: Spectrum bar graph in the zone driven by Listen Role / Hz band
drive: audio
audio_preset: spectrum
audio_media: bands
global: audio brightness frequency size scale color bands strip
resolution: 22
user_colors: 2
needs_frequency: true
slider: roll_speed 0 200 0 unit pct | Roll speed: | Scrolls the bar pattern along the spectrum axis over time.
param: audio_band_count
param: cent roll_speed
param: audio_roll_phase
param: size_audio
param: detail
param: speed_mul
param: tight_mul
param: audio_falloff
finish: audio
# Effect
void volumeMain(out vec4 out_color, in vec3 p01)
{
    float band_count = max(u_params[0], 1.0);
    float roll_phase = u_params[2];
    float size_m     = max(u_params[3], 0.35);
    float detail     = clamp(u_params[4], 0.05, 1.0);
    float tight_mul  = max(u_params[6], 0.25);
    float falloff    = max(u_params[7], 0.25);

    /* Expand axis around centre; size drives how many bands are visible. */
    float span = max(1.0, 0.70 + 0.50 * size_m);
    float axis = clamp(0.5 + (p01.x - 0.5) * span, 0.0, 1.0);
    float axis_rolled = fract(axis + roll_phase + 1.0);

    float u = (floor(axis_rolled * band_count) + 0.5) / band_count;
    float band_value = texture2D(u_media, vec2(u, 0.5)).r;
    /* Host already applies noise gate + peak_boost to the texture — no second multiply here. */

    /* Bar mask: LED lights if it lies below the band height (fills from floor). */
    float bt = clamp((band_value - p01.y + 0.025) / 0.05, 0.0, 1.0);
    float bar = bt * bt * (3.0 - 2.0 * bt);

    /* Thinner radial influence — keeps column look, not radial blob. */
    vec3 c = p01 - vec3(0.5);
    float radial = clamp(length(c) / 0.8660254, 0.0, 1.0);
    float radial_profile = clamp(1.0 - radial * 0.40, 0.60, 1.0);

    /* Very subtle sweep (0.94–1.0) — old 0.70 floor caused ambient flicker. */
    float sweep = 0.94 + 0.06 * sin((u_params[5] + axis_rolled) * 6.2831853);

    float energy = band_value * bar * radial_profile * sweep;
    energy = clamp(energy, 0.0, 1.0);

    out_color = vec4(energy, axis_rolled, 0.0, 1.0);
}
