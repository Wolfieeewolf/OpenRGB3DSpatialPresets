# Name
name: Neon ribbon
description: Neon tube arcing through the room
section: volume

# Look
swatch: 249 103 251
icon: pen hot 0.05
icon: line 0.18 0.55 0.82 0.45
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.55 0.28

# Playback
space: world
ribbons: arc
knobs: speed pulse

# Effect
v = ribbon_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
