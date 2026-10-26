# Audio

FFT-driven volumetric field effects. Each file uses `drive: audio` and `finish: audio`, binding to a Listen Role (band). All use `volumeMain(out vec4, in vec3 p01)`.

| File | Description |
|------|-------------|
| `audio-level.fs` | Rising fill surface driven by Hz band |
| `audio-paint-brush.fs` | Lissajous ribbons driven by bass / mid / treble |
| `audio-pulse.fs` | Beat-triggered shockwaves from the Spatial Anchor |
| `audio-strip-visualizer.fs` | Spectrum bars or scrolling spectrogram (strip-first) |
| `bass-punch.fs` | Kick / bass shockwaves — alias of `audio-pulse` with Low role |
| `note-sparkle.fs` | ColorChord note particle cloud (hull + sparks) |
| `spectrum-bars.fs` | Spectrum bar graph across the zone |
