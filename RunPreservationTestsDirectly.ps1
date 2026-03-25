# PowerShell script to run preservation tests directly
$ErrorActionPreference = "Stop"

Write-Host "=== Preservation Property Tests Runner ===" -ForegroundColor Cyan
Write-Host "Running tests on UNFIXED code to establish baseline behavior" -ForegroundColor Cyan
Write-Host ""

# Build the test project
Write-Host "Building test project..." -ForegroundColor Yellow
dotnet build CalibrationTuning.Tests/CalibrationTuning.Tests.csproj --configuration Debug
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Loading test assembly..." -ForegroundColor Yellow

# Load the test assembly
$testDllPath = Resolve-Path "CalibrationTuning.Tests\bin\Debug\CalibrationTuning.Tests.dll"
$assembly = [System.Reflection.Assembly]::LoadFrom($testDllPath)

# Get the test fixture type
$testFixtureType = $assembly.GetType("CalibrationTuning.Tests.Unit.MainFormLoggingTabPreservationTests")

if ($null -eq $testFixtureType) {
    Write-Host "Could not find test fixture type!" -ForegroundColor Red
    exit 1
}

# Create test fixture instance
$testFixture = [Activator]::CreateInstance($testFixtureType)

# Get test methods
$testMethods = @(
    "Preservation_StartTuning_ClearsDataGridViewAndAddsSettingRow",
    "Preservation_TuningIterations_AddDataRowsWithAllColumns",
    "Preservation_StopTuning_AddsSettingRow",
    "Preservation_DataGridView_AutoScrollsToLatestEntry",
    "Preservation_StatusPanel_DisplaysTuningStatusAndMeasurements",
    "Preservation_ConnectionTab_FunctionsCorrectly",
    "Preservation_ChartTab_FunctionsCorrectly"
)

$passedTests = 0
$failedTests = 0

Write-Host ""
Write-Host "Running tests..." -ForegroundColor Yellow
Write-Host ""

foreach ($testMethodName in $testMethods) {
    Write-Host "Running $testMethodName... " -NoNewline
    
    try {
        # Call SetUp
        $setupMethod = $testFixtureType.GetMethod("SetUp")
        $setupMethod.Invoke($testFixture, $null)
        
        # Call test method
        $testMethod = $testFixtureType.GetMethod($testMethodName)
        $testMethod.Invoke($testFixture, $null)
        
        Write-Host "PASSED" -ForegroundColor Green
        $passedTests++
    }
    catch {
        Write-Host "FAILED" -ForegroundColor Red
        Write-Host "  Error: $($_.Exception.InnerException.Message)" -ForegroundColor Red
        $failedTests++
    }
}

Write-Host ""
Write-Host "=== Test Summary ===" -ForegroundColor Cyan
Write-Host "Passed: $passedTests" -ForegroundColor Green
if ($failedTests -gt 0) {
    Write-Host "Failed: $failedTests" -ForegroundColor Red
}
Write-Host "Total: $($passedTests + $failedTests)"
Write-Host ""

if ($passedTests -eq 7 -and $failedTests -eq 0) {
    Write-Host "SUCCESS: All preservation tests PASSED on unfixed code." -ForegroundColor Green
    Write-Host "Baseline behavior has been documented and will be preserved after the fix." -ForegroundColor Green
}
else {
    Write-Host "WARNING: Some preservation tests failed on unfixed code." -ForegroundColor Yellow
    Write-Host "This may indicate issues with the test implementation or unexpected baseline behavior." -ForegroundColor Yellow
}

Write-Host ""
