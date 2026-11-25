# Name
name: Burst
description: A shell expands outward from the centre
section: volume

# Look
swatch: 255 60 40
icon: pen hot 0.06
icon: line 0.5 0.5 0.5 0.12
icon: line 0.5 0.5 0.88 0.5
icon: line 0.5 0.5 0.5 0.88
icon: line 0.5 0.5 0.12 0.5
icon: pen none 0
icon: brush ink
icon: ellipse 0.38 0.38 0.24 0.24

# Playback
space: world
knobs: speed pulse

# Effect
edge = clamp(pulse, 0.06, 0.45)
t = time_ms * 0.001 * max(0.05, speed)
front = progress * (1 + 2 * edge)
# Turbulent shock front: noise pushes the edge in and out
wob = (fbm(nx * 3 + t, ny * 3 - t * 0.7, nz * 3, 3) - 0.5) * edge * 0.8
d = front + wob - radius
cover = glow(d - edge * 0.3, edge * 0.7)
# Fireball at the origin for the first beat
if progress < 0.3
cover = max(cover, (1 - progress / 0.3) * gauss(radius, 0.35 + progress))
end
cover = cover * (1 - smoothstep(0.85, 1, progress))
if cover <= 0.02
off
end
intensity = intensity * cover
paint(clamp(radius, 0, 1))
