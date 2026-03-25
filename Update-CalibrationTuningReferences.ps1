# PowerShell script to update project references in CalibrationTuning workspace
# Run this script from any location after copying files to new workspace

$calibrationTuningRoot = "C:\Users\lras1\source\repos\GitHub\CalibrationTuning"

Write-Host "Updating CalibrationTuning project references..." -ForegroundColor Cyan

# Update CalibrationTuning.csproj
$csprojPath = Join-Path $calibrationTuningRoot "CalibrationTuning\CalibrationTuning.csproj"
if (Test-Path $csprojPath) {
    Write-Host "Updating $csprojPath" -ForegroundColor Yellow
    
    $content = Get-Content $csprojPath -Raw
    
    # Update Tegam.1830A.DeviceLibrary reference
    $content = $content -replace 'Include="\.\.\\\.\.\\Tegam\.1830A\\Tegam\.1830A\.DeviceLibrary\\Tegam\.1830A\.DeviceLibrary\.csproj"', 'Include="..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\Tegam.1830A.DeviceLibrary.csproj"'
    
    # Update Siglent.SDG6052X.DeviceLibrary reference
    $content = $content -replace 'Include="\.\.\\\.\.\\Tegam\.1830A\\Siglent\.SDG6052X\.DeviceLibrary\\Siglent\.SDG6052X\.DeviceLibrary\.csproj"', 'Include="..\..\Siglent.SDG6052X\Siglent.SDG6052X.DeviceLibrary\Siglent.SDG6052X.DeviceLibrary.csproj"'
    
    Set-Content $csprojPath -Value $content -NoNewline
    Write-Host "✓ Updated CalibrationTuning.csproj" -ForegroundColor Green
} else {
    Write-Host "✗ CalibrationTuning.csproj not found at $csprojPath" -ForegroundColor Red
}

# Update CalibrationTuning.Tests.csproj
$testCsprojPath = Join-Path $calibrationTuningRoot "CalibrationTuning.Tests\CalibrationTuning.Tests.csproj"
if (Test-Path $testCsprojPath) {
    Write-Host "Updating $testCsprojPath" -ForegroundColor Yellow
    
    $content = Get-Content $testCsprojPath -Raw
    
    # Update Tegam.1830A.DeviceLibrary reference
    $content = $content -replace 'Include="\.\.\\\.\.\\Tegam\.1830A\\Tegam\.1830A\.DeviceLibrary\\Tegam\.1830A\.DeviceLibrary\.csproj"', 'Include="..\..\Tegam.1830A\Tegam.1830A.DeviceLibrary\Tegam.1830A.DeviceLibrary.csproj"'
    
    # Update Siglent.SDG6052X.DeviceLibrary reference
    $content = $content -replace 'Include="\.\.\\\.\.\\Tegam\.1830A\\Siglent\.SDG6052X\.DeviceLibrary\\Siglent\.SDG6052X\.DeviceLibrary\.csproj"', 'Include="..\Siglent.SDG6052X\Siglent.SDG6052X.DeviceLibrary\Siglent.SDG6052X.DeviceLibrary.csproj"'
    
    Set-Content $testCsprojPath -Value $content -NoNewline
    Write-Host "✓ Updated CalibrationTuning.Tests.csproj" -ForegroundColor Green
} else {
    Write-Host "✗ CalibrationTuning.Tests.csproj not found at $testCsprojPath" -ForegroundColor Red
}

Write-Host ""
Write-Host "Project reference updates complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Add C:\Users\lras1\source\repos\GitHub\CalibrationTuning to VS Code workspace"
Write-Host "2. Test build: msbuild CalibrationTuning.sln /t:Rebuild /p:Configuration=Debug"
Write-Host "3. Initialize git: git init && git add . && git commit -m 'Initial commit'"
Write-Host "4. Push to GitHub: git remote add origin https://github.com/lras1/CalibrationTuning.git && git push -u origin main"
