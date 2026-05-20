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

### Included layouts

| File | Device |
|------|--------|
| corsair_sp120_rgb_pro.json | Corsair SP120 RGB PRO (single fan) |
| corsair_sp120_front_case_light_bottom.json | SP120 — front case ring (bottom) |
| corsair_sp120_front_case_lights_middle.json | SP120 — front case ring (middle) |
| corsair_sp120_front_case_lights_top.json | SP120 — front case ring (top) |
| razer_deathadder_elite.json | Razer DeathAdder Elite |
| amd_wraith_prism.json | AMD Wraith Prism |
| msi_mpg_x570_gaming_edge_wifi_onboard_led.json | MSI MPG X570 GAMING EDGE WIFI onboard LEDs |
| intel_arc_a770.json | Intel Arc A770 Limited Edition |

## Contributing

1. Export a layout from the plugin (**Custom Controllers** → export), or copy an existing JSON and edit it.
2. Add a new `.json` under `controllers/`.
3. Open a pull request with brand/model in the filename (e.g. `corsair_sp120_rgb_pro.json`).

Controller presets should include `name`, `category`, `brand`, `model`, `mappings`, and spacing fields where relevant. See the plugin repo for format details.

## License

Preset JSON files are contributed by the community. Unless a file states otherwise, contributions are licensed under the same terms as this repository (add a `LICENSE` file when you choose one—CC0 or MIT are common for data-only repos).
