namespace CalibrationTuning.Models
{
    /// <summary>
    /// Represents the current state of the tuning process.
    /// </summary>
    public enum TuningState
    {
        /// <summary>
        /// No tuning active, devices may or may not be connected.
        /// </summary>
        Idle,

        /// <summary>
        /// Establishing connections to both devices.
        /// </summary>
        Connecting,

        /// <summary>
        /// Active tuning loop running.
        /// </summary>
        Tuning,

        /// <summary>
        /// Waiting for power measurement from power meter.
        /// </summary>
        Measuring,

        /// <summary>
        /// Comparing measurement to setpoint, deciding next action.
        /// </summary>
        Evaluating,

        /// <summary>
        /// Target power achieved within tolerance.
        /// </summary>
        Converged,

        /// <summary>
        /// Maximum iterations reached without convergence.
        /// </summary>
        Timeout,

        /// <summary>
        /// Safety limit exceeded or device error.
        /// </summary>
        Error,

        /// <summary>
        /// User manually stopped tuning.
        /// </summary>
        Aborted
    }
}
