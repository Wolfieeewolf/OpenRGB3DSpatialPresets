# Name
name: Plane sweep
description: A bright plane wiping through the room
section: basic

# Look
swatch: 80 180 255
icon: pen hot 0.05
icon: rect 0.42 0.12 0.12 0.76
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.55 0.55

# Playback
space: world
shader: plane_sweep
knobs: speed pulse

# Effect
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
