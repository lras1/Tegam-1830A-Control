# Calibration Tuning Application

A Windows Forms application for automated RF power calibration using the Tegam 1830A Power Meter and Siglent SDG6052X Signal Generator.

## Features

- Device connection management for both instruments
- Automated tuning algorithm to achieve target power levels
- Real-time status monitoring and progress tracking
- Data logging to CSV format
- Configuration persistence
- Safety limits and validation

## Running the Application

### Normal Mode (Real Hardware)

Simply run the application:

```bash
CalibrationTuning.exe
```

The application will attempt to connect to real hardware devices via VISA.

### Simulation Mode (Mock Devices)

For testing without physical hardware, you can run in simulation mode using either:

**Option 1: Command Line Argument**
```bash
CalibrationTuning.exe --simulate
```

**Option 2: Environment Variable**
```bash
set CALIBRATION_SIMULATE=true
CalibrationTuning.exe
```

In simulation mode:
- Mock devices respond to all commands without requiring physical hardware
- Simulated measurements return realistic values
- All UI functionality works identically to real mode
- Perfect for development, testing, and demonstrations

## Configuration

Device IP addresses and tuning parameters are automatically saved to:
```
%AppData%\CalibrationTuning\settings.json
```

## Data Logging

Measurement data is logged in CSV format with columns:
- Timestamp
- Iteration
- Frequency_Hz
- Voltage
- Power_dBm
- Status

## Requirements

- .NET Framework 4.8
- Windows operating system
- For real hardware mode: VISA drivers installed
- For simulation mode: No additional requirements

## Development

The application uses dependency injection with separate mock implementations for testing:
- `Tegam.1830A.DeviceLibrary.Simulation.MockVisaCommunicationManager`
- `Siglent.SDG6052X.DeviceLibrary.Simulation.MockVisaCommunicationManager`

This allows the same codebase to work with both real and simulated devices.
