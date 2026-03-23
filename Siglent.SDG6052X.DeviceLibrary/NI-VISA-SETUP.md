# NI-VISA Setup Instructions

## Overview
This project requires the National Instruments VISA (NI-VISA) runtime to be installed on the development and deployment machines. NI-VISA provides the .NET assemblies required for TCPIP communication with the Siglent SDG6052X device.

## Installation Steps

### 1. Download NI-VISA Runtime
- Visit the [National Instruments NI-VISA download page](https://www.ni.com/en-us/support/downloads/drivers/download.ni-visa.html)
- Download NI-VISA version 20.0 or later (compatible with .NET Framework 4.0)
- The installer is approximately 1GB in size

### 2. Install NI-VISA
- Run the NI-VISA installer
- Follow the installation wizard
- The installer will place the required .NET assemblies in:
  - `C:\Program Files\IVI Foundation\VISA\Microsoft.NET\Framework32\v4.0.30319\`
  - `C:\Program Files\National Instruments\MeasurementStudioVS2010\DotNET\Assemblies\Current\`

### 3. Verify Installation
After installation, the following assemblies should be available:
- `Ivi.Visa.dll` - IVI Foundation VISA .NET interface
- `NationalInstruments.Common.dll` - NI common utilities
- `NationalInstruments.Visa.dll` - NI VISA .NET implementation

### 4. Build the Project
Once NI-VISA is installed, the project should build successfully. The .csproj file references the assemblies from their standard installation locations.

## Alternative: Manual Assembly References
If NI-VISA is installed in a non-standard location, you can manually update the assembly references:

1. Open the project in Visual Studio
2. Right-click on References in Solution Explorer
3. Select "Add Reference..."
4. Browse to the NI-VISA installation directory
5. Add references to:
   - Ivi.Visa.dll
   - NationalInstruments.Common.dll
   - NationalInstruments.Visa.dll

## Deployment Requirements
Applications using this library require NI-VISA runtime to be installed on the target machine. The runtime can be distributed with your application or users can download it from the National Instruments website.

## Version Compatibility
- **Minimum NI-VISA Version**: 20.0.0
- **Target Framework**: .NET Framework 4.0
- **Supported OS**: Windows 7 or later

## Troubleshooting

### Build Error: "Could not resolve reference"
- Ensure NI-VISA runtime is installed
- Verify the installation paths in the .csproj file match your installation
- Check that you're building for x86 or AnyCPU (not x64 only)

### Runtime Error: "Could not load file or assembly"
- Ensure NI-VISA runtime is installed on the deployment machine
- Verify the correct version of NI-VISA is installed
- Check that the application is running with appropriate permissions

## Additional Resources
- [NI-VISA .NET Library Documentation](https://www.ni.com/en-us/support/documentation/supplemental/15/national-instruments-visa--net-library.html)
- [NI-VISA User Manual](https://www.ni.com/pdf/manuals/370423a.pdf)
- [IVI VISA .NET Specification](https://www.ivifoundation.org/)
