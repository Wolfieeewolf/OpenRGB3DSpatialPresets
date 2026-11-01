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
delta = head0 - along
cover = 0
if delta >= 0 && delta <= trail
cover = 1 - (delta / trail)
end
if cover <= 0.001
h = hash01_mix(seed, 2654435761, 0)
head = fmod(head0 + h * 0.85, 1)
delta = head - along
if delta >= 0 && delta <= trail
cover = (1 - (delta / trail)) * 0.85
end
end
if cover <= 0.001
off
end
intensity = intensity * cover
paint(cover)
