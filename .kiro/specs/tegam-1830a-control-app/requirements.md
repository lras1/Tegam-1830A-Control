# Requirements Document: Tegam 1830A Control Application

## Introduction

This document specifies the functional and non-functional requirements for the Tegam 1830A Control Application. The application provides comprehensive control over the Tegam 1830A RF/Microwave Power Meter through a C# .NET solution consisting of a device library DLL, WinForms UI application, and unit test project. The system communicates with the physical device via TCPIP using the NI-VISA library and implements the SCPI protocol. A mock communication component enables testing and development without physical hardware.

## Glossary

- **Device_Library**: The .NET Framework 4.0 DLL containing all core device communication logic, SCPI command processing, and data models
- **UI_Application**: The WinForms application (.NET Framework 4.8) that provides the user interface for device control
- **VISA_Manager**: The component responsible for low-level TCPIP communication with the device using NI-VISA
- **Mock_VISA_Manager**: A simulated communication component that enables testing without physical hardware
- **SCPI_Command_Builder**: The component that constructs valid SCPI commands from application parameters
- **SCPI_Response_Parser**: The component that parses SCPI responses into strongly-typed objects
- **Power_Meter_Service**: The high-level application service coordinating device operations
- **Input_Validator**: The component that validates user input against device specifications
- **Tegam_1830A**: The Tegam 1830A RF/Microwave Power Meter hardware device
- **Frequency**: The RF/microwave frequency at which power is measured, expressed in Hz, kHz, MHz, or GHz
- **Power_Measurement**: A measurement result containing power value, frequency, sensor ID, and timestamp
- **Sensor**: A measurement sensor (1-4) that can be selected for power measurements
- **Calibration_Mode**: The type of calibration (Internal or External) to be performed
- **Data_Logging**: The process of recording measurements to a file for later analysis
- **SCPI**: Standard Commands for Programmable Instruments protocol used for device communication

## Requirements

### Requirement 1: Device Connection Management

**User Story:** As a user, I want to connect to the Tegam 1830A device over the network, so that I can control the power meter remotely.

#### Acceptance Criteria

1. WHEN a user provides a valid IPv4 address, THE UI_Application SHALL attempt to establish a TCPIP connection to the device
2. WHEN the connection attempt succeeds, THE VISA_Manager SHALL establish a VISA session and THE Power_Meter_Service SHALL query device identity
3. WHEN the device identity is retrieved, THE Power_Meter_Service SHALL verify the device model contains "Tegam 1830A"
4. IF the device model does not match Tegam 1830A, THEN THE Power_Meter_Service SHALL disconnect and return a connection failure
5. WHEN a connection is established, THE Power_Meter_Service SHALL set IsConnected to true and raise a ConnectionStateChanged event
6. WHEN a connection attempt fails, THE Power_Meter_Service SHALL set IsConnected to false and raise a ConnectionStateChanged event with error details
7. WHEN a connection timeout occurs (exceeding 5000ms), THE VISA_Manager SHALL throw a timeout exception
8. WHEN a user requests disconnection, THE VISA_Manager SHALL close the VISA session and release all resources

### Requirement 2: Frequency Configuration

**User Story:** As a user, I want to set and query the measurement frequency, so that I can measure power at specific RF/microwave frequencies.

#### Acceptance Criteria

1. WHEN a user provides a frequency value, THE Input_Validator SHALL verify it is within the Tegam 1830A range (100 kHz to 40 GHz)
2. WHEN a frequency is provided in different units (Hz, kHz, MHz, GHz), THE Input_Validator SHALL accept all valid unit formats
3. WHEN a frequency is valid, THE SCPI_Command_Builder SHALL construct a valid SCPI command in the format "FREQ {value} {unit}"
4. WHEN the frequency command is sent, THE VISA_Manager SHALL transmit it to the device and return the response
5. WHEN the device returns success, THE Power_Meter_Service SHALL query the device to verify the frequency was set
6. WHEN the device returns an error, THE Power_Meter_Service SHALL parse the error, raise a DeviceError event, and return an OperationResult with failure details
7. WHEN a user queries the current frequency, THE Power_Meter_Service SHALL send a frequency query command to the device
8. WHEN the device returns the frequency, THE SCPI_Response_Parser SHALL parse it into a FrequencyResponse object

