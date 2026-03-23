namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents the current state of the logging system.
    /// </summary>
    public enum LoggingState
    {
        /// <summary>
        /// Logging has not been started yet.
        /// </summary>
        NotStarted,

        /// <summary>
        /// Logging is currently active and writing to file.
        /// </summary>
        Active,

        /// <summary>
        /// Logging has been stopped.
        /// </summary>
        Stopped
    }
}
