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
uz = axis_z()
side = dx * uz - dz * ux
ang = side * 0.5 + 0.5
ang = ang - floor(ang)
twist = fmod(along * 3 + progress + 1, 1)
delta = abs(ang - twist)
delta = min(delta, 1 - delta)
band = clamp(pulse, 0.06, 0.4)
if delta > band
off
end
intensity = intensity * (1 - (delta / band))
paint(along)
