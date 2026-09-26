# Name
name: Confetti
description: Random specks pick colours from the gradient
section: volume

# Look
swatch: 255 80 160
icon: brush hot
icon: rect 0.18 0.18 0.16 0.16
icon: brush ink
icon: rect 0.62 0.32 0.16 0.16
icon: brush dim
icon: rect 0.36 0.66 0.16 0.16

# Playback
space: axis
knobs: speed period min_intensity

# Effect
spd = max(0.05, speed)
period = max(60, floor(max(60, period_ms) / spd + 0.5))
local = max(0, time_ms)
epoch = floor(local / period)
roll = hash_byte(xor(seed, 12648430), epoch, period, 0)
density = 0.25 + 0.5 * clamp(intensity, 0, 1)
if roll > density
off
end
pick = hash_byte(xor(seed, 12648430), epoch, period, 8)
paint(pick)
