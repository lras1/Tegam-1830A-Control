# PowerShell script to run the bug condition test
$ErrorActionPreference = "Continue"

Write-Host "Building test project..." -ForegroundColor Cyan
dotnet build CalibrationTuning.Tests/CalibrationTuning.Tests.csproj

Write-Host "`nRunning bug condition exploration test..." -ForegroundColor Cyan
dotnet test CalibrationTuning.Tests/CalibrationTuning.Tests.csproj `
    --no-build `
    --filter "FullyQualifiedName~MainFormLoggingTabBugConditionTests" `
    --logger "console;verbosity=detailed"

Write-Host "`nTest execution complete." -ForegroundColor Green
