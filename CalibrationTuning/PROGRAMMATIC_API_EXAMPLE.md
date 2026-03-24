# Programmatic API Usage

The CalibrationTuning application exposes a programmatic API through the `ITuningController` interface. This allows other threads, tasks, or processes to control the tuning process.

## Accessing the Controller

The `ITuningController` is registered in the DI container and can be accessed:

```csharp
// From within the application (e.g., another form or service)
var tuningController = serviceProvider.GetRequiredService<ITuningController>();

// Or inject it into your class constructor
public class MyAutomationService
{
    private readonly ITuningController _tuningController;
    
    public MyAutomationService(ITuningController tuningController)
    {
        _tuningController = tuningController;
    }
}
```

## API Methods

### Connection Management

```csharp
// Connect to individual devices
bool powerMeterConnected = await tuningController.ConnectPowerMeterAsync("192.168.1.100");
bool signalGenConnected = await tuningController.ConnectSignalGeneratorAsync("192.168.1.101");

// Or connect both at once
bool bothConnected = await tuningController.ConnectDevicesAsync("192.168.1.100", "192.168.1.101");

// Disconnect
tuningController.DisconnectPowerMeter();
tuningController.DisconnectSignalGenerator();
tuningController.DisconnectDevices();
```

### Setting Tuning Parameters

```csharp
var parameters = new TuningParameters
{
    FrequencyHz = 2400000000,        // 2.4 GHz
    InitialVoltage = 0.5,            // 0.5 V
    TargetPowerDbm = -10.0,          // -10 dBm setpoint
    ToleranceDb = 0.5,               // ±0.5 dB tolerance
    VoltageStepSize = 0.05,          // 0.05 V step
    MinVoltage = 0.1,                // 0.1 V minimum
    MaxVoltage = 5.0,                // 5.0 V maximum
    MaxIterations = 100,             // 100 iterations max
    SensorId = 1                     // Sensor 1
};

// Start tuning with these parameters
await tuningController.StartTuningAsync(parameters);
```

### Monitoring Tuning Status

```csharp
// Subscribe to events
tuningController.StateChanged += (sender, e) =>
{
    Console.WriteLine($"State changed: {e.PreviousState} -> {e.NewState}");
};

tuningController.ProgressUpdated += (sender, e) =>
{
    var stats = e.Statistics;
    Console.WriteLine($"Iteration {stats.CurrentIteration}: " +
                     $"Voltage={stats.CurrentVoltage:F3}V, " +
                     $"Power={stats.CurrentPowerDbm:F2}dBm, " +
                     $"Error={stats.PowerError:F2}dB");
};

tuningController.TuningCompleted += (sender, e) =>
{
    var result = e.Result;
    Console.WriteLine($"Tuning completed: {result.FinalState}");
    Console.WriteLine($"Final voltage: {result.FinalVoltage:F3}V");
    Console.WriteLine($"Final power: {result.FinalPowerDbm:F2}dBm");
    Console.WriteLine($"Iterations: {result.TotalIterations}");
    Console.WriteLine($"Duration: {result.Duration.TotalSeconds:F1}s");
};

tuningController.ErrorOccurred += (sender, e) =>
{
    Console.WriteLine($"Error: {e.GetException().Message}");
};

// Get current state
TuningState currentState = tuningController.CurrentState;
TuningParameters currentParams = tuningController.Parameters;
TuningStatistics currentStats = tuningController.Statistics;
```

### Controlling Tuning

```csharp
// Stop tuning
tuningController.StopTuning();

// Manual measurement
PowerMeasurement measurement = await tuningController.MeasureManualAsync();
Console.WriteLine($"Power: {measurement.PowerDbm:F2} dBm");
```

## Complete Example: Automated Tuning

