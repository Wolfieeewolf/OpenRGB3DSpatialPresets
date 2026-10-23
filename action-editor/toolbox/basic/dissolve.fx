# Name
name: Dissolve
description: LEDs appear in a random order
section: basic

# Look
swatch: 200 200 210
icon: brush hot
icon: rect 0.16 0.16 0.16 0.16
icon: rect 0.68 0.68 0.16 0.16
icon: brush dim
icon: rect 0.42 0.16 0.16 0.16
icon: rect 0.16 0.42 0.16 0.16
icon: rect 0.68 0.42 0.16 0.16
icon: rect 0.42 0.68 0.16 0.16

# Playback
space: axis
knobs: speed

# Effect
threshold = hash_phase(xor(seed, 1597463007), 0, 1)
if progress + 0.001 < threshold
off
end
cover = 1
if progress < threshold + 0.08
cover = (progress - threshold) / 0.08
end
intensity = intensity * clamp(cover, 0, 1)
paint(threshold)
