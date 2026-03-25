# CalibrationTuning Connection Diagnostics

## Issue Summary
Neither the Tegam 1830A Power Meter nor the Siglent SDG6052X Signal Generator connect in simulation mode in the CalibrationTuning application.

## Root Cause Analysis

### 1. Simulation Mode Detection
The application checks for simulation mode via:
- Command line argument: `--simulate` or `/simulate`
- Environment variable: `CALIBRATION_SIMULATE=true`

**Verification needed:** Is simulation mode actually being enabled?

### 2. Mock Communication Manager Registration
When simulation mode is enabled, the DI container registers:
- `Tegam._1830A.DeviceLibrary.Simulation.MockVisaCommunicationManager`
- `Siglent.SDG6052X.DeviceLibrary.Simulation.MockVisaCommunicationManager`

### 3. Connection Flow
1. User clicks "Connect" button
2. `ConnectionPanel` calls `TuningController.ConnectPowerMeterAsync(ipAddress)`
3. `TuningController` calls `PowerMeterService.Connect(ipAddress)`
4. `PowerMeterService` formats resource name as `TCPIP::{ipAddress}::INSTR`
5. `PowerMeterService` calls `MockVisaCommunicationManager.Connect(resourceName, 5000)`
6. `MockVisaCommunicationManager.Connect()` should return `true`
7. `PowerMeterService` calls `MockVisaCommunicationManager.GetDeviceIdentity()`
8. `MockVisaCommunicationManager.GetDeviceIdentity()` should return "Tegam,1830A,SN123456,1.0.0"

## Potential Issues

### Issue 1: Simulation Mode Not Enabled
**Symptom:** Real VISA communication managers are being used instead of mocks
**Fix:** Ensure simulation mode is enabled via environment variable or command line

### Issue 2: GetDeviceIdentity() Returns Null
**Symptom:** Connection fails after `Connect()` succeeds
**Cause:** `GetDeviceIdentity()` checks `IsConnected` property
**Fix:** Verify `_isConnected` is set to `true` in `Connect()` method

### Issue 3: DI Container Ambiguity
**Symptom:** Wrong communication manager injected
**Cause:** Multiple implementations of same interface
**Fix:** Already addressed with explicit factory methods in Program.cs

## Diagnostic Steps

### Step 1: Verify Simulation Mode
Add debug output to `Program.cs` `CheckSimulationMode()`:
```csharp
private static bool CheckSimulationMode()
{
    // Check command line arguments
    string[] args = Environment.GetCommandLineArgs();
    System.Diagnostics.Debug.WriteLine($"[SimMode] Command line args: {string.Join(", ", args)}");
    
    foreach (string arg in args)
    {
        if (arg.Equals("--simulate", StringComparison.OrdinalIgnoreCase) ||
            arg.Equals("/simulate", StringComparison.OrdinalIgnoreCase))
        {
            System.Diagnostics.Debug.WriteLine("[SimMode] Simulation mode ENABLED via command line");
            return true;
        }
    }

    // Check environment variable
    string envVar = Environment.GetEnvironmentVariable("CALIBRATION_SIMULATE");
    System.Diagnostics.Debug.WriteLine($"[SimMode] Environment variable CALIBRATION_SIMULATE={envVar}");
    
    if (!string.IsNullOrEmpty(envVar) && 
        (envVar.Equals("true", StringComparison.OrdinalIgnoreCase) || envVar == "1"))
    {
        System.Diagnostics.Debug.WriteLine("[SimMode] Simulation mode ENABLED via environment variable");
        return true;
    }

    System.Diagnostics.Debug.WriteLine("[SimMode] Simulation mode DISABLED");
    return false;
}
```

### Step 2: Verify Mock Registration
Already has debug output in `ConfigureServices()`:
```csharp
System.Diagnostics.Debug.WriteLine($"[DI] PowerMeterService - CommManager type: {commManager.GetType().FullName}");
System.Diagnostics.Debug.WriteLine($"[DI] SignalGeneratorService - CommManager type: {commManager.GetType().FullName}");
```

