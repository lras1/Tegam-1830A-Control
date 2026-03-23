using System;
using System.Collections.Generic;
using System.Threading;
using Tegam._1830A.DeviceLibrary.Commands;
using Tegam._1830A.DeviceLibrary.Communication;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam._1830A.DeviceLibrary.Parsing;
using Tegam._1830A.DeviceLibrary.Validation;

namespace Tegam._1830A.DeviceLibrary.Services
{
    /// <summary>
    /// Power Meter Service that provides high-level device control operations.
    /// </summary>
    public class PowerMeterService : IPowerMeterService
    {
        private readonly IVisaCommunicationManager _communicationManager;
        private readonly IScpiCommandBuilder _commandBuilder;
        private readonly IScpiResponseParser _responseParser;
        private readonly IInputValidator _validator;
        private bool _isConnected;
        private DeviceIdentity _deviceInfo;
        private double _cachedFrequency;
        private FrequencyUnit _cachedFrequencyUnit;
        private int _cachedSensorId;

        /// <summary>
        /// Gets a value indicating whether the device is currently connected.
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected && _communicationManager.IsConnected; }
        }

        /// <summary>
        /// Gets the device identity information.
        /// </summary>
        public DeviceIdentity DeviceInfo
        {
            get { return _deviceInfo; }
        }

        /// <summary>
        /// Event raised when a measurement is received.
        /// </summary>
        public event EventHandler<MeasurementEventArgs> MeasurementReceived;

        /// <summary>
        /// Event raised when a device error occurs.
        /// </summary>
        public event EventHandler<DeviceErrorEventArgs> DeviceError;

        /// <summary>
        /// Event raised when the connection state changes.
        /// </summary>
        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        /// <summary>
        /// Initializes a new instance of the PowerMeterService class.
        /// </summary>
        public PowerMeterService(
            IVisaCommunicationManager communicationManager,
            IScpiCommandBuilder commandBuilder,
            IScpiResponseParser responseParser,
            IInputValidator validator)
        {
            if (communicationManager == null)
                throw new ArgumentNullException("communicationManager");
            if (commandBuilder == null)
                throw new ArgumentNullException("commandBuilder");
            if (responseParser == null)
                throw new ArgumentNullException("responseParser");
            if (validator == null)
                throw new ArgumentNullException("validator");

            _communicationManager = communicationManager;
            _commandBuilder = commandBuilder;
            _responseParser = responseParser;
            _validator = validator;
            _isConnected = false;
            _deviceInfo = null;
            _cachedFrequency = 2.4;
            _cachedFrequencyUnit = FrequencyUnit.GHz;
            _cachedSensorId = 1;

            // Subscribe to communication errors
            _communicationManager.CommunicationError += OnCommunicationError;
        }

        /// <summary>
        /// Connects to the device at the specified IP address.
        /// </summary>
        public bool Connect(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                OnConnectionStateChanged(false, "IP address cannot be null or empty.");
                return false;
            }

            try
            {
                // Build VISA resource name
                string resourceName = string.Format("TCPIP::{0}::INSTR", ipAddress);

                // Connect using communication manager
                bool connected = _communicationManager.Connect(resourceName, 5000);

                if (!connected)
                {
                    OnConnectionStateChanged(false, "Failed to connect to device.");
                    return false;
                }

                // Query device identity
                string identityString = _communicationManager.GetDeviceIdentity();
                if (string.IsNullOrEmpty(identityString))
                {
                    _communicationManager.Disconnect();
                    OnConnectionStateChanged(false, "Failed to retrieve device identity.");
                    return false;
                }

                // Parse device identity
                try
                {
                    _deviceInfo = _responseParser.ParseIdentityResponse(identityString);
                }
                catch (Exception ex)
                {
                    _communicationManager.Disconnect();
                    OnConnectionStateChanged(false, string.Format("Failed to parse device identity: {0}", ex.Message));
                    return false;
                }

                // Verify device model
                if (!_deviceInfo.Model.Contains("1830A"))
                {
                    _communicationManager.Disconnect();
                    OnConnectionStateChanged(false, "Device model does not match Tegam 1830A.");
                    return false;
                }

                _isConnected = true;
                OnConnectionStateChanged(true);
                return true;
            }
            catch (Exception ex)
            {
                OnConnectionStateChanged(false, string.Format("Connection error: {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Disconnects from the device.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _communicationManager.Disconnect();
                _isConnected = false;
                _deviceInfo = null;
                OnConnectionStateChanged(false);
            }
            catch (Exception ex)
            {
                OnConnectionStateChanged(false, string.Format("Disconnect error: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Sets the measurement frequency.
        /// </summary>
        public OperationResult SetFrequency(double frequency, FrequencyUnit unit)
        {
            if (!IsConnected)
                return OperationResult.Failure("Device is not connected.");

            // Validate frequency
            ValidationResult validationResult = _validator.ValidateFrequency(frequency, unit);
            if (!validationResult.IsValid)
                return OperationResult.Failure(validationResult.ErrorMessage);

            try
            {
                // Build and send command
                string command = _commandBuilder.BuildFrequencyCommand(frequency, unit);
                CommandResult result = _communicationManager.SendCommand(command);

                if (!result.IsSuccess)
                {
                    OnDeviceError(new DeviceError(1, result.ErrorMessage));
                    return OperationResult.Failure(result.ErrorMessage);
                }

                // Cache the frequency
                _cachedFrequency = frequency;
                _cachedFrequencyUnit = unit;

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(1, ex.Message));
                return OperationResult.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Gets the current measurement frequency.
        /// </summary>
        public FrequencyResponse GetFrequency()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Device is not connected.");

            try
            {
                string query = _commandBuilder.BuildFrequencyQueryCommand();
                string response = _communicationManager.Query(query);
                FrequencyResponse freqResponse = _responseParser.ParseFrequencyResponse(response);

                // Update cache
                _cachedFrequency = freqResponse.Frequency;
                _cachedFrequencyUnit = freqResponse.Unit;

                return freqResponse;
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(2, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// Measures power at the current frequency.
        /// </summary>
        public PowerMeasurement MeasurePower()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Device is not connected.");

            try
            {
                string query = _commandBuilder.BuildMeasurePowerQueryCommand();
                string response = _communicationManager.Query(query);
                PowerMeasurement measurement = _responseParser.ParsePowerMeasurement(response);

                // Set frequency and sensor from cached values
                measurement = new PowerMeasurement(
                    measurement.PowerValue,
                    measurement.PowerUnit,
                    _cachedFrequency,
                    _cachedFrequencyUnit,
                    _cachedSensorId);

                // Add timestamp
                measurement.Timestamp = DateTime.Now;

                OnMeasurementReceived(measurement);
                return measurement;
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(3, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// Measures power at a specific frequency.
        /// </summary>
        public PowerMeasurement MeasurePower(double frequency, FrequencyUnit unit)
        {
            // Set frequency first
            OperationResult freqResult = SetFrequency(frequency, unit);
            if (!freqResult.IsSuccess)
                throw new InvalidOperationException(freqResult.ErrorMessage);

            // Then measure power
            return MeasurePower();
        }

        /// <summary>
        /// Takes multiple power measurements with a delay between each.
        /// </summary>
        public List<PowerMeasurement> MeasureMultiple(int count, int delayMs)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Device is not connected.");

            // Validate parameters
            ValidationResult countResult = _validator.ValidateMeasurementCount(count);
            if (!countResult.IsValid)
                throw new ArgumentException(countResult.ErrorMessage);

            ValidationResult delayResult = _validator.ValidateMeasurementDelay(delayMs);
            if (!delayResult.IsValid)
                throw new ArgumentException(delayResult.ErrorMessage);

            var measurements = new List<PowerMeasurement>();

            try
            {
                // Use high-resolution timing for precise intervals
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                long targetIntervalTicks = delayMs * System.Diagnostics.Stopwatch.Frequency / 1000;

                for (int i = 0; i < count; i++)
                {
                    // Calculate when this measurement should occur
                    long targetTicks = i * targetIntervalTicks;
                    
                    // Wait until we reach the target time
                    long remainingTicks = targetTicks - stopwatch.ElapsedTicks;
                    
                    if (remainingTicks > 0)
                    {
                        // For intervals > 15ms, use Thread.Sleep for most of the wait
                        long sleepThresholdTicks = System.Diagnostics.Stopwatch.Frequency * 15 / 1000; // 15ms in ticks
                        
                        if (remainingTicks > sleepThresholdTicks)
                        {
                            // Sleep for most of the time, leaving 15ms for spin-wait
                            int sleepMs = (int)((remainingTicks - sleepThresholdTicks) * 1000 / System.Diagnostics.Stopwatch.Frequency);
                            if (sleepMs > 0)
                            {
                                Thread.Sleep(sleepMs);
                            }
                        }
                        
                        // Spin-wait for the remaining time to get precise timing
                        while (stopwatch.ElapsedTicks < targetTicks)
                        {
                            Thread.SpinWait(10); // Prevents 100% CPU usage while maintaining precision
                        }
                    }

                    // Take the measurement
                    PowerMeasurement measurement = MeasurePower();
                    measurements.Add(measurement);
                }

                stopwatch.Stop();
                return measurements;
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(4, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// Selects a measurement sensor.
        /// </summary>
        public OperationResult SelectSensor(int sensorId)
        {
            if (!IsConnected)
                return OperationResult.Failure("Device is not connected.");

            // Validate sensor ID
            ValidationResult validationResult = _validator.ValidateSensorId(sensorId);
            if (!validationResult.IsValid)
                return OperationResult.Failure(validationResult.ErrorMessage);

            try
            {
                string command = _commandBuilder.BuildSelectSensorCommand(sensorId);
                CommandResult result = _communicationManager.SendCommand(command);

                if (!result.IsSuccess)
                {
                    OnDeviceError(new DeviceError(5, result.ErrorMessage));
                    return OperationResult.Failure(result.ErrorMessage);
                }

                // Cache the sensor ID
                _cachedSensorId = sensorId;

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(5, ex.Message));
                return OperationResult.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Gets information about the currently selected sensor.
        /// </summary>
        public SensorInfo GetCurrentSensor()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Device is not connected.");

            try
            {
                string query = _commandBuilder.BuildQuerySensorCommand();
                string response = _communicationManager.Query(query);
                
                // Parse sensor ID from response
                int sensorId = int.Parse(response.Trim());
                _cachedSensorId = sensorId;

                // Get available sensors and find the current one
                List<SensorInfo> sensors = GetAvailableSensors();
                foreach (SensorInfo sensor in sensors)
                {
                    if (sensor.SensorId == sensorId)
                        return sensor;
                }

                throw new InvalidOperationException("Current sensor not found in available sensors list.");
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(6, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// Gets a list of all available sensors.
        /// </summary>
        public List<SensorInfo> GetAvailableSensors()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Device is not connected.");

            try
            {
                string query = _commandBuilder.BuildQueryAvailableSensorsCommand();
                string response = _communicationManager.Query(query);
                List<SensorInfo> sensors = _responseParser.ParseAvailableSensors(response);
                return sensors;
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(7, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// Starts device calibration.
        /// </summary>
        public OperationResult Calibrate(CalibrationMode mode)
        {
            if (!IsConnected)
                return OperationResult.Failure("Device is not connected.");

            // Validate calibration mode
            ValidationResult validationResult = _validator.ValidateCalibrationMode(mode);
            if (!validationResult.IsValid)
                return OperationResult.Failure(validationResult.ErrorMessage);

            try
            {
                string command = _commandBuilder.BuildCalibrateCommand(mode);
                CommandResult result = _communicationManager.SendCommand(command);

                if (!result.IsSuccess)
                {
                    OnDeviceError(new DeviceError(8, result.ErrorMessage));
                    return OperationResult.Failure(result.ErrorMessage);
                }

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(8, ex.Message));
                return OperationResult.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Gets the current calibration status.
        /// </summary>
        public CalibrationStatus GetCalibrationStatus()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Device is not connected.");

            try
            {
                string query = _commandBuilder.BuildQueryCalibrationStatusCommand();
                string response = _communicationManager.Query(query);
                CalibrationStatus status = _responseParser.ParseCalibrationStatus(response);
                return status;
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(9, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// Starts data logging to a file.
        /// </summary>
        public OperationResult StartLogging(string filename)
        {
            if (!IsConnected)
                return OperationResult.Failure("Device is not connected.");

            // Validate filename
            ValidationResult validationResult = _validator.ValidateFilename(filename);
            if (!validationResult.IsValid)
                return OperationResult.Failure(validationResult.ErrorMessage);

            try
            {
                string command = _commandBuilder.BuildStartLoggingCommand(filename);
                CommandResult result = _communicationManager.SendCommand(command);

                if (!result.IsSuccess)
                {
                    OnDeviceError(new DeviceError(10, result.ErrorMessage));
                    return OperationResult.Failure(result.ErrorMessage);
                }

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(10, ex.Message));
                return OperationResult.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Stops data logging.
        /// </summary>
        public OperationResult StopLogging()
        {
            if (!IsConnected)
                return OperationResult.Failure("Device is not connected.");

            try
            {
                string command = _commandBuilder.BuildStopLoggingCommand();
                CommandResult result = _communicationManager.SendCommand(command);

                if (!result.IsSuccess)
                {
                    OnDeviceError(new DeviceError(11, result.ErrorMessage));
                    return OperationResult.Failure(result.ErrorMessage);
                }

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(11, ex.Message));
                return OperationResult.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Checks if data logging is currently active.
        /// </summary>
        public bool IsLogging()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Device is not connected.");

            try
            {
                string query = _commandBuilder.BuildQueryLoggingStatusCommand();
                string response = _communicationManager.Query(query);
                
                // Parse logging status - format: "1,filename" or "0,"
                string[] parts = response.Split(',');
                bool isLogging = parts[0].Trim() == "1";
                return isLogging;
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(12, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// Gets the system status.
        /// </summary>
        public SystemStatus GetSystemStatus()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Device is not connected.");

            try
            {
                string query = _commandBuilder.BuildSystemCommand("STATUS");
                string response = _communicationManager.Query(query);
                SystemStatus status = _responseParser.ParseSystemStatus(response);
                return status;
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(13, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// Resets the device.
        /// </summary>
        public OperationResult ResetDevice()
        {
            if (!IsConnected)
                return OperationResult.Failure("Device is not connected.");

            try
            {
                string command = _commandBuilder.BuildSystemCommand("RESET");
                CommandResult result = _communicationManager.SendCommand(command);

                if (!result.IsSuccess)
                {
                    OnDeviceError(new DeviceError(14, result.ErrorMessage));
                    return OperationResult.Failure(result.ErrorMessage);
                }

                // Reset cached values
                _cachedFrequency = 2.4;
                _cachedFrequencyUnit = FrequencyUnit.GHz;
                _cachedSensorId = 1;

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(14, ex.Message));
                return OperationResult.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Gets all pending errors from the device error queue.
        /// </summary>
        public List<DeviceError> GetErrorQueue()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Device is not connected.");

            var errors = new List<DeviceError>();

            try
            {
                // Query error queue until no more errors
                while (true)
                {
                    string query = _commandBuilder.BuildSystemCommand("ERROR");
                    string response = _communicationManager.Query(query);

                    // Check if response indicates no error (typically "0" or "0,")
                    if (response.Trim() == "0" || response.Trim() == "0,")
                        break;

                    DeviceError error = _responseParser.ParseErrorResponse(response);
                    errors.Add(error);
                }

                return errors;
            }
            catch (Exception ex)
            {
                OnDeviceError(new DeviceError(15, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// Raises the MeasurementReceived event.
        /// </summary>
        protected virtual void OnMeasurementReceived(PowerMeasurement measurement)
        {
            if (MeasurementReceived != null)
                MeasurementReceived(this, new MeasurementEventArgs(measurement));
        }

        /// <summary>
        /// Raises the DeviceError event.
        /// </summary>
        protected virtual void OnDeviceError(DeviceError error)
        {
            if (DeviceError != null)
                DeviceError(this, new DeviceErrorEventArgs(error));
        }

        /// <summary>
        /// Raises the ConnectionStateChanged event.
        /// </summary>
        protected virtual void OnConnectionStateChanged(bool isConnected, string errorMessage = null)
        {
            if (ConnectionStateChanged != null)
                ConnectionStateChanged(this, new ConnectionStateChangedEventArgs(isConnected, errorMessage));
        }

        /// <summary>
        /// Handles communication errors from the communication manager.
        /// </summary>
        private void OnCommunicationError(object sender, CommunicationErrorEventArgs e)
        {
            if (_isConnected)
            {
                _isConnected = false;
                OnConnectionStateChanged(false, e.ErrorMessage);
            }
        }
    }
}
