# Name
name: Ripple
description: Rings expand from the centre
section: volume

# Look
swatch: 40 220 255
icon: pen ink 0.06
icon: ellipse 0.1 0.1 0.8 0.8
icon: pen hot 0.06
icon: ellipse 0.28 0.28 0.44 0.44

# Playback
space: world
knobs: speed pulse

# Effect
band = clamp(pulse, 0.06, 0.4)
cover = 0
wave = abs(radius - progress)
if wave <= band
cover = 1 - (wave / band)
end
wave2 = abs(radius - fmod(progress + 0.5, 1))
if wave2 <= band
cover = max(cover, (1 - (wave2 / band)) * 0.75)
end
if cover <= 0.001
off
end
intensity = intensity * cover
paint(radius)
