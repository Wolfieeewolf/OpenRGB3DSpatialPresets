# OpenRGB 3D Spatial Presets

The **effects, tools and device layouts** for the [OpenRGB 3D Spatial](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin) plugin.

The plugin is only the engine and ships with **no effects**. Without this pack the Effect Library is empty and pressing Start does nothing.

## Install (2 minutes)

1. Install the plugin first — see the [plugin Getting Started guide](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/wiki/Getting-Started).
2. Download this repo: green **Code → Download ZIP** (or `git clone`).
3. Copy these folders from the zip into the plugin folder (create it if it doesn't exist):

   | OS | Plugin folder |
   | --- | --- |
   | Windows | `%AppData%\OpenRGB\plugins\settings\OpenRGB3DSpatialPlugin\` |
   | Linux / macOS | `~/.config/OpenRGB/plugins/settings/OpenRGB3DSpatialPlugin/` |

   | Folder | You get | Need it? |
   | --- | --- | --- |
   | `effects/` | All the looks in the **Effect Library** | **Yes** |
   | `controllers/` | Ready-made layouts for common devices (**Object Creator → Add from preset**) | Recommended |
   | `action-editor/` | Tools for building your own light shows | If you use Actions |
   | `triggers/` | Lighting that fires on events | If you use Triggers |
   | `movie-sync/` | Lighting synced to video (VLC) | If you use Movie Sync |

   Already customised `triggers.json` or swatches? Merge instead of overwriting.
4. Restart OpenRGB.

To update later, download again and copy over the top.

Browsable guide: **[Wiki](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/wiki)**. Source of truth is GitHub; GitLab ([Wolfieeewolf/OpenRGB3DSpatialPresets](https://gitlab.com/Wolfieeewolf/OpenRGB3DSpatialPresets)) is a mirror.

---

*The rest of this page is for people browsing or contributing content.*

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

## Checking content changes

```text
python scripts/smoke_presets.py
```

Optional: `python scripts/smoke_presets.py --compile-glsl` (needs `moderngl`).

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
