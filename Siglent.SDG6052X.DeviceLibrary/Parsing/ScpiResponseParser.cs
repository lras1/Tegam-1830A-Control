using Siglent.SDG6052X.DeviceLibrary.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Siglent.SDG6052X.DeviceLibrary.Parsing
{
    /// <summary>
    /// Parses SCPI responses from the SDG6052X into strongly-typed objects
    /// </summary>
    public class ScpiResponseParser : IScpiResponseParser
    {
        /// <summary>
        /// Parse a boolean response (ON/OFF, 1/0)
        /// </summary>
        public bool ParseBooleanResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty", nameof(response));

            string normalized = response.Trim().ToUpperInvariant();
            
            if (normalized == "ON" || normalized == "1")
                return true;
            
            if (normalized == "OFF" || normalized == "0")
                return false;

            throw new FormatException($"Invalid boolean response: {response}");
        }

        /// <summary>
        /// Parse a numeric response with optional units
        /// </summary>
        public double ParseNumericResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty", nameof(response));

            string normalized = response.Trim().ToUpperInvariant();
            
            // Try to parse frequency units
            double? freqValue = TryParseFrequency(normalized);
            if (freqValue.HasValue)
                return freqValue.Value;

            // Try to parse voltage units
            double? voltValue = TryParseVoltage(normalized);
            if (voltValue.HasValue)
                return voltValue.Value;

            // Try to parse dBm
            double? dbmValue = TryParseDbm(normalized);
            if (dbmValue.HasValue)
                return dbmValue.Value;

            // Try to parse as plain number
            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double plainValue))
                return plainValue;

            throw new FormatException($"Invalid numeric response: {response}");
        }

        /// <summary>
        /// Parse a string response, removing quotes if present
        /// </summary>
        public string ParseStringResponse(string response)
        {
            if (response == null)
                return null;

            string trimmed = response.Trim();
            
            // Remove surrounding quotes if present
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\"") && trimmed.Length >= 2)
                return trimmed.Substring(1, trimmed.Length - 2);

            return trimmed;
        }

        /// <summary>
        /// Parse waveform state from BSWV query response
        /// Example: "C1:BSWV WVTP,SINE,FRQ,1000HZ,AMP,5VPP,OFST,0V,PHSE,0"
        /// </summary>
        public WaveformState ParseWaveformState(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty", nameof(response));

            var state = new WaveformState();
            var parameters = ParseKeyValuePairs(response);

            if (parameters.ContainsKey("WVTP"))
                state.WaveformType = MapToWaveformType(parameters["WVTP"]);

            if (parameters.ContainsKey("FRQ"))
                state.Frequency = ParseNumericResponse(parameters["FRQ"]);

            if (parameters.ContainsKey("AMP"))
            {
                string ampStr = parameters["AMP"];
                state.Amplitude = ParseNumericResponse(ampStr);
                state.Unit = DetermineAmplitudeUnit(ampStr);
            }

            if (parameters.ContainsKey("OFST"))
                state.Offset = ParseNumericResponse(parameters["OFST"]);

            if (parameters.ContainsKey("PHSE"))
                state.Phase = ParseNumericResponse(parameters["PHSE"]);

            if (parameters.ContainsKey("DUTY"))
                state.DutyCycle = ParseNumericResponse(parameters["DUTY"]);

            return state;
        }

        /// <summary>
        /// Parse modulation state from modulation query response
        /// </summary>
        public ModulationState ParseModulationState(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty", nameof(response));

            var state = new ModulationState();
            var parameters = ParseKeyValuePairs(response);

            if (parameters.ContainsKey("STATE"))
                state.Enabled = ParseBooleanResponse(parameters["STATE"]);

            if (parameters.ContainsKey("MDTP"))
                state.Type = MapToModulationType(parameters["MDTP"]);

            if (parameters.ContainsKey("SRC"))
                state.Source = MapToModulationSource(parameters["SRC"]);

            if (parameters.ContainsKey("DEPTH") || parameters.ContainsKey("DEVI"))
            {
                if (parameters.ContainsKey("DEPTH"))
                    state.Depth = ParseNumericResponse(parameters["DEPTH"]);
                
                if (parameters.ContainsKey("DEVI"))
                    state.Deviation = ParseNumericResponse(parameters["DEVI"]);
            }

            if (parameters.ContainsKey("FRQ"))
                state.Rate = ParseNumericResponse(parameters["FRQ"]);

            if (parameters.ContainsKey("CARR_WVTP"))
                state.ModulationWaveform = MapToWaveformType(parameters["CARR_WVTP"]);

            return state;
        }

        /// <summary>
        /// Parse sweep state from sweep query response
        /// </summary>
        public SweepState ParseSweepState(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty", nameof(response));

            var state = new SweepState();
            var parameters = ParseKeyValuePairs(response);

            if (parameters.ContainsKey("STATE"))
                state.Enabled = ParseBooleanResponse(parameters["STATE"]);

            if (parameters.ContainsKey("START"))
                state.StartFrequency = ParseNumericResponse(parameters["START"]);

            if (parameters.ContainsKey("STOP"))
                state.StopFrequency = ParseNumericResponse(parameters["STOP"]);

            if (parameters.ContainsKey("TIME"))
                state.Time = ParseNumericResponse(parameters["TIME"]);

            if (parameters.ContainsKey("SWTP"))
                state.Type = MapToSweepType(parameters["SWTP"]);

            if (parameters.ContainsKey("DIR"))
                state.Direction = MapToSweepDirection(parameters["DIR"]);

            if (parameters.ContainsKey("TRSR"))
                state.TriggerSource = MapToTriggerSource(parameters["TRSR"]);

            return state;
        }

        /// <summary>
        /// Parse burst state from burst query response
        /// </summary>
        public BurstState ParseBurstState(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty", nameof(response));

            var state = new BurstState();
            var parameters = ParseKeyValuePairs(response);

            if (parameters.ContainsKey("STATE"))
                state.Enabled = ParseBooleanResponse(parameters["STATE"]);

            if (parameters.ContainsKey("MODE"))
                state.Mode = MapToBurstMode(parameters["MODE"]);

            if (parameters.ContainsKey("TRSR"))
                state.TriggerSource = MapToTriggerSource(parameters["TRSR"]);

            if (parameters.ContainsKey("EDGE"))
                state.TriggerEdge = MapToTriggerEdge(parameters["EDGE"]);

            if (parameters.ContainsKey("CYCLES"))
                state.Cycles = (int)ParseNumericResponse(parameters["CYCLES"]);

            if (parameters.ContainsKey("PRD"))
                state.Period = ParseNumericResponse(parameters["PRD"]);

            if (parameters.ContainsKey("STPS"))
                state.StartPhase = ParseNumericResponse(parameters["STPS"]);

            return state;
        }

        /// <summary>
        /// Parse device identity from *IDN? response
        /// Example: "Siglent Technologies,SDG6052X,SDG00000000001,1.01.01.32"
        /// </summary>
        public DeviceIdentity ParseIdentityResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty", nameof(response));

            string[] parts = response.Split(',');
            
            if (parts.Length < 4)
                throw new FormatException($"Invalid identity response format: {response}");

            return new DeviceIdentity
            {
                Manufacturer = parts[0].Trim(),
                Model = parts[1].Trim(),
                SerialNumber = parts[2].Trim(),
                FirmwareVersion = parts[3].Trim()
            };
        }

        /// <summary>
        /// Parse arbitrary waveform data from binary response
        /// </summary>
        public double[] ParseArbitraryWaveformData(byte[] binaryData)
        {
            if (binaryData == null || binaryData.Length == 0)
                throw new ArgumentException("Binary data cannot be null or empty", nameof(binaryData));

            // Assuming 16-bit signed integers (2 bytes per sample)
            int sampleCount = binaryData.Length / 2;
            double[] points = new double[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                short sample = BitConverter.ToInt16(binaryData, i * 2);
                // Normalize to -1.0 to +1.0 range
                points[i] = sample / 32768.0;
            }

            return points;
        }

        /// <summary>
        /// Parse error response from SYST:ERR? query
        /// Example: "0,No Error" or "-100,Command error"
        /// </summary>
        public DeviceError ParseErrorResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty", nameof(response));

            string[] parts = response.Split(new[] { ',' }, 2);
            
            if (parts.Length < 2)
                throw new FormatException($"Invalid error response format: {response}");

            if (!int.TryParse(parts[0].Trim(), out int code))
                throw new FormatException($"Invalid error code: {parts[0]}");

            return new DeviceError
            {
                Code = code,
                Message = ParseStringResponse(parts[1])
            };
        }

        #region Helper Methods - Unit Parsing

        /// <summary>
        /// Try to parse frequency with units (Hz, kHz, MHz, GHz)
        /// </summary>
        private double? TryParseFrequency(string value)
        {
            if (value.EndsWith("GHZ"))
            {
                if (double.TryParse(value.Substring(0, value.Length - 3), NumberStyles.Float, CultureInfo.InvariantCulture, out double num))
                    return num * 1e9;
            }
            else if (value.EndsWith("MHZ"))
            {
                if (double.TryParse(value.Substring(0, value.Length - 3), NumberStyles.Float, CultureInfo.InvariantCulture, out double num))
                    return num * 1e6;
            }
            else if (value.EndsWith("KHZ"))
            {
                if (double.TryParse(value.Substring(0, value.Length - 3), NumberStyles.Float, CultureInfo.InvariantCulture, out double num))
                    return num * 1e3;
            }
            else if (value.EndsWith("HZ"))
            {
                if (double.TryParse(value.Substring(0, value.Length - 2), NumberStyles.Float, CultureInfo.InvariantCulture, out double num))
                    return num;
            }

            return null;
        }

        /// <summary>
        /// Try to parse voltage with units (V, mV, VPP, VRMS)
        /// </summary>
        private double? TryParseVoltage(string value)
        {
            if (value.EndsWith("VPP"))
            {
                if (double.TryParse(value.Substring(0, value.Length - 3), NumberStyles.Float, CultureInfo.InvariantCulture, out double num))
                    return num;
            }
            else if (value.EndsWith("VRMS"))
            {
                if (double.TryParse(value.Substring(0, value.Length - 4), NumberStyles.Float, CultureInfo.InvariantCulture, out double num))
                    return num;
            }
            else if (value.EndsWith("MV"))
            {
                if (double.TryParse(value.Substring(0, value.Length - 2), NumberStyles.Float, CultureInfo.InvariantCulture, out double num))
                    return num / 1000.0;
            }
            else if (value.EndsWith("V"))
            {
                if (double.TryParse(value.Substring(0, value.Length - 1), NumberStyles.Float, CultureInfo.InvariantCulture, out double num))
                    return num;
            }

            return null;
        }

        /// <summary>
        /// Try to parse dBm value
        /// </summary>
        private double? TryParseDbm(string value)
        {
            if (value.EndsWith("DBM"))
            {
                if (double.TryParse(value.Substring(0, value.Length - 3), NumberStyles.Float, CultureInfo.InvariantCulture, out double num))
                    return num;
            }

            return null;
        }

        /// <summary>
        /// Determine amplitude unit from response string
        /// </summary>
        private AmplitudeUnit DetermineAmplitudeUnit(string value)
        {
            string normalized = value.ToUpperInvariant();
            
            if (normalized.Contains("VPP"))
                return AmplitudeUnit.Vpp;
            
            if (normalized.Contains("VRMS"))
                return AmplitudeUnit.Vrms;
            
            if (normalized.Contains("DBM"))
                return AmplitudeUnit.dBm;

            return AmplitudeUnit.Vpp; // Default
        }

        #endregion

        #region Helper Methods - SCPI to Enum Mapping

        /// <summary>
        /// Map SCPI waveform type string to enum
        /// </summary>
        private WaveformType MapToWaveformType(string scpiValue)
        {
            switch (scpiValue.ToUpperInvariant())
            {
                case "SINE":
                    return WaveformType.Sine;
                case "SQUARE":
                    return WaveformType.Square;
                case "RAMP":
                    return WaveformType.Ramp;
                case "PULSE":
                    return WaveformType.Pulse;
                case "NOISE":
                    return WaveformType.Noise;
                case "ARB":
                case "ARBITRARY":
                    return WaveformType.Arbitrary;
                case "DC":
                    return WaveformType.DC;
                case "PRBS":
                    return WaveformType.PRBS;
                case "IQ":
                    return WaveformType.IQ;
                default:
                    throw new ArgumentException($"Unknown waveform type: {scpiValue}");
            }
        }

        /// <summary>
        /// Map SCPI modulation type string to enum
        /// </summary>
        private ModulationType MapToModulationType(string scpiValue)
        {
            switch (scpiValue.ToUpperInvariant())
            {
                case "AM":
                    return ModulationType.AM;
                case "FM":
                    return ModulationType.FM;
                case "PM":
                    return ModulationType.PM;
                case "PWM":
                    return ModulationType.PWM;
                case "FSK":
                    return ModulationType.FSK;
                case "ASK":
                    return ModulationType.ASK;
                case "PSK":
                    return ModulationType.PSK;
                default:
                    throw new ArgumentException($"Unknown modulation type: {scpiValue}");
            }
        }

        /// <summary>
        /// Map SCPI modulation source string to enum
        /// </summary>
        private ModulationSource MapToModulationSource(string scpiValue)
        {
            switch (scpiValue.ToUpperInvariant())
            {
                case "INT":
                case "INTERNAL":
                    return ModulationSource.Internal;
                case "EXT":
                case "EXTERNAL":
                    return ModulationSource.External;
                case "CH1":
                    return ModulationSource.Channel1;
                case "CH2":
                    return ModulationSource.Channel2;
                default:
                    throw new ArgumentException($"Unknown modulation source: {scpiValue}");
            }
        }

        /// <summary>
        /// Map SCPI sweep type string to enum
        /// </summary>
        private SweepType MapToSweepType(string scpiValue)
        {
            switch (scpiValue.ToUpperInvariant())
            {
                case "LINE":
                case "LINEAR":
                    return SweepType.Linear;
                case "LOG":
                case "LOGARITHMIC":
                    return SweepType.Logarithmic;
                default:
                    throw new ArgumentException($"Unknown sweep type: {scpiValue}");
            }
        }

        /// <summary>
        /// Map SCPI sweep direction string to enum
        /// </summary>
        private SweepDirection MapToSweepDirection(string scpiValue)
        {
            switch (scpiValue.ToUpperInvariant())
            {
                case "UP":
                    return SweepDirection.Up;
                case "DOWN":
                    return SweepDirection.Down;
                case "UPDOWN":
                case "UP_DOWN":
                    return SweepDirection.UpDown;
                default:
                    throw new ArgumentException($"Unknown sweep direction: {scpiValue}");
            }
        }

        /// <summary>
        /// Map SCPI trigger source string to enum
        /// </summary>
        private TriggerSource MapToTriggerSource(string scpiValue)
        {
            switch (scpiValue.ToUpperInvariant())
            {
                case "INT":
                case "INTERNAL":
                    return TriggerSource.Internal;
                case "EXT":
                case "EXTERNAL":
                    return TriggerSource.External;
                case "MAN":
                case "MANUAL":
                    return TriggerSource.Manual;
                default:
                    throw new ArgumentException($"Unknown trigger source: {scpiValue}");
            }
        }

        /// <summary>
        /// Map SCPI burst mode string to enum
        /// </summary>
        private BurstMode MapToBurstMode(string scpiValue)
        {
            switch (scpiValue.ToUpperInvariant())
            {
                case "NCYC":
                case "NCYCLE":
                    return BurstMode.NCycle;
                case "GATE":
                case "GATED":
                    return BurstMode.Gated;
                default:
                    throw new ArgumentException($"Unknown burst mode: {scpiValue}");
            }
        }

        /// <summary>
        /// Map SCPI trigger edge string to enum
        /// </summary>
        private TriggerEdge MapToTriggerEdge(string scpiValue)
        {
            switch (scpiValue.ToUpperInvariant())
            {
                case "RISE":
                case "RISING":
                case "POS":
                case "POSITIVE":
                    return TriggerEdge.Rising;
                case "FALL":
                case "FALLING":
                case "NEG":
                case "NEGATIVE":
                    return TriggerEdge.Falling;
                default:
                    throw new ArgumentException($"Unknown trigger edge: {scpiValue}");
            }
        }

        #endregion

        #region Helper Methods - Parsing Utilities

        /// <summary>
        /// Parse SCPI response into key-value pairs
        /// Example: "C1:BSWV WVTP,SINE,FRQ,1000HZ,AMP,5VPP" -> {"WVTP":"SINE", "FRQ":"1000HZ", "AMP":"5VPP"}
        /// </summary>
        private Dictionary<string, string> ParseKeyValuePairs(string response)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Remove channel prefix if present (e.g., "C1:BSWV ")
            int colonIndex = response.IndexOf(':');
            if (colonIndex >= 0)
            {
                int spaceIndex = response.IndexOf(' ', colonIndex);
                if (spaceIndex >= 0)
                    response = response.Substring(spaceIndex + 1);
            }

            // Split by comma
            string[] parts = response.Split(',');

            // Process pairs
            for (int i = 0; i < parts.Length - 1; i += 2)
            {
                string key = parts[i].Trim();
                string value = parts[i + 1].Trim();
                result[key] = value;
            }

            return result;
        }

        #endregion
    }
}
