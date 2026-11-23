# OpenRGB 3D Spatial Presets

Effects, tools and device layouts for the [OpenRGB 3D Spatial](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin) plugin.

The plugin contains no effects of its own. This pack provides them, along with ready-made controller layouts, Action Editor tools, triggers and Movie Sync content. Without it, the plugin's Effect Library is empty.

## Installation

1. Install the plugin first. See the plugin's [Getting Started](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/wiki/Getting-Started) guide.
2. Download this repository (**Code → Download ZIP**, or `git clone`).
3. Copy the folders into the plugin data folder, creating it if needed:

   | OS | Folder |
   | --- | --- |
   | Windows | `%AppData%\OpenRGB\plugins\settings\OpenRGB3DSpatialPlugin\` |
   | Linux / macOS | `~/.config/OpenRGB/plugins/settings/OpenRGB3DSpatialPlugin/` |

   | Folder | Contents | Required |
   | --- | --- | --- |
   | `effects/` | Effects shown in the **Effect Library** | Yes |
   | `controllers/` | Device layouts (**Object Creator → Add from preset**) | Recommended |
   | `action-editor/` | Tools for building light shows | For Actions |
   | `triggers/` | Lighting played on events | For Triggers |
   | `movie-sync/` | Lighting synchronised with video in VLC | For Movie Sync |

   If you have customised `triggers.json` or the Action Editor swatches, merge the files rather than overwriting them.
4. Restart OpenRGB.

To update, download the latest version and copy it over the existing folders. Keep the pack in step with the plugin, as older copies may not match the current plugin version.

## Documentation

* [Wiki](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/wiki): installation, folder layout, controller presets and authoring.
* Engine contracts are in the plugin repository: [effects-engines.md](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/blob/main/Documentation/effects-engines.md) and [content-library.md](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/blob/main/Documentation/content-library.md).
* Controller presets: [catalog](docs/PRESET_CATALOG.md), [format](docs/PRESET_FORMAT.md) and [template](template.controller.json).

## Repository layout

| Folder | Used by |
| --- | --- |
| `controllers/` | Object Creator → Add from preset |
| `effects/volume`, `audio`, `media`, `shader-field`, `patterns` | Effect Library |
| `action-editor/toolbox/` | Action Editor → Tools |
| `action-editor/gradients.json`, `colors.json`, `curves.json` | Action Editor swatches (created by the plugin on save if missing) |
| `triggers/effects/` | Trigger action timelines |
| `triggers/triggers.json` | Trigger bindings (created by the plugin on save if missing) |
| `movie-sync/` | Movie Sync catalog, effects and VLC helper |
| `docs/` | Controller catalog and format |
| `scripts/` | Catalog generator and smoke test |

Finished Action packs use `*.action.json` (`openrgb3d.action`); Action Editor tools use `*.fx` under `action-editor/toolbox/`.

## Status

The pack is in alpha, alongside the plugin. Effects and layouts vary in polish.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). Content and controller presets are handled here; plugin bugs belong in the [plugin issue tracker](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/issues).

Before submitting content changes, run the smoke test:

```text
python scripts/smoke_presets.py
```

Add `--compile-glsl` to compile every shader as well (requires `moderngl`). After changing controller presets, regenerate the catalog:

```text
python scripts/generate_preset_catalog.py
```

GitHub is the main repository; [GitLab](https://gitlab.com/Wolfieeewolf/OpenRGB3DSpatialPresets) is a mirror.

## License

GPL-2.0-or-later, the same as the plugin and OpenRGB. See [LICENSE](LICENSE). Credits for the ideas and code this pack builds on are in [CREDITS.md](CREDITS.md).
