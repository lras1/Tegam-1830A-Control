# Signal Generator Connection Debug Checklist

## Issue
`ConnectSignalGeneratorAsync` returns `false`

## Debug Steps

### Step 1: Check Simulation Mode
Look for this in Debug Output:
```
[SimMode] ✓ Simulation mode ENABLED via ...
```

**If you see:**
- `[SimMode] ✗ Simulation mode DISABLED` → Simulation mode is not enabled
- **Fix**: Set environment variable `CALIBRATION_SIMULATE=true` and restart VS

### Step 2: Check DI Registration
Look for this in Debug Output:
```
[DI] SignalGeneratorService - CommManager type: Siglent.SDG6052X.DeviceLibrary.Simulation.MockVisaCommunicationManager
```

**If you see:**
- `...Communication.VisaCommunicationManager` (without "Simulation") → Real hardware manager is being used
- **Fix**: Ensure simulation mode is enabled (see Step 1)

### Step 3: Check Connection Attempt
Look for these messages:
```
[TuningController] Attempting to connect to signal generator at 192.168.1.101
[SignalGeneratorService] ConnectAsync called with ipAddress='192.168.1.101'
[SignalGeneratorService] Resource name: TCPIP::192.168.1.101::INSTR
[SignalGeneratorService] Calling _communicationManager.Connect()...
```

**If missing:** Connection attempt didn't start - check UI code

### Step 4: Check Mock Connection
Look for these messages:
```
[MockVisa-Siglent] Connect called: resourceName='TCPIP::192.168.1.101::INSTR', timeout=5000ms
[MockVisa-Siglent] ✓ Connection successful, IsConnected=true
```

**If you see:**
- `[MockVisa-Siglent] ✗ Resource name is null or empty` → Resource name not passed correctly
- No `[MockVisa-Siglent]` messages at all → Mock is not being called (DI issue)

### Step 5: Check GetDeviceIdentity
Look for these messages:
```
[SignalGeneratorService] Getting device identity...
[MockVisa-Siglent] GetDeviceIdentity called, IsConnected=true
[MockVisa-Siglent] ✓ GetDeviceIdentity returning: Siglent,SDG6052X,SDG0XEAQ1R0001,1.01.01.33R1B5
[SignalGeneratorService] Identity string: Siglent,SDG6052X,SDG0XEAQ1R0001,1.01.01.33R1B5
```

**If you see:**
- `GetDeviceIdentity called, IsConnected=false` → Connection didn't set IsConnected properly
- Exception during parsing → Identity string format issue

### Step 6: Check Final Result
Look for these messages:
```
[SignalGeneratorService] ✓ Connected to SDG6052X
[TuningController] Signal generator connection result: true
```

**If you see:**
- `[SignalGeneratorService] ✗ Connection failed` → Connect returned false
- `[SignalGeneratorService] ✗ Exception: ...` → Exception during connection
- `[TuningController] Signal generator connection result: false` → Connection failed

## Common Issues and Fixes

### Issue 1: Simulation Mode Not Enabled
**Symptoms:**
- `[SimMode] ✗ Simulation mode DISABLED`
- `[DI] ... Communication.VisaCommunicationManager` (not Mock)

**Fix:**
1. Set environment variable: `CALIBRATION_SIMULATE=true`
2. Restart Visual Studio
3. Run application again

### Issue 2: Mock Not Being Called
**Symptoms:**
- No `[MockVisa-Siglent]` messages
- Connection fails silently

**Fix:**
- Check DI registration (Step 2)
- Verify simulation mode is enabled (Step 1)
- Rebuild solution

### Issue 3: Exception During Connection
**Symptoms:**
- `[SignalGeneratorService] ✗ Exception: ...`

**Fix:**
- Read the exception message and stack trace
- Common exceptions:
  - `NullReferenceException` → Communication manager is null (DI issue)
  - `ArgumentException` → Invalid resource name format
  - `InvalidOperationException` → Device state issue

### Issue 4: Identity Parsing Fails
**Symptoms:**
- Exception when parsing identity string
- Identity string is null or empty

**Fix:**
- Check mock's `GenerateIdentityResponse()` method
- Verify response parser can handle the format

## Quick Test

Run this in the Immediate Window (Debug → Windows → Immediate) while debugging:
```csharp
// Check if simulation mode is enabled
System.Environment.GetEnvironmentVariable("CALIBRATION_SIMULATE")

// Check communication manager type
_signalGeneratorService.GetType().GetField("_communicationManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_signalGeneratorService).GetType().FullName
```

Expected results in simulation mode:
1. First command: "true" or "1"
2. Second command: "Siglent.SDG6052X.DeviceLibrary.Simulation.MockVisaCommunicationManager"

## Next Steps

1. Run the application in Debug mode (F5)
2. Open Debug Output window (View → Output, select "Debug")
3. Click "Connect" for Signal Generator
4. Copy all output messages
5. Go through this checklist to identify the issue
6. Apply the appropriate fix

## If Still Failing

If you've verified:
- ✓ Simulation mode is enabled
- ✓ Mock communication manager is registered
- ✓ Mock Connect() is being called
- ✓ Mock returns true
- ✗ But ConnectAsync still returns false

Then the issue is likely in the SignalGeneratorService logic between Connect() and the return statement. Check for:
- Exception during GetDeviceIdentity()
- Exception during ParseIdentityResponse()
- Any other exception in the try-catch block
