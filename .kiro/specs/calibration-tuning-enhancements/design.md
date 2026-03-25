# Design Document: Calibration Tuning Enhancements

## Overview

This design addresses enhancements to the CalibrationTuning application, focusing on improved measurement display, bug fixes, and real-time data visualization. The enhancements build upon the existing MVC-style architecture where controllers orchestrate device interactions and UI panels display state.

The key enhancements include:

1. **Manual Measure Enhancement**: Extend the Manual Measure button to retrieve and display frequency, voltage, and power in a single operation
2. **Start Tuning Bug Fix**: Fix missing SensorId validation that causes tuning to fail
3. **Data Grid View**: Add tabular display of measurement data with setting/data row distinction
4. **Enhanced Logging**: Extend CSV logging to distinguish between user actions (settings) and measurement data
5. **Chart Visualization**: Add real-time chart displaying power measurements with target and tolerance bands
6. **Simulation Mode Compatibility**: Ensure all features work with simulated device services
7. **UI Responsiveness**: Maintain responsive UI during tuning operations

The design maintains backward compatibility with existing CSV log formats and preserves the current controller-based architecture.

## Architecture

### Current Architecture

The application follows a controller-based pattern:

- **TuningController**: Orchestrates tuning process, manages device services, executes tuning algorithm
- **DataLoggingController**: Writes measurement data to CSV files
- **ConfigurationController**: Persists application settings to JSON files
- **MainForm**: Top-level window containing tab control
- **UserControl Panels**: ConnectionPanel, TuningPanel, StatusPanel for different UI concerns

Device services (IPowerMeterService, ISignalGeneratorService) abstract hardware communication and support simulation mode through mock implementations.

### Enhanced Architecture

The enhancements extend the existing architecture without major structural changes:

```
┌─────────────────────────────────────────────────────────────┐
│                         MainForm                             │
│  ┌──────────────────────────────────────────────────────┐   │
│  │              TabControl                               │   │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐           │   │
│  │  │Connection│  │  Tuning  │  │  Status  │  [Chart]  │   │
│  │  │  Panel   │  │  Panel   │  │  Panel   │  [New]    │   │
│  │  └──────────┘  └──────────┘  └──────────┘           │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                          │
        ┌─────────────────┼─────────────────┐
        │                 │                 │
        ▼                 ▼                 ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│   Tuning     │  │DataLogging   │  │Configuration │
│  Controller  │  │  Controller  │  │  Controller  │
└──────────────┘  └──────────────┘  └──────────────┘
        │
        ├─────────────────┐
        │                 │
        ▼                 ▼
┌──────────────┐  ┌──────────────┐
│ PowerMeter   │  │SignalGen     │
│   Service    │  │  Service     │
└──────────────┘  └──────────────┘
```

**Key Changes**:

1. **StatusPanel Enhancement**: Add DataGridView control to display measurement history
2. **New ChartPanel**: New UserControl containing Chart control for visualization
3. **TuningController Enhancement**: Add GetCurrentFrequency() and GetCurrentVoltage() methods for Manual Measure
4. **DataLoggingController Enhancement**: Add LogUserAction() method to distinguish settings from data
5. **Event Flow Enhancement**: Controllers emit events that both UI panels and logging controller subscribe to

### Component Responsibilities

**TuningController** (Enhanced):
- Add methods to retrieve current frequency and voltage from signal generator
- Validate SensorId before starting tuning
- Emit events for user actions (Connect, Disconnect, Start, Stop, Manual Measure)

**DataLoggingController** (Enhanced):
- Add Type column to CSV format (first column)
- Distinguish between "setting" rows (user actions) and "data" rows (measurements)
- Log user actions with timestamp and action name

**StatusPanel** (Enhanced):
- Add DataGridView control with columns: Type, Timestamp, Iteration, Frequency, Voltage, Power_dBm, Status
- Subscribe to tuning events and populate grid in real-time
- Auto-scroll to latest entry
- Clear grid on new session

**ChartPanel** (New):
- Display Chart control with power measurements on Y-axis, iteration on X-axis
- Show target power line and tolerance bands
- Subscribe to tuning events and add data points in real-time
- Clear chart on new session
- Auto-scale Y-axis

