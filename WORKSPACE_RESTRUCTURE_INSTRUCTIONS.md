# CalibrationTuning Workspace Restructure - Manual Steps Required

## ✅ Completed Steps

1. ✅ Created directory structure at `C:\Users\lras1\source\repos\GitHub\CalibrationTuning\`
2. ✅ Copied `CalibrationTuning\` project folder
3. ✅ Copied `CalibrationTuning.Tests\` project folder
4. ✅ Copied `CalibrationTuning.sln` solution file
5. ✅ Copied `.kiro\specs\calibration-tuning-app\` spec files
6. ✅ Copied `packages\` folder for self-contained NuGet packages

## 🔧 Manual Steps Required

### Step 1: Add CalibrationTuning to VS Code Workspace

**Option A: Add to existing multi-root workspace**
1. In VS Code, go to File → Add Folder to Workspace
2. Navigate to `C:\Users\lras1\source\repos\GitHub\CalibrationTuning`
3. Click "Add"

**Option B: Open as new workspace**
1. In VS Code, go to File → Open Folder
2. Navigate to `C:\Users\lras1\source\repos\GitHub\CalibrationTuning`
3. Click "Select Folder"

### Step 2: Update CalibrationTuning.csproj Project References

Open `C:\Users\lras1\source\repos\GitHub\CalibrationTuning\CalibrationTuning\CalibrationTuning.csproj`

Find this section:
```xml
<ItemGroup>
  <ProjectReference Include="..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\Tegam.1830A.DeviceLibrary.csproj">
    <Project>{A1B2C3D4-E5F6-47A8-B9C0-D1E2F3A4B5C6}</Project>
    <Name>Tegam.1830A.DeviceLibrary</Name>
  </ProjectReference>
  <ProjectReference Include="..\..\Tegam.1830A\Siglent.SDG6052X.DeviceLibrary\Siglent.SDG6052X.DeviceLibrary.csproj">
    <Project>{A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D}</Project>
    <Name>Siglent.SDG6052X.DeviceLibrary</Name>
  </ProjectReference>
</ItemGroup>
```

Replace with:
```xml
<ItemGroup>
  <ProjectReference Include="..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\Tegam.1830A.DeviceLibrary.csproj">
    <Project>{A1B2C3D4-E5F6-47A8-B9C0-D1E2F3A4B5C6}</Project>
    <Name>Tegam.1830A.DeviceLibrary</Name>
  </ProjectReference>
  <ProjectReference Include="..\..\Siglent.SDG6052X\Siglent.SDG6052X.DeviceLibrary\Siglent.SDG6052X.DeviceLibrary.csproj">
    <Project>{A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D}</Project>
    <Name>Siglent.SDG6052X.DeviceLibrary</Name>
  </ProjectReference>
</ItemGroup>
```

**Path Explanation:**
- From: `C:\Users\lras1\source\repos\GitHub\CalibrationTuning\CalibrationTuning\CalibrationTuning.csproj`
- To Tegam: `..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\` (up 2 levels to repos, then into Tegam.1830A)
- To Siglent: `..\..\Siglent.SDG6052X\Siglent.SDG6052X.DeviceLibrary\` (up 2 levels to GitHub, then into Siglent.SDG6052X)

### Step 3: Update CalibrationTuning.Tests.csproj Project References

Open `C:\Users\lras1\source\repos\GitHub\CalibrationTuning\CalibrationTuning.Tests\CalibrationTuning.Tests.csproj`

Find this section:
```xml
<ItemGroup>
  <ProjectReference Include="..\CalibrationTuning\CalibrationTuning.csproj">
    <Project>{E6F7A8B9-C0D1-4E2F-A3B4-C5D6E7F8A9B0}</Project>
    <Name>CalibrationTuning</Name>
  </ProjectReference>
  <ProjectReference Include="..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\Tegam.1830A.DeviceLibrary.csproj">
    <Project>{A1B2C3D4-E5F6-47A8-B9C0-D1E2F3A4B5C6}</Project>
    <Name>Tegam.1830A.DeviceLibrary</Name>
  </ProjectReference>
  <ProjectReference Include="..\..\Tegam.1830A\Siglent.SDG6052X.DeviceLibrary\Siglent.SDG6052X.DeviceLibrary.csproj">
    <Project>{A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D}</Project>
    <Name>Siglent.SDG6052X.DeviceLibrary</Name>
  </ProjectReference>
</ItemGroup>
```

Replace with:
```xml
<ItemGroup>
  <ProjectReference Include="..\CalibrationTuning\CalibrationTuning.csproj">
    <Project>{E6F7A8B9-C0D1-4E2F-A3B4-C5D6E7F8A9B0}</Project>
    <Name>CalibrationTuning</Name>
  </ProjectReference>
  <ProjectReference Include="..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\Tegam.1830A.DeviceLibrary.csproj">
    <Project>{A1B2C3D4-E5F6-47A8-B9C0-D1E2F3A4B5C6}</Project>
    <Name>Tegam.1830A.DeviceLibrary</Name>
  </ProjectReference>
  <ProjectReference Include="..\..\Siglent.SDG6052X\Siglent.SDG6052X.DeviceLibrary\Siglent.SDG6052X.DeviceLibrary.csproj">
    <Project>{A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D}</Project>
    <Name>Siglent.SDG6052X.DeviceLibrary</Name>
  </ProjectReference>
