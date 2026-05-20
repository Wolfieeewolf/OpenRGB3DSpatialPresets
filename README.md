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
   plugins/settings/OpenRGB3DSpatialPlugin/controller_presets/
   ```

   Copy any `.json` from [`controllers/`](controllers/) into that folder (one file = one preset).

3. In the plugin: **Object Creator** → **Add from preset** for controllers.

You can also clone this repo and copy files from your checkout.

## Contributing

1. Export a layout from the plugin (**Custom Controllers** → export), or copy an existing JSON and edit it.
2. Add a new `.json` under `controllers/`.
3. Open a pull request with brand/model in the filename (e.g. `corsair_sp120_rgb_pro.json`).

Controller presets should include `name`, `category`, `brand`, `model`, `mappings`, and spacing fields where relevant. See the plugin repo for format details.

## License

Preset JSON files are contributed by the community. Unless a file states otherwise, contributions are licensed under the same terms as this repository (add a `LICENSE` file when you choose one—CC0 or MIT are common for data-only repos).
