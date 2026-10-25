# Name
name: Field strata
description: GPU height-banded wash sampled onto room LEDs (Action volume-field engine)
section: volume

# Look
swatch: 60 140 220
icon: pen hot 0.04
icon: line 0.15 0.35 0.85 0.35
icon: line 0.15 0.55 0.85 0.55
icon: line 0.15 0.72 0.85 0.72
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.55 0.7 0.45

# Playback
space: world
shader: strata_wash
knobs: speed pulse

# Effect
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
