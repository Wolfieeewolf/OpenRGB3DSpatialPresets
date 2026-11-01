# Name
name: DNA Helix
class: DNAHelix
category: Volume
description: Soft double helix that stays readable on sparse LED grids
global: speed brightness frequency detail size scale fps color surface position strip bands
rainbow: true
pattern_label: Look:
pattern_key: dna_helix_shape
pattern: Classic helix
pattern: Thick rope
pattern: Twisted ribbons
pattern: Ladder (strong rungs)
slider: dna_helix_radius_pct 15 90 55 pct | Helix radius: | Strand orbit radius
slider: dna_helix_twists 50 2400 600 unit | Twists: | Turns along room height
slider: dna_helix_thickness_pct 6 70 22 pct | Strand thickness: | Core + glow width
slider: dna_helix_rung_pct 0 100 55 pct | Rungs: | Ladder rung strength
param: anim_time
param: dna_twists
param: dna_radius
param: dna_thick
param: dna_rung
param: pattern
finish: hex

# Effect
float smstep(float e0, float e1, float x)
{
    float t = clamp((x - e0) / max(e1 - e0, 1e-5), 0.0, 1.0);
    return t * t * (3.0 - 2.0 * t);
}
float dnaSoft(float d, float soft)
{
    float s = max(soft, 0.028);
    float core = exp(-(d * d) / (s * s * 0.35));
    float halo = exp(-(d * d) / (s * s * 1.70)) * 0.42;
    return clamp(core + halo, 0.0, 1.0);
}
void volumeMain(out vec4 out_color, in vec3 p01)
{
    /* Continuous anim_time - no progress wrap flash. */
    float time_sec = u_params[0];
    float twists = max(u_params[1], 0.35);
    float radius01 = clamp(u_params[2], 0.08, 0.95);
    float thickness = clamp(u_params[3], 0.020, 0.55);
    float rung_amount = clamp(u_params[4], 0.0, 1.0);
    int shape = int(clamp(u_params[5], 0.0, 3.0) + 0.5);
    float lx = p01.x * 2.0 - 1.0;
    float ly = p01.y;
    float lz = p01.z * 2.0 - 1.0;

    float progress = time_sec * 0.22;
    float phase = ly * twists * 6.2831853 + progress * 6.2831853;
    float c1 = cos(phase);
    float s1 = sin(phase);
    float c2 = -c1;
    float s2 = -s1;

    float hx1 = radius01 * c1;
    float hz1 = radius01 * s1;
    float hx2 = radius01 * c2;
    float hz2 = radius01 * s2;

    float d1 = length(vec2(lx - hx1, lz - hz1));
    float d2 = length(vec2(lx - hx2, lz - hz2));
    float d_strand = min(d1, d2);

    float core_w = max(thickness * ((shape == 1) ? 1.65 : 1.0), 0.028);
    float glow_w = core_w * ((shape == 2) ? 2.35 : 1.70);
    float strand = dnaSoft(d_strand, core_w);
    float glow = dnaSoft(d_strand, glow_w) * 0.42;
    float intensity = max(strand, glow);

    if(shape == 2)
    {
        float a = atan(lz, lx);
        float da1 = abs(mod(a - phase + 3.14159265, 6.2831853) - 3.14159265);
        float da2 = abs(mod(a - phase - 3.14159265 + 3.14159265, 6.2831853) - 3.14159265);
        float ang = min(da1, da2);
        float rad = length(vec2(lx, lz));
        float ribbon = dnaSoft(ang, 0.38 + thickness) *
                       (1.0 - smstep(radius01 * 0.30, radius01 * 1.40, abs(rad - radius01)));
        intensity = max(intensity, ribbon);
    }

    float rung = 0.0;
    if(rung_amount > 0.02)
    {
        float rung_phase = fract(ly * twists * 2.0 + progress * 0.35);
        float rung_gate = dnaSoft(abs(rung_phase - 0.5), 0.10 + 0.08 * (1.0 - rung_amount));
        float rad = length(vec2(lx, lz));
        float bridge = dnaSoft(abs(rad - radius01 * 0.55), thickness * 1.25);
        float between = 1.0 - smstep(radius01 * 0.12, radius01 * 1.08, rad);
        rung = bridge * between * rung_gate * rung_amount;
        if(shape == 3)
            rung *= 1.45;
        intensity = max(intensity, rung);
    }

    intensity = clamp(intensity, 0.0, 1.0);

    float palette01 = fract(ly * twists + progress * 0.18 + (d1 < d2 ? 0.0 : 0.5));
    float rung_hint = (rung > strand * 0.45) ? 1.0 : 0.0;
    out_color = vec4(intensity, palette01, rung_hint, 1.0);
}
