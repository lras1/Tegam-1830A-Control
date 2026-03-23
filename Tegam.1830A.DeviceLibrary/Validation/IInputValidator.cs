using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Validation
{
    /// <summary>
    /// Interface for validating user input against Tegam 1830A device specifications.
    /// </summary>
    public interface IInputValidator
    {
        /// <summary>
        /// Validates that a frequency value is within the Tegam 1830A range (100 kHz to 40 GHz).
        /// </summary>
        /// <param name="frequency">The frequency value to validate.</param>
        /// <param name="unit">The frequency unit.</param>
        /// <returns>A ValidationResult indicating whether the frequency is valid.</returns>
        ValidationResult ValidateFrequency(double frequency, FrequencyUnit unit);

        /// <summary>
        /// Validates that a sensor ID is within the valid range (1-4).
        /// </summary>
        /// <param name="sensorId">The sensor ID to validate.</param>
        /// <returns>A ValidationResult indicating whether the sensor ID is valid.</returns>
        ValidationResult ValidateSensorId(int sensorId);

        /// <summary>
        /// Validates that a calibration mode is either Internal or External.
        /// </summary>
        /// <param name="mode">The calibration mode to validate.</param>
        /// <returns>A ValidationResult indicating whether the calibration mode is valid.</returns>
        ValidationResult ValidateCalibrationMode(CalibrationMode mode);

        /// <summary>
        /// Validates that a filename is valid for logging (no invalid path characters).
        /// </summary>
        /// <param name="filename">The filename to validate.</param>
        /// <returns>A ValidationResult indicating whether the filename is valid.</returns>
        ValidationResult ValidateFilename(string filename);

        /// <summary>
        /// Validates that a measurement count is a positive integer.
        /// </summary>
        /// <param name="count">The measurement count to validate.</param>
        /// <returns>A ValidationResult indicating whether the count is valid.</returns>
        ValidationResult ValidateMeasurementCount(int count);

        /// <summary>
        /// Validates that a measurement delay is a non-negative integer.
        /// </summary>
        /// <param name="delayMs">The measurement delay in milliseconds to validate.</param>
        /// <returns>A ValidationResult indicating whether the delay is valid.</returns>
        ValidationResult ValidateMeasurementDelay(int delayMs);
    }
}
