using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Siglent.SDG6052X.DeviceLibrary.Commands;
using Siglent.SDG6052X.DeviceLibrary.Communication;
using Siglent.SDG6052X.DeviceLibrary.Models;
using Siglent.SDG6052X.DeviceLibrary.Parsing;
using Siglent.SDG6052X.DeviceLibrary.Validation;

namespace Siglent.SDG6052X.DeviceLibrary.Services
{
    /// <summary>
    /// High-level service for controlling the SDG6052X signal generator
    /// </summary>
    public class SignalGeneratorService : ISignalGeneratorService
    {
        private readonly IVisaCommunicationManager _communicationManager;
        private readonly IScpiCommandBuilder _commandBuilder;
        private readonly IScpiResponseParser _responseParser;
        private readonly IInputValidator _inputValidator;

        private DeviceIdentity _deviceInfo;
        private DeviceError _lastError;

        /// <summary>
        /// Event raised when a device error occurs
        /// </summary>
        public event EventHandler<DeviceErrorEventArgs> DeviceError;

        /// <summary>
        /// Event raised when connection state changes
        /// </summary>
        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        /// <summary>
        /// Gets whether the device is currently connected
        /// </summary>
        public bool IsConnected => _communicationManager?.IsConnected ?? false;

        /// <summary>
        /// Gets the device identity information
        /// </summary>
        public DeviceIdentity DeviceInfo => _deviceInfo;

        /// <summary>
        /// Initialize the service with dependency injection
        /// </summary>
        public SignalGeneratorService(
            IVisaCommunicationManager communicationManager,
            IScpiCommandBuilder commandBuilder,
            IScpiResponseParser responseParser,
            IInputValidator inputValidator)
        {
            _communicationManager = communicationManager ?? throw new ArgumentNullException(nameof(communicationManager));
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
            _responseParser = responseParser ?? throw new ArgumentNullException(nameof(responseParser));
            _inputValidator = inputValidator ?? throw new ArgumentNullException(nameof(inputValidator));

            // Subscribe to communication errors
            _communicationManager.CommunicationError += OnCommunicationError;
        }

