# Name
name: Pulse
description: Brightness breathes between the min and max
section: basic

# Look
swatch: 255 60 90
icon: gradr 0.12 0.12 0.76 0.76 hot dim
icon: ellipse 0.12 0.12 0.76 0.76

# Playback
space: axis
knobs: speed period min_intensity
preview: pulse

# Effect
spd = max(0.05, speed)
period = max(1, floor(max(1, period_ms) / spd + 0.5))
phase = fmod(time_ms, period) / period
wave = 0.5 - 0.5 * cos(phase * 6.28318530718)
lo = clamp(min_intensity, 0, 1)
hi = clamp(max_intensity, 0, 1)
intensity = intensity * (lo + (hi - lo) * wave)
paint(phase)
