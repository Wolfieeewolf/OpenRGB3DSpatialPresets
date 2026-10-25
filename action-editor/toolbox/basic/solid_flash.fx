# Name
name: Solid flash
description: GPU full-volume flash sampled onto room LEDs
section: basic

# Look
swatch: 255 255 255
icon: brush hot
icon: round 0.1 0.1 0.8 0.8 0.12

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
