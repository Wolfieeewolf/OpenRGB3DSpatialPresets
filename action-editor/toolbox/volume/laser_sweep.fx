# Name
name: Laser sweep
description: Bright beam sweeping across the room
section: volume

# Look
swatch: 0 229 255
icon: pen hot 0.05
icon: line 0.15 0.5 0.85 0.5
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.6 0.22

# Playback
space: world
ribbons: sweep
knobs: speed pulse

# Effect
v = ribbon_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
