using System;
using System.Threading;
using System.Threading.Tasks;
using Tegam._1830A.DeviceLibrary.Logging;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam._1830A.DeviceLibrary.Services;

namespace Tegam.WinFormsUI.Controllers
{
    /// <summary>
    /// Enhanced logging controller that extends DataLoggingController with settings tracking
    /// and automatic sampling capabilities.
    /// </summary>
    public class EnhancedLoggingController : DataLoggingController
    {
        private readonly ILogManager _logManager;
        private readonly IPowerMeterService _powerMeterService;
        private readonly FrequencyConfigurationController _frequencyController;
        private readonly SensorManagementController _sensorController;
        private readonly CalibrationController _calibrationController;
        private readonly PowerMeasurementController _powerMeasurementController;
        
        private Timer _autoSamplingTimer;
        private AutoSamplingConfig _autoSamplingConfig;
        private readonly object _samplingLock = new object();

        /// <summary>
        /// Event raised when automatic sampling completes.
        /// </summary>
        public event EventHandler AutoSamplingCompleted;

        /// <summary>
        /// Event raised when automatic sampling progress updates.
        /// </summary>
        public event EventHandler<int> AutoSamplingProgress;

        /// <summary>
        /// Initializes a new instance of the EnhancedLoggingController class.
        /// </summary>
        /// <param name="powerMeterService">The power meter service.</param>
        /// <param name="logManager">The log manager.</param>
        /// <param name="frequencyController">The frequency configuration controller.</param>
        /// <param name="sensorController">The sensor management controller.</param>
        /// <param name="calibrationController">The calibration controller.</param>
        /// <param name="powerMeasurementController">The power measurement controller (optional).</param>
        public EnhancedLoggingController(
            IPowerMeterService powerMeterService,
            ILogManager logManager,
            FrequencyConfigurationController frequencyController,
            SensorManagementController sensorController,
            CalibrationController calibrationController,
            PowerMeasurementController powerMeasurementController = null)
            : base(powerMeterService)
        {
            _powerMeterService = powerMeterService ?? throw new ArgumentNullException(nameof(powerMeterService));
            _logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));
            _frequencyController = frequencyController ?? throw new ArgumentNullException(nameof(frequencyController));
            _sensorController = sensorController ?? throw new ArgumentNullException(nameof(sensorController));
            _calibrationController = calibrationController ?? throw new ArgumentNullException(nameof(calibrationController));
            _powerMeasurementController = powerMeasurementController;

            // Subscribe to configuration change events
            _frequencyController.FrequencySet += OnFrequencySet;
            _frequencyController.FrequencyQueried += OnFrequencyQueried;
            _sensorController.SensorSelected += OnSensorSelected;
            _sensorController.CurrentSensorQueried += OnCurrentSensorQueried;
            _sensorController.AvailableSensorsQueried += OnAvailableSensorsQueried;
            _calibrationController.CalibrationStarted += OnCalibrationStarted;
            _calibrationController.CalibrationStatusQueried += OnCalibrationStatusQueried;
            powerMeterService.ConnectionStateChanged += OnConnectionStateChanged;
            
