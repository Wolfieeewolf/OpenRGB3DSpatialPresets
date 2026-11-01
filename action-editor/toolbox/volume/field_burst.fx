# Name
name: Field burst
description: Expanding shell from the room centre
section: volume

# Look
swatch: 255 80 20
icon: pen hot 0.05
icon: ellipse 0.5 0.5 0.2 0.2
icon: ellipse 0.5 0.5 0.45 0.45
icon: ellipse 0.5 0.5 0.7 0.7
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.28 0.28

# Playback
space: world
shader: radial_burst
knobs: speed pulse

# Effect
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
