# Tegam 1830A Control Application

A comprehensive C# .NET solution for controlling the Tegam 1830A RF/Microwave Power Meter via TCPIP using the SCPI protocol.

## Solution Overview

This solution consists of three projects:

### 1. Tegam.1830A.DeviceLibrary (.NET Framework 4.0)
**Location**: `Tegam.1830A.DeviceLibrary/`

Core device communication library compiled as a DLL for reuse across applications.

**Contents**:
- **Communication/** - VISA communication managers (real and mock)
  - `IVisaCommunicationManager.cs` - Communication interface
  - `VisaCommunicationManager.cs` - Real hardware communication using NI-VISA
- **Simulation/** - Mock communication for testing without hardware
  - `MockVisaCommunicationManager.cs` - Simulated device communication
  - `SimulatedDeviceState.cs` - Simulated device state management
- **Commands/** - SCPI command construction
  - `IScpiCommandBuilder.cs` - Command builder interface
  - `ScpiCommandBuilder.cs` - Builds SCPI commands from parameters
- **Parsing/** - SCPI response parsing
  - `IScpiResponseParser.cs` - Parser interface
  - `ScpiResponseParser.cs` - Parses SCPI responses into objects
- **Services/** - High-level application services
  - `IPowerMeterService.cs` - Service interface
  - `PowerMeterService.cs` - Coordinates device operations
- **Validation/** - Input validation
  - `IInputValidator.cs` - Validator interface
  - `InputValidator.cs` - Validates user input against device specs
- **Models/** - Data models and enumerations
  - `PowerMeasurement.cs` - Power measurement data
  - `FrequencyResponse.cs` - Frequency query response
  - `SensorInfo.cs` - Sensor information
  - `CalibrationStatus.cs` - Calibration status
  - `DeviceIdentity.cs` - Device identification
  - `SystemStatus.cs` - System status
  - `OperationResult.cs` - Operation result wrapper
  - `CommandResult.cs` - Command execution result
  - `ValidationResult.cs` - Validation result
  - `DeviceError.cs` - Device error information
  - Enums: `FrequencyUnit`, `PowerUnit`, `CalibrationMode`

**Key Features**:
- Synchronous API (no async/await due to .NET 4.0 limitation)
- SCPI protocol implementation for Tegam 1830A
- Mock communication for testing without hardware
- Comprehensive input validation
- Event-driven architecture for device state changes

### 2. Tegam.1830A.WinFormsUI (.NET Framework 4.8)
**Location**: `Tegam.1830A.WinFormsUI/`

Windows Forms application providing user interface for device control.

**Contents**:
- **Forms/** - User interface panels
  - `MainForm.cs` - Main application window with tabbed interface
  - `PowerMeasurementPanel.cs` - Power measurement controls
  - `FrequencyConfigurationPanel.cs` - Frequency configuration
  - `SensorManagementPanel.cs` - Sensor selection and management
  - `CalibrationPanel.cs` - Calibration controls
  - `DataLoggingPanel.cs` - Data logging configuration
- **Controllers/** - UI controllers (MVC pattern)
  - `MainFormController.cs` - Main form logic
  - `PowerMeasurementController.cs` - Power measurement logic
  - `FrequencyConfigurationController.cs` - Frequency logic
  - `SensorManagementController.cs` - Sensor management logic
  - `CalibrationController.cs` - Calibration logic
  - `DataLoggingController.cs` - Data logging logic
- **Program.cs** - Application entry point with dependency injection setup

**Key Features**:
- Tabbed interface for different device functions
- Dependency injection using Microsoft.Extensions.DependencyInjection
- Async UI operations using Task.Run() wrappers for responsiveness
- Real-time device status updates via events
- Input validation with visual feedback

### 3. Tegam.1830A.Tests (.NET Framework 4.8)
**Location**: `Tegam.1830A.Tests/`

Unit test project for comprehensive testing of the device library.

**Contents**:
- **Unit/** - Unit tests for individual components
  - `ScpiCommandBuilderTests.cs` - Command builder tests
  - `ScpiResponseParserTests.cs` - Response parser tests
  - `InputValidatorTests.cs` - Input validation tests
  - `MockCommunicationManagerTests.cs` - Mock communication tests
- **Integration/** - Integration tests using mock communication
  - `PowerMeterServiceIntegrationTests.cs` - End-to-end workflow tests
- **PropertyBased/** - Property-based tests using FsCheck
  - `PropertyBasedTests.cs` - Roundtrip and conversion tests

**Test Frameworks**:
- NUnit 3.13.3
- Moq 4.18.4 (mocking)
- FsCheck 2.16.5 (property-based testing)

## Prerequisites

### For Development
- Visual Studio 2022 or later
- .NET Framework 4.0 SDK (for Device Library)
- .NET Framework 4.8 SDK (for UI and Tests)
- NI-VISA Runtime (for hardware communication)

### For Running the Application
- Windows 7 or later (32-bit or 64-bit)
- .NET Framework 4.8 Runtime
- NI-VISA Runtime (download from National Instruments website)
- Network connectivity to Tegam 1830A device

## Building the Solution

### Using Visual Studio
1. Open `Tegam.1830A.sln` in Visual Studio
2. Build > Build Solution (or press Ctrl+Shift+B)

### Using Command Line
```bash
# Navigate to solution directory
cd path\to\Tegam.1830A

# Build using MSBuild
"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" Tegam.1830A.sln /t:Build /p:Configuration=Release /p:ResolveNuGetPackages=false
```

**Note**: The `/p:ResolveNuGetPackages=false` flag is required due to MSBuild 17.0 incompatibility with .NET Framework 4.0 projects.

## Running the Application

### Using Mock Communication (No Hardware Required)
1. Build the solution
2. Navigate to `Tegam.1830A.WinFormsUI\bin\Debug\` (or `Release\`)
3. Run `Tegam.1830A.WinFormsUI.exe`
4. The application will use mock communication by default if configured in `Program.cs`

### Using Real Hardware
1. Ensure NI-VISA Runtime is installed
2. Connect Tegam 1830A to network
3. Note the device's IP address
4. Configure `Program.cs` to use `VisaCommunicationManager` instead of `MockVisaCommunicationManager`
5. Build and run the application
6. Enter the device IP address in the connection panel
7. Click "Connect"

## Application Usage

### Connecting to Device
1. Enter the device IP address (e.g., `192.168.1.100`)
2. Click "Connect"
3. Wait for connection confirmation
4. Device information will be displayed in the status bar

### Power Measurement
1. Navigate to "Power Measurement" tab
2. Set frequency and select sensor
3. Click "Measure" for single measurement
4. Click "Measure Multiple" for continuous measurements

### Frequency Configuration
1. Navigate to "Frequency Configuration" tab
2. Enter frequency value and select unit (Hz, kHz, MHz, GHz)
3. Click "Set Frequency"
4. Click "Query Frequency" to verify

### Sensor Management
1. Navigate to "Sensor Management" tab
2. Select sensor ID (1-4)
3. Click "Select Sensor"
4. Click "Query Available Sensors" to see all sensors

### Calibration
1. Navigate to "Calibration" tab
2. Select calibration mode (Internal or External)
3. Click "Start Calibration"
4. Wait for calibration to complete

### Data Logging
1. Navigate to "Data Logging" tab
2. Click "Browse" to select log file location
3. Click "Start Logging"
4. Measurements will be recorded to file
5. Click "Stop Logging" when done

## Data Storage

### Application Configuration
**Location**: `%AppData%\Tegam1830A\settings.xml`

**Stored Data**:
- Last used IP address
- Default frequency unit preference
- Default power unit preference
- Default log file path
- Other user preferences

### Log Files
**Location**: `%AppData%\Tegam1830A\Logs\`

**Files**:
- `error.log` - Application error log

### Measurement CSV Files
**Default Location (Debug Mode)**: `Tegam.1830A.WinFormsUI\bin\Debug\measurements.csv`

**Default Location (Release Mode)**: `Tegam.1830A.WinFormsUI\bin\Release\measurements.csv`

**User-Specified Location**: When using the "Browse" button in the Data Logging panel, files can be saved anywhere

**Important Notes**:
- If you run the application from Visual Studio in Debug mode, the CSV file will be saved to the Debug folder by default
- If you don't specify a full path, the file is saved relative to the application's working directory
- Use the "Browse" button to select a specific location (recommended)

**Measurement Log Format**:
```csv
Timestamp,Frequency (Hz),Power (dBm),Sensor ID
2026-03-22 10:30:15.123,2400000000,-15.5,1
2026-03-22 10:30:16.456,2400000000,-15.3,1
```

### Application Settings
**Location**: `Tegam.1830A.WinFormsUI\app.config`

**Stored Data**:
- Connection timeout settings
- Default device configuration
- Application-level settings

## Architecture

### Layered Architecture
```
┌─────────────────────────────────────┐
│      WinForms UI (.NET 4.8)        │
│  ┌──────────┐  ┌──────────────┐    │
│  │  Forms   │  │ Controllers  │    │
│  └──────────┘  └──────────────┘    │
└─────────────────┬───────────────────┘
                  │ References
┌─────────────────▼───────────────────┐
│   Device Library DLL (.NET 4.0)    │
│  ┌──────────┐  ┌──────────────┐    │
│  │ Services │  │ Communication│    │
│  ├──────────┤  ├──────────────┤    │
│  │ Commands │  │   Parsing    │    │
│  ├──────────┤  ├──────────────┤    │
│  │Validation│  │    Models    │    │
│  └──────────┘  └──────────────┘    │
└─────────────────┬───────────────────┘
                  │ Uses
┌─────────────────▼───────────────────┐
│         NI-VISA Library            │
│    (Hardware Communication)         │
└─────────────────┬───────────────────┘
                  │ TCPIP/SCPI
┌─────────────────▼───────────────────┐
│      Tegam 1830A Device            │
└─────────────────────────────────────┘
```

### Key Design Patterns
- **Dependency Injection**: Services and controllers injected via constructor
- **Repository Pattern**: `IVisaCommunicationManager` abstracts hardware access
- **Service Layer**: `IPowerMeterService` provides high-level API
- **Builder Pattern**: `IScpiCommandBuilder` constructs SCPI commands
- **Observer Pattern**: Events for device state changes

## Troubleshooting

### "Could not load file or assembly 'Tegam.1830A.DeviceLibrary'"
- Ensure Device Library DLL is in the same directory as the executable
- Rebuild the solution

### "NI-VISA not found" or Communication Errors
- Install NI-VISA Runtime from National Instruments website
- Verify device is connected to network
- Check IP address is correct
- Verify firewall allows TCPIP communication

### UI Freezes During Operations
- This should not happen as operations are wrapped in Task.Run()
- If it does, check that controllers are using async wrappers correctly

### "The referenced component 'FSharp.Core' could not be found"
- This warning is expected for the test project if packages aren't restored
- Run `nuget restore` or restore packages in Visual Studio
- Tests will still build without FsCheck if needed

## Development Notes

### .NET Framework 4.0 Limitation
The Device Library targets .NET Framework 4.0, which does not support async/await. All methods are synchronous. The UI project (.NET 4.8) wraps these synchronous calls in `Task.Run()` to maintain UI responsiveness.

### Mock Communication
The `MockVisaCommunicationManager` simulates device behavior for testing and development without physical hardware. It maintains simulated device state and generates realistic responses to SCPI commands.

### SCPI Protocol
The application uses SCPI (Standard Commands for Programmable Instruments) protocol for device communication. Commands are built by `ScpiCommandBuilder` and responses are parsed by `ScpiResponseParser`.

## Contributing

When contributing to this project:
1. Follow existing code style and patterns
2. Add unit tests for new functionality
3. Update this README if adding new features
4. Test with both mock and real hardware (if available)

## License

[Specify license here]

## Support

For issues or questions:
- Check the error log: `%AppData%\Tegam1830A\Logs\error.log`
- Review the design document: `.kiro/specs/tegam-1830a-control-app/design.md`
- Contact [support contact]

## Version History

### Version 1.0.0 (Current)
- Initial release
- Full SCPI protocol implementation
- Mock communication for testing
- WinForms UI with tabbed interface
- Data logging support
- Comprehensive unit tests
