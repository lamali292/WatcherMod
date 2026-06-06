# release.ps1  ->  bumps patch, sets version everywhere, builds, then releases  (Watcher)
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$modName  = "Watcher"
$csproj   = "Watcher.csproj"
$manifest = "Watcher.json"
# Must match $(ModsPath)$(MSBuildProjectName) from the .csproj -- where publish drops the built mod.
$modsFolder = "D:/SteamLibrary/steamapps/common/Slay the Spire 2/mods/Watcher"
# StS2 version this build targets. Bump this one line when the game updates.
$gameVersion = "v0.107.0"

# --- read current version, compute bump ---
$proj = Get-Content $csproj -Raw
if ($proj -notmatch '<Version>(\d+)\.(\d+)\.(\d+)</Version>') {
    throw "Couldn't find <Version>X.Y.Z</Version> in $csproj"
}
$current = "{0}.{1}.{2}" -f $matches[1],$matches[2],$matches[3]
$new     = "{0}.{1}.{2}" -f $matches[1],$matches[2],([int]$matches[3] + 1)

# --- read BaseLib version straight from the PackageReference ---
if ($proj -notmatch 'Alchyr\.Sts2\.BaseLib"\s+Version="([^"]+)"') {
    throw "Couldn't find the Alchyr.Sts2.BaseLib PackageReference version in $csproj"
}
$baseLib = $matches[1]
Write-Host "$current -> $new  (StS2 $gameVersion, BaseLib $baseLib)"

# --- fail early on a stale tag, before changing anything ---
if (git tag --list "v$new") { throw "Tag v$new already exists. Delete it (git tag -d v$new) or bump." }

# --- write version into the .csproj (all three fields) ---
$proj = $proj -replace '<Version>\d+\.\d+\.\d+</Version>',                      "<Version>$new</Version>"
$proj = $proj -replace '<AssemblyVersion>\d+\.\d+\.\d+\.\d+</AssemblyVersion>', "<AssemblyVersion>$new.0</AssemblyVersion>"
$proj = $proj -replace '<FileVersion>\d+\.\d+\.\d+\.\d+</FileVersion>',         "<FileVersion>$new.0</FileVersion>"
Set-Content $csproj $proj -Encoding UTF8 -NoNewline

# --- write the manifest ---
$json = Get-Content $manifest -Raw | ConvertFrom-Json
$json.version = $new
$json | ConvertTo-Json -Depth 10 | Set-Content $manifest -Encoding UTF8
Write-Host "Set version in $manifest"

# --- build (Godot's exit-time errors are expected; a fresh .pck is the real success check) ---
dotnet publish $csproj -c Release -p:Version=$new

$pck = Join-Path $modsFolder "$modName.pck"
$dll = Join-Path $modsFolder "$modName.dll"
foreach ($f in @($pck, $dll)) {
    if (-not (Test-Path $f)) { throw "Missing build output: $f -- build did not produce it. Nothing committed." }
    if ((Get-Item $f).LastWriteTime -lt (Get-Date).AddMinutes(-10)) {
        throw "$f is stale (not rebuilt this run). Aborting."
    }
}

# --- the display name shown on Nexus (spaces are fine -- it's sent as a value, not a file name) ---
$display = "The Watcher - $new - StS2 - $gameVersion"
# --- the zip's file name. GitHub dots out spaces in asset names, so keep it space-free. ---
$safeName = ($display -replace ' - ', '-') -replace ' ', '_'   # The_Watcher-1.4.10-StS2-v0.107.0

# --- package the zip from the published mod folder ---
$stage = "dist/$modName"
if (Test-Path $stage) { Remove-Item $stage -Recurse -Force }
New-Item -ItemType Directory -Path $stage -Force | Out-Null
Copy-Item $pck, $dll, (Join-Path $modsFolder "$modName.json") $stage
$zip = "dist/$safeName.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }
Compress-Archive -Path $stage -DestinationPath $zip
Write-Host "Packaged $zip"

# --- metadata files for the workflow. Their CONTENTS keep spaces; only asset file NAMES get dotted. ---
$nameFile = "dist/nexus-display-name.txt"
Set-Content $nameFile $display -Encoding UTF8 -NoNewline

$descFile = "dist/nexus-description.txt"
$desc = @"
Works ONLY on the Beta branch of Slay the Spire 2 (game version $gameVersion).
Works with BaseLib $baseLib.
"@
Set-Content $descFile $desc -Encoding UTF8

# --- only now: commit, tag, upload (attach the zip + both metadata files) ---
git add $csproj $manifest
git commit -m "Release v$new (StS2 $gameVersion, BaseLib $baseLib)"
git tag "v$new"
git push origin HEAD --tags
gh release create "v$new" "$zip" "$nameFile" "$descFile" --title "v$new" --generate-notes
Write-Host "Released v$new"