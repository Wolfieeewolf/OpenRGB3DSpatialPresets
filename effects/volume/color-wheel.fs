# Name
name: Color Wheel
class: ColorWheel
category: Volume
description: Soft spinning hue on the selected plane for sparse LED rooms
global: speed brightness frequency detail size scale fps color surface position plane
rainbow: true
combo: direction | Direction:
option: Clockwise
option: Counter-clockwise
combo: hue_geometry_mode | Hue geometry:
option: Radial (classic)
option: Shear (bands)
option: Rings (concentric)
option: Pie slices
slider: hue_repeats 1 100 1 | Hue repeats: | Rainbow cycles across the plane
param: anim_time
param: sign direction
param: raw hue_repeats
param: plane
param: combo hue_geometry_mode
param: freq_spin
param: size_floor
finish: atlas

# Effect
void volumeMain(out vec4 out_color, in vec3 p01)
{
    /* Continuous anim_time - no progress wrap flash. */
    float progress = u_params[0] * 0.22;
    float dir = u_params[1];
    float wrap = clamp(u_params[2], 0.1, 100.0);
    int pl = int(u_params[3] + 0.5);
    int geom = int(u_params[4] + 0.5);
    float freq_spin = u_params[5];
    float size_scale = max(u_params[6], 0.2);

    float lx = (p01.x * 2.0 - 1.0) / size_scale;
    float ly = (p01.y * 2.0 - 1.0) / size_scale;
    float lz = (p01.z * 2.0 - 1.0) / size_scale;

    float u = lx;
    float v = lz;
    if(pl == 1) { u = lx; v = ly; }
    else if(pl == 2) { u = lz; v = ly; }

    float angle = 0.0;
    if(geom == 1)
    {
        float spin = progress * 6.2831855 * dir + freq_spin;
        float cu = cos(spin);
        float su = sin(spin);
        angle = (u * cu + v * su) * 3.14159265 * wrap;
    }
    else if(geom == 2)
    {
        float rad = length(vec2(u, v));
        angle = rad * 6.2831855 * wrap - progress * 6.2831855 * dir - freq_spin;
    }
    else if(geom == 3)
    {
        float slices = max(2.0, floor(wrap * 6.0 + 0.5));
        float a = atan(v, u);
        float spin = progress * 6.2831855 * dir + freq_spin;
        a = a - spin;
        float sector = floor((a / 6.2831855 + 1.0) * slices);
        angle = (sector + 0.5) / slices * 6.2831855;
    }
    else
    {
        angle = atan(v, u) * wrap;
    }

    float hue_turns = angle / 6.2831855 + freq_spin * 0.02;
    if(geom != 2)
        hue_turns += progress * dir;
    float ang = hue_turns * 6.2831855;
    out_color = vec4(cos(ang) * 0.5 + 0.5, sin(ang) * 0.5 + 0.5, 0.0, 1.0);
}
