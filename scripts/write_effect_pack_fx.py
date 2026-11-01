from pathlib import Path

root = Path(r"f:\MCP\OpenRGB3DSpatialPresets\effect-pack-effects")

files = {}

def add(section, name, body):
    files[f"{section}/{name}.fx"] = body.strip() + "\n"

add("basic", "solid", """
name: Set Level
description: Holds one colour for the whole block
section: basic
swatch: 255 80 40
space: axis
paint(0)
""")

add("basic", "fade", """
name: Fade
description: Moves along the gradient from start to end
section: basic
swatch: 255 180 40
space: axis
t = time_ms / max(1, (period_ms))
paint(progress)
""")

add("basic", "pulse", """
name: Pulse
description: Brightness breathes between the min and max
section: basic
swatch: 255 60 90
space: axis
spd = max(0.05, speed)
period = max(1, floor(max(1, period_ms) / spd + 0.5))
phase = fmod(time_ms, period) / period
wave = 0.5 - 0.5 * cos(phase * 6.28318530718)
lo = clamp(min_intensity, 0, 1)
hi = clamp(max_intensity, 0, 1)
intensity = intensity * (lo + (hi - lo) * wave)
paint(phase)
""")

add("basic", "wipe", """
name: Wipe
description: A soft front sweeps along the row
section: basic
swatch: 80 160 255
space: axis
edge = 0.08
front = progress * (1 + 2 * edge) - edge
d = front - axis
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
""")

add("basic", "chase", """
name: Chase
description: A bright head runs along the row
section: basic
swatch: 80 220 255
space: axis
head = clamp(pulse, 0.02, 1)
delta = abs(axis - progress)
delta = min(delta, 1 - delta)
if delta > head
off
end
intensity = intensity * (1 - (delta / head))
paint(progress)
""")

add("basic", "twinkle", """
name: Twinkle
description: Random LEDs flash and go dark
section: basic
swatch: 255 230 140
space: axis
spd = max(0.05, speed)
period = max(80, floor(max(80, period_ms) / spd + 0.5))
phase0 = hash_phase(seed, 0, 1)
density = 0.12 + 0.55 * clamp(intensity, 0, 1)
local = max(0, time_ms)
epoch = floor(local / period)
roll = hash_byte(xor(seed, 42405), epoch * period, period, 8)
flash = 0
if roll < density
phase_ms = fmod(local + floor(phase0 * period + 0.5), period)
phase = phase_ms / period
win = 0.30
if phase < win
flash = sin((phase / win) * 3.141592653589793)
end
end
lo = clamp(min_intensity, 0, 1)
hi = clamp(max_intensity, lo, 1)
intensity = intensity * (lo + (hi - lo) * flash)
paint_mix(phase0, min(1, phase0 + 0.35), flash)
""")

add("basic", "alternating", """
name: Alternating
description: Neighbouring LEDs swap colours
section: basic
swatch: 255 40 40
space: axis
spd = max(0.05, speed)
period = max(50, floor(max(50, period_ms) / spd + 0.5))
phase_bit = band(floor(time_ms / period), 1)
led_bit = band(xor(seed, floor(axis * 1024 + 0.5)), 1)
if xor(led_bit, phase_bit) == 0
paint(0)
else
paint(1)
end
""")

add("basic", "strobe", """
name: Strobe
description: Hard on and off flashes
section: basic
swatch: 255 255 255
space: axis
spd = max(0.05, speed)
period = max(40, floor(max(40, period_ms) / spd + 0.5))
phase = fmod(time_ms, period) / period
duty = clamp(pulse, 0.05, 0.95)
if phase > duty
off
end
paint(progress)
""")

