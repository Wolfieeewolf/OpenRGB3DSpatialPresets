# Controller preset JSON format

Portable presets are loaded from `plugins/settings/OpenRGB3DSpatialPlugin/controllers/` (copy files from this repo’s [`controllers/`](../controllers/) folder).

There are **two kinds** of JSON files. Only **portable presets** belong in this repository.

The plugin loader (`OpenRGB3DSpatialCustomController` **version 1**) rejects files that omit required fields — there is no dual-format / migration path.

---

## Portable preset (shareable)

| Field | Required | Purpose |
| ----- | -------- | ------- |
| `format` | yes | Always `OpenRGB3DSpatialCustomController` |
| `version` | yes | Always `1` |
| `name` | yes | Layout title in the plugin **Custom controllers** list (see naming rules) |
| `brand` | recommended | Hardware vendor — helps match devices across PCs |
| `model` | recommended | Product model |
| `category` | optional | Taxonomy only (`graphics_cards`, `cpu_cooler`, `fans`, …) — **not** the layout title |
| `width`, `height`, `depth` | yes | Grid size in cells |
| `spacing_mm_x/y/z` | yes | Default LED spacing in millimetres |
| `column_widths_mm` | yes | Per-column sizes (mm); length must match `width`. Uniform grids use `spacing_mm_x` repeated |
| `row_heights_mm` | yes | Per-row sizes (mm); length must match `height` |
| `layer_depths_mm` | yes | Per-layer sizes (mm); length must match `depth` |
| `layer_names` | yes | Display names for each depth layer; length must match `depth` (e.g. `"Layer 1"`) |
| `leds_per_cluster` | yes | `1` or `3` for clustered LEDs |
| `mappings` | yes | LED assignments |
| `light_blockers` | optional | Grid cells that occlude light (no LED mapping). Array of `{ "x", "y", "z" }` |

**Every mapping:**

| Field | Value |
| ----- | ----- |
| `controller_name` | Exact OpenRGB device name (`RGBController::GetName()`) |
| `controller_location` | Always `"1:1"` for presets |
| `x`, `y`, `z` | Grid cell |
| `zone_idx`, `led_idx` | OpenRGB indices |
| `granularity` | Usually `2` (per-LED) |

**Filename:** lowercase `snake_case`, e.g. `intel_arc_a770.json`, `corsair_icue_220t_sp120_front_bottom.json`.

---

## Machine export (do not commit)

Created when saving from the plugin on your PC. Uses real `HID:` / `DDP:` locations and often omits `brand`/`model`. Personal backup only — convert to portable (`"1:1"` + brand/model/category) before opening a PR.

---

## OpenRGB naming rules

| OpenRGB field | Preset JSON |
| ------------- | ----------- |
| Device **name** | `controller_name` in every mapping (verbatim) |
| **Vendor** | optional `brand` |
| **Description** | reference only — do not use as layout `name` |
| **Location** | never in presets — use `"1:1"` |

### Layout `name` (top-level)

| Situation | `name` |
| --------- | ------ |
| Full device layout | Same as `controller_name` |
| Partial zone on one device | `{controller_name} - {short label}` |
| Several layouts, same device (e.g. 220T front stack) | Short layout label OK (`220T Front Fan - Bottom`) — keep real `controller_name` in mappings |

### Do **not** use as layout `name`

Device types (`Cooler`, `Fan`, `Mouse`, `GPU`, …), category slugs (`graphics_cards`, `cpu_cooler`), or placeholders (`Graphics Card`, `Onboard LED` alone).

---

## Minimal example

See [`template.controller.json`](../template.controller.json) and [`controllers/corsair_sp120_rgb_pro.json`](../controllers/corsair_sp120_rgb_pro.json).

---

## Checklist before opening a PR

- [ ] `controller_name` matches OpenRGB **Devices** name on a real system
- [ ] All mappings use `"controller_location": "1:1"`
- [ ] Top-level `name` follows naming rules above
- [ ] `brand` and `model` set for matching
- [ ] `category` set (taxonomy slug; see catalog groupings)
- [ ] `column_widths_mm` / `row_heights_mm` / `layer_depths_mm` / `layer_names` / `leds_per_cluster` present
- [ ] `zone_idx` / `led_idx` verified
- [ ] `light_blockers` included when the device layout needs occlusion gaps
- [ ] Valid JSON, `snake_case` filename
- [ ] Exported portable (`controller_location` is `"1:1"`, not a machine `HID:` path)
- [ ] Catalog regenerated: `python scripts/generate_preset_catalog.py`
- [ ] Kit notes under `docs/device_notes/` updated when needed

Plugin contract: [OpenRGB3DSpatialPlugin CONTRIBUTING.md](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/blob/main/CONTRIBUTING.md) (Custom controllers — version 1, required grid arrays).
