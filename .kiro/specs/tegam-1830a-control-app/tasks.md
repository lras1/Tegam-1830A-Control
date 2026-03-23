# Implementation Tasks: Tegam 1830A Control Application

## Phase 1: Device Library Project Setup

- [x] 1. Create Device Library Project
  - [x] 1.1 Create new Class Library project targeting .NET Framework 4.0
  - [x] 1.2 Name project "Tegam.1830A.DeviceLibrary"
  - [x] 1.3 Add NuGet package: NationalInstruments.Visa (version 20.0.0 or compatible with .NET 4.0)
  - [x] 1.4 Create folder structure: Communication/, Commands/, Parsing/, Services/, Validation/, Models/, Simulation/
    - _Requirements: 1.0_

## Phase 2: Data Models and Enums (Tegam 1830A Specific)

- [x] 2. Implement Core Data Models
  - [x] 2.1 Create FrequencyUnit enum (Hz, kHz, MHz, GHz)
  - [x] 2.2 Create PowerUnit enum (dBm, W, mW)
  - [x] 2.3 Create CalibrationMode enum (Internal, External)
  - [x] 2.4 Create SensorInfo class with properties (SensorId, Name, MinFrequency, MaxFrequency, MinPower, MaxPower)
  - [x] 2.5 Create PowerMeasurement class with properties (PowerValue, PowerUnit, Frequency, FrequencyUnit, SensorId, Timestamp)
  - [x] 2.6 Create FrequencyResponse class with properties (Frequency, Unit)
  - [x] 2.7 Create CalibrationStatus class with properties (IsCalibrating, IsComplete, IsSuccessful, ErrorMessage)
  - [x] 2.8 Create DeviceIdentity class (Manufacturer, Model, SerialNumber, FirmwareVersion)
  - [x] 2.9 Create SystemStatus class with properties (IsReady, Temperature, ErrorCount)
  - [x] 2.10 Create OperationResult class with static factory methods (Success, Failure)
  - [x] 2.11 Create CommandResult class with properties (IsSuccess, Response, ErrorMessage)
  - [x] 2.12 Create ValidationResult class with properties (IsValid, ErrorMessage)
  - [x] 2.13 Create DeviceError class with properties (ErrorCode, ErrorMessage, Timestamp)
    - _Requirements: 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 10.0_

## Phase 3: SCPI Command Builder (Tegam 1830A Specific Commands)

- [x] 3. Implement SCPI Command Builder
  - [x] 3.1 Create IScpiCommandBuilder interface
  - [x] 3.2 Create ScpiCommandBuilder class implementing interface
  - [x] 3.3 Implement BuildFrequencyCommand() method
  - [x] 3.4 Implement BuildFrequencyQueryCommand() method
  - [x] 3.5 Implement BuildMeasurePowerCommand() method
  - [x] 3.6 Implement BuildMeasurePowerQueryCommand() method
  - [x] 3.7 Implement BuildSelectSensorCommand() method
  - [x] 3.8 Implement BuildQuerySensorCommand() method
  - [x] 3.9 Implement BuildQueryAvailableSensorsCommand() method
  - [x] 3.10 Implement BuildCalibrateCommand() method
  - [x] 3.11 Implement BuildQueryCalibrationStatusCommand() method
  - [x] 3.12 Implement BuildStartLoggingCommand() method
  - [x] 3.13 Implement BuildStopLoggingCommand() method
  - [x] 3.14 Implement BuildQueryLoggingStatusCommand() method
  - [x] 3.15 Implement BuildSystemCommand() method for reset and status queries
  - [x] 3.16 Implement helper methods for frequency unit conversion (Hz, kHz, MHz, GHz)
  - [x] 3.17 Implement helper methods for power unit conversion (dBm, W, mW)
    - _Requirements: 2.0, 3.0, 4.0, 5.0, 6.0, 10.0_

## Phase 4: SCPI Response Parser (Tegam 1830A Specific Responses)