add("basic", "spin", """
name: Spin
description: A wedge rotates around the target
section: basic
swatch: 180 80 255
space: axis
width = clamp(pulse, 0.04, 0.55)
delta = axis - progress
delta = delta - floor(delta + 0.5)
lead = width * 0.22
trail = width
cover = 0
if delta >= 0 && delta <= lead
cover = 1 - (delta / lead)
end
if delta < 0 && -delta <= trail
cover = 1 + (delta / trail)
end
if width <= 0.28
delta2 = delta - 0.5
if delta < 0
delta2 = delta + 0.5
end
delta2 = delta2 - floor(delta2 + 0.5)
cover2 = 0
if delta2 >= 0 && delta2 <= lead
cover2 = 1 - (delta2 / lead)
end
if delta2 < 0 && -delta2 <= trail
cover2 = 1 + (delta2 / trail)
end
cover = max(cover, cover2 * 0.85)
end
if cover <= 0.001
off
end
intensity = intensity * cover
paint(progress)
""")

add("basic", "candle", """
name: Candle
description: A small irregular flicker
section: basic
swatch: 255 140 40
space: axis
n1 = hash_byte(seed, floor(local_ms / 30), 30, 0)
n2 = hash_byte(seed, floor(local_ms / 30), 30, 8)
flicker = 0.55 + 0.45 * (0.65 * n1 + 0.35 * n2)
lo = clamp(min_intensity, 0, 1)
hi = clamp(max_intensity, lo, 1)
intensity = intensity * (lo + (hi - lo) * flicker)
paint(0.15 + 0.7 * n1)
""")

add("basic", "dissolve", """
name: Dissolve
description: LEDs appear in a random order
section: basic
swatch: 200 200 210
space: axis
threshold = hash_phase(xor(seed, 1597463007), 0, 1)
if progress + 0.001 < threshold
off
end
cover = 1
if progress < threshold + 0.08
cover = (progress - threshold) / 0.08
end
intensity = intensity * clamp(cover, 0, 1)
paint(threshold)
""")

add("basic", "wave", """
name: Wave
description: A sine wave travels along the row
section: basic
swatch: 40 180 255
space: axis
cycles = max(0.25, speed)
phase = progress * cycles * 6.2831853
wave = 0.5 + 0.5 * sin((axis * 6.2831853 * max(0.5, pulse * 4)) - phase)
intensity = intensity * clamp(wave, 0, 1)
paint(axis)
""")

add("basic", "cycle", """
name: Cycle
description: The whole target cycles the gradient
section: basic
swatch: 255 0 180
space: axis
spd = max(0.05, speed)
period = max(1, floor(max(1, period_ms) / spd + 0.5))
phase = fmod(time_ms, period) / period
paint(phase)
""")

add("basic", "blink", """
name: Blink
description: On for half the period, then off
section: basic
swatch: 255 255 80
space: axis
spd = max(0.05, speed)
period = max(40, floor(max(40, period_ms) / spd + 0.5))
phase = fmod(time_ms, period) / period
if phase >= 0.5
off
end
paint(0)
""")

add("basic", "colorwash", """
name: Color Wash
description: Colour drifts along the row
section: basic
swatch: 80 255 180
space: axis
t = progress + axis * 0.35
t = t - floor(t)
paint(t)
""")

add("pixel", "plasma", """
name: Plasma
description: A swirling noise field
section: pixel
swatch: 180 40 255
space: world
t = progress * 3
n = noise(nx * 2.5 + t, ny * 2.5 - t * 0.6, nz * 2.5 + t * 0.35)
n2 = noise(nx * 5 - t * 0.5, ny * 5 + t * 0.4, nz * 5)
field = clamp(0.5 * n + 0.5 * n2, 0, 1)
intensity = intensity * (0.35 + 0.65 * field)
paint(field)
""")

add("pixel", "snow", """
name: Snow
description: Flakes fall through the volume
section: pixel
swatch: 220 235 255
space: world
h = hash01_mix(seed, 9743197, 17)
fall = fmod(progress + h, 1)
flake_h = 1 - fall
drift = 0.12 * sin(fall * 10 + h * 8)
plane = nx
if height == nx
if span_z >= span_y
plane = nz
else
plane = ny
end
else
if height == ny
if span_x >= span_z
plane = nx
else
plane = nz
end
else
if span_x >= span_y
plane = nx
else
plane = ny
end
end
end
fx = fmod(h + drift + 1, 1)
d = abs(plane - fx) * 1.2 + abs(height - flake_h)
if d > 0.16
off
end
intensity = intensity * (1 - d / 0.16)
paint(h)
""")

