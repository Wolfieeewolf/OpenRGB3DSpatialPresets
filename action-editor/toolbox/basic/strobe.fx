# Name
name: Strobe
description: Hard on and off flashes
section: basic

# Look
swatch: 255 255 255
icon: brush dark
icon: round 0.08 0.08 0.84 0.84 0.12
icon: brush hot
icon: ellipse 0.28 0.28 0.44 0.44

# Playback
space: axis
knobs: speed period pulse

# Effect
spd = max(0.05, speed)
period = max(40, floor(max(40, period_ms) / spd + 0.5))
phase = fmod(time_ms, period) / period
duty = clamp(pulse, 0.05, 0.95)
if phase > duty
off
end
paint(progress)
