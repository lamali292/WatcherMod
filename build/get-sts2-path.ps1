# Resolves the game install directory from local.props (Sts2Branch -> Sts2BetaPath / Sts2PublicPath).
# Dot-source and call: $gameDir = Get-Sts2Path $props [-Branch public]
function Get-Sts2Path([xml]$Props, [string]$Branch = "") {
    $group = $Props.Project.PropertyGroup
    if (-not $Branch) { $Branch = $group.Sts2Branch }
    if (-not $Branch) { $Branch = "beta" }
    switch ($Branch) {
        "beta"   { $path = $group.Sts2BetaPath }
        "public" { $path = $group.Sts2PublicPath }
        default  { throw "Sts2Branch must be 'beta' or 'public', got '$Branch'" }
    }
    if (-not $path) { throw "local.props has no path for branch '$Branch' (Sts2BetaPath / Sts2PublicPath)" }
    return $path
}
