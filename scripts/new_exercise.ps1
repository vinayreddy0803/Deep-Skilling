param(
    [Parameter(Mandatory=$true)] [int] $Number,
    [Parameter(Mandatory=$true)] [string] $Name
)

$folderName = "Exercise$Number-$Name"
$target = Join-Path (Join-Path (Get-Location) '01-Design-Patterns') $folderName

if (Test-Path $target) {
    Write-Host "Target $target already exists. Exiting." -ForegroundColor Yellow
    exit 1
}

# Create console project
dotnet new console -o $target

# Add README from template
$template = Join-Path (Get-Location) 'templates\exercise_README.md'
$readme = Get-Content $template -Raw
$readme = $readme -replace '\{ExerciseName\}',$folderName -replace '\{PatternName\}',$Name -replace '\{ExerciseFolder\}',$folderName
Set-Content -Path (Join-Path $target 'README.md') -Value $readme -Encoding UTF8

# Stage and commit
git add $target
git commit -m "Add $folderName example"

git push -u origin main

Write-Host "Created and pushed $folderName"
