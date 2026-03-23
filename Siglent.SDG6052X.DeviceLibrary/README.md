# Siglent.SDG6052X.DeviceLibrary

Device communication library for Siglent SDG6052X arbitrary waveform generator.

## Project Information

- **Target Framework**: .NET Framework 4.0
- **Project Type**: Class Library (DLL)
- **Purpose**: Core device communication logic, SCPI command processing, and data models

## Build Instructions

To build the project:

```bash
dotnet msbuild Siglent.SDG6052X.DeviceLibrary.csproj /t:Build /p:Configuration=Debug
```

Or for Release build:

```bash
dotnet msbuild Siglent.SDG6052X.DeviceLibrary.csproj /t:Build /p:Configuration=Release
```

## Project Structure

The project will contain the following folder structure (to be created in subsequent tasks):

- **Communication/** - VISA communication managers (real and mock)
- **Commands/** - SCPI command builders
- **Parsing/** - SCPI response parsers
- **Services/** - High-level application services
- **Validation/** - Input validators
- **Models/** - Data models and enums
- **Simulation/** - Mock communication components

## Dependencies

### NI-VISA Runtime (Required)
This library requires the National Instruments VISA (NI-VISA) runtime to be installed:
- **Package**: NationalInstruments.Visa
- **Minimum Version**: 20.0.0
- **Compatibility**: .NET Framework 4.0

**Installation**: See [NI-VISA-SETUP.md](NI-VISA-SETUP.md) for detailed installation instructions.

The following assemblies are referenced from the NI-VISA installation:
- `Ivi.Visa.dll` - IVI Foundation VISA .NET interface
- `NationalInstruments.Common.dll` - NI common utilities
- `NationalInstruments.Visa.dll` - NI VISA .NET implementation

## Notes

This library is designed to be referenced by:
- WinForms UI Application (.NET Framework 4.8)
- Unit Test Project (.NET Framework 4.8)
