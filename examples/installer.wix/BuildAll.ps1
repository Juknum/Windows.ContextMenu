<#
.SYNOPSIS
    Builds the WiX MSI installer for x86 (32-bit), x64 (64-bit), and ARM64.

.DESCRIPTION
    This script publishes the ContextMenuExample project as self-contained for each target architecture
    and compiles the corresponding .msi package using WiX v4/v5.

.PARAMETER Platform
    Target platform: 'All', 'x64', 'x86', or 'arm64'. Defaults to 'All'.

.PARAMETER Configuration
    Build configuration: 'Release' (default) or 'Debug'.
#>

[CmdletBinding()]
param(
    [ValidateSet("All", "x64", "x86", "arm64")]
    [string]$Platform = "All",

    [ValidateSet("Release", "Debug")]
    [string]$Configuration = "Release"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$wixProj = Join-Path $scriptDir "Installer.wixproj"

$platformsToBuild = if ($Platform -eq "All") { @("x64", "x86", "arm64") } else { @($Platform) }

Write-Host "================================================================" -ForegroundColor Cyan
Write-Host " Building ContextMenu MSI Packages (Option 2: Self-Contained + regsvr32)" -ForegroundColor Cyan
Write-Host " Configuration: $Configuration" -ForegroundColor Cyan
Write-Host " Targets:       $($platformsToBuild -join ', ')" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan

foreach ($arch in $platformsToBuild) {
    Write-Host "`n>>> [Building MSI for $arch ($Configuration)] <<<" -ForegroundColor Yellow

    $buildArgs = @(
        "build",
        "`"$wixProj`"",
        "-c", $Configuration,
        "-p:Platform=$arch"
    )

    & dotnet @buildArgs

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to build MSI for $arch."
        exit $LASTEXITCODE
    }

    Write-Host ">>> Successfully built MSI for $arch." -ForegroundColor Green
}

Write-Host "`n================================================================" -ForegroundColor Green
Write-Host " All requested MSI packages built successfully!" -ForegroundColor Green
Write-Host " Output directory: bin\Installer\$Configuration\" -ForegroundColor Green
Write-Host "================================================================" -ForegroundColor Green