</ItemGroup>
```

### Step 4: Test Build

Open PowerShell or Command Prompt:
```powershell
cd C:\Users\lras1\source\repos\GitHub\CalibrationTuning
msbuild CalibrationTuning.sln /t:Rebuild /p:Configuration=Debug
```

Or in Visual Studio 2022:
1. Open `C:\Users\lras1\source\repos\GitHub\CalibrationTuning\CalibrationTuning.sln`
2. Build → Rebuild Solution

### Step 5: Initialize Git Repository

```powershell
cd C:\Users\lras1\source\repos\GitHub\CalibrationTuning
git init
git add .
git commit -m "Initial commit: CalibrationTuning application"
```

### Step 6: Create GitHub Repository and Push

**Option A: Create via GitHub CLI**
```powershell
gh repo create lras1/CalibrationTuning --public --source=. --remote=origin
git push -u origin main
```

**Option B: Create via GitHub Web UI**
1. Go to https://github.com/new
2. Repository name: `CalibrationTuning`
3. Click "Create repository"
4. Then run:
```powershell
git remote add origin https://github.com/lras1/CalibrationTuning.git
git branch -M main
git push -u origin main
```

### Step 7: Create .gitignore

Create `C:\Users\lras1\source\repos\GitHub\CalibrationTuning\.gitignore`:
```gitignore
## Ignore Visual Studio temporary files, build results, and
## files generated by popular Visual Studio add-ons.

# User-specific files
*.suo
*.user
*.userosscache
*.sln.docstates

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Bb]in/
[Oo]bj/
[Ll]og/

# Visual Studio cache/options directory
.vs/

# NuGet Packages
*.nupkg
packages/
!packages/repositories.config

# Visual Studio profiler
*.psess
*.vsp
*.vspx
*.sap

# ReSharper
_ReSharper*/
*.[Rr]e[Ss]harper
*.DotSettings.user

# NCrunch
_NCrunch_*
.*crunch*.local.xml
nCrunchTemp_*

# Others
*.Cache
ClientBin/
[Ss]tyle[Cc]op.*
~$*
*~
*.dbmdl
*.dbproj.schemaview
*.jfm
*.pfx
*.publishsettings
node_modules/
orleans.codegen.cs

# Backup & report files
_UpgradeReport_Files/
Backup*/
UpgradeLog*.XML
UpgradeLog*.htm
```

### Step 8: Clean Up Old Location (AFTER VERIFICATION)

**⚠️ ONLY DO THIS AFTER CONFIRMING THE NEW LOCATION BUILDS SUCCESSFULLY**

```powershell
# Remove old folders from Tegam.1830A workspace
Remove-Item -Path "C:\Users\lras1\source\repos\Tegam.1830A\CalibrationTuning" -Recurse -Force
Remove-Item -Path "C:\Users\lras1\source\repos\Tegam.1830A\CalibrationTuning.Tests" -Recurse -Force
Remove-Item -Path "C:\Users\lras1\source\repos\Tegam.1830A\CalibrationTuning.sln" -Force
Remove-Item -Path "C:\Users\lras1\source\repos\Tegam.1830A\.kiro\specs\calibration-tuning-app" -Recurse -Force
```

## 📁 Final Directory Structure

```
C:\Users\lras1\source\repos\
├── Tegam.1830A\
│   ├── Tegam.1830A.DeviceLibrary\
│   ├── Tegam.1830A.WinFormsUI\
│   ├── Tegam.1830A.Tests\
│   ├── Siglent.SDG6052X.DeviceLibrary\      (legacy location)
│   ├── Siglent.SDG6052X.WinFormsUI\         (legacy location)
│   ├── Siglent.SDG6052X.Tests\              (legacy location)
│   └── packages\
└── GitHub\
    ├── Siglent.SDG6052X\                     (working copy)
    │   ├── Siglent.SDG6052X.DeviceLibrary\
    │   ├── Siglent.SDG6052X.WinFormsUI\
    │   └── Siglent.SDG6052X.Tests\
    └── CalibrationTuning\                    ← NEW WORKSPACE
        ├── CalibrationTuning\
        ├── CalibrationTuning.Tests\
        ├── CalibrationTuning.sln
        ├── packages\
        └── .kiro\specs\calibration-tuning-app\
```

## 🎯 Quick Reference: Relative Paths

From `CalibrationTuning\CalibrationTuning\CalibrationTuning.csproj`:
- To Tegam.1830A.DeviceLibrary: `..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\`
- To Siglent.SDG6052X.DeviceLibrary: `..\..\Siglent.SDG6052X\Siglent.SDG6052X.DeviceLibrary\`

From `CalibrationTuning\CalibrationTuning.Tests\CalibrationTuning.Tests.csproj`:
- To CalibrationTuning: `..\CalibrationTuning\`
- To Tegam.1830A.DeviceLibrary: `..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\`
- To Siglent.SDG6052X.DeviceLibrary: `..\..\Siglent.SDG6052X\Siglent.SDG6052X.DeviceLibrary\`

## ✅ Verification Checklist

- [ ] CalibrationTuning folder added to VS Code workspace
- [ ] CalibrationTuning.csproj project references updated
- [ ] CalibrationTuning.Tests.csproj project references updated
- [ ] Solution builds successfully in new location
- [ ] Git repository initialized
- [ ] .gitignore created
- [ ] Code pushed to GitHub
- [ ] Old location cleaned up (after verification)
