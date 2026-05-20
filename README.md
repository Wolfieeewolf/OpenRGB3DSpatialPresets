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

| File | Device / use |
|------|----------------|
| corsair_sp120_rgb_pro.json | Corsair SP120 RGB PRO — **one fan**, single LED in 3D (buy separately or any single-fan setup) |
| corsair_icue_220t_sp120_front_bottom.json | **Corsair iCUE 220T RGB** — front intake, bottom fan (LEDs 0–7, ring layout) |
| corsair_icue_220t_sp120_front_middle.json | **Corsair iCUE 220T RGB** — front intake, middle fan (LEDs 8–15) |
| corsair_icue_220t_sp120_front_top.json | **Corsair iCUE 220T RGB** — front intake, top fan (LEDs 16–23) |
| razer_deathadder_elite.json | Razer DeathAdder Elite |
| amd_wraith_prism.json | AMD Wraith Prism |
| msi_mpg_x570_gaming_edge_wifi_onboard_led.json | MSI MPG X570 GAMING EDGE WIFI onboard LEDs |
| intel_arc_a770.json | Intel Arc A770 Limited Edition |

#### Corsair SP120 / iCUE 220T RGB

The [iCUE 220T RGB](https://www.corsair.com/us/en/p/case/icue-220t-rgb-tempered-glass-mid-tower-atx-smart-case-cc-9011133-ww/cc-9011133-ww) ships with **three** SP120 RGB PRO fans on the front. OpenRGB often exposes them as **one** device with **24 LEDs** on zone 0 (8 per fan). Use all three `corsair_icue_220t_sp120_front_*.json` presets and place them at bottom / middle / top in your 3D scene.

If you only have **one** SP120 (not the 220T stack), use `corsair_sp120_rgb_pro.json` instead.

Mappings still use `controller_name` **"Corsair SP120 RGB PRO"** so they match OpenRGB; edit that string in the JSON if your device name differs.

## Contributing

1. Export a layout from the plugin (**Custom Controllers** → export), or copy an existing JSON and edit it.
2. Add a new `.json` under `controllers/`.
3. Open a pull request with brand/model in the filename (e.g. `corsair_sp120_rgb_pro.json`).

Controller presets should include `name`, `category`, `brand`, `model`, `mappings`, and spacing fields where relevant. See the plugin repo for format details.

## License

Preset JSON files are contributed by the community. Unless a file states otherwise, contributions are licensed under the same terms as this repository (add a `LICENSE` file when you choose one—CC0 or MIT are common for data-only repos).
