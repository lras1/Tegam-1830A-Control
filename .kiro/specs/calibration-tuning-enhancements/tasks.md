# Implementation Plan: Calibration Tuning Enhancements

## Overview

This implementation plan enhances the CalibrationTuning application with improved measurement display, bug fixes, real-time data visualization, and enhanced logging. The implementation builds upon the existing MVC-style architecture, adding new UI components (DataGridView, Chart), extending controllers with new methods, and enhancing the logging system to distinguish between user actions and measurement data.

## Tasks

- [x] 1. Implement Manual Measure Enhancement
  - [x] 1.1 Add GetCurrentFrequencyAsync method to TuningController
    - Implement method to retrieve current frequency from signal generator service
    - Handle disconnection errors and return appropriate error result
    - _Requirements: 1.1, 1.5_
  
  - [x] 1.2 Add GetCurrentVoltageAsync method to TuningController
    - Implement method to retrieve current voltage from signal generator service
    - Handle disconnection errors and return appropriate error result
    - _Requirements: 1.2, 1.5_
  
  - [x] 1.3 Create ManualMeasurementResult model class
    - Define properties: IsValid, FrequencyHz, Voltage, PowerDbm, Timestamp, ErrorMessage
    - _Requirements: 1.1, 1.2, 1.3, 1.4_
  
  - [x] 1.4 Implement MeasureManualWithDetailsAsync method in TuningController
    - Call GetCurrentFrequencyAsync, GetCurrentVoltageAsync, and power meter measurement
    - Aggregate results into ManualMeasurementResult
    - Handle device disconnection errors with appropriate error messages
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 1.6_
  
  - [x] 1.5 Update Manual Measure button handler in TuningPanel
    - Disable button during operation
    - Call MeasureManualWithDetailsAsync
    - Display frequency, voltage, and power in message dialog
    - Re-enable button after operation completes
    - _Requirements: 1.4, 1.7_
  
  - [ ]* 1.6 Write property test for Manual Measure Completeness
    - **Property 1: Manual Measure Completeness**
    - **Validates: Requirements 1.1, 1.2, 1.3, 1.4**
  
  - [ ]* 1.7 Write property test for Manual Measure Button State
    - **Property 2: Manual Measure Button State**
    - **Validates: Requirements 1.7**

- [x] 2. Fix Start Tuning Bug
  - [x] 2.1 Add SensorId validation to TuningController StartTuningAsync method
    - Check if SensorId is present and valid before device configuration
    - Transition to Error state if validation fails
    - Emit ErrorOccurred event with descriptive message
    - _Requirements: 2.2, 2.3_
  
  - [x] 2.2 Add sensor selection call before measurement loop
    - Call power meter service SelectSensor method with SensorId
    - Ensure sensor is selected before first measurement
    - _Requirements: 2.4, 2.5_
  
  - [ ]* 2.3 Write property test for SensorId Validation
    - **Property 3: SensorId Validation**
    - **Validates: Requirements 2.2**
  
  - [ ]* 2.4 Write property test for Sensor Selection Before Measurement
    - **Property 4: Sensor Selection Before Measurement**
    - **Validates: Requirements 2.4**
  
  - [ ]* 2.5 Write property test for Valid Parameters Start Successfully
    - **Property 5: Valid Parameters Start Successfully**
    - **Validates: Requirements 2.1, 2.5, 2.6**

