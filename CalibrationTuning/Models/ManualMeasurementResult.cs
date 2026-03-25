using System;

namespace CalibrationTuning.Models
{
    /// <summary>
    /// Represents the result of a manual measurement operation including frequency, voltage, and power.
    /// </summary>
    public class ManualMeasurementResult
    {
        /// <summary>
        /// Indicates if the measurement is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Signal frequency in Hz.
        /// </summary>
        public double FrequencyHz { get; set; }

        /// <summary>
        /// Signal voltage.
        /// </summary>
        public double Voltage { get; set; }

        /// <summary>
        /// Measured power value in dBm.
        /// </summary>
        public double PowerDbm { get; set; }

        /// <summary>
        /// Timestamp when the measurement was taken.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Error message if measurement failed.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
