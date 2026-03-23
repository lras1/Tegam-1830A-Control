using System;

namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Abstract base class for all log entries in the enhanced logging system.
    /// </summary>
    public abstract class LogEntry
    {
        /// <summary>
        /// Gets or sets the timestamp when this log entry was created.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets the type of log entry ("Data" or "Setting").
        /// </summary>
        public abstract string Type { get; }

        /// <summary>
        /// Initializes a new instance of the LogEntry class with current timestamp.
        /// </summary>
        protected LogEntry()
        {
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Converts this log entry to a CSV line format.
        /// </summary>
        /// <returns>CSV formatted string representation of this entry.</returns>
        public abstract string ToCsvLine();

        /// <summary>
        /// Converts this log entry to a display-friendly string.
        /// </summary>
        /// <returns>Display formatted string representation of this entry.</returns>
        public abstract string ToDisplayString();
    }
}
