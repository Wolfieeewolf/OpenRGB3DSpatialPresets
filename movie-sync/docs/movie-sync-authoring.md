# Movie Sync authoring

## Sidecar cues

Place `Movie.movie_sync.json` next to the video. Each cue's `effect_id` must exist in the Movie Sync catalog (`movie-sync/catalog.json`).

**Author while watching (VLC):** install `movie-sync/vlc/openrgb_movie_sync_author.lua` as a VLC extension, keep OpenRGB Movie Sync Script API on, mark start/end on the playhead, pick a catalog look, and save. See [vlc/README.md](../vlc/README.md). Hand-editing JSON is still fine.

## Looks

1. Author in **Action Editor** → **Save for → Movie Sync** → pick a category
2. Or drop `*.action.json` under `movie-sync/effects/<category>/`
3. Reload the Movie Sync catalog in the effect UI

```text
movie-sync/
  catalog.json
  effects/
    explosions/
    pulses/
    washes/
    wraps/
  vlc/
```

Looks use the same file format as trigger actions (`openrgb3d.action` v5; v4 still loads and rewrites):

- `movie-sync/effects/_template.action.json`
- `triggers/effects/_template.action.json`

Tools (`.fx`): `action-editor/toolbox/`. For room depth prefer **volume** tools (`field_*`, `spark_burst`, `particle_*`, `neon_ribbon` / `ribbon_helix`) over basic `solid` / `wipe` / `pulse` - those read flat on a real LED map.

Showcase packs: `triggers/effects/roomscape/` plus `movie-sync/effects/washes/roomscape_helix_rain.action.json`.
