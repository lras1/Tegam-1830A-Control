using System;

namespace CalibrationTuning.Models
{
    /// <summary>
    /// Represents a power measurement result.
    /// </summary>
    public class PowerMeasurement
    {
        /// <summary>
        /// Measured power value in dBm.
        /// </summary>
        public double PowerDbm { get; set; }

        /// <summary>
        /// Timestamp when the measurement was taken.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Indicates if the measurement is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Error message if measurement failed.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
