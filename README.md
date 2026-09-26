# OpenRGB 3D Spatial Presets

Community **controller layouts**, **effect content**, **patterns**, and **timeline blocks** for the [OpenRGB 3D Spatial](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin) plugin.

The plugin is a **player**: it loads files from its data folder. This repo is the stock content set. An empty plugin data folder loads nothing.

Monitor panel sizes are not stored here. In the plugin, look up width and height in millimeters on [DisplaySpecifications](https://www.displayspecifications.com) and enter them on the display plane.

## Folders

| Folder | Contents |
|--------|----------|
| [`controllers/`](controllers/) | 3D device layouts (JSON) |
| [`effects/spatial/`](effects/spatial/) | Room volume effects (`volumeMain` + FolderVolume header) |
| [`effects/audio/`](effects/audio/) | Audio volume effects (same host; shared FFT engine in the plugin) |
| [`effects/media/`](effects/media/) | Texture / shape media effects |
| [`effects/shader-field/`](effects/shader-field/) | 2D Shader Field presets (`spatialMain`) |
| [`patterns/`](patterns/) | Strip / colormap kernels (`*.kernel`) |
| [`timelines/`](timelines/) | Shows; reusable blocks under [`timelines/blocks/`](timelines/blocks/) |

Authoring rules for effect engines live in the plugin: [effects-engines.md](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/blob/feat/folder-loaded-presets/Documentation/effects-engines.md) (branch may move to `main` after merge).

## How to install

Plugin data root (OpenRGB config directory):

```text
plugins/settings/OpenRGB3DSpatialPlugin/
```

Copy from this repo into matching folders under that root:

| From this repo | Into plugin data |
|----------------|------------------|
| `controllers/` | `…/controllers/` |
| `effects/` | `…/effects/` (keep `spatial`, `audio`, `media`, `shader-field`) |
| `patterns/` | `…/patterns/` |
| `timelines/` | `…/timelines/` (include `blocks/`) |

### Controllers only

1. Copy a device folder or a single `.json` from [`controllers/`](controllers/). Subfolders are fine.
2. In the plugin: **Object Creator** → **Add from preset**.

### Effects / patterns / timelines

Copy the trees above, then restart OpenRGB (or reload the plugin) so FolderVolume / Shader Field / kernels rescans disk.

You can also clone this repo and copy from your checkout.

## Browse controller presets

See **[docs/PRESET_CATALOG.md](docs/PRESET_CATALOG.md)** for layouts grouped by category. Kit-specific notes live under the matching section there.

## Contributing

**Controller JSON (this repo):** [preset request](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/issues/new?template=new-controller-preset.yml) · [preset fix](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/issues/new?template=preset-correction.yml)

**Plugin bugs or features** go to [OpenRGB3DSpatialPlugin](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/issues), not here.

**Have controller JSON ready?** [Pull request](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/compare). Copy **[template.controller.json](template.controller.json)** and follow **[docs/PRESET_FORMAT.md](docs/PRESET_FORMAT.md)** (version 1 required fields + OpenRGB naming rules). Machine exports with `HID:` / `DDP:` locations are personal backups — convert to portable `"1:1"` before PR.

**Effect / kernel / timeline content:** follow the plugin engine contracts (Volume / Audio / Media / Shader Field / Kernel). Spatial and audio looks need a FolderVolume header (`name`, `class`, `global`, `param`, `finish`, then `# Effect` / `volumeMain`). Shader Field files use `spatialMain`. Prefer shared `global:` knobs over one-off UI.

After adding or changing **controller** presets, regenerate the catalog:

```text
python scripts/generate_preset_catalog.py
```

For multi-file kits that need a short “when to use” note, add a sidecar under [`docs/device_notes/`](docs/device_notes/) (see an existing note for the `<!-- catalog -->` header), then regenerate.

## License

Preset JSON and content files are contributed by the community. Unless a file states otherwise, contributions are licensed under the same terms as this repository (add a `LICENSE` file when you choose one—CC0 or MIT are common for data-only repos).
