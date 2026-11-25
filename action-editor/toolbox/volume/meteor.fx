# Name
name: Meteor
description: Bright head with a fading trail through the room
section: volume

# Look
swatch: 255 160 40
icon: gradh 0.12 0.4 0.62 0.2 none hot
icon: round 0.12 0.4 0.62 0.2 0.1
icon: brush hot
icon: ellipse 0.68 0.36 0.22 0.28

# Playback
space: world
knobs: speed direction pulse

# Effect
along = sample_axis()
trail = clamp(pulse, 0.08, 0.55)
head0 = fmod(time_ms * 0.001 * max(0.05, speed), 1)
if invert == 1
head0 = 1 - head0
end
# Bright head with a halo; the tail tapers and crackles as it burns
delta = head0 - along
cover = glow(delta, 0.025 + trail * 0.06)
if delta >= 0 && delta <= trail
burn = 0.65 + 0.35 * noise(along * 35, time_ms * 0.015, 7)
cover = max(cover, pow(1 - delta / trail, 1.8) * burn)
end
if cover <= 0.02
h = hash01_mix(seed, 2654435761, 0)
head = fmod(head0 + h * 0.85, 1)
delta = head - along
cover = glow(delta, 0.025 + trail * 0.06) * 0.85
if delta >= 0 && delta <= trail
cover = max(cover, pow(1 - delta / trail, 1.8) * 0.85)
end
end
if cover <= 0.02
off
end
intensity = intensity * cover
paint(cover)
