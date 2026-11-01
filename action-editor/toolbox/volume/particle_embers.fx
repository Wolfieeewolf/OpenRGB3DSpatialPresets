# Name
name: Particle embers
description: Slow rising gold embers floating through the room
section: volume

# Look
swatch: 255 140 40
icon: pen hot 0.05
icon: ellipse 0.3 0.7 0.08 0.08
icon: ellipse 0.55 0.45 0.07 0.07
icon: ellipse 0.72 0.25 0.06 0.06
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.5 0.5

# Playback
space: world
particles: embers
knobs: speed pulse

# Effect
v = particle_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
