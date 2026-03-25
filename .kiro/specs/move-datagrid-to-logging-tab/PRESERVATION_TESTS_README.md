# Preservation Property Tests Documentation

## Overview

This document describes the preservation property tests created for Task 2 of the move-datagrid-to-logging-tab bugfix spec. These tests observe and document the baseline behavior on UNFIXED code that must be preserved after implementing the fix.

## Test File Location

`Tegam.1830A/CalibrationTuning.Tests/Unit/MainFormLoggingTabPreservationTests.cs`

## Purpose

These tests establish the baseline behavior of the DataGridView logging functionality before the fix is implemented. They should:
- **PASS on UNFIXED code** - confirming the baseline behavior
- **PASS after the fix** - confirming no regressions were introduced

## Test Coverage

The preservation tests cover the following behaviors (Requirements 3.1-3.8):

### 1. Start Tuning Preservation (Requirements 3.2, 3.5)
**Test:** `Preservation_StartTuning_ClearsDataGridViewAndAddsSettingRow`

**Validates:**
- Clicking Start Tuning clears the DataGridView
- A "Start Tuning" setting row is added with Type="setting" and Status="Start Tuning"

### 2. Tuning Iterations Preservation (Requirements 3.1, 3.3)
**Test:** `Preservation_TuningIterations_AddDataRowsWithAllColumns`

**Validates:**
- Tuning iterations add data rows to the DataGridView
- All columns are populated: Type, Timestamp, Iteration, Frequency, Voltage, Power_dBm, Status
- Data rows have Type="data"
- Values are formatted correctly (Frequency as integer, Voltage with 4 decimals, Power with 3 decimals)

### 3. Stop Tuning Preservation (Requirement 3.2)
**Test:** `Preservation_StopTuning_AddsSettingRow`

**Validates:**
- Clicking Stop Tuning adds a "Stop Tuning" setting row
- The setting row has Type="setting" and Status="Stop Tuning"

### 4. Auto-Scroll Preservation (Requirement 3.4)
**Test:** `Preservation_DataGridView_AutoScrollsToLatestEntry`

**Validates:**
- DataGridView auto-scrolls to the latest entry as new rows are added
- FirstDisplayedScrollingRowIndex is set to the last row index

### 5. Status Panel Display Preservation (Requirement 3.6)
**Test:** `Preservation_StatusPanel_DisplaysTuningStatusAndMeasurements`

**Validates:**
- StatusPanel displays "Tuning Status" group box
- StatusPanel displays "Current Measurements" group box
- Measurement labels exist: Iteration, Measured Power, Current Voltage, Power Error

### 6. Connection Tab Preservation (Requirement 3.7)
**Test:** `Preservation_ConnectionTab_FunctionsCorrectly`

**Validates:**
- Connection tab exists in the tab control
- ConnectionPanel exists on the Connection tab

### 7. Chart Tab Preservation (Requirement 3.8)
**Test:** `Preservation_ChartTab_FunctionsCorrectly`

**Validates:**
- Chart tab exists in the tab control
- ChartPanel exists on the Chart tab

## Running the Tests

### Option 1: Using dotnet test (Recommended)

```powershell
# Build the test project
dotnet build CalibrationTuning.Tests/CalibrationTuning.Tests.csproj --configuration Debug

# Run the preservation tests
dotnet test CalibrationTuning.Tests/CalibrationTuning.Tests.csproj `
    --no-build `
    --filter "FullyQualifiedName~MainFormLoggingTabPreservationTests" `
    --logger "console;verbosity=detailed"
```

### Option 2: Using the PowerShell script

```powershell
.\RunPreservationTests.ps1
```

### Option 3: Using Visual Studio Test Explorer

1. Open the solution in Visual Studio
2. Open Test Explorer (Test > Test Explorer)
3. Filter for "MainFormLoggingTabPreservationTests"
4. Run all tests in the fixture

## Expected Results

### On UNFIXED Code (Before implementing the fix)

All 7 tests should **PASS**, confirming:
- DataGridView exists in StatusPanel (on unfixed code)
- All logging methods work correctly
- StatusPanel displays status and measurements
- Connection and Chart tabs function correctly

### After Implementing the Fix

All 7 tests should still **PASS**, confirming:
- DataGridView now exists in LoggingPanel (on fixed code)
- All logging methods still work correctly (no regressions)
- StatusPanel still displays status and measurements
- Connection and Chart tabs still function correctly

## Test Implementation Notes

### Test Structure

Each test follows this pattern:
1. **Arrange**: Create MainForm with mocked dependencies
2. **Act**: Perform the action being tested (e.g., add data rows)
3. **Assert**: Verify the expected behavior

### Helper Methods

The test fixture includes several helper methods:
- `GetTabControl()`: Gets the TabControl from MainForm using reflection
- `FindStatusPanel()`: Finds StatusPanel on the Tuning tab
- `FindDataGridView()`: Finds DataGridView within a control
- `FindControlRecursive<T>()`: Recursively searches for a control of type T
- `FindControlByName<T>()`: Finds a control by field name using reflection
- `FindControlByText<T>()`: Finds a control by its Text property

### Mocking Strategy

The tests use Moq to mock:
- `ITuningController`: Tuning operations controller
- `IDataLoggingController`: Data logging controller
- `IConfigurationController`: Configuration management

Minimal configuration is provided to allow MainForm to load without errors.

## Adaptation for Fixed Code

After the fix is implemented, the tests will automatically adapt because:
- They search for DataGridView recursively (will find it in LoggingPanel instead of StatusPanel)
- They use the same public methods (AddSettingRow, AddDataRow, ClearDataGrid) which will be moved to LoggingPanel
- They verify behavior, not implementation details

The only difference is that on fixed code:
- DataGridView will be in LoggingPanel on the Logging tab
- StatusPanel will NOT contain DataGridView
- All logging functionality will work identically

## Troubleshooting

### Tests fail on unfixed code

If tests fail on unfixed code, this indicates:
- The baseline behavior is different than expected
- The test implementation may need adjustment
- The root cause analysis may need revision

### Tests fail after the fix

If tests fail after the fix, this indicates:
- A regression was introduced
- The fix changed behavior that should have been preserved
- The implementation needs to be corrected

## Integration with Task Workflow

These tests are part of Task 2 in the implementation plan:
- **Task 1**: Bug condition exploration test (FAILS on unfixed code, PASSES after fix)
- **Task 2**: Preservation property tests (PASSES on unfixed code, PASSES after fix) ← You are here
- **Task 3**: Implement the fix
- **Task 3.5**: Verify bug condition test now passes
- **Task 3.6**: Verify preservation tests still pass

## Validation Checklist

Before marking Task 2 complete, verify:
- [x] All 7 preservation tests are implemented
- [x] Tests compile without errors
- [x] Tests follow the observation-first methodology
- [x] Tests validate Requirements 3.1-3.8
- [x] Tests are documented with clear comments
- [x] Helper methods are provided for test maintainability
- [ ] Tests have been run on UNFIXED code and PASS (to be verified by user)
- [ ] Baseline behavior is documented

## Next Steps

1. Run the preservation tests on UNFIXED code
2. Verify all tests PASS
3. Document any unexpected behaviors
4. Proceed to Task 3 to implement the fix
5. After the fix, re-run these tests to verify no regressions
