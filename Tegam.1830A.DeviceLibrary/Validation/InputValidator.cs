using System;
using System.IO;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Validation
{
    /// <summary>
    /// Validates user input against Tegam 1830A device specifications.
    /// </summary>
    public class InputValidator : IInputValidator
    {
        // Tegam 1830A frequency range: 100 kHz to 40 GHz
        private const double MinFrequencyHz = 100_000;      // 100 kHz in Hz
        private const double MaxFrequencyHz = 40_000_000_000; // 40 GHz in Hz

        // Sensor ID range: 1-4
        private const int MinSensorId = 1;
        private const int MaxSensorId = 4;

        /// <summary>
        /// Validates that a frequency value is within the Tegam 1830A range (100 kHz to 40 GHz).
        /// </summary>
        public ValidationResult ValidateFrequency(double frequency, FrequencyUnit unit)
        {
            if (frequency < 0)
            {
                return new ValidationResult(false, "Frequency cannot be negative.");
            }

            // Convert frequency to Hz for comparison
            double frequencyInHz = ConvertFrequencyToHz(frequency, unit);

            if (frequencyInHz < MinFrequencyHz)
            {
                return new ValidationResult(false, 
                    $"Frequency must be at least 100 kHz. Provided: {frequency} {unit}");
            }

            if (frequencyInHz > MaxFrequencyHz)
            {
                return new ValidationResult(false, 
                    $"Frequency must not exceed 40 GHz. Provided: {frequency} {unit}");
            }

            return new ValidationResult(true);
        }

        /// <summary>
        /// Validates that a sensor ID is within the valid range (1-4).
        /// </summary>
        public ValidationResult ValidateSensorId(int sensorId)
        {
            if (sensorId < MinSensorId || sensorId > MaxSensorId)
            {
                return new ValidationResult(false, 
                    $"Sensor ID must be between {MinSensorId} and {MaxSensorId}. Provided: {sensorId}");
            }

            return new ValidationResult(true);
        }

        /// <summary>
        /// Validates that a calibration mode is either Internal or External.
        /// </summary>
        public ValidationResult ValidateCalibrationMode(CalibrationMode mode)
        {
            // CalibrationMode is an enum, so any value is technically valid
            // This method validates that the mode is one of the defined enum values
            if (!Enum.IsDefined(typeof(CalibrationMode), mode))
            {
                return new ValidationResult(false, 
                    $"Calibration mode must be either Internal or External. Provided: {mode}");
            }

            return new ValidationResult(true);
        }

        /// <summary>
        /// Validates that a filename is valid for logging (no invalid path characters).
        /// </summary>
        public ValidationResult ValidateFilename(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return new ValidationResult(false, "Filename cannot be empty or whitespace.");
            }

            // Check for invalid path characters
            char[] invalidChars = Path.GetInvalidPathChars();
            if (filename.IndexOfAny(invalidChars) >= 0)
            {
                return new ValidationResult(false, 
                    "Filename contains invalid path characters.");
            }

            // Check for invalid filename characters
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            if (filename.IndexOfAny(invalidFileNameChars) >= 0)
            {
                return new ValidationResult(false, 
                    "Filename contains invalid filename characters.");
            }

            return new ValidationResult(true);
        }

        /// <summary>
        /// Validates that a measurement count is a positive integer.
        /// </summary>
        public ValidationResult ValidateMeasurementCount(int count)
        {
            if (count <= 0)
            {
                return new ValidationResult(false, 
                    $"Measurement count must be a positive integer. Provided: {count}");
            }

            return new ValidationResult(true);
        }

        /// <summary>
        /// Validates that a measurement delay is a non-negative integer.
        /// </summary>
        public ValidationResult ValidateMeasurementDelay(int delayMs)
        {
            if (delayMs < 0)
            {
                return new ValidationResult(false, 
                    $"Measurement delay cannot be negative. Provided: {delayMs}");
            }

            return new ValidationResult(true);
        }

        /// <summary>
        /// Converts a frequency value from the specified unit to Hz.
        /// </summary>
        private double ConvertFrequencyToHz(double frequency, FrequencyUnit unit)
        {
            switch (unit)
            {
                case FrequencyUnit.Hz:
                    return frequency;
                case FrequencyUnit.kHz:
                    return frequency * 1_000;
                case FrequencyUnit.MHz:
                    return frequency * 1_000_000;
                case FrequencyUnit.GHz:
                    return frequency * 1_000_000_000;
                default:
                    throw new ArgumentException($"Unknown frequency unit: {unit}");
            }
        }
    }
}
