# Controller preset JSON format

Presets are consumed by **Object Creator → Add from preset** in OpenRGB 3D Spatial. Matching uses `controller_name` (and related fields) from `mappings`, so those strings must match what OpenRGB reports on a given PC.

## Minimal example (single LED)

```json
{
  "name": "SP120 RGB PRO (single fan)",
  "category": "fans",
  "brand": "Corsair",
  "model": "SP120 RGB PRO",
  "width": 2,
  "height": 2,
  "depth": 1,
  "spacing_mm_x": 10,
  "spacing_mm_y": 10,
  "spacing_mm_z": 10,
  "mappings": [
    {
      "x": 0,
      "y": 0,
      "z": 0,
      "controller_name": "Corsair SP120 RGB PRO",
      "controller_location": "1:1",
      "zone_idx": 0,
      "led_idx": 0,
      "granularity": 2
    }
  ]
}
```

## Multi-LED / ring example

See `controllers/corsair_icue_220t_sp120_front_bottom.json` — eight LEDs in a ring, `led_idx` 0–7 on `zone_idx` 0.

## Naming conventions

| Item | Convention |
|------|------------|
| **File** | `brand_model_role.json` — lowercase, underscores |
| **name** | Human-readable preset title in the picker |
| **model** | Product; include case/kit if the preset only applies there |
| **controller_name** | Copy **exactly** from OpenRGB’s device list |

## Multi-file kits

When one product needs several presets (e.g. three case fans on one OpenRGB device):

- Use separate files and names (`…_bottom`, `…_middle`, `…_top`).
- Document in the PR and README which files are used together.
- Split `led_idx` ranges per file; do not overlap unless intentional.

## Getting `controller_name` and indices

1. Open OpenRGB and note the device name string.
2. In the plugin, map LEDs in **Custom Controllers** or export an existing layout.
3. Confirm `zone_idx` / `led_idx` with OpenRGB’s zone/LED UI or a test export.

If the same hardware appears under different names (e.g. trailing `1`, `2`), mention all tested names in your PR.
