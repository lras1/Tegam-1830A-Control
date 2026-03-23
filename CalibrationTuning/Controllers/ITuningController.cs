using System;
using System.IO;
using System.Threading.Tasks;
using CalibrationTuning.Events;
using CalibrationTuning.Models;

namespace CalibrationTuning.Controllers
{
    /// <summary>
    /// Primary orchestrator for the tuning process.
    /// Coordinates device services, executes tuning algorithm, and manages state transitions.
    /// </summary>
    public interface ITuningController
    {
        /// <summary>
        /// Raised when the tuning state changes.
        /// </summary>
        event EventHandler<TuningStateChangedEventArgs> StateChanged;

        /// <summary>
        /// Raised when tuning progress is updated (after each measurement).
        /// </summary>
        event EventHandler<TuningProgressEventArgs> ProgressUpdated;

        /// <summary>
        /// Raised when a tuning session completes (converged, timeout, error, or aborted).
        /// </summary>
        event EventHandler<TuningCompletedEventArgs> TuningCompleted;

        /// <summary>
        /// Raised when an error occurs during device operations or tuning.
        /// </summary>
        event EventHandler<ErrorEventArgs> ErrorOccurred;

        /// <summary>
        /// Gets the current tuning state.
        /// </summary>
        TuningState CurrentState { get; }

        /// <summary>
        /// Gets the current tuning parameters.
        /// </summary>
        TuningParameters Parameters { get; }

        /// <summary>
        /// Gets the current tuning statistics.
        /// </summary>
        TuningStatistics Statistics { get; }

        /// <summary>
        /// Connects to both the power meter and signal generator devices.
        /// </summary>
        /// <param name="powerMeterIp">IP address of the power meter.</param>
        /// <param name="signalGenIp">IP address of the signal generator.</param>
        /// <returns>True if both devices connected successfully, false otherwise.</returns>
        Task<bool> ConnectDevicesAsync(string powerMeterIp, string signalGenIp);

        /// <summary>
        /// Disconnects from both devices.
        /// </summary>
        void DisconnectDevices();

        /// <summary>
        /// Starts an automated tuning session with the specified parameters.
        /// </summary>
        /// <param name="parameters">Tuning configuration parameters.</param>
        Task StartTuningAsync(TuningParameters parameters);

        /// <summary>
        /// Stops the current tuning session.
        /// </summary>
        void StopTuning();

        /// <summary>
        /// Performs a single manual power measurement.
        /// </summary>
        /// <returns>The measured power value.</returns>
        Task<PowerMeasurement> MeasureManualAsync();
    }
}
