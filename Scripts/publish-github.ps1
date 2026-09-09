param([ValidateSet('private','public')][string]$Visibility='private')
$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$account = & gh api user --jq .login
if ($LASTEXITCODE -ne 0 -or $account.Trim() -ne 'saaeiddev') { throw 'Authenticate GitHub CLI as saaeiddev first.' }
if (-not (Test-Path '.git')) {
    & git init -b main
    if ($LASTEXITCODE -ne 0) { throw 'Git initialization failed.' }
    & git add .
    & git commit -m 'feat: add Galaxy Velocity prototype and validation'
    if ($LASTEXITCODE -ne 0) { throw 'Commit failed; configure your Git author name and email.' }
}
& gh repo create 'saaeiddev/Galaxy-Velocity-Combat-Racers' "--$Visibility" --source . --remote origin --push --description 'Unity 6 combat racing prototype with an offline WebGL companion'
if ($LASTEXITCODE -ne 0) { throw 'Repository creation/push failed. Existing repositories are not overwritten.' }
