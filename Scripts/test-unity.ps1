param([Parameter(Mandatory=$true)][string]$UnityPath)
$ErrorActionPreference = 'Stop'
$project = Split-Path $PSScriptRoot -Parent
New-Item -ItemType Directory -Force (Join-Path $project 'Tests/results') | Out-Null
& $UnityPath -batchmode -nographics -projectPath $project -runTests -testPlatform EditMode -testResults (Join-Path $project 'Tests/results/unity.xml') -logFile (Join-Path $project 'Tests/results/unity.log')
if ($LASTEXITCODE -ne 0) { throw "Unity tests failed ($LASTEXITCODE); see Tests/results." }
