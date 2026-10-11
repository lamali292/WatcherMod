<#
.SYNOPSIS
    Builds the mod, launches Slay the Spire 2 with the in-game test runner armed, waits for it to finish,
    and prints the results. The mod is compiled once against beta and the same build outputs are run on
    BOTH game branches (beta, then public) by default. Exit code 0 = all passed on every branch run.

.EXAMPLE
    ./build/test.ps1                      # run every [CardTest] on beta and public
    ./build/test.ps1 -Filter Watcher      # only tests whose "Type.Method" contains "Watcher"
    ./build/test.ps1 -NoBuild -Filter Vigilance
    ./build/test.ps1 -Branch beta         # only one branch

.NOTES
    Requires Steam running (the game exe is launched directly; steam_appid.txt ships with the game).
    Game folders come from local.props (Sts2BetaPath / Sts2PublicPath).
    The mod needs a packed Watcher.pck in the mods folder; run a publish once (dotnet publish) to create it.
#>
param(
    [string]$Filter = "",
    [switch]$NoBuild,
    [ValidateSet("both", "beta", "public")][string]$Branch = "both",
    [int]$TimeoutMinutes = 5
)
$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path $PSScriptRoot -Parent
Set-Location $ProjectRoot

if (-not (Test-Path "local.props")) { throw "local.props not found. Copy local.props.example to local.props." }
$props = [xml](Get-Content "local.props")
. "$PSScriptRoot\get-sts2-path.ps1"

# The branch the mod is compiled against. Other branches get a copy of these build outputs.
$BuildBranch = "beta"

if (-not $NoBuild) {
    Write-Host "=== Building Watcher (against $BuildBranch) ===" -ForegroundColor Cyan
    dotnet build "Watcher.csproj" --nologo -v q -c ExportDebug --no-incremental "-p:Sts2Branch=$BuildBranch" | Out-Host
    if ($LASTEXITCODE -ne 0) { throw "Watcher build failed" }
}

# Runs the tests on one branch's game install and prints results.
# Returns $true when every test passed (all other output goes to the host, so the return value stays a single bool).
function Invoke-BranchTests([string]$BranchName) {
    $gameDir = Get-Sts2Path $props $BranchName
    $exe = Join-Path $gameDir "SlayTheSpire2.exe"
    if (-not (Test-Path $exe)) { throw "Game executable not found: $exe" }

    if ($BranchName -ne $BuildBranch) {
        # The mod is compiled once, against $BuildBranch, and must run unchanged on every branch.
        # Only the mod's own folders are synced so other installed mods on this branch are left alone.
        foreach ($folder in "Watcher", "BaseLib") {
            $src = Join-Path (Get-Sts2Path $props $BuildBranch) "mods\$folder"
            Write-Host "=== [$BranchName] Syncing $BuildBranch-compiled $folder from $src ===" -ForegroundColor Cyan
            robocopy $src (Join-Path $gameDir "mods\$folder") /MIR /NFL /NDL /NJH /NJS /NP | Out-Null
            if ($LASTEXITCODE -ge 8) { throw "robocopy failed ($LASTEXITCODE) syncing $folder to $BranchName" }
        }
    }

    $outFile = Join-Path $env:TEMP "watcher_tests_$BranchName.json"
    Remove-Item $outFile -ErrorAction SilentlyContinue

    $env:WATCHER_RUN_TESTS   = "1"
    $env:WATCHER_TEST_FILTER = $Filter
    $env:WATCHER_TEST_OUTPUT = $outFile
    try {
        Write-Host "=== [$BranchName] Launching game headlessly (filter: '$Filter') ===" -ForegroundColor Cyan
        $proc = Start-Process -FilePath $exe -ArgumentList "--headless", "--audio-driver", "Dummy" -WorkingDirectory (Split-Path $exe) -PassThru
        if (-not $proc.WaitForExit($TimeoutMinutes * 60 * 1000)) {
            $proc.Kill()
            throw "Timed out after $TimeoutMinutes minutes; killed the game ($BranchName)."
        }
    }
    finally {
        Remove-Item Env:WATCHER_RUN_TESTS, Env:WATCHER_TEST_FILTER, Env:WATCHER_TEST_OUTPUT -ErrorAction SilentlyContinue
    }

    if (-not (Test-Path $outFile)) {
        $log = Join-Path $env:APPDATA "SlayTheSpire2\logs\godot.log"
        throw "No result file was written for $BranchName (game exit code $($proc.ExitCode)). Check $log"
    }

    $result = Get-Content $outFile -Raw | ConvertFrom-Json
    Write-Host ""
    Write-Host ("[{0}] Seed {1}  |  {2:n1}s  |  {3} passed, {4} failed" -f `
        $BranchName, $result.Seed, ([TimeSpan]$result.Duration).TotalSeconds, $result.Passed.Count, $result.Failed.Count) `
        -ForegroundColor $(if ($result.Failed.Count -eq 0) { "Green" } else { "Red" })
    foreach ($p in $result.Passed) { Write-Host "  PASS  $p" -ForegroundColor DarkGreen }
    foreach ($f in $result.Failed) {
        Write-Host "  FAIL  $($f.Name)" -ForegroundColor Red
        Write-Host "        $($f.Message)"
    }
    Write-Host ""
    Write-Host "Full JSON: $outFile"
    return ($result.Failed.Count -eq 0)
}

$branches = if ($Branch -eq "both") { @("beta", "public") } else { @($Branch) }
$summary = [ordered]@{}
foreach ($b in $branches) {
    try { $summary[$b] = if (Invoke-BranchTests $b) { "PASSED" } else { "FAILED" } }
    catch {
        Write-Host "[$b] ERROR: $($_.Exception.Message)" -ForegroundColor Red
        $summary[$b] = "ERROR"
    }
}

Write-Host ""
Write-Host "=== Summary ===" -ForegroundColor Cyan
foreach ($b in $summary.Keys) {
    Write-Host ("  {0,-7} {1}" -f $b, $summary[$b]) -ForegroundColor $(if ($summary[$b] -eq "PASSED") { "Green" } else { "Red" })
}
exit $(if (@($summary.Values | Where-Object { $_ -ne "PASSED" }).Count -eq 0) { 0 } else { 1 })
