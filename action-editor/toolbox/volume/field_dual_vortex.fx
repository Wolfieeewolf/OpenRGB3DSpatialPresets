# Name
name: Field dual vortex
description: Twin counter-rotating swirls filling the room
section: volume

# Look
swatch: 255 90 40
icon: pen hot 0.05
icon: ellipse 0.32 0.5 0.28 0.28
icon: ellipse 0.68 0.5 0.28 0.28
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.55 0.4

# Playback
space: world
shader: dual_vortex
knobs: speed pulse

# Effect
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
