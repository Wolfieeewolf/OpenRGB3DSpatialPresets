# Name
name: Chase
description: A bright head runs along the row
section: basic

# Look
swatch: 80 220 255
icon: brush dim
icon: ellipse 0.08 0.38 0.16 0.24
icon: ellipse 0.30 0.38 0.16 0.24
icon: brush hot
icon: ellipse 0.52 0.38 0.16 0.24
icon: brush dim
icon: ellipse 0.74 0.38 0.16 0.24

# Playback
space: axis
knobs: speed direction pulse

# Effect
head = clamp(pulse, 0.02, 1)
delta = abs(axis - progress)
delta = min(delta, 1 - delta)
if delta > head
off
end
intensity = intensity * (1 - (delta / head))
paint(progress)
