# Name
name: Solid flash
description: Room-filling flash that pops then dies
section: basic

# Look
swatch: 255 255 255
icon: pen hot 0.05
icon: ellipse 0.5 0.5 0.55 0.55
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.35 0.35

# Playback
space: world
shader: solid_flash
knobs: speed pulse

# Effect
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
