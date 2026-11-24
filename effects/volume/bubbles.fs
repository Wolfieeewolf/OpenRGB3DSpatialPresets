# Name
name: Bubbles
class: Bubbles
category: Volume
description: Rising soap-film bubbles with gloss highlights and wobble - readable on sparse LED rooms
resolution: 48
global: speed brightness frequency detail size scale fps color surface position strip bands count
slider: bubble_thickness 0 100 55 unit pct | Film thickness: | Soap-shell width (0 = off / invisible)
slider: rise_speed 10 400 140 unit | Rise speed: | How fast bubbles climb
slider: spawn_interval 20 250 95 unit | Spawn interval: | Time between launches
slider: max_radius 50 350 200 unit | Bubble size: | Peak radius in the room
slider: horizontal_fill 50 180 140 unit pct | Horizontal fill: | XZ spread across the room
slider: overlap_spacing 10 100 65 unit pct | Min spacing: | Packing of bubble lanes
slider: launch_randomness 0 100 55 unit pct | Launch randomness: | Jitter in timing and path
slider: gloss 0 100 62 unit pct | Gloss: | Specular highlight on the film (0 = off)
slider: film 0 100 12 unit pct | Film fill: | Soft mist inside the bubble (0 = hollow shell only)
slider: wobble 0 100 40 unit pct | Wobble: | Shape jiggle as bubbles rise (0 = perfect spheres)
param: motion_clock
param: gcount 6 48
param: bubble_thick
param: bubble_rise
param: bubble_interval
param: bubble_radius
param: unit horizontal_fill
param: unit launch_randomness
param: hue_scroll_s
param: unit overlap_spacing
param: unit gloss
param: unit film
param: unit wobble
param: unit bubble_thickness
param: ndetail
colors: 06283D 0E7490 5EEAD4 A78BFA F5D0FE F8FAFC
finish: hex

