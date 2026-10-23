# Name
name: Wave
description: A sine wave travels along the row
section: basic

# Look
swatch: 40 180 255
icon: pen hot 0.1
icon: poly 0.08 0.5 0.32 0.18 0.68 0.82 0.92 0.5

# Playback
space: axis
knobs: speed direction pulse

# Effect
cycles = max(0.25, speed)
phase = progress * cycles * 6.2831853
wave = 0.5 + 0.5 * sin((axis * 6.2831853 * max(0.5, pulse * 4)) - phase)
intensity = intensity * clamp(wave, 0, 1)
paint(axis)
