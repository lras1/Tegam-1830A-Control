using System;
using System.Collections.Generic;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Services
{
    /// <summary>
    /// Interface for the Power Meter Service that provides high-level device control operations.
    /// </summary>
    public interface IPowerMeterService
    {
        /// <summary>
        /// Connects to the device at the specified IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address of the device</param>
        /// <returns>True if connection was successful, false otherwise</returns>
        bool Connect(string ipAddress);

        /// <summary>
        /// Disconnects from the device.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Gets a value indicating whether the device is currently connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the device identity information.
        /// </summary>
        DeviceIdentity DeviceInfo { get; }

        /// <summary>
        /// Sets the measurement frequency.
        /// </summary>
        /// <param name="frequency">The frequency value</param>
        /// <param name="unit">The frequency unit</param>
        /// <returns>An OperationResult indicating success or failure</returns>
        OperationResult SetFrequency(double frequency, FrequencyUnit unit);

        /// <summary>
        /// Gets the current measurement frequency.
        /// </summary>
        /// <returns>A FrequencyResponse containing the current frequency</returns>
        FrequencyResponse GetFrequency();

        /// <summary>
        /// Measures power at the current frequency.
        /// </summary>
        /// <returns>A PowerMeasurement containing the measurement result</returns>
        PowerMeasurement MeasurePower();

        /// <summary>
        /// Measures power at a specific frequency.
        /// </summary>
        /// <param name="frequency">The frequency value</param>
        /// <param name="unit">The frequency unit</param>
        /// <returns>A PowerMeasurement containing the measurement result</returns>
        PowerMeasurement MeasurePower(double frequency, FrequencyUnit unit);

        /// <summary>
        /// Takes multiple power measurements with a delay between each.
        /// </summary>
        /// <param name="count">The number of measurements to take</param>
        /// <param name="delayMs">The delay in milliseconds between measurements</param>
        /// <returns>A list of PowerMeasurement results</returns>
        List<PowerMeasurement> MeasureMultiple(int count, int delayMs);

        /// <summary>
        /// Selects a measurement sensor.
        /// </summary>
        /// <param name="sensorId">The sensor ID (1-4)</param>
        /// <returns>An OperationResult indicating success or failure</returns>
        OperationResult SelectSensor(int sensorId);

        /// <summary>
        /// Gets information about the currently selected sensor.
        /// </summary>
        /// <returns>A SensorInfo object containing sensor information</returns>
        SensorInfo GetCurrentSensor();

        /// <summary>
        /// Gets a list of all available sensors.
        /// </summary>
        /// <returns>A list of SensorInfo objects</returns>
        List<SensorInfo> GetAvailableSensors();

        /// <summary>
        /// Starts device calibration.
        /// </summary>
        /// <param name="mode">The calibration mode (Internal or External)</param>
        /// <returns>An OperationResult indicating success or failure</returns>
        OperationResult Calibrate(CalibrationMode mode);

        /// <summary>
        /// Gets the current calibration status.
        /// </summary>
        /// <returns>A CalibrationStatus object</returns>
        CalibrationStatus GetCalibrationStatus();

        /// <summary>
        /// Starts data logging to a file.
        /// </summary>
        /// <param name="filename">The filename for the log file</param>
        /// <returns>An OperationResult indicating success or failure</returns>
        OperationResult StartLogging(string filename);

        /// <summary>
        /// Stops data logging.
        /// </summary>
        /// <returns>An OperationResult indicating success or failure</returns>
        OperationResult StopLogging();

        /// <summary>
        /// Checks if data logging is currently active.
        /// </summary>
        /// <returns>True if logging is active, false otherwise</returns>
        bool IsLogging();

        /// <summary>
        /// Gets the system status.
        /// </summary>
        /// <returns>A SystemStatus object</returns>
        SystemStatus GetSystemStatus();

        /// <summary>
        /// Resets the device.
        /// </summary>
        /// <returns>An OperationResult indicating success or failure</returns>
        OperationResult ResetDevice();

        /// <summary>
        /// Gets all pending errors from the device error queue.
        /// </summary>
        /// <returns>A list of DeviceError objects</returns>
        List<DeviceError> GetErrorQueue();

        /// <summary>
        /// Event raised when a measurement is received.
        /// </summary>
        event EventHandler<MeasurementEventArgs> MeasurementReceived;

        /// <summary>
        /// Event raised when a device error occurs.
        /// </summary>
        event EventHandler<DeviceErrorEventArgs> DeviceError;

        /// <summary>
        /// Event raised when the connection state changes.
        /// </summary>
        event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
    }

    /// <summary>
    /// Event arguments for measurement received events.
    /// </summary>
    public class MeasurementEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the measurement data.
        /// </summary>
        public PowerMeasurement Measurement { get; set; }

        /// <summary>
        /// Initializes a new instance of the MeasurementEventArgs class.
        /// </summary>
        public MeasurementEventArgs(PowerMeasurement measurement)
        {
            Measurement = measurement;
        }
    }

    /// <summary>
    /// Event arguments for device error events.
    /// </summary>
    public class DeviceErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the error information.
        /// </summary>
        public DeviceError Error { get; set; }

        /// <summary>
        /// Initializes a new instance of the DeviceErrorEventArgs class.
        /// </summary>
        public DeviceErrorEventArgs(DeviceError error)
        {
            Error = error;
        }
    }

    /// <summary>
    /// Event arguments for connection state changed events.
    /// </summary>
    public class ConnectionStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a value indicating whether the device is connected.
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Gets the error message if connection failed.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the ConnectionStateChangedEventArgs class.
        /// </summary>
        public ConnectionStateChangedEventArgs(bool isConnected, string errorMessage = null)
        {
            IsConnected = isConnected;
            ErrorMessage = errorMessage;
        }
    }
}
