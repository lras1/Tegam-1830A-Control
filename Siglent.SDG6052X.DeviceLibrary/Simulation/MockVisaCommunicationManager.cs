using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Siglent.SDG6052X.DeviceLibrary.Communication;
using Siglent.SDG6052X.DeviceLibrary.Models;

namespace Siglent.SDG6052X.DeviceLibrary.Simulation
{
    /// <summary>
    /// Mock VISA communication manager for testing without physical hardware
    /// </summary>
    public class MockVisaCommunicationManager : IVisaCommunicationManager
    {
        private SimulatedDeviceState _deviceState;
        private bool _isConnected;
        private int _timeout;

        /// <summary>
        /// Event raised when a communication error occurs
        /// </summary>
        public event EventHandler<CommunicationErrorEventArgs> CommunicationError;

        /// <summary>
        /// Gets whether the device is currently connected
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// Initialize mock communication manager
        /// </summary>
        public MockVisaCommunicationManager()
        {
            _deviceState = new SimulatedDeviceState();
            _isConnected = false;
        }

        /// <summary>
        /// Connect to the simulated device
        /// </summary>
        public bool Connect(string resourceName, int timeout = 5000)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                RaiseCommunicationError("Resource name cannot be null or empty", null);
                return false;
            }

            _timeout = timeout;
            _isConnected = true;
            _deviceState = new SimulatedDeviceState(); // Reset state on connect
            return true;
        }

        /// <summary>
        /// Disconnect from the simulated device
        /// </summary>
        public void Disconnect()
        {
            _isConnected = false;
        }

        /// <summary>
        /// Send a command to the simulated device
        /// </summary>
        public CommandResult SendCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return new CommandResult
                {
                    Success = false,
                    Response = "Command cannot be null or empty"
                };
            }

            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (!_isConnected)
                {
                    return new CommandResult
                    {
                        Success = false,
                        Response = "Not connected to device"
                    };
                }

                // Check for simulated errors
                if (_deviceState.SimulateConnectionLoss)
                {
                    _isConnected = false;
                    throw new InvalidOperationException("Connection lost");
                }

                if (_deviceState.SimulateTimeout)
                {
                    throw new TimeoutException("Command timeout");
                }

                // Process the command
                ProcessCommand(command);

                stopwatch.Stop();

                return new CommandResult
                {
                    Success = true,
                    Response = "Command sent successfully",
                    ExecutionTimeMs = (int)stopwatch.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                RaiseCommunicationError($"Error sending command: {command}", ex);

                return new CommandResult
                {
                    Success = false,
                    Response = ex.Message,
                    Exception = ex,
                    ExecutionTimeMs = (int)stopwatch.ElapsedMilliseconds
                };
            }
        }

        /// <summary>
        /// Send a query to the simulated device
        /// </summary>
        public string Query(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Query cannot be null or empty", nameof(query));
            }

            if (!_isConnected)
            {
                throw new InvalidOperationException("Not connected to device");
            }

            // Check for simulated errors
            if (_deviceState.SimulateConnectionLoss)
            {
                _isConnected = false;
                throw new InvalidOperationException("Connection lost");
            }

            if (_deviceState.SimulateTimeout)
            {
                throw new TimeoutException("Query timeout");
            }

            // Process the query and return response
            return ProcessQuery(query);
        }

        /// <summary>
        /// Send a binary query to the simulated device
        /// </summary>
        public byte[] QueryBinary(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Query cannot be null or empty", nameof(query));
            }

            if (!_isConnected)
            {
                throw new InvalidOperationException("Not connected to device");
            }

            // For now, return empty binary data
            // This would be used for arbitrary waveform data retrieval
            return new byte[0];
        }

        /// <summary>
        /// Send a command asynchronously
        /// </summary>
        public Task<CommandResult> SendCommandAsync(string command)
        {
            return Task.Factory.StartNew(() => SendCommand(command));
        }

        /// <summary>
        /// Send a query asynchronously
        /// </summary>
        public Task<string> QueryAsync(string query)
        {
            return Task.Factory.StartNew(() => Query(query));
        }

        /// <summary>
        /// Get the simulated device identity
        /// </summary>
        public string GetDeviceIdentity()
        {
            return Query("*IDN?");
        }

        /// <summary>
        /// Get channel state for test verification
        /// </summary>
        public SimulatedChannelState GetChannelState(int channel)
        {
            if (_deviceState.Channels.ContainsKey(channel))
            {
                return _deviceState.Channels[channel];
            }
            return null;
        }

        /// <summary>
        /// Simulate an error by adding it to the error queue
        /// </summary>
        public void SimulateError(int code, string message)
        {
            _deviceState.ErrorQueue.Enqueue(new DeviceError
            {
                Code = code,
                Message = message
            });
        }

        /// <summary>
        /// Simulate connection loss
        /// </summary>
        public void SimulateConnectionLoss()
        {
            _deviceState.SimulateConnectionLoss = true;
        }

        /// <summary>
        /// Simulate timeout
        /// </summary>
        public void SimulateTimeout()
        {
            _deviceState.SimulateTimeout = true;
        }

        /// <summary>
        /// Raise communication error event
        /// </summary>
        private void RaiseCommunicationError(string message, Exception exception)
        {
            CommunicationError?.Invoke(this, new CommunicationErrorEventArgs
            {
                Message = message,
                Exception = exception,
                Timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// Dispose of resources
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }

        /// <summary>
        /// Process a command and update simulated state
        /// </summary>
        private void ProcessCommand(string command)
        {
            command = command.Trim();

            // System commands
            if (command == "*CLS")
            {
                _deviceState.ErrorQueue.Clear();
                return;
            }

            if (command == "*RST")
            {
                _deviceState.Reset();
                return;
            }

            // Parse channel number from command
            var channelMatch = Regex.Match(command, @"C(\d+):");
            if (!channelMatch.Success)
            {
                return; // Not a channel-specific command
            }

            int channel = int.Parse(channelMatch.Groups[1].Value);
            if (!_deviceState.Channels.ContainsKey(channel))
            {
                SimulateError(-100, "Invalid channel");
                return;
            }

            var state = _deviceState.Channels[channel];

            // Basic waveform command (BSWV)
            if (command.Contains("BSWV"))
            {
                ParseBasicWaveformCommand(command, state);
            }
            // Output state command (OUTP)
            else if (command.Contains("OUTP"))
            {
                var match = Regex.Match(command, @"OUTP\s+(ON|OFF)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    state.OutputEnabled = match.Groups[1].Value.ToUpper() == "ON";
                }
            }
            // Load command (OUTP:LOAD)
            else if (command.Contains("OUTP:LOAD"))
            {
                var match = Regex.Match(command, @"OUTP:LOAD\s+(\w+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    string loadValue = match.Groups[1].Value.ToUpper();
                    if (loadValue == "HZ")
                    {
                        state.Load = LoadImpedance.HighZ;
                    }
                    else if (double.TryParse(loadValue, out double impedance))
                    {
                        state.Load = LoadImpedance.Custom(impedance);
                    }
                }
            }
            // Modulation commands
            else if (command.Contains("MDWV"))
            {
                ParseModulationCommand(command, state);
            }
            else if (command.Contains("MDWV:STATE"))
            {
                var match = Regex.Match(command, @"MDWV:STATE\s+(ON|OFF)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    state.ModulationEnabled = match.Groups[1].Value.ToUpper() == "ON";
                }
            }
            // Sweep commands
            else if (command.Contains("SWWV"))
            {
                ParseSweepCommand(command, state);
            }
            else if (command.Contains("SWWV:STATE"))
            {
                var match = Regex.Match(command, @"SWWV:STATE\s+(ON|OFF)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    state.SweepEnabled = match.Groups[1].Value.ToUpper() == "ON";
                }
            }
            // Burst commands
            else if (command.Contains("BTWV"))
            {
                ParseBurstCommand(command, state);
            }
            else if (command.Contains("BTWV:STATE"))
            {
                var match = Regex.Match(command, @"BTWV:STATE\s+(ON|OFF)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    state.BurstEnabled = match.Groups[1].Value.ToUpper() == "ON";
                }
            }
            // Arbitrary waveform selection
            else if (command.Contains("ARWV"))
            {
                var match = Regex.Match(command, @"ARWV\s+NAME,(\w+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    state.SelectedArbitraryWaveform = match.Groups[1].Value;
                }
            }
        }

        /// <summary>
        /// Process a query and return simulated response
        /// </summary>
        private string ProcessQuery(string query)
        {
            query = query.Trim();

            // System queries
            if (query == "*IDN?")
            {
                return GenerateIdentityResponse();
            }

            if (query == "SYST:ERR?")
            {
                if (_deviceState.ErrorQueue.Count > 0)
                {
                    var error = _deviceState.ErrorQueue.Dequeue();
                    return $"{error.Code},\"{error.Message}\"";
                }
                return "0,\"No Error\"";
            }

            // Parse channel number from query
            var channelMatch = Regex.Match(query, @"C(\d+):");
            if (!channelMatch.Success)
            {
                return "ERROR";
            }

            int channel = int.Parse(channelMatch.Groups[1].Value);
            if (!_deviceState.Channels.ContainsKey(channel))
            {
                return "ERROR";
            }

            var state = _deviceState.Channels[channel];

            // Basic waveform query
            if (query.Contains("BSWV?"))
            {
                return GenerateWaveformQueryResponse(channel, state);
            }
            // Output state query
            else if (query.Contains("OUTP?"))
            {
                return state.OutputEnabled ? "ON" : "OFF";
            }
            // Load query
            else if (query.Contains("OUTP:LOAD?"))
            {
                if (state.Load.Type == LoadType.HighZ)
                {
                    return "HZ";
                }
                return state.Load.Value.ToString("F1", CultureInfo.InvariantCulture);
            }
            // Modulation query
            else if (query.Contains("MDWV?"))
            {
                return GenerateModulationQueryResponse(channel, state);
            }
            else if (query.Contains("MDWV:STATE?"))
            {
                return state.ModulationEnabled ? "ON" : "OFF";
            }
            // Sweep query
            else if (query.Contains("SWWV?"))
            {
                return GenerateSweepQueryResponse(channel, state);
            }
            else if (query.Contains("SWWV:STATE?"))
            {
                return state.SweepEnabled ? "ON" : "OFF";
            }
            // Burst query
            else if (query.Contains("BTWV?"))
            {
                return GenerateBurstQueryResponse(channel, state);
            }
            else if (query.Contains("BTWV:STATE?"))
            {
                return state.BurstEnabled ? "ON" : "OFF";
            }

            return "ERROR";
        }

        /// <summary>
        /// Parse basic waveform command and update state
        /// </summary>
        private void ParseBasicWaveformCommand(string command, SimulatedChannelState state)
        {
            // Parse waveform type
            var waveMatch = Regex.Match(command, @"WVTP,(\w+)", RegexOptions.IgnoreCase);
            if (waveMatch.Success)
            {
                string waveType = waveMatch.Groups[1].Value.ToUpper();
                state.WaveformType = ParseWaveformType(waveType);
            }

            // Parse frequency
            var freqMatch = Regex.Match(command, @"FRQ,([\d.]+)(HZ|KHZ|MHZ)?", RegexOptions.IgnoreCase);
            if (freqMatch.Success)
            {
                double freq = double.Parse(freqMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                string unit = freqMatch.Groups[2].Value.ToUpper();
                state.Frequency = ConvertFrequencyToHz(freq, unit);
            }

            // Parse amplitude
            var ampMatch = Regex.Match(command, @"AMP,([\d.]+)(VPP|VRMS|DBM)?", RegexOptions.IgnoreCase);
            if (ampMatch.Success)
            {
                state.Amplitude = double.Parse(ampMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                string unit = ampMatch.Groups[2].Value.ToUpper();
                state.AmplitudeUnit = ParseAmplitudeUnit(unit);
            }

            // Parse offset
            var offsetMatch = Regex.Match(command, @"OFST,([\d.-]+)V?", RegexOptions.IgnoreCase);
            if (offsetMatch.Success)
            {
                state.Offset = double.Parse(offsetMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            }

            // Parse phase
            var phaseMatch = Regex.Match(command, @"PHSE,([\d.-]+)", RegexOptions.IgnoreCase);
            if (phaseMatch.Success)
            {
                state.Phase = double.Parse(phaseMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            }

            // Parse duty cycle
            var dutyMatch = Regex.Match(command, @"DUTY,([\d.]+)", RegexOptions.IgnoreCase);
            if (dutyMatch.Success)
            {
                state.DutyCycle = double.Parse(dutyMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Parse modulation command and update state
        /// </summary>
        private void ParseModulationCommand(string command, SimulatedChannelState state)
        {
            // Parse modulation type
            var typeMatch = Regex.Match(command, @"MDWV\s+(\w+)", RegexOptions.IgnoreCase);
            if (typeMatch.Success)
            {
                string modType = typeMatch.Groups[1].Value.ToUpper();
                state.ModulationType = ParseModulationType(modType);
            }

            // Parse modulation depth
            var depthMatch = Regex.Match(command, @"DEPTH,([\d.]+)", RegexOptions.IgnoreCase);
            if (depthMatch.Success)
            {
                state.ModulationDepth = double.Parse(depthMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            }

            // Parse modulation frequency
            var freqMatch = Regex.Match(command, @"FRQ,([\d.]+)(HZ|KHZ|MHZ)?", RegexOptions.IgnoreCase);
            if (freqMatch.Success)
            {
                double freq = double.Parse(freqMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                string unit = freqMatch.Groups[2].Value.ToUpper();
                state.ModulationFrequency = ConvertFrequencyToHz(freq, unit);
            }

            // Parse deviation
            var devMatch = Regex.Match(command, @"DEVI,([\d.]+)(HZ|KHZ|MHZ)?", RegexOptions.IgnoreCase);
            if (devMatch.Success)
            {
                double dev = double.Parse(devMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                string unit = devMatch.Groups[2].Value.ToUpper();
                state.ModulationDeviation = ConvertFrequencyToHz(dev, unit);
            }
        }

        /// <summary>
        /// Parse sweep command and update state
        /// </summary>
        private void ParseSweepCommand(string command, SimulatedChannelState state)
        {
            // Parse start frequency
            var startMatch = Regex.Match(command, @"START,([\d.]+)(HZ|KHZ|MHZ)?", RegexOptions.IgnoreCase);
            if (startMatch.Success)
            {
                double freq = double.Parse(startMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                string unit = startMatch.Groups[2].Value.ToUpper();
                state.SweepStartFrequency = ConvertFrequencyToHz(freq, unit);
            }

            // Parse stop frequency
            var stopMatch = Regex.Match(command, @"STOP,([\d.]+)(HZ|KHZ|MHZ)?", RegexOptions.IgnoreCase);
            if (stopMatch.Success)
            {
                double freq = double.Parse(stopMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                string unit = stopMatch.Groups[2].Value.ToUpper();
                state.SweepStopFrequency = ConvertFrequencyToHz(freq, unit);
            }

            // Parse sweep time
            var timeMatch = Regex.Match(command, @"TIME,([\d.]+)", RegexOptions.IgnoreCase);
            if (timeMatch.Success)
            {
                state.SweepTime = double.Parse(timeMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            }

            // Parse sweep type
            var typeMatch = Regex.Match(command, @"SWTP,(\w+)", RegexOptions.IgnoreCase);
            if (typeMatch.Success)
            {
                string sweepType = typeMatch.Groups[1].Value.ToUpper();
                state.SweepType = sweepType == "LOG" ? SweepType.Logarithmic : SweepType.Linear;
            }

            // Parse sweep direction
            var dirMatch = Regex.Match(command, @"DIR,(\w+)", RegexOptions.IgnoreCase);
            if (dirMatch.Success)
            {
                string dir = dirMatch.Groups[1].Value.ToUpper();
                state.SweepDirection = ParseSweepDirection(dir);
            }
        }

        /// <summary>
        /// Parse burst command and update state
        /// </summary>
        private void ParseBurstCommand(string command, SimulatedChannelState state)
        {
            // Parse burst mode
            var modeMatch = Regex.Match(command, @"GATE,(\w+)", RegexOptions.IgnoreCase);
            if (modeMatch.Success)
            {
                string mode = modeMatch.Groups[1].Value.ToUpper();
                state.BurstMode = mode == "NCYC" ? BurstMode.NCycle : BurstMode.Gated;
            }

            // Parse cycles
            var cyclesMatch = Regex.Match(command, @"TRSR,(\d+)", RegexOptions.IgnoreCase);
            if (cyclesMatch.Success)
            {
                state.BurstCycles = int.Parse(cyclesMatch.Groups[1].Value);
            }

            // Parse period
            var periodMatch = Regex.Match(command, @"PRD,([\d.]+)", RegexOptions.IgnoreCase);
            if (periodMatch.Success)
            {
                state.BurstPeriod = double.Parse(periodMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Generate identity response
        /// </summary>
        private string GenerateIdentityResponse()
        {
            return $"{_deviceState.Identity.Manufacturer},{_deviceState.Identity.Model}," +
                   $"{_deviceState.Identity.SerialNumber},{_deviceState.Identity.FirmwareVersion}";
        }

        /// <summary>
        /// Generate waveform query response
        /// </summary>
        private string GenerateWaveformQueryResponse(int channel, SimulatedChannelState state)
        {
            string waveType = FormatWaveformType(state.WaveformType);
            string freq = FormatFrequency(state.Frequency);
            string amp = FormatAmplitude(state.Amplitude, state.AmplitudeUnit);
            string offset = $"{state.Offset:F3}V";
            string phase = $"{state.Phase:F1}";

            var response = $"C{channel}:BSWV WVTP,{waveType},FRQ,{freq},AMP,{amp},OFST,{offset},PHSE,{phase}";

            if (state.WaveformType == WaveformType.Square || state.WaveformType == WaveformType.Pulse)
            {
                response += $",DUTY,{state.DutyCycle:F2}";
            }

            return response;
        }

        /// <summary>
        /// Generate modulation query response
        /// </summary>
        private string GenerateModulationQueryResponse(int channel, SimulatedChannelState state)
        {
            string modType = FormatModulationType(state.ModulationType);
            string depth = $"{state.ModulationDepth:F1}";
            string freq = FormatFrequency(state.ModulationFrequency);

            return $"C{channel}:MDWV {modType},DEPTH,{depth},FRQ,{freq}";
        }

        /// <summary>
        /// Generate sweep query response
        /// </summary>
        private string GenerateSweepQueryResponse(int channel, SimulatedChannelState state)
        {
            string start = FormatFrequency(state.SweepStartFrequency);
            string stop = FormatFrequency(state.SweepStopFrequency);
            string time = $"{state.SweepTime:F3}S";
            string type = state.SweepType == SweepType.Linear ? "LINE" : "LOG";

            return $"C{channel}:SWWV START,{start},STOP,{stop},TIME,{time},SWTP,{type}";
        }

        /// <summary>
        /// Generate burst query response
        /// </summary>
        private string GenerateBurstQueryResponse(int channel, SimulatedChannelState state)
        {
            string mode = state.BurstMode == BurstMode.NCycle ? "NCYC" : "GATE";
            string cycles = $"{state.BurstCycles}";
            string period = $"{state.BurstPeriod:F6}S";

            return $"C{channel}:BTWV GATE,{mode},TRSR,{cycles},PRD,{period}";
        }

        // Helper methods for parsing and formatting
        private WaveformType ParseWaveformType(string type)
        {
            switch (type)
            {
                case "SINE": return WaveformType.Sine;
                case "SQUARE": return WaveformType.Square;
                case "RAMP": return WaveformType.Ramp;
                case "PULSE": return WaveformType.Pulse;
                case "NOISE": return WaveformType.Noise;
                case "ARB": return WaveformType.Arbitrary;
                case "DC": return WaveformType.DC;
                case "PRBS": return WaveformType.PRBS;
                case "IQ": return WaveformType.IQ;
                default: return WaveformType.Sine;
            }
        }

        private string FormatWaveformType(WaveformType type)
        {
            switch (type)
            {
                case WaveformType.Sine: return "SINE";
                case WaveformType.Square: return "SQUARE";
                case WaveformType.Ramp: return "RAMP";
                case WaveformType.Pulse: return "PULSE";
                case WaveformType.Noise: return "NOISE";
                case WaveformType.Arbitrary: return "ARB";
                case WaveformType.DC: return "DC";
                case WaveformType.PRBS: return "PRBS";
                case WaveformType.IQ: return "IQ";
                default: return "SINE";
            }
        }

        private ModulationType ParseModulationType(string type)
        {
            switch (type)
            {
                case "AM": return ModulationType.AM;
                case "FM": return ModulationType.FM;
                case "PM": return ModulationType.PM;
                case "PWM": return ModulationType.PWM;
                case "FSK": return ModulationType.FSK;
                case "ASK": return ModulationType.ASK;
                case "PSK": return ModulationType.PSK;
                default: return ModulationType.AM;
            }
        }

        private string FormatModulationType(ModulationType type)
        {
            return type.ToString().ToUpper();
        }

        private SweepDirection ParseSweepDirection(string dir)
        {
            switch (dir)
            {
                case "UP": return SweepDirection.Up;
                case "DOWN": return SweepDirection.Down;
                case "UPDOWN": return SweepDirection.UpDown;
                default: return SweepDirection.Up;
            }
        }

        private AmplitudeUnit ParseAmplitudeUnit(string unit)
        {
            switch (unit)
            {
                case "VPP": return AmplitudeUnit.Vpp;
                case "VRMS": return AmplitudeUnit.Vrms;
                case "DBM": return AmplitudeUnit.dBm;
                default: return AmplitudeUnit.Vpp;
            }
        }

        private double ConvertFrequencyToHz(double value, string unit)
        {
            switch (unit)
            {
                case "KHZ": return value * 1000;
                case "MHZ": return value * 1000000;
                default: return value;
            }
        }

        private string FormatFrequency(double hz)
        {
            if (hz >= 1000000)
            {
                return $"{hz / 1000000:F6}MHZ";
            }
            else if (hz >= 1000)
            {
                return $"{hz / 1000:F3}KHZ";
            }
            else
            {
                return $"{hz:F3}HZ";
            }
        }

        private string FormatAmplitude(double value, AmplitudeUnit unit)
        {
            switch (unit)
            {
                case AmplitudeUnit.Vpp: return $"{value:F3}VPP";
                case AmplitudeUnit.Vrms: return $"{value:F3}VRMS";
                case AmplitudeUnit.dBm: return $"{value:F2}DBM";
                default: return $"{value:F3}VPP";
            }
        }
    }
}
