#!/usr/bin/env python3
"""Static smoke checks for OpenRGB3DSpatialPresets content.

Validates FolderVolume / Shader Field headers, bans legacy GLSL, scans for
reserved GLSL identifiers in effect bodies, and does light timeline .fx checks.

Exit 0 on success. Use --compile-glsl with moderngl for optional Core 410
compile of volumeMain / spatialMain wrappers (skipped if moderngl missing
unless --require-compile).
"""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

# Keys FolderVolume ReadSpec / SpatialShaderCatalog know how to skip or handle.
KNOWN_HEADER_KEYS = {
    "name",
    "description",
    "section",
    "pattern",
    "index",
    "palette",
    "class",
    "category",
    "finish",
    "param",
    "pattern_key",
    "colors",
    "supports_strip_colormap",
    "supports_height_bands",
    "slider",
    "resolution",
    "rainbow",
    "needs_frequency",
    "needs_arms",  # reserved no-op (catalog skip); prefer slider: num_arms
    "show_axis",  # reserved no-op
    "soft_catch",  # legacy no-op (FolderVolume / catalog skip)
    "user_colors",
    "pattern_label",
    "combo",
    "option",
    "pattern_index",
    "pattern_source",
    "sample",
    "flow",
    "global",
    "media",
    "drive",
    "audio_preset",
    "audio_media",
    "shader",
    "media_layout",
    "instances",
}

LEGACY_GLSL = re.compile(
    r"\b(texture2D|attribute|varying|gl_FragColor|ftransform|gl_TexCoord)\b"
)

# Reserved / keywords that break as identifiers under #version 410 core.
# Allow layout( as a qualifier.
RESERVED_ID = re.compile(
    r"\b(layout|packed|shared|flat|smooth|noperspective|centroid|patch|buffer|"
    r"atomic_uint|coherent|volatile|restrict|readonly|writeonly|subroutine|"
    r"precise|invariant|attribute|varying|common|partition|active|asm|class|"
    r"union|enum|typedef|template|this|resource|public|private|protected|"
    r"namespace|using|goto|inline|noinline|extern|static|input|output|"
    r"half|fixed|long|short|unsigned|superp|external|interface|filter|sizeof|cast|"
    r"hvec[234]|fvec[234])\b"
)

# Keys the plugin saves for every effect (SpatialEffect3D::SaveSettings plus audio / media /
# pattern settings). A look's slider / combo / pattern_key shares that JSON object, so reusing
# one makes the two settings overwrite each other.
RESERVED_SETTING_KEYS = {
    "speed", "brightness", "frequency", "detail", "size", "scale_value", "scale_inverted",
    "effect_bounds_mode", "rainbow_mode", "spatial_mapping_mode", "compass_layer_spin_preset",
    "effect_sampler_influence_centi", "effect_sampler_compass_north_offset_deg",
    "effect_compass_discrete_zones", "intensity", "sharpness", "smoothing",
    "sampling_resolution", "edge_profile", "edge_thickness", "glow_level", "axis_scale_x",
    "axis_scale_y", "axis_scale_z", "rotation_yaw", "rotation_pitch", "rotation_roll",
    "shear_xy", "shear_xz", "shear_yz", "volume_shape", "axis_scale_rotation_yaw",
    "axis_scale_rotation_pitch", "axis_scale_rotation_roll", "colors", "fps", "path_axis",
    "band_thickness", "edge_fade", "instance_count", "plane", "surface_mask", "offset_x",
    "offset_y", "offset_z", "reference_mode", "global_ref_x", "global_ref_y", "global_ref_z",
    "custom_ref_x", "custom_ref_y", "custom_ref_z", "use_custom_ref", "room_output_role",
    "room_relay_light", "room_emitter_controllers", "room_receiver_controllers",
    "stratum_layout_mode", "stratum_band_speed_pct", "stratum_band_tight_pct",
    "stratum_band_phase_deg", "stratum_band_motion_kind", "stratum_band_motion_rate",
    "strip_cmap_on", "strip_cmap_kernel", "strip_cmap_rep", "strip_cmap_unfold",
    "strip_cmap_dir", "media_path", "tile_repeat_enabled", "audio_smoothing", "low_hz",
    "high_hz", "falloff", "peak_boost", "drive_mode", "sustain_reject", "register_role",
    "pulse_color_mode", "beat_wave_mode", "wave_spread", "wave_decay",
}


def check_setting_keys(header: dict[str, list[str]], path: Path, issues: list[str]) -> None:
    seen: set[str] = set()
    for kind in ("slider", "combo", "pattern_key"):
        for value in header.get(kind, []):
            name = value.split("|")[0].split()
            if not name:
                continue
            key = name[0]
            if key in RESERVED_SETTING_KEYS:
                issues.append(f"{path}: {kind} '{key}' clashes with a built-in setting key")
            if key in seen:
                issues.append(f"{path}: setting key '{key}' is used twice")
            seen.add(key)


FINISH_VALUES = {
    "depth",
    "hex",
    "hsv",
    "spiral",
    "atlas",
    "surface",
    "rgb",
    "audio",
}

