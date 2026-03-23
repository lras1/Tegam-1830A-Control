using System;
using System.Collections.Generic;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Parsing
{
    /// <summary>
    /// Interface for parsing SCPI responses from the Tegam 1830A device.
    /// </summary>
    public interface IScpiResponseParser
    {
        /// <summary>
        /// Parses a boolean response from the device.
        /// </summary>
        bool ParseBooleanResponse(string response);

        /// <summary>
        /// Parses a numeric response from the device.
        /// </summary>
        double ParseNumericResponse(string response);

        /// <summary>
        /// Parses a string response from the device.
        /// </summary>
        string ParseStringResponse(string response);

        /// <summary>
        /// Parses a power measurement response from the device.
        /// </summary>
        PowerMeasurement ParsePowerMeasurement(string response);

        /// <summary>
        /// Parses a frequency response from the device.
        /// </summary>
        FrequencyResponse ParseFrequencyResponse(string response);

        /// <summary>
        /// Parses sensor information from the device.
        /// </summary>
        SensorInfo ParseSensorInfo(string response);

        /// <summary>
        /// Parses a list of available sensors from the device.
        /// </summary>
        List<SensorInfo> ParseAvailableSensors(string response);

        /// <summary>
        /// Parses calibration status from the device.
        /// </summary>
        CalibrationStatus ParseCalibrationStatus(string response);

        /// <summary>
        /// Parses device identity information from the device.
        /// </summary>
        DeviceIdentity ParseIdentityResponse(string response);

        /// <summary>
        /// Parses system status from the device.
        /// </summary>
        SystemStatus ParseSystemStatus(string response);

        /// <summary>
        /// Parses an error response from the device.
        /// </summary>
        DeviceError ParseErrorResponse(string response);
    }
}
