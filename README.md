# OpenRGB 3D Spatial Presets

Community **controller layouts** and **content** for the [OpenRGB 3D Spatial](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin) plugin.

The plugin is the **engine only** — all looks live here. **Same folder names as the plugin data root** — install is copy-paste.

```text
%AppData%\OpenRGB\plugins\settings\OpenRGB3DSpatialPlugin\
```

## Folders

| This repo | Plugin UI |
|-----------|-----------|
| `controllers/` | Object Creator → Add from preset |
| `effects/volume\|audio\|media\|shader-field\|patterns/` | Effect Library |
| `action-editor/toolbox/` | Action Editor → Tools |
| `action-editor/{gradients,colors,curves}.json` | Action Editor swatches (plugin create-on-save) |
| `triggers/effects/` | Trigger action timelines |
| `triggers/triggers.json` | When → play bindings (plugin create-on-save) |
| `movie-sync/` | Movie Sync catalog + looks + VLC helper |

```text
OpenRGB3DSpatialPresets/          (= AppData …/OpenRGB3DSpatialPlugin/)
  controllers/                    (nested brand/device OK)
  effects/
    volume/
    audio/
    media/
    shader-field/
    patterns/
  action-editor/
    toolbox/
    gradients.json                (optional; plugin writes on save)
    colors.json
    curves.json
  triggers/
    effects/
    triggers.json                 (optional; plugin writes on save)
  movie-sync/
    catalog.json
    effects/
      explosions|pulses|washes|wraps/
    vlc/
  docs/
  scripts/
```

**Action Editor** authors timelines; **Save for** chooses Triggers or Movie Sync.  
Finished packs use `*.action.json` (`openrgb3d.action`). Timeline tools use `*.fx` under `action-editor/toolbox/`.

Engine contracts: [effects-engines.md](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/blob/main/Documentation/effects-engines.md) · [content-library.md](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/blob/main/Documentation/content-library.md).

## Install

Copy the folders above into the plugin data root. Restart OpenRGB.

```text
python scripts/smoke_presets.py
```

Optional: `python scripts/smoke_presets.py --compile-glsl` (needs `moderngl`).

### Controllers only

Copy from [`controllers/`](controllers/), then **Object Creator** → **Add from preset**.

## Browse controller presets

**[docs/PRESET_CATALOG.md](docs/PRESET_CATALOG.md)** · format: **[docs/PRESET_FORMAT.md](docs/PRESET_FORMAT.md)** · template: **[template.controller.json](template.controller.json)**

## Contributing

Controller issues/PRs stay in this repo. Plugin bugs go to [OpenRGB3DSpatialPlugin](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/issues).

After controller changes:

```text
python scripts/generate_preset_catalog.py
```
