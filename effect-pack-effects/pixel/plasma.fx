# Name
name: Plasma
description: A swirling noise field
section: pixel

# Look
swatch: 180 40 255
icon: brush dim
icon: ellipse 0.08 0.14 0.42 0.42
icon: brush hot
icon: ellipse 0.5 0.1 0.38 0.38
icon: brush ink
icon: ellipse 0.28 0.5 0.38 0.38

# Playback
space: world
knobs: speed

# Effect
t = progress * 3
n = noise(nx * 2.5 + t, ny * 2.5 - t * 0.6, nz * 2.5 + t * 0.35)
n2 = noise(nx * 5 - t * 0.5, ny * 5 + t * 0.4, nz * 5)
field = clamp(0.5 * n + 0.5 * n2, 0, 1)
intensity = intensity * (0.35 + 0.65 * field)
paint(field)
