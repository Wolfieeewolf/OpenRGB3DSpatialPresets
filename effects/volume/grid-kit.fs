# Name
name: Grid Kit
class: GridKit
category: Volume
description: Scanner lasers, a hologram cube, roaming boxes, a Rubik cube, a plasma orb, hyperspace and lightning for 3D LED rooms
global: speed brightness frequency detail size scale fps color surface position strip bands path thickness
pattern_label: Mode:
pattern_key: mode
pattern: Scanner Laser
pattern: Hologram Box
pattern: Moving Boxes
pattern: Rubik Cube
pattern: Plasma Orb
pattern: Hyperspace
pattern: Lightning
slider: density 20 100 70 unit pct | Density: | How much: beams + trail (Scanner), edge pulses (Hologram), box count, Rubik moves, arc rate (Plasma Orb), star count (Hyperspace), storm level (Lightning: 20 normal, 50 storm, 75 wide storm, 100 the gods are angry)
slider: sway 0 150 70 unit pct | Sway: | Shape: beam wobble (Scanner), tumble (Hologram), orbit spread, tail length (Plasma Orb), tunnel width (Hyperspace), bolt jag and branch reach (Lightning)
param: pattern
param: motion_clock
param: gthickness
param: size_star
param: path_axis
param: ndetail
param: hue_scroll_s
param: unit sway
param: unit density
finish: hex

# Effect
/* Globals → params.
 * Speed → motion_clock (0 stops). Frequency → hue_scroll_s (hue only).
 * Detail → ndetail (edge sharpness). Size → size_star. Thickness → gthickness.
 * Path → path_axis. Scale = occupancy (engine atlas, not packed here). */
