# Contributing to Juknum.Windows.ContextMenu

Thank you for your interest in contributing to **Juknum.Windows.ContextMenu**! This guide outlines the development environment setup, build instructions, project architecture, debugging workflow, and contribution guidelines.

---

## Table of Contents

- [Prerequisites](#prerequisites)
- [Repository Structure](#repository-structure)
- [Building the Project](#building-the-project)
  - [Command Line (CLI)](#command-line-cli)
  - [Visual Studio / IDE](#visual-studio--ide)
  - [Building Packages (NuGet)](#building-packages-nuget)
- [Build Output Layout](#build-output-layout)
- [Examples & COM Registration](#examples--com-registration)
  - [Debug Builds & Explorer Hooks](#debug-builds--explorer-hooks)
  - [Testing and Debugging in Explorer](#testing-and-debugging-in-explorer)
- [Coding Standards](#coding-standards)
- [Pull Request Workflow](#pull-request-workflow)

---

## Prerequisites

To build and test the complete solution, ensure your environment meets the following requirements:

1. **Operating System**:
   - **Windows 10 / 11** or **Windows Server** is required for registering and testing COM shell extensions in Windows Explorer.
   - *(Note: While non-Windows systems can inspect and compile standard cross-platform code, targeting `*-windows` and running COM registration scripts requires Windows).*

2. **SDKs and Frameworks**:
   - [.NET 10.0 SDK](https://dotnet.microsoft.com/download) (or latest SDK).
   - [.NET 8.0 SDK](https://dotnet.microsoft.com/download).
   - **.NET Framework 4.6.2, 4.7.2, and 4.8.1 Targeting Packs** (installed via Visual Studio Installer under the *.NET desktop development* workload).

3. **Tools & IDE**:
   - [Visual Studio 2022](https://visualstudio.microsoft.com/) (version 17.10+ recommended for `.slnx` solution format support) or [VS Code](https://code.visualstudio.com/) / [JetBrains Rider](https://www.jetbrains.com/rider/).
   - **PowerShell 5.1+ / PowerShell 7+** (used by the example build scripts).

4. **Administrator Privileges**:
   - When building the sample projects in `Debug` mode, administrative permissions are required because the pre/post-build scripts register/unregister COM classes in system registry keys and restart `explorer.exe`.

---

## Repository Structure

```
├── .github/                 # GitHub workflows & assets
│   ├── assets/              # Documentation images and screenshots
│   └── workflows/           # CI and NuGet publishing workflows
├── bin/                     # Centralized build outputs (generated during build)
├── obj/                     # Centralized intermediate files (generated during build)
├── examples/                # Example projects demonstrating context menu handlers
│   ├── net10.0-windows/     # Modern .NET (10.0) COM host example
│   └── net481/              # .NET Framework 4.8.1 COM / RegAsm example
├── src/                     # Core library source code
│   ├── Utils/               # Helpers (e.g. RegistrationHelper)
│   ├── CommandEnumerator.cs # Internal IEnumExplorerCommand implementation
│   ├── ExplorerCommand.cs   # Base class for single context menu items
│   ├── ExplorerCommandMenu.cs # Base class for context submenus
│   └── Juknum.Windows.ContextMenu.csproj
├── Directory.Build.props    # Centralized MSBuild output path configuration
├── visual.slnx              # Solution file
├── README.md                # General project overview and usage guide
└── CONTRIBUTING.md          # Contribution guidelines and build instructions
```

---

## Building the Project

### Command Line (CLI)

Open a terminal (PowerShell or Command Prompt) at the root of the repository:

#### 1. Restore Dependencies
```powershell
dotnet restore
```

#### 2. Build the Core Library
- **Build all target frameworks (Release):**
  ```powershell
  dotnet build src/Juknum.Windows.ContextMenu.csproj -c Release
  ```
- **Build a specific target framework:**
  ```powershell
  dotnet build src/Juknum.Windows.ContextMenu.csproj -f net10.0-windows -c Debug
  ```
  *(Supported targets: `net462`, `net472`, `net481`, `net8.0-windows`, `net9.0-windows`, `net10.0-windows`)*

#### 3. Build the Whole Solution
```powershell
dotnet build visual.slnx -c Release
```

> [!TIP]
> To build the solution in `Debug` configuration without triggering automatic COM registration on your local machine, build in `Release` configuration or build the library under `src/` directly.

### Visual Studio / IDE

1. Open `visual.slnx` in Visual Studio 2022 (v17.10 or newer).
2. If you plan to build and test the `examples/` projects in `Debug` configuration:
   - Run Visual Studio **as Administrator** so post-build scripts have permission to write to registry hives and restart `explorer.exe`.
3. Select your desired Configuration (`Debug` or `Release`) and Platform (`Any CPU` / `x64`).
4. Build the solution (**Ctrl + Shift + B**).

### Building Packages (NuGet)

To pack the NuGet package locally:

```powershell
dotnet pack src/Juknum.Windows.ContextMenu.csproj -c Release --output ./nupkgs /p:Version=1.0.0
```

The resulting `.nupkg` and `.snupkg` symbol packages will be written to the `./nupkgs` directory.

---

## Build Output Layout

This repository uses [`Directory.Build.props`](Directory.Build.props) to centralize all build artifacts at the repository root next to `src/`:

- **Output Binaries**: `bin/<ProjectName>/<Configuration>/<TargetFramework>/`
- **Intermediate Objects**: `obj/<ProjectName>/<Configuration>/<TargetFramework>/`

This keeps project source folders clean and prevents build artifacts from cluttering individual project directories.

---

## Examples & COM Registration

The `examples/` directory demonstrates how to implement and register context menu handlers:

### Debug Builds & Explorer Hooks

When building sample projects in `Debug` mode:
- **Pre-Build Script (`PreBuild.ps1`)**:
  - Stops `explorer.exe`.
  - Unregisters previously registered COM servers (`regsvr32 /u` for .NET 8+/10+ `.comhost.dll`, or `RegAsm.exe /u` for .NET Framework).
- **Post-Build Script (`PostBuild.ps1`)**:
  - Registers the newly built COM server (`regsvr32` for `.comhost.dll`, or `RegAsm.exe /codebase` for .NET Framework).
  - Restarts `explorer.exe`.

> [!IMPORTANT]
> The pre/post-build scripts will briefly terminate and restart Windows Explorer. If you do not want this behavior while making quick code adjustments, switch the active build configuration to **Release**.

### Testing and Debugging in Explorer

1. Build an example in **Debug** mode from an elevated terminal or elevated Visual Studio.
2. Open Windows Explorer and right-click on any file, directory, or directory background (depending on `RegistrationHelper` settings).
3. Verify that your menu item or submenu appears and triggers the expected action.
4. To debug issues:
   - You can attach the Visual Studio debugger to the `explorer.exe` process (select **Native and Managed (.NET Core / .NET Framework)** code types).
   - Alternatively, use `System.Diagnostics.Debugger.Launch()` inside `ExplorerCommand.Execute()` to trigger a debugger prompt when the menu action is clicked.

---

## Coding Standards

- **Language Version**: Latest C# language features are enabled across projects.
- **Nullable Reference Types**: `<Nullable>enable</Nullable>` is enforced. Ensure all new code avoids unhandled nullability warnings.
- **Implicit Usings**: Enabled for supported target frameworks.
- **Documentation**: All public types and methods should include descriptive XML doc comments (`/// <summary>...`).
- **COM Guidelines**:
  - Always use unique GUIDs (`[Guid("...")]`) for each command/menu class.
  - Apply `[ComVisible(true)]` and `[ClassInterface(ClassInterfaceType.None)]` to exposed COM classes.
  - Only register top-level classes via `[ComRegisterFunction]`; child commands in `ExplorerCommandMenu` should not register themselves separately in the registry.
- **Strong-Name Signing**: The core assembly is signed using `Juknum.Windows.ContextMenu.snk`. Keep `<SignAssembly>true</SignAssembly>` intact.

---

## Pull Request Workflow

1. **Fork and Clone** the repository.
2. **Create a Feature Branch**:
   ```bash
   git checkout -b feature/my-new-feature
   ```
3. **Make Changes and Verify**:
   - Ensure the solution builds cleanly with zero errors and zero warnings across all target frameworks.
   - Test both modern .NET (`net8.0-windows`, `net10.0-windows`) and .NET Framework (`net481`) where applicable.
4. **Commit Your Changes**:
   - Write clear, concise commit messages following conventional commit standards where possible.
5. **Push and Open a Pull Request**:
   - Push your branch to your fork and submit a Pull Request targeting the `main` branch.
   - Describe what the PR accomplishes, why the changes were made, and how they were tested.
