# PowerShell script to run the preservation tests
$ErrorActionPreference = "Continue"

Write-Host "Building test project..." -ForegroundColor Cyan
dotnet build CalibrationTuning.Tests/CalibrationTuning.Tests.csproj

Write-Host "`nRunning preservation property tests..." -ForegroundColor Cyan
dotnet test CalibrationTuning.Tests/CalibrationTuning.Tests.csproj `
    --no-build `
    --filter "FullyQualifiedName~MainFormLoggingTabPreservationTests" `
    --logger "console;verbosity=detailed"

Write-Host "`nTest execution complete." -ForegroundColor Green
