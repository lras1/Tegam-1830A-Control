using System;
using CalibrationTuning.Models;

namespace CalibrationTuning.Events
{
    /// <summary>
    /// Event arguments for tuning state changes.
    /// </summary>
    public class TuningStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The previous tuning state.
        /// </summary>
        public TuningState PreviousState { get; set; }

        /// <summary>
        /// The new tuning state.
        /// </summary>
        public TuningState NewState { get; set; }

        public TuningStateChangedEventArgs(TuningState previousState, TuningState newState)
        {
            PreviousState = previousState;
            NewState = newState;
        }
    }
}
