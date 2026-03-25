using System;
using System.Collections.Generic;
using CalibrationTuning.Models;

namespace CalibrationTuning.Controllers
{
    /// <summary>
    /// Manages CSV file writing for tuning sessions.
    /// </summary>
    public interface IDataLoggingController
    {
        /// <summary>
        /// Raised when a measurement is successfully logged.
        /// </summary>
        event EventHandler<int> MeasurementLogged;

        /// <summary>
        /// Raised when a logging operation error occurs.
        /// </summary>
        event EventHandler<string> OperationError;

        /// <summary>
        /// Gets whether logging is currently active.
        /// </summary>
        bool IsLogging { get; }

        /// <summary>
        /// Gets the current log file path.
        /// </summary>
        string CurrentLogFile { get; }

        /// <summary>
        /// Starts logging to the specified file path.
        /// </summary>
        /// <param name="filePath">Path to the log file.</param>
        void StartLogging(string filePath);

        /// <summary>
        /// Stops logging and closes the log file.
        /// </summary>
        void StopLogging();

        /// <summary>
        /// Logs a single measurement data point.
        /// </summary>
        /// <param name="dataPoint">The data point to log.</param>
        /// <param name="status">Status indicator (e.g., "Tuning", "Converged", "Manual").</param>
        void LogMeasurement(TuningDataPoint dataPoint, string status);

        /// <summary>
        /// Logs the start of a tuning session with parameters.
        /// </summary>
        /// <param name="parameters">Tuning parameters for the session.</param>
        void LogSessionStart(TuningParameters parameters);

        /// <summary>
        /// Logs the end of a tuning session with final results.
        /// </summary>
        /// <param name="result">Final tuning result.</param>
        void LogSessionEnd(TuningResult result);

        /// <summary>
        /// Logs a user action (Connect, Disconnect, Start Tuning, Stop Tuning, Manual Measure).
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="parameters">Optional parameters associated with the action.</param>
        void LogUserAction(string actionName, Dictionary<string, string> parameters = null);
    }
}