- [x] 3. Implement Enhanced Logging System
  - [x] 3.1 Create UserActionEventArgs model class
    - Define properties: ActionName, Timestamp, Parameters
    - _Requirements: 4.1_
  
  - [x] 3.2 Add UserActionOccurred event to TuningController
    - Define event with UserActionEventArgs
    - _Requirements: 4.1_
  
  - [x] 3.3 Add LogUserAction method to DataLoggingController
    - Accept actionName and optional parameters dictionary
    - Write setting row to CSV with Type="setting"
    - Include timestamp and action name in Status column
    - _Requirements: 4.1, 4.3, 4.4, 4.7, 4.8, 4.9, 4.10, 4.11_
  
  - [x] 3.4 Update CSV format to include Type column as first column
    - Modify CSV header to include Type as first column
    - Update data row writing to include Type="data"
    - Maintain backward compatibility by preserving column order after Type
    - _Requirements: 4.3, 4.5, 4.6_
  
  - [x] 3.5 Emit UserActionOccurred events from TuningController
    - Emit event for Connect action
    - Emit event for Disconnect action
    - Emit event for Start Tuning action
    - Emit event for Stop Tuning action
    - Emit event for Manual Measure action
    - _Requirements: 4.7, 4.8, 4.9, 4.10, 4.11_
  
  - [x] 3.6 Subscribe DataLoggingController to UserActionOccurred event
    - Subscribe to TuningController.UserActionOccurred
    - Call LogUserAction when event is raised
    - _Requirements: 4.1_
  
  - [ ]* 3.7 Write property test for User Actions Create Setting Rows
    - **Property 6: User Actions Create Setting Rows**
    - **Validates: Requirements 3.3, 4.1, 4.4**
  
  - [ ]* 3.8 Write property test for CSV Type Column First
    - **Property 12: CSV Type Column First**
    - **Validates: Requirements 4.3**
  
  - [ ]* 3.9 Write property test for CSV Backward Compatibility
    - **Property 13: CSV Backward Compatibility**
    - **Validates: Requirements 4.6**

- [x] 4. Implement Data Grid View
  - [x] 4.1 Create DataGridRow model class
    - Define properties: Type, Timestamp, Iteration, Frequency, Voltage, PowerDbm, Status
    - _Requirements: 3.2_
  
  - [x] 4.2 Add DataGridView control to StatusPanel
    - Add DataGridView to StatusPanel designer
    - Configure columns: Type, Timestamp, Iteration, Frequency, Voltage, Power_dBm, Status
    - Set column widths, formats, and read-only properties
    - _Requirements: 3.1, 3.2, 3.5, 3.8, 3.9, 3.10, 3.11_
  
  - [x] 4.3 Implement AddSettingRow method in StatusPanel
    - Accept actionName and timestamp
    - Create DataGridRow with Type="setting"
    - Add row to DataGridView
    - Auto-scroll to latest entry
    - Marshal to UI thread if needed
    - _Requirements: 3.3, 3.7_
  
  - [x] 4.4 Implement AddDataRow method in StatusPanel
    - Accept measurement data (iteration, frequency, voltage, power, status)
    - Create DataGridRow with Type="data"
    - Add row to DataGridView with proper formatting
    - Auto-scroll to latest entry
    - Marshal to UI thread if needed
    - _Requirements: 3.4, 3.7, 3.8, 3.9, 3.10, 3.11_
  
  - [x] 4.5 Subscribe StatusPanel to TuningController events
    - Subscribe to UserActionOccurred event and call AddSettingRow
    - Subscribe to MeasurementTaken event and call AddDataRow
    - _Requirements: 3.3, 3.4_
  
  - [x] 4.6 Implement ClearDataGrid method in StatusPanel
    - Clear all rows from DataGridView
    - Call on new tuning session start
    - _Requirements: 3.6_
  
  - [ ]* 4.7 Write property test for DataGridView Read-Only
    - **Property 8: DataGridView Read-Only**
    - **Validates: Requirements 3.5**
  
  - [ ]* 4.8 Write property test for Session Start Clears Displays
    - **Property 9: Session Start Clears Displays**
    - **Validates: Requirements 3.6, 5.9**
  
  - [ ]* 4.9 Write property test for DataGridView Auto-Scroll
    - **Property 10: DataGridView Auto-Scroll**
    - **Validates: Requirements 3.7**
  
  - [ ]* 4.10 Write property test for Display Formatting Correctness
    - **Property 11: Display Formatting Correctness**
    - **Validates: Requirements 3.8, 3.9, 3.10, 3.11**
  
  - [ ]* 4.11 Write property test for Measurements Create Data Rows
    - **Property 7: Measurements Create Data Rows**
    - **Validates: Requirements 3.4, 4.2, 4.5**

