# Movie Sync

Film player stock content. Lives next to the feature, not under Effect Library `effects/`.

```text
movie-sync/
  catalog.json
  effects/
    explosions|pulses|washes|wraps/
    _template.action.json
  vlc/
  docs/
```

| Item | Role |
|------|------|
| Format | `openrgb3d.action` v5 (v4 still loads) |
| Catalog | `catalog.json` (ids → packs under `effects/`) |
| Template | `effects/_template.action.json` |
| Looks | Action Editor → **Save for → Movie Sync** + category |
| Cue timelines | Sidecar next to the video (`Movie.movie_sync.json`) |
| VLC | Playback intf + **Author** extension - see [vlc/README.md](vlc/README.md) |

Cue timelines (when to fire) stay beside the movie, never inside OpenRGB. See [movie-sync-authoring.md](docs/movie-sync-authoring.md).
