# Name
name: Spark burst
description: GPU particle burst sampled onto room LEDs (Action particle-field engine)
section: volume

# Look
swatch: 255 180 80
icon: pen hot 0.05
icon: ellipse 0.5 0.5 0.12 0.12
icon: pen none 0
icon: brush ink
icon: ellipse 0.5 0.5 0.42 0.42
icon: ellipse 0.5 0.5 0.62 0.62

# Playback — particles: burst density cloud in room space (true volumeMain)
space: world
particles: burst
knobs: speed pulse

# Effect — particle_* samples the GPU field at this LED
v = particle_paint()
if v <= 0.001
off
end
intensity = intensity * clamp(v, 0, 1)
