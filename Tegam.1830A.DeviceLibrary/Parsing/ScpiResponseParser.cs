using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Parsing
{
    /// <summary>
    /// Parses SCPI responses from the Tegam 1830A RF/Microwave Power Meter.
    /// </summary>
    public class ScpiResponseParser : IScpiResponseParser
    {
        /// <summary>
        /// Parses a boolean response from the device.
        /// </summary>
        public bool ParseBooleanResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty.", nameof(response));

            string trimmed = response.Trim();

            if (trimmed == "1" || trimmed.Equals("ON", StringComparison.OrdinalIgnoreCase) || 
                trimmed.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                return true;

            if (trimmed == "0" || trimmed.Equals("OFF", StringComparison.OrdinalIgnoreCase) || 
                trimmed.Equals("FALSE", StringComparison.OrdinalIgnoreCase))
                return false;

            throw new FormatException(string.Format("Cannot parse boolean response: {0}", response));
        }

        /// <summary>
        /// Parses a numeric response from the device.
        /// </summary>
        public double ParseNumericResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty.", nameof(response));

            string trimmed = response.Trim();
            double result;

            if (!double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                throw new FormatException(string.Format("Cannot parse numeric response: {0}", response));

            return result;
        }

        /// <summary>
        /// Parses a string response from the device.
        /// </summary>
        public string ParseStringResponse(string response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            // Remove surrounding quotes if present
            string trimmed = response.Trim();
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
                return trimmed.Substring(1, trimmed.Length - 2);

            return trimmed;
        }

        /// <summary>
        /// Parses a power measurement response from the device.
        /// Expected format: "+12.34 dBm" or "0.025 W" or "25 mW"
        /// </summary>
        public PowerMeasurement ParsePowerMeasurement(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty.", nameof(response));

            string trimmed = response.Trim();
            
            // Split by space to separate value and unit
            string[] parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                throw new FormatException(string.Format("Invalid power measurement response format: {0}", response));

            double powerValue;
            if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out powerValue))
                throw new FormatException(string.Format("Cannot parse power value: {0}", parts[0]));

            PowerUnit unit = ParsePowerUnit(parts[1]);

            // Create measurement with default frequency and sensor (these should be set by the service)
            return new PowerMeasurement(powerValue, unit, 0, FrequencyUnit.Hz, 0);
        }

        /// <summary>
        /// Parses a frequency response from the device.
        /// Expected format: "2.4 GHZ" or "2400 MHZ" or "2400000 KHZ"
        /// </summary>
        public FrequencyResponse ParseFrequencyResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty.", nameof(response));

            string trimmed = response.Trim();
            
            // Split by space to separate value and unit
            string[] parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                throw new FormatException(string.Format("Invalid frequency response format: {0}", response));

            double frequency;
            if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out frequency))
                throw new FormatException(string.Format("Cannot parse frequency value: {0}", parts[0]));

            FrequencyUnit unit = ParseFrequencyUnit(parts[1]);

            return new FrequencyResponse(frequency, unit);
        }

        /// <summary>
        /// Parses sensor information from the device.
        /// Expected format: "1,Sensor1,100KHZ,40GHZ,-50DBM,+20DBM"
        /// </summary>
        public SensorInfo ParseSensorInfo(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty.", nameof(response));

            string trimmed = response.Trim();
            string[] parts = trimmed.Split(',');

            if (parts.Length < 7)
                throw new FormatException(string.Format("Invalid sensor info response format: {0}", response));

            int sensorId;
            if (!int.TryParse(parts[0].Trim(), out sensorId))
                throw new FormatException(string.Format("Cannot parse sensor ID: {0}", parts[0]));

            string name = parts[1].Trim();

            // Parse min frequency
            string minFreqStr = parts[2].Trim();
            double minFreq = ParseFrequencyValue(minFreqStr);

            // Parse max frequency
            string maxFreqStr = parts[3].Trim();
            double maxFreq = ParseFrequencyValue(maxFreqStr);

            // Parse min power
            string minPowerStr = parts[4].Trim();
            double minPower = ParsePowerValue(minPowerStr);

            // Parse max power
            string maxPowerStr = parts[5].Trim();
            double maxPower = ParsePowerValue(maxPowerStr);

            return new SensorInfo(sensorId, name, minFreq, maxFreq, minPower, maxPower);
        }

        /// <summary>
        /// Parses a list of available sensors from the device.
        /// Expected format: "1,Sensor1,100KHZ,40GHZ,-50DBM,+20DBM;2,Sensor2,100KHZ,40GHZ,-50DBM,+20DBM"
        /// </summary>
        public List<SensorInfo> ParseAvailableSensors(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty.", nameof(response));

            var sensors = new List<SensorInfo>();
            string trimmed = response.Trim();
            
            // Split by semicolon to get individual sensor entries
            string[] sensorEntries = trimmed.Split(';');

            foreach (string entry in sensorEntries)
            {
                if (!string.IsNullOrWhiteSpace(entry))
                {
                    sensors.Add(ParseSensorInfo(entry));
                }
            }

            return sensors;
        }

        /// <summary>
        /// Parses calibration status from the device.
        /// Expected format: "0,1,1,OK" (isCalibrating, isComplete, isSuccessful, errorMessage)
        /// </summary>
        public CalibrationStatus ParseCalibrationStatus(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty.", nameof(response));

            string trimmed = response.Trim();
            string[] parts = trimmed.Split(',');

            if (parts.Length < 3)
                throw new FormatException(string.Format("Invalid calibration status response format: {0}", response));

            bool isCalibrating = ParseBooleanResponse(parts[0].Trim());
            bool isComplete = ParseBooleanResponse(parts[1].Trim());
            bool isSuccessful = ParseBooleanResponse(parts[2].Trim());
            
            string errorMessage = parts.Length > 3 ? parts[3].Trim() : null;

            return new CalibrationStatus(isCalibrating, isComplete, isSuccessful, errorMessage);
        }

        /// <summary>
        /// Parses device identity information from the device.
        /// Expected format: "Tegam,1830A,SN12345,1.0.0"
        /// </summary>
        public DeviceIdentity ParseIdentityResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty.", nameof(response));

            string trimmed = response.Trim();
            string[] parts = trimmed.Split(',');

            if (parts.Length < 4)
                throw new FormatException(string.Format("Invalid identity response format: {0}", response));

            string manufacturer = parts[0].Trim();
            string model = parts[1].Trim();
            string serialNumber = parts[2].Trim();
            string firmwareVersion = parts[3].Trim();

            return new DeviceIdentity(manufacturer, model, serialNumber, firmwareVersion);
        }

        /// <summary>
        /// Parses system status from the device.
        /// Expected format: "1,25.5,0" (isReady, temperature, errorCount)
        /// </summary>
        public SystemStatus ParseSystemStatus(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty.", nameof(response));

            string trimmed = response.Trim();
            string[] parts = trimmed.Split(',');

            if (parts.Length < 3)
                throw new FormatException(string.Format("Invalid system status response format: {0}", response));

            bool isReady = ParseBooleanResponse(parts[0].Trim());
            
            double temperature;
            if (!double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out temperature))
                throw new FormatException(string.Format("Cannot parse temperature: {0}", parts[1]));

            int errorCount;
            if (!int.TryParse(parts[2].Trim(), out errorCount))
                throw new FormatException(string.Format("Cannot parse error count: {0}", parts[2]));

            return new SystemStatus(isReady, temperature, errorCount);
        }

        /// <summary>
        /// Parses an error response from the device.
        /// Expected format: "100,Device not ready" or just "100"
        /// </summary>
        public DeviceError ParseErrorResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty.", nameof(response));

            string trimmed = response.Trim();
            string[] parts = trimmed.Split(new[] { ',' }, 2);

            int errorCode;
            if (!int.TryParse(parts[0].Trim(), out errorCode))
                throw new FormatException(string.Format("Cannot parse error code: {0}", parts[0]));

            string errorMessage = parts.Length > 1 ? parts[1].Trim() : string.Empty;

            return new DeviceError(errorCode, errorMessage);
        }

        /// <summary>
        /// Parses a power unit string and returns the corresponding PowerUnit enum value.
        /// </summary>
        private PowerUnit ParsePowerUnit(string unitString)
        {
            if (string.IsNullOrWhiteSpace(unitString))
                throw new ArgumentException("Unit string cannot be null or empty.", nameof(unitString));

            string upper = unitString.Trim().ToUpperInvariant();

            switch (upper)
            {
                case "DBM":
                    return PowerUnit.dBm;
                case "W":
                    return PowerUnit.W;
                case "MW":
                    return PowerUnit.mW;
                default:
                    throw new FormatException(string.Format("Unknown power unit: {0}", unitString));
            }
        }

        /// <summary>
        /// Parses a frequency unit string and returns the corresponding FrequencyUnit enum value.
        /// </summary>
        private FrequencyUnit ParseFrequencyUnit(string unitString)
        {
            if (string.IsNullOrWhiteSpace(unitString))
                throw new ArgumentException("Unit string cannot be null or empty.", nameof(unitString));

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
                    throw new FormatException(string.Format("Unknown frequency unit: {0}", unitString));
            }
        }

        /// <summary>
        /// Parses a frequency value string (e.g., "100KHZ", "2.4GHZ") and returns the numeric value in Hz.
        /// </summary>
        private double ParseFrequencyValue(string valueString)
        {
            if (string.IsNullOrWhiteSpace(valueString))
                throw new ArgumentException("Value string cannot be null or empty.", nameof(valueString));

            string trimmed = valueString.Trim();
            
            // Extract numeric part and unit part
            int unitStartIndex = 0;
            for (int i = trimmed.Length - 1; i >= 0; i--)
            {
                if (!char.IsLetter(trimmed[i]))
                {
                    unitStartIndex = i + 1;
                    break;
                }
            }

            if (unitStartIndex == 0)
                throw new FormatException(string.Format("Invalid frequency value format: {0}", valueString));

            string numericPart = trimmed.Substring(0, unitStartIndex).Trim();
            string unitPart = trimmed.Substring(unitStartIndex).Trim();

            double value;
            if (!double.TryParse(numericPart, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                throw new FormatException(string.Format("Cannot parse frequency numeric value: {0}", numericPart));

            FrequencyUnit unit = ParseFrequencyUnit(unitPart);

            // Convert to Hz
            switch (unit)
            {
                case FrequencyUnit.Hz:
                    return value;
                case FrequencyUnit.kHz:
                    return value * 1000;
                case FrequencyUnit.MHz:
                    return value * 1000000;
                case FrequencyUnit.GHz:
                    return value * 1000000000;
                default:
                    throw new FormatException(string.Format("Unknown frequency unit: {0}", unit));
            }
        }

        /// <summary>
        /// Parses a power value string (e.g., "-50DBM", "+20DBM", "0.025W") and returns the numeric value in Watts.
        /// </summary>
        private double ParsePowerValue(string valueString)
        {
            if (string.IsNullOrWhiteSpace(valueString))
                throw new ArgumentException("Value string cannot be null or empty.", nameof(valueString));

            string trimmed = valueString.Trim();
            
            // Extract numeric part and unit part
            int unitStartIndex = 0;
            for (int i = trimmed.Length - 1; i >= 0; i--)
            {
                if (!char.IsLetter(trimmed[i]))
                {
                    unitStartIndex = i + 1;
                    break;
                }
            }

            if (unitStartIndex == 0)
                throw new FormatException(string.Format("Invalid power value format: {0}", valueString));

            string numericPart = trimmed.Substring(0, unitStartIndex).Trim();
            string unitPart = trimmed.Substring(unitStartIndex).Trim();

            double value;
            if (!double.TryParse(numericPart, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                throw new FormatException(string.Format("Cannot parse power numeric value: {0}", numericPart));

            PowerUnit unit = ParsePowerUnit(unitPart);

            // Convert to Watts
            switch (unit)
            {
                case PowerUnit.W:
                    return value;
                case PowerUnit.mW:
                    return value / 1000;
                case PowerUnit.dBm:
                    // dBm to W: W = 10^(dBm/10) / 1000
                    return Math.Pow(10, value / 10) / 1000;
                default:
                    throw new FormatException(string.Format("Unknown power unit: {0}", unit));
            }
        }
    }
}
