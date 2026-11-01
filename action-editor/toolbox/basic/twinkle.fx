# Name
name: Twinkle
description: Random LEDs flash and go dark
section: basic

# Look
swatch: 255 230 140
icon: brush hot
icon: ellipse 0.14 0.18 0.16 0.16
icon: ellipse 0.68 0.28 0.12 0.12
icon: brush ink
icon: ellipse 0.42 0.46 0.2 0.2
icon: ellipse 0.28 0.7 0.12 0.12

# Playback
space: axis
knobs: speed period min_intensity

# Effect
spd = max(0.05, speed)
period = max(80, floor(max(80, period_ms) / spd + 0.5))
phase0 = hash_phase(seed, 0, 1)
density = 0.12 + 0.55 * clamp(intensity, 0, 1)
local = max(0, time_ms)
epoch = floor(local / period)
roll = hash_byte(xor(seed, 42405), epoch * period, period, 8)
flash = 0
if roll < density
phase_ms = fmod(local + floor(phase0 * period + 0.5), period)
phase = phase_ms / period
win = 0.30
if phase < win
flash = sin((phase / win) * 3.141592653589793)
end
end
lo = clamp(min_intensity, 0, 1)
hi = clamp(max_intensity, lo, 1)
intensity = intensity * (lo + (hi - lo) * flash)
paint_mix(phase0, min(1, phase0 + 0.35), flash)
