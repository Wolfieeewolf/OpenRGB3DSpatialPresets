# Name
name: Field aurora
description: GPU aurora curtains through the room (Action volume-field)
section: volume

# Look
swatch: 40 255 160
icon: pen hot 0.04
icon: line 0.25 0.15 0.3 0.85
icon: line 0.5 0.1 0.55 0.9
icon: line 0.75 0.2 0.7 0.8
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.55 0.55

# Playback
space: world
shader: aurora_curtain
knobs: speed pulse

# Effect
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
