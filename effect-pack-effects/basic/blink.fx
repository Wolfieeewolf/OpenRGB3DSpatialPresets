# Name
name: Blink
description: On for half the period, then off
section: basic

# Look
swatch: 255 255 80
icon: brush hot
icon: round 0.16 0.22 0.68 0.56 0.12

# Playback
space: axis
knobs: speed period

# Effect
spd = max(0.05, speed)
period = max(40, floor(max(40, period_ms) / spd + 0.5))
phase = fmod(time_ms, period) / period
if phase >= 0.5
off
end
paint(0)
