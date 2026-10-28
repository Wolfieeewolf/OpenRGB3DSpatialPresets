# Name
name: Plasma
class: Plasma
global: speed brightness frequency detail size scale fps color surface position strip bands
category: Volume
description: Layered room plasma that stays readable on sparse LED grids
colors: 00FF00 FF00FF FFFF00
finish: depth
supports_strip_colormap: true
supports_height_bands: true
pattern_key: pattern_type
param: anim_time
param: freq_scale
param: pattern

# Patterns
pattern: Classic | Layered sines in X/Y plus mild radial and Z terms.
pattern: Swirl | Polar swirl in the horizontal plane with Z modulation.
pattern: Ripple | Radial rings from center.
pattern: Organic | Coupled flows with nested sines.
pattern: Noise | High-frequency grain.
pattern: CubeFire | Rising tongues with floor heat.

# Effect
float finalizePlasma01(float raw, float bias, float gain)
{
    float v = (raw + 6.0) / 12.0;
    v = clamp(v + bias, 0.0, 1.0);
    if(gain > 0.01 && abs(gain - 1.0) > 0.001)
        v = pow(v, gain);
    return clamp(v, 0.0, 1.0);
}
void volumeMain(out vec4 out_color, in vec3 p01)
{
    /* Continuous anim_time - no progress wrap flash. */
    float prog = u_params[0];
    float freq_scale_e = max(u_params[1], 0.05);
    int pattern_type = int(u_params[2] + 0.5);
    /* Soften spatial freq so sparse rooms keep broad lobes. */
    float fs = freq_scale_e * 0.72;
    float coord1 = p01.x;
    float coord2 = p01.y;
    float coord3 = p01.z;
    float plasma_value = 0.5;

    if(pattern_type == 0)
    {
        plasma_value =
            sin((coord1 + prog * 0.55) * fs * 8.0) +
            sin((coord2 + prog * 0.48) * fs * 6.5) +
            sin((coord1 + coord2 + prog * 0.38) * fs * 5.0) +
            cos((coord1 - coord2 + prog * 0.62) * fs * 5.5) +
            sin(sqrt(coord1 * coord1 + coord2 * coord2) * fs * 4.2 + prog * 0.42) +
            cos(coord3 * fs * 3.4 + prog * 0.28);
        plasma_value = finalizePlasma01(plasma_value, 0.02, 1.05);
    }
    else if(pattern_type == 1)
    {
        float angle = atan(coord2 - 0.5, coord1 - 0.5);
        float radius = sqrt((coord1 - 0.5) * (coord1 - 0.5) + (coord2 - 0.5) * (coord2 - 0.5));
        plasma_value =
            sin(angle * 3.0 + radius * fs * 7.5 + prog * 0.70) +
            sin(angle * 2.0 - radius * fs * 5.5 + prog * 0.52) +
            cos(angle * 5.0 + radius * fs * 3.6 - prog * 0.60) +
            sin(coord3 * fs * 4.2 + prog * 0.35) +
            cos((angle * 2.0 + coord3 * fs * 2.8) + prog * 0.40);
        plasma_value = finalizePlasma01(plasma_value, -0.02, 1.22);
    }
    else if(pattern_type == 2)
    {
        float dist_from_center = sqrt((coord1 - 0.5) * (coord1 - 0.5) + (coord2 - 0.5) * (coord2 - 0.5));
        plasma_value =
            sin(dist_from_center * fs * 9.0 - prog * 0.95) +
            sin(dist_from_center * fs * 13.0 - prog * 0.72) +
            cos(dist_from_center * fs * 6.5 + prog * 0.55) +
            sin((coord1 + coord2) * fs * 4.0 + prog * 0.28) * 0.45 +
            cos(coord3 * fs * 3.2 - prog * 0.18) * 0.40;
        plasma_value = finalizePlasma01(plasma_value, 0.03, 1.15);
    }
    else if(pattern_type == 3)
    {
        float flow1 = sin(coord1 * fs * 6.5 + sin(coord2 * fs * 9.0 + prog * 0.35) + prog * 0.18);
        float flow2 = cos(coord2 * fs * 7.0 + cos(coord3 * fs * 8.5 + prog * 0.40));
        float flow3 = sin(coord3 * fs * 5.5 + sin(coord1 * fs * 10.0 + prog * 0.22));
        float flow4 = cos((coord1 + coord2) * fs * 4.5 + sin(prog * 0.45));
        float flow5 = sin((coord2 + coord3) * fs * 4.0 + cos(prog * 0.52));
        plasma_value = flow1 + flow2 + flow3 + flow4 + flow5;
        plasma_value = finalizePlasma01(plasma_value, 0.05, 0.92);
    }
    else if(pattern_type == 4)
    {
        float n1 = sin((coord1 + prog * 0.18) * fs * 22.0) *
                   sin((coord2 + prog * 0.12) * fs * 28.0) *
                   sin((coord3 + prog * 0.22) * fs * 18.0);
        float n2 = sin((coord1 * 2.0 + coord2 + prog * 0.30) * fs * 12.0) *
                   cos((coord2 * 1.5 + coord3 + prog * 0.35) * fs * 14.0);
        float n3 = cos((coord1 + coord2 * 1.8 + coord3) * fs * 9.0 + prog * 0.55);
        plasma_value = n1 * 0.45 + n2 * 0.40 + n3 * 0.25;
        plasma_value = finalizePlasma01(plasma_value, 0.0, 1.35);
    }
    else
    {
        float y = coord2 - prog * 0.18;
        float turb = 0.20 * sin((coord1 * 2.6 + coord3 * 2.2) * fs * 6.0 + prog * 1.1);
        float xw = coord1 + turb * sin(y * fs * 7.5 + prog * 0.55);
        float zw = coord3 + turb * cos(y * fs * 7.0 - prog * 0.48);
        float tongues =
            sin(xw * fs * 10.0 + prog * 0.90) +
            cos(zw * fs * 8.5 - prog * 0.70) +
            0.7 * sin((xw + zw) * fs * 6.5 + y * fs * 11.0 - prog * 1.4) +
            0.5 * cos(y * fs * 12.0 + prog * 0.42);
        float floor_heat = pow(clamp(1.0 - coord2, 0.0, 1.0), 0.50);
        plasma_value = tongues * (0.35 + 0.65 * floor_heat) + 1.35 * floor_heat;
        plasma_value = pow(clamp((plasma_value + 3.0) / 6.0, 0.0, 1.0), 0.68);
    }

    /* Mild lift so sparse LEDs stay above black in troughs. */
    plasma_value = clamp(plasma_value * 1.08 + 0.04, 0.0, 1.0);
    out_color = vec4(plasma_value, plasma_value, plasma_value, 1.0);
}
