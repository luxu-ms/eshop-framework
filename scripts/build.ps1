[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug'
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $repositoryRoot 'eShopLegacy.sln'
$packagesDirectory = Join-Path $repositoryRoot 'packages'

$nugetCommand = Get-Command nuget.exe -ErrorAction SilentlyContinue
$nugetPath = if ($nugetCommand) { $nugetCommand.Source } else { $null }
if (-not $nugetPath) {
    $fallbackNugetPath = Join-Path $env:ProgramFiles 'NuGet\nuget.exe'
    if (Test-Path $fallbackNugetPath) {
        $nugetPath = $fallbackNugetPath
    }
}

if (-not $nugetPath) {
    throw 'nuget.exe was not found. Install NuGet CLI or add it to PATH.'
}

$vswherePath = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path $vswherePath)) {
    throw 'vswhere.exe was not found. Install Visual Studio Build Tools with the web development workload.'
}

$msbuildPath = & $vswherePath -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (-not $msbuildPath) {
    throw 'Full-framework MSBuild was not found.'
}

& $nugetPath restore $solution -PackagesDirectory $packagesDirectory -NonInteractive
if ($LASTEXITCODE -ne 0) {
    throw "NuGet restore failed with exit code $LASTEXITCODE."
}

& $msbuildPath $solution /t:Build /p:Configuration=$Configuration /m /v:minimal
if ($LASTEXITCODE -ne 0) {
    throw "MSBuild failed with exit code $LASTEXITCODE."
}