- [x] 4. Implement SCPI Response Parser
  - [x] 4.1 Create IScpiResponseParser interface
  - [x] 4.2 Create ScpiResponseParser class implementing interface
  - [x] 4.3 Implement ParseBooleanResponse() method
  - [x] 4.4 Implement ParseNumericResponse() method
  - [x] 4.5 Implement ParseStringResponse() method
  - [x] 4.6 Implement ParsePowerMeasurement() method
  - [x] 4.7 Implement ParseFrequencyResponse() method
  - [x] 4.8 Implement ParseSensorInfo() method
  - [x] 4.9 Implement ParseAvailableSensors() method
  - [x] 4.10 Implement ParseCalibrationStatus() method
  - [x] 4.11 Implement ParseIdentityResponse() method
  - [x] 4.12 Implement ParseSystemStatus() method
  - [x] 4.13 Implement ParseErrorResponse() method
  - [x] 4.14 Implement helper methods for power unit parsing (dBm, W, mW)
  - [x] 4.15 Implement helper methods for frequency unit parsing (Hz, kHz, MHz, GHz)
    - _Requirements: 2.0, 3.0, 4.0, 5.0, 8.0, 10.0_

## Phase 5: Input Validator (Tegam 1830A Specific Validation)

- [x] 5. Implement Input Validator
  - [x] 5.1 Create IInputValidator interface
  - [x] 5.2 Create InputValidator class implementing interface
  - [x] 5.3 Implement ValidateFrequency() method (100 kHz to 40 GHz range)
  - [x] 5.4 Implement ValidateSensorId() method (1-4 range)
  - [x] 5.5 Implement ValidateCalibrationMode() method
  - [x] 5.6 Implement ValidateFilename() method for logging
  - [x] 5.7 Implement ValidateMeasurementCount() method
  - [x] 5.8 Implement ValidateMeasurementDelay() method
    - _Requirements: 7.0_

## Phase 6: VISA Communication Manager (Real)

- [x] 6. Implement Real VISA Communication Manager
  - [x] 6.1 Create IVisaCommunicationManager interface
  - [x] 6.2 Create VisaCommunicationManager class implementing interface
  - [x] 6.3 Implement Connect() method with VISA session establishment
  - [x] 6.4 Implement Disconnect() method with resource cleanup
  - [x] 6.5 Implement IsConnected property
  - [x] 6.6 Implement SendCommand() method
  - [x] 6.7 Implement Query() method
  - [x] 6.8 Implement QueryBinary() method
  - [x] 6.9 Remove SendCommandAsync() method (not supported in .NET 4.0)
  - [x] 6.10 Remove QueryAsync() method (not supported in .NET 4.0)
  - [x] 6.11 Implement GetDeviceIdentity() method
  - [x] 6.12 Implement CommunicationError event
  - [x] 6.13 Implement IDisposable pattern for resource cleanup
  - [x] 6.14 Implement timeout handling (5000ms default)
  - [x] 6.15 Implement error handling and exception wrapping
    - _Requirements: 1.0, 8.0_

## Phase 7: Mock VISA Communication Manager (Simulation)

- [x] 7. Implement Mock VISA Communication Manager
  - [x] 7.1 Create SimulatedDeviceState class with frequency, sensor, calibration, and logging state
  - [x] 7.2 Create MockVisaCommunicationManager class implementing IVisaCommunicationManager
  - [x] 7.3 Implement Connect() method with simulated connection
  - [x] 7.4 Implement Disconnect() method
  - [x] 7.5 Implement IsConnected property
  - [x] 7.6 Implement SendCommand() method with SCPI command parsing
  - [x] 7.7 Implement Query() method with response generation
  - [x] 7.8 Implement QueryBinary() method
  - [x] 7.9 Remove SendCommandAsync() method (not supported in .NET 4.0)
  - [x] 7.10 Remove QueryAsync() method (not supported in .NET 4.0)
  - [x] 7.11 Implement GetDeviceIdentity() method returning simulated identity
  - [x] 7.12 Implement ProcessCommand() method for state updates
  - [x] 7.13 Implement ProcessQuery() method for response generation
  - [x] 7.14 Implement GeneratePowerMeasurementResponse() method
  - [x] 7.15 Implement GenerateIdentityResponse() method
  - [x] 7.16 Implement SimulateError() method for error injection
  - [x] 7.17 Implement SimulateConnectionLoss() method
  - [x] 7.18 Implement SimulateTimeout() method
  - [x] 7.19 Implement parameter validation in mock
  - [x] 7.20 Implement GetDeviceState() method for test verification
    - _Requirements: 9.0_

