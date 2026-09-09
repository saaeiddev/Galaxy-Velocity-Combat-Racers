param([Parameter(Mandatory=$true)][string]$UnityPath)
$ErrorActionPreference = 'Stop'
$project = Split-Path $PSScriptRoot -Parent
if (-not (Test-Path $UnityPath)) { throw 'Unity editor executable not found.' }
New-Item -ItemType Directory -Force (Join-Path $project 'Builds') | Out-Null
& $UnityPath -batchmode -quit -projectPath $project -executeMethod GalaxyVelocity.Editor.ProjectSetup.BuildWindows -logFile (Join-Path $project 'Builds/windows-build.log')
if ($LASTEXITCODE -ne 0) { throw "Unity build failed ($LASTEXITCODE); see Builds/windows-build.log." }
