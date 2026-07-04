
[CmdletBinding()]
param(
	[Parameter(Mandatory)][string] $Configuration,
	[Parameter(Mandatory)][string] $TargetPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "================================================================"
Write-Host "| Pre-Build Script: Unregistering old assemblies from Explorer |"
Write-Host "================================================================"
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

if (Test-Path $TargetPath) {

	try {
		$output = & $regAsm $TargetPath /unregister 2>&1
		$output | ForEach-Object { Write-Host ">> [RegAsm] $_" }

		if ($LASTEXITCODE -ne 0) {
			Write-Warning ">> RegAsm.exe exited with code $LASTEXITCODE. Check the output above for details."
			exit $LASTEXITCODE
		} else {
			Write-Host ">> Successfully unregistered assembly: $TargetPath"
		}
	} catch {
		Write-Error ">> An error occurred while trying to unregister the assembly: $_"
		exit 1
	}
}

Write-Host ">> Done unregistering assemblies."
Write-Host ""
Write-Host "================================================================"
Write-Host ""