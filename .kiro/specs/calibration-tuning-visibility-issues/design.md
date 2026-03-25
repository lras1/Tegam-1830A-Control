# Calibration Tuning Visibility Issues Bugfix Design

## Overview

This bugfix addresses three visibility and error reporting issues in the CalibrationTuning application. The bugs prevent users from seeing the DataGridView in StatusPanel, the Chart in ChartPanel, and specific error messages when tuning fails. The fix ensures all UI controls are properly visible and error messages are displayed to users via MessageBox dialogs.

## Glossary

- **Bug_Condition (C)**: The condition that triggers the bug - when UI controls are not visible or error messages are not displayed
- **Property (P)**: The desired behavior - UI controls should be visible and error messages should be shown to users
- **Preservation**: Existing tuning functionality, data logging, and measurement behavior that must remain unchanged
- **StatusPanel**: User control in TuningPanel that displays tuning status and measurement history in a DataGridView
- **ChartPanel**: User control in MainForm's Chart tab that displays real-time power measurements
- **TuningController.ErrorOccurred**: Event raised when errors occur during device operations or tuning
- **TuningPanel.HandleError**: Method that processes ErrorOccurred events but currently doesn't display messages to users

## Bug Details

### Bug Condition

The bugs manifest in three distinct scenarios:

**Bug 1: DataGridView Not Visible**
The DataGridView in StatusPanel is created and configured but not visible to users. The control exists in memory with proper columns but has incorrect size or visibility settings.

**Bug 2: Chart Not Visible**
The Chart control in ChartPanel is created with proper configuration (axes, series, legend) but not visible to users. The control exists but has incorrect size or visibility settings.

**Bug 3: Error Messages Not Displayed**
When TuningController raises ErrorOccurred events, the TuningPanel.HandleError method receives them but doesn't display the error message to users. Users only see "Status: Error" without knowing what went wrong.

**Formal Specification:**
```
FUNCTION isBugCondition(input)
  INPUT: input of type UIState or ErrorEvent
  OUTPUT: boolean
  
  RETURN (input.controlType == "DataGridView" AND input.parent == "StatusPanel" AND NOT input.isVisible)
         OR (input.controlType == "Chart" AND input.parent == "ChartPanel" AND NOT input.isVisible)
         OR (input.eventType == "ErrorOccurred" AND NOT input.messageDisplayedToUser)
END FUNCTION
```

### Examples

- **Bug 1 Example**: User runs application and navigates to Tuning tab. StatusPanel is visible but DataGridView shows no content. Expected: DataGridView displays columns (Type, Timestamp, Iteration, Frequency, Voltage, Power_dBm, Status).

- **Bug 2 Example**: User navigates to Chart tab. Tab is visible but appears empty. Expected: Chart displays with axes labeled "Iteration" (X) and "Power (dBm)" (Y), grid lines, and legend.

- **Bug 3 Example**: User clicks Start Tuning with invalid SensorId. Status changes to "Error" but no message explains the problem. Expected: MessageBox displays "Invalid SensorId. Must be 1 or 2."

- **Edge Case**: User clicks Start Tuning while devices are disconnected. Status shows "Error" without explanation. Expected: MessageBox displays "Both devices must be connected before starting tuning".

## Expected Behavior

### Preservation Requirements

**Unchanged Behaviors:**
- Tuning process execution with valid parameters and connected devices must continue to work
- DataGridView and Chart updates during tuning must continue to function
- Manual Measure dialog must continue to display frequency, voltage, and power values
- CSV logging of setting and data rows must continue to work correctly
- All existing event subscriptions and data flow must remain intact

**Scope:**
All inputs that do NOT involve the three specific bug conditions should be completely unaffected by this fix. This includes:
- Normal tuning workflow (connect, configure, start, measure, stop)
- Data logging to CSV files
- Configuration persistence (save/load)
- Device connection and disconnection
- Manual measurement functionality
- State transitions and progress updates

## Hypothesized Root Cause

Based on the bug description and code analysis, the most likely issues are:

