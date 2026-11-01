# Name
name: Space
class: Starfield
category: Volume
description: Stars, streaks, and tunnels that fill a sparse LED room
global: speed brightness frequency detail size scale fps color surface position strip bands count
pattern_label: Mode:
pattern_key: mode
pattern: Stars
pattern: Twinkle
pattern: Warp
pattern: Hyperdrive
pattern: Blackhole
pattern: Wormhole
pattern: Point Tunnel
pattern: Fibonacci Sphere
pattern: Crystal
pattern: Plasma Globe
slider: star_size 8 140 48 unit pct | Feature size: | Star / streak / wall thickness
slider: fill_amount 40 100 100 unit pct | Field of view: | Zoom of the virtual camera (100 = full room)
slider: drift_amount 0 100 22 unit pct | Sway: | Camera / field drift amount
slider: twinkle_speed 0 100 35 unit pct | Sparkle: | Twinkle / arc pulse rate
param: progress_raw
param: anim_time
param: pattern
param: gcount 16 48
param: unit star_size
param: size_star
param: unit fill_amount
param: unit drift_amount
param: unit twinkle_speed
param: hue_scroll1
finish: hex

# Effect
float hash11(float n)
{
    return fract(sin(n * 127.1) * 43758.5453);
}
float hash_signed(float n)
{
    return hash11(n) * 2.0 - 1.0;
}
float softband(float d, float s)
{
    float ss = max(s, 0.022);
    return exp(-(d * d) / (ss * ss));
}
/* Fat core + readable halo - sized for sparse device grids. */
float sfPoint(float d, float soft)
{
    float s = max(soft, 0.028);
    float core = exp(-(d * d) / (s * s * 0.32));
    float halo = exp(-(d * d) / (s * s * 1.75)) * 0.42;
    return clamp(core + halo, 0.0, 1.0);
}
float sfCross(vec3 dlt, float soft, float rot)
{
    float s = max(soft, 0.026);
    float c = cos(rot);
    float sn = sin(rot);
    vec3 q = vec3(c * dlt.x + sn * dlt.z, dlt.y, -sn * dlt.x + c * dlt.z);
    float arm_w = s * 0.38;
    float arm_l = s * 2.55;
    float plus = max(
        exp(-(q.x * q.x) / (arm_w * arm_w)) * exp(-(q.y * q.y + q.z * q.z) / (arm_l * arm_l)),
        exp(-(q.y * q.y) / (arm_w * arm_w)) * exp(-(q.x * q.x + q.z * q.z) / (arm_l * arm_l)));
    float c45 = 0.70710678;
    vec3 r = vec3(c45 * (q.x - q.z), q.y, c45 * (q.x + q.z));
    float xarm = max(
        exp(-(r.x * r.x) / (arm_w * arm_w)) * exp(-(r.y * r.y + r.z * r.z) / (arm_l * arm_l)),
        exp(-(r.z * r.z) / (arm_w * arm_w)) * exp(-(r.x * r.x + r.y * r.y) / (arm_l * arm_l)));
    return clamp(max(plus, xarm), 0.0, 1.0);
}
float sfStreak(vec3 dlt, vec3 dir, float soft_along, float soft_side)
{
    float dn = length(dir);
    vec3 u = (dn > 1e-4) ? (dir / dn) : vec3(0.0, 0.0, 1.0);
    float along = dot(dlt, u);
    float side = length(dlt - u * along);
    float a = max(soft_along, 0.028);
    float b = max(soft_side, 0.018);
    float head = exp(-(along * along) / (a * a * 0.55));
    float trail = exp(-(along * along) / (a * a * 2.8)) * step(along, 0.0);
    return clamp(max(head, trail * 0.60) * exp(-(side * side) / (b * b)), 0.0, 1.0);
}
vec2 tunnelPath(float x, float sway)
{
    float TAU = 6.2831853;
    vec2 offs = vec2(0.0);
    offs.x = 0.2 * sin(TAU * x * 0.5) + 0.4 * sin(TAU * x * 0.2 + 0.3);
    offs.y = 0.3 * cos(TAU * x * 0.3) + 0.2 * cos(TAU * x * 0.1);
    offs *= smoothstep(1.0, 4.0, abs(x) + 1.0);
    offs *= (0.2 + 0.8 * clamp(sway, 0.0, 1.0));
    return offs;
}
vec3 sfRotX(vec3 p, float a)
{
    float c = cos(a);
    float s = sin(a);
    return vec3(p.x, c * p.y - s * p.z, s * p.y + c * p.z);
}
vec3 sfRotY(vec3 p, float a)
{
    float c = cos(a);
    float s = sin(a);
    return vec3(c * p.x + s * p.z, p.y, -s * p.x + c * p.z);
}
vec3 sfRotZ(vec3 p, float a)
{
    float c = cos(a);
    float s = sin(a);
    return vec3(c * p.x - s * p.y, s * p.x + c * p.y, p.z);
}
void volumeMain(out vec4 out_color, in vec3 p01)
{
    /* progress kept for phase flavour; continuous motion uses anim_time (u_params[1]). */
    float progress = u_params[0];
    float time_sec = u_params[1];
    int mode = int(clamp(u_params[2], 0.0, 9.0) + 0.5);
    int count = int(clamp(u_params[3], 12.0, 48.0) + 0.5);
    float thickness = max(u_params[4], 0.03);
    float size_m = clamp(u_params[5], 0.35, 2.5);
    float fill = clamp(u_params[6], 0.4, 1.0);
    float drift = clamp(u_params[7], 0.0, 1.0);
    float twinkle = clamp(u_params[8], 0.0, 1.0);
    float hue_scroll = u_params[9];

    float fov = 2.0 / max(fill, 0.4);
    vec3 led = (p01 - vec3(0.5)) * fov;
    float sway = drift * 0.22;
    float sway_c = cos(time_sec * 0.35 * max(sway, 0.02) * 6.0 + 0.4);
    float sway_s = sin(time_sec * 0.28 * max(sway, 0.02) * 6.0);
    float led_vx = led.x * (1.0 + sway * 0.08 * sway_c) - led.y * sway * 0.12 * sway_s;
    float led_vy = led.y * (1.0 + sway * 0.08 * sway_c) + led.x * sway * 0.12 * sway_s;
    float led_vz = led.z;
    vec3 led_p = vec3(led_vx, led_vy, led_vz);
    float radial = length(vec2(led_vx, led_vy));
    float ang = atan(led_vy, led_vx);
    float voxel = 2.0 / max(u_res, 8.0);
    float soft_floor = max(voxel * 1.85, 0.030);
    float span = 0.96 * (0.72 + 0.48 * size_m);

    float intensity = 0.0;
    float palette01 = 0.5;
    float hotness = 0.0;

    if(mode == 4)
    {
        /* Blackhole - accretion disk + photon ring + jets (room-scaled). */
        float r_xy = max(radial, 1e-4);
        float horizon = 0.16 * (0.85 + 0.55 * size_m);
        float disk_outer = span * 1.02;
        float disk_thick = max(0.070, max(thickness * 0.85 * size_m, soft_floor));
        float in_horizon = step(r_xy, horizon) * step(-0.40 * size_m, led_vz) * step(led_vz, 0.40 * size_m);

        float plane = exp(-(led_vz * led_vz) / (disk_thick * disk_thick * 3.0));
        float disk = plane * smoothstep(horizon * 1.02, horizon * 1.45, r_xy)
                           * (1.0 - smoothstep(disk_outer * 0.78, disk_outer, r_xy));
        float clock = time_sec * (0.55 + 0.85 * drift);
        float spiral = 0.5 + 0.5 * sin(5.0 * ang - log(r_xy + 0.02) * 4.2 - clock * 3.2);
        float arms = 0.5 + 0.5 * sin(2.0 * ang - log(r_xy + 0.02) * 2.1 - clock * 1.6);
        float photon = exp(-pow((r_xy - horizon * 1.28) / max(0.045 + 0.05 * thickness, soft_floor), 2.0));
        float lens = exp(-pow((r_xy - horizon) / 0.12, 2.0)) * 0.60;
        float jet = exp(-(r_xy * r_xy) / max(0.05 * size_m, 0.025))
                  * exp(-pow(abs(led_vz) - disk_outer * 0.55, 2.0) / max(0.22, 0.10 * size_m))
                  * 0.42;

        intensity = disk * (0.30 + 0.45 * spiral + 0.30 * arms) + photon * 1.70 + lens + jet;
        intensity *= 1.0 - in_horizon;
        intensity *= 1.0 - smoothstep(1.20, 1.65, length(vec2(led_vx, led_vy)) / max(fov * 0.5, 1.0));
        intensity = clamp(intensity * 1.15, 0.0, 1.0);
        palette01 = fract(1.0 - r_xy / max(disk_outer, 0.1) + hue_scroll);
        hotness = clamp(1.0 - (r_xy - horizon) / max(disk_outer * 0.55, 0.2), 0.0, 1.0) * 0.60 + photon * 0.45;
    }
    else if(mode == 5)
    {
        /* Wormhole - thick tunnel wall + rings + helix. */
        float depth = clamp(led_vz * 0.5 + 0.5, 0.0, 1.0);
        float tunnel_r = 0.58 * (0.75 + 0.55 * size_m);
        float wall_w = max(0.060, max(thickness * 0.75 * size_m, soft_floor));
        float wall = exp(-pow((radial - tunnel_r) / wall_w, 2.0));
        float clock = time_sec * (0.40 + 0.70 * drift);
        float rings = 0.5 + 0.5 * cos((depth * 6.5 + clock * 0.85) * 6.2831853);
        float helix = 0.5 + 0.5 * cos(7.0 * ang + depth * (6.0 + drift * 5.0) - clock * 5.5);
        float perspective = 0.35 + 0.65 * (1.0 - depth * 0.85);
        float core_glow = (1.0 - smoothstep(0.0, tunnel_r * 0.85, radial)) * 0.18
                          * (0.40 + 0.60 * rings) * perspective;
        float mouth = exp(-pow(abs(led_vz) - 0.92 * span / max(fov * 0.5, 1.0), 2.0) / 0.14)
                    * smoothstep(tunnel_r * 0.65, tunnel_r * 1.20, radial) * 0.50;

        intensity = wall * (0.35 + 0.35 * rings + 0.55 * helix) * perspective + core_glow + mouth;
        if(radial > tunnel_r + wall_w * 3.2)
            intensity *= 0.0;
        intensity = clamp(intensity * 1.35, 0.0, 1.0);
        palette01 = fract(depth + ang * 0.08 + hue_scroll);
        hotness = depth * 0.40 + mouth * 0.35;
    }
    else if(mode == 6)
    {
        /* Point Tunnel - fat ring dots scrolling on continuous time. */
        float TAU = 6.2831853;
        float cam_z = time_sec * (0.55 + 0.55 * drift);
        float n_pts = max(float(count), 16.0);
        float rep = TAU / n_pts;
        float point_sz = max(0.048, max(thickness * 0.62 * size_m, soft_floor));
        float z_sig = max(0.16, point_sz * 3.6 + 0.12 * size_m);
        float best = 0.0;
        float best_shade = 0.0;
        float spin = cam_z * 0.35;
        for(int i = 1; i <= 24; i++)
        {
            float fi = float(i);
            float layers = 24.0;
            float pz = 1.0 - (fi / layers);
            pz -= mod(cam_z, 4.0 / layers);
            float pz_draw = pz;
            if(pz_draw < 0.05)
                pz_draw += 4.0 / layers;
            if(pz_draw > 0.03 && pz_draw < 1.2)
            {
                float path_t = cam_z + pz_draw;
                vec2 cam_offs = tunnelPath(cam_z, drift);
                vec2 offs = tunnelPath(path_t, drift) - cam_offs;
                float den = pz_draw * 0.8 + 0.4;
                float ring_rad = 0.22 * (1.0 / (den * den)) * (0.80 + 0.70 * size_m);
                float ring_z = pz_draw * 2.0 - 1.0;
                vec2 p = vec2(led_vx, led_vy) + offs;
                float pa = atan(p.y, p.x) + spin;
                float slot = floor(pa / rep + 0.5);
                float a0 = slot * rep;
                vec2 nearest = vec2(cos(a0), sin(a0)) * ring_rad;
                float pdist = length(p - nearest);
                float zdist = abs(led_vz - ring_z);
                float shade = clamp(1.0 - pz_draw, 0.08, 1.0);
                float glow = softband(pdist, point_sz) * softband(zdist, z_sig);
                float stripe = (mod(floor(fi * 0.5), 2.0) < 0.5) ? 1.0 : 0.60;
                float contrib = glow * shade * stripe;
                if(contrib > best)
                {
                    best = contrib;
                    best_shade = shade;
                }
            }
        }
        intensity = clamp(best * 2.05, 0.0, 1.0);
        palette01 = fract(best_shade + hue_scroll);
        hotness = clamp(best_shade * best_shade * 0.85, 0.0, 1.0);
        if(twinkle > 0.01)
        {
            float shimmer = 0.78 + 0.22 * sin(time_sec * (2.2 + twinkle * 4.5) + ang * 8.0 + cam_z);
            intensity *= mix(1.0, shimmer, twinkle);
        }
    }
    else if(mode == 7)
    {
        /* Fibonacci Sphere - fat rotating shell points. */
        float golden = 2.39996323;
        float n = max(float(count), 16.0);
        float rad = span;
        float point_sz = max(0.050, max(thickness * 0.62 * size_m, soft_floor));
        float tumble = 0.35 + 1.4 * drift;
        float ax = time_sec * tumble * 0.55;
        float ay = time_sec * tumble * 0.72 + progress * 0.15;
        float best = 0.0;
        float best_y = 0.0;
        for(int i = 0; i < 48; i++)
        {
            float fi = float(i);
            float slot_on = 1.0 - step(float(count), fi);
            float y = 1.0 - (fi + 0.5) / n * 2.0;
            float rr = sqrt(max(0.0, 1.0 - y * y));
            float th = fi * golden;
            vec3 pt = vec3(cos(th) * rr, y, sin(th) * rr) * rad;
            pt = sfRotY(sfRotX(pt, ax), ay);
            float d = length(led_p - pt);
            float glow = softband(d, point_sz) * slot_on;
            if(glow > best)
            {
                best = glow;
                best_y = y * 0.5 + 0.5;
            }
        }
        intensity = clamp(best * 1.95, 0.0, 1.0);
        palette01 = fract(best_y + hue_scroll);
        hotness = clamp(best * 0.55, 0.0, 1.0);
        if(twinkle > 0.01)
            intensity *= 0.82 + 0.18 * sin(time_sec * (2.6 + twinkle * 3.8) + best_y * 9.0);
    }
    else if(mode == 8)
    {
        /* Crystal - octahedron lattice with fat vertices. */
        float n = max(float(count), 16.0);
        float rad = span;
        float point_sz = max(0.048, max(thickness * 0.60 * size_m, soft_floor));
        float tumble = 0.4 + 1.5 * drift;
        float ax = time_sec * tumble * 0.70;
        float ay = time_sec * tumble * 0.50;
        float az = time_sec * tumble * 0.38;
        float best = 0.0;
        float best_h = 0.0;
        for(int i = 0; i < 48; i++)
        {
            float fi = float(i);
            float slot_on = 1.0 - step(float(count), fi);
            float y = 1.0 - (fi + 0.5) / n * 2.0;
            float ring = max(0.08, 1.0 - abs(y));
            float th = fi * 2.39996323 + floor(fi * 0.25) * 0.785398;
            vec3 pt = normalize(vec3(cos(th) * ring, y, sin(th) * ring)) * rad;
            float l1 = abs(pt.x) + abs(pt.y) + abs(pt.z);
            pt *= (rad * 1.05) / max(l1, 1e-3);
            pt = sfRotZ(sfRotY(sfRotX(pt, ax), ay), az);
            float d = length(led_p - pt);
            float glow = softband(d, point_sz) * slot_on;
            float edge = softband(d, point_sz * 2.4) * 0.28 * slot_on;
            float contrib = max(glow, edge);
            if(contrib > best)
            {
                best = contrib;
                best_h = fract(fi / n + abs(y) * 0.35);
            }
        }
        intensity = clamp(best * 1.90, 0.0, 1.0);
        palette01 = fract(best_h + hue_scroll);
        hotness = clamp(best * 0.5, 0.0, 1.0);
    }
    else if(mode == 9)
    {
        /* Plasma Globe - glass shell + fat lightning arcs. */
        float R = span;
        float rr = length(led_p);
        float shell_w = max(0.055, max(thickness * 0.58 * size_m, soft_floor));
        float shell = softband(rr - R, shell_w);
        float glass = pow(1.0 - clamp(abs(rr - R) / max(R * 0.20, 0.05), 0.0, 1.0), 2.0) * 0.55;
        float core = exp(-(rr * rr) / max(0.06 * size_m, 0.030)) * 0.28;
        int arc_n = int(clamp(float(count) * 0.40, 5.0, 14.0) + 0.5);
        float arcs = 0.0;
        float hot_arc = 0.0;
        float spin = time_sec * (0.35 + 0.85 * drift);
        for(int i = 0; i < 14; i++)
        {
            float fi = float(i);
            float slot_on = 1.0 - step(float(arc_n), fi);
            vec3 axv = normalize(vec3(
                hash_signed(fi * 3.1 + 1.0),
                hash_signed(fi * 5.7 + 2.0),
                hash_signed(fi * 7.3 + 3.0)));
            axv = sfRotY(sfRotX(axv, spin * 0.7 + fi * 0.4), spin * 1.1);
            vec3 pdir = led_p / max(rr, 1e-3);
            float eq = abs(dot(pdir, axv));
            float bolt_w = 0.065 + 0.070 * thickness;
            float on_shell = softband(rr - R, shell_w * 1.8);
            float pulse = 0.50 + 0.50 * sin(time_sec * (2.0 + twinkle * 3.2) + fi * 2.3);
            float jag = 0.5 + 0.5 * sin(dot(pdir, axv.yzx) * 12.0 + time_sec * 4.5 + fi);
            float bolt = softband(eq, bolt_w) * on_shell * pulse * (0.45 + 0.55 * jag) * slot_on;
            arcs = max(arcs, bolt);
            hot_arc = max(hot_arc, bolt * pulse);
        }
        intensity = clamp(glass * 0.55 + shell * 0.30 + arcs * 1.75 + core, 0.0, 1.0);
        palette01 = fract(rr / max(R, 0.1) + hue_scroll + arcs * 0.15);
        hotness = clamp(hot_arc * 0.9 + core * 0.5, 0.0, 1.0);
    }
    else if(mode == 0 || mode == 1)
    {
        /* Stars / Twinkle - fat volumetric points; continuous fly-through on time. */
        float soft = max(0.040, max(thickness * 0.70 * size_m, soft_floor));
        float travel = time_sec * ((mode == 0) ? 0.18 : 0.04);
        float best = 0.0;
        float best_h = 0.0;
        float best_hot = 0.0;
        float golden = 2.39996323;
        for(int i = 0; i < 48; i++)
        {
            float fi = float(i);
            float slot_on = 1.0 - step(float(count), fi);
            /* Stratified fill: golden spiral in XZ + hashed height (fewer empty pockets). */
            float ring = sqrt((fi + 0.5) / max(float(count), 1.0));
            float th = fi * golden;
            float hx = 0.5 + cos(th) * ring * 0.48;
            float hz = 0.5 + sin(th) * ring * 0.48;
            float hy = hash11(fi * 78.233 + 22.0);
            hx = mix(hx, hash11(fi * 12.9898 + 11.0), 0.35);
            hz = mix(hz, hash11(fi * 45.164 + 33.0), 0.35);
            vec3 c = vec3(hx, hy, hz) * 2.0 - 1.0;
            c *= span;
            float z01 = fract(hz - travel + drift * 0.015 * sin(time_sec * 0.7 + fi));
            c.z = (z01 * 2.0 - 1.0) * span;
            c.x += sin(time_sec * 0.32 + fi) * sway * 0.10 * span;
            c.y += cos(time_sec * 0.26 + fi * 1.3) * sway * 0.10 * span;

            vec3 dlt = led_p - c;
            float d = length(dlt);
            float glow = sfPoint(d, soft);
            float mag = 0.60 + 0.40 * hash11(fi * 3.7 + 40.0);
            float bright = mag;

            if(mode == 1)
            {
                float rate = 0.85 + twinkle * 3.8 + hash11(fi + 35.0) * 2.2;
                float ph = time_sec * rate + hz * 6.2831853;
                /* Wider duty so sparse rooms still catch flashes. */
                float flash = pow(max(0.0, sin(ph)), 4.5);
                float hold = smoothstep(0.15, 0.55, flash);
                bright = 0.18 + 0.82 * hold;
                float crossv = sfCross(dlt, soft * 1.05, fi * 1.7) * hold;
                glow = max(glow * (0.40 + 0.60 * hold), crossv * 1.25);
            }
            else if(twinkle > 0.01)
            {
                float ph = time_sec * (1.1 + twinkle * 2.2) + fi;
                bright *= 0.72 + 0.28 * pow(max(0.0, sin(ph)), 3.5);
            }

            float contrib = glow * bright * slot_on;
            if(contrib > best)
            {
                best = contrib;
                best_h = fract(hx * 0.7 + hy * 0.3 + hue_scroll);
                best_hot = (mode == 1) ? clamp((bright - 0.35) * 1.2, 0.0, 1.0) : (1.0 - z01) * 0.35;
            }
        }
        intensity = clamp(best * 2.05, 0.0, 1.0);
        palette01 = best_h;
        hotness = best_hot;
    }
    else
    {
        /* Warp / Hyperdrive - continuous fly-through streaks (no progress wrap flash). */
        float soft = max(0.038, max(thickness * 0.65 * size_m, soft_floor));
        float travel = time_sec * ((mode == 3) ? 0.62 : 0.38);
        float streak_mul = (mode == 3) ? (14.0 + 16.0 * size_m) : (8.0 + 10.0 * size_m);
        float best = 0.0;
        float best_h = 0.0;
        float best_hot = 0.0;
        for(int i = 0; i < 48; i++)
        {
            float fi = float(i);
            float slot_on = 1.0 - step(float(count), fi);
            float dir_x = hash_signed(fi * 12.9898 + 31.0);
            float dir_y = hash_signed(fi * 78.233 + 32.0);
            float dir_len = length(vec2(dir_x, dir_y)) + 1e-4;
            float ux = dir_x / dir_len;
            float uy = dir_y / dir_len;
            float aim = 0.18 + 1.10 * hash11(fi * 45.1 + 33.0);
            float seed_d = hash11(fi * 91.7 + 34.0);

            float depth = fract(seed_d - travel);
            float nearness = 1.0 - depth;
            float persp = 1.0 / max(0.05, 0.06 + depth * 0.94);
            float px = ux * aim * persp * span * 0.95;
            float py = uy * aim * persp * span * 0.95;
            float pz = (depth * 2.0 - 1.0) * span;

            vec3 c = vec3(px, py, pz);
            vec3 dlt = led_p - c;
            vec3 motion = normalize(vec3(px * 0.15, py * 0.15, -1.0) + vec3(ux, uy, 0.0) * 0.05);
            float along_s = soft * (1.0 + nearness * streak_mul * 0.14);
            float side_s = soft * ((mode == 3) ? 0.70 : 0.85);
            float streak = sfStreak(dlt, motion, along_s, side_s);
            float core = sfPoint(length(dlt), soft * 0.95);
            float bright = 0.28 + 0.95 * nearness;
            if(mode == 3)
                bright = 0.22 + 1.20 * nearness;

            float contrib = max(streak, core * 0.90) * bright * slot_on;
            contrib *= 1.0 - smoothstep(1.40, 1.90, length(vec2(led_vx, led_vy)) / max(span, 0.5));

            if(contrib > best)
            {
                best = contrib;
                best_h = fract(nearness * 0.65 + hue_scroll + fi * 0.02);
                best_hot = clamp(nearness * ((mode == 3) ? 0.85 : 0.50), 0.0, 1.0);
            }
        }
        intensity = clamp(best * ((mode == 3) ? 2.10 : 1.90), 0.0, 1.0);
        palette01 = best_h;
        hotness = best_hot;
    }

    out_color = vec4(clamp(intensity, 0.0, 1.0), fract(palette01), clamp(hotness, 0.0, 1.0), 1.0);
}
