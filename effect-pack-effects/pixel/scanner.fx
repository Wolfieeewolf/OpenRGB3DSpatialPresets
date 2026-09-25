# Name
name: Scanner
description: A bar travels out and back
section: pixel

# Look
swatch: 40 255 120
icon: brush dim
icon: rect 0.1 0.42 0.8 0.16
icon: brush hot
icon: rect 0.42 0.18 0.16 0.64

# Playback
space: axis
knobs: speed direction pulse

# Effect
head = progress * 2
if head > 1
head = 2 - head
end
if invert == 1
head = 1 - head
end
half_w = max(0.02, pulse * 0.5)
dist = abs(axis - head)
cover = 0
if dist <= half_w
cover = 1 - (dist / half_w)
end
if cover <= 0.001
off
end
intensity = intensity * cover
paint(head)