```csharp
using System;
using System.Threading.Tasks;
using CalibrationTuning.Controllers;
using CalibrationTuning.Models;

public class AutomatedTuningExample
{
    private readonly ITuningController _tuningController;
    private TaskCompletionSource<TuningResult> _tuningCompletionSource;
    
    public AutomatedTuningExample(ITuningController tuningController)
    {
        _tuningController = tuningController;
        
        // Subscribe to events
        _tuningController.TuningCompleted += OnTuningCompleted;
        _tuningController.ErrorOccurred += OnErrorOccurred;
    }
    
    public async Task<TuningResult> RunAutomatedTuningAsync(
        string powerMeterIp,
        string signalGenIp,
        double frequencyHz,
        double targetPowerDbm)
    {
        try
        {
            // Connect devices
            Console.WriteLine("Connecting to devices...");
            bool connected = await _tuningController.ConnectDevicesAsync(powerMeterIp, signalGenIp);
            
            if (!connected)
            {
                throw new Exception("Failed to connect to devices");
            }
            
            Console.WriteLine("Devices connected successfully");
            
            // Configure tuning parameters
            var parameters = new TuningParameters
            {
                FrequencyHz = frequencyHz,
                InitialVoltage = 0.5,
                TargetPowerDbm = targetPowerDbm,
                ToleranceDb = 0.5,
                VoltageStepSize = 0.05,
                MinVoltage = 0.1,
                MaxVoltage = 5.0,
                MaxIterations = 100,
                SensorId = 1
            };
            
            // Start tuning and wait for completion
            Console.WriteLine($"Starting tuning to {targetPowerDbm} dBm at {frequencyHz / 1e9} GHz...");
            _tuningCompletionSource = new TaskCompletionSource<TuningResult>();
            
            await _tuningController.StartTuningAsync(parameters);
            
            // Wait for tuning to complete
            TuningResult result = await _tuningCompletionSource.Task;
            
            Console.WriteLine($"Tuning completed: {result.FinalState}");
            Console.WriteLine($"Final voltage: {result.FinalVoltage:F3} V");
            Console.WriteLine($"Final power: {result.FinalPowerDbm:F2} dBm");
            Console.WriteLine($"Power error: {result.PowerError:F2} dB");
            Console.WriteLine($"Iterations: {result.TotalIterations}");
            Console.WriteLine($"Duration: {result.Duration.TotalSeconds:F1} seconds");
            
            return result;
        }
        finally
        {
            // Disconnect devices
            _tuningController.DisconnectDevices();
            Console.WriteLine("Devices disconnected");
        }
    }
    
    private void OnTuningCompleted(object sender, TuningCompletedEventArgs e)
    {
        _tuningCompletionSource?.TrySetResult(e.Result);
    }
    
    private void OnErrorOccurred(object sender, System.IO.ErrorEventArgs e)
    {
        Console.WriteLine($"Error occurred: {e.GetException().Message}");
    }
}

// Usage
public static async Task Main()
{
    // Get tuning controller from DI container
    var tuningController = serviceProvider.GetRequiredService<ITuningController>();
    
    var automation = new AutomatedTuningExample(tuningController);
    
    // Run automated tuning
    var result = await automation.RunAutomatedTuningAsync(
        powerMeterIp: "192.168.1.100",
        signalGenIp: "192.168.1.101",
        frequencyHz: 2400000000,  // 2.4 GHz
        targetPowerDbm: -10.0     // -10 dBm
    );
    
    if (result.FinalState == TuningState.Converged)
    {
        Console.WriteLine("Tuning successful!");
    }
    else
    {
        Console.WriteLine($"Tuning failed: {result.ErrorMessage}");
    }
}
```

## Thread Safety

The `ITuningController` implementation is thread-safe for:
- Reading current state, parameters, and statistics
- Subscribing/unsubscribing from events
- Calling connection methods from different threads

However, you should not:
- Start multiple tuning sessions simultaneously
- Call `StartTuningAsync` while tuning is already active

## Inter-Process Communication

For communication between separate processes, you would need to add:

1. **Named Pipes or TCP/IP Server** - Create a service that wraps `ITuningController`
2. **Shared Memory** - For high-performance data exchange
3. **Message Queue** - For asynchronous command/status updates

Example wrapper service:

```csharp
public class TuningControlService
{
    private readonly ITuningController _controller;
    private TcpListener _listener;
    
    public void StartServer(int port)
    {
        _listener = new TcpListener(IPAddress.Loopback, port);
        _listener.Start();
        
        Task.Run(async () =>
        {
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        });
    }
    
    private async Task HandleClientAsync(TcpClient client)
    {
        using (var stream = client.GetStream())
        using (var reader = new StreamReader(stream))
        using (var writer = new StreamWriter(stream) { AutoFlush = true })
        {
            string command = await reader.ReadLineAsync();
            
            // Parse command and call appropriate ITuningController method
            // Send response back to client
        }
    }
}
```

## Data Logging

All tuning data is automatically logged to CSV files via the `IDataLoggingController`. You can access this programmatically:

```csharp
var dataLoggingController = serviceProvider.GetRequiredService<IDataLoggingController>();

// Start logging
dataLoggingController.StartLogging("C:\\Logs\\tuning_session.csv");

// Stop logging
dataLoggingController.StopLogging();

// Get current log file path
string currentLogFile = dataLoggingController.CurrentLogFile;
```