## Phase 8: Power Meter Service (Tegam 1830A Specific)

- [x] 8. Implement Power Meter Service
  - [x] 8.1 Create IPowerMeterService interface
  - [x] 8.2 Create PowerMeterService class implementing interface
  - [x] 8.3 Implement constructor with dependency injection
  - [x] 8.4 Implement Connect() method (synchronous, no async in .NET 4.0)
  - [x] 8.5 Implement Disconnect() method (synchronous, no async in .NET 4.0)
  - [x] 8.6 Implement IsConnected property
  - [x] 8.7 Implement DeviceInfo property
  - [x] 8.8 Implement SetFrequency() method (synchronous)
  - [x] 8.9 Implement GetFrequency() method (synchronous)
  - [x] 8.10 Implement MeasurePower() method (synchronous)
  - [x] 8.11 Implement MeasureMultiple() method (synchronous)
  - [x] 8.12 Implement SelectSensor() method (synchronous)
  - [x] 8.13 Implement GetCurrentSensor() method (synchronous)
  - [x] 8.14 Implement GetAvailableSensors() method (synchronous)
  - [x] 8.15 Implement Calibrate() method (synchronous)
  - [x] 8.16 Implement GetCalibrationStatus() method (synchronous)
  - [x] 8.17 Implement StartLogging() method (synchronous)
  - [x] 8.18 Implement StopLogging() method (synchronous)
  - [x] 8.19 Implement IsLogging() method (synchronous)
  - [x] 8.20 Implement GetSystemStatus() method (synchronous)
  - [x] 8.21 Implement ResetDevice() method (synchronous)
  - [x] 8.22 Implement GetErrorQueue() method (synchronous)
  - [x] 8.23 Implement MeasurementReceived event
  - [x] 8.24 Implement DeviceError event
  - [x] 8.25 Implement ConnectionStateChanged event
  - [x] 8.26 Implement state caching mechanism
  - [x] 8.27 Implement GetLastDeviceError() helper method
    - _Requirements: 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 8.0, 10.0_

## Phase 9: Configuration Manager and Error Handler

- [ ] 9. Implement Configuration Manager and Error Handler
  - [ ] 9.1 Create IConfigurationManager interface
  - [ ] 9.2 Create ConfigurationManager class implementing interface
  - [ ] 9.3 Implement GetLastIpAddress() method
  - [ ] 9.4 Implement SaveLastIpAddress() method
  - [ ] 9.5 Implement GetConnectionTimeout() method
  - [ ] 9.6 Implement GetDefaultFrequencyUnit() method
  - [ ] 9.7 Implement SaveDefaultFrequencyUnit() method
  - [ ] 9.8 Implement GetDefaultPowerUnit() method
  - [ ] 9.9 Implement SaveDefaultPowerUnit() method
  - [ ] 9.10 Implement GetDefaultLogPath() method
  - [ ] 9.11 Implement SaveDefaultLogPath() method
  - [ ] 9.12 Implement GetSetting<T>() generic method
  - [ ] 9.13 Implement SaveSetting<T>() generic method
  - [ ] 9.14 Create IErrorHandler interface
  - [ ] 9.15 Create ErrorHandler class implementing interface
  - [ ] 9.16 Implement LogError() method
  - [ ] 9.17 Implement LogWarning() method
  - [ ] 9.18 Implement LogInfo() method
  - [ ] 9.19 Implement TryRecoverFromError() method
  - [ ] 9.20 Implement ErrorOccurred event
  - [ ] 9.21 Implement GetRecentErrors() method
    - _Requirements: 8.0, 11.0_

## Phase 10: Unit Test Project Setup

