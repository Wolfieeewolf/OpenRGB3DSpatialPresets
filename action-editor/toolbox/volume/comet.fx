# Name
name: Comet
description: Bright head with a short trail along the row
section: volume

# Look
swatch: 180 220 255
icon: gradh 0.08 0.4 0.7 0.2 none hot
icon: round 0.08 0.4 0.7 0.2 0.1
icon: brush hot
icon: ellipse 0.72 0.36 0.2 0.26

# Playback
space: axis
knobs: speed direction pulse

# Effect
head = fmod(time_ms * 0.001 * max(0.05, speed), 1)
along = axis
if invert == 1
head = 1 - head
along = 1 - along
end
trail = clamp(pulse, 0.08, 0.7)
delta = head - along
if delta < 0 || delta > trail
off
end
cover = 1 - (delta / trail)
intensity = intensity * cover
paint(cover)