- [x] 5. Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [x] 6. Implement Chart Tab
  - [x] 6.1 Create ChartPanel UserControl
    - Create new UserControl class ChartPanel
    - Add Chart control to designer
    - _Requirements: 5.1_
  
  - [x] 6.2 Initialize Chart control in ChartPanel constructor
    - Configure chart type as line chart
    - Set up X-axis for iteration numbers with grid lines
    - Set up Y-axis for power (dBm) with auto-scale and grid lines
    - Enable legend at top-right
    - _Requirements: 5.2, 5.3, 5.7, 5.8, 5.12_
  
  - [x] 6.3 Create measurement series in ChartPanel
    - Add Series for measurement data with blue color, circle markers, line width 2
    - _Requirements: 5.4, 5.10, 5.11_
  
  - [x] 6.4 Create target and tolerance series in ChartPanel
    - Add Series for target line with green dashed line, line width 2
    - Add Series for upper tolerance with red dashed line, line width 1
    - Add Series for lower tolerance with red dashed line, line width 1
    - _Requirements: 5.5, 5.6, 5.11_
  
  - [x] 6.5 Implement AddDataPoint method in ChartPanel
    - Accept iteration number and power value
    - Add data point to measurement series
    - Auto-scale Y-axis to fit all data points
    - Marshal to UI thread if needed
    - _Requirements: 5.4, 5.7_
  
  - [x] 6.6 Implement SetTargetAndTolerance method in ChartPanel
    - Accept target power and tolerance values
    - Update target line series
    - Update upper and lower tolerance series
    - _Requirements: 5.5, 5.6_
  
  - [x] 6.7 Implement ClearChart method in ChartPanel
    - Clear all data points from all series
    - Call on new tuning session start
    - _Requirements: 5.9_
  
  - [x] 6.8 Add ChartPanel to MainForm tab control
    - Add new tab "Chart" to MainForm
    - Add ChartPanel instance to Chart tab
    - _Requirements: 5.1_
  
  - [x] 6.9 Subscribe ChartPanel to TuningController events
    - Subscribe to MeasurementTaken event and call AddDataPoint
    - Subscribe to TuningStarted event and call SetTargetAndTolerance
    - Subscribe to TuningStarted event and call ClearChart
    - _Requirements: 5.4, 5.9_
  
  - [ ]* 6.10 Write property test for Chart Data Point Addition
    - **Property 14: Chart Data Point Addition**
    - **Validates: Requirements 5.4**
  
  - [ ]* 6.11 Write property test for Chart Y-Axis Auto-Scale
    - **Property 15: Chart Y-Axis Auto-Scale**
    - **Validates: Requirements 5.7**
  
  - [ ]* 6.12 Write property test for Chart Color Distinction
    - **Property 16: Chart Color Distinction**
    - **Validates: Requirements 5.11**

- [x] 7. Implement Simulation Mode Compatibility
  - [x] 7.1 Update mock signal generator service for new methods
    - Implement GetCurrentFrequencyAsync to return simulated frequency
    - Implement GetCurrentVoltageAsync to return simulated voltage
    - _Requirements: 6.1_
  
  - [x] 7.2 Verify simulation mode for all new features
    - Test Manual Measure with simulated devices
    - Test Start Tuning with simulated devices
    - Test DataGridView population with simulated data
    - Test Chart display with simulated data
    - Test CSV logging with simulated data
    - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6_
  
  - [ ]* 7.3 Write property test for Simulation Mode Equivalence
    - **Property 17: Simulation Mode Equivalence**
    - **Validates: Requirements 6.1, 6.2, 6.3, 6.4, 6.5, 6.6**

- [x] 8. Implement UI Responsiveness
  - [x] 8.1 Ensure all UI updates use Control.Invoke or BeginInvoke
    - Review DataGridView update code for thread marshaling
    - Review Chart update code for thread marshaling
    - _Requirements: 7.5, 7.6_
  
  - [x] 8.2 Add thread synchronization to DataLoggingController
    - Add lock object for CSV file writes
    - Ensure only one thread writes at a time
    - _Requirements: 7.1, 7.2_
  
  - [x] 8.3 Verify Stop button responsiveness during tuning
    - Test Stop button responds within 500ms during tuning
    - _Requirements: 7.4_
  
  - [ ]* 8.4 Write property test for Real-Time Display Updates
    - **Property 18: Real-Time Display Updates**
    - **Validates: Requirements 7.1, 7.2**

- [x] 9. Final checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation
- Property tests validate universal correctness properties
- Unit tests validate specific examples and edge cases
- All enhancements maintain backward compatibility with existing CSV format
- Thread safety is critical for UI updates from controller events
