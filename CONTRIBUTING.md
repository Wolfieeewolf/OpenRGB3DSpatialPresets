# Contributing

Thank you for helping grow the content pack for [OpenRGB 3D Spatial](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin).

This repo holds **drop-in content**: controller layouts, effect looks (`.fs` / `.kernel`), Action Editor tools (`.fx`), trigger / Movie Sync actions, and helpers. The plugin DLL ships none of that.

## Which repository?

| Topic | Repository |
|-------|------------|
| Controller layouts, looks, Action tools, triggers, Movie Sync content | **OpenRGB3DSpatialPresets** (this repo) |
| Plugin bugs, engines, UI, Screen Mirror, viewport | [OpenRGB3DSpatialPlugin](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/issues) |
| Engine / AppData contracts | Plugin [`Documentation/`](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/tree/main/Documentation) |
| Monitor panel size (mm) | [DisplaySpecifications](https://www.displayspecifications.com) in the plugin (not stored here) |

## Effect / Action / trigger content

- Follow the plugin contracts: [effects-engines.md](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/blob/main/Documentation/effects-engines.md), [content-library.md](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/blob/main/Documentation/content-library.md), [shader-conversion.md](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin/blob/main/Documentation/shader-conversion.md).
- One primary file per look; put it in the matching `effects/<engine>/` or `action-editor/toolbox/` folder.
- Run `python scripts/smoke_presets.py` before opening a PR when you touch effects or actions.

## Controller presets

The sections below are for **`controllers/*.json`** only.

## Best way to contribute (this repo)

| You have… | Use |
|-----------|-----|
| A working `.json` file (exported or edited) | **[Pull request](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/compare)** — the PR template has a checklist |
| Hardware but no layout yet | **[Preset request form](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/issues/new?template=new-controller-preset.yml)** |
| A fix to an existing preset | **[Preset fix form](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/issues/new?template=preset-correction.yml)** or PR |

**Blank issues are disabled.** Maintainers: do not use **Blank issue**; pick a template from [New issue](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/issues/new).

### What makes a useful preset request

- **One device or kit per issue** (not a shopping list of hardware).
- **Exact OpenRGB device name** from your device list (or say you cannot find it).
- **"Not sure"** on LED layout is OK if you describe zones/counts as well as you can.
- If you already exported JSON, open a **[pull request](https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/compare)** instead of an issue.

Vague one-liners may be closed so the form can be resubmitted with detail.

## Before you open a PR

1. **Export from the plugin** (recommended): build the layout in OpenRGB 3D Spatial → **Custom Controllers** → export, or use **Add from preset** on a close match and adjust.
2. Place **one preset per file** under `controllers/`.
3. **Filename**: lowercase, underscores, brand and model first, e.g. `corsair_icue_220t_sp120_front_bottom.json`.
4. Fill the **PR description** using the template (device names, testing, when to use this file).
5. Set `brand`, `model`, and `category` on the JSON (required for the catalog).
6. Regenerate the catalog: `python scripts/generate_preset_catalog.py`.
7. For multi-file kits that need a short “when to use” note, add or update a sidecar under [`docs/device_notes/`](docs/device_notes/) (copy the `<!-- catalog -->` header from an existing note), then regenerate again.

## Required JSON fields

Each file must be a single JSON object matching plugin **version 1** (`OpenRGB3DSpatialCustomController`). Missing required fields will not load.

| Field | Purpose |
|-------|---------|
| `format` | Always `OpenRGB3DSpatialCustomController` |
| `version` | Always `1` |
| `name` | Layout title in the plugin — must follow [OpenRGB naming rules](docs/PRESET_FORMAT.md) (usually same as `controller_name`, or `{device} - {zone}` / kit label like `220T Front Fan - Bottom`) |
| `category` | e.g. `fans`, `graphics_cards`, `motherboard`, `mouses` — see plugin preset picker |
| `brand` | Manufacturer |
| `model` | Product or kit name (can mention case/kit, e.g. `iCUE 220T RGB (front SP120 stack)`) |
| `width`, `height`, `depth` | Grid size for LED positions |
| `spacing_mm_x`, `spacing_mm_y`, `spacing_mm_z` | Millimeters between grid steps |
| `column_widths_mm` | Array length = `width` (often `spacing_mm_x` repeated) |
| `row_heights_mm` | Array length = `height` |
| `layer_depths_mm` | Array length = `depth` |
| `layer_names` | Array length = `depth` (e.g. `"Layer 1"`) |
| `leds_per_cluster` | `1` or `3` |
| `mappings` | Array of LED placements (see below) |

Each mapping entry needs:

| Field | Purpose |
|-------|---------|
| `x`, `y`, `z` | Position on the preset grid |
| `controller_name` | **Exact** name as shown in OpenRGB (critical for matching) |
| `controller_location` | Always `1:1` in this repo (portable); machine `HID:` / `DDP:` paths are for personal backups only |
| `zone_idx`, `led_idx` | Zone and LED index in OpenRGB |
| `granularity` | Typically `2` (same as plugin exports) |

Optional: `light_blockers`; multiple presets for one physical product (e.g. three front fans in one case) — use clear `name` / `model` text and filenames so users know they need more than one file.

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
