using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CalibrationTuning.Events;
using CalibrationTuning.Models;
using Tegam._1830A.DeviceLibrary.Services;
using Siglent.SDG6052X.DeviceLibrary.Services;

namespace CalibrationTuning.Controllers
{
    /// <summary>
    /// Primary orchestrator for the tuning process.
    /// Coordinates device services, executes tuning algorithm, and manages state transitions.
    /// </summary>
    public class TuningController : ITuningController
    {
        // Device service references
        private readonly IPowerMeterService _powerMeterService;
        private readonly ISignalGeneratorService _signalGeneratorService;

        // State tracking
        private TuningState _currentState;
        private volatile bool _stopRequested;
        private TuningParameters _parameters;
        private TuningStatistics _statistics;

        /// <summary>
        /// Raised when the tuning state changes.
        /// </summary>
        public event EventHandler<TuningStateChangedEventArgs> StateChanged;

        /// <summary>
        /// Raised when tuning progress is updated (after each measurement).
        /// </summary>
        public event EventHandler<TuningProgressEventArgs> ProgressUpdated;

        /// <summary>
        /// Raised when a tuning session completes (converged, timeout, error, or aborted).
        /// </summary>
        public event EventHandler<TuningCompletedEventArgs> TuningCompleted;

        /// <summary>
        /// Raised when an error occurs during device operations or tuning.
        /// </summary>
        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        /// <summary>
        /// Raised when a user action occurs (Connect, Disconnect, Start Tuning, Stop Tuning, Manual Measure).
        /// </summary>
        public event EventHandler<UserActionEventArgs> UserActionOccurred;

        /// <summary>
        /// Gets the current tuning state.
        /// </summary>
        public TuningState CurrentState
        {
            get { return _currentState; }
            private set
            {
                if (_currentState != value)
                {
                    TuningState previousState = _currentState;
                    _currentState = value;
                    OnStateChanged(previousState, value);
                }
            }
        }

        /// <summary>
        /// Gets the current tuning parameters.
        /// </summary>
        public TuningParameters Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// Gets the current tuning statistics.
        /// </summary>
        public TuningStatistics Statistics
        {
            get { return _statistics; }
        }

        /// <summary>
        /// Initializes a new instance of the TuningController class.
        /// </summary>
        /// <param name="powerMeterService">Power meter service for measurements.</param>
        /// <param name="signalGeneratorService">Signal generator service for voltage control.</param>
        public TuningController(
            IPowerMeterService powerMeterService,
            ISignalGeneratorService signalGeneratorService)
        {
            _powerMeterService = powerMeterService ?? throw new ArgumentNullException(nameof(powerMeterService));
            _signalGeneratorService = signalGeneratorService ?? throw new ArgumentNullException(nameof(signalGeneratorService));

            _currentState = TuningState.Idle;
            _statistics = new TuningStatistics();
        }

        /// <summary>
        /// Connects to both the power meter and signal generator devices.
        /// </summary>
        /// <param name="powerMeterIp">IP address of the power meter.</param>
        /// <param name="signalGenIp">IP address of the signal generator.</param>
        /// <returns>True if both devices connected successfully, false otherwise.</returns>
        public async Task<bool> ConnectDevicesAsync(string powerMeterIp, string signalGenIp)
        {
            if (CurrentState != TuningState.Idle)
            {
                OnErrorOccurred("Cannot connect devices while tuning is active");
                return false;
            }

            CurrentState = TuningState.Connecting;

            try
            {
                // Connect to signal generator (async)
                bool signalGenConnected = await _signalGeneratorService.ConnectAsync(signalGenIp);
                if (!signalGenConnected)
                {
                    CurrentState = TuningState.Error;
                    OnErrorOccurred("Failed to connect to signal generator at " + signalGenIp);
                    return false;
                }

                // Connect to power meter (synchronous)
                bool powerMeterConnected = _powerMeterService.Connect(powerMeterIp);
                if (!powerMeterConnected)
                {
                    // Disconnect signal generator if power meter connection fails
                    await _signalGeneratorService.DisconnectAsync();
                    CurrentState = TuningState.Error;
                    OnErrorOccurred("Failed to connect to power meter at " + powerMeterIp);
                    return false;
                }

                // Both devices connected successfully
                CurrentState = TuningState.Idle;
                OnUserActionOccurred("Connect");
                return true;
            }
            catch (Exception ex)
            {
                CurrentState = TuningState.Error;
                OnErrorOccurred("Connection error: " + ex.Message);
                
                // Attempt to disconnect any connected devices
                try
                {
                    if (_signalGeneratorService.IsConnected)
                    {
                        await _signalGeneratorService.DisconnectAsync();
                    }
                    if (_powerMeterService.IsConnected)
                    {
                        _powerMeterService.Disconnect();
                    }
                }
                catch
                {
                    // Ignore cleanup errors
                }
                
                return false;
            }
        }

