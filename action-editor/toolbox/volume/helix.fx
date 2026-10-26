# Name
name: Helix
description: A twisted ribbon around the aim axis
section: volume

# Look
swatch: 80 255 200
icon: pen hot 0.1
icon: arc 0.16 0.08 0.68 0.42 0 180
icon: arc 0.16 0.42 0.68 0.42 180 180

# Playback
space: world
knobs: speed direction pulse

# Effect
along = sample_axis()
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
twist = fmod(along * 3 + time_ms * 0.001 * max(0.05, speed) + 1, 1)
delta = abs(ang - twist)
delta = min(delta, 1 - delta)
band = clamp(pulse, 0.06, 0.4)
if delta > band
off
end
intensity = intensity * (1 - (delta / band))
paint(along)
