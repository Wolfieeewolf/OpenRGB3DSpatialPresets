# Name
name: Particle swarm
description: Flocking particle cloud drifting through the room
section: volume

# Look
swatch: 64 220 240
icon: pen hot 0.05
icon: ellipse 0.32 0.42 0.07 0.07
icon: ellipse 0.48 0.38 0.06 0.06
icon: ellipse 0.58 0.52 0.07 0.07
icon: ellipse 0.70 0.45 0.05 0.05
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.52 0.42

# Playback
space: world
particles: swarm
knobs: speed pulse

# Effect
v = particle_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
