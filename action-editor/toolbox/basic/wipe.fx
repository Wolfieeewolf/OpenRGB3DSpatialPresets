# Name
name: Wipe
description: A soft front sweeps along the row
section: basic

# Look
swatch: 80 160 255
icon: brush dim
icon: rect 0.06 0.22 0.88 0.56
icon: brush hot
icon: rect 0.06 0.22 0.44 0.56

# Playback
space: axis
knobs: speed direction

# Effect
edge = 0.08
front = progress * (1 + 2 * edge) - edge
d = front - axis
cover = 0
if d >= edge
cover = 1
else
if d > -edge
cover = (d + edge) / (2 * edge)
end
end
if cover <= 0.001
off
end
intensity = intensity * cover
paint(progress)