### Requirement 3: Power Measurement

**User Story:** As a user, I want to measure RF/microwave power at configured frequencies, so that I can verify signal levels and device performance.

#### Acceptance Criteria

1. WHEN a user requests a power measurement, THE Power_Meter_Service SHALL send a measurement query to the device
2. WHEN the device returns a measurement response, THE SCPI_Response_Parser SHALL parse it into a PowerMeasurement object containing power value, unit, and timestamp
3. WHEN a measurement is received, THE Power_Meter_Service SHALL raise a MeasurementReceived event with the measurement data
4. WHEN multiple measurements are requested, THE Power_Meter_Service SHALL collect them with specified delay between measurements
5. WHEN a measurement is taken, THE Power_Meter_Service SHALL include the current frequency and sensor ID in the result
6. WHEN the device returns a measurement error, THE Power_Meter_Service SHALL parse the error and raise a DeviceError event
7. WHEN measurements are displayed in the UI, THE UI_Application SHALL format power values with appropriate units (dBm, W, mW)

### Requirement 4: Sensor Management

**User Story:** As a user, I want to select and manage different measurement sensors, so that I can use the appropriate sensor for my measurement range.

#### Acceptance Criteria

1. WHEN a user selects a sensor, THE Input_Validator SHALL verify the sensor ID is valid (1-4)
2. WHEN a sensor ID is valid, THE SCPI_Command_Builder SHALL construct the command "SENS:SEL {sensorId}"
3. WHEN the sensor selection command is sent, THE VISA_Manager SHALL transmit it to the device
4. WHEN the device returns success, THE Power_Meter_Service SHALL update the current sensor state
5. WHEN a user queries available sensors, THE Power_Meter_Service SHALL send a query command to the device
6. WHEN the device returns available sensors, THE SCPI_Response_Parser SHALL parse them into a list of SensorInfo objects
7. WHEN a sensor is selected, THE Power_Meter_Service SHALL raise an event indicating the sensor change
8. WHEN the current sensor is queried, THE Power_Meter_Service SHALL return the SensorInfo for the active sensor

### Requirement 5: Calibration

**User Story:** As a user, I want to perform internal and external calibration, so that I can ensure measurement accuracy.

#### Acceptance Criteria

1. WHEN a user initiates internal calibration, THE Input_Validator SHALL verify the calibration mode is valid (Internal or External)
2. WHEN calibration mode is valid, THE SCPI_Command_Builder SHALL construct the appropriate calibration command
3. WHEN the calibration command is sent, THE VISA_Manager SHALL transmit it to the device
4. WHEN calibration is in progress, THE Power_Meter_Service SHALL poll the device for calibration status
5. WHEN calibration completes, THE Power_Meter_Service SHALL return the calibration status with success or failure indication
6. WHEN calibration fails, THE Power_Meter_Service SHALL parse the error and raise a DeviceError event
7. WHEN a user queries calibration status, THE Power_Meter_Service SHALL send a status query to the device
8. WHEN the device returns calibration status, THE SCPI_Response_Parser SHALL parse it into a CalibrationStatus object

### Requirement 6: Data Logging

**User Story:** As a user, I want to log measurements to files, so that I can record and analyze power measurements over time.

#### Acceptance Criteria

1. WHEN a user starts data logging, THE Input_Validator SHALL verify the filename is valid and contains no invalid path characters
2. WHEN a filename is valid, THE Power_Meter_Service SHALL create or open the log file
3. WHEN logging is started, THE Power_Meter_Service SHALL record the start time and initial device configuration
4. WHEN measurements are taken while logging is active, THE Power_Meter_Service SHALL append them to the log file with timestamp
5. WHEN a user stops logging, THE Power_Meter_Service SHALL close the log file and return success
6. WHEN logging is active, THE Power_Meter_Service SHALL provide a method to query logging status
7. WHEN a log file is created, THE Power_Meter_Service SHALL include a header with device information and measurement parameters
8. WHEN measurements are logged, THE format SHALL include timestamp, frequency, power value, unit, and sensor ID

### Requirement 7: Parameter Validation

**User Story:** As a user, I want the application to validate my input parameters, so that I don't send invalid commands to the device.

#### Acceptance Criteria

