# Name
name: Field ripple
description: Thick radial ripple that floods the room then fades
section: volume

# Look
swatch: 100 200 255
icon: pen hot 0.05
icon: ellipse 0.5 0.5 0.55 0.55
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.28 0.28

# Playback
space: world
shader: ripple_ring
knobs: speed pulse

# Effect
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
