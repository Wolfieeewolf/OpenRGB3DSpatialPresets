# Name
name: Sphere wipe
description: A sphere grows from the centre
section: volume

# Look
swatch: 80 180 255
icon: pen dim 0.06
icon: ellipse 0.1 0.1 0.8 0.8
icon: pen none 0
icon: brush hot
icon: ellipse 0.3 0.3 0.4 0.4

# Playback
space: world
knobs: speed

# Effect
edge = 0.12
t = time_ms * 0.001 * max(0.05, speed)
front = progress * (1 + 2 * edge) - edge
# Ragged, glowing edge as the sphere grows
d = front - radius + (fbm(nx * 3 - t, ny * 3, nz * 3 + t, 3) - 0.5) * edge * 1.2
cover = max(smoothstep(-edge, edge, d), glow(d, edge * 0.8))
if cover <= 0.02
off
end
intensity = intensity * cover
paint(progress)
