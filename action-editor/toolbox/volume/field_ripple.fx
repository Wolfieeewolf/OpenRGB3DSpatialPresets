# Name
name: Field ripple
description: GPU volume ripple sampled onto room LEDs (Action volume-field engine)
section: volume

# Look
swatch: 100 200 255
icon: pen hot 0.05
icon: ellipse 0.5 0.5 0.55 0.55
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.28 0.28

# Playback — shader body: sibling ripple_ring.fs (or effects/volume/)
space: world
shader: ripple_ring
knobs: speed pulse

# Effect — field_* samples the prepared GPU atlas at this LED
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
