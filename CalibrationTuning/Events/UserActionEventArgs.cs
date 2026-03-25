using System;
using System.Collections.Generic;

namespace CalibrationTuning.Events
{
    /// <summary>
    /// Event arguments for user actions (Connect, Disconnect, Start Tuning, Stop Tuning, Manual Measure).
    /// </summary>
    public class UserActionEventArgs : EventArgs
    {
        /// <summary>
        /// Name of the action performed.
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Timestamp when the action occurred.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Optional parameters associated with the action.
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        public UserActionEventArgs()
        {
            Parameters = new Dictionary<string, string>();
        }
    }
}
