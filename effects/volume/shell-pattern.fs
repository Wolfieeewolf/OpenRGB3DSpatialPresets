# Name
name: Shell Pattern
class: ShellPattern
category: Volume
description: Shell contour extrude and cube displays from Display and Pattern
global: speed brightness frequency detail size scale fps color surface position strip bands thickness edge
resolution: 22
pattern_source: kernels
pattern_label: Pattern:
pattern_key: shellpattern_pattern_id
combo: shellpattern_display_mode 1 | Display:
option: Shell (wave height)
option: Extrude (solid by coordinate)
option: Shell (radial XZ)
option: Contour bands
option: Bars (rising columns)
option: Ripples (water rings)
option: Droplets (falling)
option: Fireworks (bursts)
option: Explosion (blast + sparks)
option: Rain (streaks)
slider: shellpattern_wave_amplitude 20 200 85 unit pct | Shell amplitude: | Wave / fill strength. Global Size scales feature size.
param: combo shellpattern_display_mode
param: shell_amp
param: motion_clock
param: shell_sigma
param: ndetail
param: size
param: freq_n
param: pattern
param: motion_clock
param: strip_reps
param: strip_unfold
param: strip_dir
finish: hex
# Effect
float fractf(float x) { return x - floor(x); }
float hash11(float x) { return fractf(sin(x * 12.9898) * 43758.547); }
float smstep(float e0, float e1, float x)
{
    float t = clamp((x - e0) / max(e1 - e0, 1e-5), 0.0, 1.0);
    return t * t * (3.0 - 2.0 * t);
}
float satTanh(float x)
{
    float e = exp(clamp(2.0 * x, -20.0, 20.0));
    return (e - 1.0) / (e + 1.0);
}
float evalStripKernelSigned(int kid, float s01, float phase01, float repeats, float time_sec)
{
    /* Keep time_sec as packed. 0 freezes Speed / Static room; do not fall back to u_time. */
    float rep = max(repeats, 1.0);
    float tsec = time_sec * 0.35;
    float ph = fractf(phase01 * 0.35 + time_sec * 0.08);
    float r = 1.0 + (rep - 1.0) * 0.72;
    float u_phase = fractf(s01 * r + ph + 1000.0);
    float TWO_PI = 6.2831853;
    float k = sin(TWO_PI * u_phase);

    /* 8 visible families from kid â€” keep this short so GLSL 1.10 compilers succeed. */
    float fam = floor(mod(float(kid) + 0.01, 8.0));
    if(fam < 0.5)
        k = sin(TWO_PI * u_phase);
    else if(fam < 1.5)
        k = 2.0 * u_phase - 1.0;
    else if(fam < 2.5)
        k = (1.0 - abs(2.0 * u_phase - 1.0)) * 2.0 - 1.0;
    else if(fam < 3.5)
        k = (u_phase < 0.5) ? 1.0 : -1.0;
    else if(fam < 4.5)
    {
        float u = fractf(s01 * r * 3.0 + ph);
        k = smstep(0.0, 0.12, u) * (1.0 - smstep(0.88, 1.0, u)) * 2.0 - 1.0;
    }
    else if(fam < 5.5)
        k = pow(1.0 - u_phase, 2.2) * 2.0 - 1.0;
    else if(fam < 6.5)
    {
        float t = s01 * r * 28.0 + ph * 11.0;
        float tw = fractf(t);
        float h = hash11(floor(t) * 0.031 + 9.1);
        float bright = (h > 0.72) ? 1.0 : -0.65;
        float decay = max(0.0, 1.0 - tw * 1.8);
        k = bright * decay + (-0.65) * (1.0 - decay);
    }
    else
    {
        float t = s01 * r * 5.0 + ph * 2.0;
        float i = floor(t);
        float f = t - i;
        float a = hash11(i);
        float b = hash11(i + 1.0);
        float s = f * f * (3.0 - 2.0 * f);
        k = (a + (b - a) * s) * 2.0 - 1.0;
    }
    return clamp(k, -1.0, 1.0);
}
const float STRIP_PI = 3.14159265;
const float STRIP_TWO_PI = 6.2831853;

