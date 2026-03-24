using System;
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
        public async Task<bool> ConnectPowerMeterAsync(string powerMeterIp)
        {
            try
            {
                bool powerMeterConnected = _powerMeterService.Connect(powerMeterIp);
                if (!powerMeterConnected)
                {
                    OnErrorOccurred("Failed to connect to power meter at " + powerMeterIp);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                OnErrorOccurred("Power meter connection error: " + ex.Message);
                return false;
            }
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
                bool signalGenConnected = await _signalGeneratorService.ConnectAsync(signalGenIp);
                if (!signalGenConnected)
                {
                    OnErrorOccurred("Failed to connect to signal generator at " + signalGenIp);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
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

            var startTime = DateTime.Now;

            try
            {
                CurrentState = TuningState.Tuning;

                // Configure signal generator frequency
                var waveformParams = new Siglent.SDG6052X.DeviceLibrary.Models.WaveformParameters
                {
                    Frequency = parameters.FrequencyHz,
                    Amplitude = parameters.InitialVoltage,
                    Unit = Siglent.SDG6052X.DeviceLibrary.Models.AmplitudeUnit.Vpp
                };

                var setWaveformResult = await _signalGeneratorService.SetBasicWaveformAsync(
                    1, 
                    Siglent.SDG6052X.DeviceLibrary.Models.WaveformType.Sine, 
                    waveformParams);

                if (!setWaveformResult.Success)
                {
                    CurrentState = TuningState.Error;
                    OnErrorOccurred("Failed to configure signal generator: " + setWaveformResult.Message);
                    return;
                }

                // Configure power meter frequency
                var setFreqResult = _powerMeterService.SetFrequency(
                    parameters.FrequencyHz, 
                    Tegam._1830A.DeviceLibrary.Models.FrequencyUnit.Hz);

                if (!setFreqResult.IsSuccess)
                {
                    CurrentState = TuningState.Error;
                    OnErrorOccurred("Failed to configure power meter frequency: " + setFreqResult.ErrorMessage);
                    return;
                }

                // Configure power meter sensor
                var setSensorResult = _powerMeterService.SelectSensor(parameters.SensorId);
                if (!setSensorResult.IsSuccess)
                {
                    CurrentState = TuningState.Error;
                    OnErrorOccurred("Failed to select power meter sensor: " + setSensorResult.ErrorMessage);
                    return;
                }

                // Enable signal generator output
                var enableOutputResult = await _signalGeneratorService.SetOutputStateAsync(1, true);
                if (!enableOutputResult.Success)
                {
                    CurrentState = TuningState.Error;
                    OnErrorOccurred("Failed to enable signal generator output: " + enableOutputResult.Message);
                    return;
                }

                // Iterative measurement loop
                for (int iteration = 1; iteration <= parameters.MaxIterations; iteration++)
                {
                    // Check if tuning was stopped
                    if (CurrentState == TuningState.Aborted)
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
                        return;
                    }

                    _statistics.CurrentIteration = iteration;
                    CurrentState = TuningState.Measuring;

                    // Small delay for signal settling
                    await Task.Delay(100);

                    // Measure current power
                    var powerMeasurement = _powerMeterService.MeasurePower();
                    
                    if (powerMeasurement == null)
                    {
                        CurrentState = TuningState.Error;
                        await _signalGeneratorService.SetOutputStateAsync(1, false);
                        OnErrorOccurred("Failed to measure power from power meter");
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
                        return;
                    }

                    // Apply new voltage
                    _statistics.CurrentVoltage = newVoltage;
                    
                    var updateWaveformParams = new Siglent.SDG6052X.DeviceLibrary.Models.WaveformParameters
                    {
                        Frequency = parameters.FrequencyHz,
                        Amplitude = newVoltage,
                        Unit = Siglent.SDG6052X.DeviceLibrary.Models.AmplitudeUnit.Vpp
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

            // Transition to Aborted state
            // The tuning loop will detect this state change and terminate
            CurrentState = TuningState.Aborted;
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
    }
}
