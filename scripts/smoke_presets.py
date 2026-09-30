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
    r"namespace|using|goto|inline|noinline|extern|static|input|output)\b"
)

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

VOLUME_FOLDERS = ("spatial", "audio", "media")


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
        cand = ROOT / "effects" / folder / f"{sid}.fs"
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
                f"{path}: hash_byte 2nd arg is bare 'epoch' — use epoch*period (see twinkle.fx)"
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
uniform float u_params[4];
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

    effects = ROOT / "effects"
    for folder in VOLUME_FOLDERS:
        for path in sorted((effects / folder).glob("*.fs")):
            header, body, _ = split_fs_header_body(path.read_text(encoding="utf-8"))
            body = resolve_shader_body(path, header, body)
            frag = volume_head + body + volume_tail
            try:
                ctx.program(vertex_shader=vert, fragment_shader=frag)
            except Exception as e:
                issues.append(f"{path}: GLSL compile failed: {str(e).splitlines()[0]}")

    sf = effects / "shader-field"
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

    effects = ROOT / "effects"
    for folder in VOLUME_FOLDERS:
        d = effects / folder
        if not d.is_dir():
            issues.append(f"missing folder {d}")
            continue
        for path in sorted(d.glob("*.fs")):
            check_volume_fs(path, issues)

    sf = effects / "shader-field"
    if sf.is_dir():
        for path in sorted(sf.glob("*.fs")):
            check_shader_field_fs(path, issues)

    blocks = ROOT / "timelines" / "blocks"
    if blocks.is_dir():
        for path in sorted(blocks.rglob("*.fx")):
            check_fx(path, issues)
    else:
        issues.append(f"missing folder {blocks}")

    if args.compile_glsl or args.require_compile:
        try_compile_glsl(issues, require=args.require_compile)

    if issues:
        print(f"FAIL ({len(issues)} issue(s)):")
        for i in issues:
            print(f"  - {i}")
        return 1

    n_vol = sum(len(list((effects / f).glob("*.fs"))) for f in VOLUME_FOLDERS if (effects / f).is_dir())
    n_sf = len(list(sf.glob("*.fs"))) if sf.is_dir() else 0
    n_fx = len(list(blocks.rglob("*.fx"))) if blocks.is_dir() else 0
    print(f"OK: {n_vol} volume/media/audio .fs, {n_sf} shader-field .fs, {n_fx} timeline .fx")
    return 0


if __name__ == "__main__":
    sys.exit(main())
