# Name
name: Color Wash
description: Colour drifts along the row
section: basic
alias: color_wash

# Look
swatch: 80 255 180
icon: gradd 0.08 0.08 0.84 0.84 dim hot
icon: round 0.08 0.08 0.84 0.84 0.12

# Playback
space: axis
knobs: speed direction

# Effect
t = time_ms * 0.001 * max(0.05, speed) + axis * 0.35
t = t - floor(t)
paint(t)
