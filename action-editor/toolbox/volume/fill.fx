# Name
name: Fill
description: Colour closes in from the outside
section: volume

# Look
swatch: 255 120 200
icon: brush dim
icon: ellipse 0.1 0.1 0.8 0.8
icon: brush hot
icon: ellipse 0.3 0.3 0.4 0.4

# Playback
space: world
knobs: speed

# Effect
edge = 0.12
front = (1 - progress) * (1 + edge)
d = radius - front
cover = 0
if d >= edge
cover = 1
else
if d > -edge
cover = (d + edge) / (2 * edge)
end
end
if cover <= 0.001
off
end
intensity = intensity * cover
paint(radius)
