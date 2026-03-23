# Requirements Document

## Introduction

This document specifies the functional and non-functional requirements for the Siglent SDG6052X Control Application. The application provides comprehensive control over the Siglent SDG6052X dual-channel arbitrary waveform generator through a C# .NET solution consisting of a device library DLL, WinForms UI application, and unit test project. The system communicates with the physical device via TCPIP using the NI-VISA library and implements the SCPI protocol. A mock communication component enables testing and development without physical hardware.

## Glossary

- **Device_Library**: The .NET Framework 4.0 DLL containing all core device communication logic, SCPI command processing, and data models
- **UI_Application**: The WinForms application (.NET Framework 4.8) that provides the user interface for device control
- **VISA_Manager**: The component responsible for low-level TCPIP communication with the device using NI-VISA
- **Mock_VISA_Manager**: A simulated communication component that enables testing without physical hardware
- **SCPI_Command_Builder**: The component that constructs valid SCPI commands from application parameters
- **SCPI_Response_Parser**: The component that parses SCPI responses into strongly-typed objects
- **Signal_Generator_Service**: The high-level application service coordinating device operations
- **Input_Validator**: The component that validates user input against device specifications
- **SDG6052X**: The Siglent SDG6052X dual-channel arbitrary waveform generator hardware device
- **Channel**: One of the two independent output channels (Channel 1 or Channel 2) on the SDG6052X
- **Waveform_Parameters**: The set of parameters defining a waveform (frequency, amplitude, offset, phase, etc.)
- **Load_Impedance**: The output load setting (50Ω, High-Z, or custom value)
- **Modulation**: The process of varying a carrier waveform parameter (AM, FM, PM, PWM, FSK, ASK, PSK)
- **Sweep**: A frequency sweep operation that varies frequency over time
- **Burst**: A mode that outputs a specific number of waveform cycles or gates the output
- **Arbitrary_Waveform**: A user-defined waveform stored as an array of data points

## Requirements

### Requirement 1: Device Connection Management

**User Story:** As a user, I want to connect to the SDG6052X device over the network, so that I can control the signal generator remotely.

#### Acceptance Criteria

1. WHEN a user provides a valid IPv4 address, THE UI_Application SHALL attempt to establish a TCPIP connection to the device
2. WHEN the connection attempt succeeds, THE VISA_Manager SHALL establish a VISA session and THE Signal_Generator_Service SHALL query device identity
3. WHEN the device identity is retrieved, THE Signal_Generator_Service SHALL verify the device model contains "SDG6052X"
4. IF the device model does not match SDG6052X, THEN THE Signal_Generator_Service SHALL disconnect and return a connection failure
5. WHEN a connection is established, THE Signal_Generator_Service SHALL set IsConnected to true and raise a ConnectionStateChanged event
6. WHEN a connection attempt fails, THE Signal_Generator_Service SHALL set IsConnected to false and raise a ConnectionStateChanged event with error details
7. WHEN a connection timeout occurs (exceeding 5000ms), THE VISA_Manager SHALL throw a timeout exception
8. WHEN a user requests disconnection, THE VISA_Manager SHALL close the VISA session and release all resources

### Requirement 2: Basic Waveform Configuration

**User Story:** As a user, I want to configure basic waveforms on each channel, so that I can generate standard signal types with specific parameters.

#### Acceptance Criteria

1. WHEN a user selects a waveform type (Sine, Square, Ramp, Pulse, Noise, Arbitrary, DC, PRBS, IQ), THE UI_Application SHALL display appropriate parameter controls
2. WHEN a user provides waveform parameters, THE Input_Validator SHALL validate frequency against waveform-specific limits
3. WHEN a user provides amplitude and offset values, THE Input_Validator SHALL validate that |offset| + (amplitude/2) does not exceed maximum voltage for the load impedance
4. WHEN all parameters are valid, THE SCPI_Command_Builder SHALL construct a valid SCPI command in the format "C{channel}:BSWV WVTP,{type},FRQ,{freq},AMP,{amp},OFST,{offset},PHSE,{phase}"
5. WHEN the SCPI command is sent, THE VISA_Manager SHALL transmit the command to the device and return the response
6. WHEN the device returns success, THE Signal_Generator_Service SHALL query the device state to verify the configuration
7. WHEN the device returns an error, THE Signal_Generator_Service SHALL parse the error, raise a DeviceError event, and return an OperationResult with failure details
8. WHEN configuring one channel, THE Device_Library SHALL ensure the other channel state remains unchanged

### Requirement 3: Parameter Validation

**User Story:** As a user, I want the application to validate my input parameters, so that I don't send invalid commands to the device.

#### Acceptance Criteria

1. WHEN a frequency value is provided, THE Input_Validator SHALL verify it is within the range for the selected waveform type (Sine: 1µHz-500MHz, Square: 1µHz-200MHz, Ramp: 1µHz-50MHz, Pulse: 1µHz-100MHz)
2. WHEN an amplitude value is provided with 50Ω load, THE Input_Validator SHALL verify it is between 1mVpp and 20Vpp
3. WHEN an amplitude value is provided with High-Z load, THE Input_Validator SHALL verify it is between 2mVpp and 40Vpp
4. WHEN an offset value is provided with 50Ω load, THE Input_Validator SHALL verify it is between -10V and +10V
5. WHEN an offset value is provided with High-Z load, THE Input_Validator SHALL verify it is between -20V and +20V
6. WHEN a phase value is provided, THE Input_Validator SHALL verify it is between 0° and 360°
7. WHEN a duty cycle is provided for square or pulse waveforms, THE Input_Validator SHALL verify it is between 0.01% and 99.99%
8. WHEN validation fails, THE Input_Validator SHALL return a ValidationResult with IsValid false and a descriptive error message
9. WHEN validation fails, THE Signal_Generator_Service SHALL not send any command to the device

### Requirement 4: Output Control

**User Story:** As a user, I want to enable and disable channel outputs, so that I can control when signals are generated.

#### Acceptance Criteria

1. WHEN a user enables output for a channel, THE SCPI_Command_Builder SHALL construct the command "C{channel}:OUTP ON"
2. WHEN a user disables output for a channel, THE SCPI_Command_Builder SHALL construct the command "C{channel}:OUTP OFF"
3. WHEN the output state command is sent, THE VISA_Manager SHALL transmit it to the device
4. WHEN the device confirms the output state change, THE Signal_Generator_Service SHALL return success
5. WHEN enabling output on one channel, THE Device_Library SHALL not affect the output state of the other channel

