# CalibrationTuning Workspace Restructure Plan

## Current Structure
```
C:\Users\lras1\source\repos\
├── Tegam.1830A\
│   ├── CalibrationTuning\              ← MOVE THIS
│   ├── CalibrationTuning.Tests\        ← MOVE THIS
│   ├── CalibrationTuning.sln           ← MOVE THIS
│   ├── .kiro\specs\calibration-tuning-app\  ← MOVE THIS
│   └── Tegam.1830A.DeviceLibrary\
└── GitHub\
    └── Siglent.SDG6052X\
        └── Siglent.SDG6052X.DeviceLibrary\
```

## Target Structure
```
C:\Users\lras1\source\repos\
├── Tegam.1830A\
│   └── Tegam.1830A.DeviceLibrary\
└── GitHub\
    ├── Siglent.SDG6052X\
    │   └── Siglent.SDG6052X.DeviceLibrary\
    └── CalibrationTuning\              ← NEW LOCATION
        ├── CalibrationTuning\
        ├── CalibrationTuning.Tests\
        ├── CalibrationTuning.sln
        └── .kiro\specs\calibration-tuning-app\
```

## Steps to Execute

### 1. Create Target Directory Structure
- Create `C:\Users\lras1\source\repos\GitHub\CalibrationTuning\`
- Create `.kiro\specs\` subdirectory

### 2. Copy Files (preserving structure)
- Copy `Tegam.1830A\CalibrationTuning\` → `GitHub\CalibrationTuning\CalibrationTuning\`
- Copy `Tegam.1830A\CalibrationTuning.Tests\` → `GitHub\CalibrationTuning\CalibrationTuning.Tests\`
- Copy `Tegam.1830A\CalibrationTuning.sln` → `GitHub\CalibrationTuning\CalibrationTuning.sln`
- Copy `Tegam.1830A\.kiro\specs\calibration-tuning-app\` → `GitHub\CalibrationTuning\.kiro\specs\calibration-tuning-app\`

### 3. Update Project References in CalibrationTuning.csproj
**Current (INCORRECT):**
```xml
<ProjectReference Include="..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\Tegam.1830A.DeviceLibrary.csproj">
<ProjectReference Include="..\..\Tegam.1830A\Siglent.SDG6052X.DeviceLibrary\Siglent.SDG6052X.DeviceLibrary.csproj">
```

**Target (CORRECT):**
```xml
<ProjectReference Include="..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\Tegam.1830A.DeviceLibrary.csproj">
<ProjectReference Include="..\Siglent.SDG6052X\Siglent.SDG6052X.DeviceLibrary\Siglent.SDG6052X.DeviceLibrary.csproj">
```

### 4. Update Project References in CalibrationTuning.Tests.csproj
**Current (INCORRECT):**
```xml
<ProjectReference Include="..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\Tegam.1830A.DeviceLibrary.csproj">
<ProjectReference Include="..\..\Tegam.1830A\Siglent.SDG6052X.DeviceLibrary\Siglent.SDG6052X.DeviceLibrary.csproj">
```

**Target (CORRECT):**
```xml
<ProjectReference Include="..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\Tegam.1830A.DeviceLibrary.csproj">
<ProjectReference Include="..\Siglent.SDG6052X\Siglent.SDG6052X.DeviceLibrary\Siglent.SDG6052X.DeviceLibrary.csproj">
```

### 5. Update packages.config HintPath references
Update all NuGet package HintPaths from `..\packages\` to `..\..\Tegam.1830A\packages\` OR copy packages folder to new location.

### 6. Create .gitignore for new workspace
Copy relevant entries from Tegam.1830A\.gitignore

### 7. Initialize Git Repository
```bash
cd C:\Users\lras1\source\repos\GitHub\CalibrationTuning
git init
git add .
git commit -m "Initial commit: CalibrationTuning application"
git remote add origin https://github.com/lras1/CalibrationTuning.git
git push -u origin main
```

### 8. Test Build
```bash
cd C:\Users\lras1\source\repos\GitHub\CalibrationTuning
msbuild CalibrationTuning.sln /t:Rebuild /p:Configuration=Debug
```

### 9. Clean Up Old Location (AFTER VERIFICATION)
- Delete `Tegam.1830A\CalibrationTuning\`
- Delete `Tegam.1830A\CalibrationTuning.Tests\`
- Delete `Tegam.1830A\CalibrationTuning.sln`
- Delete `Tegam.1830A\.kiro\specs\calibration-tuning-app\`

## Path Reference Summary

### From CalibrationTuning.csproj perspective:
- **Current location**: `C:\Users\lras1\source\repos\Tegam.1830A\CalibrationTuning\CalibrationTuning.csproj`
- **New location**: `C:\Users\lras1\source\repos\GitHub\CalibrationTuning\CalibrationTuning\CalibrationTuning.csproj`

### Relative paths from new location:
- To Tegam.1830A.DeviceLibrary: `..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\`
- To Siglent.SDG6052X.DeviceLibrary: `..\Siglent.SDG6052X\Siglent.SDG6052X.DeviceLibrary\`
- To packages (if shared): `..\..\Tegam.1830A\packages\`

## Notes
- The current project references are ALREADY INCORRECT (they go up 2 levels then back into Tegam.1830A, which doesn't make sense from current location)
- After restructuring, the paths will be correct for the new sibling workspace structure
- Consider whether to share packages folder or have separate packages per workspace
