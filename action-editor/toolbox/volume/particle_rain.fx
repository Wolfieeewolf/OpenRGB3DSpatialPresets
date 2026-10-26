# Name
name: Particle rain
description: Rain streaks with wind shear through the room
section: volume

# Look
swatch: 140 190 255
icon: pen hot 0.04
icon: line 0.3 0.15 0.3 0.85
icon: line 0.5 0.1 0.5 0.9
icon: line 0.7 0.2 0.7 0.8
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.55 0.55

# Playback
space: world
particles: rain
knobs: speed pulse

# Effect
v = particle_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
