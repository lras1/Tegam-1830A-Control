using System;
using System.IO;
using CalibrationTuning.Models;
using Newtonsoft.Json;

namespace CalibrationTuning.Controllers
{
    /// <summary>
    /// Handles application settings persistence using JSON files in AppData.
    /// </summary>
    public class ConfigurationController : IConfigurationController
    {
        private readonly string _configDirectory;
        private readonly string _settingsFilePath;

        /// <summary>
        /// Initializes a new instance of the ConfigurationController.
        /// </summary>
        public ConfigurationController()
        {
            // Store configuration in %AppData%\CalibrationTuning
            _configDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CalibrationTuning"
            );

            _settingsFilePath = Path.Combine(_configDirectory, "settings.json");

            // Ensure the configuration directory exists
            EnsureConfigDirectoryExists();
        }

        /// <summary>
        /// Loads the last used tuning parameters from storage.
        /// </summary>
        /// <returns>The last tuning parameters, or default values if none exist.</returns>
        public TuningParameters LoadLastParameters()
        {
            try
            {
                var settings = LoadSettings();
                return settings?.TuningParameters ?? GetDefaultTuningParameters();
            }
            catch (Exception)
            {
                // Return defaults if loading fails
                return GetDefaultTuningParameters();
            }
        }

        /// <summary>
        /// Saves tuning parameters to storage.
        /// </summary>
        /// <param name="parameters">Parameters to save.</param>
        public void SaveParameters(TuningParameters parameters)
        {
            try
            {
                var settings = LoadSettings() ?? new ApplicationSettings();
                settings.TuningParameters = parameters;
                SaveSettings(settings);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to save tuning parameters.", ex);
            }
        }

        /// <summary>
        /// Loads device configuration (IP addresses) from storage.
        /// </summary>
        /// <returns>Device configuration, or default values if none exist.</returns>
        public DeviceConfiguration LoadDeviceConfiguration()
        {
            try
            {
                var settings = LoadSettings();
                return settings?.DeviceConfiguration ?? GetDefaultDeviceConfiguration();
            }
            catch (Exception)
            {
                // Return defaults if loading fails
                return GetDefaultDeviceConfiguration();
            }
        }

        /// <summary>
        /// Saves device configuration to storage.
        /// </summary>
        /// <param name="config">Device configuration to save.</param>
        public void SaveDeviceConfiguration(DeviceConfiguration config)
        {
            try
            {
                var settings = LoadSettings() ?? new ApplicationSettings();
                settings.DeviceConfiguration = config;
                SaveSettings(settings);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to save device configuration.", ex);
            }
        }

        /// <summary>
        /// Loads the last used log file path from storage.
        /// </summary>
        /// <returns>Last log file path, or empty string if none exists.</returns>
        public string LoadLastLogPath()
        {
            try
            {
                var settings = LoadSettings();
                return settings?.LastLogPath ?? string.Empty;
            }
            catch (Exception)
            {
                // Return empty string if loading fails
                return string.Empty;
            }
        }

        /// <summary>
        /// Saves the log file path to storage.
        /// </summary>
        /// <param name="path">Log file path to save.</param>
        public void SaveLastLogPath(string path)
        {
            try
            {
                var settings = LoadSettings() ?? new ApplicationSettings();
                settings.LastLogPath = path;
                SaveSettings(settings);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to save log file path.", ex);
            }
        }

        /// <summary>
        /// Ensures the configuration directory exists.
        /// </summary>
        private void EnsureConfigDirectoryExists()
        {
            if (!Directory.Exists(_configDirectory))
            {
                Directory.CreateDirectory(_configDirectory);
            }
        }

        /// <summary>
        /// Loads application settings from JSON file.
        /// </summary>
        /// <returns>Application settings, or null if file doesn't exist.</returns>
        private ApplicationSettings LoadSettings()
        {
            if (!File.Exists(_settingsFilePath))
            {
                return null;
            }

            string json = File.ReadAllText(_settingsFilePath);
            return JsonConvert.DeserializeObject<ApplicationSettings>(json);
        }

        /// <summary>
        /// Saves application settings to JSON file.
        /// </summary>
        /// <param name="settings">Settings to save.</param>
        private void SaveSettings(ApplicationSettings settings)
        {
            EnsureConfigDirectoryExists();
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(_settingsFilePath, json);
        }

        /// <summary>
        /// Gets default tuning parameters.
        /// </summary>
        /// <returns>Default tuning parameters.</returns>
        private TuningParameters GetDefaultTuningParameters()
        {
            return new TuningParameters
            {
                FrequencyHz = 2400000000, // 2.4 GHz
                InitialVoltage = 0.5,
                TargetPowerDbm = -10.0,
                MaxStdDevDb = 0.5,
                ConfidenceK = 2.0,
                StabilityWindow = 10,
                VoltageStepSize = 0.05,
                MinVoltage = 0.01,
                MaxVoltage = 5.0,
                MaxIterations = 100,
                SensorId = 1
            };
        }

        /// <summary>
        /// Gets default device configuration.
        /// </summary>
        /// <returns>Default device configuration.</returns>
        private DeviceConfiguration GetDefaultDeviceConfiguration()
        {
            return new DeviceConfiguration
            {
                PowerMeterIpAddress = "192.168.1.100",
                SignalGeneratorIpAddress = "192.168.1.101"
            };
        }

        /// <summary>
        /// Internal class to hold all application settings.
        /// </summary>
        private class ApplicationSettings
        {
            public TuningParameters TuningParameters { get; set; }
            public DeviceConfiguration DeviceConfiguration { get; set; }
            public string LastLogPath { get; set; }
        }
    }
}
