# Name
name: Sparse spark
description: GPU sparse sparkle field sampled onto room LEDs
section: pixel

# Look
swatch: 255 220 120
icon: brush hot
icon: ellipse 0.18 0.22 0.12 0.12
icon: ellipse 0.62 0.48 0.1 0.1
icon: ellipse 0.4 0.7 0.14 0.14

# Playback
space: world
shader: sparse_spark
knobs: speed pulse

# Effect
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