# FolderVolume categories under effects/ (legacy name "spatial" → volume).
VOLUME_FOLDERS = ("volume", "audio", "media")
EFFECTS = ROOT / "effects"
ACTION_EDITOR = ROOT / "action-editor"
TRIGGERS = ROOT / "triggers"
MOVIE_SYNC = ROOT / "movie-sync"


def split_fs_header_body(text: str) -> tuple[dict[str, list[str]], str, list[str]]:
    """Return (header_map key->values, body, parse_errors)."""
    header: dict[str, list[str]] = {}
    errors: list[str] = []
    lines = text.splitlines()
    body_start = 0
    for i, line in enumerate(lines):
        trimmed = line.strip()
        if not trimmed or trimmed.startswith("#"):
            continue
        colon = trimmed.find(":")
        if colon <= 0:
            body_start = i
            break
        key = trimmed[:colon].strip()
        val = trimmed[colon + 1 :].strip()
        if key not in KNOWN_HEADER_KEYS:
            errors.append(f"unknown header key '{key}' (parser would stop before finish/param)")
            body_start = i
            break
        header.setdefault(key, []).append(val)
    else:
        body_start = len(lines)
    body = "\n".join(lines[body_start:]) + ("\n" if body_start < len(lines) else "")
    return header, body, errors


def strip_glsl_comments(code: str) -> str:
    code = re.sub(r"/\*.*?\*/", "", code, flags=re.S)
    code = re.sub(r"//.*?$", "", code, flags=re.M)
    return code


def check_reserved_ids(body: str, path: Path, issues: list[str]) -> None:
    code = strip_glsl_comments(body)
    for m in RESERVED_ID.finditer(code):
        word = m.group(1)
        after = code[m.end() : m.end() + 1]
        if word == "layout" and after == "(":
            continue
        # Approximate line number
        line_no = code[: m.start()].count("\n") + 1
        issues.append(f"{path}: reserved GLSL identifier '{word}' near body line {line_no}")


def resolve_shader_body(path: Path, header: dict[str, list[str]], body: str) -> str:
    """If header has shader: <id>, load that sibling .fs body (bass-punch → audio-pulse)."""
    overrides = header.get("shader") or []
    if not overrides:
        return body
    sid = overrides[0].strip()
    if not sid:
        return body
    for folder in VOLUME_FOLDERS:
        cand = EFFECTS / folder / f"{sid}.fs"
        if cand.is_file():
            _, obody, _ = split_fs_header_body(cand.read_text(encoding="utf-8"))
            return obody
    return body


def check_volume_fs(path: Path, issues: list[str]) -> None:
    text = path.read_text(encoding="utf-8")
    header, body, errs = split_fs_header_body(text)
    for e in errs:
        issues.append(f"{path}: {e}")
    if "class" not in header:
        issues.append(f"{path}: missing class:")
    if "finish" not in header:
        issues.append(f"{path}: missing finish:")
    else:
        for f in header["finish"]:
            if f not in FINISH_VALUES:
                issues.append(f"{path}: unknown finish: {f}")
    check_setting_keys(header, path, issues)
    body = resolve_shader_body(path, header, body)
    if "volumeMain" not in body:
        issues.append(f"{path}: body missing volumeMain")
    if LEGACY_GLSL.search(body):
        issues.append(f"{path}: legacy GLSL token in body ({LEGACY_GLSL.search(body).group(1)})")
    check_reserved_ids(body, path, issues)


def check_shader_field_fs(path: Path, issues: list[str]) -> None:
    text = path.read_text(encoding="utf-8")
    header, body, errs = split_fs_header_body(text)
    for e in errs:
        issues.append(f"{path}: {e}")
    if "spatialMain" not in body:
        issues.append(f"{path}: body missing spatialMain")
    if LEGACY_GLSL.search(body):
        issues.append(f"{path}: legacy GLSL token in body ({LEGACY_GLSL.search(body).group(1)})")
    check_reserved_ids(body, path, issues)


def check_fx(path: Path, issues: list[str]) -> None:
    text = path.read_text(encoding="utf-8")
    if not re.search(r"^name:\s*\S", text, re.M):
        issues.append(f"{path}: missing name:")
    if "# Effect" not in text and not re.search(r"^# Effect", text, re.M):
        # some files use "# Effect" section marker
        if "paint" not in text and "off" not in text and "paint_stop" not in text:
            issues.append(f"{path}: no effect body markers (paint/off)")
    # Bare epoch as hash_byte 2nd arg (should be epoch*period like twinkle)
    for m in re.finditer(r"hash_byte\s*\(([^)]*)\)", text):
        args = [a.strip() for a in m.group(1).split(",")]
        if len(args) >= 2 and re.fullmatch(r"epoch", args[1]):
            issues.append(
                f"{path}: hash_byte 2nd arg is bare 'epoch' - use epoch*period (see twinkle.fx)"
            )


