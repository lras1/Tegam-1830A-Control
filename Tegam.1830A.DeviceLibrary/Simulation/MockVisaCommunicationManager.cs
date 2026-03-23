using System;
using System.Threading;
using Tegam._1830A.DeviceLibrary.Communication;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Simulation
{
    /// <summary>
    /// Mock VISA communication manager that simulates device communication for testing purposes.
    /// </summary>
    public class MockVisaCommunicationManager : IVisaCommunicationManager
    {
        private SimulatedDeviceState _deviceState;
        private bool _isConnected;
        private bool _disposed;

        /// <summary>
        /// Gets a value indicating whether the device is currently connected.
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected; }
        }

        /// <summary>
        /// Event raised when a communication error occurs.
        /// </summary>
        public event EventHandler<CommunicationErrorEventArgs> CommunicationError;

        /// <summary>
        /// Initializes a new instance of the MockVisaCommunicationManager class.
        /// </summary>
        public MockVisaCommunicationManager()
        {
            _deviceState = new SimulatedDeviceState();
            _isConnected = false;
            _disposed = false;
        }

        /// <summary>
        /// Establishes a simulated connection to the device.
        /// </summary>
        public bool Connect(string resourceName, int timeout = 5000)
        {
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
                    OnCommunicationError("Simulated connection loss.");
                    return false;
                }

                // Check if we should simulate timeout
                if (_deviceState.ShouldSimulateTimeout)
                {
                    OnCommunicationError("Simulated timeout.");
                    return false;
                }

                _isConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                OnCommunicationError(string.Format("Connection failed: {0}", ex.Message), ex);
                return false;
            }
        }

        /// <summary>
        /// Closes the simulated connection to the device.
        /// </summary>
        public void Disconnect()
        {
            _isConnected = false;
        }

        /// <summary>
        /// Sends a simulated command to the device.
        /// </summary>
        public CommandResult SendCommand(string command)
        {
            if (!IsConnected)
                return new CommandResult(false, null, "Device is not connected.");

            if (string.IsNullOrWhiteSpace(command))
                return new CommandResult(false, null, "Command cannot be null or empty.");

            try
            {
                // Check if we should simulate an error
                if (_deviceState.ShouldSimulateError)
                {
                    _deviceState.ShouldSimulateError = false;
                    return new CommandResult(false, null, "Simulated device error.");
                }

                // Check if we should simulate timeout
                if (_deviceState.ShouldSimulateTimeout)
                {
                    _deviceState.ShouldSimulateTimeout = false;
                    throw new TimeoutException("Simulated timeout.");
                }

                // Process the command
                ProcessCommand(command);
                return new CommandResult(true, "OK");
            }
            catch (TimeoutException ex)
            {
                string errorMsg = string.Format("Command timeout: {0}", ex.Message);
                OnCommunicationError(errorMsg, ex);
                return new CommandResult(false, null, errorMsg);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Command failed: {0}", ex.Message);
                OnCommunicationError(errorMsg, ex);
                return new CommandResult(false, null, errorMsg);
            }
        }

        /// <summary>
        /// Sends a simulated query command to the device.
        /// </summary>
        public string Query(string query)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Device is not connected.");

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be null or empty.", nameof(query));

            try
            {
                // Check if we should simulate an error
                if (_deviceState.ShouldSimulateError)
                {
                    _deviceState.ShouldSimulateError = false;
                    throw new InvalidOperationException("Simulated device error.");
                }

                // Check if we should simulate timeout
                if (_deviceState.ShouldSimulateTimeout)
                {
                    _deviceState.ShouldSimulateTimeout = false;
                    throw new TimeoutException("Simulated timeout.");
                }

                // Process the query and return response
                string response = ProcessQuery(query);
                return response;
            }
            catch (TimeoutException ex)
            {
                string errorMsg = string.Format("Query timeout: {0}", ex.Message);
                OnCommunicationError(errorMsg, ex);
                throw new InvalidOperationException(errorMsg, ex);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Query failed: {0}", ex.Message);
                OnCommunicationError(errorMsg, ex);
                throw new InvalidOperationException(errorMsg, ex);
            }
        }

        /// <summary>
        /// Sends a simulated binary query command to the device.
        /// </summary>
        public byte[] QueryBinary(string query)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Device is not connected.");

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be null or empty.", nameof(query));

            try
            {
                string response = Query(query);
                return System.Text.Encoding.ASCII.GetBytes(response);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Binary query failed: {0}", ex.Message);
                OnCommunicationError(errorMsg, ex);
                throw new InvalidOperationException(errorMsg, ex);
            }
        }

        /// <summary>
        /// Gets the simulated device identity string.
        /// </summary>
        public string GetDeviceIdentity()
        {
            if (!IsConnected)
                return null;

            try
            {
                return _deviceState.GenerateIdentityResponse();
            }
            catch (Exception ex)
            {
                OnCommunicationError(string.Format("Failed to get device identity: {0}", ex.Message), ex);
                return null;
            }
        }

        /// <summary>
        /// Processes a simulated command and updates device state.
        /// </summary>
        private void ProcessCommand(string command)
        {
            string upperCommand = command.ToUpperInvariant().Trim();

            if (upperCommand.StartsWith("FREQ "))
            {
                // Parse frequency command: FREQ {value} {unit}
                string[] parts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    double frequency;
                    if (double.TryParse(parts[1], out frequency))
                    {
                        _deviceState.CurrentFrequency = frequency;
                        FrequencyUnit unit = ParseFrequencyUnit(parts[2]);
                        _deviceState.CurrentFrequencyUnit = unit;
                    }
                }
            }
            else if (upperCommand.StartsWith("SENS:SEL "))
            {
                // Parse sensor selection command: SENS:SEL {sensorId}
                string[] parts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    int sensorId;
                    if (int.TryParse(parts[1], out sensorId) && sensorId >= 1 && sensorId <= 4)
                    {
                        _deviceState.CurrentSensorId = sensorId;
                    }
                }
            }
            else if (upperCommand.StartsWith("CAL:START "))
            {
                // Parse calibration command: CAL:START {mode}
                _deviceState.IsCalibrating = true;
            }
            else if (upperCommand == "LOG:STOP")
            {
                _deviceState.IsLogging = false;
                _deviceState.LogFilename = null;
            }
            else if (upperCommand.StartsWith("LOG:START "))
            {
                // Parse logging command: LOG:START "{filename}"
                int quoteStart = command.IndexOf('"');
                int quoteEnd = command.LastIndexOf('"');
                if (quoteStart >= 0 && quoteEnd > quoteStart)
                {
                    _deviceState.LogFilename = command.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
                    _deviceState.IsLogging = true;
                }
            }
            else if (upperCommand == "*RST")
            {
                _deviceState.Reset();
            }
            else if (upperCommand == "*CLS")
            {
                // Clear status - no state change needed
            }
        }

        /// <summary>
        /// Processes a simulated query command and returns the response.
        /// </summary>
        private string ProcessQuery(string query)
        {
            string upperQuery = query.ToUpperInvariant().Trim();

            if (upperQuery == "FREQ?")
            {
                return _deviceState.GenerateFrequencyResponse();
            }
            else if (upperQuery == "MEAS:POW?")
            {
                return _deviceState.GeneratePowerMeasurementResponse();
            }
            else if (upperQuery == "SENS:SEL?")
            {
                return _deviceState.GenerateSensorSelectionResponse();
            }
            else if (upperQuery == "SENS:LIST?")
            {
                return _deviceState.GenerateAvailableSensorsResponse();
            }
            else if (upperQuery == "CAL:STAT?")
            {
                return _deviceState.GenerateCalibrationStatusResponse();
            }
            else if (upperQuery == "LOG:STAT?")
            {
                return _deviceState.GenerateLoggingStatusResponse();
            }
            else if (upperQuery == "*IDN?")
            {
                return _deviceState.GenerateIdentityResponse();
            }
            else if (upperQuery == "*STB?")
            {
                return _deviceState.GenerateSystemStatusResponse();
            }
            else if (upperQuery == "SYST:ERR?")
            {
                return _deviceState.GenerateErrorResponse();
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unknown query: {0}", query));
            }
        }

        /// <summary>
        /// Parses a frequency unit string and returns the corresponding FrequencyUnit enum value.
        /// </summary>
        private FrequencyUnit ParseFrequencyUnit(string unitString)
        {
            string upper = unitString.Trim().ToUpperInvariant();

            switch (upper)
            {
                case "HZ":
                    return FrequencyUnit.Hz;
                case "KHZ":
                    return FrequencyUnit.kHz;
                case "MHZ":
                    return FrequencyUnit.MHz;
                case "GHZ":
                    return FrequencyUnit.GHz;
                default:
                    return FrequencyUnit.GHz;
            }
        }

        /// <summary>
        /// Gets the current device state for testing purposes.
        /// </summary>
        public SimulatedDeviceState GetDeviceState()
        {
            return _deviceState;
        }

        /// <summary>
        /// Raises the CommunicationError event.
        /// </summary>
        protected virtual void OnCommunicationError(string errorMessage, Exception exception = null)
        {
            CommunicationError?.Invoke(this, new CommunicationErrorEventArgs(errorMessage, exception));
        }

        /// <summary>
        /// Disposes the communication manager and releases all resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected dispose method.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Disconnect();
            }

            _disposed = true;
        }

        /// <summary>
        /// Finalizer to ensure resources are cleaned up.
        /// </summary>
        ~MockVisaCommunicationManager()
        {
            Dispose(false);
        }
    }
}