/* GLSL 1.10 has no tanh. */
float stripSatTanh(float x)
{
    float e = exp(clamp(2.0 * x, -20.0, 20.0));
    return (e - 1.0) / (e + 1.0);
}

float stripUnfoldCoord01(float lx, float ly, float lz, int unfold_mode, float dir_deg)
{
    float s = 0.5;
    if(unfold_mode == 0)
        s = 0.5 + 0.5 * stripSatTanh(lx);
    else if(unfold_mode == 1)
        s = 0.5 + 0.5 * stripSatTanh(ly);
    else if(unfold_mode == 2)
        s = 0.5 + 0.5 * stripSatTanh(lz);
    else if(unfold_mode == 3 || unfold_mode == 8)
    {
        /* 3 = PlaneXZ; 8 = StaticRoomPlane (same spatial map; phase freeze is elsewhere). */
        float r = dir_deg * (STRIP_PI / 180.0);
        float w = cos(r) * lx + sin(r) * lz;
        s = clamp(0.5 + 0.35 * w, 0.0, 1.0);
    }
    else if(unfold_mode == 4)
    {
        float ang = atan(lz, lx);
        if(ang < 0.0)
            ang += STRIP_TWO_PI;
        s = ang / STRIP_TWO_PI;
        if(s >= 1.0)
            s -= 1.0;
        if(s < 0.0)
            s += 1.0;
        return s;
    }
    else if(unfold_mode == 5)
        s = 0.5 + 0.5 * stripSatTanh((lx + ly + lz) / 3.0);
    else if(unfold_mode == 6)
        s = clamp((abs(lx) + abs(ly) + abs(lz)) / 3.0, 0.0, 1.0);
    else
        /* 7 EffectPhaseOnly needs phase/time â€” use stripUnfoldKernelInputs. */
        s = 0.5;

    return clamp(s, 0.0, 1.0);
}

/* Matches StripColormapComputeS01 / SampleEffectStripColormap01 unfold/phase/time.
 * Returns vec3(s01, phase_eff, time_eff). */
vec3 stripUnfoldKernelInputs(float lx, float ly, float lz, int unfold_mode, float dir_deg,
                             float phase01, float time_sec)
{
    float s = 0.5;
    float phase_eff = phase01;
    float time_eff = time_sec;
    if(unfold_mode == 7)
    {
        s = fractf(phase01 + time_sec * 0.12 + 1000.0);
    }
    else if(unfold_mode == 8)
    {
        s = stripUnfoldCoord01(lx, ly, lz, 3, dir_deg);
        phase_eff = 0.0;
        time_eff = 0.0;
    }
    else
    {
        s = stripUnfoldCoord01(lx, ly, lz, unfold_mode, dir_deg);
    }
    return vec3(s, phase_eff, time_eff);
}

