# Name
name: Spin
description: A wedge rotates around the target
section: basic

# Look
swatch: 180 80 255
icon: pen dim 0.1
icon: ellipse 0.16 0.16 0.68 0.68
icon: pen none 0
icon: brush hot
icon: pie 0.16 0.16 0.68 0.68 40 70

# Playback
space: axis
knobs: speed direction pulse
axis: angle

# Effect
width = clamp(pulse, 0.04, 0.55)
delta = axis - progress
delta = delta - floor(delta + 0.5)
lead = width * 0.22
trail = width
cover = 0
if delta >= 0 && delta <= lead
cover = 1 - (delta / lead)
end
if delta < 0 && -delta <= trail
cover = 1 + (delta / trail)
end
if width <= 0.28
delta2 = delta - 0.5
if delta < 0
delta2 = delta + 0.5
end
delta2 = delta2 - floor(delta2 + 0.5)
cover2 = 0
if delta2 >= 0 && delta2 <= lead
cover2 = 1 - (delta2 / lead)
end
if delta2 < 0 && -delta2 <= trail
cover2 = 1 + (delta2 / trail)
end
cover = max(cover, cover2 * 0.85)
end
if cover <= 0.001
off
end
intensity = intensity * cover
paint(progress)
