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
t = time_ms * 0.001 * max(0.05, speed) * 3
# Warped fractal noise: slow billowing smoke rather than flat blotches
w = fbm(nx * 2 + t * 0.3, ny * 2, nz * 2 - t * 0.2, 3)
field = fbm(nx * 2.5 + w * 1.5 + t, ny * 2.5 - t * 0.6, nz * 2.5 + w + t * 0.35, 4)
field = smoothstep(0.25, 0.8, field)
intensity = intensity * (0.3 + 0.7 * field)
paint(field)
