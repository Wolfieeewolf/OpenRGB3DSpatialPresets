#!/usr/bin/env pwsh
# Publish local .wiki-seed/ to the GitHub wiki repo.
# One-time prerequisite: open
#   https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/wiki
# and create the first page (any text). That creates the .wiki.git remote.
#
# Usage (from repo root):
#   .\scripts\publish-wiki.ps1

$ErrorActionPreference = 'Stop'
$Root = Split-Path $PSScriptRoot -Parent
$Seed = Join-Path $Root '.wiki-seed'
$Wiki = Join-Path (Split-Path $Root -Parent) 'OpenRGB3DSpatialPresets.wiki'
$Remote = 'https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets.wiki.git'

if (-not (Test-Path $Seed)) {
    throw @"
Missing seed folder: $Seed
Recreate a local .wiki-seed/ with the wiki markdown pages, then re-run.
"@
}

if (Test-Path $Wiki) {
    Remove-Item -Recurse -Force $Wiki
}

Write-Host "Cloning $Remote ..."
git clone $Remote $Wiki
if ($LASTEXITCODE -ne 0) {
    throw @"
Clone failed. Create the first wiki page in the GitHub UI first:
  https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/wiki
Then re-run this script.
"@
}

Copy-Item (Join-Path $Seed '*') $Wiki -Force
Set-Location $Wiki
git add -A
$status = git status --porcelain
if (-not $status) {
    Write-Host 'Wiki already up to date.'
    exit 0
}
git -c user.name='WolfieeewWolf' -c user.email='wolfieee@msn.com' commit -m 'docs(wiki): sync presets content-pack pages from .wiki-seed'
git push origin HEAD
Write-Host "Done. https://github.com/Wolfieeewolf/OpenRGB3DSpatialPresets/wiki"
