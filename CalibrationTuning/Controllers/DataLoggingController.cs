using System;
using System.Globalization;
using System.IO;
using System.Text;
using CalibrationTuning.Models;

namespace CalibrationTuning.Controllers
{
    /// <summary>
    /// Manages CSV file writing for tuning sessions.
    /// </summary>
    public class DataLoggingController : IDataLoggingController
    {
        private StreamWriter _writer;
        private string _currentLogFile;
        private bool _isLogging;
        private double _sessionFrequency;
        private readonly object _lockObject = new object();

        /// <summary>
        /// Raised when a measurement is successfully logged.
        /// </summary>
        public event EventHandler<int> MeasurementLogged;

        /// <summary>
        /// Raised when a logging operation error occurs.
        /// </summary>
        public event EventHandler<string> OperationError;

        /// <summary>
        /// Gets whether logging is currently active.
        /// </summary>
        public bool IsLogging
        {
            get
            {
                lock (_lockObject)
                {
                    return _isLogging;
                }
            }
        }

        /// <summary>
        /// Gets the current log file path.
        /// </summary>
        public string CurrentLogFile
        {
            get
            {
                lock (_lockObject)
                {
                    return _currentLogFile;
                }
            }
        }

        /// <summary>
        /// Starts logging to the specified file path.
        /// </summary>
        /// <param name="filePath">Path to the log file.</param>
        public void StartLogging(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                OnOperationError("Log file path cannot be empty.");
                return;
            }

            lock (_lockObject)
            {
                try
                {
                    // Stop any existing logging
                    if (_isLogging)
                    {
                        StopLoggingInternal();
                    }

                    // Ensure directory exists
                    string directory = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Determine if file exists to decide whether to write header
                    bool fileExists = File.Exists(filePath);

                    // Open file for appending
                    _writer = new StreamWriter(filePath, append: true, Encoding.UTF8);

                    // Write header if new file
                    if (!fileExists)
                    {
                        _writer.WriteLine("Timestamp,Iteration,Frequency_Hz,Voltage,Power_dBm,Status");
                        _writer.Flush();
                    }

                    _currentLogFile = filePath;
                    _isLogging = true;
                }
                catch (Exception ex)
                {
                    OnOperationError($"Failed to start logging: {ex.Message}");
                    _isLogging = false;
                    _currentLogFile = null;
                    _writer?.Dispose();
                    _writer = null;
                }
            }
        }

        /// <summary>
        /// Stops logging and closes the log file.
        /// </summary>
        public void StopLogging()
        {
            lock (_lockObject)
            {
                StopLoggingInternal();
            }
        }

        private void StopLoggingInternal()
        {
            if (_writer != null)
            {
                try
                {
                    _writer.Flush();
                    _writer.Dispose();
                }
                catch (Exception ex)
                {
                    OnOperationError($"Error closing log file: {ex.Message}");
                }
                finally
                {
                    _writer = null;
                }
            }

            _isLogging = false;
            _currentLogFile = null;
        }

        /// <summary>
        /// Logs a single measurement data point.
        /// </summary>
        /// <param name="dataPoint">The data point to log.</param>
        /// <param name="status">Status indicator (e.g., "Tuning", "Converged", "Manual").</param>
        public void LogMeasurement(TuningDataPoint dataPoint, string status)
        {
            if (dataPoint == null)
            {
                return;
            }

            lock (_lockObject)
            {
                if (!_isLogging || _writer == null)
                {
                    return;
                }

                try
                {
                    // Format: Timestamp,Iteration,Frequency_Hz,Voltage,Power_dBm,Status
                    string timestamp = dataPoint.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    // Frequency is stored in session parameters, use 0 as placeholder for now
                    // The frequency will be consistent throughout a session
                    string line = $"{timestamp},{dataPoint.Iteration},{_sessionFrequency:F0},{dataPoint.Voltage:F6},{dataPoint.PowerDbm:F3},{status ?? "Unknown"}";
                    
                    _writer.WriteLine(line);
                    _writer.Flush();

                    OnMeasurementLogged(dataPoint.Iteration);
                }
                catch (Exception ex)
                {
                    OnOperationError($"Failed to log measurement: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Logs the start of a tuning session with parameters.
        /// </summary>
        /// <param name="parameters">Tuning parameters for the session.</param>
        public void LogSessionStart(TuningParameters parameters)
        {
            if (parameters == null)
            {
                return;
            }

            lock (_lockObject)
            {
                if (!_isLogging || _writer == null)
                {
                    return;
                }

                try
                {
                    // Store frequency for use in measurement logging
                    _sessionFrequency = parameters.FrequencyHz;

                    _writer.WriteLine();
                    _writer.WriteLine($"# Session Start: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    _writer.WriteLine($"# Frequency: {parameters.FrequencyHz} Hz");
                    _writer.WriteLine($"# Target Power: {parameters.TargetPowerDbm} dBm");
                    _writer.WriteLine($"# Tolerance: {parameters.ToleranceDb} dB");
                    _writer.WriteLine($"# Initial Voltage: {parameters.InitialVoltage} V");
                    _writer.WriteLine($"# Voltage Step: {parameters.VoltageStepSize} V");
                    _writer.WriteLine($"# Voltage Range: {parameters.MinVoltage} V to {parameters.MaxVoltage} V");
                    _writer.WriteLine($"# Max Iterations: {parameters.MaxIterations}");
                    _writer.WriteLine($"# Sensor ID: {parameters.SensorId}");
                    _writer.Flush();
                }
                catch (Exception ex)
                {
                    OnOperationError($"Failed to log session start: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Logs the end of a tuning session with final results.
        /// </summary>
        /// <param name="result">Final tuning result.</param>
        public void LogSessionEnd(TuningResult result)
        {
            if (result == null)
            {
                return;
            }

            lock (_lockObject)
            {
                if (!_isLogging || _writer == null)
                {
                    return;
                }

                try
                {
                    _writer.WriteLine();
                    _writer.WriteLine($"# Session End: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    _writer.WriteLine($"# Final State: {result.FinalState}");
                    _writer.WriteLine($"# Total Iterations: {result.TotalIterations}");
                    _writer.WriteLine($"# Final Voltage: {result.FinalVoltage} V");
                    _writer.WriteLine($"# Final Power: {result.FinalPowerDbm} dBm");
                    _writer.WriteLine($"# Power Error: {result.PowerError} dB");
                    _writer.WriteLine($"# Duration: {result.Duration}");
                    
                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        _writer.WriteLine($"# Error: {result.ErrorMessage}");
                    }
                    
                    _writer.WriteLine();
                    _writer.Flush();
                }
                catch (Exception ex)
                {
                    OnOperationError($"Failed to log session end: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Raises the MeasurementLogged event.
        /// </summary>
        private void OnMeasurementLogged(int iteration)
        {
            MeasurementLogged?.Invoke(this, iteration);
        }

        /// <summary>
        /// Raises the OperationError event.
        /// </summary>
        private void OnOperationError(string errorMessage)
        {
            OperationError?.Invoke(this, errorMessage);
        }
    }
}
