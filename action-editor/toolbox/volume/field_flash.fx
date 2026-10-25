# Name
name: Field flash
description: GPU full-room title flash (Action volume-field engine)
section: volume

# Look
swatch: 255 255 255
icon: pen hot 0.05
icon: ellipse 0.5 0.5 0.55 0.55
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.35 0.35

# Playback
space: world
shader: title_flash
knobs: speed pulse

# Effect
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
