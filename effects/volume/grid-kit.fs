# Name
name: Grid Kit
class: GridKit
category: Volume
description: Animated planes, wire cubes, roaming boxes, and bolts for sparse LED rooms
global: speed brightness frequency detail size scale fps color surface position strip bands path thickness
pattern_label: Mode:
pattern_key: mode
pattern: Plane Sweep
pattern: Wireframe Box
pattern: Moving Boxes
pattern: Rubik Cube
pattern: Sphere Roam
pattern: Axis Send
pattern: Lightning
slider: density 20 100 70 unit pct | Density: | Feature count / fill / scramble amount
slider: sway 0 150 70 unit pct | Sway: | Wobble, orbit spread, bolt jag
param: pattern
param: motion_clock
param: gthickness
param: size_star
param: path_axis
param: ndetail
param: hue_scroll_s
param: unit sway
param: unit density
param: freq_n
param: phase37
finish: hex

# Effect
/* Globals → params:
 * Speed → motion_clock (0 stops). Frequency → freq_n + hue_scroll_s.
 * Detail → ndetail (edge sharpness). Size → size_star. Thickness → gthickness.
 * Path → path_axis. Scale = occupancy (engine atlas, not packed here). */
float softband(float d, float w)
{
    float hw = max(w, 0.024);
    float core = exp(-(d * d) / (hw * hw * 0.50));
    float halo = exp(-(d * d) / (hw * hw * 1.75)) * 0.40;
    return clamp(core + halo, 0.0, 1.0);
}
float hash21(vec2 p)
{
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}
vec3 gkRotY(vec3 p, float a)
{
    float c = cos(a);
    float s = sin(a);
    return vec3(c * p.x + s * p.z, p.y, -s * p.x + c * p.z);
}
vec3 gkRotX(vec3 p, float a)
{
    float c = cos(a);
    float s = sin(a);
    return vec3(p.x, c * p.y - s * p.z, s * p.y + c * p.z);
}
vec3 gkRotZ(vec3 p, float a)
{
    float c = cos(a);
    float s = sin(a);
    return vec3(c * p.x - s * p.y, s * p.x + c * p.y, p.z);
}
float boxEdgeGlow(vec3 p, vec3 b, float edge_w)
{
    float near_x = softband(abs(p.x) - b.x, edge_w);
    float near_y = softband(abs(p.y) - b.y, edge_w);
    float near_z = softband(abs(p.z) - b.z, edge_w);
    float inside = step(abs(p.x), b.x + edge_w)
                 * step(abs(p.y), b.y + edge_w)
                 * step(abs(p.z), b.z + edge_w);
    return max(near_x * near_y, max(near_y * near_z, near_z * near_x)) * inside;
}
float facePlane(vec3 p, vec3 b, float edge_w, int face)
{
    float d = 0.0;
    float mask = 1.0;
    if(face == 0)
    {
        d = p.x - b.x;
        mask = step(abs(p.y), b.y + edge_w) * step(abs(p.z), b.z + edge_w);
    }
    else if(face == 1)
    {
        d = -p.x - b.x;
        mask = step(abs(p.y), b.y + edge_w) * step(abs(p.z), b.z + edge_w);
    }
    else if(face == 2)
    {
        d = p.y - b.y;
        mask = step(abs(p.x), b.x + edge_w) * step(abs(p.z), b.z + edge_w);
    }
    else if(face == 3)
    {
        d = -p.y - b.y;
        mask = step(abs(p.x), b.x + edge_w) * step(abs(p.z), b.z + edge_w);
    }
    else if(face == 4)
    {
        d = p.z - b.z;
        mask = step(abs(p.x), b.x + edge_w) * step(abs(p.y), b.y + edge_w);
    }
    else
    {
        d = -p.z - b.z;
        mask = step(abs(p.x), b.x + edge_w) * step(abs(p.y), b.y + edge_w);
    }
    return softband(d, edge_w) * mask;
}
int nearestFace(vec3 p, vec3 b)
{
    float dxp = abs(p.x - b.x);
    float dxm = abs(p.x + b.x);
    float dyp = abs(p.y - b.y);
    float dym = abs(p.y + b.y);
    float dzp = abs(p.z - b.z);
    float dzm = abs(p.z + b.z);
    float best = dxp;
    int face = 0;
    if(dxm < best) { best = dxm; face = 1; }
    if(dyp < best) { best = dyp; face = 2; }
    if(dym < best) { best = dym; face = 3; }
    if(dzp < best) { best = dzp; face = 4; }
    if(dzm < best) { best = dzm; face = 5; }
    return face;
}
/* Twist one Rubik layer around face axis. soft_w>0.5 = feathered anim edge. */
vec3 gkTwistLayer(vec3 q, int face, float layer, float ang, float cell, float soft_w)
{
    float lc = q.x;
    if(face >= 2 && face < 4)
        lc = q.y;
    else if(face >= 4)
        lc = q.z;
    float target = layer * cell;
    float on = step(abs(lc - target), cell * 0.52);
    if(soft_w > 0.5)
        on = 1.0 - smoothstep(cell * 0.40, cell * 0.58, abs(lc - target));
    float sgn = (mod(float(face), 2.0) < 0.5) ? 1.0 : -1.0;
    vec3 r = q;
    if(face < 2)
        r = gkRotX(q, ang * sgn);
    else if(face < 4)
        r = gkRotY(q, ang * sgn);
    else
        r = gkRotZ(q, ang * sgn);
    return mix(q, r, on);
}
void gkRubikMove(float mid, float cycle_id, out int face, out float layer, out float dir)
{
    face = int(floor(hash21(vec2(mid + 0.17, cycle_id * 0.13 + 1.1)) * 6.0));
    layer = floor(hash21(vec2(mid + 0.31, cycle_id * 0.17 + 2.2)) * 3.0) - 1.0;
    dir = (hash21(vec2(mid + 0.53, cycle_id * 0.19 + 3.3)) > 0.5) ? 1.0 : -1.0;
}
void volumeMain(out vec4 out_color, in vec3 p01)
{
    int mode = int(clamp(u_params[0], 0.0, 6.0) + 0.5);
    /* motion_clock = time * GetMotionHz() * speed_mul — Speed 0 freezes. */
    float clock = u_params[1];
    float thick = max(u_params[2], 0.01);
    float size_m = clamp(u_params[3], 0.35, 2.5);
    int axis = int(clamp(u_params[4], 0.0, 2.0) + 0.5);
    float detail = clamp(u_params[5], 0.05, 1.0);
    float hue_scroll = fract(u_params[6]);
    float sway = clamp(u_params[7], 0.0, 1.5);
    float density = clamp(u_params[8], 0.15, 1.0);
    float freq_n = clamp(u_params[9], 0.0, 1.0);
    float phase2 = fract(u_params[10]);

    /* Frequency scales how busy motion is (on top of Speed's clock rate). */
    float busy = 0.45 + 0.90 * freq_n;
    float motion = clock * busy;

    vec3 p = (p01 - 0.5) * 2.0;
    /* Thickness maps more strongly so the global slider is obvious. */
    float half_w = max(0.022, thick * mix(1.35, 0.55, detail));
    float intensity = 0.0;
    float color_drv = hue_scroll;

    if(mode == 0)
    {
        /* Plane Sweep — Speed/Freq drive sweep; Density = crests; Sway = wobble; Path = axis. */
        float progress = fract(motion * (0.55 + 0.35 * density) + phase2 * 0.05);
        float tri = 1.0 - abs(2.0 * progress - 1.0);
        float span = 0.92 * clamp(size_m, 0.55, 1.90);
        float center = (tri * 2.0 - 1.0) * span;
        float wob = sin(motion * (1.10 + 0.80 * sway) + phase2 * 6.2831853) * sway * 0.14;
        float coord = (axis == 0) ? p.x : ((axis == 1) ? p.y : p.z);
        float lat0 = (axis == 0) ? p.y : p.x;
        float lat1 = (axis == 2) ? p.y : p.z;
        coord += wob * (0.55 + 0.45 * lat0);

        float crests = mix(1.0, 3.0, density);
        float best = 0.0;
        for(int i = 0; i < 3; i++)
        {
            float fi = float(i);
            float slot = 1.0 - step(crests, fi);
            float off = (fi - 0.5 * (crests - 1.0)) * mix(0.18, 0.36, density);
            float c = center + off * span;
            float band = softband(coord - c, half_w * (2.25 - 0.35 * fi));
            float wake = softband(coord - (c - sign(center + 1e-4) * 0.12), half_w * 3.3) * 0.38;
            best = max(best, max(band, wake) * slot * (1.0 - 0.18 * fi));
        }
        float cross = softband(lat0 - sin(motion * 0.85 + center) * 0.38 * sway, half_w * 2.9) * 0.24 * density;
        intensity = max(best, cross);
        color_drv = fract(0.5 + 0.5 * progress + 0.35 * lat0 + 0.20 * lat1 + hue_scroll);
    }
    else if(mode == 1)
    {
        /* Wireframe Box — rotates with Speed; Frequency = spin/hotspot rate; Size = cube. */
        float box_s = 0.72 * clamp(size_m, 0.55, 1.65);
        vec3 b = vec3(box_s);
        float spin = motion * (0.55 + 0.35 * density);
        float tip = motion * (0.35 + 0.40 * sway) + phase2 * 6.2831853;
        vec3 q = p;
        if(axis == 0)
        {
            q = gkRotX(q, spin);
            q = gkRotY(q, tip * 0.45);
        }
        else if(axis == 1)
        {
            q = gkRotY(q, spin);
            q = gkRotX(q, tip * 0.45 * (0.35 + 0.65 * sway));
        }
        else
        {
            q = gkRotZ(q, spin);
            q = gkRotY(q, tip * 0.45);
        }
        float ew = half_w * mix(2.0, 3.0, density);
        float edges = boxEdgeGlow(q, b, ew);
        float faces = 0.0;
        for(int f = 0; f < 6; f++)
            faces = max(faces, facePlane(q, b, ew * 1.15, f));

        int hot = int(mod(floor(motion * (1.10 + 0.70 * density) + phase2 * 3.0), 6.0));
        float hot_face = facePlane(q, b, ew * 1.55, hot);
        float pulse = 0.70 + 0.30 * sin(motion * (2.2 + sway) + float(hot));
        intensity = max(edges * 1.05, faces * 0.28);
        intensity = max(intensity, hot_face * 0.95 * pulse);
        float ax_c = (axis == 0) ? q.x : ((axis == 1) ? q.y : q.z);
        color_drv = fract(atan(q.z, q.x) / 6.2831853 + 0.5 + hue_scroll + 0.10 * ax_c + float(hot) * 0.08);
    }
    else if(mode == 2)
    {
        /* Moving Boxes — wider orbits; Speed/Freq = roam rate; Size = box + orbit. */
        float best = 0.0;
        float best_h = 0.0;
        float n_vis = mix(4.0, 7.0, density);
        float orbit = mix(0.78, 1.05, clamp(sway / 1.5, 0.0, 1.0)) * clamp(size_m, 0.55, 1.75);
        for(int i = 0; i < 7; i++)
        {
            float fi = float(i);
            float slot_on = 1.0 - step(n_vis, fi);
            float seed = fi * 1.37 + 0.3;
            float ax = motion * (0.62 + 0.19 * fi) + seed + phase2 * 1.7;
            float ay = motion * (0.48 + 0.14 * fi) + seed * 0.7;
            float ph = fi * 0.8975979;
            float r_scale = mix(0.72, 1.0, hash21(vec2(seed, 0.4)));
            vec3 c = vec3(
                sin(ax + ph) * orbit * r_scale,
                sin(ay * 1.15 + seed) * (orbit * 0.55) * r_scale,
                cos(ax * 0.92 + ph * 1.3) * orbit * r_scale);
            /* Bias roam plane from Path axis. */
            if(axis == 1)
                c = vec3(c.x, c.z, c.y);
            else if(axis == 2)
                c = vec3(c.z, c.y, c.x);
            float half_box = mix(0.08, 0.15, 0.5 + 0.5 * hash21(vec2(seed, 2.1)))
                           * clamp(size_m, 0.55, 1.45);
            vec3 d = abs(p - c) - vec3(half_box);
            float box_d = length(max(d, 0.0)) + min(max(d.x, max(d.y, d.z)), 0.0);
            float glow = softband(box_d, half_w * 1.70) * slot_on;
            best = max(best, glow);
            if(glow >= best - 1e-5)
                best_h = fract(fi * 0.13 + hue_scroll + motion * 0.03);
        }
        intensity = best;
        color_drv = best_h;
    }
    else if(mode == 3)
    {
        /* Rubik — committed 90° layer turns (no snap-back).
         * Cycle: scramble → hold messy → solve → hold clean → new random scramble…
         * Clock is offset so the look starts already scrambled. Density = move count. */
        float box_s = 0.88 * clamp(size_m, 0.55, 1.45);
        vec3 b = vec3(box_s);
        float cell = max(box_s * 2.0 / 3.0, 0.12);
        float half_pi = 1.5707963;

        float n_moves = floor(mix(6.0, 12.0, density) + 0.5);
        float hold_dur = mix(1.35, 2.10, 1.0 - 0.35 * density);
        float half = n_moves;
        float cycle_len = half + hold_dur + half + hold_dur;
        float clock_m = motion * mix(0.48, 0.95, density) + phase2 * 0.11;
        /* Skip past the first scramble so we open on a messed-up cube. */
        float shifted = clock_m + half;
        float cycle_id = floor(shifted / cycle_len);
        float t = mod(shifted, cycle_len);

        /* Phases: scramble → hold scrambled → solve → hold solved. */
        int n_done = 0;
        int cur_idx = -1;
        float cur_ang = 0.0;
        float sway_n = clamp(sway / 1.5, 0.0, 1.0);

        if(t < half)
        {
            float u = t;
            int step_i = int(min(floor(u), n_moves - 1.0));
            float frac = clamp(u - float(step_i), 0.0, 1.0);
            float e = smoothstep(0.0, 1.0, frac);
            e = e + sin(3.14159265 * e) * 0.10 * sway_n * (1.0 - frac);
            cur_idx = step_i;
            n_done = step_i;
            cur_ang = clamp(e, 0.0, 1.0) * half_pi;
        }
        else if(t < half + hold_dur)
        {
            n_done = int(n_moves);
            cur_idx = -1;
            cur_ang = 0.0;
        }
        else if(t < half + hold_dur + half)
        {
            float u = t - half - hold_dur;
            int step_i = int(min(floor(u), n_moves - 1.0));
            float frac = clamp(u - float(step_i), 0.0, 1.0);
            float e = smoothstep(0.0, 1.0, frac);
            e = e + sin(3.14159265 * e) * 0.10 * sway_n * (1.0 - frac);
            cur_idx = int(n_moves) - 1 - step_i;
            n_done = cur_idx;
            cur_ang = (1.0 - clamp(e, 0.0, 1.0)) * half_pi;
        }
        else
        {
            n_done = 0;
            cur_idx = -1;
            cur_ang = 0.0;
        }

        /* Forward warp for spinning geometry. */
        vec3 q = p;
        for(int i = 0; i < 12; i++)
        {
            float on_i = 1.0 - step(float(n_done), float(i));
            int mf;
            float ml;
            float md;
            gkRubikMove(float(i), cycle_id, mf, ml, md);
            vec3 q2 = gkTwistLayer(q, mf, ml, md * half_pi, cell, 0.0);
            q = mix(q, q2, on_i);
        }
        if(cur_idx >= 0)
        {
            int cf;
            float cl;
            float cd;
            gkRubikMove(float(cur_idx), cycle_id, cf, cl, cd);
            q = gkTwistLayer(q, cf, cl, cd * cur_ang, cell, 1.0);
        }

        /* Inverse map → home facelets for true sticker colors. */
        vec3 h = p;
        if(cur_idx >= 0)
        {
            int cf;
            float cl;
            float cd;
            gkRubikMove(float(cur_idx), cycle_id, cf, cl, cd);
            h = gkTwistLayer(h, cf, cl, -cd * cur_ang, cell, 1.0);
        }
        for(int i = 11; i >= 0; i--)
        {
            float on_i = 1.0 - step(float(n_done), float(i));
            int mf;
            float ml;
            float md;
            gkRubikMove(float(i), cycle_id, mf, ml, md);
            vec3 h2 = gkTwistLayer(h, mf, ml, -md * half_pi, cell, 0.0);
            h = mix(h, h2, on_i);
        }

        vec3 aq = abs(q);
        float m = max(aq.x, max(aq.y, aq.z));
        float shell = softband(m - box_s, half_w * mix(1.5, 2.1, detail));
        float in_face = step(abs(q.x), b.x + half_w * 2.0)
                      * step(abs(q.y), b.y + half_w * 2.0)
                      * step(abs(q.z), b.z + half_w * 2.0);

        int face = nearestFace(h, b);
        vec2 uv;
        if(face == 0 || face == 1)
            uv = h.yz;
        else if(face == 2 || face == 3)
            uv = h.xz;
        else
            uv = h.xy;
        vec2 cell_i = floor((uv + box_s) / cell);
        cell_i = clamp(cell_i, vec2(0.0), vec2(2.0));

        vec2 local = abs(mod(uv + box_s, cell) - cell * 0.5);
        float groove = softband(min(local.x, local.y), half_w * mix(0.45, 0.75, detail));
        float sticker = shell * in_face * (1.0 - groove * 0.80);

        /* Six solid face hues; stickers track through every committed turn. */
        color_drv = (float(face) + 0.5) / 6.0;
        color_drv = fract(color_drv + hue_scroll * 0.12);

        float edges = boxEdgeGlow(q, b, half_w * 1.45);
        intensity = max(sticker * 1.05, edges * 0.70);
        intensity *= 0.90 + 0.10 * (0.5 + 0.5 * sin(motion * 0.35));
    }
    else if(mode == 4)
    {
        float a = motion * (0.75 + 0.45 * density);
        float span = 0.78 * clamp(size_m, 0.55, 1.65);
        vec3 c = vec3(
            sin(a) * 0.72,
            sin(a * 1.3 + phase2 * 6.2831853) * 0.52,
            cos(a * 0.85 + 1.1) * 0.72) * span;
        if(axis == 1)
            c = vec3(c.x, c.z, c.y);
        else if(axis == 2)
            c = vec3(c.z, c.y, c.x);
        float rad = mix(0.20, 0.38, 0.35 + 0.65 * density) * clamp(size_m, 0.55, 1.55);
        float d0 = length(p - c);
        intensity = softband(d0, max(rad * 0.85, half_w));
        for(int i = 1; i <= 4; i++)
        {
            float fi = float(i);
            float a2 = a - fi * 0.28 * (0.55 + 0.45 * sway);
            vec3 c2 = vec3(
                sin(a2) * 0.72,
                sin(a2 * 1.3 + phase2 * 6.2831853) * 0.52,
                cos(a2 * 0.85 + 1.1) * 0.72) * span;
            if(axis == 1)
                c2 = vec3(c2.x, c2.z, c2.y);
            else if(axis == 2)
                c2 = vec3(c2.z, c2.y, c2.x);
            float d2 = length(p - c2);
            float r2 = rad * (1.0 - 0.12 * fi);
            intensity = max(intensity, softband(d2, max(r2 * 0.90, half_w)) * (1.0 - 0.18 * fi));
        }
        color_drv = fract(motion * 0.12 + hue_scroll);
    }
    else if(mode == 5)
    {
        float best = 0.0;
        float best_h = 0.0;
        float n_vis = mix(6.0, 12.0, density);
        float span = 0.95 * clamp(size_m, 0.55, 1.75);
        for(int i = 0; i < 12; i++)
        {
            float fi = float(i);
            float slot_on = 1.0 - step(n_vis, fi);
            float h = hash21(vec2(fi * 0.37, 1.9));
            float h2 = hash21(vec2(fi * 0.91, 4.2));
            float lat0 = (h * 2.0 - 1.0) * 0.82 * span;
            float lat1 = (h2 * 2.0 - 1.0) * 0.82 * span;
            float speed = 0.45 + 0.75 * hash21(vec2(fi, 7.1));
            float pos = (fract(motion * speed * 0.35 + h) * 2.0 - 1.0) * 0.96 * span;
            vec3 c;
            vec3 dir;
            if(axis == 0)
            {
                c = vec3(pos, lat0, lat1);
                dir = vec3(1.0, 0.0, 0.0);
            }
            else if(axis == 1)
            {
                c = vec3(lat0, pos, lat1);
                dir = vec3(0.0, 1.0, 0.0);
            }
            else
            {
                c = vec3(lat0, lat1, pos);
                dir = vec3(0.0, 0.0, 1.0);
            }
            vec3 dlt = p - c;
            float along = abs(dot(dlt, dir));
            float side = length(dlt - dir * dot(dlt, dir));
            float glow = exp(-(along * along) / max(half_w * half_w * 7.5, 1e-4))
                       * exp(-(side * side) / max(half_w * half_w * 1.40, 1e-4));
            glow *= slot_on;
            best = max(best, glow);
            if(glow >= best)
                best_h = fract(h + hue_scroll);
        }
        intensity = best;
        color_drv = best_h;
    }
    else
    {
        float best = 0.0;
        float best_h = 0.0;
        float bolts = mix(2.0, 3.0, density);
        float span = 0.94 * clamp(size_m, 0.55, 1.75);
        for(int b = 0; b < 3; b++)
        {
            float fb = float(b);
            float slot_on = 1.0 - step(bolts, fb);
            float seed = fb * 2.3 + floor(motion * (0.90 + 0.50 * freq_n) + fb);
            float flash = fract(motion * (0.55 + fb * 0.18) + hash21(vec2(seed, 0.2)));
            float alive = (1.0 - smoothstep(0.0, 0.78, flash)) * slot_on;
            vec3 a0 = vec3(
                hash21(vec2(seed, 1.0)) * 2.0 - 1.0,
                hash21(vec2(seed, 2.0)) * 2.0 - 1.0,
                hash21(vec2(seed, 3.0)) * 2.0 - 1.0) * 0.92 * span;
            vec3 a1 = vec3(
                hash21(vec2(seed, 4.0)) * 2.0 - 1.0,
                hash21(vec2(seed, 5.0)) * 2.0 - 1.0,
                hash21(vec2(seed, 6.0)) * 2.0 - 1.0) * 0.92 * span;
            vec3 m0 = mix(a0, a1, 0.33) + vec3(
                hash21(vec2(seed, 7.0)) - 0.5,
                hash21(vec2(seed, 8.0)) - 0.5,
                hash21(vec2(seed, 9.0)) - 0.5) * 0.38 * sway;
            vec3 m1 = mix(a0, a1, 0.66) + vec3(
                hash21(vec2(seed, 10.0)) - 0.5,
                hash21(vec2(seed, 11.0)) - 0.5,
                hash21(vec2(seed, 12.0)) - 0.5) * 0.38 * sway;
            float dline = 1e5;
            for(int s = 0; s < 3; s++)
            {
                vec3 u = (s == 0) ? a0 : ((s == 1) ? m0 : m1);
                vec3 v = (s == 0) ? m0 : ((s == 1) ? m1 : a1);
                vec3 ab = v - u;
                float t = clamp(dot(p - u, ab) / max(dot(ab, ab), 1e-4), 0.0, 1.0);
                dline = min(dline, length(p - (u + ab * t)));
            }
            float glow = softband(dline, half_w * 1.95) * alive;
            best = max(best, glow);
            if(glow >= best)
                best_h = fract(0.55 + fb * 0.08 + hue_scroll);
        }
        intensity = best;
        color_drv = best_h;
    }

    intensity = clamp(intensity * mix(1.08, 1.40, detail * 0.5 + 0.5), 0.0, 1.0);
    out_color = vec4(intensity, fract(color_drv), 0.0, 1.0);
}
