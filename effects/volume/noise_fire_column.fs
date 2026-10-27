# Name
name: Noise fire column
class: NoiseFireColumn
category: Volume
description: Height-driven noise fire column - fuel in XZ, heat along Y
global: color speed brightness size scale detail
user_colors: 1
resolution: 32
colors: FF6A1A
slider: fuel 0 100 78 unit pct | Fuel | Base burn strength
slider: flicker 0 100 62 unit pct | Flicker | Noise liveliness
slider: column 0 100 48 unit pct | Column | How tight the column is
param: unit fuel
param: unit flicker
param: unit column
finish: rgb

# Effect
float nfcHash(float n)
{
    return fract(sin(n) * 43758.5453123);
}
float nfcNoise3(vec3 p)
{
    vec3 i = floor(p);
    vec3 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float n = i.x + i.y * 57.0 + i.z * 113.0;
    float a = mix(nfcHash(n), nfcHash(n + 1.0), f.x);
    float b = mix(nfcHash(n + 57.0), nfcHash(n + 58.0), f.x);
    float c = mix(nfcHash(n + 113.0), nfcHash(n + 114.0), f.x);
    float d = mix(nfcHash(n + 170.0), nfcHash(n + 171.0), f.x);
    return mix(mix(a, b, f.y), mix(c, d, f.y), f.z);
}
float nfcNoiseStack(vec3 p, int octaves, float falloff)
{
    float sum = 0.0;
    float amp = 0.5;
    float tot = 0.0;
    vec3 q = p;
    for(int o = 0; o < 4; ++o)
    {
        if(o >= octaves) break;
        sum += amp * nfcNoise3(q);
        tot += amp;
        amp *= falloff;
        q *= 2.03;
    }
    return sum / max(tot, 1e-4);
}
vec3 nfcHsv2rgb(vec3 c)
{
    vec3 rgb = clamp(abs(mod(c.x * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    rgb = rgb * rgb * (3.0 - 2.0 * rgb);
    return c.z * mix(vec3(1.0), rgb, c.y);
}
void volumeMain(out vec4 out_color, in vec3 p01)
{
    float fuel = clamp(u_params[0], 0.05, 1.0);
    float flick = clamp(u_params[1], 0.05, 1.0);
    float col_w = mix(0.42, 0.1, clamp(u_params[2], 0.0, 1.0));
    float h = clamp(p01.y, 0.0, 1.0);
    vec2 xz = p01.xz - vec2(0.5);
    float radial = length(xz);
    float mask = exp(-pow(radial / max(col_w, 0.04), 2.2));
    float base = fuel * (1.0 - smoothstep(0.12, 0.92, h));
    vec3 np = vec3(xz * 4.0, h * 2.4 - u_time * (1.4 + flick));
    float n = nfcNoiseStack(np, 3, 0.52);
    float tip = nfcNoise3(vec3(xz * 7.0, h * 4.5 - u_time * 2.8));
    float dens = mask * base * (0.3 + 0.7 * n) * (0.65 + 0.35 * tip);
    dens *= smoothstep(0.02, 0.1, h);
    float core = exp(-pow(radial / max(col_w * 0.45, 0.02), 2.0));
    vec3 hot = nfcHsv2rgb(vec3(0.08 + 0.06 * (1.0 - h), 0.9, 0.7 + 0.3 * dens));
    vec3 cool = nfcHsv2rgb(vec3(0.02 + 0.04 * n, 0.75, 0.35 + 0.4 * dens));
    vec3 col = mix(cool, hot, core);
    out_color = vec4(clamp(col * dens * 1.65, 0.0, 1.0), 1.0);
}
