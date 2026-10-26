# Name
name: Field portal
description: Spinning energy portal with pull toward the centre
section: volume

# Look
swatch: 180 80 255
icon: pen hot 0.05
icon: ellipse 0.5 0.5 0.45 0.45
icon: ellipse 0.5 0.5 0.22 0.22
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.35 0.35

# Playback
space: world
shader: portal_vortex
knobs: speed pulse

# Effect
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
