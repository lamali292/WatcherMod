# release.ps1  ->  bumps patch, sets version everywhere, builds, then releases  (Watcher)
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$modName  = "Watcher"
$csproj   = "Watcher.csproj"
$manifest = "Watcher.json"
# Must match $(ModsPath)$(MSBuildProjectName) from the .csproj -- where publish drops the built mod.
$modsFolder = "D:/SteamLibrary/steamapps/common/Slay the Spire 2/mods/Watcher"

# --- read current version, compute bump ---
$proj = Get-Content $csproj -Raw
if ($proj -notmatch '<Version>(\d+)\.(\d+)\.(\d+)</Version>') {
    throw "Couldn't find <Version>X.Y.Z</Version> in $csproj"
}
$current = "{0}.{1}.{2}" -f $matches[1],$matches[2],$matches[3]
$new     = "{0}.{1}.{2}" -f $matches[1],$matches[2],([int]$matches[3] + 1)
Write-Host "$current -> $new"

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

# --- package the zip from the published mod folder ---
$stage = "dist/$modName"
if (Test-Path $stage) { Remove-Item $stage -Recurse -Force }
New-Item -ItemType Directory -Path $stage -Force | Out-Null
Copy-Item $pck, $dll, (Join-Path $modsFolder "$modName.json") $stage
$zip = "dist/$modName-$new.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }
Compress-Archive -Path $stage -DestinationPath $zip
Write-Host "Packaged $zip"

# --- only now: commit, tag, upload ---
git add $csproj $manifest
git commit -m "Release v$new"
git tag "v$new"
git push origin HEAD --tags
gh release create "v$new" $zip --title "v$new" --generate-notes
Write-Host "Released v$new"