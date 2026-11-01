# Name
name: Particle meteors
description: Meteor streaks diving through the room
section: volume

# Look
swatch: 255 160 40
icon: pen hot 0.05
icon: line 0.2 0.15 0.7 0.85
icon: line 0.45 0.1 0.85 0.7
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.5 0.5

# Playback
space: world
particles: meteors
knobs: speed pulse

# Effect
v = particle_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
