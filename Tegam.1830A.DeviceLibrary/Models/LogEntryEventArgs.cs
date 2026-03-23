using System;

namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Event arguments for log entry events.
    /// </summary>
    public class LogEntryEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the log entry.
        /// </summary>
        public LogEntry Entry { get; set; }

        /// <summary>
        /// Initializes a new instance of the LogEntryEventArgs class.
        /// </summary>
        /// <param name="entry">The log entry.</param>
        public LogEntryEventArgs(LogEntry entry)
        {
            Entry = entry;
        }
    }
}