- [ ] 10. Create Unit Test Project
  - [ ] 10.1 Create new Unit Test project targeting .NET Framework 4.8
  - [ ] 10.2 Name project "Tegam.1830A.Tests"
  - [ ] 10.3 Add project reference to Device Library
  - [ ] 10.4 Add NuGet packages: NUnit, NUnit3TestAdapter, Moq, FsCheck, Microsoft.NET.Test.Sdk
  - [ ] 10.5 Create folder structure: Unit/, Integration/, PropertyBased/
    - _Requirements: 1.0_

## Phase 11: Unit Tests for Device Library

- [ ] 11. Implement Unit Tests
  - [ ] 11.1 Create ScpiCommandBuilderTests class
  - [ ] 11.2 Write tests for BuildFrequencyCommand() with all frequency units
  - [ ] 11.3 Write tests for frequency unit conversion (Hz, kHz, MHz, GHz)
  - [ ] 11.4 Write tests for power unit conversion (dBm, W, mW)
  - [ ] 11.5 Write tests for sensor selection command building
  - [ ] 11.6 Write tests for calibration command building
  - [ ] 11.7 Write tests for logging command building
  - [ ] 11.8 Create ScpiResponseParserTests class
  - [ ] 11.9 Write tests for ParsePowerMeasurement() with valid responses
  - [ ] 11.10 Write tests for ParsePowerMeasurement() with malformed responses
  - [ ] 11.11 Write tests for numeric parsing with various units
  - [ ] 11.12 Write tests for ParseFrequencyResponse()
  - [ ] 11.13 Write tests for ParseSensorInfo()
  - [ ] 11.14 Write tests for ParseCalibrationStatus()
  - [ ] 11.15 Write tests for ParseIdentityResponse()
  - [ ] 11.16 Write tests for ParseErrorResponse()
  - [ ] 11.17 Create InputValidatorTests class
  - [ ] 11.18 Write tests for ValidateFrequency() with boundary values
  - [ ] 11.19 Write tests for ValidateSensorId() with valid and invalid IDs
  - [ ] 11.20 Write tests for ValidateCalibrationMode()
  - [ ] 11.21 Write tests for ValidateFilename()
  - [ ] 11.22 Write tests for ValidateMeasurementCount()
  - [ ] 11.23 Create MockCommunicationManagerTests class
  - [ ] 11.24 Write tests for simulated state management
  - [ ] 11.25 Write tests for SCPI response generation
  - [ ] 11.26 Write tests for error simulation
  - [ ] 11.27 Write tests for validation in mock
    - _Requirements: 2.0, 3.0, 4.0, 5.0, 7.0, 9.0_

## Phase 12: Integration Tests

- [ ] 12. Implement Integration Tests
  - [ ] 12.1 Create PowerMeterServiceIntegrationTests class
  - [ ] 12.2 Write test for end-to-end frequency configuration using mock
  - [ ] 12.3 Write test for connection management
  - [ ] 12.4 Write test for error propagation through layers
  - [ ] 12.5 Write test for event raising (DeviceError, ConnectionStateChanged)
  - [ ] 12.6 Write test for async operation coordination
  - [ ] 12.7 Write test for state verification after commands
  - [ ] 12.8 Write test for power measurement workflow
  - [ ] 12.9 Write test for sensor selection workflow
  - [ ] 12.10 Write test for calibration workflow
  - [ ] 12.11 Write test for data logging workflow
  - [ ] 12.12 Write test for multiple measurements with delays
    - _Requirements: 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 8.0_

## Phase 13: Property-Based Tests

- [ ] 13. Implement Property-Based Tests
  - [ ] 13.1 Create PropertyBasedTests class
  - [ ] 13.2 Write property test for command-parse roundtrip
    - **Property 1: Command-Response Roundtrip Consistency**
    - **Validates: Requirements 2.0, 3.0, 4.0**
  - [ ] 13.3 Write property test for validation consistency
    - **Property 2: Validation Idempotence**
    - **Validates: Requirements 7.0**
  - [ ] 13.4 Write property test for frequency unit conversion
    - **Property 3: Frequency Unit Conversion Accuracy**
    - **Validates: Requirements 2.0**
  - [ ] 13.5 Write property test for power unit conversion
    - **Property 4: Power Unit Conversion Accuracy**
    - **Validates: Requirements 3.0**
    - _Requirements: 2.0, 3.0, 4.0, 7.0_

