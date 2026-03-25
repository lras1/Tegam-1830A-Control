# Requirements Document

## Introduction

This document specifies requirements for enhancing the CalibrationTuning application with improved measurement display, bug fixes, real-time data visualization, and enhanced logging capabilities. The enhancements address usability issues, fix critical bugs in the tuning workflow, and add data visualization features to improve operator awareness during calibration sessions.

## Glossary

- **Manual_Measure_Button**: UI button that triggers a single power measurement without starting automated tuning
- **Start_Tuning_Button**: UI button that initiates an automated tuning session
- **Data_Grid_View**: WinForms DataGridView control displaying measurement data in tabular format
- **Chart_Control**: WinForms Chart control displaying power measurements graphically
- **Data_Logging_Controller**: Controller responsible for writing measurement data to CSV files
- **Tuning_Controller**: Controller that orchestrates the tuning process and device interactions
- **Signal_Generator**: Siglent SDG6052X device that generates RF signals
- **Power_Meter**: Tegam 1830A device that measures RF power
- **Tuning_Session**: A complete automated tuning cycle from start to convergence, timeout, or error
- **Setting_Row**: Log entry recording user actions or configuration changes
- **Data_Row**: Log entry recording actual measurement data points
- **Simulation_Mode**: Operating mode where device services return simulated data instead of communicating with hardware
- **Target_Power_Line**: Horizontal reference line on chart showing the desired power setpoint
- **Tolerance_Band**: Visual region on chart showing acceptable power range (target ± tolerance)

## Requirements

### Requirement 1: Manual Measure Enhancement

**User Story:** As an operator, I want the Manual Measure button to display frequency, voltage, and power, so that I can verify all signal generator settings before starting tuning.

#### Acceptance Criteria

1. WHEN THE Manual_Measure_Button is clicked, THE Tuning_Controller SHALL retrieve the current frequency from THE Signal_Generator
2. WHEN THE Manual_Measure_Button is clicked, THE Tuning_Controller SHALL retrieve the current voltage from THE Signal_Generator
3. WHEN THE Manual_Measure_Button is clicked, THE Tuning_Controller SHALL retrieve the current power from THE Power_Meter
4. WHEN all measurements are retrieved, THE Application SHALL display frequency, voltage, and power in a message dialog
5. IF THE Signal_Generator is not connected, THEN THE Application SHALL display an error message indicating signal generator disconnection
6. IF THE Power_Meter is not connected, THEN THE Application SHALL display an error message indicating power meter disconnection
7. WHILE THE Manual_Measure_Button operation is in progress, THE Manual_Measure_Button SHALL be disabled

### Requirement 2: Start Tuning Bug Fix

**User Story:** As an operator, I want the Start Tuning button to work correctly, so that I can perform automated calibration without errors.

#### Acceptance Criteria

1. WHEN THE Start_Tuning_Button is clicked with valid parameters, THE Tuning_Controller SHALL transition to Tuning state
2. WHEN THE Tuning_Controller starts tuning, THE Tuning_Controller SHALL verify that SensorId is present in TuningParameters
3. IF SensorId is missing or invalid, THEN THE Tuning_Controller SHALL transition to Error state and report the validation failure
4. WHEN THE Tuning_Controller configures devices, THE Tuning_Controller SHALL select the sensor on THE Power_Meter before starting measurements
5. WHEN THE Tuning_Controller completes device configuration, THE Tuning_Controller SHALL begin the measurement loop without errors
6. FOR ALL valid tuning parameter combinations, starting tuning SHALL NOT result in Error state unless device communication fails

### Requirement 3: Data Grid View Implementation

**User Story:** As an operator, I want to see measurement data in a table during tuning, so that I can track the tuning progress and review historical data points.

#### Acceptance Criteria

1. THE Application SHALL display a Data_Grid_View on the Status tab
2. THE Data_Grid_View SHALL contain columns: Type, Timestamp, Iteration, Frequency, Voltage, Power_dBm, Status
3. WHEN a user action occurs (Connect, Disconnect, Start Tuning, Stop Tuning, Manual Measure), THE Application SHALL add a setting row to THE Data_Grid_View
4. WHEN a measurement is taken during tuning, THE Application SHALL add a data row to THE Data_Grid_View
5. THE Data_Grid_View SHALL be read-only for users
6. WHEN a new tuning session starts, THE Application SHALL clear THE Data_Grid_View
7. THE Data_Grid_View SHALL auto-scroll to show the most recent entry
8. THE Data_Grid_View SHALL display timestamps in format "yyyy-MM-dd HH:mm:ss.fff"
9. THE Data_Grid_View SHALL display frequency in Hz with no decimal places
10. THE Data_Grid_View SHALL display voltage with 4 decimal places
11. THE Data_Grid_View SHALL display power with 3 decimal places

