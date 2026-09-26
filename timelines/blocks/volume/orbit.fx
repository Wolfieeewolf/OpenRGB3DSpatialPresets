# Name
name: Orbit
description: A band rotates around the aim axis
section: volume

# Look
swatch: 255 80 200
icon: pen dim 0.06
icon: ellipse 0.14 0.14 0.72 0.72
icon: pen none 0
icon: brush hot
icon: ellipse 0.7 0.42 0.18 0.18

# Playback
space: world
knobs: speed direction pulse

# Effect
ux = axis_x()
uy = axis_y()
uz = axis_z()
rx = 0
ry = 1
rz = 0
if abs(uy) > 0.9
rx = 1
ry = 0
end
tx = ry * uz - rz * uy
ty = rz * ux - rx * uz
tz = rx * uy - ry * ux
tlen = max(0.000001, sqrt(tx * tx + ty * ty + tz * tz))
tx = tx / tlen
ty = ty / tlen
tz = tz / tlen
bx = uy * tz - uz * ty
by = uz * tx - ux * tz
bz = ux * ty - uy * tx
u = dx * tx + dy * ty + dz * tz
v = dx * bx + dy * by + dz * bz
ang = atan2(v, u) / (2 * 3.141592653589793) + 0.5
ang = ang - floor(ang)
width = clamp(pulse, 0.06, 0.5)
delta = ang - progress
delta = delta - floor(delta + 0.5)
lead = width * 0.25
trail = width
cover = 0
if delta >= 0 && delta <= lead
cover = 1 - (delta / lead)
end
if delta < 0 && -delta <= trail
cover = 1 + (delta / trail)
end
cover = cover * (0.45 + 0.55 * (1 - clamp(radius, 0, 1) * 0.5))
if cover <= 0.001
off
end
intensity = intensity * cover
paint(progress)
