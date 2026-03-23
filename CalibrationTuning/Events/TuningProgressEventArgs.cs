using System;
using CalibrationTuning.Models;

namespace CalibrationTuning.Events
{
    /// <summary>
    /// Event arguments for tuning progress updates.
    /// </summary>
    public class TuningProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Current tuning statistics.
        /// </summary>
        public TuningStatistics Statistics { get; set; }

        public TuningProgressEventArgs(TuningStatistics statistics)
        {
            Statistics = statistics;
        }
    }
}
