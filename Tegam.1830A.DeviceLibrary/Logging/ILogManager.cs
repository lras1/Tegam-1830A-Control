using System;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Logging
{
    /// <summary>
    /// Interface for managing log operations.
    /// </summary>
    public interface ILogManager
    {
        /// <summary>
        /// Starts logging to the specified file.
        /// </summary>
        /// <param name="filename">The path to the log file.</param>
        void StartLogging(string filename);

        /// <summary>
        /// Stops logging and closes the log file.
        /// </summary>
        void StopLogging();

        /// <summary>
        /// Logs an entry (queues it for writing).
        /// </summary>
        /// <param name="entry">The log entry to write.</param>
        void LogEntry(LogEntry entry);

        /// <summary>
        /// Gets the current logging state.
        /// </summary>
        LoggingState CurrentState { get; }

        /// <summary>
        /// Gets the total number of entries logged.
        /// </summary>
        int TotalEntryCount { get; }

        /// <summary>
        /// Gets the current log file path.
        /// </summary>
        string CurrentLogFile { get; }

        /// <summary>
        /// Event raised when an entry is logged.
        /// </summary>
        event EventHandler<LogEntryEventArgs> EntryLogged;

        /// <summary>
        /// Event raised when the logging state changes.
        /// </summary>
        event EventHandler<LoggingStateChangedEventArgs> StateChanged;

        /// <summary>
        /// Event raised when a write error occurs.
        /// </summary>
        event EventHandler<WriteErrorEventArgs> WriteError;
    }
}
