# Movie Sync authoring

## Sidecar cues

Place `Movie.movie_sync.json` next to the video. Each cue’s `effect_id` must exist in the Movie Sync catalog (`movie-sync/catalog.json`).

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

Looks use the same file format as trigger actions (`openrgb3d.action` v4):

- `movie-sync/effects/_template.action.json`
- `triggers/effects/_template.action.json`

Tools (`.fx`): `action-editor/toolbox/`.