## Components and Interfaces

### Enhanced TuningController Interface

```csharp
public interface ITuningController
{
    // Existing members...
    
    // New methods for Manual Measure enhancement
    Task<double> GetCurrentFrequencyAsync();
    Task<double> GetCurrentVoltageAsync();
    Task<ManualMeasurementResult> MeasureManualWithDetailsAsync();
    
    // New events for user action logging
    event EventHandler<UserActionEventArgs> UserActionOccurred;
}
```

### ManualMeasurementResult Model

```csharp
public class ManualMeasurementResult
{
    public bool IsValid { get; set; }
    public double FrequencyHz { get; set; }
    public double Voltage { get; set; }
    public double PowerDbm { get; set; }
    public DateTime Timestamp { get; set; }
    public string ErrorMessage { get; set; }
}
```

### UserActionEventArgs Model

```csharp
public class UserActionEventArgs : EventArgs
{
    public string ActionName { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}
```

### Enhanced DataLoggingController Interface

```csharp
public interface IDataLoggingController
{
    // Existing members...
    
    // New method for logging user actions
    void LogUserAction(string actionName, Dictionary<string, string> parameters = null);
}
```

### DataGridRow Model

```csharp
public class DataGridRow
{
    public string Type { get; set; }  // "setting" or "data"
    public DateTime Timestamp { get; set; }
    public int? Iteration { get; set; }  // Null for settings
    public double? Frequency { get; set; }
    public double? Voltage { get; set; }
    public double? PowerDbm { get; set; }
    public string Status { get; set; }
}
```

### ChartPanel UserControl

```csharp
public class ChartPanel : UserControl
{
    private Chart _chart;
    private Series _measurementSeries;
    private Series _targetLineSeries;
    private Series _upperToleranceSeries;
    private Series _lowerToleranceSeries;
    
    public ChartPanel(ITuningController tuningController);
    private void InitializeChart();
    private void AddDataPoint(TuningDataPoint dataPoint);
    private void SetTargetAndTolerance(double targetDbm, double toleranceDb);
    private void ClearChart();
}
```

## Data Models

### Enhanced TuningParameters

No changes required - SensorId already exists in the model.

### Enhanced CSV Format

The CSV format is extended with a Type column as the first column:

```
Type,Timestamp,Iteration,Frequency_Hz,Voltage,Power_dBm,Status
setting,2024-01-15 10:30:00.000,,,,,Connect
setting,2024-01-15 10:30:05.000,,,,,Start Tuning
data,2024-01-15 10:30:06.123,1,2400000000,0.5000,-12.345,Tuning
data,2024-01-15 10:30:06.456,2,2400000000,0.5500,-11.234,Tuning
data,2024-01-15 10:30:06.789,3,2400000000,0.6000,-10.123,Converged
setting,2024-01-15 10:30:07.000,,,,,Stop Tuning
```

**Setting Rows**:
- Type: "setting"
- Timestamp: When action occurred
- Iteration, Frequency, Voltage, Power_dBm: Empty
- Status: Action name (Connect, Disconnect, Start Tuning, Stop Tuning, Manual Measure)

**Data Rows**:
- Type: "data"
- Timestamp: When measurement was taken
- Iteration: Measurement iteration number
- Frequency: Signal frequency in Hz
- Voltage: Signal voltage
- Power_dBm: Measured power
- Status: Tuning state (Tuning, Converged, etc.)

### DataGridView Column Configuration

| Column Name | Data Type | Format | Width | Read-Only |
|-------------|-----------|--------|-------|-----------|
| Type | String | - | 80 | Yes |
| Timestamp | DateTime | yyyy-MM-dd HH:mm:ss.fff | 180 | Yes |
| Iteration | Int32? | N0 | 80 | Yes |
| Frequency | Double? | F0 | 120 | Yes |
| Voltage | Double? | F4 | 100 | Yes |
| Power_dBm | Double? | F3 | 100 | Yes |
| Status | String | - | 120 | Yes |

### Chart Configuration

**Chart Type**: Line chart with multiple series

**Series Configuration**:
- **Measurement Series**: Blue line, marker style Circle, line width 2
- **Target Line**: Green dashed line, line width 2
- **Upper Tolerance**: Red dashed line, line width 1
- **Lower Tolerance**: Red dashed line, line width 1

