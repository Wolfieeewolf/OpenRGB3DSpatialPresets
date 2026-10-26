# Name
name: Spark burst
description: Explosive spark cloud from the room centre
section: volume

# Look
swatch: 255 180 80
icon: pen hot 0.05
icon: ellipse 0.5 0.5 0.12 0.12
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.42 0.42
icon: ellipse 0.5 0.5 0.62 0.62

# Playback
space: world
particles: burst
knobs: speed pulse

# Effect
v = particle_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
