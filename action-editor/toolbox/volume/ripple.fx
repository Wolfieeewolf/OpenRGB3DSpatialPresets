# Name
name: Ripple
description: Rings expand from the centre
section: volume

# Look
swatch: 40 220 255
icon: pen ink 0.06
icon: ellipse 0.1 0.1 0.8 0.8
icon: pen hot 0.06
icon: ellipse 0.28 0.28 0.44 0.44

# Playback
space: world
knobs: speed pulse

# Effect
band = clamp(pulse, 0.06, 0.4)
t = time_ms * 0.001 * max(0.05, speed)
front = fmod(t, 1)
# Rings wobble a little so they read as water, not a perfect sphere
r = radius + (noise(nx * 4, ny * 4 + t, nz * 4) - 0.5) * band * 0.6
cover = glow(r - front, band * 0.7)
cover = max(cover, glow(r - fmod(front + 0.5, 1), band * 0.7) * 0.75)
if cover <= 0.02
off
end
intensity = intensity * cover
paint(radius)
