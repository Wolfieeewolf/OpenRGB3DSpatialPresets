# Name
name: Alternating
description: Neighbouring LEDs swap colours
section: basic

# Look
swatch: 255 40 40
icon: brush hot
icon: rect 0.08 0.18 0.40 0.64
icon: brush dim
icon: rect 0.52 0.18 0.40 0.64

# Playback
space: axis
knobs: speed period direction

# Effect
spd = max(0.05, speed)
period = max(50, floor(max(50, period_ms) / spd + 0.5))
phase_bit = band(floor(time_ms / period), 1)
led_bit = band(xor(seed, floor(axis * 1024 + 0.5)), 1)
if xor(led_bit, phase_bit) == 0
paint(0)
else
paint(1)
end
