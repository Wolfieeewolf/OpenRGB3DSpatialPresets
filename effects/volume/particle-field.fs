# Name
name: Particle Field
class: ParticleField
category: Volume
description: Soft particles for sparse LED rooms - snow, embers, rain, fireworks with gloss and trails
resolution: 48
global: speed brightness frequency detail size scale fps color surface position strip bands count
pattern_label: Mode:
pattern_key: pf_mode
pattern: Float / Fuzzy
pattern: Snow
pattern: Embers
pattern: Sparkle
pattern: Attract
pattern: Rain
pattern: Fireworks
slider: pf_size 0 140 78 unit pct | Particle size: | Core size of each particle (0 = off)
slider: pf_thickness 0 140 110 unit pct | Softness: | Halo / streak width (0 = sharpest)
slider: pf_motion 10 250 75 unit | Motion: | Drift / fall / orbit speed scale
slider: pf_noise 0 150 35 unit pct | Noise: | Path wobble and scatter (0 = off)
slider: pf_fill 0 160 100 unit pct | Fill: | How far particles spread (0 = clustered centre)
slider: twinkle 0 100 35 unit pct | Twinkle: | Sparkle / flicker amount (0 = off)
slider: glow 0 100 45 unit pct | Glow: | Extra soft halo around particles (0 = off)
slider: stretch 0 100 50 unit pct | Stretch: | Streak / rain / ember length (0 = round)
param: motion_clock
param: pattern
param: gcount 8 48
param: psize
param: pthick
param: unit pf_motion
param: unit pf_noise
param: unit pf_fill
param: hue_scroll_s
param: freq_n
param: unit twinkle
param: unit glow
param: unit stretch
param: unit pf_size
param: ndetail
colors: 0B1026 1E3A8A 6366F1 A78BFA FDE68A FFF7ED
finish: hex

