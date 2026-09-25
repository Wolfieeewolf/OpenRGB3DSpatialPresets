# OpenRGB 3D Spatial Presets

Community **3D controller layouts** for the [OpenRGB 3D Spatial](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin) plugin.

Monitor panel sizes are not stored here. In the plugin, look up width and height in millimeters on [DisplaySpecifications](https://www.displayspecifications.com) and enter them on the display plane.

## Folders

| Folder | Contents |
|--------|----------|
| [`controllers/`](controllers/) | Custom controller layout JSON (LED positions in 3D space) |

## How to install

1. Open your OpenRGB **configuration directory** (Settings → “Open Config Directory” or similar).
2. Copy preset files into:

   ```
   plugins/settings/OpenRGB3DSpatialPlugin/controllers/
   ```

   Copy a device folder, or a single `.json`, from [`controllers/`](controllers/) into that folder. Subfolders are fine. The plugin loads every controller JSON under `controllers/`, including `keyboards/razer` and `fans/corsair`.

3. In the plugin: **Object Creator** → **Add from preset** for controllers.

You can also clone this repo and copy files from your checkout.

## Browse presets

See **[docs/PRESET_CATALOG.md](docs/PRESET_CATALOG.md)** for the full list grouped by category (brand, model, layout, file). Kit-specific notes (for example Corsair SP120 / iCUE 220T) live under the matching category section there.

## Contributing

**Issues for this repo only** (controller JSON): [preset request](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/issues/new?template=new-controller-preset.yml) · [preset fix](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/issues/new?template=preset-correction.yml)

**Plugin bugs or features** go to [OpenRGB3DSpatialPlugin](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/issues), not here.

**Have JSON ready?** [Pull request](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/compare). Copy **[template.controller.json](template.controller.json)** and follow **[docs/PRESET_FORMAT.md](docs/PRESET_FORMAT.md)** (version 1 required fields + OpenRGB naming rules). Machine exports with `HID:` / `DDP:` locations are personal backups — convert to portable `"1:1"` before PR.

After adding or changing presets, regenerate the catalog:

```
python scripts/generate_preset_catalog.py
```

For multi-file kits that need a short “when to use” note, add a sidecar under [`docs/device_notes/`](docs/device_notes/) (see an existing note for the `<!-- catalog -->` header), then regenerate.

## License

Preset JSON files are contributed by the community. Unless a file states otherwise, contributions are licensed under the same terms as this repository (add a `LICENSE` file when you choose one—CC0 or MIT are common for data-only repos).