float shellIntensityGaussian(float ly, float surface_y, float sigma, float amp)
{
    float d = abs(ly - surface_y);
    float sig = max(sigma, 0.02);
    float d_cut = 3.0 * sig * max(1.0, amp);
    if(d > d_cut)
        return 0.0;
    float g = exp(-(d * d) / (sig * sig));
    return min(1.0, g);
}
float cubeHash11(float n)
{
    return fractf(sin(n * 127.1) * 43758.5453);
}
float softBand(float d, float sigma)
{
    float s = max(sigma, 0.02);
    return exp(-(d * d) / (s * s));
}
float lifeEnv(float life)
{
    float a = smstep(0.0, 0.14, life);
    float b = 1.0 - smstep(0.78, 1.0, life);
    return a * b;
}
float staggerLife(float clock, float rate, float seed)
{
    return fractf(clock * rate + seed);
}
float patternMod(float k)
{
    return 0.72 + 0.28 * (0.5 + 0.5 * k);
}
vec3 burstOrigin(float seed)
{
    return vec3(cubeHash11(seed) * 2.0 - 1.0,
                cubeHash11(seed + 1.7) * 2.0 - 1.0,
                cubeHash11(seed + 3.1) * 2.0 - 1.0) * 0.42;
}
float evalBars(vec3 l, float k, float amp, float clock, float sigma, float detail)
{
    float n = 2.0 + floor(2.5 * detail + 0.5);
    float cell = 2.0 / max(n, 1.0);
    float cx = (floor((l.x + 1.0) / cell) + 0.5) * cell - 1.0;
    float cz = (floor((l.z + 1.0) / cell) + 0.5) * cell - 1.0;
    float dx = l.x - cx;
    float dz = l.z - cz;
    float half_w = cell * (0.34 + 0.12 * amp);
    if(abs(dx) > half_w || abs(dz) > half_w)
        return 0.0;
    float id = cubeHash11(cx * 12.7 + cz * 91.3);
    float breathe = 0.5 + 0.5 * sin(clock * 6.2831853 + id * 6.2831853);
    float h = clamp(0.22 + 0.58 * (0.5 + 0.5 * k) * amp + 0.32 * breathe, 0.12, 1.15);
    float y01 = (l.y + 1.0) * 0.5;
    if(y01 < 0.0 || y01 > h + 0.10)
        return 0.0;
    float edge = 1.0 - max(abs(dx), abs(dz)) / max(half_w, 0.001);
    float body = edge * (0.55 + 0.45 * clamp(y01 / max(h, 0.001), 0.0, 1.0));
    float tip = softBand(y01 - h, 0.12 + sigma * 0.5);
    return clamp(body + tip * 0.55, 0.0, 1.0);
}
float rippleRing(float r, float clock, float rate, float seed, float size_m, float sigma)
{
    float age = staggerLife(clock, rate, seed);
    float R = age * (1.05 + 0.55 * size_m);
    return softBand(r - R, 0.06 + sigma * 0.35) * (1.0 - age) * lifeEnv(age);
}
float evalRipples(vec3 l, float k, float amp, float clock, float sigma, float detail, float size_m)
{
    float r = length(vec2(l.x, l.z));
    float freq = 1.6 + 2.4 * detail;
    float cycles = clock * (3.2 + 2.8 * amp);
    float travel = fractf(cycles) * 6.2831853;
    float wave1 = sin(r * freq - travel);
    float crest1 = pow(max(0.0, 0.5 + 0.5 * wave1), 1.9 + 1.2 * (1.0 - clamp(sigma * 2.0, 0.0, 1.0)));
    float travel2 = fractf(cycles * 0.55) * 6.2831853;
    float wave2 = sin(r * freq * 0.62 - travel2 + 1.3);
    float crest2 = pow(max(0.0, 0.5 + 0.5 * wave2), 2.4) * 0.48;
    float drop = max(rippleRing(r, clock, 0.55, 0.17, size_m, sigma),
                 max(rippleRing(r, clock, 0.55, 0.50, size_m, sigma),
                     rippleRing(r, clock, 0.55, 0.83, size_m, sigma)));
    float fall = exp(-r * (0.16 + 0.14 * (1.0 - clamp(size_m * 0.45, 0.0, 1.0))));
    float y_plane = softBand(l.y, 0.32 + 0.42 * size_m);
    float v = (crest1 + crest2) * fall + drop * 0.95;
    return clamp(v * y_plane * patternMod(k) * (0.85 + 0.25 * amp), 0.0, 1.0);
}
float oneDroplet(vec3 l, vec3 c, float life, float rad, float sigma)
{
    float py = 1.05 - life * 2.35;
    vec3 p = vec3(c.x, py, c.z);
    float d = length(l - p);
    return softBand(d, rad + sigma * 0.35) * lifeEnv(life);
}
float evalDroplets(vec3 l, float k, float amp, float clock, float sigma, float detail, float size_m, float freq_n)
{
    float n = 2.4 + 2.4 * detail + 2.0 * freq_n;
    float cell = 2.0 / max(n, 1.0);
    float cx = (floor((l.x + 1.0) / cell) + 0.5) * cell - 1.0;
    float cz = (floor((l.z + 1.0) / cell) + 0.5) * cell - 1.0;
    float id = cubeHash11(cx * 9.3 + cz * 17.1);
    float rate = 0.55 + 0.85 * amp;
    float rad = 0.10 + 0.08 * size_m;
    vec3 c = vec3(cx, 0.0, cz);
    float v = max(oneDroplet(l, c, staggerLife(clock, rate, id), rad, sigma),
                  oneDroplet(l, c, staggerLife(clock, rate, id + 0.5), rad, sigma));
    return clamp(v * patternMod(k), 0.0, 1.0);
}
float oneBurst(vec3 l, vec3 origin, float life, float amp, float sigma, float detail, float size_m, float clock)
{
    float fade = lifeEnv(life);
    float expand = max(0.0, (life - 0.18) / 0.82);
    float R = expand * (0.35 + 0.55 * size_m);
    vec3 dlt = l - origin;
    float d = length(dlt);
    float v = softBand(d - R, 0.07 + sigma * 0.4) * (1.0 - expand);
    float ang = atan(dlt.z, dlt.x);
    float spark = 0.55 + 0.45 * sin(ang * (5.0 + 3.0 * detail) + clock * 3.4);
    return v * spark * fade * (0.9 + 0.25 * amp);
}
float evalFireworks(vec3 l, float k, float amp, float clock, float sigma, float detail, float size_m)
{
    float rate = 0.42 + 0.28 * amp;
    float v = max(oneBurst(l, burstOrigin(0.11), staggerLife(clock, rate, 0.22), amp, sigma, detail, size_m, clock),
              max(oneBurst(l, burstOrigin(0.37), staggerLife(clock, rate, 0.52), amp, sigma, detail, size_m, clock),
                  oneBurst(l, burstOrigin(0.73), staggerLife(clock, rate, 0.74), amp, sigma, detail, size_m, clock)));
    return clamp(v * patternMod(k), 0.0, 1.0);
}
float oneBlast(vec3 l, float life, float amp, float sigma, float detail, float size_m)
{
    float fade = lifeEnv(life);
    float R = life * (0.7 + 0.85 * size_m);
    float d = length(l);
    float shell = softBand(d - R, 0.09 + sigma * 0.45) * (1.0 - life * 0.85);
    float ang = atan(l.z, l.x);
    float rays = 0.5 + 0.5 * sin(ang * (6.0 + 6.0 * detail) + life * 6.2831853);
    float streak = softBand(d - R * 0.7, 0.22) * rays * (1.0 - life);
    float core = 0.0;
    if(life < 0.22)
        core = softBand(d, 0.16) * (1.0 - life / 0.22);
    return max(shell, max(streak * 0.8, core)) * fade;
}
float evalExplosion(vec3 l, float k, float amp, float clock, float sigma, float detail, float size_m)
{
    float rate = 0.38 + 0.22 * amp;
    float v = max(oneBlast(l, staggerLife(clock, rate, 0.20), amp, sigma, detail, size_m),
              max(oneBlast(l, staggerLife(clock, rate, 0.52), amp, sigma, detail, size_m),
                  oneBlast(l, staggerLife(clock, rate, 0.74), amp, sigma, detail, size_m)));
    return clamp(v * patternMod(k), 0.0, 1.0);
}
float oneStreak(vec3 l, float cx, float cz, float life, float slant, float sigma, float size_m)
{
    float py = 1.2 - life * 2.5;
    float dx = l.x - cx - slant * (l.y - py) * 0.18;
    float radial = length(vec2(dx, l.z - cz));
    float along = softBand(l.y - py, 0.32 + 0.2 * size_m);
    float thin = softBand(radial, 0.055 + sigma * 0.28);
    return thin * along * lifeEnv(life);
}
float evalRain(vec3 l, float k, float amp, float clock, float sigma, float detail, float size_m, float freq_n)
{
    float n = 3.2 + 3.2 * detail + 2.4 * freq_n;
    float cell = 2.0 / max(n, 1.0);
    float cx = (floor((l.x + 1.0) / cell) + 0.5) * cell - 1.0;
    float cz = (floor((l.z + 1.0) / cell) + 0.5) * cell - 1.0;
    float col = cubeHash11(cx * 11.3 + cz * 19.7);
    float rate = 0.70 + 0.90 * amp;
    float slant = 0.4 + 0.3 * freq_n;
    float v = max(oneStreak(l, cx, cz, staggerLife(clock, rate, col), slant, sigma, size_m),
                  oneStreak(l, cx, cz, staggerLife(clock, rate, col + 0.5), slant, sigma, size_m));
    return clamp(v * patternMod(k), 0.0, 1.0);
}

