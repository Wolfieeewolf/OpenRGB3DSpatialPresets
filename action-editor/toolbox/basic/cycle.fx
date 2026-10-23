# Name
name: Cycle
description: The whole target cycles the gradient
section: basic

# Look
swatch: 255 0 180
icon: pen hot 0.1
icon: arc 0.16 0.16 0.68 0.68 30 280

# Playback
space: axis
knobs: speed period

# Effect
spd = max(0.05, speed)
period = max(1, floor(max(1, period_ms) / spd + 0.5))
phase = fmod(time_ms, period) / period
paint(phase)
