## Controller preset submission

<!-- Thank you for contributing! Fill in every section below. -->

### Preset file(s)

<!-- List new or changed files under controllers/ -->

- [ ] `controllers/` …

### Hardware

| | |
|---|---|
| **Brand / model** | <!-- e.g. Corsair iCUE 220T RGB --> |
| **Category** | <!-- fans, graphics_cards, keyboard, … --> |
| **Product link** (optional) | <!-- manufacturer or retailer URL --> |

### OpenRGB device info

Copy from **OpenRGB’s device list** on the system where you built this preset:

| | |
|---|---|
| **Device name** (`controller_name` in JSON) | <!-- exact string --> |
| **Location** (if not `1:1`) | |
| **Zones / LED count used** | <!-- e.g. zone 0, LEDs 0–7 --> |

### When to use this preset

<!-- e.g. "Single SP120 fan only" vs "Bottom fan of 220T front stack — use with _middle and _top presets" -->

### How it was created

- [ ] Exported from OpenRGB 3D Spatial (Custom Controllers)
- [ ] Edited from an existing preset in this repo
- [ ] Hand-authored (describe method)

### Testing

- [ ] JSON validates (single object, valid syntax)
- [ ] Tested **Add from preset** and added to 3D scene
- [ ] LEDs map to the correct physical lights
- [ ] README table updated (if this is a kit or needs a "when to use" note)

### Related presets / duplicates

<!-- Link to other files in this repo users should install together, or explain why this differs from an existing preset -->

### Screenshots (optional)

<!-- Viewport or OpenRGB device list — helps reviewers -->

### Checklist

- [ ] One preset per `.json` file
- [ ] Filename is lowercase with underscores (`brand_model_variant.json`)
- [ ] `name`, `category`, `brand`, `model` are set
- [ ] Every mapping uses the correct `controller_name`, `zone_idx`, and `led_idx`
- [ ] I agree to license this contribution under the repo license (see README)