### Requirement 4: Enhanced Logging System

**User Story:** As an operator, I want logs to distinguish between settings and data, so that I can easily analyze measurement data separately from configuration changes.

#### Acceptance Criteria

1. WHEN a user action occurs, THE Data_Logging_Controller SHALL write a setting row to the CSV file
2. WHEN a measurement is taken, THE Data_Logging_Controller SHALL write a data row to the CSV file
3. THE Data_Logging_Controller SHALL include a Type column as the first column in CSV files
4. THE Data_Logging_Controller SHALL set Type to "setting" for user actions
5. THE Data_Logging_Controller SHALL set Type to "data" for measurement data points
6. THE Data_Logging_Controller SHALL maintain backward compatibility with existing CSV format by appending the Type column
7. WHEN Connect action occurs, THE Data_Logging_Controller SHALL log a setting row with action "Connect"
8. WHEN Disconnect action occurs, THE Data_Logging_Controller SHALL log a setting row with action "Disconnect"
9. WHEN Start Tuning action occurs, THE Data_Logging_Controller SHALL log a setting row with action "Start Tuning"
10. WHEN Stop Tuning action occurs, THE Data_Logging_Controller SHALL log a setting row with action "Stop Tuning"
11. WHEN Manual Measure action occurs, THE Data_Logging_Controller SHALL log a setting row with action "Manual Measure"

### Requirement 5: Chart Tab Implementation

**User Story:** As an operator, I want to see a real-time chart of power measurements, so that I can visually monitor tuning convergence and identify trends.

#### Acceptance Criteria

1. THE Application SHALL display a Chart_Control on the Chart tab
2. THE Chart_Control SHALL display Power_dBm on the Y-axis
3. THE Chart_Control SHALL display Iteration number on the X-axis
4. WHEN a measurement is taken during tuning, THE Application SHALL add a data point to THE Chart_Control
5. THE Chart_Control SHALL display a Target_Power_Line showing the target power setpoint
6. THE Chart_Control SHALL display upper and lower Tolerance_Band lines
7. THE Chart_Control SHALL auto-scale the Y-axis to fit all data points
8. THE Chart_Control SHALL display a legend identifying the measurement series, target line, and tolerance bands
9. WHEN a new tuning session starts, THE Application SHALL clear THE Chart_Control
10. THE Chart_Control SHALL use a line chart type for the measurement series
11. THE Chart_Control SHALL use different colors for measurement data, target line, and tolerance bands
12. THE Chart_Control SHALL display grid lines for improved readability

### Requirement 6: Simulation Mode Compatibility

**User Story:** As a developer, I want all enhancements to work in simulation mode, so that I can test the application without hardware.

#### Acceptance Criteria

1. WHEN THE Application runs in Simulation_Mode, THE Manual_Measure_Button SHALL return simulated frequency, voltage, and power values
2. WHEN THE Application runs in Simulation_Mode, THE Start_Tuning_Button SHALL execute the tuning algorithm with simulated measurements
3. WHEN THE Application runs in Simulation_Mode, THE Data_Grid_View SHALL populate with simulated measurement data
4. WHEN THE Application runs in Simulation_Mode, THE Chart_Control SHALL display simulated measurement data
5. WHEN THE Application runs in Simulation_Mode, THE Data_Logging_Controller SHALL write simulated data to CSV files
6. FOR ALL features, behavior in Simulation_Mode SHALL match behavior with real hardware except for data source

### Requirement 7: UI Responsiveness

**User Story:** As an operator, I want the UI to remain responsive during tuning, so that I can monitor progress and stop tuning if needed.

#### Acceptance Criteria

1. WHILE THE Tuning_Controller is executing a tuning session, THE Application SHALL update THE Data_Grid_View in real-time
2. WHILE THE Tuning_Controller is executing a tuning session, THE Application SHALL update THE Chart_Control in real-time
3. WHILE THE Tuning_Controller is executing a tuning session, THE Application SHALL remain responsive to user input
4. WHEN THE Stop_Tuning_Button is clicked during tuning, THE Application SHALL respond within 500 milliseconds
5. WHEN measurements are added to THE Data_Grid_View, THE Application SHALL not block the UI thread
6. WHEN data points are added to THE Chart_Control, THE Application SHALL not block the UI thread
