# OpenRGB 3D Spatial Presets

Community **content pack** for the [OpenRGB 3D Spatial](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin) plugin: controller layouts, effect looks, Action tools, triggers, and Movie Sync helpers.

The plugin repo is the **engine only** — all drop-in looks live here. **Same folder names as the plugin data root** — install is copy-paste.

Browsable guide: **[Wiki](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/wiki)** (once published).

```text
%AppData%\OpenRGB\plugins\settings\OpenRGB3DSpatialPlugin\     (Windows)
~/.config/OpenRGB/plugins/settings/OpenRGB3DSpatialPlugin/     (Linux)
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
OpenRGB3DSpatialPresets/          (= AppData .../OpenRGB3DSpatialPlugin/)
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

Engine contracts (plugin repo): [effects-engines.md](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/blob/main/Documentation/effects-engines.md) · [content-library.md](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/blob/main/Documentation/content-library.md).

## Install

1. Install a matching [OpenRGB3DSpatialPlugin](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin) build.
2. Copy the folders above into the plugin data root (merge OK — keep your local `triggers.json` / swatches if you already customized them).
3. Restart OpenRGB.

```text
python scripts/smoke_presets.py
```

Optional: `python scripts/smoke_presets.py --compile-glsl` (needs `moderngl`).

### Controllers only

Copy from [`controllers/`](controllers/), then **Object Creator** → **Add from preset**.

## Browse controller presets

**[docs/PRESET_CATALOG.md](docs/PRESET_CATALOG.md)** · format: **[docs/PRESET_FORMAT.md](docs/PRESET_FORMAT.md)** · template: **[template.controller.json](template.controller.json)**

## What to expect

This pack is **community / alpha** content. Looks and layouts vary in polish. Prefer the wiki + folder READMEs for how each area is meant to be authored. Plugin bugs belong on the [plugin issues](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/issues) list — keep this repo for content and controller presets.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). Controller issues/PRs stay here. Effect / Action / trigger content PRs are welcome when they follow the engine contracts linked above.

After controller changes:

```text
python scripts/generate_preset_catalog.py
```

## License

Content in this repo follows the same spirit as the plugin unless a file says otherwise — check individual files and [CONTRIBUTING.md](CONTRIBUTING.md) before redistributing.
