# Name
name: Field electric
description: Crackling lightning forks jumping through the room
section: volume

# Look
swatch: 160 200 255
icon: pen hot 0.05
icon: line 0.35 0.15 0.55 0.45
icon: line 0.55 0.45 0.4 0.8
icon: line 0.55 0.45 0.75 0.7
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.4 0.5

# Playback
space: world
shader: electric_storm
knobs: speed pulse

# Effect
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
