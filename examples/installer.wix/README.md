# WiX MSI Installer Example (Option 2: Self-Contained + COM Registration)

This example demonstrates how to package a .NET Windows Explorer context menu handler into an **MSI installer** using **WiX Toolset (v4 / v5)**:
- Bundles all runtime and dependent DLLs into the installation folder (**self-contained** publish).
- Registers and unregisters the COM host via **elevated deferred Custom Actions** executing `regsvr32.exe`.
- Supports **x86 (32-bit)**, **x64 (64-bit)**, and **ARM64 (Windows on ARM)** architectures.

---

## Supported Architectures

| Architecture | Platform Flag | .NET RID | Program Files Folder | System Tool Path | Target Systems |
|---|---|---|---|---|---|
| **x64** | `-p:Platform=x64` | `win-x64` | `ProgramFiles64Folder` | `[System64Folder]regsvr32.exe` | Standard 64-bit Windows 10/11 |
| **x86** | `-p:Platform=x86` | `win-x86` | `ProgramFilesFolder` | `[SystemFolder]regsvr32.exe` | 32-bit Windows |
| **ARM64** | `-p:Platform=arm64` | `win-arm64` | `ProgramFiles64Folder` | `[System64Folder]regsvr32.exe` | Windows 11 on ARM (Snapdragon X Elite, Surface Pro Copilot+, etc.) |

---

## How It Works

1. **Self-Contained Publish**: When the WiX project builds, its `PublishContextMenuPayload` target invokes:
   ```bash
   dotnet publish ..\net10.0-windows\.NET 10.0.csproj -c Release -r <RID> --self-contained true -o publish\<RID>\
   ```
   This outputs the native `ContextMenuExample.comhost.dll` along with all required .NET Core runtime files (`coreclr.dll`, `hostfxr.dll`, etc.) and dependencies (`Vanara.PInvoke.*.dll`, `Juknum.Windows.ContextMenu.dll`).

2. **File Harvesting**: WiX bundles all files from the `publish/<RID>/` directory into the target installation directory (`C:\Program Files\ContextMenuExample`).

3. **COM Registration via Custom Actions**:
   - **On Install / Upgrade**: Windows Installer runs `regsvr32.exe /s "[#ComHostDll]"` after copying files. This loads the COM host, which executes `[ComRegisterFunction]` in `ContextMenuExample.cs` and calls `RegistrationHelper.RegisterCommand(...)` to write the Explorer verbs (`*\shell`, `Directory\shell`, `Directory\Background\shell`).
   - **On Uninstall**: Windows Installer runs `regsvr32.exe /u /s "[#ComHostDll]"` before deleting files. This executes `[ComUnregisterFunction]` to cleanly remove the Explorer verbs and COM keys from the registry.

---

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) (or matching target framework).
- WiX Toolset SDK (v4 or v5):
  ```bash
  dotnet new install WixToolset.Template
  ```

---

## Building the MSIs

### Build All Architectures with PowerShell

```powershell
.\BuildAll.ps1
```

### Or Build a Specific Architecture with `dotnet build`

- **64-bit (x64)**:
  ```bash
  dotnet build Installer.wixproj -c Release -p:Platform=x64
  ```

- **32-bit (x86)**:
  ```bash
  dotnet build Installer.wixproj -c Release -p:Platform=x86
  ```

- **ARM64**:
  ```bash
  dotnet build Installer.wixproj -c Release -p:Platform=arm64
  ```

The resulting `.msi` packages will be placed in `bin/Installer/<Configuration>/`.

---

## Installing & Testing

To install (requires elevation):
```cmd
msiexec /i bin\Installer\Release\ContextMenuExample-x64.msi
```

To uninstall:
```cmd
msiexec /x bin\Installer\Release\ContextMenuExample-x64.msi
```

Or silently:
```cmd
msiexec /i ContextMenuExample-x64.msi /qn /norestart
msiexec /x ContextMenuExample-x64.msi /qn /norestart
```
