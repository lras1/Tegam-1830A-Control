# CalibrationTuning Application

A Windows Forms application for automated calibration and tuning of RF signal generators using power meter feedback.

## Features

- **Dual Device Control**: Integrates Tegam 1830A RF Power Meter and Siglent SDG6052X Signal Generator
- **Automated Tuning**: Iterative voltage adjustment to achieve target power setpoint
- **Safety Limits**: Configurable voltage and iteration limits
- **Data Logging**: CSV export of tuning sessions
- **Configuration Persistence**: Settings saved to %AppData%\CalibrationTuning
- **Simulation Mode**: Test without physical hardware

## System Requirements

- Windows 7 or later
- .NET Framework 4.8
- Visual Studio 2022 (for development)

## Quick Start

### Running in Simulation Mode (No Hardware Required)

**Option 1: Using Batch File**
1. Build the project in Debug mode
2. Run `RunSimulation.bat` from the CalibrationTuning folder
3. Click Connect buttons for both devices
4. Configure tuning parameters and start tuning

**Option 2: Using Environment Variable**
1. Set environment variable: `CALIBRATION_SIMULATE=true`
2. Restart Visual Studio
3. Run the application
4. Click Connect buttons for both devices

**Option 3: Using Command Line**
1. Build the project
2. Run from command line: `CalibrationTuning.exe --simulate`
3. Click Connect buttons for both devices

### Running with Real Hardware

1. Ensure both devices are powered on and connected to the network
2. Note the IP addresses of both devices
3. Run the application (without simulation mode)
4. Enter the correct IP addresses in the Connection tab
5. Click Connect for each device
6. Wait for "Status: Connected" confirmation

## Application Structure

### Tabs

1. **Connection**: Connect/disconnect devices, view connection status
2. **Tuning**: Configure and execute tuning operations
3. **Status**: View real-time tuning progress and statistics
4. **Chart**: Visualization (placeholder - not implemented)

### Tuning Parameters

- **Frequency**: Signal frequency (1 Hz to 500 GHz)
- **Initial Voltage**: Starting voltage amplitude (0.001-10V)
- **Setpoint**: Target power level in dBm (-100 to +100)
- **Tolerance**: Acceptable deviation from setpoint (0.01-10 dB)
- **Voltage Step**: Adjustment increment per iteration (0.001-1V)
- **Min/Max Voltage**: Safety limits (0-10V)
- **Max Iterations**: Maximum tuning attempts (1-10000)

### Tuning Algorithm

1. Set signal generator to initial frequency and voltage
2. Measure power with power meter
3. Compare measured power to setpoint
4. If within tolerance, tuning complete
5. If too low, increase voltage by step amount
6. If too high, decrease voltage by step amount
7. Repeat until within tolerance or max iterations reached

## Configuration Files

### Application Settings
Location: `%AppData%\CalibrationTuning\settings.json`

Contains:
- Last used device IP addresses
- Last used tuning parameters
- Safety limit settings

### Data Logs
Location: `%AppData%\CalibrationTuning\Logs\`

CSV format with columns:
- Timestamp
- Iteration
- Frequency_Hz
- Voltage
- Power_dBm
- Status

## Programmatic API

The application can be controlled programmatically via the `ITuningController` interface. See `PROGRAMMATIC_API_EXAMPLE.md` for details on:
- Connecting devices from code
- Setting tuning parameters
- Monitoring tuning progress via events
- Accessing data logs

## Troubleshooting

### Connections Fail in Simulation Mode

1. **Verify simulation mode is enabled**:
   - Check Debug Output window in Visual Studio
   - Look for `[SimMode] ✓ Simulation mode ENABLED` message
   - If not present, simulation mode is not active

2. **Enable simulation mode**:
   - Set environment variable `CALIBRATION_SIMULATE=true`
   - OR run with `--simulate` command line argument
   - OR use `RunSimulation.bat`

3. **Check Debug Output**:
   - Open View → Output in Visual Studio
   - Select "Debug" from the dropdown
   - Look for diagnostic messages:
     - `[SimMode]` - Simulation mode detection
     - `[DI]` - Dependency injection registration
     - `[TuningController]` - Connection attempts
     - `[MockVisa-Tegam]` - Tegam mock behavior
     - `[MockVisa-Siglent]` - Siglent mock behavior

### Connections Fail with Real Hardware

1. **Verify network connectivity**:
   - Ping the device IP addresses
   - Ensure devices are on the same network
   - Check firewall settings

2. **Verify device configuration**:
   - Ensure VISA drivers are installed
   - Check device network settings
   - Verify SCPI over TCP/IP is enabled

3. **Check IP addresses**:
   - Tegam 1830A default: Check device display
   - Siglent SDG6052X default: Check device menu

### Tuning Doesn't Start

1. **Verify both devices are connected**:
   - Both status labels should show "Connected" in green
   - If not, connect devices first

2. **Check tuning parameters**:
   - All parameters must be within valid ranges
   - NumericUpDown controls enforce limits automatically

3. **Check for errors**:
   - Look at Status tab for error messages
   - Check Debug Output window for exceptions

## Development

### Project Structure

```
CalibrationTuning/
├── Controllers/
│   ├── TuningController.cs          # Main tuning logic
│   ├── ConfigurationController.cs   # Settings persistence
│   └── DataLoggingController.cs     # CSV logging
├── Models/
│   ├── TuningParameters.cs          # Tuning configuration
│   ├── TuningResult.cs              # Tuning outcome
│   ├── TuningState.cs               # State machine
│   └── DeviceConfiguration.cs       # Device settings
├── UserControls/
│   ├── ConnectionPanel.cs           # Device connection UI
│   ├── TuningPanel.cs               # Tuning configuration UI
│   └── StatusPanel.cs               # Progress display UI
├── Events/
│   ├── TuningStateChangedEventArgs.cs
│   ├── TuningProgressEventArgs.cs
│   └── TuningCompletedEventArgs.cs
├── MainForm.cs                      # Main application window
└── Program.cs                       # Entry point, DI setup
```

### Dependencies

- **Tegam.1830A.DeviceLibrary**: Power meter communication
- **Siglent.SDG6052X.DeviceLibrary**: Signal generator communication
- **Microsoft.Extensions.DependencyInjection**: IoC container
- **Newtonsoft.Json**: Configuration serialization

### Building

```bash
# Restore NuGet packages
nuget restore CalibrationTuning.sln

# Build Debug
msbuild CalibrationTuning.sln /p:Configuration=Debug

# Build Release
msbuild CalibrationTuning.sln /p:Configuration=Release
```

### Testing

Unit tests are located in `CalibrationTuning.Tests` project:
- `TuningControllerTests.cs`: Controller logic tests
- `MainFormConfigurationTests.cs`: Configuration persistence tests

Run tests:
```bash
nunit3-console CalibrationTuning.Tests\bin\Debug\CalibrationTuning.Tests.dll
```

## License

[Your License Here]

## Support

For issues or questions:
1. Check the troubleshooting section above
2. Review `CONNECTION_DIAGNOSTICS.md` for detailed diagnostics
3. Check Debug Output window for diagnostic messages
4. Review `PROGRAMMATIC_API_EXAMPLE.md` for API usage

## Version History

### v1.0.0 (2026-03-24)
- Initial release
- Dual device integration
- Automated tuning algorithm
- Simulation mode support
- Configuration persistence
- Data logging
