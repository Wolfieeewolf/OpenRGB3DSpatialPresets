# Media

Image / GIF / texture projection effects. Each file uses `finish: rgb` and reads `u_media` (the host-loaded texture). Not for audio or movie-sync cues.

Prefer `resolution: 40`–`48` so projected edges stay crisp on the room grid. Soft falloffs may use `vfxCoreRim` / `vfxShell` when touching envelope math.

| File | Description |
|------|-------------|
| `omni-shape-texture.fs` | Image / GIF mapped onto a crisp 3D shape (sphere, cube, octahedron, ...) |
| `texture-projection.fs` | Planar or spherical image / GIF projection across the 3D layout |