**Axis Configuration**:
- **X-Axis**: Iteration number, integer labels, grid lines enabled
- **Y-Axis**: Power (dBm), auto-scale, grid lines enabled

**Legend**: Enabled, positioned at top-right


## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Manual Measure Completeness

*For any* connected signal generator and power meter, when manual measure is invoked, the result SHALL contain valid frequency, voltage, and power values.

**Validates: Requirements 1.1, 1.2, 1.3, 1.4**

### Property 2: Manual Measure Button State

*For any* manual measure operation in progress, the manual measure button SHALL be disabled until the operation completes.

**Validates: Requirements 1.7**

### Property 3: SensorId Validation

*For any* TuningParameters object, if SensorId is missing or invalid, then starting tuning SHALL fail with a validation error.

**Validates: Requirements 2.2**

### Property 4: Sensor Selection Before Measurement

*For any* tuning session, the power meter sensor SHALL be selected before the first measurement is taken.

**Validates: Requirements 2.4**

### Property 5: Valid Parameters Start Successfully

*For any* valid TuningParameters with connected devices, starting tuning SHALL transition to Tuning state and begin measurements without error.

**Validates: Requirements 2.1, 2.5, 2.6**

### Property 6: User Actions Create Setting Rows

*For any* user action (Connect, Disconnect, Start Tuning, Stop Tuning, Manual Measure), both the DataGridView and CSV log SHALL contain a setting row with Type="setting" and the action name in Status.

**Validates: Requirements 3.3, 4.1, 4.4**

### Property 7: Measurements Create Data Rows

*For any* measurement taken during tuning, both the DataGridView and CSV log SHALL contain a data row with Type="data" and all measurement values populated.

**Validates: Requirements 3.4, 4.2, 4.5**

### Property 8: DataGridView Read-Only

*For any* DataGridView state, all cells SHALL be read-only and users SHALL NOT be able to edit values.

**Validates: Requirements 3.5**

### Property 9: Session Start Clears Displays

*For any* new tuning session start, both the DataGridView and Chart SHALL be cleared of previous session data.

**Validates: Requirements 3.6, 5.9**

### Property 10: DataGridView Auto-Scroll

*For any* new row added to the DataGridView, the view SHALL auto-scroll to make the most recent entry visible.

**Validates: Requirements 3.7**

### Property 11: Display Formatting Correctness

*For any* data row displayed in the DataGridView, timestamps SHALL be formatted as "yyyy-MM-dd HH:mm:ss.fff", frequency with 0 decimal places, voltage with 4 decimal places, and power with 3 decimal places.

**Validates: Requirements 3.8, 3.9, 3.10, 3.11**

### Property 12: CSV Type Column First

*For any* CSV file written by the DataLoggingController, the first column SHALL be named "Type" and SHALL contain either "setting" or "data" for each row.

**Validates: Requirements 4.3**

### Property 13: CSV Backward Compatibility

*For any* CSV file written with the new format, removing the first column SHALL produce a valid CSV file in the old format.

**Validates: Requirements 4.6**

### Property 14: Chart Data Point Addition

*For any* measurement taken during tuning, a corresponding data point SHALL be added to the Chart with X-coordinate equal to iteration number and Y-coordinate equal to power in dBm.

**Validates: Requirements 5.4**

### Property 15: Chart Y-Axis Auto-Scale

*For any* set of data points in the Chart, the Y-axis range SHALL include all data points plus the target and tolerance lines.

**Validates: Requirements 5.7**

### Property 16: Chart Color Distinction

*For any* Chart configuration, the measurement series, target line, and tolerance bands SHALL use distinct colors that are visually distinguishable.

**Validates: Requirements 5.11**

### Property 17: Simulation Mode Equivalence

*For any* feature operation, behavior in simulation mode SHALL match behavior with real hardware except that data values come from simulated sources instead of physical devices.

**Validates: Requirements 6.1, 6.2, 6.3, 6.4, 6.5, 6.6**

### Property 18: Real-Time Display Updates

*For any* tuning session in progress, both the DataGridView and Chart SHALL update within 200ms of each measurement being taken.