# Effect
float buHash(float seed, float salt)
{
    return vfxHash11(seed * 12.9898 + salt * 78.233);
}
/* Hollow soap film + optional interior mist + rim-light highlight. */
float buSoap(vec3 dlt, float radius, float thick, float gloss01, float film01, float wobble01, float phase, float seed)
{
    float r = max(radius, 0.012);
    /* Wobble: slight ellipsoid stretch that breathes with rise phase. */
    vec3 q = dlt;
    if(wobble01 > 0.01)
    {
        float w = wobble01;
        float breath = 0.55 + 0.45 * sin(phase * 6.2831853 * 2.0 + seed);
        float sx = 1.0 + w * 0.22 * sin(seed * 2.1 + phase * 9.0) * breath;
        float sy = 1.0 - w * 0.18 * breath;
        float sz = 1.0 + w * 0.20 * cos(seed * 1.7 + phase * 8.0) * breath;
        q = vec3(dlt.x / sx, dlt.y / sy, dlt.z / sz);
    }

    float sd = vfxSdSphere(q, r);
    float shell = vfxShell(sd, max(thick, 0.012));
    /* Thin-film bias: brighter near the silhouette rim (sparse-LED readable). */
    float rim_bias = vfxSoftBand(abs(sd), max(thick * 1.8, 0.018)) * 0.35;
    float soap = vfxSaturate(shell + rim_bias * shell);

    /* Interior mist — real bubbles are mostly hollow; keep this quiet. */
    float inside = 1.0 - smoothstep(-thick, r * 0.15, sd);
    float mist = inside * film01 * 0.28 * (0.65 + 0.35 * (1.0 - length(q) / max(r, 1e-4)));

    /* Specular: fixed “light from above-front” so each bubble gets a hot spot. */
    float highlight = 0.0;
    if(gloss01 > 0.01)
    {
        vec3 n = q / max(length(q), 1e-4);
        vec3 light = normalize(vec3(0.35, 0.85, 0.25));
        float spec = pow(vfxSaturate(dot(n, light)), mix(18.0, 48.0, gloss01));
        /* Only on / near the shell, not the hollow core. */
        float on_film = vfxSoftBand(abs(sd), max(thick * 2.2, 0.02));
        highlight = spec * on_film * gloss01 * 1.35;
    }

    return vfxSaturate(soap + mist + highlight);
}
void volumeMain(out vec4 out_color, in vec3 p01)
{
    float time_sec = u_params[0];
    int count = int(clamp(u_params[1], 6.0, 48.0) + 0.5);
    float thick = max(u_params[2], 0.0);
    float rise_rate = max(u_params[3], 0.0);
    float interval = max(u_params[4], 0.12);
    float max_r = max(u_params[5], 0.05);
    float fill = clamp(u_params[6], 0.5, 1.8);
    float launch_jitter = clamp(u_params[7], 0.0, 1.0);
    float hue_scroll = fract(u_params[8]);
    float spacing = clamp(u_params[9], 0.10, 1.0);
    float gloss01 = clamp(u_params[10], 0.0, 1.0);
    float film01 = clamp(u_params[11], 0.0, 1.0);
    float wobble01 = clamp(u_params[12], 0.0, 1.0);
    float thick01 = clamp(u_params[13], 0.0, 1.0);
    float detail = clamp(u_params[14], 0.05, 1.0);

    /* Film thickness 0 = off (works even before a plugin rebuild of bubble_thick). */
    if(thick01 < 0.01 || thick < 1e-4)
    {
        out_color = vec4(0.0);
        return;
    }

    float intensity = 0.0;
    float hue01 = hue_scroll;
    const float golden = 2.39996323;
    float voxel = 2.0 / max(u_res, 8.0);
    float soft = max(thick * mix(1.15, 0.75, detail), max(voxel, 0.016));

    for(int i = 0; i < 48; i++)
    {
        float fi = float(i);
        float slot_on = 1.0 - step(float(count), fi);
        float seed = fi * 265.443 + 101.390;
        float cycle_mul = (1.0 + 0.35 * launch_jitter) + (2.0 * launch_jitter) * buHash(seed, 11.0);
        float active_frac = (0.55 - 0.24 * launch_jitter) + (0.06 + 0.22 * launch_jitter) * buHash(seed, 12.0);
        float cycle_i = max(0.12, interval * cycle_mul);
        float active_window = max(0.05, cycle_i * active_frac);
        float offset_i = cycle_i * buHash(seed, 13.0);
        float phase_i = mod(time_sec * rise_rate + offset_i, cycle_i);
        float in_window = 1.0 - step(active_window, phase_i);

        float radius_phase = phase_i / active_window;
        /* Birth / pop envelope — soft open, quick pop at the top. */
        float grow = smoothstep(0.0, 0.14, radius_phase) * (1.0 - smoothstep(0.78, 1.0, radius_phase));
        float pop = 1.0 - 0.35 * smoothstep(0.88, 1.0, radius_phase);
        float radius = (0.18 + 0.82 * pow(radius_phase, 0.85)) * max_r;
        float ring = sqrt((fi + 0.5) / float(count));
        float ang = fi * golden;
        float spread = 0.78 * fill * (0.55 + 0.45 * spacing);
        vec3 c;
        c.x = 0.5 + cos(ang) * ring * spread;
        c.y = radius_phase;
        c.z = 0.5 + sin(ang) * ring * spread;
        /* Drift while rising — soap bubbles wander, not straight columns. */
        float drift = 0.025 + 0.045 * launch_jitter;
        c.x += sin(time_sec * 0.55 + seed + radius_phase * 3.5) * drift;
        c.z += cos(time_sec * 0.48 + seed * 1.3 + radius_phase * 2.8) * drift;
        c = clamp(c, vec3(0.0), vec3(1.0));

        vec3 dlt = p01 - c;
        float value = buSoap(dlt, radius, soft * (0.80 + 0.40 * radius / max(max_r, 1e-3)),
                             gloss01, film01, wobble01, radius_phase, seed);
        value *= slot_on * in_window * grow * pop;

        /* Tiny rise wake under the bubble (0 when film/gloss already carry the look). */
        float wake = 0.0;
        if(gloss01 > 0.01 || film01 > 0.01)
        {
            vec3 wake_d = dlt - vec3(0.0, radius * 0.55, 0.0);
            wake = vfxStreak(wake_d, vec3(0.0, -1.0, 0.0), radius * 0.85, radius * 0.22)
                 * 0.22 * grow * (0.35 + 0.65 * gloss01);
        }
        value = vfxSaturate(max(value, wake));

        if(value > intensity)
        {
            intensity = value;
            /* Iridescent bias around the shell — hue walks with angle + phase. */
            float ang_h = atan(dlt.z, dlt.x) * 0.1591549;
            hue01 = fract(fi * 0.111 + hue_scroll + 0.08 * radius_phase + 0.12 * ang_h * gloss01);
        }
    }

    /* Finish gloss (.b) — must not redeclare slider gloss01 (GLSL compile fail → black). */
    float finish_gloss = clamp(max(gloss01, pow(clamp(intensity, 0.0, 1.0), 1.35) * 0.92), 0.0, 1.0);
    out_color = vec4(clamp(intensity, 0.0, 1.0), hue01, finish_gloss, 1.0);
}
