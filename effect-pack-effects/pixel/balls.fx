# Name
name: Balls
description: Four glowing balls move through the room
section: pixel

# Look
swatch: 80 140 255
icon: brush hot
icon: ellipse 0.1 0.14 0.4 0.4
icon: brush ink
icon: ellipse 0.5 0.46 0.4 0.4

# Playback
space: world
knobs: speed pulse

# Effect
rad = clamp(pulse * 0.5, 0.1, 0.35)
best = 1
i = 0
while i < 4
ph = i / 4
cx = 0.5 + 0.38 * sin((progress + ph) * 6.2831853)
cy = 0.5 + 0.38 * cos((progress * 1.2 + ph) * 6.2831853)
cz = 0.5 + 0.38 * sin((progress * 0.85 + ph * 1.7) * 6.2831853)
ddx = nx - cx
ddy = ny - cy
ddz = nz - cz
diag = max(0.00001, sqrt(span_x * span_x + span_y * span_y + span_z * span_z))
eps = diag * 0.02
if span_x <= eps
ddx = 0
end
if span_y <= eps
ddy = 0
end
if span_z <= eps
ddz = 0
end
best = min(best, sqrt(ddx * ddx + ddy * ddy + ddz * ddz))
i = i + 1
end
if best > rad
off
end
intensity = intensity * (1 - best / rad)
paint(progress)
