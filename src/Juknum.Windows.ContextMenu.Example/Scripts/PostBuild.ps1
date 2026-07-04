
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

# Define potential paths for RegAsm.exe
$regAsmPaths = @(
	"$env:SystemRoot\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe",
	"$env:SystemRoot\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe"
)

# Find the first valid RegAsm.exe path
$regAsm = $regAsmPaths | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $regAsm) {
	Write-Error ">> RegAsm.exe not found in expected locations. Ensure .NET Framework is installed."
	exit 1
}

if (-not (Test-Path $TargetPath)) {
	Write-Error ">> Target path '$TargetPath' does not exist. Ensure the build output is correct."
	exit 1
}

try {

	if ($psVersionTable.PSVersion.Major -eq 5) {
		$output = & $regAsm $TargetPath /codebase
	}
	else {
		$output = & $regAsm $TargetPath /codebase 2>&1
	}

	$output | ForEach-Object { Write-Host ">> [RegAsm] $_" }

	if ($LASTEXITCODE -ne 0) {
		Write-Error ">> RegAsm.exe failed with exit code $LASTEXITCODE."
		exit $LASTEXITCODE
	}
	else {
		Write-Host ">> Assembly registration completed successfully."
	}

} catch {
	Write-Error ">> An error occurred during assembly registration: $_"
	exit 1
}

Write-Host ">> Done registering assemblies."
Write-Host ""
Write-Host "================================================================"
Write-Host ""