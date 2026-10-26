# Name
name: Ribbon helix
description: Neon helix tube spinning through the room
section: volume

# Look
swatch: 83 188 40
icon: pen hot 0.05
icon: ellipse 0.5 0.2 0.35 0.12
icon: ellipse 0.5 0.5 0.35 0.12
icon: ellipse 0.5 0.8 0.35 0.12
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.4 0.55

# Playback
space: world
ribbons: helix
knobs: speed pulse

# Effect
v = ribbon_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