float softband(float d, float w)
{
    float hw = max(w, 0.036);
    float core = exp(-(d * d) / (hw * hw * 0.50));
    float halo = exp(-(d * d) / (hw * hw * 1.85)) * 0.48;
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
float gkSegDist(vec3 p, vec3 a, vec3 b)
{
    vec3 ab = b - a;
    float t = clamp(dot(p - a, ab) / max(dot(ab, ab), 1e-6), 0.0, 1.0);
    return length(p - (a + ab * t));
}
/* Lightning channel vertex k of 8: jagged in the middle, pinned at both ends. */
vec3 gkBoltVertex(vec3 a, vec3 e, vec3 pu, vec3 pv, float seed, float k, float jag)
{
    float t = k / 8.0;
    float taper = sin(3.14159265 * t);
    return mix(a, e, t)
         + pu * (hash21(vec2(seed, 20.0 + k)) * 2.0 - 1.0) * jag * taper
         + pv * (hash21(vec2(seed, 40.0 + k)) * 2.0 - 1.0) * jag * taper;
}
/* One strike, s = 0..1: dim stepped leader, blinding return stroke, up to two
 * re-strikes, then a short afterglow. */
float gkStrikeEnvelope(float s, float restrike)
{
    if(s < 0.0 || s > 1.0)
        return 0.0;
    float leader = 0.30 * (1.0 - step(0.07, s));
    float stroke = exp(-max(s - 0.07, 0.0) * 22.0) * step(0.07, s);
    float r1 = 0.85 * exp(-max(s - 0.22, 0.0) * 28.0) * step(0.22, s) * step(0.35, restrike);
    float r2 = 0.65 * exp(-max(s - 0.40, 0.0) * 30.0) * step(0.40, s) * step(0.65, restrike);
    float after = 0.12 * (1.0 - smoothstep(0.07, 1.0, s)) * step(0.07, s);
    return clamp(max(max(leader, stroke), max(max(r1, r2), after)), 0.0, 1.0);
}
/* Plasma Orb roaming path (Lissajous), turned to the Path axis. */
vec3 gkOrbPath(float a, float span, int axis)
{
    vec3 c = vec3(sin(a) * 0.72, sin(a * 1.71) * 0.52, cos(a * 0.85 + 1.1) * 0.72) * span;
    if(axis == 1)
        return vec3(c.x, c.z, c.y);
    if(axis == 2)
        return vec3(c.z, c.y, c.x);
    return c;
}
void volumeMain(out vec4 out_color, in vec3 p01)
{
    int mode = int(clamp(u_params[0], 0.0, 6.0) + 0.5);
    /* motion_clock = time * GetMotionHz() * speed_mul — Speed 0 freezes. */
    float clock = u_params[1];
    float thick = max(u_params[2], 0.08);
    float size_m = clamp(u_params[3], 0.35, 2.5);
    int axis = int(clamp(u_params[4], 0.0, 2.0) + 0.5);
    float detail = clamp(u_params[5], 0.05, 1.0);
    float hue_scroll = fract(u_params[6]);
    float sway = clamp(u_params[7], 0.0, 1.5);
    float density = clamp(u_params[8], 0.15, 1.0);

    /* Speed alone sets the pace (plugin-wide rule); 0.45 keeps the original default feel. */
    float motion = clock * 0.45;

    vec3 p = (p01 - 0.5) * 2.0;
    /* Path axis and two sideways directions, shared by the modes that travel along it. */
    vec3 ax_dir = (axis == 0) ? vec3(1.0, 0.0, 0.0) : ((axis == 1) ? vec3(0.0, 1.0, 0.0) : vec3(0.0, 0.0, 1.0));
    vec3 lat_u = (axis == 0) ? vec3(0.0, 1.0, 0.0) : vec3(1.0, 0.0, 0.0);
    vec3 lat_v = (axis == 2) ? vec3(0.0, 1.0, 0.0) : vec3(0.0, 0.0, 1.0);
    /* Thickness maps hard; floor keeps sparse viewport grids lit. */
    float half_w = max(0.048, thick * mix(1.55, 0.70, detail));
    float intensity = 0.0;
    float color_drv = hue_scroll;

    if(mode == 0)
    {
        /* Scanner Laser — razor-thin beams sweep along the Path axis, leave a fading
         * afterimage behind them and glint where they cross LEDs. Density = beam count and
         * trail length; Sway = beam wobble. */
        float span = 0.92 * clamp(size_m, 0.55, 1.90);
        float sway_n = clamp(sway / 1.5, 0.0, 1.0);
        float phase = motion * (0.55 + 0.35 * density) * 3.14159265;
        float lat0 = dot(p, lat_u);
        float lat1 = dot(p, lat_v);
        float coord = dot(p, ax_dir) + sin(motion * 1.3 + lat0 * 2.0) * sway_n * 0.10;
        int n_beams = 1 + int(floor(density * 2.0 + 0.5));
        float trail_len = 0.15 + 0.45 * density;
        float best = 0.0;
        for(int i = 0; i < 3; i++)
        {
            if(i >= n_beams)
                break;
            float fi = float(i);
            float ph = phase + fi * 2.0943951;
            /* Sine sweep: velocity eases to zero at each end, so the trail shrinks and
             * regrows on the other side instead of flipping. */
            float center = sin(ph) * span;
            float vel = cos(ph);
            float rel = coord - center;
            float beam = softband(rel, half_w * 0.75);
            float behind = -rel * sign(vel);
            float trail = (behind > 0.0) ? exp(-behind / trail_len) * 0.45 * abs(vel) : 0.0;
            float halo = softband(rel, half_w * 4.0) * 0.22;
            float cell = hash21(floor(vec2(lat0, lat1) * 7.0) + vec2(floor(motion * 6.0), fi * 3.0));
            float glint = step(0.90, cell) * softband(rel, half_w * 1.6) * 0.90;
            best = max(best, max(max(beam, glint), max(trail, halo)));
        }
        intensity = best;
        color_drv = fract(0.5 + 0.35 * lat0 + 0.20 * lat1 + hue_scroll);
    }
    else if(mode == 1)
    {
        /* Hologram Box — a projected wireframe cube: energy pulses race along the edges, a
         * scanline passes through, faint faces, and the odd glitch flicker.
         * Density = pulses per edge; Sway = tumble. */
        float box_s = 0.72 * clamp(size_m, 0.55, 1.65);
        vec3 b = vec3(box_s);
        float sway_n = clamp(sway / 1.5, 0.0, 1.0);
        float spin = motion * 0.70;
        float tip = motion * (0.35 + 0.40 * sway_n);
        float g_seed = floor(motion * 6.0);
        float glitch = step(0.94, hash21(vec2(g_seed, 5.5)));
        vec3 q = p + lat_u * glitch * (hash21(vec2(g_seed, 6.6)) - 0.5) * 0.12;
        if(axis == 0)
        {
            q = gkRotX(q, spin);
            q = gkRotY(q, tip * 0.45);
        }
        else if(axis == 1)
        {
            q = gkRotY(q, spin);
            q = gkRotX(q, tip * 0.45 * (0.35 + 0.65 * sway_n));
        }
        else
        {
            q = gkRotZ(q, spin);
            q = gkRotY(q, tip * 0.45);
        }
        float ew = half_w * 2.2;
        float nx = softband(abs(q.x) - b.x, ew);
        float ny = softband(abs(q.y) - b.y, ew);
        float nz = softband(abs(q.z) - b.z, ew);
        float inside = step(abs(q.x), b.x + ew) * step(abs(q.y), b.y + ew) * step(abs(q.z), b.z + ew);
        /* Each edge family carries pulses running along its own length. */
        float dash = mix(2.0, 6.0, density) * 3.14159265 / box_s;
        float px = pow(0.5 + 0.5 * sin(q.x * dash - motion * 4.0), 3.0);
        float py = pow(0.5 + 0.5 * sin(q.y * dash - motion * 4.0 + 1.3), 3.0);
        float pz = pow(0.5 + 0.5 * sin(q.z * dash - motion * 4.0 + 2.6), 3.0);
        float edges = max(ny * nz * (0.45 + 0.55 * px),
                      max(nx * nz * (0.45 + 0.55 * py), nx * ny * (0.45 + 0.55 * pz))) * inside;
        float faces = 0.0;
        for(int f = 0; f < 6; f++)
            faces = max(faces, facePlane(q, b, ew * 1.1, f));
        float ax_c = (axis == 0) ? q.x : ((axis == 1) ? q.y : q.z);
        float scan = softband(ax_c - sin(motion * 0.9) * box_s, half_w * 1.2) * inside * 0.75;
        intensity = max(max(edges * 1.10, faces * 0.18), scan) * (1.0 - 0.6 * glitch);
        color_drv = fract(0.50 + 0.06 * ax_c + hue_scroll);
    }
    else if(mode == 2)
    {
        /* Moving Boxes — boxes roam on wide orbits; Speed = roam rate; Size = box + orbit; Density = count. */
        float best = 0.0;
        float best_h = 0.0;
        float n_vis = mix(4.0, 7.0, density);
        float orbit = mix(0.78, 1.05, clamp(sway / 1.5, 0.0, 1.0)) * clamp(size_m, 0.55, 1.75);
        for(int i = 0; i < 7; i++)
        {
            float fi = float(i);
            float slot_on = 1.0 - step(n_vis, fi);
            float seed = fi * 1.37 + 0.3;
            float ax = motion * (0.62 + 0.19 * fi) + seed;
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
        float half_n = n_moves;
        float cycle_len = half_n + hold_dur + half_n + hold_dur;
        float clock_m = motion * mix(0.48, 0.95, density);
        /* Skip past the first scramble so we open on a messed-up cube. */
        float shifted = clock_m + half_n;
        float cycle_id = floor(shifted / cycle_len);
        float t = mod(shifted, cycle_len);

        /* Phases: scramble → hold scrambled → solve → hold solved. */
        int n_done = 0;
        int cur_idx = -1;
        float cur_ang = 0.0;
        float sway_n = clamp(sway / 1.5, 0.0, 1.0);

        if(t < half_n)
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
        else if(t < half_n + hold_dur)
        {
            n_done = int(n_moves);
            cur_idx = -1;
            cur_ang = 0.0;
        }
        else if(t < half_n + hold_dur + half_n)
        {
            float u = t - half_n - hold_dur;
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
        /* Plasma Orb — white-hot core, swirling corona, comet tail of sparks, and electric
         * arcs that jump to the walls. Density = arc rate; Sway = tail length. */
        float a = motion * 0.95;
        float span = 0.78 * clamp(size_m, 0.55, 1.65);
        float sway_n = clamp(sway / 1.5, 0.0, 1.0);
        vec3 c = gkOrbPath(a, span, axis);
        float rad = 0.24 * clamp(size_m, 0.55, 1.55);
        vec3 dp = p - c;
        float d0 = length(dp);
        vec3 nrm = dp / max(d0, 1e-4);
        /* Corona: two drifting sine fields over the surface direction = swirling plasma. */
        float swirl = 0.5 + 0.25 * sin(dot(nrm, vec3(5.1, 3.7, 4.3)) * 2.6 + motion * 7.0)
                          + 0.25 * sin(dot(nrm, vec3(-3.3, 4.9, 2.1)) * 3.1 - motion * 5.3);
        float core = softband(d0, max(rad * 0.45, half_w));
        float corona = exp(-max(d0 - rad * 0.45, 0.0) / (rad * 0.75)) * (0.35 + 0.55 * swirl);
        /* Faint light the orb throws on the room, so sparse LEDs far from it still react. */
        float room_light = exp(-d0 / 0.60) * 0.18;
        float best = max(max(core, corona), room_light);
        float best_h = fract(0.62 + hue_scroll);

        /* Comet tail: sparks along the recent path, shrinking and jittering. */
        for(int i = 1; i <= 6; i++)
        {
            float fi = float(i);
            float lag = fi * (0.10 + 0.12 * sway_n);
            vec3 ci = gkOrbPath(a - lag, span, axis)
                    + (vec3(hash21(vec2(fi, floor(motion * 9.0))), hash21(vec2(fi + 3.1, floor(motion * 9.0))),
                            hash21(vec2(fi + 6.2, floor(motion * 9.0)))) - 0.5) * rad * 0.45;
            float spark = softband(length(p - ci), max(rad * (0.42 - 0.05 * fi), half_w * 0.8)) * (1.0 - 0.14 * fi);
            if(spark > best)
            {
                best = spark;
                best_h = fract(0.66 + 0.02 * fi + hue_scroll);
            }
        }

        /* Arcs: up to two discharges from the orb to a wall, timed like lightning. */
        float n_arcs = 1.0 + step(0.55, density);
        for(int j = 0; j < 2; j++)
        {
            float fj = float(j);
            if(fj >= n_arcs)
                break;
            float cycle = motion * (0.35 + 0.90 * density) * (0.8 + 0.4 * hash21(vec2(fj, 2.7))) + fj * 0.47;
            float seed = 31.0 + fj * 5.3 + floor(cycle);
            float env = gkStrikeEnvelope(fract(cycle) / 0.35, hash21(vec2(seed, 14.0)));
            if(env <= 0.0)
                continue;
            vec3 rv = vec3(hash21(vec2(seed, 1.0)), hash21(vec2(seed, 2.0)), hash21(vec2(seed, 3.0))) - 0.5;
            vec3 rdir = rv / max(length(rv), 1e-3);
            vec3 a0 = c + rdir * rad * 0.5;
            vec3 e0 = clamp(c + rdir * 2.5, vec3(-1.0), vec3(1.0)) * 0.98;
            vec3 dir = normalize(e0 - a0);
            vec3 pu = normalize(cross(dir, (abs(dir.y) < 0.9) ? vec3(0.0, 1.0, 0.0) : vec3(1.0, 0.0, 0.0)));
            vec3 pv = cross(dir, pu);
            float jag = 0.10 * length(e0 - a0);
            float d_arc = 1e5;
            vec3 prev = a0;
            for(int k = 1; k <= 8; k++)
            {
                vec3 vk = gkBoltVertex(a0, e0, pu, pv, seed, float(k), jag);
                d_arc = min(d_arc, gkSegDist(p, prev, vk));
                prev = vk;
            }
            float arc = max(softband(d_arc, half_w * 1.1), exp(-d_arc / 0.16) * 0.35) * env;
            if(arc > best)
            {
                best = arc;
                best_h = fract(0.60 + hue_scroll);
            }
        }
        intensity = best;
        color_drv = best_h;
    }
    else if(mode == 5)
    {
        /* Hyperspace — star streaks burst out of a vanishing point at the far end of the
         * Path axis, speed up and stretch as they pass. Density = star count;
         * Sway = tunnel width. */
        float span = clamp(size_m, 0.55, 1.75);
        float sway_n = clamp(sway / 1.5, 0.0, 1.0);
        int n_stars = 8 + int(floor(density * 16.0 + 0.5));
        float spread = mix(0.55, 1.45, sway_n);
        vec3 vp = ax_dir * 0.95;
        float best = 0.0;
        float best_h = fract(0.58 + hue_scroll);
        for(int i = 0; i < 24; i++)
        {
            if(i >= n_stars)
                break;
            float fi = float(i);
            float cycle = motion * (0.55 + 0.45 * hash21(vec2(fi, 7.1))) + hash21(vec2(fi, 1.9)) * 4.0;
            float seed = fi * 3.7 + floor(cycle);
            float t = fract(cycle);
            float ang = hash21(vec2(seed, 1.0)) * 6.2831853;
            float off = (0.25 + 0.75 * hash21(vec2(seed, 2.0))) * spread;
            vec3 ray = normalize(-ax_dir + (lat_u * cos(ang) + lat_v * sin(ang)) * off);
            /* t*t = accelerating away from the vanishing point; tail length grows with speed. */
            float travel = 2.6 * span;
            float head = t * t;
            float tail = max(t - (0.06 + 0.22 * t), 0.0);
            vec3 hp = vp + ray * travel * head;
            vec3 tp = vp + ray * travel * tail * tail;
            float bright = smoothstep(0.0, 0.25, t) * (1.0 - smoothstep(0.85, 1.0, t)) * (0.35 + 0.65 * t);
            float ds = gkSegDist(p, tp, hp);
            float glow = max(softband(ds, half_w * (0.55 + 0.75 * t)), exp(-ds / 0.12) * 0.30) * bright;
            if(glow > best)
            {
                best = glow;
                best_h = fract(0.56 + 0.10 * hash21(vec2(seed, 3.0)) + hue_scroll);
            }
        }
        /* Soft glow at the vanishing point so the whole tunnel reads on sparse LEDs. */
        float vp_glow = exp(-length(p - vp) / 0.35) * 0.30;
        intensity = max(best, vp_glow);
        color_drv = best_h;
    }
    else
    {
        /* Lightning — a dim leader grows from one area to another, then a blinding return
         * stroke, re-strikes and forked branches; the sky flashes so sparse rooms light up.
         * Density = storm level (20 normal, 50 storm, 75 wide storm, 100 the gods are angry).
         * Sway = jag and branch reach. Path = strike direction. */
        float storm = clamp((density - 0.15) / 0.85, 0.0, 1.0);
        float sway_n = clamp(sway / 1.5, 0.0, 1.0);
        float span = 0.96 * clamp(size_m, 0.55, 1.75);
        int n_bolts = 1 + int(floor(storm * 5.0 + 0.5));
        int n_branch = 1 + int(floor(storm * 2.5));
        float rate = 0.30 + 0.85 * storm;
        float fire_prob = mix(0.65, 1.0, storm);
        float jag = (0.10 + 0.22 * sway_n) * span;
        float core_w = half_w * (1.25 + 0.60 * storm);
        float best = 0.0;
        float sky = 0.0;
        float best_h = fract(0.60 + hue_scroll);
        for(int b = 0; b < 6; b++)
        {
            if(b >= n_bolts)
                break;
            float fb = float(b);
            /* One cycle per bolt; the strike uses the first 42%, the rest is dark. */
            float cycle = motion * rate * (0.85 + 0.30 * hash21(vec2(fb, 0.7))) + hash21(vec2(fb, 0.2)) * 3.0;
            float seed = fb * 7.13 + floor(cycle);
            float s = fract(cycle) / 0.42;
            float fires = step(1.0 - fire_prob, hash21(vec2(seed, 13.0)));
            float env = gkStrikeEnvelope(s, hash21(vec2(seed, 14.0))) * fires;
            if(env <= 0.0)
                continue;

            /* Strike from one end of the Path axis to the other; wide storms add
             * cloud-to-cloud crawlers that run sideways across the top. */
            vec2 r0 = vec2(hash21(vec2(seed, 1.0)), hash21(vec2(seed, 2.0))) * 2.0 - 1.0;
            vec2 r1 = vec2(hash21(vec2(seed, 3.0)), hash21(vec2(seed, 4.0))) * 2.0 - 1.0;
            vec3 a = (ax_dir + lat_u * r0.x * 0.85 + lat_v * r0.y * 0.85) * span;
            vec3 e = (-ax_dir + lat_u * r1.x * 0.85 + lat_v * r1.y * 0.85) * span;
            if(hash21(vec2(seed, 15.0)) < max(storm - 0.45, 0.0) * 1.1)
                e = (ax_dir * mix(0.55, 0.95, hash21(vec2(seed, 5.0))) - lat_u * r0.x * 0.95 + lat_v * r1.y * 0.95) * span;
            vec3 dir = normalize(e - a);
            vec3 pu = normalize(cross(dir, (abs(dir.y) < 0.9) ? vec3(0.0, 1.0, 0.0) : vec3(1.0, 0.0, 0.0)));
            vec3 pv = cross(dir, pu);

            /* The leader grows vertex by vertex during the first 7% of the strike. */
            float reveal = clamp(s / 0.07, 0.0, 1.0) * 8.0;
            float d_main = 1e5;
            vec3 prev = a;
            for(int k = 1; k <= 8; k++)
            {
                float fk = float(k);
                vec3 vk = gkBoltVertex(a, e, pu, pv, seed, fk, jag);
                float grow = clamp(reveal - (fk - 1.0), 0.0, 1.0);
                if(grow > 0.0)
                    d_main = min(d_main, gkSegDist(p, prev, mix(prev, vk, grow)));
                prev = vk;
            }

            float d_fork = 1e5;
            for(int j = 0; j < 3; j++)
            {
                if(j >= n_branch)
                    break;
                float fj = float(j);
                float kb = 2.0 + floor(hash21(vec2(seed, 60.0 + fj)) * 4.0);
                if(reveal < kb)
                    continue;
                vec3 root = gkBoltVertex(a, e, pu, pv, seed, kb, jag);
                float side = hash21(vec2(seed, 70.0 + fj)) * 6.2831853;
                vec3 bdir = normalize(dir * 0.55 + (pu * cos(side) + pv * sin(side)) * 0.85);
                float blen = (0.35 + 0.45 * sway_n) * span * (0.6 + 0.4 * hash21(vec2(seed, 80.0 + fj)));
                float breveal = clamp((reveal - kb) / 2.0, 0.0, 1.0) * 3.0;
                vec3 bp = root;
                for(int m = 1; m <= 3; m++)
                {
                    float fm = float(m);
                    vec3 bq = root + bdir * blen * (fm / 3.0)
                            + (pu * (hash21(vec2(seed + fj, 90.0 + fm)) - 0.5)
                             + pv * (hash21(vec2(seed + fj, 95.0 + fm)) - 0.5)) * jag * 0.9;
                    float g = clamp(breveal - (fm - 1.0), 0.0, 1.0);
                    if(g > 0.0)
                        d_fork = min(d_fork, gkSegDist(p, bp, mix(bp, bq, g)));
                    bp = bq;
                }
            }

            float core = softband(d_main, core_w) * env;
            float fork = softband(d_fork, core_w * 0.65) * env * 0.60;
            float halo = exp(-min(d_main, d_fork * 1.4) / (0.18 + 0.30 * storm)) * env * (0.30 + 0.25 * storm);
            float glow = max(core, max(fork, halo));
            if(glow > best)
            {
                best = glow;
                best_h = fract(0.60 + 0.05 * hash21(vec2(seed, 9.0)) + hue_scroll);
            }
            sky = max(sky, env * (0.10 + 0.25 * storm));
        }
        intensity = max(best, sky);
        color_drv = best_h;
    }

    intensity = clamp(intensity * mix(1.08, 1.40, detail * 0.5 + 0.5), 0.0, 1.0);
    out_color = vec4(intensity, fract(color_drv), 0.0, 1.0);
}