1. **DataGridView Size Issue**: The DataGridView in StatusPanel may have incorrect Size property
   - Current code: `Size = new Size(520, 265)`
   - The control may need explicit visibility or different dimensions
   - The parent GroupBox may be too small to show the DataGridView

2. **Chart Size Issue**: The Chart in ChartPanel may have incorrect Size property
   - Current code: `Size = new Size(760, 540)`
   - The control may need explicit visibility or different dimensions
   - The chart may not be properly added to the Controls collection

3. **Missing Error Display**: TuningPanel.HandleError receives error messages but doesn't show them
   - Current code: Method only calls `UpdateControlStates(isTuning: false)`
   - No MessageBox.Show call to display the error message to users
   - Users see state change to "Error" but not the specific error message

4. **InitializeComponent Missing**: StatusPanel and ChartPanel may be missing InitializeComponent calls
   - Both controls have `InitializeComponent()` calls but the Designer files may not exist
   - This could prevent proper control initialization

## Correctness Properties

Property 1: Bug Condition - UI Controls Visible

_For any_ UI state where the application is running and the user navigates to the Tuning tab or Chart tab, the fixed code SHALL ensure that the DataGridView in StatusPanel and the Chart in ChartPanel are visible with proper dimensions and content.

**Validates: Requirements 2.1, 2.2**

Property 2: Bug Condition - Error Messages Displayed

_For any_ error event raised by TuningController.ErrorOccurred, the fixed TuningPanel.HandleError method SHALL display the error message to the user via MessageBox, ensuring users understand what went wrong.

**Validates: Requirements 2.3**

Property 3: Preservation - Tuning Functionality

_For any_ tuning operation with valid parameters and connected devices, the fixed code SHALL produce exactly the same behavior as the original code, preserving all tuning, measurement, and data logging functionality.

**Validates: Requirements 3.1, 3.2, 3.3, 3.4**

## Fix Implementation

### Changes Required

Assuming our root cause analysis is correct:

**File**: `Tegam.1830A/CalibrationTuning/UserControls/StatusPanel.cs`

**Function**: `InitializeControls`

**Specific Changes**:
1. **Fix DataGridView Size**: Increase DataGridView height to ensure visibility
   - Change `Size = new Size(520, 265)` to `Size = new Size(520, 260)` or verify parent GroupBox size
   - Ensure DataGridView is properly anchored and visible

2. **Verify GroupBox Size**: Ensure _dataGridGroup has sufficient height
   - Current: `Size = new Size(540, 300)`
   - DataGridView needs space for header + rows (25 header + 265 content = 290 minimum)

3. **Add Explicit Visibility**: Set `Visible = true` on DataGridView if needed

**File**: `Tegam.1830A/CalibrationTuning/UserControls/ChartPanel.cs`

**Function**: `InitializeChart`

**Specific Changes**:
1. **Fix Chart Size**: Verify Chart dimensions are appropriate for parent container
   - Current: `Size = new Size(760, 540)`
   - Ensure Chart is properly anchored and visible

2. **Add Explicit Visibility**: Set `Visible = true` on Chart if needed

3. **Verify Controls.Add**: Ensure Chart is added to UserControl's Controls collection
   - Current code has `this.Controls.Add(_chart)` which appears correct

**File**: `Tegam.1830A/CalibrationTuning/UserControls/TuningPanel.cs`

**Function**: `HandleError`

**Specific Changes**:
1. **Display Error Message**: Add MessageBox.Show call to display error to user
   ```csharp
   private void HandleError(string errorMessage)
   {
       // Display error message to user
       MessageBox.Show(
           errorMessage,
           "Tuning Error",
           MessageBoxButtons.OK,
           MessageBoxIcon.Error);
       
       // Re-enable controls after error
       UpdateControlStates(isTuning: false);
   }
   ```

2. **Thread Safety**: Ensure MessageBox is shown on UI thread (already handled by InvokeRequired check in TuningController_ErrorOccurred)

## Testing Strategy

### Validation Approach

