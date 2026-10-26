# Movie Cue spatial notes

Stock looks are layout-agnostic (`target: all`). Volume shaders expand from the layer **Spatial Anchor**; room/axis sweeps use mapped lights.

| Class | Role |
|-------|------|
| volume | Radial / shell from display origin (`.fs` in `shaders/` or `packs/blocks/volume/`) |
| plane | Room XYZ sweep (`axis_space: room`) |
| sparse | Per-LED / twinkle / debris |
| strata | Height bands (horizon / floor / ceiling) |
| utility | Flat wash / blackout / strobe OK |

Authoring: [movie-sync-authoring.md](movie-sync-authoring.md).
