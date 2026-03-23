# Implementation Plan: Calibration Tuning Application

## Overview

This implementation plan creates a WinForms application that integrates the existing Tegam 1830A and Siglent SDG6052X device libraries to perform automated RF power calibration through iterative voltage adjustment. The implementation follows a layered architecture with clear separation between UI, controller logic, and device services.

## Tasks

- [x] 1. Set up project structure and core interfaces
  - Create new WinForms project targeting .NET Framework 4.8
  - Add references to Tegam.1830A.DeviceLibrary and Siglent.SDG6052X.DeviceLibrary
  - Install NuGet packages: Microsoft.Extensions.DependencyInjection, System.Windows.Forms.DataVisualization
  - Define core interfaces: ITuningController, IDataLoggingController, IConfigurationController
  - Create data model classes: TuningParameters, TuningStatistics, TuningDataPoint, TuningResult
  - Create TuningState enumeration and event argument classes
  - _Requirements: 1.1, 1.2, 2.1, 3.1, 4.1_

- [x] 2. Implement configuration management
  - [x] 2.1 Create ConfigurationController class
    - Implement IConfigurationController interface
    - Add methods for loading/saving TuningParameters to JSON file in AppData
    - Add methods for loading/saving DeviceConfiguration (IP addresses)
    - Add methods for loading/saving last log file path
    - _Requirements: 11.1, 11.2, 11.3, 11.4, 11.5_
  
  - [ ]* 2.2 Write unit tests for ConfigurationController
    - Test saving and loading parameters
    - Test handling of missing configuration files
    - Test invalid JSON handling
    - _Requirements: 11.1, 11.2, 11.3, 11.4_

- [ ] 3. Implement data logging
  - [x] 3.1 Create DataLoggingController class
    - Implement IDataLoggingController interface
    - Implement CSV file writing with headers: Timestamp, Iteration, Frequency_Hz, Voltage, Power_dBm, Status
    - Implement LogMeasurement method for tuning data points
    - Implement LogSessionStart and LogSessionEnd methods
    - Add error handling for file write failures with event notification
    - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5, 7.6_
  
  - [ ]* 3.2 Write unit tests for DataLoggingController
    - Test CSV file creation and appending
    - Test measurement logging format
    - Test file write error handling
    - _Requirements: 7.3, 7.5, 7.6_

- [x] 4. Implement tuning controller core logic
  - [x] 4.1 Create TuningController class skeleton
    - Implement ITuningController interface
    - Add properties for CurrentState, Parameters, Statistics
    - Define all events: StateChanged, ProgressUpdated, TuningCompleted, ErrorOccurred
    - Add fields for device service references (IPowerMeterService, ISignalGeneratorService)
    - _Requirements: 1.1, 1.2, 5.1_
  
  - [x] 4.2 Implement device connection logic
    - Implement ConnectDevicesAsync method to connect both devices
    - Implement DisconnectDevices method
    - Add connection status tracking and event emission
    - Handle connection failures with appropriate error events
    - _Requirements: 1.3, 1.4, 1.5, 1.6, 10.3_
  
  - [x] 4.3 Implement tuning algorithm
    - Implement StartTuningAsync method with state machine
    - Configure signal generator frequency and initial voltage
    - Configure power meter frequency and sensor
    - Implement iterative measurement loop with proportional control
    - Calculate power error and adjust voltage by step size
    - Check convergence criteria (within tolerance)
    - Emit ProgressUpdated events after each iteration
    - _Requirements: 5.2, 5.3, 5.4, 5.5, 5.6, 5.8_
  
  - [x] 4.4 Implement safety limits and termination conditions
    - Check voltage against min/max limits before applying
    - Terminate with error if voltage limits exceeded
    - Check for power meter overload condition
    - Implement max iterations timeout
    - Disable signal generator output on error or completion
    - Emit TuningCompleted event with TuningResult
    - _Requirements: 5.7, 6.1, 6.2, 6.3, 6.4, 6.5_
  
  - [x] 4.5 Implement manual measurement and stop controls
    - Implement MeasureManualAsync method for single power reading
    - Implement StopTuning method to abort active tuning session
    - Handle state transitions for abort scenario
    - _Requirements: 5.9, 12.1, 12.2, 12.3_

- [x] 5. Checkpoint - Ensure controller logic is complete
  - Ensure all tests pass, ask the user if questions arise.

- [x] 6. Implement main form UI structure
  - [x] 6.1 Create MainForm with tab layout
    - Design MainForm with TabControl for Connection, Tuning, and Chart tabs
    - Add menu bar with File and Help menus
    - Add status bar for connection status display
    - _Requirements: 1.1, 1.2_
  
  - [x] 6.2 Create ConnectionPanel user control
    - Add IP address input fields for power meter and signal generator
    - Add Connect and Disconnect buttons
    - Add connection status labels for each device
    - Wire up button click events to TuningController methods
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 1.6_
  
  - [x] 6.3 Create TuningPanel user control
    - Add input controls for frequency, initial voltage, setpoint, tolerance, voltage step, min/max voltage, max iterations
    - Add sensor selection dropdown
    - Add Start Tuning and Stop Tuning buttons
    - Add Manual Measure button
    - Add validation for all numeric inputs
    - Disable tuning controls when devices disconnected
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 3.3, 3.4, 4.1, 4.2, 4.3, 4.4, 4.5, 4.6, 4.7, 4.8, 6.1, 6.3, 12.1_