1. WHEN a frequency value is provided, THE Input_Validator SHALL verify it is within the range 100 kHz to 40 GHz
2. WHEN a frequency value is outside the valid range, THE Input_Validator SHALL return a ValidationResult with IsValid false and a descriptive error message
3. WHEN a sensor ID is provided, THE Input_Validator SHALL verify it is between 1 and 4
4. WHEN a sensor ID is invalid, THE Input_Validator SHALL return a ValidationResult with IsValid false
5. WHEN a calibration mode is provided, THE Input_Validator SHALL verify it is either Internal or External
6. WHEN a filename is provided for logging, THE Input_Validator SHALL verify it does not contain invalid path characters
7. WHEN a measurement count is provided, THE Input_Validator SHALL verify it is a positive integer
8. WHEN a measurement delay is provided, THE Input_Validator SHALL verify it is a non-negative integer
9. WHEN validation fails, THE Power_Meter_Service SHALL not send any command to the device

### Requirement 8: Error Handling and Recovery

**User Story:** As a user, I want the application to handle errors gracefully, so that I can understand what went wrong and recover from failures.

#### Acceptance Criteria

1. WHEN the device returns an error response, THE SCPI_Response_Parser SHALL parse it into a DeviceError object containing error code and message
2. WHEN an error is parsed, THE Power_Meter_Service SHALL raise a DeviceError event with error details
3. WHEN a communication error occurs, THE VISA_Manager SHALL raise a CommunicationError event
4. WHEN a connection is lost, THE Power_Meter_Service SHALL set IsConnected to false and raise a ConnectionStateChanged event
5. WHEN an error occurs during a measurement, THE Power_Meter_Service SHALL return an OperationResult with failure details
6. WHEN an error occurs during calibration, THE Power_Meter_Service SHALL stop the calibration process and return failure status
7. WHEN an error occurs during logging, THE Power_Meter_Service SHALL close the log file and raise a DeviceError event
8. WHEN the device error queue is queried, THE Power_Meter_Service SHALL retrieve all pending errors from the device

### Requirement 9: Mock Communication for Testing

**User Story:** As a developer, I want to test the application without physical hardware, so that I can develop and verify functionality in isolation.

#### Acceptance Criteria

1. WHEN the Mock_VISA_Manager is used, THE Power_Meter_Service SHALL operate identically to real hardware communication
2. WHEN a command is sent to the Mock_VISA_Manager, THE response SHALL match the expected format for that command
3. WHEN a frequency command is sent to the mock, THE mock SHALL store the frequency and return it in subsequent queries
4. WHEN a sensor selection command is sent to the mock, THE mock SHALL store the sensor ID and return it in subsequent queries
5. WHEN a measurement query is sent to the mock, THE mock SHALL return a simulated power measurement with realistic values
6. WHEN a calibration command is sent to the mock, THE mock SHALL simulate calibration with appropriate status responses
7. WHEN a logging command is sent to the mock, THE mock SHALL track logging state and simulate file operations
8. WHEN the mock is used, THE Device_Library SHALL not require NI-VISA to be installed

### Requirement 10: System Status and Information

**User Story:** As a user, I want to query device status and information, so that I can verify device health and configuration.

#### Acceptance Criteria

1. WHEN a user queries device identity, THE Power_Meter_Service SHALL send an identity query to the device
2. WHEN the device returns identity information, THE SCPI_Response_Parser SHALL parse it into a DeviceIdentity object
3. WHEN a user queries system status, THE Power_Meter_Service SHALL send a status query to the device
4. WHEN the device returns system status, THE SCPI_Response_Parser SHALL parse it into a SystemStatus object
5. WHEN a user requests to reset the device, THE Power_Meter_Service SHALL send a reset command to the device
6. WHEN the device is reset, THE Power_Meter_Service SHALL wait for the device to complete reset and verify connection
7. WHEN a user queries the error queue, THE Power_Meter_Service SHALL retrieve all pending errors from the device
8. WHEN errors are retrieved, THE SCPI_Response_Parser SHALL parse them into a list of DeviceError objects


### Requirement 11: Configuration Management

**User Story:** As a user, I want the application to remember my settings, so that I don't have to reconfigure the application each time I use it.

#### Acceptance Criteria

