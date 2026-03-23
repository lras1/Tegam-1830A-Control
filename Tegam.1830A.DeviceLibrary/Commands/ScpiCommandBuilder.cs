using System;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Commands
{
    /// <summary>
    /// Builds SCPI commands for the Tegam 1830A RF/Microwave Power Meter.
    /// </summary>
    public class ScpiCommandBuilder : IScpiCommandBuilder
    {
        /// <summary>
        /// Builds a SCPI command to set the measurement frequency.
        /// </summary>
        public string BuildFrequencyCommand(double frequency, FrequencyUnit unit)
        {
            if (frequency < 0)
                throw new ArgumentException("Frequency must be non-negative.", nameof(frequency));

            string unitString = ConvertFrequencyUnitToString(unit);
            return string.Format("FREQ {0} {1}", frequency, unitString);
        }

        /// <summary>
        /// Builds a SCPI query command to get the current measurement frequency.
        /// </summary>
        public string BuildFrequencyQueryCommand()
        {
            return "FREQ?";
        }

        /// <summary>
        /// Builds a SCPI command to initiate a power measurement.
        /// </summary>
        public string BuildMeasurePowerCommand()
        {
            return "MEAS:POW";
        }

        /// <summary>
        /// Builds a SCPI query command to get the measured power value.
        /// </summary>
        public string BuildMeasurePowerQueryCommand()
        {
            return "MEAS:POW?";
        }

        /// <summary>
        /// Builds a SCPI command to select a specific sensor (1-4).
        /// </summary>
        public string BuildSelectSensorCommand(int sensorId)
        {
            if (sensorId < 1 || sensorId > 4)
                throw new ArgumentException("Sensor ID must be between 1 and 4.", nameof(sensorId));

            return string.Format("SENS:SEL {0}", sensorId);
        }

        /// <summary>
        /// Builds a SCPI query command to get the currently selected sensor.
        /// </summary>
        public string BuildQuerySensorCommand()
        {
            return "SENS:SEL?";
        }

        /// <summary>
        /// Builds a SCPI query command to get all available sensors.
        /// </summary>
        public string BuildQueryAvailableSensorsCommand()
        {
            return "SENS:LIST?";
        }

        /// <summary>
        /// Builds a SCPI command to start calibration.
        /// </summary>
        public string BuildCalibrateCommand(CalibrationMode mode)
        {
            string modeString = mode == CalibrationMode.Internal ? "INT" : "EXT";
            return string.Format("CAL:START {0}", modeString);
        }

        /// <summary>
        /// Builds a SCPI query command to get the calibration status.
        /// </summary>
        public string BuildQueryCalibrationStatusCommand()
        {
            return "CAL:STAT?";
        }

        /// <summary>
        /// Builds a SCPI command to start data logging to a file.
        /// </summary>
        public string BuildStartLoggingCommand(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Filename cannot be null or empty.", nameof(filename));

            return string.Format("LOG:START \"{0}\"", filename);
        }

        /// <summary>
        /// Builds a SCPI command to stop data logging.
        /// </summary>
        public string BuildStopLoggingCommand()
        {
            return "LOG:STOP";
        }

        /// <summary>
        /// Builds a SCPI query command to get the logging status.
        /// </summary>
        public string BuildQueryLoggingStatusCommand()
        {
            return "LOG:STAT?";
        }

        /// <summary>
        /// Builds a SCPI command for system operations (reset, status queries, etc.).
        /// </summary>
        public string BuildSystemCommand(string commandType, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandType))
                throw new ArgumentException("Command type cannot be null or empty.", nameof(commandType));

            switch (commandType.ToUpper())
            {
                case "RESET":
                    return "*RST";
                case "STATUS":
                    return "*STB?";
                case "IDENTITY":
                    return "*IDN?";
                case "CLEAR":
                    return "*CLS";
                case "ERROR":
                    return "SYST:ERR?";
                default:
                    throw new ArgumentException(string.Format("Unknown system command type: {0}", commandType), nameof(commandType));
            }
        }

        /// <summary>
        /// Converts a FrequencyUnit enum value to its SCPI string representation.
        /// </summary>
        private string ConvertFrequencyUnitToString(FrequencyUnit unit)
        {
            switch (unit)
            {
                case FrequencyUnit.Hz:
                    return "HZ";
                case FrequencyUnit.kHz:
                    return "KHZ";
                case FrequencyUnit.MHz:
                    return "MHZ";
                case FrequencyUnit.GHz:
                    return "GHZ";
                default:
                    throw new ArgumentException(string.Format("Unknown frequency unit: {0}", unit), nameof(unit));
            }
        }

        /// <summary>
        /// Converts a PowerUnit enum value to its SCPI string representation.
        /// </summary>
        private string ConvertPowerUnitToString(PowerUnit unit)
        {
            switch (unit)
            {
                case PowerUnit.dBm:
                    return "DBM";
                case PowerUnit.W:
                    return "W";
                case PowerUnit.mW:
                    return "MW";
                default:
                    throw new ArgumentException(string.Format("Unknown power unit: {0}", unit), nameof(unit));
            }
        }

        /// <summary>
        /// Converts a frequency value from one unit to another.
        /// </summary>
        public double ConvertFrequency(double value, FrequencyUnit fromUnit, FrequencyUnit toUnit)
        {
            if (fromUnit == toUnit)
                return value;

            // Convert to Hz first
            double valueInHz = ConvertFrequencyToHz(value, fromUnit);

            // Convert from Hz to target unit
            return ConvertFrequencyFromHz(valueInHz, toUnit);
        }

        /// <summary>
        /// Converts a frequency value to Hz.
        /// </summary>
        private double ConvertFrequencyToHz(double value, FrequencyUnit unit)
        {
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
                    throw new ArgumentException(string.Format("Unknown frequency unit: {0}", unit), nameof(unit));
            }
        }

        /// <summary>
        /// Converts a frequency value from Hz to the specified unit.
        /// </summary>
        private double ConvertFrequencyFromHz(double valueInHz, FrequencyUnit unit)
        {
            switch (unit)
            {
                case FrequencyUnit.Hz:
                    return valueInHz;
                case FrequencyUnit.kHz:
                    return valueInHz / 1000;
                case FrequencyUnit.MHz:
                    return valueInHz / 1000000;
                case FrequencyUnit.GHz:
                    return valueInHz / 1000000000;
                default:
                    throw new ArgumentException(string.Format("Unknown frequency unit: {0}", unit), nameof(unit));
            }
        }

        /// <summary>
        /// Converts a power value from one unit to another.
        /// </summary>
        public double ConvertPower(double value, PowerUnit fromUnit, PowerUnit toUnit)
        {
            if (fromUnit == toUnit)
                return value;

            // Convert to W first
            double valueInWatts = ConvertPowerToWatts(value, fromUnit);

            // Convert from W to target unit
            return ConvertPowerFromWatts(valueInWatts, toUnit);
        }

        /// <summary>
        /// Converts a power value to Watts.
        /// </summary>
        private double ConvertPowerToWatts(double value, PowerUnit unit)
        {
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
                    throw new ArgumentException(string.Format("Unknown power unit: {0}", unit), nameof(unit));
            }
        }

        /// <summary>
        /// Converts a power value from Watts to the specified unit.
        /// </summary>
        private double ConvertPowerFromWatts(double valueInWatts, PowerUnit unit)
        {
            switch (unit)
            {
                case PowerUnit.W:
                    return valueInWatts;
                case PowerUnit.mW:
                    return valueInWatts * 1000;
                case PowerUnit.dBm:
                    // W to dBm: dBm = 10 * log10(W * 1000)
                    return 10 * Math.Log10(valueInWatts * 1000);
                default:
                    throw new ArgumentException(string.Format("Unknown power unit: {0}", unit), nameof(unit));
            }
        }
    }
}
