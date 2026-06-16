# Run all exercises under 01-Design-Patterns
$base = Join-Path (Get-Location) "01-Design-Patterns"
Get-ChildItem -Path $base -Directory | ForEach-Object {
    $dir = $_.FullName
    $proj = Get-ChildItem -Path $dir -Filter *.csproj -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($proj) {
        Write-Host "\n=== Running $($_.Name) ==="
        dotnet run --project $proj.FullName
    }
    else {
        Write-Host "Skipping $($_.Name): no .csproj found"
    }
}
