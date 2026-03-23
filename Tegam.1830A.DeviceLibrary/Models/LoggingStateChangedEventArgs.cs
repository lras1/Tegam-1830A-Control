using System;

namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Event arguments for logging state change events.
    /// </summary>
    public class LoggingStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the previous logging state.
        /// </summary>
        public LoggingState OldState { get; set; }

        /// <summary>
        /// Gets or sets the new logging state.
        /// </summary>
        public LoggingState NewState { get; set; }

        /// <summary>
        /// Gets or sets the filename associated with the state change.
        /// </summary>
        public string Filename { get; set; }
    }
}
