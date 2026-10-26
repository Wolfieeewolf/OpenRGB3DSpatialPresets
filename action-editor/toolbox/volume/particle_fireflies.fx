# Name
name: Particle fireflies
description: Soft firefly points drifting and blinking
section: volume

# Look
swatch: 255 220 80
icon: pen hot 0.05
icon: ellipse 0.35 0.4 0.08 0.08
icon: ellipse 0.62 0.55 0.07 0.07
icon: ellipse 0.48 0.7 0.06 0.06
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.5 0.5

# Playback
space: world
particles: fireflies
knobs: speed pulse

# Effect
v = particle_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
