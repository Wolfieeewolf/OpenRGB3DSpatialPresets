# Name
name: Noise fire column
class: NoiseFireColumn
category: Volume
description: Rising fire column that fills sparse LEDs - fuel in XZ, heat along Y (denser atlas + vfx soft edges)
global: speed brightness frequency detail size scale fps color surface position strip bands thickness
resolution: 40
colors: 5C0A0A A01510 E83B00 FF7A18 F5C84A FFF5D6
slider: fuel 0 100 62 unit pct | Fuel: | Base burn strength (mid already burns hot)
slider: flicker 0 100 55 unit pct | Flicker: | Noise liveliness
slider: column 0 100 28 unit pct | Column: | How tight the column is (higher = thinner)
param: unit fuel
param: unit flicker
param: unit column
param: motion_clock
param: size_star
param: ndetail
param: freq_n
param: gthickness
finish: hex
# Source: Godot VFX Library burning.gdshader (haowg) - motion/colour language only: rising heat tongues, heat-wave sway. Clean-room GLSL; no code copied.
# Source: Effekseer sample fire / Magical Spiral FirePot (ideas only) - hot core, cooler smoke rim.
# Colour via host Colors / Rainbow (finish hex). Default swatches are a crimson→amber→white fire ramp.

# Effect
void volumeMain(out vec4 out_color, in vec3 p01)
{
    /* Mid-slider bias only (pow); Fuel/Flicker 0 still mean off/calm. */
    float fuel = pow(clamp(u_params[0], 0.0, 1.0), 0.62);
    float flick = pow(clamp(u_params[1], 0.0, 1.0), 0.68);
    float col_t = clamp(u_params[2], 0.0, 1.0);
    /* Rise clock: u_time is Speed-scaled seconds (0 at Speed 0). Prefer host motion_clock
       when packed (includes stratum speed_mul); never let a dead param freeze a live u_time. */
    float clock = max(u_time, u_params[3]);
    float size_m = clamp(u_params[4], 0.35, 2.5);
    float detail = clamp(u_params[5], 0.05, 1.0);
    float freq_n = clamp(u_params[6], 0.0, 1.0);
    float thick = max(u_params[7], 0.08);

    float col_w = mix(0.78, 0.16, pow(col_t, 1.20)) * clamp(size_m, 0.55, 1.85);
    float soft_k = mix(1.65, 1.30, clamp(thick, 0.0, 1.5));
    float h = clamp(p01.y, 0.0, 1.0);
    vec2 xz = p01.xz - vec2(0.5);

    float rise = clock * (1.85 + 1.55 * flick) * (0.90 + 0.55 * freq_n);
    /* Heat-wave sway — stronger near the fuel bed. */
    float sway_amp = mix(0.022, 0.070, flick) * (0.30 + 0.70 * (1.0 - h));
    vec2 heat = vec2(
        vfxNoise3(vec3(xz * 1.55, rise * 0.38)) - 0.5,
        vfxNoise3(vec3(xz.yx * 1.55 + 2.7, rise * 0.33)) - 0.5);
    xz += heat * sway_amp * 2.0;
    float radial = length(xz);

    float mask = exp(-pow(radial / max(col_w, 0.08), soft_k));
    float halo = exp(-pow(radial / max(col_w * mix(1.70, 2.35, clamp(thick, 0.0, 1.0)), 0.14), 1.40))
               * mix(0.40, 0.58, clamp(thick, 0.0, 1.0));
    float base = fuel * (1.0 - smoothstep(0.06, 0.98, h));
    base *= 0.62 + 0.38 * smoothstep(0.0, 0.14, h);

    float nscale = mix(2.9, 4.8, detail);
    float octaves = mix(2.5, 4.0, detail);

    vec3 np = vec3(xz * nscale, h * (2.2 + 1.0 * detail) - rise);
    float n = vfxFbm3(np, octaves);
    float tip = vfxNoise3(vec3(xz * (nscale * 1.8), h * 4.0 - rise * 1.85));
    float tongue = vfxNoise3(vec3(xz * (nscale * 0.60) + tip * 0.15, h * 1.4 - rise * 0.65));

    /* Strong tongue/tip contrast so rise reads on sparse LEDs (not a flat orange slab). */
    float dens = (mask + halo) * base * (0.28 + 0.72 * n) * (0.48 + 0.52 * tip);
    dens *= 0.55 + 0.45 * tongue;
    dens *= smoothstep(0.008, 0.07, h);
    dens = clamp(dens * mix(1.35, 1.85, detail * 0.5 + 0.5), 0.0, 1.0);

    float core = exp(-pow(radial / max(col_w * 0.48, 0.04), 1.75));
    float white_hot = core * dens * (1.0 - h * 0.45);

    float intensity = clamp(dens * 1.55, 0.0, 1.0);
    /* Heat walks the Look ramp with the rising tongues, not only height. */
    float heat01 = clamp(
        0.06 + 0.18 * (1.0 - h)
      + 0.28 * core
      + 0.22 * white_hot
      + 0.22 * tongue
      + 0.16 * tip
      + 0.10 * sin(rise * 2.4 + h * 8.0 + tongue * 3.0),
        0.0, 1.0);
    float gloss01 = clamp(white_hot * 0.90 + tip * 0.30 + tongue * 0.20 + core * 0.30 * (1.0 - h), 0.0, 1.0);

    out_color = vec4(intensity, heat01, gloss01, 1.0);
}