## Phase 14: WinForms UI Project Setup

- [x] 14. Create WinForms UI Project
  - [x] 14.1 Create new WinForms App project targeting .NET Framework 4.8
  - [x] 14.2 Name project "Tegam.1830A.WinFormsUI"
  - [x] 14.3 Add project reference to Device Library DLL
  - [x] 14.4 Add NuGet packages: Microsoft.Extensions.DependencyInjection
  - [x] 14.5 Create folder structure: Forms/, Controllers/
    - _Requirements: 1.0, 12.0_

## Phase 15: Main Form and Connection UI

- [x] 15. Implement Main Form
  - [x] 15.1 Design MainForm with connection controls (IP address input, connect/disconnect buttons)
  - [x] 15.2 Add status label for connection state
  - [x] 15.3 Add tab control for different measurement sections
  - [x] 15.4 Implement btnConnect_Click event handler
  - [x] 15.5 Implement btnDisconnect_Click event handler
  - [x] 15.6 Subscribe to ConnectionStateChanged event
  - [x] 15.7 Subscribe to DeviceError event
  - [x] 15.8 Implement EnableControls() method to enable/disable UI based on connection
  - [x] 15.9 Display device identity information when connected
  - [x] 15.10 Implement dependency injection setup in Program.cs
  - [x] 15.11 Wrap synchronous service calls in Task.Run() for UI responsiveness
  - [x] 15.12 Implement proper UI thread marshaling with Invoke()
    - _Requirements: 1.0, 12.0, 13.0, 14.0_

## Phase 16: UI Controllers

- [x] 16. Implement UI Controllers
  - [x] 16.1 Create MainFormController class
  - [x] 16.2 Create PowerMeasurementController class
  - [x] 16.3 Create FrequencyConfigurationController class
  - [x] 16.4 Create SensorManagementController class
  - [x] 16.5 Create CalibrationController class
  - [x] 16.6 Create DataLoggingController class
  - [x] 16.7 Implement dependency injection for all controllers
  - [x] 16.8 Wrap synchronous service calls in Task.Run() in all controllers
    - _Requirements: 12.0, 13.0_

## Phase 17: Power Measurement UI

- [x] 17. Implement Power Measurement Form/Panel
  - [x] 17.1 Design power measurement controls (measurement button, result display)
  - [x] 17.2 Add numeric input for frequency
  - [x] 17.3 Add unit selector for frequency (Hz, kHz, MHz, GHz)
  - [x] 17.4 Add sensor selector (1-4)
  - [x] 17.5 Add result display for power value and unit
  - [x] 17.6 Add timestamp display for measurements
  - [x] 17.7 Add "Measure Multiple" controls (count, delay between measurements)
  - [x] 17.8 Implement btnMeasure_Click event handler
  - [x] 17.9 Implement btnMeasureMultiple_Click event handler
  - [x] 17.10 Implement input validation with visual feedback
  - [x] 17.11 Subscribe to MeasurementReceived event
  - [x] 17.12 Add progress indicator for multiple measurements
    - _Requirements: 2.0, 3.0, 4.0, 13.0, 14.0_

## Phase 18: Frequency Configuration UI

- [x] 18. Implement Frequency Configuration Form/Panel
  - [x] 18.1 Design frequency configuration controls
  - [x] 18.2 Add numeric input for frequency
  - [x] 18.3 Add unit selector for frequency (Hz, kHz, MHz, GHz)
  - [x] 18.4 Add "Set Frequency" button
  - [x] 18.5 Add "Query Current Frequency" button
  - [x] 18.6 Add display for current frequency
  - [x] 18.7 Implement btnSetFrequency_Click event handler
  - [x] 18.8 Implement btnQueryFrequency_Click event handler
  - [x] 18.9 Implement input validation with visual feedback
  - [x] 18.10 Add frequency range information display (100 kHz to 40 GHz)
    - _Requirements: 2.0, 13.0, 14.0_

