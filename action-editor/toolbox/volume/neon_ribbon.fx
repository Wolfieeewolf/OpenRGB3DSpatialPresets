# Name
name: Neon ribbon
description: GPU neon tube sampled onto room LEDs (Action ribbon-field engine)
section: volume

# Look
swatch: 249 103 251
icon: pen hot 0.05
icon: line 0.18 0.55 0.82 0.45
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.55 0.28

# Playback — ribbons: soft neon tube in room space (true volumeMain)
space: world
ribbons: arc
knobs: speed pulse

# Effect — ribbon_* samples the GPU tube at this LED
v = ribbon_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
