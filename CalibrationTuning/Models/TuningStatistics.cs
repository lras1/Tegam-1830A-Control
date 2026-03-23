using System;
using System.Collections.Generic;

namespace CalibrationTuning.Models
{
    /// <summary>
    /// Real-time statistics during a tuning session.
    /// </summary>
    public class TuningStatistics
    {
        /// <summary>
        /// Current iteration number.
        /// </summary>
        public int CurrentIteration { get; set; }

        /// <summary>
        /// Current signal voltage.
        /// </summary>
        public double CurrentVoltage { get; set; }

        /// <summary>
        /// Current measured power in dBm.
        /// </summary>
        public double CurrentPowerDbm { get; set; }

        /// <summary>
        /// Power error (Measured - Target) in dB.
        /// </summary>
        public double PowerError { get; set; }

        /// <summary>
        /// Elapsed time since tuning started.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }

        /// <summary>
        /// History of all data points collected during tuning.
        /// </summary>
        public List<TuningDataPoint> History { get; set; }

        public TuningStatistics()
        {
            History = new List<TuningDataPoint>();
        }
    }
}
