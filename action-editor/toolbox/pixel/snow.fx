# Name
name: Snow
description: Flakes fall through the volume
section: pixel
alias: meteors

# Look
swatch: 220 235 255
icon: pen hot 0.06
icon: line 0.22 0.28 0.38 0.28
icon: line 0.3 0.16 0.3 0.4
icon: line 0.62 0.42 0.78 0.42
icon: line 0.7 0.3 0.7 0.54
icon: line 0.42 0.72 0.58 0.72
icon: line 0.5 0.6 0.5 0.84

# Playback
space: world
knobs: speed

# Effect
h = hash01_mix(seed, 9743197, 17)
fall = fmod(progress + h, 1)
flake_h = 1 - fall
drift = 0.12 * sin(fall * 10 + h * 8)
plane = nx
if height == nx
if span_z >= span_y
plane = nz
else
plane = ny
end
else
if height == ny
if span_x >= span_z
plane = nx
else
plane = nz
end
else
if span_x >= span_y
plane = nx
else
plane = ny
end
end
end
fx = fmod(h + drift + 1, 1)
d = abs(plane - fx) * 1.2 + abs(height - flake_h)
if d > 0.16
off
end
intensity = intensity * (1 - d / 0.16)
paint(h)
