
[CmdletBinding()]
param(
	[Parameter(Mandatory)][string] $Configuration,
	[Parameter(Mandatory)][string] $TargetPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "==============================================================="
Write-Host "| Post-Build Script: Registering new assemblies with Explorer |"
Write-Host "==============================================================="
Write-Host ""

# Only proceed if the configuration is Debug; otherwise, skip
if ($Configuration -ne "Debug") {
	Write-Host ">> Configuration is not Debug, skipping assembly un-registration."
	exit 1
}

$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
	Write-Warning ">> This script requires administrative privileges to unregister assemblies. Relaunching with elevated permissions..."

	$scriptPath = $MyInvocation.MyCommand.Path
	$arguments = "-Configuration `"$Configuration`" -TargetPath `"$TargetPath`""

	$proc = Start-Process powershell -Wait -PassThru -Verb RunAs -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`" $arguments"
	exit $proc.ExitCode
}

$regsvr32 = "$env:SystemRoot\System32\regsvr32.exe"
if (-not (Test-Path $regsvr32)) {
	Write-Error ">> regsvr32.exe not found at expected location: $regsvr32"
	exit 1
}

$comHostPath = [System.IO.Path]::ChangeExtension($TargetPath, ".comhost.dll")

if (-not (Test-Path $comHostPath)) {
	Write-Error ">> COM host '$comHostPath' does not exist. Ensure <EnableComHosting> is set and the build output is correct."
	exit 1
}

try {
	$output = & $regsvr32 /s $comHostPath 2>&1
	$output | ForEach-Object { Write-Host ">> [regsvr32] $_" }

	Write-Host ">> COM host registration completed successfully: $comHostPath"
} catch {
	Write-Error ">> An error occurred during COM host registration: $_"
	exit 1
}

Write-Host ">> Done registering assemblies."
Write-Host ""
Write-Host "================================================================"
Write-Host ""