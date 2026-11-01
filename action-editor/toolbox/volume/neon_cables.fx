# Name
name: Neon cables
description: Loose multi-cable neon bundle weaving through the room
section: volume

# Look
swatch: 38 191 255
icon: pen hot 0.05
icon: line 0.12 0.35 0.88 0.55
icon: line 0.15 0.55 0.85 0.40
icon: line 0.18 0.70 0.82 0.65
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.55 0.32

# Playback
space: world
ribbons: cables
knobs: speed pulse

# Effect
v = ribbon_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