The testing strategy follows a two-phase approach: first, surface counterexamples that demonstrate the bugs on unfixed code, then verify the fixes work correctly and preserve existing behavior.

### Exploratory Bug Condition Checking

**Goal**: Surface counterexamples that demonstrate the bugs BEFORE implementing the fix. Confirm or refute the root cause analysis. If we refute, we will need to re-hypothesize.

**Test Plan**: Run the application and manually verify the three bug conditions. Document the exact behavior observed.

**Test Cases**:
1. **DataGridView Visibility Test**: Launch app, navigate to Tuning tab, observe StatusPanel (will fail on unfixed code - DataGridView not visible)
2. **Chart Visibility Test**: Launch app, navigate to Chart tab, observe ChartPanel (will fail on unfixed code - Chart not visible)
3. **Error Message Test**: Launch app, click Start Tuning without connecting devices, observe error handling (will fail on unfixed code - no MessageBox shown)
4. **Invalid SensorId Test**: Modify code to pass invalid SensorId, start tuning, observe error (will fail on unfixed code - no MessageBox shown)

**Expected Counterexamples**:
- DataGridView exists in memory but is not rendered on screen
- Chart exists in memory but is not rendered on screen
- Error messages are logged/handled internally but not displayed to users
- Possible causes: incorrect Size properties, missing Visible=true, missing MessageBox.Show call

### Fix Checking

**Goal**: Verify that for all inputs where the bug condition holds, the fixed code produces the expected behavior.

**Pseudocode:**
```
FOR ALL uiState WHERE isBugCondition(uiState) DO
  result := renderUI_fixed(uiState)
  ASSERT controlsAreVisible(result) AND errorMessagesDisplayed(result)
END FOR
```

**Test Cases**:
1. **DataGridView Visible After Fix**: Launch app, navigate to Tuning tab, verify DataGridView shows columns
2. **Chart Visible After Fix**: Launch app, navigate to Chart tab, verify Chart shows axes and legend
3. **Error Message Displayed After Fix**: Click Start Tuning without devices, verify MessageBox appears with error text
4. **Multiple Errors Displayed**: Trigger multiple error conditions, verify each shows appropriate MessageBox

### Preservation Checking

**Goal**: Verify that for all inputs where the bug condition does NOT hold, the fixed code produces the same result as the original code.

**Pseudocode:**
```
FOR ALL input WHERE NOT isBugCondition(input) DO
  ASSERT originalBehavior(input) = fixedBehavior(input)
END FOR
```

**Testing Approach**: Property-based testing is recommended for preservation checking because:
- It generates many test cases automatically across the input domain
- It catches edge cases that manual unit tests might miss
- It provides strong guarantees that behavior is unchanged for all non-buggy inputs

**Test Plan**: Observe behavior on UNFIXED code first for normal tuning operations, then write property-based tests capturing that behavior.

**Test Cases**:
1. **Normal Tuning Preservation**: Connect devices, start tuning with valid parameters, verify tuning executes normally and DataGridView/Chart update with data
2. **Manual Measure Preservation**: Connect devices, click Manual Measure, verify dialog shows frequency/voltage/power
3. **Data Logging Preservation**: Perform tuning session, verify CSV file contains correct setting and data rows
4. **Configuration Preservation**: Set parameters, close app, reopen, verify parameters are restored

### Unit Tests

- Test StatusPanel initialization and verify DataGridView is visible
- Test ChartPanel initialization and verify Chart is visible
- Test TuningPanel.HandleError displays MessageBox with correct error text
- Test edge cases (empty error message, very long error message)

### Property-Based Tests

- Generate random UI navigation sequences and verify controls remain visible
- Generate random error conditions and verify MessageBox is always displayed
- Generate random tuning parameter combinations and verify preservation of tuning behavior

### Integration Tests

- Test full application flow: launch, connect, configure, start tuning, observe UI updates
- Test error scenarios: start tuning without devices, invalid parameters, device disconnection during tuning
- Test that DataGridView and Chart update correctly during tuning sessions
- Test that CSV logging continues to work after UI fixes