## Phase 19: Sensor Management UI

- [x] 19. Implement Sensor Management Form/Panel
  - [x] 19.1 Design sensor management controls
  - [x] 19.2 Add sensor selector (1-4)
  - [x] 19.3 Add "Select Sensor" button
  - [x] 19.4 Add "Query Current Sensor" button
  - [x] 19.5 Add "Query Available Sensors" button
  - [x] 19.6 Add list box to display available sensors with specifications
  - [x] 19.7 Add display for current sensor information
  - [x] 19.8 Implement btnSelectSensor_Click event handler
  - [x] 19.9 Implement btnQueryCurrentSensor_Click event handler
  - [x] 19.10 Implement btnQueryAvailableSensors_Click event handler
  - [x] 19.11 Implement input validation with visual feedback
    - _Requirements: 4.0, 13.0, 14.0_

## Phase 20: Calibration UI

- [x] 20. Implement Calibration Form/Panel
  - [x] 20.1 Design calibration controls
  - [x] 20.2 Add calibration mode selector (Internal, External)
  - [x] 20.3 Add "Start Calibration" button
  - [x] 20.4 Add "Query Calibration Status" button
  - [x] 20.5 Add status display for calibration progress
  - [x] 20.6 Add result display for calibration success/failure
  - [x] 20.7 Add progress indicator during calibration
  - [x] 20.8 Implement btnStartCalibration_Click event handler
  - [x] 20.9 Implement btnQueryCalibrationStatus_Click event handler
  - [x] 20.10 Implement polling mechanism for calibration status
  - [x] 20.11 Implement input validation with visual feedback
    - _Requirements: 5.0, 13.0, 14.0_

## Phase 21: Data Logging UI

- [x] 21. Implement Data Logging Form/Panel
  - [x] 21.1 Design data logging controls
  - [x] 21.2 Add filename input with file dialog
  - [x] 21.3 Add "Start Logging" button
  - [x] 21.4 Add "Stop Logging" button
  - [x] 21.5 Add logging status display
  - [x] 21.6 Add log file location display
  - [x] 21.7 Add "Open Log File" button
  - [x] 21.8 Add measurement count display during logging
  - [x] 21.9 Implement btnStartLogging_Click event handler
  - [x] 21.10 Implement btnStopLogging_Click event handler
  - [x] 21.11 Implement btnOpenLogFile_Click event handler
  - [x] 21.12 Implement input validation with visual feedback
  - [x] 21.13 Subscribe to MeasurementReceived event to update count
    - _Requirements: 6.0, 13.0, 14.0_

## Phase 22: Final Integration and Testing

- [ ] 22. Final Integration
  - [ ] 22.1 Run all unit tests and verify 80%+ code coverage
  - [ ] 22.2 Run all integration tests with mock communication
  - [ ] 22.3 Run all property-based tests
  - [ ] 22.4 Test UI application with mock communication manager
  - [ ] 22.5 Test UI application with real Tegam 1830A device (if available)
  - [ ] 22.6 Verify all SCPI commands generate correct syntax
  - [ ] 22.7 Verify all SCPI responses are parsed correctly
  - [ ] 22.8 Verify all validation rules are enforced
  - [ ] 22.9 Verify error handling and event raising
  - [ ] 22.10 Verify UI responsiveness with async operations
  - [ ] 22.11 Test connection/disconnection scenarios
  - [ ] 22.12 Test frequency configuration workflows
  - [ ] 22.13 Test power measurement workflows
  - [ ] 22.14 Test sensor selection workflows
  - [ ] 22.15 Test calibration workflows
  - [ ] 22.16 Test data logging workflows
  - [ ] 22.17 Build release version of Device Library DLL
  - [ ] 22.18 Build release version of WinForms UI application
  - [ ] 22.19 Create deployment package with dependencies
  - [ ] 22.20 Create README.md with installation instructions
  - [ ] 22.21 Test deployment package on clean machine
    - _Requirements: 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0_
