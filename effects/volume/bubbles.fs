# Name
name: Bubbles
class: Bubbles
category: Volume
description: Soft rising bubbles that stay visible on sparse LED grids
global: speed brightness frequency detail size scale fps color surface position strip bands count
slider: bubble_thickness 8 100 72 unit pct | Ring thickness: | Shell / glow width
slider: rise_speed 10 400 140 unit | Rise speed: | How fast bubbles climb
slider: spawn_interval 20 250 95 unit | Spawn interval: | Time between launches
slider: max_radius 50 350 200 unit | Bubble size: | Peak radius in the room
slider: horizontal_fill 50 180 140 unit pct | Horizontal fill: | XZ spread across the room
slider: overlap_spacing 10 100 65 unit pct | Min spacing: | Packing of bubble lanes
slider: launch_randomness 0 100 55 unit pct | Launch randomness: | Jitter in timing and path
param: time
param: gcount 6 48
param: bubble_thick
param: bubble_rise
param: bubble_interval
param: bubble_radius
param: unit horizontal_fill
param: unit launch_randomness
param: hue_scroll_s
param: unit overlap_spacing
finish: hex

# Effect
float hash01(float seed, float salt)
{
    return fract(sin(seed * 12.9898 + salt * 78.233) * 43758.5453);
}
float bbSoftRing(float d, float radius, float thick)
{
    float s = max(thick, 0.028);
    float shell = abs(d - radius);
    float ring = exp(-(shell * shell) / (s * s * 0.55));
    float fill = exp(-(d * d) / max((radius + s) * (radius + s) * 0.55, 1e-4)) * 0.22;
    float halo = exp(-(shell * shell) / (s * s * 1.85)) * 0.40;
    return clamp(ring + halo + fill * step(d, radius + s), 0.0, 1.0);
}
void volumeMain(out vec4 out_color, in vec3 p01)
{
    float time_sec = u_params[0];
    int count = int(clamp(u_params[1], 6.0, 48.0) + 0.5);
    float thick = max(u_params[2], 0.022);
    float rise_rate = max(u_params[3], 0.0);
    float interval = max(u_params[4], 0.12);
    float max_r = max(u_params[5], 0.05);
    float fill = clamp(u_params[6], 0.5, 1.8);
    float launch_jitter = clamp(u_params[7], 0.0, 1.0);
    float hue_scroll = fract(u_params[8]);
    float spacing = clamp(u_params[9], 0.10, 1.0);

    float intensity = 0.0;
    float hue01 = hue_scroll;
    const float golden = 2.39996323;
    float voxel = 2.0 / max(u_res, 8.0);
    float soft = max(thick, max(voxel, 0.028));

    for(int i = 0; i < 48; i++)
    {
        float fi = float(i);
        float slot_on = 1.0 - step(float(count), fi);
        float seed = fi * 265.443 + 101.390;
        float cycle_mul = (1.0 + 0.35 * launch_jitter) + (2.0 * launch_jitter) * hash01(seed, 11.0);
        float active_frac = (0.55 - 0.24 * launch_jitter) + (0.06 + 0.22 * launch_jitter) * hash01(seed, 12.0);
        float cycle_i = max(0.12, interval * cycle_mul);
        float active_window = max(0.05, cycle_i * active_frac);
        float offset_i = cycle_i * hash01(seed, 13.0);
        float phase_i = mod(time_sec * rise_rate + offset_i, cycle_i);
        float in_window = 1.0 - step(active_window, phase_i);

        float radius_phase = phase_i / active_window;
        float grow = smoothstep(0.0, 0.18, radius_phase) * (1.0 - smoothstep(0.82, 1.0, radius_phase));
        float radius = (0.20 + 0.80 * radius_phase) * max_r;
        float ring = sqrt((fi + 0.5) / float(count));
        float ang = fi * golden;
        float spread = 0.78 * fill * (0.55 + 0.45 * spacing);
        vec3 c;
        c.x = 0.5 + cos(ang) * ring * spread;
        c.y = radius_phase;
        c.z = 0.5 + sin(ang) * ring * spread;
        c.x += sin(time_sec * 0.55 + seed) * 0.03 * launch_jitter;
        c.z += cos(time_sec * 0.48 + seed * 1.3) * 0.03 * launch_jitter;
        c = clamp(c, vec3(0.0), vec3(1.0));

        float d = length(p01 - c);
        float value = bbSoftRing(d, radius, soft * (0.85 + 0.35 * radius / max(max_r, 1e-3)));
        value *= slot_on * in_window * grow;
        if(value > intensity)
        {
            intensity = value;
            hue01 = fract(fi * 0.111 + hue_scroll + 0.05 * radius_phase);
        }
    }

    out_color = vec4(clamp(intensity, 0.0, 1.0), hue01, 0.0, 1.0);
}
