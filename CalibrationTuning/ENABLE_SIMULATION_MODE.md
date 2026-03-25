# How to Enable Simulation Mode - CRITICAL FIX

## The Problem
You're seeing `System.NotImplementedException` because the **real** VISA communication manager is being used instead of the **mock** communication manager. This happens when simulation mode is NOT enabled.

## The Solution
You MUST enable simulation mode using one of these methods:

---

## Method 1: Environment Variable (RECOMMENDED)

### Windows 10/11:
1. Press `Win + X` and select "System"
2. Click "Advanced system settings" on the right
3. Click "Environment Variables" button
4. Under "User variables", click "New"
5. Variable name: `CALIBRATION_SIMULATE`
6. Variable value: `true`
7. Click OK on all dialogs
8. **IMPORTANT**: Close and restart Visual Studio
9. Rebuild the solution (Build → Rebuild Solution)
10. Run the application (F5)

### Verify it worked:
- Debug Output should show: `[SimMode] ✓ Simulation mode ENABLED via environment variable`
- If you don't see this message, the environment variable isn't set correctly

---

## Method 2: Visual Studio Debug Settings (EASIER)

1. In Visual Studio, right-click the **CalibrationTuning** project (not the solution)
2. Select "Properties"
3. Go to the "Debug" tab on the left
4. In "Command line arguments" field, enter: `--simulate`
5. Save (Ctrl+S)
6. Run the application (F5)

### Verify it worked:
- Debug Output should show: `[SimMode] ✓ Simulation mode ENABLED via command line`

---

## Method 3: Batch File (FOR RELEASE BUILDS)

1. Build the project in Debug mode
2. Navigate to `CalibrationTuning` folder
3. Double-click `RunSimulation.bat`
4. Application will launch with simulation mode enabled

---

## How to Verify Simulation Mode is Enabled

After enabling simulation mode and running the app, check the Debug Output window:

1. In Visual Studio, go to View → Output
2. Select "Debug" from the dropdown (not "Build")
3. Look for these messages when the app starts:

```
[SimMode] Command line args: ...
[SimMode] Environment variable CALIBRATION_SIMULATE=true
[SimMode] ✓ Simulation mode ENABLED via environment variable
[DI] Creating PowerMeterService
[DI] PowerMeterService - CommManager type: Tegam._1830A.DeviceLibrary.Simulation.MockVisaCommunicationManager
[DI] Creating SignalGeneratorService
[DI] SignalGeneratorService - CommManager type: Siglent.SDG6052X.DeviceLibrary.Simulation.MockVisaCommunicationManager
```

### If you see this instead:
```
[SimMode] ✗ Simulation mode DISABLED - using real hardware
```
Then simulation mode is NOT enabled. Go back and try one of the methods above.

---

## Why This Happens

The Siglent.SDG6052X.DeviceLibrary is compiled with `NO_VISA` defined, which means:
- The real `VisaCommunicationManager` throws `NotImplementedException` 
- You MUST use the `MockVisaCommunicationManager` for testing
- The mock is only registered when simulation mode is enabled

Without simulation mode:
- ❌ Real VISA manager is used
- ❌ Throws NotImplementedException
- ❌ Connections fail

With simulation mode:
- ✅ Mock VISA manager is used
- ✅ Simulates device communication
- ✅ Connections succeed

---

## Quick Test After Enabling

1. Enable simulation mode using Method 1 or 2 above
2. Rebuild the solution (Build → Rebuild Solution)
3. Run the application (F5)
4. Check Debug Output for `[SimMode] ✓ Simulation mode ENABLED`
5. Go to Connection tab
6. Click "Connect" for Signal Generator
7. Should see "Status: Connected" in green within 1 second
8. Click "Connect" for Power Meter
9. Should see "Status: Connected" in green within 1 second

---

## Still Not Working?

If you've enabled simulation mode but still see `NotImplementedException`:

1. **Verify the environment variable is set**:
   - Open Command Prompt
   - Type: `echo %CALIBRATION_SIMULATE%`
   - Should show: `true`
   - If it shows `%CALIBRATION_SIMULATE%`, the variable is not set

2. **Restart Visual Studio**:
   - Environment variables are only read when VS starts
   - Close VS completely
   - Open VS again
   - Rebuild solution
   - Run application

3. **Check the Debug Output**:
   - If you don't see ANY `[SimMode]` messages, you're running an old build
   - Do a Clean Solution (Build → Clean Solution)
   - Then Rebuild Solution (Build → Rebuild Solution)
   - Run again

4. **Use Method 2 (Command Line Arguments)**:
   - This doesn't require restarting VS
   - Easier to verify it's working
   - Just add `--simulate` to project properties

---

## Summary

**YOU MUST ENABLE SIMULATION MODE** to use the CalibrationTuning application without physical hardware.

The easiest method is **Method 2** (Visual Studio Debug Settings):
1. Right-click CalibrationTuning project → Properties
2. Debug tab → Command line arguments: `--simulate`
3. Save and run (F5)

Then verify you see `[SimMode] ✓ Simulation mode ENABLED` in the Debug Output window.
