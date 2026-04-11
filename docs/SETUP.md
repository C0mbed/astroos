# AstroOS Dev Environment Setup

Post-reinstall guide for the AstroOS development machine.

---

## 1. Windows Configuration

Run PowerShell as Administrator:

```powershell
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
```

Enable Developer Mode:
- Settings → System → For developers → Developer Mode → On

---

## 2. Install Tools via winget

```powershell
winget install --id Git.Git -e --accept-source-agreements --accept-package-agreements
winget install --id GitHub.cli -e --accept-source-agreements --accept-package-agreements
winget install --id OpenJS.NodeJS.LTS -e --accept-source-agreements --accept-package-agreements
winget install --id Microsoft.VisualStudio.2022.Community -e --accept-source-agreements --accept-package-agreements `
  --override "--add Microsoft.VisualStudio.Workload.ManagedDesktop --add Microsoft.VisualStudio.Workload.Universal --includeRecommended"
```

Install Claude Code (after Node.js):

```powershell
npm install -g @anthropic-ai/claude-code
```

### Windows App Runtime

The app targets Windows App SDK 1.8. After reinstall, install the latest runtime:

```powershell
winget source update
winget install --id Microsoft.WindowsAppRuntime.1.8 --accept-source-agreements --accept-package-agreements
```

If winget does not have a newer version, download the `.msixbundle` from the
[Windows App SDK releases page](https://github.com/microsoft/WindowsAppSDK/releases) and run:

```powershell
Add-AppxPackage -Path .\Microsoft.WindowsAppRuntime.Release.1.8.msixbundle -ForceApplicationShutdown -ForceUpdateFromAnyVersion
```

---

## 3. Git Configuration

```powershell
git config --global user.name "Matt Farncombe"
git config --global user.email "matt@farncombe.co"
gh auth login
```

---

## 4. Clone and First Build

```powershell
git clone https://github.com/C0mbed/astroos.git
cd astroos
dotnet restore src/AstroOS.UI/AstroOS.UI.csproj -p:Platform=x64
dotnet build src/AstroOS.UI/AstroOS.UI.csproj -p:Platform=x64
```

Build prerequisites:
- .NET 9 SDK (installed with Visual Studio workloads above)
- Windows SDK 10.0.22621 or later (installed with VS workloads)
- `makepri.exe` at `C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\`

---

## 5. Restore Claude Code Config

```powershell
Copy-Item -Path "G:\My Drive\DevBackup\.claude" -Destination "C:\Users\$env:USERNAME\.claude" -Recurse -Force
```

---

## 6. Launch the App

```powershell
powershell -ExecutionPolicy Bypass -Command "Start-Process 'src\AstroOS.UI\bin\x64\Debug\net9.0-windows10.0.22000.0\win-x64\AstroOS.UI.exe'"
```

> **Note:** The app requires the Windows App Runtime 1.8 to be installed and compatible
> with the OS build. On Insider Preview builds the runtime may crash — install a stable
> Windows 11 release build (26100 series or later) for reliable operation.