**Validates: Requirements 7.1, 7.2**

## Error Handling

### Device Connection Errors

**Manual Measure Disconnection Handling**:
- If signal generator is not connected, return ManualMeasurementResult with IsValid=false and ErrorMessage indicating signal generator disconnection
- If power meter is not connected, return ManualMeasurementResult with IsValid=false and ErrorMessage indicating power meter disconnection
- Display error message to user via message dialog

**Tuning Start Validation Errors**:
- If SensorId is missing or invalid, transition to Error state
- Emit ErrorOccurred event with descriptive message
- Do not attempt to configure devices or start measurement loop

### Data Logging Errors

**File Write Failures**:
- If CSV file cannot be written, emit OperationError event
- Continue tuning operation (logging failure should not stop tuning)
- Display error notification to user

**Directory Creation Failures**:
- If log directory cannot be created, emit OperationError event
- Disable logging for the session
- Allow user to select different log path

### UI Update Errors

**DataGridView Population Errors**:
- Catch exceptions during row addition
- Log error to debug output
- Continue tuning operation (display failure should not stop tuning)

**Chart Update Errors**:
- Catch exceptions during data point addition
- Log error to debug output
- Continue tuning operation (chart failure should not stop tuning)

### Thread Safety

**Cross-Thread UI Updates**:
- All UI updates from controller events must use Control.Invoke or Control.BeginInvoke
- DataGridView updates must be marshaled to UI thread
- Chart updates must be marshaled to UI thread

**Concurrent Access to DataLoggingController**:
- Use lock object to synchronize CSV file writes
- Ensure only one thread writes to file at a time

## Testing Strategy

### Dual Testing Approach

The testing strategy employs both unit tests and property-based tests:

**Unit Tests**: Focus on specific examples, edge cases, and integration points
- Test specific user action names are logged correctly (Connect, Disconnect, etc.)
- Test DataGridView and Chart initialization
- Test error handling for disconnected devices
- Test CSV file structure with known inputs
- Test UI control configuration (columns, chart series, etc.)

**Property-Based Tests**: Verify universal properties across all inputs
- Generate random TuningParameters and verify validation
- Generate random measurement sequences and verify logging/display
- Generate random user action sequences and verify setting rows
- Test formatting with random numeric values
- Test simulation mode with random parameters

### Property-Based Testing Configuration

**Framework**: Use NUnit with FsCheck for property-based testing in C#

**Test Configuration**:
- Minimum 100 iterations per property test
- Each test tagged with: **Feature: calibration-tuning-enhancements, Property {number}: {property_text}**

**Example Property Test Structure**:

```csharp
[Test]
[Property(MaxTest = 100)]
public void Property1_ManualMeasureCompleteness()
{
    // Feature: calibration-tuning-enhancements, Property 1: Manual Measure Completeness
    Prop.ForAll<double, double, double>((freq, voltage, power) =>
    {
        // Arrange: Setup mock services to return test values
        // Act: Call MeasureManualWithDetailsAsync
        // Assert: Result contains all three values
    }).QuickCheckThrowOnFailure();
}
```

### Test Coverage Goals

**Unit Test Coverage**:
- All new methods in TuningController (GetCurrentFrequencyAsync, GetCurrentVoltageAsync, MeasureManualWithDetailsAsync)
- All new methods in DataLoggingController (LogUserAction)
- ChartPanel initialization and configuration
- StatusPanel DataGridView initialization
- Error handling paths

**Property Test Coverage**:
- All 18 correctness properties
- Focus on data transformation and formatting
- Focus on state transitions and validation
- Focus on simulation mode equivalence

### Integration Testing

**End-to-End Scenarios**:
- Complete tuning session with DataGridView and Chart updates
- Manual measure with all three values displayed
- User action logging throughout session lifecycle
- CSV file generation with mixed setting and data rows
- Simulation mode full session

**UI Testing**:
- Verify DataGridView auto-scroll behavior
- Verify Chart rendering with multiple data points
- Verify button state changes during operations
- Verify error dialogs display correctly

### Simulation Mode Testing

All tests should run in both real hardware mode and simulation mode to verify Property 17 (Simulation Mode Equivalence). Use dependency injection to swap between real and simulated device services.

