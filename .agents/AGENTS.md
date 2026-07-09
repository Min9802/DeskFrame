# Workspace Agent Rules - DeskFrame

This document contains configuration rules and guidelines specifically for AI Coding Agents when working on the DeskFrame project.

## 📌 Automatic Version Update Rule (Version Control)

To ensure the Windows Installer (`.msi`) package works correctly, automatically uninstalling older versions and overwriting with the new version (rather than installing them side-by-side as two separate apps), you **must update the application version** every time you make major source code changes or bug fixes.

### 1. Files to be updated simultaneously
Whenever you increment the version, you must update the exact same version number in these two locations:
* **C# Project File:** [DeskFrame.csproj](../DeskFrame/DeskFrame.csproj)
  Update the values inside the `<AssemblyVersion>` and `<FileVersion>` tags:
  ```xml
  <AssemblyVersion>1.44.1</AssemblyVersion>
  <FileVersion>1.44.1</FileVersion>
  ```
* **WiX Product Configuration:** [Product.wxs](../Installer/Product.wxs)
  Update the `Version` attribute in the `<Package>` tag:
  ```xml
  <Package Name="DeskFrame"
           Manufacturer="Min9802"
           Version="1.44.1"
           ...
  ```

### 2. Version Increment Rules (Semantic Versioning)
Follow the `Major.Minor.Patch` version format (e.g., `1.44.0`):
* **Patch Version** (Third number, e.g., `1.44.0` -> `1.44.1`): Increment when applying bug fixes, minor optimizations, or changes that do not alter the main features of the application.
* **Minor Version** (Second number, e.g., `1.44.0` -> `1.45.0`): Increment when adding significant new features or changing the UI/configuration of the frames.
* **Major Version** (First number, e.g., `1.44.0` -> `2.0.0`): Increment when making major architectural changes or breaking changes.

### 3. Execution Workflow after code modification
1. Modify the code as requested.
2. Increment the version number simultaneously in both `DeskFrame.csproj` and `Product.wxs`.
3. Compile and package the application using the command:
   `powershell -ExecutionPolicy Bypass -File build.ps1`
