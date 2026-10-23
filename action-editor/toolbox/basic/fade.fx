# Name
name: Fade
description: Moves along the gradient from start to end
section: basic

# Look
swatch: 255 180 40
icon: gradh 0.06 0.22 0.88 0.56 dim hot
icon: round 0.06 0.22 0.88 0.56 0.12

# Playback
space: axis
knobs: color_to
color: ends

# Effect
t = time_ms / max(1, (period_ms))
paint(progress)
