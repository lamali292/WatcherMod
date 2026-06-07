# release.ps1  ->  bumps patch, sets version everywhere, builds, then releases  (Watcher)
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$modName  = "Watcher"
$csproj   = "Watcher.csproj"
$manifest = "Watcher.json"

# Resolve game + mods paths exactly as the build does -- this honours local.props AND
# the OS defaults in Watcher.csproj, so there's one source of truth, not a copy here.
# (`-getProperty` needs .NET SDK 8.0.200+. Older SDK fallback below.)
$gameRoot   = (& dotnet msbuild $csproj -getProperty:Sts2Path).Trim()
$modsFolder = Join-Path ((& dotnet msbuild $csproj -getProperty:ModsPath).Trim()) $modName
if ([string]::IsNullOrWhiteSpace($gameRoot)) { throw "Couldn't resolve Sts2Path from $csproj (SDK 8.0.200+?)." }
# Fallback for older SDKs -- parse local.props directly:
#   $steam = (Select-Xml ./local.props -XPath '//SteamLibraryPath').Node.InnerText
#   $gameRoot = Join-Path $steam 'common/Slay the Spire 2'

# StS2 version this build targets, read from the game's own release_info.json
# (the 'version' field already includes the leading 'v', e.g. v0.107.0).
$relPath = Join-Path $gameRoot "release_info.json"
if (-not (Test-Path $relPath)) {
    throw "release_info.json not found at $relPath -- check the game path, or Steam hasn't written it for this build."
}
$gameVersion = (Get-Content $relPath -Raw | ConvertFrom-Json).version
if ([string]::IsNullOrWhiteSpace($gameVersion)) { throw "No 'version' in $relPath" }

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

# --- the display name shown on Nexus (spaces are fine -- it rides on the release NAME) ---
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

# --- Steam branch this build came from. release_info.json's "branch" is just the
#     version, so read the real branch from Steam's app manifest (betakey).
#     Empty betakey == the default/public branch. ---
$steamApps = Split-Path (Split-Path $gameRoot -Parent) -Parent   # ...\steamapps
$acf = Get-ChildItem $steamApps -Filter "appmanifest_*.acf" -ErrorAction SilentlyContinue |
        Where-Object { (Get-Content $_.FullName -Raw) -match '"name"\s+"Slay the Spire 2"' } |
        Select-Object -First 1
$branch = "default"
if ($acf -and ((Get-Content $acf.FullName -Raw) -match '"betakey"\s+"([^"]+)"')) {
    $branch = $matches[1]
}

# --- Nexus metadata rides on the release itself: NAME = display, BODY = banner + auto-notes.
#     No metadata files are attached -- the release carries only the zip. ---
$banner = @"
Works on the '$branch' branch of Slay the Spire 2 (game version $gameVersion).
Works with BaseLib $baseLib.
"@

# --- commit + tag first; generate-notes reads the pushed tag ---
git add $csproj $manifest
git commit -m "Release v$new (StS2 $gameVersion, BaseLib $baseLib)"
git tag "v$new"
git push origin HEAD --tags

# --- generate GitHub's auto release notes for this tag, then prepend the banner.
#     Building the full body up front means the single `gh release create` below
#     publishes with the complete body, so the release event carries all of it. ---
$repo = gh repo view --json nameWithOwner -q ".nameWithOwner"
$auto = gh api "repos/$repo/releases/generate-notes" -f tag_name="v$new" -q ".body"
$body = "$banner`n`n---`n`n$auto"

# --- create the release once, with the combined body (attach ONLY the zip) ---
gh release create "v$new" "$zip" --title "$display" --notes "$body"
Write-Host "Released v$new"