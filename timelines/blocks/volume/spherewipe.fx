# Name
name: Sphere Wipe
description: A sphere grows from the centre
section: volume
alias: sphere_wipe

# Look
swatch: 80 180 255
icon: pen dim 0.06
icon: ellipse 0.1 0.1 0.8 0.8
icon: pen none 0
icon: brush hot
icon: ellipse 0.3 0.3 0.4 0.4

# Playback
space: world
knobs: speed

# Effect
edge = 0.12
front = progress * (1 + 2 * edge) - edge
d = front - radius
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
paint(progress)
