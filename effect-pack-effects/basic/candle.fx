# Name
name: Candle
description: A small irregular flicker
section: basic
alias: candle_flicker

# Look
swatch: 255 140 40
icon: brush ink
icon: poly 0.5 0.1 0.82 0.55 0.5 0.9 0.18 0.55
icon: brush hot
icon: ellipse 0.40 0.42 0.2 0.28

# Playback
space: axis
knobs: min_intensity

# Effect
n1 = hash_byte(seed, floor(local_ms / 30), 30, 0)
n2 = hash_byte(seed, floor(local_ms / 30), 30, 8)
flicker = 0.55 + 0.45 * (0.65 * n1 + 0.35 * n2)
lo = clamp(min_intensity, 0, 1)
hi = clamp(max_intensity, lo, 1)
intensity = intensity * (lo + (hi - lo) * flicker)
paint(0.15 + 0.7 * n1)