            // Subscribe to power measurement events if controller is provided
            if (_powerMeasurementController != null)
            {
                _powerMeasurementController.MeasurementCompleted += OnMeasurementCompleted;
            }
        }

        /// <summary>
        /// Handles the FrequencySet event from FrequencyConfigurationController.
        /// </summary>
        private void OnFrequencySet(object sender, EventArgs e)
        {
            // Don't query here - it can cause deadlocks
            // The actual value will be logged when FrequencyQueried event fires
            var settingEntry = new SettingEntry
            {
                SettingName = "Frequency",
                SettingValue = "Set",
                Context = "User set via UI"
            };
            _logManager.LogEntry(settingEntry);
        }

        /// <summary>
        /// Handles the SensorSelected event from SensorManagementController.
        /// </summary>
        private void OnSensorSelected(object sender, EventArgs e)
        {
            // Don't query here - it can cause deadlocks
            // The actual value will be logged when CurrentSensorQueried event fires
            var settingEntry = new SettingEntry
            {
                SettingName = "Sensor",
                SettingValue = "Selected",
                Context = "User selected via UI"
            };
            _logManager.LogEntry(settingEntry);
        }

        /// <summary>
        /// Handles the FrequencyQueried event from FrequencyConfigurationController.
        /// </summary>
        private void OnFrequencyQueried(object sender, FrequencyResponse frequency)
        {
            var settingEntry = new SettingEntry
            {
                SettingName = "Frequency",
                SettingValue = string.Format("{0} {1}", frequency.Frequency, frequency.Unit),
                Context = "Queried via UI"
            };
            _logManager.LogEntry(settingEntry);
        }

        /// <summary>
        /// Handles the CurrentSensorQueried event from SensorManagementController.
        /// </summary>
        private void OnCurrentSensorQueried(object sender, SensorInfo sensor)
        {
            var settingEntry = new SettingEntry
            {
                SettingName = "Current_Sensor",
                SettingValue = string.Format("Sensor {0}: {1}", sensor.SensorId, sensor.Name),
                Context = "Queried via UI"
            };
            _logManager.LogEntry(settingEntry);
        }

        /// <summary>
        /// Handles the AvailableSensorsQueried event from SensorManagementController.
        /// </summary>
        private void OnAvailableSensorsQueried(object sender, System.Collections.Generic.List<SensorInfo> sensors)
        {
            var settingEntry = new SettingEntry
            {
                SettingName = "Available_Sensors",
                SettingValue = string.Format("{0} sensors available", sensors.Count),
                Context = "Queried via UI"
            };
            _logManager.LogEntry(settingEntry);
        }

        /// <summary>
        /// Handles the CalibrationStarted event from CalibrationController.
        /// </summary>
        private void OnCalibrationStarted(object sender, EventArgs e)
        {
            var settingEntry = new SettingEntry
            {
                SettingName = "Calibration",
                SettingValue = "Started",
                Context = "User initiated via UI"
            };
            _logManager.LogEntry(settingEntry);
        }

        /// <summary>
        /// Handles the CalibrationStatusQueried event from CalibrationController.
        /// </summary>
        private void OnCalibrationStatusQueried(object sender, CalibrationStatus status)
        {
            string statusValue;
            if (status.IsCalibrating)
            {
                statusValue = "In Progress";
            }
            else if (status.IsComplete)
            {
                statusValue = status.IsSuccessful ? "Completed Successfully" : string.Format("Failed: {0}", status.ErrorMessage);
            }
            else
            {
                statusValue = "Idle";
            }

            var settingEntry = new SettingEntry
            {
                SettingName = "Calibration_Status",
                SettingValue = statusValue,
                Context = "Queried via UI"
            };
            _logManager.LogEntry(settingEntry);
        }

        /// <summary>
        /// Handles the MeasurementCompleted event from PowerMeasurementController.
        /// </summary>
        private void OnMeasurementCompleted(object sender, PowerMeasurement measurement)
        {
            var dataEntry = new DataEntry
            {
                Frequency = measurement.Frequency,
                FrequencyUnit = measurement.FrequencyUnit,
                Power = measurement.PowerValue,
                SensorId = measurement.SensorId,
                Timestamp = measurement.Timestamp
            };
            _logManager.LogEntry(dataEntry);
        }

        /// <summary>
        /// Handles the ConnectionStateChanged event from IPowerMeterService.
        /// </summary>
        private void OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            var settingEntry = new SettingEntry
            {
                SettingName = "Connection",
                SettingValue = e.IsConnected ? "Connected" : "Disconnected",
                Context = string.IsNullOrEmpty(e.ErrorMessage) ? "Device connection state changed" : e.ErrorMessage
            };
            _logManager.LogEntry(settingEntry);
        }

        /// <summary>
        /// Triggers a single manual measurement and logs it as a DataEntry.
        /// </summary>
        public void ManualSample()
        {
            try
            {
                var measurement = _powerMeterService.MeasurePower();
                
                var dataEntry = new DataEntry
                {
                    Frequency = measurement.Frequency,
                    FrequencyUnit = measurement.FrequencyUnit,
                    Power = measurement.PowerValue,
                    SensorId = measurement.SensorId,
                    Timestamp = measurement.Timestamp
                };
                
                _logManager.LogEntry(dataEntry);
            }
            catch (Exception)
            {
                // Errors will be logged via LogManager.WriteError event
            }
        }

        /// <summary>
        /// Starts automatic sampling with the specified rate and count.
        /// </summary>
        /// <param name="rateMs">Sample rate in milliseconds.</param>
        /// <param name="count">Total number of samples to collect.</param>
        public void StartAutomaticSampling(int rateMs, int count)
        {
            lock (_samplingLock)
            {
                if (_autoSamplingConfig != null && _autoSamplingConfig.IsActive)
                {
                    throw new InvalidOperationException("Automatic sampling is already active");
                }

                _autoSamplingConfig = new AutoSamplingConfig
                {
                    SampleRateMs = rateMs,
                    SampleCount = count,
                    IsActive = true,
                    CompletedCount = 0
                };

                // Log automatic sampling start
                var settingEntry = new SettingEntry
                {
                    SettingName = "Auto_Sampling",
                    SettingValue = string.Format("Started: rate={0}ms, count={1}", rateMs, count),
                    Context = "User initiated"
                };
                _logManager.LogEntry(settingEntry);

                // Start timer
                _autoSamplingTimer = new Timer(AutoSamplingCallback, null, 0, rateMs);
            }
        }

        /// <summary>
        /// Stops automatic sampling.
        /// </summary>
        public void StopAutomaticSampling()
        {
            lock (_samplingLock)
            {
                if (_autoSamplingConfig == null || !_autoSamplingConfig.IsActive)
                {
                    return;
                }

                int completedCount = _autoSamplingConfig.CompletedCount;
                _autoSamplingConfig.IsActive = false;

                // Stop and dispose timer
                if (_autoSamplingTimer != null)
                {
                    _autoSamplingTimer.Dispose();
                    _autoSamplingTimer = null;
                }

                // Log automatic sampling stop
                var settingEntry = new SettingEntry
                {
                    SettingName = "Auto_Sampling",
                    SettingValue = string.Format("Stopped: {0} samples collected", completedCount),
                    Context = "User initiated"
                };
                _logManager.LogEntry(settingEntry);

                _autoSamplingConfig = null;

                // Raise completion event
                AutoSamplingCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Timer callback for automatic sampling.
        /// </summary>
        private void AutoSamplingCallback(object state)
        {
            // Don't block - run measurement on thread pool
            Task.Run(() =>
            {
                lock (_samplingLock)
                {
                    if (_autoSamplingConfig == null || !_autoSamplingConfig.IsActive)
                    {
                        return;
                    }

                    try
                    {
                        // Trigger measurement (this may take time, so we're on a background thread)
                        var measurement = _powerMeterService.MeasurePower();
                        
                        var dataEntry = new DataEntry
                        {
                            Frequency = measurement.Frequency,
                            FrequencyUnit = measurement.FrequencyUnit,
                            Power = measurement.PowerValue,
                            SensorId = measurement.SensorId,
                            Timestamp = measurement.Timestamp
                        };
                        
                        _logManager.LogEntry(dataEntry);

                        _autoSamplingConfig.CompletedCount++;

                        // Raise progress event
                        AutoSamplingProgress?.Invoke(this, _autoSamplingConfig.CompletedCount);

                        // Check if we've reached the target count
                        if (_autoSamplingConfig.CompletedCount >= _autoSamplingConfig.SampleCount)
                        {
                            StopAutomaticSampling();
                        }
                    }
                    catch (Exception)
                    {
                        // Log error but continue sampling
                        // Note: OperationError event from base class will be raised by subscribers
                    }
                }
            });
        }

        /// <summary>
        /// Gets the current automatic sampling configuration.
        /// </summary>
        public AutoSamplingConfig GetAutoSamplingConfig()
        {
            lock (_samplingLock)
            {
                return _autoSamplingConfig;
            }
        }

        /// <summary>
        /// Logs a button click action for querying or changing settings.
        /// </summary>
        /// <param name="buttonName">The name of the button clicked.</param>
        /// <param name="action">The action performed.</param>
        public void LogButtonAction(string buttonName, string action)
        {
            var settingEntry = new SettingEntry
            {
                SettingName = buttonName,
                SettingValue = action,
                Context = "User button click"
            };
            _logManager.LogEntry(settingEntry);
        }

        /// <summary>
        /// Logs a setting query action.
        /// </summary>
        /// <param name="settingName">The name of the setting queried.</param>
        /// <param name="settingValue">The value retrieved.</param>
        public void LogSettingQuery(string settingName, string settingValue)
        {
            var settingEntry = new SettingEntry
            {
                SettingName = settingName,
                SettingValue = settingValue,
                Context = "Setting queried"
            };
            _logManager.LogEntry(settingEntry);
        }
    }
}
