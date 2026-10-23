# Name
name: Noise
description: A moving 3D noise field
section: volume
alias: plasma3d

# Look
swatch: 140 80 255
icon: brush hot
icon: ellipse 0.16 0.16 0.16 0.16
icon: ellipse 0.52 0.16 0.16 0.16
icon: ellipse 0.34 0.52 0.16 0.16
icon: ellipse 0.7 0.7 0.16 0.16
icon: brush dim
icon: ellipse 0.34 0.16 0.16 0.16
icon: ellipse 0.16 0.52 0.16 0.16
icon: ellipse 0.52 0.52 0.16 0.16
icon: ellipse 0.16 0.7 0.16 0.16
icon: ellipse 0.52 0.7 0.16 0.16

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