Expected output in simulation mode:
```
[DI] PowerMeterService - CommManager type: Tegam._1830A.DeviceLibrary.Simulation.MockVisaCommunicationManager
[DI] SignalGeneratorService - CommManager type: Siglent.SDG6052X.DeviceLibrary.Simulation.MockVisaCommunicationManager
```

### Step 3: Verify Connection Flow
Already has debug output in `TuningController`:
```csharp
System.Diagnostics.Debug.WriteLine($"[TuningController] Attempting to connect to power meter at {powerMeterIp}");
System.Diagnostics.Debug.WriteLine($"[TuningController] Power meter connection result: {powerMeterConnected}");
```

### Step 4: Add Debug Output to Mock Connect
Add to `MockVisaCommunicationManager.Connect()`:
```csharp
public bool Connect(string resourceName, int timeout = 5000)
{
    System.Diagnostics.Debug.WriteLine($"[MockVisa-Tegam] Connect called with resourceName={resourceName}, timeout={timeout}");
    
    if (string.IsNullOrWhiteSpace(resourceName))
        throw new ArgumentException("Resource name cannot be null or empty.", nameof(resourceName));

    if (timeout < 0)
        throw new ArgumentException("Timeout cannot be negative.", nameof(timeout));

    try
    {
        // Simulate connection delay
        Thread.Sleep(100);

        // Check if we should simulate connection loss
        if (_deviceState.ShouldSimulateConnectionLoss)
        {
            System.Diagnostics.Debug.WriteLine("[MockVisa-Tegam] Simulating connection loss");
            OnCommunicationError("Simulated connection loss.");
            return false;
        }

        // Check if we should simulate timeout
        if (_deviceState.ShouldSimulateTimeout)
        {
            System.Diagnostics.Debug.WriteLine("[MockVisa-Tegam] Simulating timeout");
            OnCommunicationError("Simulated timeout.");
            return false;
        }

        _isConnected = true;
        System.Diagnostics.Debug.WriteLine("[MockVisa-Tegam] Connection successful, IsConnected=true");
        return true;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[MockVisa-Tegam] Connection exception: {ex}");
        OnCommunicationError(string.Format("Connection failed: {0}", ex.Message), ex);
        return false;
    }
}
```

## Quick Fix: Enable Simulation Mode

### Option 1: Environment Variable (Recommended)
1. Open System Properties → Advanced → Environment Variables
2. Add user variable: `CALIBRATION_SIMULATE=true`
3. Restart Visual Studio and the application

### Option 2: Command Line Argument
1. In Visual Studio, right-click CalibrationTuning project → Properties
2. Go to Debug tab
3. Add to "Command line arguments": `--simulate`
4. Save and run

### Option 3: Batch File
Create `RunSimulation.bat` in CalibrationTuning folder:
```batch
@echo off
set CALIBRATION_SIMULATE=true
start "" "bin\Debug\CalibrationTuning.exe"
```

## Expected Behavior in Simulation Mode

### Power Meter (Tegam 1830A)
- Connect should succeed immediately (100ms delay)
- Device identity: "Tegam,1830A,SN123456,1.0.0"
- Power measurements: Random values between -50 dBm and +20 dBm
- All SCPI commands accepted and simulated

### Signal Generator (Siglent SDG6052X)
- Connect should succeed immediately
- Device identity: "Siglent,SDG6052X,SDG0XEAQ1R0001,1.01.01.33R1B5"
- All waveform commands accepted and simulated
- Output state changes tracked

## Troubleshooting

### If connections still fail after enabling simulation mode:
1. Check Debug Output window in Visual Studio for diagnostic messages
2. Look for "[SimMode]" messages to confirm simulation mode is enabled
3. Look for "[DI]" messages to confirm mock managers are registered
4. Look for "[TuningController]" messages to see connection attempts
5. Look for "[MockVisa-Tegam]" and "[MockVisa-Siglent]" messages to see mock behavior

### Common Issues:
- **Environment variable not set correctly**: Restart VS after setting
- **Wrong communication manager type**: Check DI registration output
- **Exception during connection**: Check for stack traces in Debug Output
- **GetDeviceIdentity() returns null**: Check IsConnected property is set