void volumeMain(out vec4 out_color, in vec3 p01)
{
    int disp = int(clamp(u_params[0], 0.0, 9.0) + 0.5);
    float amp = clamp(u_params[1], 0.2, 2.5);
    float clock = u_params[2];
    float sigma = max(u_params[3], 0.03);
    float detail = clamp(u_params[4], 0.0, 1.0);
    float size_m = clamp(u_params[5], 0.08, 3.0);
    float freq_n = clamp(u_params[6], 0.0, 1.0);
    int kid = int(u_params[7] + 0.5);
    float phase_clock = u_params[8];
    float repeats = max(u_params[9], 1.0);
    int unfold_mode = int(u_params[10] + 0.5);
    float dir_deg = u_params[11];

    vec3 l = p01 * 2.0 - 1.0;
    vec3 uf = stripUnfoldKernelInputs(l.x, l.y, l.z, unfold_mode, dir_deg, phase_clock, clock);
    float k = evalStripKernelSigned(kid, uf.x, uf.y, repeats, uf.z);

    float intensity = 0.0;
    if(disp == 0)
        intensity = shellIntensityGaussian(l.y, amp * k, max(sigma, 0.02), amp);
    else if(disp == 1)
        intensity = clamp((k + 1.0) * 0.5, 0.0, 1.0);
    else if(disp == 2)
    {
        float k01 = clamp((k + 1.0) * 0.5, 0.0, 1.0);
        float r_span = (0.82 + 0.40 * clamp(amp, 0.2, 2.5) / 2.0) * max(size_m, 0.75);
        float surface_r = (0.15 + 0.85 * k01) * r_span;
        intensity = shellIntensityGaussian(length(vec2(l.x, l.z)), surface_r, sigma, max(1.0, amp));
    }
    else if(disp == 3)
    {
        float sig = max(0.035, 0.05 + sigma * 0.45);
        intensity = exp(-(k * k) / (sig * sig));
    }
    else if(disp == 4)
        intensity = evalBars(l, k, amp, clock, sigma, detail);
    else if(disp == 5)
        intensity = evalRipples(l, k, amp, clock, sigma, detail, size_m);
    else if(disp == 6)
        intensity = evalDroplets(l, k, amp, clock, sigma, detail, size_m, freq_n);
    else if(disp == 7)
        intensity = evalFireworks(l, k, amp, clock, sigma, detail, size_m);
    else if(disp == 8)
        intensity = evalExplosion(l, k, amp, clock, sigma, detail, size_m);
    else
        intensity = evalRain(l, k, amp, clock, sigma, detail, size_m, freq_n);

    out_color = vec4(clamp(intensity, 0.0, 1.0), clamp((k + 1.0) * 0.5, 0.0, 1.0), 0.0, 1.0);
}