- [ ] 7. Implement real-time status display
  - [x] 7.1 Create StatusPanel user control
    - Add labels for current iteration, measured power, current voltage, power error
    - Add label for tuning status (Idle, Tuning, Converged, Timeout, Error, Aborted)
    - Subscribe to TuningController.ProgressUpdated event
    - Update display on each progress event
    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6_
  
  - [x] 7.2 Wire StatusPanel to TuningController events
    - Subscribe to StateChanged event to update status label
    - Subscribe to ProgressUpdated event to update measurement displays
    - Subscribe to TuningCompleted event to show final result
    - Subscribe to ErrorOccurred event to display error messages
    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6, 10.1, 10.2_

- [ ] 8. Implement tuning history visualization
  - [~] 8.1 Create ChartControl with Chart component
    - Add Chart control to display power vs iteration
    - Configure chart with X-axis (Iteration) and Y-axis (Power dBm)
    - Add series for measured power data points
    - Add horizontal line series for setpoint
    - Add horizontal line series for tolerance bounds
    - _Requirements: 9.1, 9.3, 9.4_
  
  - [~] 8.2 Wire ChartControl to tuning events
    - Subscribe to TuningController.ProgressUpdated event
    - Add data point to chart series on each progress update
    - Clear chart data when new tuning session starts
    - Retain chart data when tuning completes
    - _Requirements: 9.2, 9.5_

- [ ] 9. Integrate data logging with tuning controller
  - [~] 9.1 Wire DataLoggingController to tuning events
    - Add file path selection control to TuningPanel
    - Call DataLoggingController.StartLogging when tuning starts
    - Subscribe to TuningController.ProgressUpdated to log each measurement
    - Call DataLoggingController.LogSessionEnd when tuning completes
    - Log manual measurements with "Manual" status
    - Display logging errors to user but continue tuning
    - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.6, 12.4_

- [ ] 10. Implement configuration persistence
  - [-] 10.1 Wire ConfigurationController to application lifecycle
    - Load device IP addresses on MainForm load
    - Load last tuning parameters on MainForm load
    - Load last log file path on MainForm load
    - Save all configuration on MainForm closing
    - Populate UI controls with loaded values
    - _Requirements: 11.1, 11.2, 11.3, 11.4_

- [ ] 11. Implement error handling and recovery
  - [~] 11.1 Add error handling to device operations
    - Wrap all device service calls in try-catch blocks
    - Emit ErrorOccurred events for device errors
    - Display error messages in MessageBox dialogs
    - Terminate tuning session on communication failures
    - _Requirements: 10.1, 10.2, 10.3_
  
  - [~] 11.2 Add retry and abort options
    - Add Retry button to error message dialogs where applicable
    - Ensure Stop button aborts tuning on any error
    - Log all errors to data log file
    - _Requirements: 10.4, 10.5_

- [x] 12. Implement dependency injection and application startup
  - [x] 12.1 Configure dependency injection container
    - Create ServiceCollection in Program.cs
    - Register device services (IPowerMeterService, ISignalGeneratorService)
    - Register controllers (ITuningController, IDataLoggingController, IConfigurationController)
    - Register MainForm with injected dependencies
    - _Requirements: 1.1, 1.2_
  
  - [x] 12.2 Update Program.cs entry point
    - Build service provider
    - Resolve MainForm from container
    - Run application with dependency-injected form
    - _Requirements: 1.1, 1.2_

- [ ] 13. Final integration and testing
  - [~] 13.1 Test complete tuning workflow
    - Manually test device connection
    - Manually test parameter configuration
    - Manually test tuning process with convergence
    - Manually test tuning timeout scenario
    - Manually test voltage limit scenarios
    - Manually test manual measurement
    - Verify data logging output
    - Verify configuration persistence
    - _Requirements: 1.3, 1.4, 2.5, 2.6, 3.2, 3.4, 4.1-4.8, 5.1-5.9, 6.2, 6.4, 7.1-7.6, 8.1-8.6, 9.1-9.5, 11.4, 12.2, 12.3_
  
  - [~] 13.2 Test error scenarios
    - Test connection failures
    - Test device communication errors during tuning
    - Test file write errors
    - Test invalid parameter inputs
    - Verify error messages and recovery options
    - _Requirements: 1.5, 7.6, 10.1, 10.2, 10.3, 10.4_

- [~] 14. Final checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- The implementation uses existing device libraries (Tegam.1830A.DeviceLibrary and Siglent.SDG6052X.DeviceLibrary)
- All code is written in C# targeting .NET Framework 4.8
- Checkpoints ensure incremental validation at key milestones
