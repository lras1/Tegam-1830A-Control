using System;
using CalibrationTuning.Models;

namespace CalibrationTuning.Events
{
    /// <summary>
    /// Event arguments for tuning completion.
    /// </summary>
    public class TuningCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Final tuning result.
        /// </summary>
        public TuningResult Result { get; set; }

        public TuningCompletedEventArgs(TuningResult result)
        {
            Result = result;
        }
    }
}
