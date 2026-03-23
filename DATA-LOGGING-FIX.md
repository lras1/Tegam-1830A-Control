# Data Logging Fix

## Issue Identified

The data logging feature was only writing the CSV header but not the actual measurement data.

### Root Cause

The `DataLoggingController.PowerMeterService_MeasurementReceived` event handler was only incrementing a counter but **not writing the measurement data to the CSV file**.

**Original Code** (Lines 27-33):
```csharp
private void PowerMeterService_MeasurementReceived(object sender, EventArgs e)
{
    if (!string.IsNullOrEmpty(_currentLogFile))
    {
        _measurementCount++;
        MeasurementLogged?.Invoke(this, _measurementCount);
    }
}
```

This code:
- ✅ Incremented the counter
- ✅ Raised the `MeasurementLogged` event
- ❌ **Did NOT write data to the CSV file**

## Fix Applied

Updated the event handler to actually write measurement data to the CSV file.

**Fixed Code**:
```csharp
private void PowerMeterService_MeasurementReceived(object sender, EventArgs e)
{
    if (!string.IsNullOrEmpty(_currentLogFile) && e is MeasurementEventArgs measurementArgs)
    {
        try
        {
            // Write measurement to CSV file
            lock (_fileLock)
            {
                using (var writer = new StreamWriter(_currentLogFile, true))
                {
                    var measurement = measurementArgs.Measurement;
                    writer.WriteLine($"{measurement.Timestamp:yyyy-MM-dd HH:mm:ss.fff}," +
                                   $"{measurement.Frequency}," +
                                   $"{measurement.PowerValue}," +
                                   $"{measurement.SensorId}");
                }
            }

            _measurementCount++;
            MeasurementLogged?.Invoke(this, _measurementCount);
        }
        catch (Exception ex)
        {
            OperationError?.Invoke(this, $"Error writing to log file: {ex.Message}");
        }
    }
}
```

### Changes Made:

1. **Cast EventArgs to MeasurementEventArgs** - To access the actual measurement data
2. **Added file writing logic** - Writes each measurement to the CSV file
3. **Added thread safety** - Uses `lock (_fileLock)` to prevent concurrent file access issues
4. **Added error handling** - Catches and reports file I/O errors
5. **Proper CSV formatting** - Writes timestamp, frequency, power, and sensor ID

## CSV File Location

### When Running in Visual Studio Debug Mode
**Full Path**: `C:\Users\[YourUsername]\source\repos\Tegam.1830A\Tegam.1830A.WinFormsUI\bin\Debug\measurements.csv`

### When Running in Visual Studio Release Mode
**Full Path**: `C:\Users\[YourUsername]\source\repos\Tegam.1830A\Tegam.1830A.WinFormsUI\bin\Release\measurements.csv`

### When Running from Deployed Application
The CSV file is saved to the application's working directory unless a full path is specified.

### Recommended Usage
Use the "Browse" button in the Data Logging panel to select a specific location for the CSV file.

## CSV File Format

```csv
Timestamp,Frequency (Hz),Power (dBm),Sensor ID
2026-03-22 10:30:15.123,2400000000,-15.5,1
2026-03-22 10:30:16.456,2400000000,-15.3,1
2026-03-22 10:30:17.789,2400000000,-15.4,1
```

**Columns**:
- **Timestamp**: Date and time with milliseconds (yyyy-MM-dd HH:mm:ss.fff)
- **Frequency (Hz)**: Measurement frequency in Hertz
- **Power (dBm)**: Power measurement in dBm
- **Sensor ID**: Sensor identifier (1-4)

## Testing the Fix

1. Build the solution
2. Run the application
3. Connect to device (or use mock mode)
4. Navigate to "Data Logging" tab
5. Enter filename or browse for location
6. Click "Start Logging"
7. Take measurements (manually or continuously)
8. Click "Stop Logging"
9. Open the CSV file - you should now see all measurements

## Expected Results

- CSV file should contain header row
- CSV file should contain one row per measurement
- Each row should have timestamp, frequency, power, and sensor ID
- File should be updated in real-time as measurements are taken

## Files Modified

- `Tegam.1830A.WinFormsUI/Controllers/DataLoggingController.cs` - Fixed event handler
- `README.md` - Updated CSV file location documentation