def try_compile_glsl(issues: list[str], require: bool) -> None:
    try:
        import moderngl
    except ImportError:
        if require:
            issues.append("moderngl not installed (--require-compile)")
        else:
            print("note: moderngl not installed; skipping --compile-glsl")
        return

    vert = """#version 410 core
layout(location=0) in vec2 a_position;
void main() { gl_Position = vec4(a_position, 0.0, 1.0); }
"""
    volume_head = """#version 410 core
uniform float u_time;
uniform float u_res;
uniform vec2 u_atlas;
uniform float u_params[64];
uniform sampler2D u_media;
out vec4 frag_color;
void volumeMain(out vec4 out_color, in vec3 p01);
"""
    volume_tail = """
void main() {
    float n = max(u_res, 1.0);
    float fx = floor(gl_FragCoord.x);
    float fy = floor(gl_FragCoord.y);
    float slice = floor(fy / n);
    float ly = fy - slice * n;
    vec3 p01 = vec3((fx + 0.5) / n, (ly + 0.5) / n, (slice + 0.5) / n);
    vec4 c = vec4(0.0);
    volumeMain(c, clamp(p01, 0.0, 1.0));
    frag_color = vec4(clamp(c.rgb, 0.0, 1.0), 1.0);
}
"""
    spatial_head = """#version 410 core
uniform float u_time;
uniform vec2 u_resolution;
uniform float u_params[8];
out vec4 frag_color;
void spatialMain(out vec4 out_color, in vec2 frag_coord);
"""
    spatial_tail = """
void main() {
    vec4 c = vec4(0.0);
    spatialMain(c, gl_FragCoord.xy);
    frag_color = clamp(c, 0.0, 1.0);
}
"""

    try:
        ctx = moderngl.create_standalone_context(require=410)
    except Exception as e:
        msg = f"OpenGL 410 context unavailable: {e}"
        if require:
            issues.append(msg)
        else:
            print(f"note: {msg}; skipping compile")
        return

    for folder in VOLUME_FOLDERS:
        for path in sorted((EFFECTS / folder).glob("*.fs")):
            header, body, _ = split_fs_header_body(path.read_text(encoding="utf-8"))
            body = resolve_shader_body(path, header, body)
            frag = volume_head + body + volume_tail
            try:
                ctx.program(vertex_shader=vert, fragment_shader=frag)
            except Exception as e:
                issues.append(f"{path}: GLSL compile failed: {str(e).splitlines()[0]}")

    sf = EFFECTS / "shader-field"
    if sf.is_dir():
        for path in sorted(sf.glob("*.fs")):
            _, body, _ = split_fs_header_body(path.read_text(encoding="utf-8"))
            frag = spatial_head + body + spatial_tail
            try:
                ctx.program(vertex_shader=vert, fragment_shader=frag)
            except Exception as e:
                issues.append(f"{path}: GLSL compile failed: {str(e).splitlines()[0]}")


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--compile-glsl", action="store_true", help="Compile wrappers via moderngl")
    ap.add_argument(
        "--require-compile",
        action="store_true",
        help="Fail if moderngl / GL 410 context unavailable",
    )
    args = ap.parse_args()
    issues: list[str] = []

    if not EFFECTS.is_dir():
        issues.append(f"missing folder {EFFECTS}")

    for folder in VOLUME_FOLDERS:
        d = EFFECTS / folder
        if not d.is_dir():
            issues.append(f"missing folder {d}")
            continue
        for path in sorted(d.glob("*.fs")):
            check_volume_fs(path, issues)

    sf = EFFECTS / "shader-field"
    if sf.is_dir():
        for path in sorted(sf.glob("*.fs")):
            check_shader_field_fs(path, issues)

    toolbox = ACTION_EDITOR / "toolbox"
    if toolbox.is_dir():
        for path in sorted(toolbox.rglob("*.fx")):
            check_fx(path, issues)
    else:
        issues.append(f"missing folder {toolbox}")

    trigger_effects = TRIGGERS / "effects"
    if not trigger_effects.is_dir():
        issues.append(f"missing folder {trigger_effects}")

    movie_effects = MOVIE_SYNC / "effects"
    if not movie_effects.is_dir():
        issues.append(f"missing folder {movie_effects}")

    if args.compile_glsl or args.require_compile:
        try_compile_glsl(issues, require=args.require_compile)

    if issues:
        print(f"FAIL ({len(issues)} issue(s)):")
        for i in issues:
            print(f"  - {i}")
        return 1

    n_vol = sum(len(list((EFFECTS / f).glob("*.fs"))) for f in VOLUME_FOLDERS if (EFFECTS / f).is_dir())
    n_sf = len(list(sf.glob("*.fs"))) if sf.is_dir() else 0
    n_fx = len(list(toolbox.rglob("*.fx"))) if toolbox.is_dir() else 0
    n_trig = len(list(trigger_effects.glob("*.action.json"))) if trigger_effects.is_dir() else 0
    n_movie = len(list(movie_effects.rglob("*.action.json"))) if movie_effects.is_dir() else 0
    print(
        f"OK: {n_vol} volume/media/audio .fs, {n_sf} shader-field .fs, "
        f"{n_fx} action-editor .fx, {n_trig} trigger actions, {n_movie} movie-sync looks"
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())
