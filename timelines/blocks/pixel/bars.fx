# Name
name: Bars
description: Stripes travel along the aim direction
section: pixel

# Look
swatch: 255 200 40
icon: brush ink
icon: rect 0.14 0.55 0.14 0.35
icon: rect 0.32 0.35 0.14 0.55
icon: brush hot
icon: rect 0.5 0.2 0.14 0.7
icon: brush ink
icon: rect 0.68 0.45 0.14 0.45

# Playback
space: world
knobs: speed direction

# Effect
along = sample_axis()
pos = fmod(along * 5 + progress * 5 + 1, 1)
if pos > 0.62
off
end
intensity = intensity * (0.75 + 0.25 * (1 - pos / 0.62))
paint(along)
