using System;

namespace CalibrationTuning.Models
{
    /// <summary>
    /// Represents a single measurement data point during tuning.
    /// </summary>
    public class TuningDataPoint
    {
        /// <summary>
        /// Iteration number in the tuning sequence.
        /// </summary>
        public int Iteration { get; set; }

        /// <summary>
        /// Timestamp when the measurement was taken.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Signal voltage at the time of measurement.
        /// </summary>
        public double Voltage { get; set; }

        /// <summary>
        /// Measured power in dBm.
        /// </summary>
        public double PowerDbm { get; set; }
    }
}
