# Name
name: Noise fire column
class: NoiseFireColumn
category: Volume
description: Rising fire column that fills sparse LEDs - fuel in XZ, heat along Y
global: color speed brightness size scale detail
user_colors: 1
resolution: 32
colors: FF6A1A
slider: fuel 0 100 82 unit pct | Fuel: | Base burn strength
slider: flicker 0 100 68 unit pct | Flicker: | Noise liveliness
slider: column 0 100 42 unit pct | Column: | How tight the column is (higher = thinner)
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
    /* Wider default column so sparse rooms still catch heat. */
    float col_w = mix(0.55, 0.14, clamp(u_params[2], 0.0, 1.0));
    float h = clamp(p01.y, 0.0, 1.0);
    vec2 xz = p01.xz - vec2(0.5);
    float radial = length(xz);

    float mask = exp(-pow(radial / max(col_w, 0.06), 1.85));
    float halo = exp(-pow(radial / max(col_w * 1.65, 0.10), 1.55)) * 0.38;
    float base = fuel * (1.0 - smoothstep(0.08, 0.96, h));
    base *= 0.55 + 0.45 * smoothstep(0.0, 0.18, h);

    /* Continuous u_time rise - no progress wrap. */
    float rise = u_time * (1.55 + 1.35 * flick);
    vec3 np = vec3(xz * 3.6, h * 2.6 - rise);
    float n = nfcNoiseStack(np, 3, 0.55);
    float tip = nfcNoise3(vec3(xz * 6.5, h * 4.2 - rise * 1.85));
    float tongue = nfcNoise3(vec3(xz * 2.2 + tip * 0.15, h * 1.4 - rise * 0.65));

    float dens = (mask + halo) * base * (0.28 + 0.72 * n) * (0.62 + 0.38 * tip);
    dens *= 0.75 + 0.25 * tongue;
    dens *= smoothstep(0.015, 0.09, h);

    float core = exp(-pow(radial / max(col_w * 0.42, 0.035), 1.9));
    float white_hot = core * dens * (1.0 - h * 0.55);
    vec3 hot = nfcHsv2rgb(vec3(0.07 + 0.05 * (1.0 - h), 0.88, 0.72 + 0.28 * dens));
    vec3 cool = nfcHsv2rgb(vec3(0.02 + 0.05 * n, 0.78, 0.32 + 0.45 * dens));
    vec3 ember = nfcHsv2rgb(vec3(0.04 + 0.03 * tip, 0.92, 0.55 + 0.40 * dens));
    vec3 col = mix(cool, hot, core);
    col = mix(col, ember, clamp(tongue * 0.45, 0.0, 0.55));
    col += vec3(1.0, 0.92, 0.75) * white_hot * 0.35;

    out_color = vec4(clamp(col * dens * 1.85, 0.0, 1.0), 1.0);
}