add("pixel", "fire", """
name: Fire
description: Heat rises and flickers
section: pixel
swatch: 255 90 20
space: world
rise = 1 - height
flicker = noise(nx * 4, height * 3 + progress * 6, nz * 4)
heat = clamp(rise * (0.4 + 0.6 * flicker) + 0.08 * flicker, 0, 1)
if heat < 0.08
off
end
intensity = intensity * heat
paint(heat)
""")

add("pixel", "balls", """
name: Balls
description: Four glowing balls move through the room
section: pixel
swatch: 80 140 255
space: world
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
""")

add("pixel", "bars", """
name: Bars
description: Stripes travel along the aim direction
section: pixel
swatch: 255 200 40
space: world
along = sample_axis()
pos = fmod(along * 5 + progress * 5 + 1, 1)
if pos > 0.62
off
end
intensity = intensity * (0.75 + 0.25 * (1 - pos / 0.62))
paint(along)
""")

add("pixel", "scanner", """
name: Scanner
description: A bar travels out and back
section: pixel
swatch: 40 255 120
space: axis
head = progress * 2
if head > 1
head = 2 - head
end
if invert == 1
head = 1 - head
end
half_w = max(0.02, pulse * 0.5)
dist = abs(axis - head)
cover = 0
if dist <= half_w
cover = 1 - (dist / half_w)
end
if cover <= 0.001
off
end
intensity = intensity * cover
paint(head)
""")

add("volume", "spherewipe", """
name: Sphere Wipe
description: A sphere grows from the centre
section: volume
swatch: 80 180 255
space: world
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
""")

add("volume", "orbit", """
name: Orbit
description: A band rotates around the aim axis
section: volume
swatch: 255 80 200
space: world
ux = axis_x()
uy = axis_y()
uz = axis_z()
rx = 0
ry = 1
rz = 0
if abs(uy) > 0.9
rx = 1
ry = 0
end
tx = ry * uz - rz * uy
ty = rz * ux - rx * uz
tz = rx * uy - ry * ux
tlen = max(0.000001, sqrt(tx * tx + ty * ty + tz * tz))
tx = tx / tlen
ty = ty / tlen
tz = tz / tlen
bx = uy * tz - uz * ty
by = uz * tx - ux * tz
bz = ux * ty - uy * tx
u = dx * tx + dy * ty + dz * tz
v = dx * bx + dy * by + dz * bz
ang = atan2(v, u) / (2 * 3.141592653589793) + 0.5
ang = ang - floor(ang)
width = clamp(pulse, 0.06, 0.5)
delta = ang - progress
delta = delta - floor(delta + 0.5)
lead = width * 0.25
trail = width
cover = 0
if delta >= 0 && delta <= lead
cover = 1 - (delta / lead)
end
if delta < 0 && -delta <= trail
cover = 1 + (delta / trail)
end
cover = cover * (0.45 + 0.55 * (1 - clamp(radius, 0, 1) * 0.5))
if cover <= 0.001
off
end
intensity = intensity * cover
paint(progress)
""")

add("volume", "ripple", """
name: Ripple
description: Rings expand from the centre
section: volume
swatch: 40 220 255
space: world
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
""")

add("volume", "meteor", """
name: Meteor
description: A bright head with a fading trail
section: volume
swatch: 255 160 40
space: world
along = sample_axis()
trail = clamp(pulse, 0.08, 0.55)
delta = progress - along
cover = 0
if delta >= 0 && delta <= trail
cover = 1 - (delta / trail)
end
if cover <= 0.001
h = hash01_mix(seed, 2654435761, 0)
head = fmod(progress + h * 0.85, 1)
delta = head - along
if delta >= 0 && delta <= trail
cover = (1 - (delta / trail)) * 0.85
end
end
if cover <= 0.001
off
end
intensity = intensity * cover
paint(cover)
""")

