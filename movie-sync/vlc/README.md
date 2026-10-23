# VLC Movie Sync helper

Sidecar-only workflow (no cue library inside OpenRGB).

## Everyday use

1. Add **Movie Sync** to the effect stack in OpenRGB and start it (Script API on).
2. Keep cues next to the video:
   - `Spooderman2.mp4`
   - `Spooderman2.movie_sync.json`
3. Play the video in VLC (Lua interface installed once â€” below).

OpenRGB discovers the sidecar through the VLC helper and fires catalog `effect_id`s.

## Install Lua once (Windows)

| File | Copy to |
|------|---------|
| `openrgb_movie_sync_intf.lua` | `%APPDATA%\vlc\lua\intf\openrgb_movie_sync.lua` (**must rename** â€” VLC loads the stem from `lua-intf`) |

In `%APPDATA%\vlc\vlcrc`:

```text
extraintf=luaintf
lua-intf=openrgb_movie_sync
```

Fully quit and restart VLC.

## Cue file

See `../` for catalog packs (`explosions/`, `pulses/`, `washes/`, `wraps/`).
Cue scripts reference those `effect_id`s by timestamp â€” they stay beside the movie, never inside OpenRGB.