# Effect
float pfHash(float seed, float salt)
{
    return vfxHash11(seed * 12.9898 + salt * 78.233);
}
float pfSmstep(float e0, float e1, float x)
{
    float t = clamp((x - e0) / max(e1 - e0, 1e-5), 0.0, 1.0);
    return t * t * (3.0 - 2.0 * t);
}
vec3 pfWrap01(vec3 p)
{
    return p - floor(p);
}
vec3 pfDelta01(vec3 a, vec3 b)
{
    vec3 d = a - b;
    d.x = d.x - floor(d.x + 0.5);
    d.y = d.y - floor(d.y + 0.5);
    d.z = d.z - floor(d.z + 0.5);
    return d;
}
vec3 pfDrift(vec3 p, float clock)
{
    vec3 d1 = vec3(
        sin(p.y * 4.2 + clock * 1.05 + 0.2),
        sin(p.z * 3.8 + clock * 0.88 + 1.1),
        sin(p.x * 4.0 + clock * 0.96 + 2.0));
    vec3 d2 = vec3(
        sin(p.z * 7.1 + clock * 0.42 + 0.7),
        sin(p.x * 6.6 + clock * 0.37 + 1.9),
        sin(p.y * 6.8 + clock * 0.40 + 0.3));
    return d1 * 0.65 + d2 * 0.35;
}
vec3 pfSafeDir(vec3 v, vec3 fallback)
{
    float dn = length(v);
    return (dn > 1e-4) ? (v / dn) : fallback;
}
float pfPoint(float d, float soft, float glow01)
{
    float core = vfxCoreRim(d, max(soft, 0.022));
    float halo = vfxSoftBand(d, max(soft * 2.1, 0.040)) * 0.40 * glow01;
    return vfxSaturate(core + halo);
}
float pfStreak(vec3 dlt, vec3 dir, float soft_along, float soft_side, float glow01)
{
    float body = vfxStreak(dlt, dir, max(soft_along, 0.028), max(soft_side, 0.018));
    float head = vfxCoreRim(length(dlt), max(soft_side * 1.15, 0.020)) * 0.85;
    float halo = vfxSoftBand(length(dlt), max(soft_along * 0.55, 0.035)) * 0.28 * glow01;
    return vfxSaturate(max(body, head) + halo);
}
/* Hex-ish snow flake: disc + 3 soft arms. */
float pfFlake(vec3 dlt, float soft, float rot, float glow01)
{
    float s = max(soft, 0.024);
    float d = length(dlt);
    float disc = vfxCoreRim(d, s * 1.05) * 0.72;
    float arms = 0.0;
    for(int a = 0; a < 3; a++)
    {
        float ang = rot + float(a) * 1.04719755;
        float c = cos(ang);
        float sn = sin(ang);
        vec2 q = vec2(c * dlt.x + sn * dlt.z, -sn * dlt.x + c * dlt.z);
        float arm = exp(-(q.x * q.x) / (s * s * 0.22)) * exp(-(q.y * q.y) / (s * s * 2.4));
        arms = max(arms, arm);
    }
    float y_fall = exp(-(dlt.y * dlt.y) / (s * s * 0.85));
    float halo = vfxSoftBand(d, s * 2.0) * 0.32 * glow01;
    return vfxSaturate((disc + arms * 0.95) * y_fall + halo);
}
float pfStar(vec3 dlt, float soft, float rot, float glow01)
{
    float s = max(soft, 0.022);
    float d = length(dlt);
    float core = vfxCoreRim(d, s * 0.85);
    float c = cos(rot);
    float sn = sin(rot);
    vec3 q = vec3(c * dlt.x + sn * dlt.z, dlt.y, -sn * dlt.x + c * dlt.z);
    float arm_w = s * 0.34;
    float arm_l = s * 2.25;
    float plus = max(
        exp(-(q.x * q.x) / (arm_w * arm_w)) * exp(-(q.y * q.y + q.z * q.z) / (arm_l * arm_l)),
        exp(-(q.y * q.y) / (arm_w * arm_w)) * exp(-(q.x * q.x + q.z * q.z) / (arm_l * arm_l)));
    float c45 = 0.70710678;
    vec3 r = vec3(c45 * (q.x - q.z), q.y, c45 * (q.x + q.z));
    float crossv = max(
        exp(-(r.x * r.x) / (arm_w * arm_w)) * exp(-(r.y * r.y + r.z * r.z) / (arm_l * arm_l)),
        exp(-(r.z * r.z) / (arm_w * arm_w)) * exp(-(r.x * r.x + r.y * r.y) / (arm_l * arm_l)));
    float halo = vfxSoftBand(d, s * 2.2) * 0.38 * glow01;
    return vfxSaturate(core + max(plus, crossv) * 0.95 + halo);
}
float pfWisp(vec3 dlt, vec3 axis, float soft, float glow01)
{
    float body = vfxStreak(dlt, axis, soft * 1.9, soft * 0.70, glow01);
    return body;
}
/* Rain droplet: capsule falling along streak_dir. */
float pfDrop(vec3 dlt, vec3 dir, float soft, float stretch01, float glow01)
{
    vec3 u = pfSafeDir(dir, vec3(0.0, -1.0, 0.0));
    float len = soft * mix(1.2, 3.6, stretch01);
    vec3 a = u * (-len * 0.15);
    vec3 b = u * (len * 0.85);
    float sd = vfxSdCapsule(dlt, a, b, soft * 0.42);
    float body = vfxShell(sd, soft * 0.55);
    float head = vfxCoreRim(length(dlt - b), soft * 0.65);
    float halo = vfxSoftBand(length(dlt), soft * 1.8) * 0.25 * glow01;
    return vfxSaturate(max(body, head) + halo);
}
void volumeMain(out vec4 out_color, in vec3 p01)
{
    float clock_in = u_params[0];
    int mode = int(clamp(u_params[1], 0.0, 6.0) + 0.5);
    float count_f = clamp(u_params[2], 8.0, 48.0);
    float size01 = clamp(u_params[3], 0.0, 0.30);
    float thick = clamp(u_params[4], 0.0, 0.22);
    float motion = clamp(u_params[5], 0.05, 2.5);
    float noise_amt = clamp(u_params[6], 0.0, 1.5);
    float fill = clamp(u_params[7], 0.0, 1.6);
    float hue_scroll = fract(u_params[8]);
    float freq_n = clamp(u_params[9], 0.0, 1.0);
    float twinkle01 = clamp(u_params[10], 0.0, 1.0);
    float glow01 = clamp(u_params[11], 0.0, 1.0);
    float stretch01 = clamp(u_params[12], 0.0, 1.0);
    float size_gate = clamp(u_params[13], 0.0, 1.6);
    float detail = clamp(u_params[14], 0.05, 1.0);

    if(size_gate < 0.01 || size01 < 0.008)
    {
        out_color = vec4(0.0);
        return;
    }

    float clock = clock_in * (0.70 + 0.70 * freq_n);
    float soft_mul = mix(0.55, 1.35, clamp(thick / 0.13, 0.0, 1.5));
    soft_mul *= mix(1.10, 0.78, detail);

    float intensity = 0.0;
    float hue01 = hue_scroll;
    float golden = 2.39996323;
    float span = mix(0.12, 0.94, clamp(fill / 1.0, 0.0, 1.0));
    float voxel = 2.0 / max(u_res, 8.0);

    for(int i = 0; i < 48; i++)
    {
        float slot_on = step(float(i) + 0.5, count_f);
        float fi = float(i);
        float seed = fi * 265.443 + 101.390;
        float ring = sqrt((fi + 0.5) / max(count_f, 1.0));
        float ang = fi * golden;

        vec3 base;
        base.x = 0.5 + cos(ang) * ring * span * (0.55 + 0.45 * pfHash(seed, 1.0));
        base.y = 0.10 + 0.80 * pfHash(seed, 2.0);
        base.z = 0.5 + sin(ang) * ring * span * (0.55 + 0.45 * pfHash(seed, 3.0));

        float life = 1.0;
        float rad = size01 * (0.55 + 0.45 * pfHash(seed, 4.0));
        vec3 c = base;
        float ph = 0.0;
        float value = 0.0;
        float particle_hue = fract(fi * 0.097 + hue_scroll + 0.15 * pfHash(seed, 20.0));
        int wrap_field = 1;
        int shape = 0;
        vec3 streak_dir = vec3(0.0, 1.0, 0.0);
        float spin = clock * (0.6 + 1.4 * pfHash(seed, 40.0)) + seed;
        float stretch_l = stretch01;

        if(mode == 0)
        {
            vec3 n = pfDrift(base, clock * (0.55 + 0.45 * motion));
            c = pfWrap01(base + n * (0.10 * noise_amt + 0.045 * motion));
            rad *= 1.30;
            particle_hue = fract(particle_hue + 0.04 * n.x);
            shape = 5;
            streak_dir = pfSafeDir(n + vec3(0.05, 0.35, 0.02), vec3(0.1, 1.0, 0.05));
        }
        else if(mode == 1)
        {
            float cycle = 2.8 + 2.4 * pfHash(seed, 5.0);
            ph = fract(clock * (0.42 * motion) / cycle + pfHash(seed, 6.0));
            c.y = 1.0 - ph;
            float sway = sin(clock * 1.15 + seed) * 0.08 * motion;
            c.x = base.x + sway + (pfHash(seed, 7.0) - 0.5) * 0.05 * noise_amt;
            c.z = base.z + cos(clock * 0.92 + seed * 0.7) * 0.07 * motion;
            c = pfWrap01(c);
            rad *= 0.95;
            life = pfSmstep(0.0, 0.10, ph) * (1.0 - pfSmstep(0.90, 1.0, ph));
            particle_hue = fract(0.55 + 0.08 * pfHash(seed, 41.0) + hue_scroll * 0.15);
            shape = 1;
            spin = clock * 1.8 + seed * 2.1;
        }
        else if(mode == 2)
        {
            float cycle = 2.2 + 2.2 * pfHash(seed, 8.0);
            ph = fract(clock * (0.38 * motion) / cycle + pfHash(seed, 9.0));
            c.y = ph;
            float wob_x = sin(clock * 0.95 + seed) * 0.05 * motion;
            float wob_z = cos(clock * 0.82 + seed * 1.3) * 0.05 * motion;
            c.x = base.x + wob_x;
            c.z = base.z + wob_z;
            c = pfWrap01(c);
            float flick = 0.80 + 0.20 * sin(clock * 9.5 + seed * 3.1)
                        * (0.5 + 0.5 * sin(clock * 3.7 + seed));
            if(twinkle01 > 0.01)
                flick *= mix(1.0, 0.45 + 0.55 * abs(sin(clock * 14.0 + seed * 5.0)), twinkle01);
            life = pfSmstep(0.0, 0.12, ph) * (1.0 - pfSmstep(0.75, 1.0, ph)) * flick;
            rad *= 0.82;
            particle_hue = fract(0.02 + 0.12 * pfHash(seed, 11.0) + 0.08 * (1.0 - ph) + hue_scroll);
            shape = 2;
            stretch_l = mix(0.35, 1.0, stretch01);
            streak_dir = pfSafeDir(vec3(wob_x * 0.4, 1.0, wob_z * 0.4), vec3(0.0, 1.0, 0.0));
        }
        else if(mode == 3)
        {
            float cycle = 1.6 + 1.8 * pfHash(seed, 12.0);
            ph = fract(clock * (0.55 * motion) / cycle + pfHash(seed, 13.0));
            float env = pfSmstep(0.0, 0.04, ph) * (1.0 - pfSmstep(0.08, 0.18, ph));
            float alive = step(0.58 - 0.22 * freq_n, pfHash(seed, 14.0));
            float spark = 1.0;
            if(twinkle01 > 0.01)
                spark = mix(1.0, step(0.35, pfHash(seed + floor(clock * 8.0), 44.0)), twinkle01);
            life = env * alive * spark;
            c = base;
            c.y = 0.15 + 0.70 * pfHash(seed, 15.0);
            rad *= 0.62;
            wrap_field = 0;
            shape = 3;
            spin = seed * 1.7 + clock * 0.4;
            particle_hue = fract(0.12 + fi * 0.11 + hue_scroll);
        }
        else if(mode == 4)
        {
            float t = clock * (0.70 * motion);
            float pull = 0.28 + 0.50 * pfHash(seed, 16.0);
            vec3 orbit_p;
            orbit_p.x = 0.5 + cos(t + ang) * ring * span * (0.32 + 0.38 * noise_amt);
            orbit_p.y = 0.5 + sin(t * 0.82 + seed) * ring * span * 0.52;
            orbit_p.z = 0.5 + sin(t + ang) * ring * span * (0.32 + 0.38 * noise_amt);
            vec3 hub = vec3(0.5, 0.5, 0.5);
            c = mix(hub, mix(base, orbit_p, 0.72), 0.35 + 0.65 * pull);
            rad *= 1.00;
            wrap_field = 0;
            shape = 2;
            stretch_l = mix(0.25, 0.95, stretch01);
            streak_dir = pfSafeDir(vec3(-sin(t + ang), cos(t * 0.82 + seed) * 0.35, cos(t + ang)),
                                   vec3(1.0, 0.0, 0.0));
        }
        else if(mode == 5)
        {
            float cycle = 0.85 + 0.90 * pfHash(seed, 21.0);
            ph = fract(clock * (0.95 * motion) / cycle + pfHash(seed, 22.0));
            c.y = 1.0 - ph;
            float slant = (pfHash(seed, 23.0) - 0.5) * 0.12 * motion;
            c.x = base.x + slant * ph + (pfHash(seed, 24.0) - 0.5) * 0.035 * noise_amt;
            c.z = base.z + slant * ph * 0.6;
            c = pfWrap01(c);
            rad *= 0.55;
            life = pfSmstep(0.0, 0.06, ph) * (1.0 - pfSmstep(0.86, 1.0, ph));
            particle_hue = fract(0.55 + fi * 0.03 + hue_scroll);
            shape = 6;
            stretch_l = mix(0.40, 1.0, stretch01);
            streak_dir = pfSafeDir(vec3(slant, -1.0, slant * 0.55), vec3(0.0, -1.0, 0.0));
        }
        else
        {
            float burst_id = floor(fi / 6.0);
            float slot = mod(fi, 6.0);
            float cycle = 2.0 + 1.6 * pfHash(burst_id * 17.1 + 3.0, 30.0);
            ph = fract(clock * (0.40 * motion) / cycle + pfHash(burst_id, 31.0));
            vec3 origin_b;
            origin_b.x = 0.5 + (pfHash(burst_id, 32.0) - 0.5) * span;
            origin_b.y = 0.38 + 0.28 * pfHash(burst_id, 33.0);
            origin_b.z = 0.5 + (pfHash(burst_id, 34.0) - 0.5) * span;
            float bang = pfSmstep(0.08, 0.20, ph);
            float fade = 1.0 - pfSmstep(0.58, 0.92, ph);
            float boom = bang * fade;
            float ang_b = slot * 1.04719755 + pfHash(burst_id, 35.0) * 6.2831853;
            float elev = (pfHash(seed, 36.0) - 0.35) * 1.2;
            float expand = boom * (0.12 + 0.28 * span) * (0.7 + 0.45 * motion);
            c = origin_b;
            c.x += cos(ang_b) * expand;
            c.z += sin(ang_b) * expand;
            c.y += elev * expand - boom * boom * 0.16 * motion;
            life = boom * (0.65 + 0.35 * step(0.2, pfHash(seed, 37.0)));
            if(twinkle01 > 0.01)
                life *= mix(1.0, 0.55 + 0.45 * abs(sin(clock * 18.0 + seed * 7.0)), twinkle01);
            rad *= 0.70 + 0.40 * (1.0 - ph);
            particle_hue = fract(pfHash(burst_id * 17.1, 38.0) * 0.9 + hue_scroll + ph * 0.08);
            wrap_field = 0;
            shape = 2;
            stretch_l = mix(0.45, 1.0, stretch01);
            streak_dir = pfSafeDir(c - origin_b + vec3(0.0, 0.05, 0.0), vec3(0.0, 1.0, 0.0));
            if(ph < 0.12)
            {
                c = mix(vec3(origin_b.x, 0.08, origin_b.z), origin_b, ph / 0.12);
                life = pfSmstep(0.0, 0.05, ph) * (1.0 - pfSmstep(0.10, 0.12, ph));
                rad *= 0.60;
                shape = 0;
            }
        }

        vec3 dlt = (wrap_field == 1) ? pfDelta01(p01, c) : (p01 - c);
        float soft = max(max(thick, 0.016) * soft_mul * (0.55 + 0.55 * (rad / max(size01, 1e-4))), max(voxel, 0.018));
        float along = soft * mix(1.6, 3.4, stretch_l);

        if(shape == 1)
            value = pfFlake(dlt, soft * 1.15, spin, glow01);
        else if(shape == 2)
            value = pfStreak(dlt, streak_dir, along, soft * 0.55, glow01);
        else if(shape == 3)
            value = pfStar(dlt, soft * 1.10, spin, glow01);
        else if(shape == 4)
            value = pfStreak(dlt, streak_dir, along * 1.15, soft * 0.40, glow01);
        else if(shape == 5)
            value = pfWisp(dlt, streak_dir, soft * 1.35, glow01);
        else if(shape == 6)
            value = pfDrop(dlt, streak_dir, soft, stretch_l, glow01);
        else
            value = pfPoint(length(dlt), soft, glow01);

        if(twinkle01 > 0.01 && (mode == 0 || mode == 1 || mode == 3))
        {
            float tw = 0.65 + 0.35 * sin(clock * (6.0 + 8.0 * pfHash(seed, 50.0)) + seed * 4.0);
            value *= mix(1.0, tw, twinkle01);
        }

        value *= life * slot_on;
        if(value > intensity)
        {
            intensity = value;
            hue01 = particle_hue;
        }
    }

    float gloss01 = clamp(pow(clamp(intensity, 0.0, 1.0), 1.35) * 0.92, 0.0, 1.0);
    out_color = vec4(clamp(intensity, 0.0, 1.0), hue01, gloss01, 1.0);
}
