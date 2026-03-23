# Siglent SDG6052X Control Application - Deployment Package

## Contents

This deployment package contains the Siglent SDG6052X Control Application and all required dependencies.

### Main Application
- `Siglent.SDG6052X.WinFormsUI.exe` - Main application executable
- `Siglent.SDG6052X.WinFormsUI.exe.config` - Application configuration file

### Device Library
- `Siglent.SDG6052X.DeviceLibrary.dll` - Core device communication library

### Dependencies
- `Microsoft.Extensions.DependencyInjection.dll` - Dependency injection framework
- `Microsoft.Extensions.DependencyInjection.Abstractions.dll` - DI abstractions
- `Microsoft.Bcl.AsyncInterfaces.dll` - Async interfaces support
- `System.Threading.Tasks.Extensions.dll` - Task extensions
- `System.Runtime.CompilerServices.Unsafe.dll` - Runtime support
- `System.Configuration.ConfigurationManager.dll` - Configuration management

## System Requirements

- Windows 7 or later
- .NET Framework 4.8 or later
- Network connectivity to the Siglent SDG6052X device (for real device mode)

## Installation

1. Extract all files to a directory of your choice
2. Ensure .NET Framework 4.8 is installed on your system
3. Run `Siglent.SDG6052X.WinFormsUI.exe`

## Configuration

The application can run in two modes:

### Simulation Mode (Default)
- No physical device required
- Simulates device responses for testing and development
- Configured in `Siglent.SDG6052X.WinFormsUI.exe.config`:
  ```xml
  <add key="UseSimulation" value="true" />
  ```

### Real Device Mode
- Requires Siglent SDG6052X device connected to network
- Requires NI-VISA runtime installed
- Change configuration to:
  ```xml
  <add key="UseSimulation" value="false" />
  ```

## Usage

1. Launch the application
2. Enter the IP address of your SDG6052X device (or use default for simulation)
3. Click "Connect"
4. Use the tabs to configure:
   - Waveform parameters (frequency, amplitude, offset, phase)
   - Modulation settings (AM, FM, PM, PWM, FSK, ASK, PSK)
   - Sweep configuration (linear/logarithmic sweeps)
   - Burst mode (N-Cycle, Gated)
   - Arbitrary waveform management

## Features

- Dual-channel waveform generation control
- Multiple waveform types: Sine, Square, Ramp, Pulse, Noise, Arbitrary, DC
- Advanced modulation capabilities
- Frequency sweep functionality
- Burst mode operation
- Arbitrary waveform upload and management
- Real-time device state querying
- Input validation with visual feedback
- Simulation mode for offline testing

## Support

For issues or questions, refer to the project documentation or contact support.

## Version

Version: 1.0.0
Build Date: March 21, 2026
