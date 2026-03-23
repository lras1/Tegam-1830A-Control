using System;

namespace CalibrationTuning.Models
{
    /// <summary>
    /// Final result of a tuning session.
    /// </summary>
    public class TuningResult
    {
        /// <summary>
        /// Final state when tuning completed.
        /// </summary>
        public TuningState FinalState { get; set; }

        /// <summary>
        /// Total number of iterations performed.
        /// </summary>
        public int TotalIterations { get; set; }

        /// <summary>
        /// Final voltage value.
        /// </summary>
        public double FinalVoltage { get; set; }

        /// <summary>
        /// Final measured power in dBm.
        /// </summary>
        public double FinalPowerDbm { get; set; }

        /// <summary>
        /// Final power error (Measured - Target) in dB.
        /// </summary>
        public double PowerError { get; set; }

        /// <summary>
        /// Total duration of the tuning session.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Error message if tuning failed.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