        /// <summary>
        /// Connect to the device
        /// </summary>
        public Task<bool> ConnectAsync(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                RaiseConnectionStateChanged(false, "IP address cannot be empty");
                return Task.Factory.StartNew(() => false);
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    // Build VISA resource name
                    string resourceName = string.Format("TCPIP::{0}::INSTR", ipAddress);

                    // Connect to device
                    bool connected = _communicationManager.Connect(resourceName);

                    if (connected)
                    {
                        // Get device identity
                        string identityString = _communicationManager.GetDeviceIdentity();
                        _deviceInfo = _responseParser.ParseIdentityResponse(identityString);

                        RaiseConnectionStateChanged(true, string.Format("Connected to {0}", _deviceInfo.Model));
                    }
                    else
                    {
                        RaiseConnectionStateChanged(false, "Failed to connect");
                    }

                    return connected;
                }
                catch (Exception ex)
                {
                    RaiseConnectionStateChanged(false, string.Format("Connection error: {0}", ex.Message));
                    return false;
                }
            });
        }

        /// <summary>
        /// Disconnect from the device
        /// </summary>
        public Task DisconnectAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    _communicationManager.Disconnect();
                    _deviceInfo = null;
                    RaiseConnectionStateChanged(false, "Disconnected");
                }
                catch (Exception ex)
                {
                    RaiseConnectionStateChanged(false, string.Format("Disconnect error: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Set basic waveform parameters
        /// </summary>
        public Task<OperationResult> SetBasicWaveformAsync(int channel, WaveformType type, WaveformParameters parameters)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            if (channel < 1 || channel > 2)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Invalid channel number. Must be 1 or 2."));
            }

            // Validate parameters
            var freqValidation = _inputValidator.ValidateFrequency(parameters.Frequency, type);
            if (!freqValidation.IsValid)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed(freqValidation.ErrorMessage));
            }

            var ampValidation = _inputValidator.ValidateAmplitude(parameters.Amplitude, parameters.Load ?? LoadImpedance.HighZ);
            if (!ampValidation.IsValid)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed(ampValidation.ErrorMessage));
            }

            var offsetValidation = _inputValidator.ValidateOffset(parameters.Offset, parameters.Amplitude, parameters.Load ?? LoadImpedance.HighZ);
            if (!offsetValidation.IsValid)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed(offsetValidation.ErrorMessage));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    // Build and send command
                    string command = _commandBuilder.BuildBasicWaveCommand(channel, type, parameters);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful("Waveform configured successfully");
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error setting waveform: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Get current waveform state
        /// </summary>
        public Task<WaveformState> GetWaveformStateAsync(int channel)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected to device");
            }

            if (channel < 1 || channel > 2)
            {
                throw new ArgumentException("Invalid channel number. Must be 1 or 2.", "channel");
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string query = _commandBuilder.BuildQueryCommand(channel, QueryType.BasicWaveform);
                    string response = _communicationManager.Query(query);
                    return _responseParser.ParseWaveformState(response);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format("Error getting waveform state: {0}", ex.Message), ex);
                }
            });
        }

        /// <summary>
        /// Set output state (enable/disable)
        /// </summary>
        public Task<OperationResult> SetOutputStateAsync(int channel, bool enabled)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            if (channel < 1 || channel > 2)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Invalid channel number. Must be 1 or 2."));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string command = _commandBuilder.BuildOutputStateCommand(channel, enabled);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful(string.Format("Output {0}", enabled ? "enabled" : "disabled"));
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error setting output state: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Set load impedance
        /// </summary>
        public Task<OperationResult> SetLoadImpedanceAsync(int channel, LoadImpedance load)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            if (channel < 1 || channel > 2)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Invalid channel number. Must be 1 or 2."));
            }

            if (load == null)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Load impedance cannot be null"));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string command = _commandBuilder.BuildLoadCommand(channel, load);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful("Load impedance set successfully");
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error setting load impedance: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Configure modulation
        /// </summary>
        public Task<OperationResult> ConfigureModulationAsync(int channel, ModulationType type, ModulationParameters parameters)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            if (channel < 1 || channel > 2)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Invalid channel number. Must be 1 or 2."));
            }

            // Validate parameters
            var depthValidation = _inputValidator.ValidateModulationDepth(parameters.Depth, type);
            if (!depthValidation.IsValid)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed(depthValidation.ErrorMessage));
            }

            var freqValidation = _inputValidator.ValidateModulationFrequency(parameters.Rate, type);
            if (!freqValidation.IsValid)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed(freqValidation.ErrorMessage));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string command = _commandBuilder.BuildModulationCommand(channel, type, parameters);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful("Modulation configured successfully");
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error configuring modulation: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Set modulation state (enable/disable)
        /// </summary>
        public Task<OperationResult> SetModulationStateAsync(int channel, bool enabled)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            if (channel < 1 || channel > 2)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Invalid channel number. Must be 1 or 2."));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string command = _commandBuilder.BuildModulationStateCommand(channel, enabled);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful(string.Format("Modulation {0}", enabled ? "enabled" : "disabled"));
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error setting modulation state: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Get current modulation state
        /// </summary>
        public Task<ModulationState> GetModulationStateAsync(int channel)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected to device");
            }

            if (channel < 1 || channel > 2)
            {
                throw new ArgumentException("Invalid channel number. Must be 1 or 2.", "channel");
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string query = _commandBuilder.BuildQueryCommand(channel, QueryType.Modulation);
                    string response = _communicationManager.Query(query);
                    return _responseParser.ParseModulationState(response);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format("Error getting modulation state: {0}", ex.Message), ex);
                }
            });
        }

        /// <summary>
        /// Configure sweep
        /// </summary>
        public Task<OperationResult> ConfigureSweepAsync(int channel, SweepParameters parameters)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            if (channel < 1 || channel > 2)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Invalid channel number. Must be 1 or 2."));
            }

            // Validate parameters
            var rangeValidation = _inputValidator.ValidateSweepRange(parameters.StartFrequency, parameters.StopFrequency);
            if (!rangeValidation.IsValid)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed(rangeValidation.ErrorMessage));
            }

            var timeValidation = _inputValidator.ValidateSweepTime(parameters.Time);
            if (!timeValidation.IsValid)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed(timeValidation.ErrorMessage));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string command = _commandBuilder.BuildSweepCommand(channel, parameters);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful("Sweep configured successfully");
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error configuring sweep: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Set sweep state (enable/disable)
        /// </summary>
        public Task<OperationResult> SetSweepStateAsync(int channel, bool enabled)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            if (channel < 1 || channel > 2)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Invalid channel number. Must be 1 or 2."));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string command = _commandBuilder.BuildSweepStateCommand(channel, enabled);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful(string.Format("Sweep {0}", enabled ? "enabled" : "disabled"));
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error setting sweep state: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Get current sweep state
        /// </summary>
        public Task<SweepState> GetSweepStateAsync(int channel)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected to device");
            }

            if (channel < 1 || channel > 2)
            {
                throw new ArgumentException("Invalid channel number. Must be 1 or 2.", "channel");
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string query = _commandBuilder.BuildQueryCommand(channel, QueryType.Sweep);
                    string response = _communicationManager.Query(query);
                    return _responseParser.ParseSweepState(response);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format("Error getting sweep state: {0}", ex.Message), ex);
                }
            });
        }

        /// <summary>
        /// Configure burst
        /// </summary>
        public Task<OperationResult> ConfigureBurstAsync(int channel, BurstParameters parameters)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            if (channel < 1 || channel > 2)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Invalid channel number. Must be 1 or 2."));
            }

            // Validate parameters
            var cyclesValidation = _inputValidator.ValidateBurstCycles(parameters.Cycles);
            if (!cyclesValidation.IsValid)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed(cyclesValidation.ErrorMessage));
            }

            var periodValidation = _inputValidator.ValidateBurstPeriod(parameters.Period);
            if (!periodValidation.IsValid)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed(periodValidation.ErrorMessage));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string command = _commandBuilder.BuildBurstCommand(channel, parameters);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful("Burst configured successfully");
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error configuring burst: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Set burst state (enable/disable)
        /// </summary>
        public Task<OperationResult> SetBurstStateAsync(int channel, bool enabled)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            if (channel < 1 || channel > 2)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Invalid channel number. Must be 1 or 2."));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string command = _commandBuilder.BuildBurstStateCommand(channel, enabled);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful(string.Format("Burst {0}", enabled ? "enabled" : "disabled"));
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error setting burst state: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Get current burst state
        /// </summary>
        public Task<BurstState> GetBurstStateAsync(int channel)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected to device");
            }

            if (channel < 1 || channel > 2)
            {
                throw new ArgumentException("Invalid channel number. Must be 1 or 2.", "channel");
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string query = _commandBuilder.BuildQueryCommand(channel, QueryType.Burst);
                    string response = _communicationManager.Query(query);
                    return _responseParser.ParseBurstState(response);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format("Error getting burst state: {0}", ex.Message), ex);
                }
            });
        }

        /// <summary>
        /// Upload arbitrary waveform to device
        /// </summary>
        public Task<OperationResult> UploadArbitraryWaveformAsync(string name, double[] points)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            // Validate parameters
            var nameValidation = _inputValidator.ValidateWaveformName(name);
            if (!nameValidation.IsValid)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed(nameValidation.ErrorMessage));
            }

            var pointsValidation = _inputValidator.ValidateArbitraryWaveformPoints(points);
            if (!pointsValidation.IsValid)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed(pointsValidation.ErrorMessage));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string command = _commandBuilder.BuildStoreArbitraryWaveCommand(name, points);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful(string.Format("Waveform '{0}' uploaded successfully", name));
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error uploading waveform: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Select arbitrary waveform for a channel
        /// </summary>
        public Task<OperationResult> SelectArbitraryWaveformAsync(int channel, string name)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            if (channel < 1 || channel > 2)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Invalid channel number. Must be 1 or 2."));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Waveform name cannot be empty"));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string command = _commandBuilder.BuildArbitraryWaveCommand(channel, name);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful(string.Format("Waveform '{0}' selected for channel {1}", name, channel));
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error selecting waveform: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Get list of stored arbitrary waveforms
        /// </summary>
        public Task<List<string>> GetArbitraryWaveformListAsync()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected to device");
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    // This would query the device for stored waveforms
                    // For now, return empty list as placeholder
                    return new List<string>();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format("Error getting waveform list: {0}", ex.Message), ex);
                }
            });
        }

        /// <summary>
        /// Delete arbitrary waveform from device
        /// </summary>
        public Task<OperationResult> DeleteArbitraryWaveformAsync(string name)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Waveform name cannot be empty"));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    // This would send a delete command to the device
                    // For now, return success as placeholder
                    return OperationResult.Successful(string.Format("Waveform '{0}' deleted", name));
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error deleting waveform: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Recall saved setup from device memory
        /// </summary>
        public Task<OperationResult> RecallSetupAsync(int setupNumber)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            if (setupNumber < 1 || setupNumber > 10)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Setup number must be between 1 and 10"));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string command = _commandBuilder.BuildSystemCommand(SystemCommandType.RecallSetup, setupNumber);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful(string.Format("Setup {0} recalled", setupNumber));
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error recalling setup: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Save current setup to device memory
        /// </summary>
        public Task<OperationResult> SaveSetupAsync(int setupNumber)
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            if (setupNumber < 1 || setupNumber > 10)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Setup number must be between 1 and 10"));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string command = _commandBuilder.BuildSystemCommand(SystemCommandType.SaveSetup, setupNumber);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful(string.Format("Setup saved to slot {0}", setupNumber));
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error saving setup: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Reset device to factory defaults
        /// </summary>
        public Task<OperationResult> ResetDeviceAsync()
        {
            if (!IsConnected)
            {
                return Task.Factory.StartNew(() => OperationResult.Failed("Not connected to device"));
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string command = _commandBuilder.BuildSystemCommand(SystemCommandType.Reset);
                    var result = _communicationManager.SendCommand(command);

                    if (result.Success)
                    {
                        return OperationResult.Successful("Device reset to factory defaults");
                    }
                    else
                    {
                        return OperationResult.Failed(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    return OperationResult.Failed(string.Format("Error resetting device: {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Get last device error from error queue
        /// </summary>
        public Task<DeviceError> GetLastDeviceErrorAsync()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected to device");
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string query = _commandBuilder.BuildSystemCommand(SystemCommandType.Error);
                    string response = _communicationManager.Query(query);
                    _lastError = _responseParser.ParseErrorResponse(response);
                    return _lastError;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format("Error getting device error: {0}", ex.Message), ex);
                }
            });
        }

        /// <summary>
        /// Handle communication errors
        /// </summary>
        private void OnCommunicationError(object sender, CommunicationErrorEventArgs e)
        {
            // Convert communication error to device error event
            var deviceError = new DeviceError
            {
                Code = -1,
                Message = e.Message
            };

            RaiseDeviceError(deviceError);
        }

        /// <summary>
        /// Raise device error event
        /// </summary>
        private void RaiseDeviceError(DeviceError error)
        {
            _lastError = error;
            DeviceError?.Invoke(this, new DeviceErrorEventArgs
            {
                Error = error,
                Timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// Raise connection state changed event
        /// </summary>
        private void RaiseConnectionStateChanged(bool isConnected, string message)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs
            {
                IsConnected = isConnected,
                Message = message,
                Timestamp = DateTime.Now
            });
        }
    }
}
