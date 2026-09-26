# Name
name: Fire
description: Heat rises and flickers
section: pixel

# Look
swatch: 255 90 20
icon: gradv 0.22 0.14 0.22 0.72 hot dim
icon: round 0.22 0.14 0.22 0.72 0.1
icon: gradv 0.52 0.32 0.22 0.54 hot dim
icon: round 0.52 0.32 0.22 0.54 0.1

# Playback
space: world
knobs: speed

# Effect
rise = 1 - height
flicker = noise(nx * 4, height * 3 + progress * 6, nz * 4)
heat = clamp(rise * (0.4 + 0.6 * flicker) + 0.08 * flicker, 0, 1)
if heat < 0.08
off
end
intensity = intensity * heat
paint(heat)