        /// <summary>
        /// Disconnects from both devices.
        /// </summary>
        public void DisconnectDevices()
        {
            if (CurrentState == TuningState.Tuning || CurrentState == TuningState.Measuring || CurrentState == TuningState.Evaluating)
            {
                OnErrorOccurred("Cannot disconnect devices while tuning is active. Stop tuning first.");
                return;
            }

            try
            {
                // Disconnect power meter (synchronous)
                if (_powerMeterService.IsConnected)
                {
                    _powerMeterService.Disconnect();
                }

                // Disconnect signal generator (async, but we'll use Task.Run to avoid async void)
                if (_signalGeneratorService.IsConnected)
                {
                    Task.Run(async () => await _signalGeneratorService.DisconnectAsync()).Wait();
                }

                CurrentState = TuningState.Idle;
                OnUserActionOccurred("Disconnect");
            }
            catch (Exception ex)
            {
                OnErrorOccurred("Disconnection error: " + ex.Message);
            }
        }

        /// <summary>
        /// Connects to the power meter only.
        /// </summary>
        /// <param name="powerMeterIp">IP address of the power meter.</param>
        /// <returns>True if connected successfully, false otherwise.</returns>
        public Task<bool> ConnectPowerMeterAsync(string powerMeterIp)
        {
            return Task.Run(() =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"[TuningController] Attempting to connect to power meter at {powerMeterIp}");
                    bool powerMeterConnected = _powerMeterService.Connect(powerMeterIp);
                    System.Diagnostics.Debug.WriteLine($"[TuningController] Power meter connection result: {powerMeterConnected}");
                    
                    if (!powerMeterConnected)
                    {
                        OnErrorOccurred("Failed to connect to power meter at " + powerMeterIp);
                        return false;
                    }
                    OnUserActionOccurred("Connect Power Meter");
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[TuningController] Power meter connection exception: {ex}");
                    OnErrorOccurred("Power meter connection error: " + ex.Message);
                    return false;
                }
            });
        }

        /// <summary>
        /// Disconnects from the power meter only.
        /// </summary>
        public void DisconnectPowerMeter()
        {
            try
            {
                if (_powerMeterService.IsConnected)
                {
                    _powerMeterService.Disconnect();
                    OnUserActionOccurred("Disconnect Power Meter");
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred("Power meter disconnection error: " + ex.Message);
            }
        }

        /// <summary>
        /// Connects to the signal generator only.
        /// </summary>
        /// <param name="signalGenIp">IP address of the signal generator.</param>
        /// <returns>True if connected successfully, false otherwise.</returns>
        public async Task<bool> ConnectSignalGeneratorAsync(string signalGenIp)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[TuningController] Attempting to connect to signal generator at {signalGenIp}");
                bool signalGenConnected = await _signalGeneratorService.ConnectAsync(signalGenIp);
                System.Diagnostics.Debug.WriteLine($"[TuningController] Signal generator connection result: {signalGenConnected}");
                
                if (!signalGenConnected)
                {
                    OnErrorOccurred("Failed to connect to signal generator at " + signalGenIp);
                    return false;
                }
                OnUserActionOccurred("Connect Signal Generator");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TuningController] Signal generator connection exception: {ex}");
                OnErrorOccurred("Signal generator connection error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Disconnects from the signal generator only.
        /// </summary>
        public void DisconnectSignalGenerator()
        {
            try
            {
                if (_signalGeneratorService.IsConnected)
                {
                    Task.Run(async () => await _signalGeneratorService.DisconnectAsync()).Wait();
                    OnUserActionOccurred("Disconnect Signal Generator");
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred("Signal generator disconnection error: " + ex.Message);
            }
        }

        /// <summary>
        /// Starts an automated tuning session with the specified parameters.
        /// </summary>
        /// <param name="parameters">Tuning configuration parameters.</param>
        public async Task StartTuningAsync(TuningParameters parameters)
        {
            if (parameters == null)
            {
                OnErrorOccurred("Tuning parameters cannot be null");
                return;
            }

            // Validate SensorId
            if (parameters.SensorId < 1 || parameters.SensorId > 2)
            {
                CurrentState = TuningState.Error;
                OnErrorOccurred("Invalid SensorId. Must be 1 or 2.");
                CurrentState = TuningState.Idle;
                return;
            }

            if (!_powerMeterService.IsConnected || !_signalGeneratorService.IsConnected)
            {
                OnErrorOccurred("Both devices must be connected before starting tuning");
                return;
            }

            if (CurrentState != TuningState.Idle)
            {
                OnErrorOccurred("Cannot start tuning while another session is active");
                return;
            }

            // Store parameters and initialize statistics
            _parameters = parameters;
            _statistics = new TuningStatistics();
            _statistics.CurrentVoltage = parameters.InitialVoltage;
            _stopRequested = false;

            var startTime = DateTime.Now;

            try
            {
                CurrentState = TuningState.Tuning;
                OnUserActionOccurred("Start Tuning");
                System.Diagnostics.Debug.WriteLine($"[TuningController] StartTuning: Freq={parameters.FrequencyHz}, Voltage={parameters.InitialVoltage}, Target={parameters.TargetPowerDbm}, SensorId={parameters.SensorId}");

                // Configure signal generator frequency
                var waveformParams = new Siglent.SDG6052X.DeviceLibrary.Models.WaveformParameters
                {
                    Frequency = parameters.FrequencyHz,
                    Amplitude = parameters.InitialVoltage,
                    Unit = Siglent.SDG6052X.DeviceLibrary.Models.AmplitudeUnit.Vpp,
                    Load = Siglent.SDG6052X.DeviceLibrary.Models.LoadImpedance.HighZ
                };

                System.Diagnostics.Debug.WriteLine("[TuningController] Calling SetBasicWaveformAsync...");
                var setWaveformResult = await _signalGeneratorService.SetBasicWaveformAsync(
                    1, 
                    Siglent.SDG6052X.DeviceLibrary.Models.WaveformType.Sine, 
                    waveformParams).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine($"[TuningController] SetBasicWaveform result: Success={setWaveformResult.Success}, Message={setWaveformResult.Message}");

                if (!setWaveformResult.Success)
                {
                    CurrentState = TuningState.Error;
                    System.Diagnostics.Debug.WriteLine($"[TuningController] SetBasicWaveform FAILED: {setWaveformResult.Message}");
                    OnErrorOccurred("Failed to configure signal generator: " + setWaveformResult.Message);
                    CurrentState = TuningState.Idle;
                    return;
                }

                // Configure power meter frequency
                System.Diagnostics.Debug.WriteLine("[TuningController] Calling SetFrequency...");
                var setFreqResult = _powerMeterService.SetFrequency(
                    parameters.FrequencyHz, 
                    Tegam._1830A.DeviceLibrary.Models.FrequencyUnit.Hz);
                System.Diagnostics.Debug.WriteLine($"[TuningController] SetFrequency result: Success={setFreqResult.IsSuccess}, Error={setFreqResult.ErrorMessage}");

                if (!setFreqResult.IsSuccess)
                {
                    CurrentState = TuningState.Error;
                    System.Diagnostics.Debug.WriteLine($"[TuningController] SetFrequency FAILED: {setFreqResult.ErrorMessage}");
                    OnErrorOccurred("Failed to configure power meter frequency: " + setFreqResult.ErrorMessage);
                    CurrentState = TuningState.Idle;
                    return;
                }

                // Configure power meter sensor
                System.Diagnostics.Debug.WriteLine("[TuningController] Calling SelectSensor...");
                var setSensorResult = _powerMeterService.SelectSensor(parameters.SensorId);
                System.Diagnostics.Debug.WriteLine($"[TuningController] SelectSensor result: Success={setSensorResult.IsSuccess}");
                if (!setSensorResult.IsSuccess)
                {
                    CurrentState = TuningState.Error;
                    System.Diagnostics.Debug.WriteLine($"[TuningController] SelectSensor FAILED: {setSensorResult.ErrorMessage}");
                    OnErrorOccurred("Failed to select power meter sensor: " + setSensorResult.ErrorMessage);
                    CurrentState = TuningState.Idle;
                    return;
                }

                // Enable signal generator output
                System.Diagnostics.Debug.WriteLine("[TuningController] Calling SetOutputStateAsync...");
                var enableOutputResult = await _signalGeneratorService.SetOutputStateAsync(1, true).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine($"[TuningController] SetOutputState result: Success={enableOutputResult.Success}");
                if (!enableOutputResult.Success)
                {
                    CurrentState = TuningState.Error;
                    OnErrorOccurred("Failed to enable signal generator output: " + enableOutputResult.Message);
                    CurrentState = TuningState.Idle;
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[TuningController] Entering measurement loop, MaxIterations={parameters.MaxIterations}, SampleDelay={parameters.SampleDelayMs}ms");

                // Iterative measurement loop (MaxIterations=0 means continuous until stopped)
                bool continuous = parameters.MaxIterations == 0;
                int maxIter = continuous ? int.MaxValue : parameters.MaxIterations;
                for (int iteration = 1; iteration <= maxIter; iteration++)
                {
                    // Check if stop was requested
                    if (_stopRequested)
                    {
                        // Disable signal generator output
                        await _signalGeneratorService.SetOutputStateAsync(1, false);
                        
                        var abortResult = new TuningResult
                        {
                            FinalState = TuningState.Aborted,
                            TotalIterations = iteration - 1,
                            FinalVoltage = _statistics.CurrentVoltage,
                            FinalPowerDbm = _statistics.CurrentPowerDbm,
                            PowerError = _statistics.PowerError,
                            Duration = DateTime.Now - startTime,
                            ErrorMessage = "Tuning aborted by user"
                        };
                        
                        OnTuningCompleted(abortResult);
                        CurrentState = TuningState.Idle;
                        return;
                    }

                    _statistics.CurrentIteration = iteration;
                    CurrentState = TuningState.Measuring;

                    if (iteration == 1)
                        System.Diagnostics.Debug.WriteLine("[TuningController] First iteration starting...");

                    // Small delay for signal settling (configurable sample rate)
                    await Task.Delay(parameters.SampleDelayMs).ConfigureAwait(false);

                    // Measure current power
                    var powerMeasurement = _powerMeterService.MeasurePower();
                    
                    if (iteration == 1)
                        System.Diagnostics.Debug.WriteLine($"[TuningController] First measurement: Power={powerMeasurement?.PowerValue}");
                    
                    if (powerMeasurement == null)
                    {
                        CurrentState = TuningState.Error;
                        await _signalGeneratorService.SetOutputStateAsync(1, false);
                        OnErrorOccurred("Failed to measure power from power meter");
                        CurrentState = TuningState.Idle;
                        return;
                    }

                    // Check for power meter overload condition (Requirement 6.5)
                    var errorQueue = _powerMeterService.GetErrorQueue();
                    if (errorQueue != null && errorQueue.Count > 0)
                    {
                        // Check for overload or range errors
                        foreach (var error in errorQueue)
                        {
                            // Common SCPI error codes for overload/range issues:
                            // -222: Data out of range
                            // -350: Queue overflow
                            // Device-specific overload errors typically in range 100-999
                            if (error.ErrorCode == -222 || 
                                (error.ErrorCode >= 100 && error.ErrorCode <= 999 && 
                                 (error.ErrorMessage.ToLower().Contains("overload") || 
                                  error.ErrorMessage.ToLower().Contains("over range") ||
                                  error.ErrorMessage.ToLower().Contains("limit"))))
                            {
                                CurrentState = TuningState.Error;
                                await _signalGeneratorService.SetOutputStateAsync(1, false);
                                
                                var overloadResult = new TuningResult
                                {
                                    FinalState = TuningState.Error,
                                    TotalIterations = iteration,
                                    FinalVoltage = _statistics.CurrentVoltage,
                                    FinalPowerDbm = _statistics.CurrentPowerDbm,
                                    PowerError = _statistics.PowerError,
                                    Duration = DateTime.Now - startTime,
                                    ErrorMessage = "Power meter overload detected: " + error.ErrorMessage
                                };
                                
                                OnTuningCompleted(overloadResult);
                                OnErrorOccurred("Power meter overload detected: " + error.ErrorMessage);
                                CurrentState = TuningState.Idle;
                                return;
                            }
                        }
                    }

                    // Update statistics
                    _statistics.CurrentPowerDbm = powerMeasurement.PowerValue;
                    _statistics.PowerError = powerMeasurement.PowerValue - parameters.TargetPowerDbm;
                    _statistics.ElapsedTime = DateTime.Now - startTime;

                    // Add data point to history
                    var dataPoint = new TuningDataPoint
                    {
                        Iteration = iteration,
                        Timestamp = DateTime.Now,
                        Voltage = _statistics.CurrentVoltage,
                        PowerDbm = _statistics.CurrentPowerDbm
                    };
                    _statistics.History.Add(dataPoint);

                    // Emit progress event
                    OnProgressUpdated();

                    CurrentState = TuningState.Evaluating;

                    // Check convergence criteria
                    if (Math.Abs(_statistics.PowerError) <= parameters.ToleranceDb)
                    {
                        // Converged!
                        CurrentState = TuningState.Converged;
                        await _signalGeneratorService.SetOutputStateAsync(1, false);
                        
                        var result = new TuningResult
                        {
                            FinalState = TuningState.Converged,
                            TotalIterations = iteration,
                            FinalVoltage = _statistics.CurrentVoltage,
                            FinalPowerDbm = _statistics.CurrentPowerDbm,
                            PowerError = _statistics.PowerError,
                            Duration = DateTime.Now - startTime
                        };
                        
                        OnTuningCompleted(result);
                        CurrentState = TuningState.Idle;
                        return;
                    }

                    // Calculate voltage adjustment using proportional control
                    double newVoltage;
                    if (_statistics.PowerError < -parameters.ToleranceDb)
                    {
                        // Power too low, increase voltage
                        newVoltage = _statistics.CurrentVoltage + parameters.VoltageStepSize;
                    }
                    else
                    {
                        // Power too high, decrease voltage
                        newVoltage = _statistics.CurrentVoltage - parameters.VoltageStepSize;
                    }

                    // Check voltage safety limits
                    if (newVoltage > parameters.MaxVoltage)
                    {
                        CurrentState = TuningState.Error;
                        await _signalGeneratorService.SetOutputStateAsync(1, false);
                        
                        var result = new TuningResult
                        {
                            FinalState = TuningState.Error,
                            TotalIterations = iteration,
                            FinalVoltage = _statistics.CurrentVoltage,
                            FinalPowerDbm = _statistics.CurrentPowerDbm,
                            PowerError = _statistics.PowerError,
                            Duration = DateTime.Now - startTime,
                            ErrorMessage = "Maximum voltage limit exceeded"
                        };
                        
                        OnTuningCompleted(result);
                        OnErrorOccurred("Maximum voltage limit exceeded");
                        CurrentState = TuningState.Idle;
                        return;
                    }

                    if (newVoltage < parameters.MinVoltage)
                    {
                        CurrentState = TuningState.Error;
                        await _signalGeneratorService.SetOutputStateAsync(1, false);
                        
                        var result = new TuningResult
                        {
                            FinalState = TuningState.Error,
                            TotalIterations = iteration,
                            FinalVoltage = _statistics.CurrentVoltage,
                            FinalPowerDbm = _statistics.CurrentPowerDbm,
                            PowerError = _statistics.PowerError,
                            Duration = DateTime.Now - startTime,
                            ErrorMessage = "Minimum voltage limit exceeded"
                        };
                        
                        OnTuningCompleted(result);
                        OnErrorOccurred("Minimum voltage limit exceeded");
                        CurrentState = TuningState.Idle;
                        return;
                    }

                    // Apply new voltage
                    _statistics.CurrentVoltage = newVoltage;
                    
                    var updateWaveformParams = new Siglent.SDG6052X.DeviceLibrary.Models.WaveformParameters
                    {
                        Frequency = parameters.FrequencyHz,
                        Amplitude = newVoltage,
                        Unit = Siglent.SDG6052X.DeviceLibrary.Models.AmplitudeUnit.Vpp,
                        Load = Siglent.SDG6052X.DeviceLibrary.Models.LoadImpedance.HighZ
                    };

                    var updateResult = await _signalGeneratorService.SetBasicWaveformAsync(
                        1, 
                        Siglent.SDG6052X.DeviceLibrary.Models.WaveformType.Sine, 
                        updateWaveformParams);

                    if (!updateResult.Success)
                    {
                        CurrentState = TuningState.Error;
                        await _signalGeneratorService.SetOutputStateAsync(1, false);
                        OnErrorOccurred("Failed to update signal generator voltage: " + updateResult.Message);
                        CurrentState = TuningState.Idle;
                        return;
                    }

                    CurrentState = TuningState.Tuning;
                }

                // Max iterations reached without convergence
                CurrentState = TuningState.Timeout;
                await _signalGeneratorService.SetOutputStateAsync(1, false);
                
                var timeoutResult = new TuningResult
                {
                    FinalState = TuningState.Timeout,
                    TotalIterations = parameters.MaxIterations,
                    FinalVoltage = _statistics.CurrentVoltage,
                    FinalPowerDbm = _statistics.CurrentPowerDbm,
                    PowerError = _statistics.PowerError,
                    Duration = DateTime.Now - startTime,
                    ErrorMessage = "Maximum iterations reached without convergence"
                };
                
                OnTuningCompleted(timeoutResult);
                CurrentState = TuningState.Idle;
            }
            catch (Exception ex)
            {
                CurrentState = TuningState.Error;
                
                // Attempt to disable output
                try
                {
                    await _signalGeneratorService.SetOutputStateAsync(1, false);
                }
                catch
                {
                    // Ignore cleanup errors
                }
                
                var errorResult = new TuningResult
                {
                    FinalState = TuningState.Error,
                    TotalIterations = _statistics.CurrentIteration,
                    FinalVoltage = _statistics.CurrentVoltage,
                    FinalPowerDbm = _statistics.CurrentPowerDbm,
                    PowerError = _statistics.PowerError,
                    Duration = DateTime.Now - startTime,
                    ErrorMessage = ex.Message
                };
                
                OnTuningCompleted(errorResult);
                OnErrorOccurred("Tuning error: " + ex.Message);
                CurrentState = TuningState.Idle;
            }
        }

        /// <summary>
        /// Stops the current tuning session.
        /// </summary>
        public void StopTuning()
        {
            // Check if tuning is active
            if (CurrentState != TuningState.Tuning && 
                CurrentState != TuningState.Measuring && 
                CurrentState != TuningState.Evaluating)
            {
                OnErrorOccurred("No active tuning session to stop");
                return;
            }

            // Set volatile flag so background thread sees it immediately
            _stopRequested = true;
            OnUserActionOccurred("Stop Tuning");
        }

        /// <summary>
        /// Performs a single manual power measurement.
        /// </summary>
        /// <returns>The measured power value.</returns>
        public Task<PowerMeasurement> MeasureManualAsync()
        {
            return Task.Run(() =>
            {
                // Verify devices are connected
                if (!_powerMeterService.IsConnected)
                {
                    OnErrorOccurred("Power meter is not connected");
                    return new PowerMeasurement
                    {
                        IsValid = false,
                        ErrorMessage = "Power meter is not connected",
                        Timestamp = DateTime.Now
                    };
                }

                // Verify not currently tuning
                if (CurrentState == TuningState.Tuning || 
                    CurrentState == TuningState.Measuring || 
                    CurrentState == TuningState.Evaluating)
                {
                    OnErrorOccurred("Cannot perform manual measurement while tuning is active");
                    return new PowerMeasurement
                    {
                        IsValid = false,
                        ErrorMessage = "Cannot perform manual measurement while tuning is active",
                        Timestamp = DateTime.Now
                    };
                }

                try
                {
                    // Perform measurement
                    var powerMeasurement = _powerMeterService.MeasurePower();
                
                    if (powerMeasurement == null)
                    {
                        OnErrorOccurred("Failed to measure power from power meter");
                        return new PowerMeasurement
                        {
                            IsValid = false,
                            ErrorMessage = "Failed to measure power from power meter",
                            Timestamp = DateTime.Now
                        };
                    }

                    // Return successful measurement
                    return new PowerMeasurement
                    {
                        PowerDbm = powerMeasurement.PowerValue,
                        IsValid = true,
                        Timestamp = DateTime.Now
                    };
                }
                catch (Exception ex)
                {
                    OnErrorOccurred("Manual measurement error: " + ex.Message);
                    return new PowerMeasurement
                    {
                        IsValid = false,
                        ErrorMessage = ex.Message,
                        Timestamp = DateTime.Now
                    };
                }
            });
        }

        /// <summary>
        /// Gets the current frequency from the signal generator.
        /// </summary>
        /// <returns>Current frequency in Hz.</returns>
        public async Task<double> GetCurrentFrequencyAsync()
        {
            if (!_signalGeneratorService.IsConnected)
            {
                throw new InvalidOperationException("Signal generator is not connected");
            }

            try
            {
                var waveformState = await _signalGeneratorService.GetWaveformStateAsync(1);
                return waveformState.Frequency;
            }
            catch (Exception ex)
            {
                OnErrorOccurred("Failed to get current frequency: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets the current voltage from the signal generator.
        /// </summary>
        /// <returns>Current voltage.</returns>
        public async Task<double> GetCurrentVoltageAsync()
        {
            if (!_signalGeneratorService.IsConnected)
            {
                throw new InvalidOperationException("Signal generator is not connected");
            }

            try
            {
                var waveformState = await _signalGeneratorService.GetWaveformStateAsync(1);
                return waveformState.Amplitude;
            }
            catch (Exception ex)
            {
                OnErrorOccurred("Failed to get current voltage: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Performs a manual measurement with full details (frequency, voltage, and power).
        /// </summary>
        /// <returns>Manual measurement result with all values.</returns>
        public async Task<ManualMeasurementResult> MeasureManualWithDetailsAsync()
        {
            var result = new ManualMeasurementResult
            {
                Timestamp = DateTime.Now
            };

            // Verify devices are connected
            if (!_signalGeneratorService.IsConnected)
            {
                result.IsValid = false;
                result.ErrorMessage = "Signal generator is not connected";
                OnErrorOccurred(result.ErrorMessage);
                return result;
            }

            if (!_powerMeterService.IsConnected)
            {
                result.IsValid = false;
                result.ErrorMessage = "Power meter is not connected";
                OnErrorOccurred(result.ErrorMessage);
                return result;
            }

            // Verify not currently tuning
            if (CurrentState == TuningState.Tuning || 
                CurrentState == TuningState.Measuring || 
                CurrentState == TuningState.Evaluating)
            {
                result.IsValid = false;
                result.ErrorMessage = "Cannot perform manual measurement while tuning is active";
                OnErrorOccurred(result.ErrorMessage);
                return result;
            }

            try
            {
                // Get frequency from signal generator
                result.FrequencyHz = await GetCurrentFrequencyAsync();

                // Get voltage from signal generator
                result.Voltage = await GetCurrentVoltageAsync();

                // Measure power from power meter
                var powerMeasurement = _powerMeterService.MeasurePower();
                
                if (powerMeasurement == null)
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Failed to measure power from power meter";
                    OnErrorOccurred(result.ErrorMessage);
                    return result;
                }

                result.PowerDbm = powerMeasurement.PowerValue;
                result.IsValid = true;

                OnUserActionOccurred("Manual Measure");

                return result;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = ex.Message;
                OnErrorOccurred("Manual measurement error: " + ex.Message);
                return result;
            }
        }

        /// <summary>
        /// Raises the StateChanged event.
        /// </summary>
        private void OnStateChanged(TuningState previousState, TuningState newState)
        {
            StateChanged?.Invoke(this, new TuningStateChangedEventArgs(previousState, newState));
        }

        /// <summary>
        /// Raises the ProgressUpdated event.
        /// </summary>
        private void OnProgressUpdated()
        {
            System.Diagnostics.Debug.WriteLine($"[TuningController] OnProgressUpdated firing. Stats: Iter={_statistics?.CurrentIteration}, Power={_statistics?.CurrentPowerDbm}");
            ProgressUpdated?.Invoke(this, new TuningProgressEventArgs(_statistics));
        }

        /// <summary>
        /// Raises the TuningCompleted event.
        /// </summary>
        private void OnTuningCompleted(TuningResult result)
        {
            TuningCompleted?.Invoke(this, new TuningCompletedEventArgs(result));
        }

        /// <summary>
        /// Raises the ErrorOccurred event.
        /// </summary>
        private void OnErrorOccurred(string errorMessage)
        {
            ErrorOccurred?.Invoke(this, new ErrorEventArgs(new Exception(errorMessage)));
        }

        /// <summary>
        /// Raises the UserActionOccurred event.
        /// </summary>
        private void OnUserActionOccurred(string actionName, Dictionary<string, string> parameters = null)
        {
            UserActionOccurred?.Invoke(this, new UserActionEventArgs
            {
                ActionName = actionName,
                Timestamp = DateTime.Now,
                Parameters = parameters ?? new Dictionary<string, string>()
            });
        }
    }
}
