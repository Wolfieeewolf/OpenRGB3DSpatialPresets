# VLC Movie Sync helpers

Sidecar-only workflow (no cue library inside OpenRGB).

| Script | Role |
|--------|------|
| `openrgb_movie_sync_intf.lua` | **Playback** — reads `<video>.movie_sync.json` and fires cues |
| `openrgb_movie_sync_author.lua` | **Authoring** — mark start/end while watching, pick a look, save the sidecar |
| `openrgb_movie_lights.lua` | **Manual playback** — VLC extension; opens a file-picker to load a cue script manually (supports DVD chapter-gating; no auto-detect) |

OpenRGB must be running with **Movie Sync** on the effect stack and **Script API** enabled (default port `27180`).

## Everyday playback

1. Add **Movie Sync** to the effect stack in OpenRGB and start it (Script API on).
2. Keep cues next to the video:
   - `Spooderman2.mp4`
   - `Spooderman2.movie_sync.json`
3. Play the video in VLC with the playback Lua interface installed (below).

## Author cues while watching (rough MVP)

1. Same as playback step 1 (OpenRGB + Movie Sync + Script API).
2. Install the **author extension** (below).
3. Open the video in VLC.
4. **View → OpenRGB Movie Sync Author** (or the Extensions / Lua menu entry).
5. **Reload catalog** — fills the look list from `GET /v1/catalog`.
6. Scrub or play:
   - **Mark start** at the playhead
   - **Mark end** for a level hold, or **Clear end** for a pulse
   - Pick a look → optional **Preview look**
   - **Save cue** — appends/sorts into `<video>.movie_sync.json` (creates the file if needed)
7. Repeat for the next cue. Playback helper picks up the sidecar on the next media arm / restart.

Menu shortcuts (same extension): Mark start / Mark end / Clear end / Save cue / Reload catalog.

## Install once (Windows)

### Playback interface

| File | Copy to |
|------|---------|
| `openrgb_movie_sync_intf.lua` | `%APPDATA%\vlc\lua\intf\openrgb_movie_sync.lua` (**must rename** — VLC loads the stem from `lua-intf`) |

In `%APPDATA%\vlc\vlcrc`:

```text
extraintf=luaintf
lua-intf=openrgb_movie_sync
```

Fully quit and restart VLC.

### Author extension

| File | Copy to |
|------|---------|
| `openrgb_movie_sync_author.lua` | `%APPDATA%\vlc\lua\extensions\openrgb_movie_sync_author.lua` (keep this name) |

Restart VLC. No `vlcrc` change required for the extension.

### Movie Lights extension (manual / DVD)

| File | Copy to |
|------|---------|
| `openrgb_movie_lights.lua` | `%APPDATA%\vlc\lua\extensions\openrgb_movie_lights.lua` (keep this name) |

Restart VLC. Open with **View → OpenRGB Movie Lights**. Use **Load script…** to pick a `.movie_sync.json` cue file manually. Supports DVD chapter-gating via `feature_title` / `start_chapter` fields.

## Cue file

Looks live under `movie-sync/effects/` (`explosions/`, `pulses/`, `washes/`, `wraps/`) and `catalog.json`.  
Cue scripts reference those `effect_id`s by timestamp — they stay beside the movie, never inside OpenRGB.

See `_example_sidecar.movie_sync.json` and [movie-sync-authoring.md](../docs/movie-sync-authoring.md).
