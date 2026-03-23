using CalibrationTuning.Models;

namespace CalibrationTuning.Controllers
{
    /// <summary>
    /// Handles application settings persistence.
    /// </summary>
    public interface IConfigurationController
    {
        /// <summary>
        /// Loads the last used tuning parameters from storage.
        /// </summary>
        /// <returns>The last tuning parameters, or default values if none exist.</returns>
        TuningParameters LoadLastParameters();

        /// <summary>
        /// Saves tuning parameters to storage.
        /// </summary>
        /// <param name="parameters">Parameters to save.</param>
        void SaveParameters(TuningParameters parameters);

        /// <summary>
        /// Loads device configuration (IP addresses) from storage.
        /// </summary>
        /// <returns>Device configuration, or default values if none exist.</returns>
        DeviceConfiguration LoadDeviceConfiguration();

        /// <summary>
        /// Saves device configuration to storage.
        /// </summary>
        /// <param name="config">Device configuration to save.</param>
        void SaveDeviceConfiguration(DeviceConfiguration config);

        /// <summary>
        /// Loads the last used log file path from storage.
        /// </summary>
        /// <returns>Last log file path, or empty string if none exists.</returns>
        string LoadLastLogPath();

        /// <summary>
        /// Saves the log file path to storage.
        /// </summary>
        /// <param name="path">Log file path to save.</param>
        void SaveLastLogPath(string path);
    }
}
