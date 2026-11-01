<!-- catalog
title: Corsair SP120 / iCUE 220T RGB
prefixes: corsair_icue_220t_, corsair_sp120_
-->

The [iCUE 220T RGB](https://www.corsair.com/us/en/p/case/icue-220t-rgb-tempered-glass-mid-tower-atx-smart-case-cc-9011133-ww/cc-9011133-ww) ships with **three** SP120 RGB PRO fans on the front. OpenRGB often exposes them as **one** device with **24 LEDs** on zone 0 (8 per fan). Use all three `corsair_icue_220t_sp120_front_*.json` presets and place them at bottom / middle / top in your 3D scene.

If you only have **one** SP120 (not the 220T stack), use `corsair_sp120_rgb_pro.json` instead.

Mappings still use `controller_name` **"Corsair SP120 RGB PRO"** so they match OpenRGB; edit that string in the JSON if your device name differs.
