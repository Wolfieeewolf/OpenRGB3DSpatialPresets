name: Noise Contour
class: shader_field_noise_contour
category: Shader Field
description: Soft topographic contour bands from flowing noise (Wash / Density / look sliders; vfx colour)
global: color speed brightness size scale detail frequency
user_colors: 1
colors: #40E0FF #FFFFFF
param: zoom
param: contrast
param: hue
param: detail
param: wash
param: density
param: bands
slider: wash 0 100 60 pct | Wash | Soft fill (0 = off — crests only on black, 100 = full soft look)
slider: density 0 100 50 pct | Density | Feature spacing / count (0 = sparse / nearly off, 100 = dense)
slider: bands 0 100 55 pct | Bands | Contour count (0 = off)

float ncHash11(float t){return fract(sin(t*56789.0)*56789.0);}
float ncHash21(vec2 uv){return ncHash11(ncHash11(uv.x)+2.0*ncHash11(uv.y));}
vec2 ncGrad(vec2 uv){float t=ncHash21(uv);return vec2(cos(6.2831853*t),sin(6.2831853*t));}
float ncNoise(vec2 uv,float r){
    float ca=cos(r);float sa=sin(r);vec2 uvi=floor(uv);vec2 uvf=uv-uvi;
    vec2 g00=ncGrad(uvi);vec2 g10=ncGrad(uvi+vec2(1.0,0.0));
    vec2 g01=ncGrad(uvi+vec2(0.0,1.0));vec2 g11=ncGrad(uvi+vec2(1.0,1.0));
    g00=vec2(ca*g00.x-sa*g00.y,sa*g00.x+ca*g00.y);g10=vec2(ca*g10.x-sa*g10.y,sa*g10.x+ca*g10.y);
    g01=vec2(ca*g01.x-sa*g01.y,sa*g01.x+ca*g01.y);g11=vec2(ca*g11.x-sa*g11.y,sa*g11.x+ca*g11.y);
    float f00=dot(g00,uvf);float f10=dot(g10,uvf-vec2(1.0,0.0));
    float f01=dot(g01,uvf-vec2(0.0,1.0));float f11=dot(g11,uvf-vec2(1.0,1.0));
    float sx=uvf.x*uvf.x*(3.0-2.0*uvf.x);float sy=uvf.y*uvf.y*(3.0-2.0*uvf.y);
    return ((mix(mix(f00,f10,sx),mix(f01,f11,sx),sy)/0.7)+1.0)*0.5;
}
float ncFbm(vec2 uv,float r){return (ncNoise(uv,r)+ncNoise(uv*2.0,r)*0.5)/1.5;}
void spatialMain(out vec4 out_color, in vec2 frag_coord)
{
    float wash = clamp(u_params[4], 0.0, 1.0);
    float density = clamp(u_params[5], 0.0, 1.0);
    float tweak = clamp(u_params[6], 0.0, 1.0);
    float dens_mul = mix(0.12, 2.50, density);

    if(density < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    if(tweak < 0.01) { out_color = vec4(0.0, 0.0, 0.0, 1.0); return; }

    vec2 uv = frag_coord / max(u_resolution, vec2(1.0));
    float zoom = max(u_params[0], 0.25);
    float contrast = clamp(u_params[1], 0.35, 2.5);
    float hue = fract(u_params[2]);
    float detail = clamp(u_params[3], 0.05, 1.0);
    vec2 p = (uv - 0.5) * vec2(u_resolution.x / max(u_resolution.y, 1.0), 1.0) * (2.4 * (zoom * dens_mul) * (0.7 + detail));
    float noise_fac =(ncFbm(p * (2.0 + 2.5 * detail), u_time * 1.437)) ;
    float contour = 0.5 * (1.0 - cos((18.0 + 28.0 * detail) * mix(0.35, 2.2, tweak) * 3.14159265 * noise_fac));
    float h = fract(hue + noise_fac * 0.35 + u_time * 0.05225);
    vec3 rgb = mix(vec3(0.85, 0.05, 0.75), vec3(0.05, 0.85, 0.90), noise_fac);
    vec3 tint = vfxHsv2rgb(vec3(h, 0.85, 1.0));
    /* Wash: 0 = crests on black only; soft body scales with wash. */
    {
        float _raw = clamp(contour, 0.0, 1.0);
        float _crest = pow(_raw, 2.85);
        contour = mix(_crest, _raw, wash);
    }
    float clip = smoothstep(0.55, mix(0.85, 0.98, contrast / 2.5), contour);
    out_color = vec4(mix(rgb, tint, 0.35) * clip * contour, 1.0);
}