add("volume", "noise3d", """
name: Noise
description: A moving 3D noise field
section: volume
swatch: 140 80 255
space: world
t = progress * 3
n = noise(nx * 2.5 + t, ny * 2.5 - t * 0.6, nz * 2.5 + t * 0.35)
n2 = noise(nx * 5 - t * 0.5, ny * 5 + t * 0.4, nz * 5)
field = clamp(0.5 * n + 0.5 * n2, 0, 1)
intensity = intensity * (0.35 + 0.65 * field)
paint(field)
""")

add("volume", "burst", """
name: Burst
description: A shell expands outward
section: volume
swatch: 255 60 40
space: world
edge = clamp(pulse, 0.06, 0.45)
front = progress * (1 + 2 * edge)
d = front - radius
cover = 0
if d >= 0 && d <= edge
cover = 1 - (d / edge)
end
if d < 0 && -d <= edge * 0.35
cover = 1 + (d / (edge * 0.35))
end
if progress < 0.2
cover = max(cover, (1 - progress / 0.2) * (1 - clamp(radius, 0, 1)))
end
if cover <= 0.001
off
end
intensity = intensity * cover
paint(clamp(radius, 0, 1))
""")

add("volume", "confetti", """
name: Confetti
description: Random specks pick colours from the gradient
section: volume
swatch: 255 80 160
space: axis
spd = max(0.05, speed)
period = max(60, floor(max(60, period_ms) / spd + 0.5))
local = max(0, time_ms)
epoch = floor(local / period)
roll = hash_byte(xor(seed, 12648430), epoch, period, 0)
density = 0.25 + 0.5 * clamp(intensity, 0, 1)
if roll > density
off
end
pick = hash_byte(xor(seed, 12648430), epoch, period, 8)
paint(pick)
""")

add("volume", "comet", """
name: Comet
description: A head with a short trail
section: volume
swatch: 180 220 255
space: axis
head = progress
along = axis
if invert == 1
head = 1 - head
along = 1 - along
end
trail = clamp(pulse, 0.08, 0.7)
delta = head - along
if delta < 0 || delta > trail
off
end
cover = 1 - (delta / trail)
intensity = intensity * cover
paint(cover)
""")

add("volume", "helix", """
name: Helix
description: A twisted ribbon around the aim axis
section: volume
swatch: 80 255 200
space: world
along = sample_axis()
ux = axis_x()
uz = axis_z()
side = dx * uz - dz * ux
ang = side * 0.5 + 0.5
ang = ang - floor(ang)
twist = fmod(along * 3 + progress + 1, 1)
delta = abs(ang - twist)
delta = min(delta, 1 - delta)
band = clamp(pulse, 0.06, 0.4)
if delta > band
off
end
intensity = intensity * (1 - (delta / band))
paint(along)
""")

add("volume", "fill", """
name: Fill
description: Colour closes in from the outside
section: volume
swatch: 255 120 200
space: world
edge = 0.12
front = (1 - progress) * (1 + edge)
d = radius - front
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
paint(radius)
""")

knobs = {
    "basic/fade.fx": "color_to",
    "basic/pulse.fx": "speed period min_intensity",
    "basic/wipe.fx": "speed direction",
    "basic/chase.fx": "speed direction pulse",
    "basic/twinkle.fx": "speed period min_intensity",
    "basic/alternating.fx": "speed period direction",
    "basic/strobe.fx": "speed period pulse",
    "basic/spin.fx": "speed direction pulse",
    "basic/candle.fx": "min_intensity",
    "basic/dissolve.fx": "speed",
    "basic/wave.fx": "speed direction pulse",
    "basic/cycle.fx": "speed period",
    "basic/blink.fx": "speed period",
    "basic/colorwash.fx": "speed direction",
    "pixel/plasma.fx": "speed",
    "pixel/snow.fx": "speed",
    "pixel/fire.fx": "speed",
    "pixel/balls.fx": "speed pulse",
    "pixel/bars.fx": "speed direction",
    "pixel/scanner.fx": "speed direction pulse",
    "volume/spherewipe.fx": "speed",
    "volume/orbit.fx": "speed direction pulse",
    "volume/ripple.fx": "speed pulse",
    "volume/meteor.fx": "speed direction pulse",
    "volume/noise3d.fx": "speed",
    "volume/burst.fx": "speed pulse",
    "volume/confetti.fx": "speed period min_intensity",
    "volume/comet.fx": "speed direction pulse",
    "volume/helix.fx": "speed direction pulse",
    "volume/fill.fx": "speed",
}
for rel, knob_line in knobs.items():
    text = files[rel]
    text = text.replace("space: axis\n", "space: axis\nknobs: " + knob_line + "\n", 1)
    text = text.replace("space: world\n", "space: world\nknobs: " + knob_line + "\n", 1)
    files[rel] = text

