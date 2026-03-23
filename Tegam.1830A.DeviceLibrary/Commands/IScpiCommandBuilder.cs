using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Commands
{
    /// <summary>
    /// Interface for building SCPI commands for the Tegam 1830A RF/Microwave Power Meter.
    /// </summary>
    public interface IScpiCommandBuilder
    {
        // Frequency configuration
        /// <summary>
        /// Builds a SCPI command to set the measurement frequency.
        /// </summary>
        string BuildFrequencyCommand(double frequency, FrequencyUnit unit);

        /// <summary>
        /// Builds a SCPI query command to get the current measurement frequency.
        /// </summary>
        string BuildFrequencyQueryCommand();

        // Power measurement
        /// <summary>
        /// Builds a SCPI command to initiate a power measurement.
        /// </summary>
        string BuildMeasurePowerCommand();

        /// <summary>
        /// Builds a SCPI query command to get the measured power value.
        /// </summary>
        string BuildMeasurePowerQueryCommand();

        // Sensor management
        /// <summary>
        /// Builds a SCPI command to select a specific sensor (1-4).
        /// </summary>
        string BuildSelectSensorCommand(int sensorId);

        /// <summary>
        /// Builds a SCPI query command to get the currently selected sensor.
        /// </summary>
        string BuildQuerySensorCommand();

        /// <summary>
        /// Builds a SCPI query command to get all available sensors.
        /// </summary>
        string BuildQueryAvailableSensorsCommand();

        // Calibration
        /// <summary>
        /// Builds a SCPI command to start calibration.
        /// </summary>
        string BuildCalibrateCommand(CalibrationMode mode);

        /// <summary>
        /// Builds a SCPI query command to get the calibration status.
        /// </summary>
        string BuildQueryCalibrationStatusCommand();

        // Data logging
        /// <summary>
        /// Builds a SCPI command to start data logging to a file.
        /// </summary>
        string BuildStartLoggingCommand(string filename);

        /// <summary>
        /// Builds a SCPI command to stop data logging.
        /// </summary>
        string BuildStopLoggingCommand();

        /// <summary>
        /// Builds a SCPI query command to get the logging status.
        /// </summary>
        string BuildQueryLoggingStatusCommand();

        // System commands
        /// <summary>
        /// Builds a SCPI command for system operations (reset, status queries, etc.).
        /// </summary>
        string BuildSystemCommand(string commandType, params object[] parameters);
    }
}
