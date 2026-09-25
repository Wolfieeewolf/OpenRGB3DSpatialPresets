# Name
name: Burst
description: A shell expands outward
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
front = progress * (1 + 2 * edge)
d = front - radius
cover = 0
if d >= 0 && d <= edge
cover = 1 - (d / edge)
end
if d < 0 && -d <= edge * 0.35
cover = 1 + (d / (edge * 0.35))
end
if progress < 0.2
cover = max(cover, (1 - progress / 0.2) * (1 - clamp(radius, 0, 1)))
end
if cover <= 0.001
off
end
intensity = intensity * cover
paint(clamp(radius, 0, 1))