icons = {
    "basic/solid.fx": ["round 0.08 0.08 0.84 0.84 0.12"],
    "basic/fade.fx": ["gradh 0.06 0.22 0.88 0.56 dim hot", "round 0.06 0.22 0.88 0.56 0.12"],
    "basic/pulse.fx": ["gradr 0.12 0.12 0.76 0.76 hot dim", "ellipse 0.12 0.12 0.76 0.76"],
    "basic/wipe.fx": ["brush dim", "rect 0.06 0.22 0.88 0.56", "brush hot", "rect 0.06 0.22 0.44 0.56"],
    "basic/chase.fx": ["brush dim", "ellipse 0.08 0.38 0.16 0.24", "ellipse 0.30 0.38 0.16 0.24", "brush hot", "ellipse 0.52 0.38 0.16 0.24", "brush dim", "ellipse 0.74 0.38 0.16 0.24"],
    "basic/twinkle.fx": ["brush hot", "ellipse 0.14 0.18 0.16 0.16", "ellipse 0.68 0.28 0.12 0.12", "brush ink", "ellipse 0.42 0.46 0.2 0.2", "ellipse 0.28 0.7 0.12 0.12"],
    "basic/alternating.fx": ["brush hot", "rect 0.08 0.18 0.40 0.64", "brush dim", "rect 0.52 0.18 0.40 0.64"],
    "basic/strobe.fx": ["brush dark", "round 0.08 0.08 0.84 0.84 0.12", "brush hot", "ellipse 0.28 0.28 0.44 0.44"],
    "basic/spin.fx": ["pen dim 0.1", "ellipse 0.16 0.16 0.68 0.68", "pen none 0", "brush hot", "pie 0.16 0.16 0.68 0.68 40 70"],
    "basic/candle.fx": ["brush ink", "poly 0.5 0.1 0.82 0.55 0.5 0.9 0.18 0.55", "brush hot", "ellipse 0.40 0.42 0.2 0.28"],
    "basic/dissolve.fx": ["brush hot", "rect 0.16 0.16 0.16 0.16", "rect 0.68 0.68 0.16 0.16", "brush dim", "rect 0.42 0.16 0.16 0.16", "rect 0.16 0.42 0.16 0.16", "rect 0.68 0.42 0.16 0.16", "rect 0.42 0.68 0.16 0.16"],
    "basic/wave.fx": ["pen hot 0.1", "poly 0.08 0.5 0.32 0.18 0.68 0.82 0.92 0.5"],
    "basic/cycle.fx": ["pen hot 0.1", "arc 0.16 0.16 0.68 0.68 30 280"],
    "basic/blink.fx": ["brush hot", "round 0.16 0.22 0.68 0.56 0.12"],
    "basic/colorwash.fx": ["gradd 0.08 0.08 0.84 0.84 dim hot", "round 0.08 0.08 0.84 0.84 0.12"],
    "pixel/plasma.fx": ["brush dim", "ellipse 0.08 0.14 0.42 0.42", "brush hot", "ellipse 0.5 0.1 0.38 0.38", "brush ink", "ellipse 0.28 0.5 0.38 0.38"],
    "pixel/snow.fx": ["pen hot 0.06", "line 0.22 0.28 0.38 0.28", "line 0.3 0.16 0.3 0.4", "line 0.62 0.42 0.78 0.42", "line 0.7 0.3 0.7 0.54", "line 0.42 0.72 0.58 0.72", "line 0.5 0.6 0.5 0.84"],
    "pixel/fire.fx": ["gradv 0.22 0.14 0.22 0.72 hot dim", "round 0.22 0.14 0.22 0.72 0.1", "gradv 0.52 0.32 0.22 0.54 hot dim", "round 0.52 0.32 0.22 0.54 0.1"],
    "pixel/balls.fx": ["brush hot", "ellipse 0.1 0.14 0.4 0.4", "brush ink", "ellipse 0.5 0.46 0.4 0.4"],
    "pixel/bars.fx": ["brush ink", "rect 0.14 0.55 0.14 0.35", "rect 0.32 0.35 0.14 0.55", "brush hot", "rect 0.5 0.2 0.14 0.7", "brush ink", "rect 0.68 0.45 0.14 0.45"],
    "pixel/scanner.fx": ["brush dim", "rect 0.1 0.42 0.8 0.16", "brush hot", "rect 0.42 0.18 0.16 0.64"],
    "volume/spherewipe.fx": ["pen dim 0.06", "ellipse 0.1 0.1 0.8 0.8", "pen none 0", "brush hot", "ellipse 0.3 0.3 0.4 0.4"],
    "volume/orbit.fx": ["pen dim 0.06", "ellipse 0.14 0.14 0.72 0.72", "pen none 0", "brush hot", "ellipse 0.7 0.42 0.18 0.18"],
    "volume/ripple.fx": ["pen ink 0.06", "ellipse 0.1 0.1 0.8 0.8", "pen hot 0.06", "ellipse 0.28 0.28 0.44 0.44"],
    "volume/meteor.fx": ["gradh 0.12 0.4 0.62 0.2 none hot", "round 0.12 0.4 0.62 0.2 0.1", "brush hot", "ellipse 0.68 0.36 0.22 0.28"],
    "volume/noise3d.fx": ["brush hot", "ellipse 0.16 0.16 0.16 0.16", "ellipse 0.52 0.16 0.16 0.16", "ellipse 0.34 0.52 0.16 0.16", "ellipse 0.7 0.7 0.16 0.16", "brush dim", "ellipse 0.34 0.16 0.16 0.16", "ellipse 0.16 0.52 0.16 0.16", "ellipse 0.52 0.52 0.16 0.16", "ellipse 0.16 0.7 0.16 0.16", "ellipse 0.52 0.7 0.16 0.16"],
    "volume/burst.fx": ["pen hot 0.06", "line 0.5 0.5 0.5 0.12", "line 0.5 0.5 0.88 0.5", "line 0.5 0.5 0.5 0.88", "line 0.5 0.5 0.12 0.5", "pen none 0", "brush ink", "ellipse 0.38 0.38 0.24 0.24"],
    "volume/confetti.fx": ["brush hot", "rect 0.18 0.18 0.16 0.16", "brush ink", "rect 0.62 0.32 0.16 0.16", "brush dim", "rect 0.36 0.66 0.16 0.16"],
    "volume/comet.fx": ["gradh 0.08 0.4 0.7 0.2 none hot", "round 0.08 0.4 0.7 0.2 0.1"],
    "volume/helix.fx": ["pen hot 0.1", "arc 0.16 0.08 0.68 0.42 0 180", "arc 0.16 0.42 0.68 0.42 180 180"],
    "volume/fill.fx": ["brush dim", "ellipse 0.1 0.1 0.8 0.8", "brush hot", "ellipse 0.3 0.3 0.4 0.4"],
}
for rel, lines in icons.items():
    block = "".join("icon: " + line + "\n" for line in lines)
    text = files[rel]
    mark = text.find("knobs:")
    if mark < 0:
        mark = text.find("space:")
    end = text.find("\n", mark)
    text = text[:end + 1] + block + text[end + 1:]
    files[rel] = text

for rel, text in files.items():
    path = root / rel
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8")
    old = path.with_suffix(".json")
    if old.exists():
        old.unlink()

print(len(files))
