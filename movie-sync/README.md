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
| Format | `openrgb3d.action` v4 |
| Catalog | `catalog.json` (ids → packs under `effects/`) |
| Template | `effects/_template.action.json` |
| Authoring | Action Editor → **Save for → Movie Sync** + category |

Cue timelines (when to fire) are **not** here — they are sidecars next to the video. See [movie-sync-authoring.md](docs/movie-sync-authoring.md).
