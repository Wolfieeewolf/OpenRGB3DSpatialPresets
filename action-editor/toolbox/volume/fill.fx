# Name
name: Fill
description: Colour closes in from the outside
section: volume

# Look
swatch: 255 120 200
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
front = (1 - progress) * (1 + edge)
# Ragged, glowing edge as the light closes in
d = radius - front + (fbm(nx * 3 + t, ny * 3, nz * 3 - t, 3) - 0.5) * edge * 1.2
cover = max(smoothstep(-edge, edge, d), glow(d, edge * 0.8))
if cover <= 0.02
off
end
intensity = intensity * cover
paint(radius)