1. WHEN the application starts, THE Configuration_Manager SHALL load saved settings from the configuration file
2. WHEN a user connects to a device, THE Configuration_Manager SHALL save the IP address for future use
3. WHEN a user changes default units (frequency or power), THE Configuration_Manager SHALL persist the preference
4. WHEN a user sets a default log path, THE Configuration_Manager SHALL save it to the configuration file
5. WHEN the application starts, THE Configuration_Manager SHALL provide default values if no configuration file exists
6. WHEN settings are saved, THE Configuration_Manager SHALL write them to the user's AppData directory
7. WHEN a setting is requested, THE Configuration_Manager SHALL return the saved value or a default value
8. WHEN the configuration file is corrupted, THE Configuration_Manager SHALL use default values and log a warning

### Requirement 12: UI Controllers and Dependency Injection

**User Story:** As a developer, I want the UI to use dependency injection and controllers, so that the code is maintainable and testable.

#### Acceptance Criteria

1. WHEN the application starts, THE Program.cs SHALL configure a ServiceCollection with all dependencies
2. WHEN services are registered, THE ServiceCollection SHALL register IPowerMeterService, IVisaCommunicationManager, and all controllers
3. WHEN the MainForm is created, THE ServiceProvider SHALL inject all required dependencies via constructor
4. WHEN a controller is created, THE ServiceProvider SHALL inject the IPowerMeterService dependency
5. WHEN a panel is created, THE ServiceProvider SHALL inject the corresponding controller
6. WHEN the application uses mock mode, THE ServiceCollection SHALL register MockVisaCommunicationManager instead of VisaCommunicationManager
7. WHEN the application uses real hardware, THE ServiceCollection SHALL register VisaCommunicationManager
8. WHEN the application exits, THE ServiceProvider SHALL dispose all disposable services

### Requirement 13: Asynchronous UI Operations

**User Story:** As a user, I want the UI to remain responsive during device operations, so that the application doesn't freeze.

#### Acceptance Criteria

1. WHEN a long-running operation is initiated, THE UI SHALL wrap synchronous service calls in Task.Run()
2. WHEN an async operation is running, THE UI SHALL remain responsive to user input
3. WHEN an async operation completes, THE UI SHALL marshal results back to the UI thread using Invoke()
4. WHEN multiple measurements are taken, THE UI SHALL update progress without blocking
5. WHEN calibration is in progress, THE UI SHALL show progress indication without freezing
6. WHEN data logging is active, THE UI SHALL continue to respond to user interactions
7. WHEN an error occurs during an async operation, THE UI SHALL display the error message on the UI thread
8. WHEN the user closes the form during an async operation, THE application SHALL wait for the operation to complete or cancel it gracefully

### Requirement 14: Error Display and User Feedback

**User Story:** As a user, I want clear error messages and feedback, so that I understand what went wrong and how to fix it.

#### Acceptance Criteria

1. WHEN a validation error occurs, THE UI SHALL display a message box with the validation error message
2. WHEN a device error occurs, THE UI SHALL display a message box with the device error details
3. WHEN a communication error occurs, THE UI SHALL display a message box indicating connection loss
4. WHEN an operation succeeds, THE UI SHALL provide visual feedback (status bar update, color change, etc.)
5. WHEN an operation fails, THE UI SHALL provide visual feedback (error icon, red text, etc.)
6. WHEN the device is not connected, THE UI SHALL disable all measurement and configuration controls
7. WHEN the device is connected, THE UI SHALL enable all measurement and configuration controls
8. WHEN an error message is displayed, THE message SHALL include actionable information for the user

### Requirement 15: Deployment and Installation

**User Story:** As a user, I want to install the application easily, so that I can start using it quickly.

#### Acceptance Criteria

1. WHEN the application is deployed, THE deployment package SHALL include all required DLLs
2. WHEN the application is deployed, THE deployment package SHALL include the Device Library DLL
3. WHEN the application is deployed, THE deployment package SHALL include Microsoft.Extensions.DependencyInjection DLLs
4. WHEN the application is deployed, THE deployment package SHALL include a README.md with installation instructions
5. WHEN the application is deployed, THE deployment package SHALL include an app.config file
6. WHEN the application is installed, THE user SHALL be able to run it without additional configuration
7. WHEN the application runs, THE application SHALL create necessary directories in AppData if they don't exist
8. WHEN the application runs, THE application SHALL check for NI-VISA installation and display a warning if not found
