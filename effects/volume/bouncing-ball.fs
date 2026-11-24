# Name
name: Bouncing Ball
class: BouncingBall
category: Volume
description: Soft bouncing spheres with optional trails and wall squash - readable on sparse LED rooms
resolution: 48
global: speed brightness frequency detail size scale fps color surface position strip bands count
slider: ball_size 0 140 72 unit pct | Ball size: | Sphere radius (scales with Size). 0 = off.
slider: softness 0 160 88 unit pct | Softness: | Core / rim glow width (0 = sharpest)
slider: trail 0 100 40 unit pct | Trail: | Motion streak behind each ball (0 = off)
slider: squash 0 100 55 unit pct | Squash: | Flatten against walls on impact (0 = off)
slider: gravity 0 100 48 unit pct | Gravity: | Extra vertical bounce bias (0 = free / billiard)
param: ball_sim
param: gcount 2 32
param: size
param: unit ball_size
param: unit softness
param: unit trail
param: unit squash
param: unit gravity
param: hue_scroll_s
param: hue_density
param: ndetail
colors: 7C2D12 C2410C EA580C FBBF24 FEF3C7 FFFBEB
finish: hex

# Effect
float bbHash(float n)
{
    return vfxHash11(n);
}
float bounce1D(float x0, float v, float t, float lo, float hi)
{
    float L = max(hi - lo, 1e-4);
    float p = (x0 - lo) + v * t;
    float period = 2.0 * L;
    float m = mod(p, period);
    if(m < 0.0) m += period;
    return (m <= L) ? (lo + m) : (hi - (m - L));
}
/* Instantaneous bounce velocity sign from triangle-wave path. */
float bounceVel1D(float x0, float v, float t, float lo, float hi)
{
    float L = max(hi - lo, 1e-4);
    float p = (x0 - lo) + v * t;
    float period = 2.0 * L;
    float m = mod(p, period);
    if(m < 0.0) m += period;
    float going = step(m, L);
    return abs(v) * (going * 2.0 - 1.0) * sign(v + 1e-6);
}
void volumeMain(out vec4 out_color, in vec3 p01)
{
    float sim_t = u_params[0];
    int count = int(clamp(u_params[1], 2.0, 32.0) + 0.5);
    float size_m = max(u_params[2], 0.35);
    float size01 = clamp(u_params[3], 0.0, 1.6);
    float soft01 = clamp(u_params[4], 0.0, 1.8);
    float trail01 = clamp(u_params[5], 0.0, 1.0);
    float squash01 = clamp(u_params[6], 0.0, 1.0);
    float grav01 = clamp(u_params[7], 0.0, 1.0);
    float hue_scroll = fract(u_params[8]);
    float hue_dens = max(u_params[9], 1.0);
    float detail = clamp(u_params[10], 0.05, 1.0);

    if(size01 < 0.01)
    {
        out_color = vec4(0.0);
        return;
    }

    float radius = clamp((0.028 + 0.14 * size01) * (0.55 + 0.55 * size_m), 0.022, 0.30);
    float soft = max(radius * mix(0.55, 1.55, soft01 / 1.6) * mix(1.15, 0.75, detail), 0.020);
    float lo = radius;
    float hi = 1.0 - radius;
    /* Speed from global Motion (Speed) is already in ball_sim; keep a mild room pace. */
    float speed = mix(0.22, 0.78, 0.55);

    float intensity = 0.0;
    float hue01 = hue_scroll;

    for(int k = 0; k < 32; k++)
    {
        float fk = float(k);
        float slot_on = 1.0 - step(float(count), fk);
        float hx = bbHash(fk * 131.0 + 1.7);
        float hy = bbHash(fk * 313.0 + 5.0);
        float hz = bbHash(fk * 919.0 + 2.3);
        float x0 = mix(lo, hi, hx);
        float y0 = mix(lo, hi, hy);
        float z0 = mix(lo, hi, hz);

        float vx = (bbHash(fk * 733.0) * 2.0 - 1.0) * speed * mix(0.75, 1.15, bbHash(fk * 11.0));
        float vy = (bbHash(fk * 577.0) * 2.0 - 1.0) * speed * mix(0.85, 1.25, bbHash(fk * 19.0));
        float vz = (bbHash(fk * 829.0) * 2.0 - 1.0) * speed * mix(0.75, 1.15, bbHash(fk * 29.0));
        /* Gravity: bias vertical travel; 0 keeps free XYZ billiard. */
        float g_boost = mix(1.0, 1.85, grav01);
        float g_side = mix(1.0, 0.72, grav01);
        vy *= g_boost;
        vx *= g_side;
        vz *= g_side;
        vy += (1.0 - 2.0 * step(0.5, bbHash(fk * 41.0))) * speed * mix(0.12, 0.55, grav01);

        float t = sim_t + bbHash(fk * 47.0) * 3.0;
        float px = bounce1D(x0, vx, t, lo, hi);
        float py = bounce1D(y0, vy, t, lo, hi);
        float pz = bounce1D(z0, vz, t, lo, hi);
        vec3 c = vec3(px, py, pz);

        float wall_x = 1.0 - smoothstep(0.0, radius * 2.2, min(px - lo, hi - px));
        float wall_y = 1.0 - smoothstep(0.0, radius * 2.2, min(py - lo, hi - py));
        float wall_z = 1.0 - smoothstep(0.0, radius * 2.2, min(pz - lo, hi - pz));
        float squash_amt = squash01;
        float wx = mix(1.0, mix(1.0, 0.55, squash_amt), wall_x);
        float wy = mix(1.0, mix(1.0, 0.55, squash_amt), wall_y);
        float wz = mix(1.0, mix(1.0, 0.55, squash_amt), wall_z);

        vec3 dlt = p01 - c;
        dlt.x /= max(wx, 0.32);
        dlt.y /= max(wy, 0.32);
        dlt.z /= max(wz, 0.32);

        float sd = vfxSdSphere(dlt, radius);
        float body = vfxCoreRim(max(sd, 0.0), soft);
        /* Hot core inside the sphere for a solid ball read on sparse LEDs. */
        float inside = 1.0 - smoothstep(-radius * 0.15, radius * 0.35, sd);
        float core = inside * (0.55 + 0.45 * (1.0 - vfxSaturate(length(dlt) / max(radius, 1e-4))));
        float ball = vfxSaturate(max(body, core));

        float tvx = bounceVel1D(x0, vx, t, lo, hi);
        float tvy = bounceVel1D(y0, vy, t, lo, hi);
        float tvz = bounceVel1D(z0, vz, t, lo, hi);
        vec3 vel = vec3(tvx, tvy, tvz);
        float streak = 0.0;
        if(trail01 > 0.01)
        {
            float along = radius * mix(1.1, 2.8, trail01);
            float side = radius * mix(0.38, 0.72, soft01 / 1.6);
            streak = vfxStreak(dlt, -vel, along, side) * trail01;
            /* Impact flash near walls. */
            float impact = max(wall_x, max(wall_y, wall_z));
            streak = max(streak, streak * (1.0 + 0.55 * impact * squash_amt));
        }

        float v = vfxSaturate(max(ball, streak * 0.85)) * slot_on;
        if(v > intensity)
        {
            intensity = v;
            float dens_extra = max(hue_dens - 1.0, 0.0);
            float local = clamp(length(dlt) / max(radius, 1e-4), 0.0, 2.0);
            hue01 = fract(hue_scroll + fk * 0.1056 + local * dens_extra * 0.14 + core * 0.04);
        }
    }

    float gloss01 = clamp(pow(clamp(intensity, 0.0, 1.0), 1.35) * 0.92, 0.0, 1.0);
    out_color = vec4(clamp(intensity, 0.0, 1.0), hue01, gloss01, 1.0);
}
