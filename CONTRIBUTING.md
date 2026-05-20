# Contributing controller presets

Thank you for helping grow the preset library. This repo only holds **controller layout JSON** for [OpenRGB 3D Spatial](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin).

## Best way to contribute

| You have… | Use |
|-----------|-----|
| A working `.json` file (exported or edited) | **[Pull request](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/compare)** — the PR template has a checklist |
| Hardware but no layout yet | **[New controller preset](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/issues/new?template=new-controller-preset.yml)** issue |
| A fix to an existing preset | **[Preset correction](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/issues/new?template=preset-correction.yml)** issue or PR |

Monitor sizes are **not** accepted here; use [DisplaySpecifications](https://www.displayspecifications.com) in the plugin.

## Before you open a PR

1. **Export from the plugin** (recommended): build the layout in OpenRGB 3D Spatial → **Custom Controllers** → export, or use **Add from preset** on a close match and adjust.
2. Place **one preset per file** under `controllers/`.
3. **Filename**: lowercase, underscores, brand and model first, e.g. `corsair_icue_220t_sp120_front_bottom.json`.
4. Fill the **PR description** using the template (device names, testing, when to use this file).
5. Update the **README** preset table if the preset is a named kit (case, GPU model, etc.) or needs a short “when to use” note.

## Required JSON fields

Each file must be a single JSON object with at least:

| Field | Purpose |
|-------|---------|
| `name` | Short label in **Add from preset** (e.g. `220T Front Fan - Bottom`) |
| `category` | e.g. `fans`, `graphics_cards`, `motherboard`, `mouses` — see plugin preset picker |
| `brand` | Manufacturer |
| `model` | Product or kit name (can mention case/kit, e.g. `iCUE 220T RGB (front SP120 stack)`) |
| `width`, `height`, `depth` | Grid size for LED positions |
| `spacing_mm_x`, `spacing_mm_y`, `spacing_mm_z` | Millimeters between grid steps |
| `mappings` | Array of LED placements (see below) |

Each mapping entry needs:

| Field | Purpose |
|-------|---------|
| `x`, `y`, `z` | Position on the preset grid |
| `controller_name` | **Exact** name as shown in OpenRGB (critical for matching) |
| `controller_location` | Usually `1:1` — copy from an export on your system if matching fails |
| `zone_idx`, `led_idx` | Zone and LED index in OpenRGB |
| `granularity` | Typically `2` (same as plugin exports) |

Optional: multiple presets for one physical product (e.g. three front fans in one case) — use clear `name` / `model` text and filenames so users know they need more than one file.

See [docs/PRESET_FORMAT.md](docs/PRESET_FORMAT.md) for examples and naming tips.

## Review criteria

Maintainers will check:

- Valid JSON, one preset per file
- `controller_name` matches a real OpenRGB device string (or documented variants)
- LED indices and zones are consistent with the description
- Filename and labels describe **what hardware** the preset is for
- No duplicate preset for the same device/role unless justified (e.g. simple vs detailed layout)

## License

By contributing, you agree your preset JSON is licensed under the same terms as this repository (see [README](README.md#license)).
