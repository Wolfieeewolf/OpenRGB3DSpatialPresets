name: Bass Punch
class: BassPunch
category: Audio
description: Kick / bass shockwaves — defaults to Low listen role, pair with Note Sparkle
drive: audio
audio_preset: low_punch
shader: audio-pulse
global: audio speed brightness frequency size scale color bands strip
resolution: 24
user_colors: 1
needs_frequency: true
slider: particle_amount 0 100 12 | Surface sparks: | Sparse spark particles on the shell (0 = smooth ring only).
slider: onset_trigger 5 95 34 unit pct | Beat trigger: | Onset sensitivity — raise to ignore quiet hits, lower for hair trigger.
param: audio_beat_mode
param: size_audio
param: detail
param: audio_falloff
param: audio_pulse_speed
param: audio_radius_basis
param: audio_pulse_half_w
param: audio_max_travel
param: cent particle_amount
param: audio_pulse_decay
param: audio_pulse_hw
param: audio_pulse_hh
param: audio_pulse_hd
param: tight_mul
finish: audio
