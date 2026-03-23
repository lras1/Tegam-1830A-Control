# NI-VISA Setup for Tegam.1830A.DeviceLibrary

## Overview
The Tegam.1830A.DeviceLibrary requires the National Instruments VISA library for TCPIP communication with the Tegam 1830A device.

## Installation Instructions

### Option 1: Using NuGet Package Manager (Recommended)
1. Open Package Manager Console in Visual Studio
2. Run the following command:
   ```
   Install-Package NationalInstruments.Visa -Version 20.0.0
   ```

### Option 2: Manual Installation
1. Download the NI-VISA runtime from National Instruments website
2. Install the NI-VISA runtime on your system
3. Add a reference to the NI-VISA assembly in the project

## Compatibility Notes
- The project targets .NET Framework 4.0
- NationalInstruments.Visa version 20.0.0 or compatible is required
- Ensure NI-VISA runtime is installed on the development machine

## Troubleshooting
If you encounter issues with NuGet package resolution:
1. Clear the NuGet cache: `nuget locals all -clear`
2. Restore packages: `dotnet restore`
3. Rebuild the solution

## References
- [National Instruments VISA Documentation](https://www.ni.com/en-us/support/documentation/supplemental/06/visa-overview.html)
- [NI-VISA NuGet Package](https://www.nuget.org/packages/NationalInstruments.Visa/)
