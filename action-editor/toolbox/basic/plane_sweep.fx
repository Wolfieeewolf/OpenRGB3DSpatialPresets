# Name
name: Plane sweep
description: GPU plane wipe through the room volume
section: basic

# Look
swatch: 80 180 255
icon: brush dim
icon: rect 0.08 0.2 0.84 0.6
icon: brush hot
icon: rect 0.42 0.12 0.12 0.76

# Playback
space: world
shader: plane_sweep
knobs: speed pulse

# Effect
v = field_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
