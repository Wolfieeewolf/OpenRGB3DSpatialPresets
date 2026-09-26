# Name
name: Hex Lattice
class: HexLattice
category: Spatial
global: speed brightness frequency detail size scale fps color surface position strip
description: Honeycomb lattice of hex prisms. Speed drives motion, Frequency cycles hue, Detail sets hex count, Size scales the cells.
rainbow: true
resolution: 22
user_colors: 1
supports_strip_colormap: true
finish: hex
pattern_label: Flow mode:
pattern_key: hexlattice_flow_mode
flow: 0.68
flow: 1
flow: 1.55
param: flow_progress
param: hue_time
param: detail_norm
param: size_floor
param: unit hexlattice_breathing_amount
param: unit hexlattice_pulse_amount
param: unit hexlattice_turbulence_amount
param: flow_mul

# Controls
slider: hexlattice_breathing_amount 0 200 35 unit pct | Breathing amount:
slider: hexlattice_pulse_amount 0 200 25 unit pct | Pulse amount:
pattern: Calm
pattern: Active
pattern: Aggressive
slider: hexlattice_turbulence_amount 0 200 15 unit pct | Turbulence amount:

# Effect
float wave01(float x)
{
    return 0.5 + 0.5 * sin(6.2831853 * x);
}
float hash12(vec2 p)
{
    return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.547);
}
void volumeMain(out vec4 out_color, in vec3 p01)
{
    float flow_t = u_params[0];
    float hue_t = u_params[1];
    float detail_norm = clamp(u_params[2], 0.0, 1.0);
    float base_scale = max(u_params[3], 0.2);
    float breathing_amount = clamp(u_params[4], 0.0, 2.0);
    float pulse_amount = clamp(u_params[5], 0.0, 2.0);
    float turbulence = clamp(u_params[6], 0.0, 2.0);
    float flow_mul = max(u_params[7], 0.15);
    float breathe = 1.0 + (wave01(flow_t * 0.30) - 0.5) * 0.35 * breathing_amount;
    // Soft-cap cell density so Detail=200 does not explode shader cost / aliasing.
    float cells = min(12.0, (5.0 + 7.0 * detail_norm) / base_scale * breathe);

    vec3 local01 = p01;
    vec2 uv = vec2(local01.x, local01.z);
    uv += turbulence * 0.07 * vec2(sin(6.2831853 * (local01.y * 0.8 + flow_t * 0.11)),
                                   cos(6.2831853 * (local01.y * 0.8 - flow_t * 0.09)));
    uv *= cells;
    uv += flow_t * flow_mul * vec2(0.22, 0.31);

    vec2 r = vec2(1.0, 1.7320508);
    vec2 hr = r * 0.5;
    vec2 a = mod(uv, r) - hr;
    vec2 b = mod(uv - hr, r) - hr;
    vec2 gv = (dot(a, a) < dot(b, b)) ? a : b;
    vec2 id = uv - gv;

    float hd = max(dot(abs(gv), vec2(0.5, 0.8660254)), abs(gv).x);
    float edge_w = 0.20 - 0.08 * detail_norm;
    float edge = smoothstep(0.5 - edge_w, 0.5 - edge_w * 0.15, hd);

    float hcell = hash12(id);
    float pulse = wave01(flow_t * (0.20 + 0.35 * hcell) * flow_mul + hcell);
    float cell_fill = (0.06 + 0.30 * pulse * pulse_amount) * (1.0 - edge);

    float v = clamp(edge * (0.80 + 0.20 * pulse) + cell_fill, 0.0, 1.0);

    float h01 = fract(id.x * 0.045 + id.y * 0.030 + hcell * 0.18 + (local01.y - 0.5) * 0.10 + hue_t);
    out_color = vec4(v, h01, 0.0, 1.0);
}
