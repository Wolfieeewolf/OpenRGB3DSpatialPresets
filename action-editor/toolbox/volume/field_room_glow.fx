# Name
name: Field room glow
description: Full-room wash — every LED stays lit with slow colour drift
section: volume

# Look
swatch: 80 160 255
icon: pen hot 0.04
icon: ellipse 0.5 0.5 0.7 0.7
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.45 0.45

# Playback
space: world
shader: room_glow
knobs: speed pulse

# Effect
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
